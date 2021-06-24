using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundleBrowser.ExtractAssets
{
    public partial class ArchiveStorageHeader
    {
        public enum NodeFlags : int
        {
            kNodeFlagsDefault = 0,
            /// <summary>
            /// Node is directory.
            /// </summary>
            kNodeDirectory = 0x00000001,
            /// <summary>
            /// Node is deleted (can be useful for patches).
            /// </summary>
            kNodeDeleted = 0x00000002,
            /// <summary>
            /// Node is SerializedFile.
            /// </summary>
            kNodeSerializedFile = 0x00000004,
        }

        /// <summary>
        /// Filesystem node data. (File or directory).
        /// </summary>
        public class Node
        {
            #region [Fields]
            /// <summary>
            /// Position of the node's data in the archive storage.
            /// </summary>
            public long offset;
            /// <summary>
            /// Node (file) size.
            /// </summary>
            public long size;
            /// <summary>
            /// Node flags.
            /// </summary>
            public uint flags;
            /// <summary>
            /// Node path.
            /// </summary>
            public string path;
            #endregion

            #region [API]
            public bool IsDirectory() => (flags & (int)NodeFlags.kNodeDirectory) != 0;
            public bool IsSerializedFile() => (flags & (int)NodeFlags.kNodeSerializedFile) != 0;

            public override string ToString()
            {
                var tempFlagStrs = new List<string>();
                foreach (var item in Enum.GetValues(typeof(NodeFlags)))
                {
                    var tempFlag = (NodeFlags)item;
                    if ((flags & (uint)tempFlag) == 0) continue;
                    tempFlagStrs.Add(tempFlag.ToString());
                }
                if (tempFlagStrs.Count == 0) tempFlagStrs.Add(flags.ToString());

                return $"offset:[{offset}] size:[{size}] flags:[{string.Join(" | ", tempFlagStrs)}] path:[{path}]";
            }
            #endregion
        }
    }
}