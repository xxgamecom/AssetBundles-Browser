using System;
using UnityEngine;

namespace AssetBundleBrowser.ExtractAssets
{
    public partial class ArchiveStorageHeader
    {
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
                return $"signature:[{signature}]version:[{version}]unityVersion:[{unityVersion}]unityRevision:[{unityRevision}]" +
                    $"size:[{size}]compressedBlocksInfoSize:[{compressedBlocksInfoSize}]uncompressedBlocksInfoSize:[{uncompressedBlocksInfoSize}]" +
                    $"flags:[{flags}]";
            }

            public bool CheckCompressionSupported(out Compression.CompressionType varCompressType)
            {
                varCompressType = Compression.CompressionType.kCompressionCount;
                var tempTypeVal = (int)(flags & (int)ArchiveFlags.kArchiveCompressionTypeMask);
                if (!Enum.IsDefined(typeof(Compression.CompressionType), tempTypeVal))
                {
                    Debug.LogError($"[{tempTypeVal}] NotIsDefined CompressionType.");
                    return false;
                }
                varCompressType = (Compression.CompressionType)tempTypeVal;
                Debug.Log($"CheckCompressionSupported:[{varCompressType}]");
                switch (varCompressType)
                {
                    case Compression.CompressionType.kCompressionNone:
                    case Compression.CompressionType.kCompressionLzma:
                    case Compression.CompressionType.kCompressionLz4:
                    case Compression.CompressionType.kCompressionLz4HC:
                        return true;
                    default:
                        return false;
                }
            }
            #endregion
        }
    }
}

