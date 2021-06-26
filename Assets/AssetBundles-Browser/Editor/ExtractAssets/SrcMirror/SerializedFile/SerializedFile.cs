using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundleBrowser.ExtractAssets
{
    public partial class SerializedFile
    {
        public SerializedFileHeader Header;

        public void Parse(EndianBinaryReader varStream)
        {
            Header = SerializedFileHeader.Parse(varStream);
            Debug.LogError(Header);
        }
    }
}