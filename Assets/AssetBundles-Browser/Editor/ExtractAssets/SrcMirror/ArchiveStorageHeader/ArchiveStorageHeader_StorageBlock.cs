using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundleBrowser.ExtractAssets
{
    public partial class ArchiveStorageHeader
    {
        public enum StorageBlockFlags : int
        {
            kStorageBlockCompressionTypeMask = (1 << 6) - 1,
            /// <summary>
            /// Block is streamed.
            /// </summary>
            kStorageBlockStreamed = 1 << 6,
        }

        public class StorageBlock
        {
            #region [Fields]
            /// <summary>
            /// Uncompressed size
            /// </summary>
            public uint compressedSize;
            /// <summary>
            /// Compressed block size.
            /// </summary>
            public uint uncompressedSize;
            /// <summary>
            /// StorageBlockFlags
            /// </summary>
            public ushort flags;
            #endregion

            #region [API]
            public Compression.CompressionType GetCompressionType()
            {
                var tempTypeVal = flags & (int)StorageBlockFlags.kStorageBlockCompressionTypeMask;
                if (!Enum.IsDefined(typeof(Compression.CompressionType), tempTypeVal))
                {
                    throw new NotSupportedException();
                }
                return (Compression.CompressionType)tempTypeVal;
            }
            public void SetCompressionType(int varCompression)
            {
                flags = (ushort)((flags & ~(int)StorageBlockFlags.kStorageBlockCompressionTypeMask) | (varCompression & (int)StorageBlockFlags.kStorageBlockCompressionTypeMask));
            }
            public bool IsStreamed() => (flags & (int)StorageBlockFlags.kStorageBlockStreamed) != 0;
            public void SetStreamed(bool v)
            {
                flags = (ushort)((flags & ~(int)StorageBlockFlags.kStorageBlockStreamed) | (v ? (int)StorageBlockFlags.kStorageBlockStreamed : 0));
            }

            public override string ToString()
            {
                var tempFlagStrs = new List<string>();
                foreach (var item in Enum.GetValues(typeof(StorageBlockFlags)))
                {
                    var tempFlag = (StorageBlockFlags)item;
                    if ((flags & (ushort)tempFlag) == 0) continue;
                    tempFlagStrs.Add(tempFlag.ToString());
                }
                if (tempFlagStrs.Count == 0) tempFlagStrs.Add(flags.ToString());

                return $"compressedSize:[{compressedSize}] uncompressedSize:[{uncompressedSize}] flags:[{string.Join(" | ", tempFlagStrs)}]";
            }
            #endregion
        }
    }
}