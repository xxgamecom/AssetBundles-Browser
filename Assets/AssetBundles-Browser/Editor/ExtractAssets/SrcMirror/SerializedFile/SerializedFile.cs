using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace AssetBundleBrowser.ExtractAssets
{
    public partial class SerializedFile
    {
        #region [Fields]
        public SerializedFileHeader Header;

        public string UnityVersion;
        public BuildTarget TargetPlatform = BuildTarget.NoTarget;
        public bool EnableTypeTree;
        public List<SerializedType> Types;
        #endregion

        public void Parse(EndianBinaryReader varStream)
        {
            Header = SerializedFileHeader.Parse(varStream);
            varStream.endian = (EndianType)Header.Endianess;

            UnityVersion = varStream.ReadStringToNull();

            var tempBuildTarget = varStream.ReadInt32();
            if (Enum.IsDefined(typeof(BuildTarget), tempBuildTarget))
            {
                TargetPlatform = (BuildTarget)tempBuildTarget;
            }

            EnableTypeTree = varStream.ReadBoolean();

            var tempTypeCount = varStream.ReadInt32();
            Types = new List<SerializedType>(tempTypeCount);
            for (int i = 0; i < tempTypeCount; ++i)
            {
                var tempType = new SerializedType
                {
                    classID = (PersistentTypeID)varStream.ReadInt32(),
                    IsStrippedType = varStream.ReadBoolean(),
                    ScriptTypeIndex = varStream.ReadInt16()
                };

                if (tempType.classID == PersistentTypeID.MonoBehaviour)
                {
                    tempType.m_ScriptID = varStream.ReadBytes(16);
                }
                tempType.m_OldTypeHash = varStream.ReadBytes(16);

                if (EnableTypeTree)
                {
                    tempType.mTypeTree = new TypeTree();
                    tempType.mTypeTree.Parse(varStream,Header.Version);
                }

                Types.Add(tempType);
            }

        }

        #region [Override]
        public override string ToString()
        {
            return $"UnityVersion:[{UnityVersion}] TargetPlatform:[{TargetPlatform}] EnableTypeTree:[{EnableTypeTree}] Header:[{Header}]";
        }
        #endregion
    }
}