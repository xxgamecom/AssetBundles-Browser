using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundleBrowser.ExtractAssets
{
    public partial class ArchiveStorageHeader
    {
        public class Node
        {
            public long offset;
            public long size;
            public uint flags;
            public string path;

            public override string ToString()
            {
                return $"offset:[{offset}]size:[{size}]flags:[{flags}]path:[{path}]";
            }
        }
    }
}