using System;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System.Collections.Generic;

using UObject = UnityEngine.Object;

namespace AssetBundleBrowser.ExtractAssets
{
    public class AssetBundleArchive
    {
        [MenuItem("ExtractAssets/Test")]
        public static void ExtractAssets()
        {
            var tempFilePath = EditorUtility.OpenFilePanel("ExtractAssets", Path.Combine(Application.dataPath, "../"), "*");
            if (string.IsNullOrEmpty(tempFilePath)) return;
            TestABFile(tempFilePath);
        }

        [MenuItem("ExtractAssets/QuickTest")]
        public static void Quick_ExtractAssets()
        {
            TestABFile(Path.Combine(Application.streamingAssetsPath, "quardandtriangle-prefab"));
        }

        private static void TestABFile(string varFilePath)
        {
            var tempStream = File.OpenRead(varFilePath);
            var tempBinaryStream = new EndianBinaryReader(tempStream);
            var tempStorage = new ArchiveStorageHeader(tempBinaryStream);
            Debug.LogError(tempStorage.HeaderInfo);
            Debug.LogError(string.Join(",", tempStorage.BlocksInfo));
            Debug.LogError(string.Join(",", tempStorage.DirectoryInfo));
            foreach (var item in tempStorage.DirectoryInfo)
            {
                if (!item.IsSerializedFile()) continue;

                var tempReader = new EndianBinaryReader(item.Context);
                var tempSF = new SerializedFile();
                tempSF.Parse(tempReader);

                var tempSet = new HashSet<string>();
                var tempObjMap = tempSF.ObjectMap;
                foreach (var tempKvp in tempObjMap)
                {
                    var tempIdx = tempKvp.Value.typeID;
                    var tempType = tempSF.Types[tempIdx];
                    var tempTreeNodes = tempType.mTypeTree.Nodes;

                    var tempCalssSet = new HashSet<string>();
                    foreach (var node in tempTreeNodes)
                    {
                        //if (tempCalssSet.Contains(node.m_Type)) continue;
                        GetClsFileds(node, tempTreeNodes);
                        break;
                    }
                }
            }
        }

        private static void GetClsFileds(TypeTreeNode varBase, List<TypeTreeNode> varNodes)
        {
            var tempIdx = varNodes.FindIndex(0, n => n == varBase) + 1;
            if (tempIdx == 0) return;

            var tempFieldlevel = varBase.m_Level + 1;
            var tempFieldNames = new Dictionary<string, string>();
            for (int i = tempIdx; i < varNodes.Count; ++i)
            {
                var tempNode = varNodes[i];
                if (tempNode.m_Level <= varBase.m_Level) break;

                if (tempNode.m_Level == tempFieldlevel)
                {
                    tempFieldNames.Add(tempNode.m_Name, tempNode.GetNodeCsharpType(varNodes));
                }
            }

            foreach (var tempKvp in tempFieldNames)
            {
                Debug.LogFormat("[{0}-{1}]{2}", varBase.m_Type, tempKvp.Key, tempKvp.Value);
            }

        }


        class GameObject
        {
            public List<UObject> m_component;
            public string m_layer;
            public string m_Name;
            public ushort m_Tag;
            public bool m_Isactive;
        }

    }
}