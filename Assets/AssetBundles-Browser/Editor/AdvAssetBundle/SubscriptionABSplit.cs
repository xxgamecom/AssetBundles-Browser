using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace AssetBundleBrowser.AdvAssetBundle
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SubscriptionABSplitAttribute : Attribute
    {
        #region [Construct]
        public SubscriptionABSplitAttribute()
        {

        }
        #endregion

        #region [API]
        public static List<AssetDsc> ABSplitInfo()
        {
            var tempVals = new List<AssetDsc>();
            var tempSubSplitType = typeof(SubscriptionABSplitAttribute);
            var tempReturnType = typeof(List<AssetDsc>);
            var tempAssemblys = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var tempAssembly in tempAssemblys)
            {
                var tempTypes = tempAssembly.GetExportedTypes();
                foreach (var tempType in tempTypes)
                {
                    var tempMethods = tempType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                    foreach (var tempMethod in tempMethods)
                    {
                        if (tempMethod.ReturnType != tempReturnType) continue;
                        if (!tempMethod.IsDefined(tempSubSplitType, false)) continue;

                        var tempTypeVals = tempMethod.Invoke(null, null) as List<AssetDsc>;
                        if (tempTypeVals == null || tempTypeVals.Count == 0) continue;

                        tempVals.AddRange(tempTypeVals);
                    }
                }
            }
            return tempVals;
        }
        #endregion
    }

    public class test
    {
        [SubscriptionABSplit]
        public static void API()
        {

        }

        [SubscriptionABSplit]
        public void API2()
        {

        }

        [SubscriptionABSplit]
        public static List<AssetDsc> API32()
        {
            var temPS = new List<AssetDsc>();
            temPS.Add(new AssetDsc("aaa"));
            temPS.Add(new AssetDsc("aaa"));
            return temPS;
        }

        [MenuItem("Test/issueTest")]
        public static void TestMenu()
        {
            {
                var tempAB = AssetDatabase.GetAllAssetBundleNames();
                var tempBuilList = new List<AssetBundleBuild>();
                foreach (var item in tempAB)
                {
                    var tempS = AssetDatabase.GetAssetPathsFromAssetBundle(item);

                    var tempBuild = new AssetBundleBuild();
                    tempBuild.assetBundleName = item;
                    tempBuild.assetNames = tempS;
                    Debug.LogError($"[{item}] {string.Join(";", tempS)}");
                    tempBuilList.Add(tempBuild);
                }
                var tempManifest = BuildPipeline.BuildAssetBundles("AssetBundles/Android_1", tempBuilList.ToArray(), BuildAssetBundleOptions.None, BuildTarget.Android);
            }

            {
                var buildManifest = BuildPipeline.BuildAssetBundles("AssetBundles/Android", BuildAssetBundleOptions.None, BuildTarget.Android);
            }

            {
                var tempVal = SubscriptionABSplitAttribute.ABSplitInfo();
                Debug.LogError(tempVal.Count);

                var tempSet = new HashSet<AssetDsc>(tempVal);
                Debug.LogError(tempSet.Count);

                var tempS = new AssetDsc("s", "");
                var tempS2 = new AssetDsc("s");
                Debug.LogError(tempS == tempS2);
            }


        }

        [MenuItem("Test/TestABBuild")]
        public static void TestABBuild()
        {
            var tempABs = AssetDatabase.GetAllAssetBundleNames();
            var tempList = new List<AssetBundleBuild>(tempABs.Length);
            foreach (var item in tempABs)
            {
                var tempAsts = AssetDatabase.GetAssetPathsFromAssetBundle(item);

                tempList.Add(new AssetBundleBuild { assetNames = tempAsts, assetBundleName = item });
                Debug.LogWarning(string.Join(";", tempAsts));
            }
            var tempBuilds = tempList.ToArray();
            var tempInfo = RedundanciesOp.Optimize(ref tempBuilds).OptimizedAssetBundleInfo();
            Debug.LogError(tempInfo.Length);
        }


        #region [MeshInfo]
        private static string[] _UVTitles = new string[] { "uv", "uv2", "uv3", "uv4", "uv5", "uv6", "uv7", "uv8" };

        [MenuItem("Test/MeshInfo")]
        public static void GetMeshInfo()
        {
            var tempModels = AssetDatabase.FindAssets("t:Model");
            var tempDic = new Dictionary<string, HashSet<string>>();
            foreach (var tempGUID in tempModels)
            {
                var tempAstPath = AssetDatabase.GUIDToAssetPath(tempGUID);
                var tempObj = AssetDatabase.LoadAssetAtPath<GameObject>(tempAstPath);

                var tempMeshs = tempObj.GetComponentsInChildren<MeshFilter>().Select(mf => mf.sharedMesh);
                tempMeshs.Union(tempObj.GetComponentsInChildren<SkinnedMeshRenderer>().Select(smr => smr.sharedMesh));

                var tempAllChannels = DocumentMeshInfos(tempMeshs);
                tempDic.Add(tempAstPath, tempAllChannels);
                Debug.LogFormat("[{0}_{1}] {2}", tempAllChannels.Count, string.Join(",", tempAllChannels), tempAstPath);
            }
            var tempCSVStr = new List<string>(tempDic.Count + 1);
            tempCSVStr.Add("AssetPath,vertexAttributeCount,Attributes");
            var tempSortDic = tempDic.OrderBy(kvp => kvp.Key);
            foreach (var tempKvp in tempSortDic)
            {
                tempCSVStr.Add(string.Format("{0},{1},{2}", tempKvp.Key, tempKvp.Value.Count, string.Join(" | ", tempKvp.Value)));
            }

            File.WriteAllText(Path.Combine(Application.dataPath, "../MeshInfo.csv"), string.Join("\n", tempCSVStr));
        }

        [MenuItem("Test/SelectionMeshInfo")]
        public static void SelectionMeshInfo()
        {
            var tempObj = Selection.activeObject as GameObject;
            var tempMeshs = tempObj.GetComponentsInChildren<MeshFilter>().Select(mf => mf.sharedMesh);
            var tempChannels = DocumentMeshInfos(tempMeshs);
        }
        [MenuItem("Test/AssetBundleMehsinfo")]
        public static void AssetBundleMehsinfo()
        {
            var tempABPath = EditorUtility.OpenFolderPanel("ABDir", Application.dataPath,string.Empty);
            var tempABfiles = Directory.GetFiles(tempABPath, "*");
            foreach (var item in tempABfiles)
            {
                var tempAB = AssetBundle.LoadFromFile(item);
                if (tempAB == null) continue;

                var tempObjs = tempAB.LoadAllAssets();
                foreach (var tempObj in tempObjs)
                {
                    if (tempObj is Mesh)
                    {
                        Debug.LogErrorFormat("[{0}] = {1}", tempObj.name, string.Join(",", DocumentMeshInfos(tempObj as Mesh)));
                    }
                    else if (tempObj is GameObject)
                    {
                        var tempMeshs = (tempObj as GameObject).GetComponentsInChildren<MeshFilter>().Select(mf => mf.sharedMesh);
                        tempMeshs.Union((tempObj as GameObject).GetComponentsInChildren<SkinnedMeshRenderer>().Select(smr => smr.sharedMesh));

                        foreach (var tempMesh in tempMeshs)
                        {
                            Debug.LogErrorFormat("[{0}] = {1}", tempMesh.name, string.Join(",", DocumentMeshInfos(tempMesh)));
                        }
                    }
                }
                tempAB.Unload(true);
            }
        }

        private static HashSet<string> DocumentMeshInfos(IEnumerable<Mesh> varMeshInstances)
        {
            var tempAllChannels = new HashSet<string>();

            if (varMeshInstances == null || varMeshInstances.Count() == 0) return tempAllChannels;

            foreach (var tempMesh in varMeshInstances)
            {
                var tempChannels = DocumentMeshInfos(tempMesh);
                tempAllChannels.UnionWith(tempChannels);
            }
            return tempAllChannels;
        }
        private static HashSet<string> DocumentMeshInfos(Mesh varMesh)
        {
            var tempChannels = new HashSet<string>();
            if (varMesh == null) return tempChannels;

            varMesh = GameObject.Instantiate(varMesh);

            if (varMesh.vertices.Length != 0) tempChannels.Add("vertices");
            if (varMesh.normals.Length != 0) tempChannels.Add("normals");
            if (varMesh.tangents.Length != 0) tempChannels.Add("tangents");
            if (varMesh.colors.Length != 0) tempChannels.Add("colors");

            var tempList = new List<Vector2>();
            for (int i = 0; i < 8; ++i)
            {
                tempList.Clear();
                varMesh.GetUVs(i, tempList);
                if (tempList.Count != 0) tempChannels.Add(_UVTitles[i]);
            }

            if (tempChannels.Count != varMesh.vertexAttributeCount)
            {
                Debug.LogErrorFormat("[{0}]ChannelsValid({1}) doesn't match vertexAttributeCount({2}).", varMesh.name, tempChannels.Count, varMesh.vertexAttributeCount);
            }
            GameObject.DestroyImmediate(varMesh);
            return tempChannels;
        }
        #endregion

    }
}