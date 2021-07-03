using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundleBrowser.ExtractAssets
{
    public interface IExtractable<T>
    {
        public void Deserialize(EndianBinaryReader varReader);
        public T Serialize();
    }
}