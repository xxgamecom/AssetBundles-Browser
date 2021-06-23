using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundleBrowser.ExtractAssets
{
    public partial class ArchiveStorageHeader
    {
        public class StorageBlock
        {
            public uint compressedSize;
            public uint uncompressedSize;
            public ushort flags;


            public override string ToString()
            {
                return $"compressedSize:[{compressedSize}]uncompressedSize:[{uncompressedSize}]flags:[{flags}]";
            }
        }
    }
}

