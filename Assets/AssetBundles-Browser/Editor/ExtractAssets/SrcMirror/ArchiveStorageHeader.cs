using System;
using UnityEngine;

namespace AssetBundleBrowser.ExtractAssets
{
    public class ArchiveStorageHeader
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
        public static ArchiveStorageHeader Parse(EndianBinaryReader varReader)
        {
            var tempHead = new ArchiveStorageHeader()
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

        public bool CheckCompressionSupported()
        {
            var tempTypeVal = (int)(this.flags & (int)ArchiveStorageHeader.ArchiveFlags.kArchiveCompressionTypeMask);
            if (!Enum.IsDefined(typeof(Compression.CompressionType), tempTypeVal))
            {
                Debug.LogError($"[{tempTypeVal}] NotIsDefined CompressionType.");
                return false;
            }
            var tempEnumType = (Compression.CompressionType)tempTypeVal;
            Debug.Log($"CheckCompressionSupported:[{tempEnumType}]");
            switch (tempEnumType)
            {
                case Compression.CompressionType.kCompressionNone:
                case Compression.CompressionType.kCompressionLz4:
                case Compression.CompressionType.kCompressionLz4HC:
                    return true;
                default:
                    return false;
            }
        }
        public override string ToString()
        {
            return $"signature:[{signature}]version:[{version}]unityVersion:[{unityVersion}]unityRevision:[{unityRevision}]" +
                $"size:[{size}]compressedBlocksInfoSize:[{compressedBlocksInfoSize}]uncompressedBlocksInfoSize:[{uncompressedBlocksInfoSize}]" +
                $"flags:[{flags}]";
        }
        #endregion
    }
}
