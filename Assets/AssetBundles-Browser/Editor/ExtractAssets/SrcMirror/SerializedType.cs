using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundleBrowser.ExtractAssets
{
    public class SerializedType
    {
        #region [Fields]
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
        #endregion

        #region [API]
        public void Parse(EndianBinaryReader varStream, bool varEnableTypeTree, SerializedFileFormatVersion varFormat, bool varRefType = false)
        {
            classID = (PersistentTypeID)varStream.ReadInt32();
            IsStrippedType = varStream.ReadBoolean();
            ScriptTypeIndex = varStream.ReadInt16();

            if (classID == PersistentTypeID.MonoBehaviour)
            {
                m_ScriptID = varStream.ReadBytes(16);
            }
            m_OldTypeHash = varStream.ReadBytes(16);

            if (varEnableTypeTree)
            {
                mTypeTree = new TypeTree();
                mTypeTree.Parse(varStream, varFormat);

                if (varFormat >= SerializedFileFormatVersion.kStoresTypeDependencies)
                {
                    if (varRefType)
                    {
                        m_TypeDependencies = varStream.ReadInt32Array();
                    }
                    else
                    {
                        m_KlassName = varStream.ReadStringToNull();
                        m_NameSpace = varStream.ReadStringToNull();
                        m_AsmName = varStream.ReadStringToNull();
                    }
                }
            }
        }
        #endregion

        #region [Override]
        public override string ToString()
        {
            return $"classID:[{classID}] IsStrippedType:[{IsStrippedType}] ScriptTypeIndex:[{ScriptTypeIndex}] mTypeTree:[{mTypeTree}] m_ScriptID:[{m_ScriptID}] " +
                $"m_OldTypeHash:[{string.Join("", m_OldTypeHash)}] m_TypeDependencies:[{string.Join("", m_TypeDependencies)}]" +
                $"m_KlassName:[{m_KlassName}] m_NameSpace:[{m_NameSpace}] m_AsmName:[{m_AsmName}]";
        }
        #endregion
    }
}