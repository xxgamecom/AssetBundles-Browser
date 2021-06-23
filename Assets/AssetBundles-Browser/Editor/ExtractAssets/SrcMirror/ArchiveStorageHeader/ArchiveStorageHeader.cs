using System;
using System.IO;
using UnityEngine;
using SevenZip;

namespace AssetBundleBrowser.ExtractAssets
{
    public partial class ArchiveStorageHeader
    {
        #region [Enum]
        public enum ArchiveFlags : int
        {
            kArchiveCompressionTypeMask = (1 << 6) - 1,
            kArchiveBlocksAndDirectoryInfoCombined = 1 << 6,    //< Block is streamed.
            kArchiveBlocksInfoAtTheEnd = 1 << 7,
            kArchiveOldWebPluginCompatibility = 1 << 8,
        }
        #endregion

        #region [Fields]
        public Header HeaderInfo;
        public StorageBlock[] m_BlocksInfo;
        public Node[] m_DirectoryInfo;
        #endregion

        #region [Construct]
        public ArchiveStorageHeader(EndianBinaryReader varStream)
        {
            Parse(varStream);
            ReadBlocksInfoAndDirectory(varStream);
        }
        #endregion

        #region [API]

        #endregion

        #region [Business]
        protected virtual void Parse(EndianBinaryReader varReader)
        {
            HeaderInfo = Header.Parse(varReader);
        }

        private void ReadBlocksInfoAndDirectory(EndianBinaryReader reader)
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
            if (!HeaderInfo.CheckCompressionSupported(out var tempCompressType))
            {
                throw new NotSupportedException();
            }
            switch (tempCompressType)
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
                m_BlocksInfo = new StorageBlock[blocksInfoCount];
                for (int i = 0; i < blocksInfoCount; i++)
                {
                    m_BlocksInfo[i] = new StorageBlock
                    {
                        uncompressedSize = blocksInfoReader.ReadUInt32(),
                        compressedSize = blocksInfoReader.ReadUInt32(),
                        flags = blocksInfoReader.ReadUInt16()
                    };
                }

                var nodesCount = blocksInfoReader.ReadInt32();
                m_DirectoryInfo = new Node[nodesCount];
                for (int i = 0; i < nodesCount; i++)
                {
                    m_DirectoryInfo[i] = new Node
                    {
                        offset = blocksInfoReader.ReadInt64(),
                        size = blocksInfoReader.ReadInt64(),
                        flags = blocksInfoReader.ReadUInt32(),
                        path = blocksInfoReader.ReadStringToNull(),
                    };
                }
            }
        }
        #endregion
    }
}
