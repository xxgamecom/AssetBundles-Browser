using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace AssetBundleBrowser.ExtractAssets
{
    public partial class SerializedFile
    {
        public enum Alignment : int { kSectionAlignment = 16, kMetadataAllocation = 4 << 10 };

        #region [Fields]
        public SerializedFileHeader Header;

        public string UnityVersion;
        public BuildTarget TargetPlatform = BuildTarget.NoTarget;
        public bool EnableTypeTree;
        public List<SerializedType> Types;
        /// <summary>
        /// k = LocalIdentifierInFileType,v = ObjectInfo
        /// </summary>
        public Dictionary<long, ObjectInfo> ObjectMap;
        public List<LocalSerializedObjectIdentifier> ScriptTypes;
        public List<FileIdentifier> FileIdentifiers;
        public List<SerializedType> RefTypes;
        public string userInformation;
        #endregion

        public void Parse(EndianBinaryReader varStream)
        {
            Header = new SerializedFileHeader();
            Header.Parse(varStream);
            Debug.LogError(Header);

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
                var tempType = new SerializedType();
                tempType.Parse(varStream, EnableTypeTree, Header.Version);
                Types.Add(tempType);
            }

            var tempObjCount = varStream.ReadInt32();
            ObjectMap = new Dictionary<long, ObjectInfo>(tempObjCount);
            for (int i = 0; i < tempObjCount; ++i)
            {
                Debug.Log(Header.Version);
                var tempObjInfo = new ObjectInfo();
                tempObjInfo.Parse(varStream);

                ObjectMap.Add(tempObjInfo.m_PathID, tempObjInfo);
            }

            var tempScriptCount = varStream.ReadInt32();
            ScriptTypes = new List<LocalSerializedObjectIdentifier>(tempScriptCount);
            for (int i = 0; i < tempScriptCount; ++i)
            {
                var tempScriptTyps = new LocalSerializedObjectIdentifier();
                tempScriptTyps.Parse(varStream);
                ScriptTypes.Add(tempScriptTyps);
            }

            var tempExternalsCount = varStream.ReadInt32();
            FileIdentifiers = new List<FileIdentifier>(tempExternalsCount);
            for (int i = 0; i < tempExternalsCount; ++i)
            {
                var tempFileID = new FileIdentifier();
                tempFileID.Parse(varStream);
                FileIdentifiers.Add(tempFileID);
            }

            var tempRefTypesCount = varStream.ReadInt32();
            RefTypes = new List<SerializedType>(tempRefTypesCount);
            for (int i = 0; i < tempRefTypesCount; ++i)
            {
                var tempType = new SerializedType();
                tempType.Parse(varStream, EnableTypeTree, Header.Version, true);
                Types.Add(tempType);
            }

            userInformation = varStream.ReadStringToNull();

            if (varStream.Position < (int)Alignment.kMetadataAllocation)
            {
                varStream.Seek((int)Alignment.kMetadataAllocation - varStream.Position, SeekOrigin.Current);
            }
            else
            {
                varStream.AlignStream((int)Alignment.kSectionAlignment);
            }
        }

        #region [Override]
        public override string ToString()
        {
            return $"UnityVersion:[{UnityVersion}] TargetPlatform:[{TargetPlatform}] EnableTypeTree:[{EnableTypeTree}] Header:[{Header}]" +
                $"Types({Types.Count}):[{string.Join(",", Types)}]";
        }
        #endregion
    }
}