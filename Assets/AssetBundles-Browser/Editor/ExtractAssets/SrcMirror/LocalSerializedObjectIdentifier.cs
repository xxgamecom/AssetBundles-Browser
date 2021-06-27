using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundleBrowser.ExtractAssets
{
    public class LocalSerializedObjectIdentifier
    {
        public int localSerializedFileIndex;
        public long localIdentifierInFile;

        public void Parse(EndianBinaryReader varStream)
        {
            localSerializedFileIndex = varStream.ReadInt32();
            varStream.AlignStream();
            localIdentifierInFile = varStream.ReadInt64();
        }
    }
}