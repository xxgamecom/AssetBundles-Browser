using System;
using System.IO;
using UnityEngine;
using SevenZip;
using System.Collections.Generic;
using System.Linq;

namespace AssetBundleBrowser.ExtractAssets
{
    /// <summary>
    /// Header definitions of the ArchiveStorage
    /// </summary>
    public partial class ArchiveStorageHeader
    {
        #region [Enum]
        public enum ArchiveFlags : int
        {
            kArchiveCompressionTypeMask = (1 << 6) - 1,
            /// <summary>
            /// Block is streamed.
            /// </summary>
            kArchiveBlocksAndDirectoryInfoCombined = 1 << 6,
            kArchiveBlocksInfoAtTheEnd = 1 << 7,
            kArchiveOldWebPluginCompatibility = 1 << 8,
        }
        #endregion

        #region [Fields]
        public Header HeaderInfo;
        /// <summary>
        /// Information about compressed data blocks
        /// </summary>
        public List<StorageBlock> BlocksInfo;
        /// <summary>
        /// Files mapping information
        /// </summary>
        public List<Node> DirectoryInfo;
        #endregion

        #region [Construct]
        public ArchiveStorageHeader(EndianBinaryReader varStream)
        {
            HeaderInfo = Header.Parse(varStream);
            ReadBlocksInfoAndDirectory(varStream);
        }
        #endregion

        #region [API]

        #endregion

        #region [Business]

        private long ReadBlocksInfoAndDirectory(EndianBinaryReader reader)
        {
            byte[] blocksInfoBytes;
            if (HeaderInfo.version >= 7)
            {
                reader.AlignStream(16);
            }
            if ((HeaderInfo.flags & (int)ArchiveFlags.kArchiveBlocksInfoAtTheEnd) != 0)
            {
                var position = reader.Position;
                reader.Position = reader.BaseStream.Length - HeaderInfo.compressedBlocksInfoSize;
                blocksInfoBytes = reader.ReadBytes((int)HeaderInfo.compressedBlocksInfoSize);
                reader.Position = position;
            }
            else //0x40 kArchiveBlocksAndDirectoryInfoCombined
            {
                blocksInfoBytes = reader.ReadBytes((int)HeaderInfo.compressedBlocksInfoSize);
            }
            var blocksInfoCompressedStream = new MemoryStream(blocksInfoBytes);
            MemoryStream blocksInfoUncompresseddStream;
            switch (HeaderInfo.GetBlocksInfoCompressionType())
            {
                default: //None
                    {
                        blocksInfoUncompresseddStream = blocksInfoCompressedStream;
                        break;
                    }
                case Compression.CompressionType.kCompressionLzma:
                    {
                        blocksInfoUncompresseddStream = new MemoryStream((int)(HeaderInfo.uncompressedBlocksInfoSize));
                        SevenZipHelper.StreamDecompress(blocksInfoCompressedStream, blocksInfoUncompresseddStream, HeaderInfo.compressedBlocksInfoSize, HeaderInfo.uncompressedBlocksInfoSize);
                        blocksInfoUncompresseddStream.Position = 0;
                        blocksInfoCompressedStream.Close();
                        break;
                    }
                case Compression.CompressionType.kCompressionLz4:
                case Compression.CompressionType.kCompressionLz4HC:
                    {
                        var uncompressedBytes = new byte[HeaderInfo.uncompressedBlocksInfoSize];
                        using (var decoder = new Lz4DecoderStream(blocksInfoCompressedStream))
                        {
                            decoder.Read(uncompressedBytes, 0, uncompressedBytes.Length);
                        }
                        blocksInfoUncompresseddStream = new MemoryStream(uncompressedBytes);
                        break;
                    }
            }
            using (var blocksInfoReader = new EndianBinaryReader(blocksInfoUncompresseddStream))
            {
                var uncompressedDataHash = blocksInfoReader.ReadBytes(16);
                var blocksInfoCount = blocksInfoReader.ReadInt32();
                BlocksInfo = new List<StorageBlock>(blocksInfoCount);
                for (int i = 0; i < blocksInfoCount; i++)
                {
                    var tempBlock = new StorageBlock
                    {
                        uncompressedSize = blocksInfoReader.ReadUInt32(),
                        compressedSize = blocksInfoReader.ReadUInt32(),
                        flags = blocksInfoReader.ReadUInt16()
                    };
                    BlocksInfo.Add(tempBlock);
                }

                var nodesCount = blocksInfoReader.ReadInt32();
                DirectoryInfo = new List<Node>(nodesCount);
                for (int i = 0; i < nodesCount; i++)
                {
                    var tempDirInfo = new Node
                    {
                        offset = blocksInfoReader.ReadInt64(),
                        size = blocksInfoReader.ReadInt64(),
                        flags = blocksInfoReader.ReadUInt32(),
                        path = blocksInfoReader.ReadStringToNull(),
                    };
                    DirectoryInfo.Add(tempDirInfo);
                }
            }

            return reader.Position;
        }
        #endregion
    }
}
