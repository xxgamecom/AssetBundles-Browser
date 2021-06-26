using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundleBrowser.ExtractAssets
{
    public class SerializedType
    {
        public PersistentTypeID classID;
        public bool IsStrippedType;
        public short ScriptTypeIndex = -1;
        public TypeTree mTypeTree;
        /// <summary>
        /// Hash generated from assembly name, namespace, and class name, only available for script.<Hash128>
        /// </summary>
        public byte[] m_ScriptID;
        /// <summary>
        /// Old type tree hash.<Hash128>
        /// </summary>
        public byte[] m_OldTypeHash;
        public int[] m_TypeDependencies;
        public string m_KlassName;
        public string m_NameSpace;
        public string m_AsmName;
    }
}