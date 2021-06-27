using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundleBrowser.ExtractAssets
{
    public partial class SerializedFile
    {
        public class ObjectInfo
        {
            /// <summary>
            /// LocalIdentifierInFile
            /// </summary>
            public long m_PathID;

            /// <summary>
            /// Need objectInfo.byteStart += header.m_DataOffset;;
            /// </summary>
            public long byteStart;
            public uint byteSize;
            /// <summary>
            /// Index of SerializedFile.Types;
            /// </summary>
            public int typeID;



            public void Parse(EndianBinaryReader varStream)
            {
                m_PathID = varStream.ReadInt64();

                byteStart = varStream.ReadInt64();
                byteSize = varStream.ReadUInt32();
                typeID = varStream.ReadInt32();
            }
        }
    }
}