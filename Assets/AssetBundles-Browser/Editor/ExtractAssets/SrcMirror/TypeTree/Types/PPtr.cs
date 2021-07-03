using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundleBrowser.ExtractAssets
{
    public class PPtr<T> : Object, IExtractable<PPtr<T>>
    {
        #region [Fields]
        public int m_FileID;
        public long m_PathID;
        #endregion

        #region [IExtractable]
        public void Deserialize(EndianBinaryReader varReader)
        {
            m_FileID = varReader.ReadInt32();
            m_PathID = varReader.ReadInt64();
        }
        public PPtr<T> Serialize() { return null; }
        #endregion
    }
}