using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundleBrowser.ExtractAssets
{
    public interface IExtractable<T>
    {
        void Deserialize(EndianBinaryReader varReader);
        T Serialize();
    }
}