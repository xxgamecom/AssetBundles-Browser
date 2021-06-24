using System;
using System.Collections.Generic;

namespace AssetBundleBrowser.ExtractAssets
{
    public partial class ArchiveStorageHeader
    {
        /// <summary>
        /// Uncompressed archive file header (~46bytes).
        /// There might have the following layouts:
        /// header [ blocks directory ] [ data ] - Unity3d/UnityArchive
        /// header[data][blocks directory] - Unity3d/UnityArchive
        /// header blocks[directory data] - UnityRaw/UnityWeb - solid compression
        /// Directory lives inside the data blocks(compressed or not)
        /// </summary>
        public class Header
        {
            #region [Fields]
            public string signature;
            public uint version;
            public string unityVersion;
            public string unityRevision;

            public long size;
            public uint compressedBlocksInfoSize;
            public uint uncompressedBlocksInfoSize;
            public uint flags;
            #endregion

            #region [API]
            public static Header Parse(EndianBinaryReader varReader)
            {
                var tempHead = new Header()
                {
                    signature = varReader.ReadStringToNull(),
                    version = varReader.ReadUInt32(),
                    unityVersion = varReader.ReadStringToNull(),
                    unityRevision = varReader.ReadStringToNull(),

                    size = varReader.ReadInt64(),
                    compressedBlocksInfoSize = varReader.ReadUInt32(),
                    uncompressedBlocksInfoSize = varReader.ReadUInt32(),
                    flags = varReader.ReadUInt32(),
                };
                return tempHead;
            }

            public override string ToString()
            {
                var tempFlagStrs = new List<string>();
                foreach (var item in Enum.GetValues(typeof(ArchiveFlags)))
                {
                    var tempFlag = (ArchiveFlags)item;
                    if ((flags & (uint)tempFlag) == 0) continue;
                    tempFlagStrs.Add(tempFlag.ToString());
                }
                if (tempFlagStrs.Count == 0) tempFlagStrs.Add(flags.ToString());

                return $"signature:[{signature}] version:[{version}] unityVersion:[{unityVersion}] unityRevision:[{unityRevision}]" +
                    $" size:[{size}] compressedBlocksInfoSize:[{compressedBlocksInfoSize}] uncompressedBlocksInfoSize:[{uncompressedBlocksInfoSize}]" +
                    $" flags:[{string.Join(" | ", tempFlagStrs)}]";
            }

            public Compression.CompressionType GetBlocksInfoCompressionType()
            {
                var tempTypeVal = (int)(flags & (int)ArchiveFlags.kArchiveCompressionTypeMask);
                if (!Enum.IsDefined(typeof(Compression.CompressionType), tempTypeVal))
                {
                    throw new NotSupportedException($"[{tempTypeVal}] NotIsDefined CompressionType.");
                }
                return (Compression.CompressionType)tempTypeVal;
            }
            public void SetBlocksInfoCompressionType(uint compression)
            {
                flags = (flags & ~(uint)ArchiveFlags.kArchiveCompressionTypeMask) | (compression & (uint)ArchiveFlags.kArchiveCompressionTypeMask);
            }
            public bool HasBlocksAndDirectoryInfoCombined() => (flags & (uint)ArchiveFlags.kArchiveBlocksAndDirectoryInfoCombined) != 0;
            public void SetBlocksAndDirectoryInfoCombined(bool v)
            {
                flags = (flags & ~(uint)ArchiveFlags.kArchiveBlocksAndDirectoryInfoCombined) | (v ? (uint)ArchiveFlags.kArchiveBlocksAndDirectoryInfoCombined : 0);
            }
            public bool HasBlocksInfoAtTheEnd() => (flags & (uint)ArchiveFlags.kArchiveBlocksInfoAtTheEnd) != 0;
            public void SetBlocksInfoAtTheEnd(bool v)
            {
                flags = (flags & ~(uint)ArchiveFlags.kArchiveBlocksInfoAtTheEnd) | (v ? (uint)ArchiveFlags.kArchiveBlocksInfoAtTheEnd : 0);
            }
            #endregion
        }
    }
}