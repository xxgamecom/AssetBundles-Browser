using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using SObject = System.Object;

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

        [MenuItem("Test/DiffTest")]
        public static void DiffTest()
        {
            var tempStrs = new string[] { "111","222"};
            File.WriteAllText("E:/aaa.txt", string.Join("\n", tempStrs));

            //AssetDatabase.StartAssetEditing();
            //var tempModels = AssetDatabase.FindAssets("t:Model");
            //foreach (var tempGUID in tempModels)
            //{
            //    try
            //    {
            //        var tempAstPath = AssetDatabase.GUIDToAssetPath(tempGUID);
            //        var tempIm = AssetImporter.GetAtPath(tempAstPath) as ModelImporter;
            //        if (tempIm == null) continue;

            //        Debug.LogError($"{tempAstPath} ss= {tempIm.defaultClipAnimations.Length}");
            //    }
            //    catch (Exception e) { Debug.LogException(e); }
            //    //break;
            //}
            //AssetDatabase.StopAssetEditing();
            //AssetDatabase.SaveAssets();
            //AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            //var tempPath = "E:/puffer_abdiff-Test/Debug_DynamicDownload/Res/AB";
            //var tempPath_2 = "E:/puffer_abdiff-Test/Debug_DynamicDownload/Res/AB_2";
            //Directory.CreateDirectory(tempPath_2);
            //var tempPatch = "E:/puffer_abdiff-Test/PatchResult/13aeb71d551955fcf58e7d6228165f57";
            //var tempRet = ABDiffPatch.PerformPatch(tempPatch, tempPath, tempPath_2);
            //Debug.LogError(tempRet);

            //var tempError = ABDiffPatch.GetPatchFailedPathList();
            //if (tempError != null && tempError.Length != 0)
            //{
            //    Debug.LogErrorFormat("[LocalResourcesABDiffUpdate] FailedPathList[{0}] - [{1}]", tempError.Length, string.Join(";", tempError));
            //}
            //else
            //{
            //    Debug.LogError("[LocalResourcesABDiffUpdate] Finished without Failed.");
            //}
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
            tempMeshs = tempObj.GetComponentsInChildren<SkinnedMeshRenderer>().Select(smr => smr.sharedMesh);
            tempChannels = DocumentMeshInfos(tempMeshs);
        }
        [MenuItem("Test/AssetBundleMehsinfo")]
        public static void AssetBundleMehsinfo()
        {
            var tempABPath = EditorUtility.OpenFolderPanel("ABDir", Application.dataPath, string.Empty);
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
            if (varMesh.bindposes.Length != 0) tempChannels.Add("bindposes");
            if (varMesh.boneWeights.Length != 0) tempChannels.Add("boneWeights");

            var tempList = new List<Vector2>();
            for (int i = 0; i < 8; ++i)
            {
                tempList.Clear();
                varMesh.GetUVs(i, tempList);
                if (tempList.Count != 0) tempChannels.Add(_UVTitles[i]);
            }

            if (tempChannels.Count != varMesh.vertexAttributeCount)
            {
                Debug.LogWarningFormat("[{0}]ChannelsValid({1}) doesn't match vertexAttributeCount({2}).", varMesh.name, tempChannels.Count, varMesh.vertexAttributeCount);
            }
            GameObject.DestroyImmediate(varMesh);
            return tempChannels;
        }
        #endregion

        [MenuItem("AAAA/AAAAA")]
        private static void IsABDiffGrayPassed()
        {
            var tempOpenID = "00000000000000000000000001DC191A";
            tempOpenID = "3FA0065DF0DC7E58204C4FE478854092";
            uint tempHashBer = 5381;
            foreach (var item in tempOpenID)
            {
                tempHashBer = tempHashBer * 33 + item;
            }
            tempHashBer = tempHashBer ^ (tempHashBer >> 16);

            var tempLocalVal = tempHashBer % 100;
            Debug.LogError(tempLocalVal);
        }

        private static void TestCount()
        {
            var tempPath = "C:/WorkSpace/Unity_proj/Library/metadata";
            var tempDirs = Directory.GetDirectories(tempPath);
            var tempInfo = new List<string>();
            foreach (var item in tempDirs)
            {
                tempInfo.Add($"{Path.GetFileName(item)}    {Directory.GetFiles(item).Length}");
            }
            Debug.Log(string.Join("\n", tempInfo));
        }

        [MenuItem("MeshTest/Hit")]
        public static void MeshDiffInfo()
        {
            TestCount();
            return;

            var tempKeepPath = "E:/MeshDataTest/MeshInfosDump_keep_sort.csv";
            var tempStripPath = "E:/MeshDataTest/MeshInfosDump_strip_sort.csv";
            var tempKeepDic = ReadCsv(tempKeepPath);
            var tempStripDic = ReadCsv(tempStripPath);

            var tempKeepAssets = GetABAssets("E:/MeshDataTest/VisualizeBuildData_strip/CustomBuildReport.txt");
            var tempModelInfos = ReadModelInfo("E:/MeshDataTest/modeInfos.txt");

            var tempDiffFbxUV = new HashSet<string>();
            var tempDiffFbxUV_Count = new HashSet<string>();

            var tempDiffFbxMeta = new HashSet<string>();
            var tempDiffFbxMeta_Count = new HashSet<string>();

            foreach (var tempkvp in tempKeepDic)
            {
                var tempABName = tempkvp.Key;
                if (tempkvp.Value.Count != tempStripDic[tempkvp.Key].Count)
                {
                    Debug.Log(tempkvp.Key);
                }

                for (int i = 0; i < tempkvp.Value.Count; i++)
                {
                    var tempKeep = tempkvp.Value[i];
                    var tempStrip = tempStripDic[tempkvp.Key][i];
                    if (tempKeep.Key != tempStrip.Key)
                    {
                        int.TryParse(tempKeep.Key, out var v1);
                        int.TryParse(tempStrip.Key, out var v2);

                        if (v1 != v2)
                        {
                            Debug.Log(tempkvp.Key);
                            continue;
                        }
                    }

                    if (tempKeep.Value != tempStrip.Value)
                    {
                        if (!tempKeepAssets.ContainsKey(tempABName))
                        {
                            Debug.LogError(tempABName);
                            return;
                        }

                        var tempDiffChannel = DiffChannel(tempKeep.Value, tempStrip.Value);
                        //Debug.Log(string.Join(",", tempDiffChannel));

                        foreach (var item in tempKeepAssets[tempABName])
                        {
                            if (!tempModelInfos.ContainsKey(item.ToLower()))
                            {
                                Debug.Log(item);
                                return;
                            }

                            if (tempModelInfos[item.ToLower()].Contains(tempKeep.Key))
                            {
                                var tempMetaImp = tempDiffChannel.Contains("normals") || tempDiffChannel.Contains("tangents");

                                var tempFBXModifyGuid = $"{item},{tempKeep.Key},{tempKeep.Value} => {tempStrip.Value}";
                                if (tempMetaImp)
                                {
                                    tempDiffFbxMeta.Add(tempFBXModifyGuid);
                                    tempDiffFbxMeta_Count.Add(item);
                                }
                                tempDiffChannel.Remove("normals");
                                tempDiffChannel.Remove("tangents");
                                if (tempDiffChannel.Count != 0)
                                {
                                    tempDiffFbxUV.Add(tempFBXModifyGuid);
                                    tempDiffFbxUV_Count.Add(item);
                                }
                            }
                        }
                        //Debug.Log(string.Join(" | ", tempKeepAssets[tempABName]));
                    }
                }
            }

            File.WriteAllText("E:/MeshDataTest/diff_fbx_uv.txt", tempDiffFbxUV_Count.Count.ToString() + "\n" + string.Join("\n", tempDiffFbxUV.OrderBy(s => s)));
            File.WriteAllText("E:/MeshDataTest/diff_fbx_meta.txt", tempDiffFbxMeta_Count.Count.ToString() + "\n" + string.Join("\n", tempDiffFbxMeta.OrderBy(s => s)));

        }

        private static Dictionary<string, HashSet<string>> ReadModelInfo(string varPath)
        {
            var tempDic = new Dictionary<string, HashSet<string>>();
            var tempModelInfos = File.ReadAllLines(varPath);
            foreach (var item in tempModelInfos)
            {
                var tempInfo = item.Split(',');
                var tempFBX = tempInfo[0];
                var tempMeshNames = tempInfo.Length == 2 ? tempInfo[1] : string.Empty;
                var tempSet = new HashSet<string>();
                var tempMeshs = tempMeshNames.Split('|');
                foreach (var v in tempMeshs) tempSet.Add(v);
                tempDic.Add(tempFBX.ToLower(), tempSet);
            }

            return tempDic;
        }

        [MenuItem("MeshTest/GetModelMeshInfos")]
        public static void GetModelMeshInfos()
        {
            var tempModels = AssetDatabase.FindAssets("t:Model");
            var tempModelInfos = new Dictionary<string, List<string>>();
            foreach (var tempGUID in tempModels)
            {
                var tempSet = new List<string>();
                var tempAstPath = AssetDatabase.GUIDToAssetPath(tempGUID);
                var tempObj = AssetDatabase.LoadAssetAtPath<GameObject>(tempAstPath);

                var tempMeshs = tempObj.GetComponentsInChildren<MeshFilter>().Select(mf => mf.sharedMesh);
                foreach (var item in tempMeshs) tempSet.Add(item.name);

                tempMeshs = tempObj.GetComponentsInChildren<SkinnedMeshRenderer>().Select(smr => smr.sharedMesh);
                foreach (var item in tempMeshs) tempSet.Add(item.name);

                tempModelInfos.Add(tempAstPath, tempSet);
            }

            var tempContext = string.Empty;
            foreach (var tempKvp in tempModelInfos)
            {
                tempContext += string.Format("{0},{1}\n", tempKvp.Key, string.Join("|", tempKvp.Value));
            }

            File.WriteAllText("E:/MeshDataTest/modeInfos.txt", tempContext);
        }

        private static List<string> DiffChannel(string varKeep, string varStrip)
        {
            var tempDiff = new List<string>();
            var tempV1 = new List<string>(varKeep.Split('|'));
            var tempV2 = new List<string>(varStrip.Split('|'));
            tempV1 = tempV1.Select(s => s.Trim()).ToList();
            tempV2 = tempV2.Select(s => s.Trim()).ToList();
            foreach (var item in tempV1)
            {
                if (tempV2.FindIndex(0, s => s == item) == -1)
                {
                    tempDiff.Add(item);
                }
            }
            tempDiff.Sort();
            return tempDiff;
        }

        private static Dictionary<string, List<string>> GetABAssets(string varPath)
        {
            var tempDic = new Dictionary<string, List<string>>();

            var tempContxt = File.ReadAllLines(varPath);
            for (int i = 0; i < tempContxt.Length; i++)
            {
                var tempAssets = new List<string>();
                var tempABName = tempContxt[i];

                int j = i + 1;
                for (; j < tempContxt.Length; j++)
                {
                    if (tempContxt[j] == "----------------------------------------------------------------------------------------------------")
                    {
                        break;
                    }

                    if (tempContxt[j].ToLower().EndsWith(".fbx"))
                    {
                        tempAssets.Add(tempContxt[j]);
                    }
                }
                i = j;
                if (tempAssets.Count != 0)
                {
                    tempDic.Add(tempABName, tempAssets);
                }
            }


            return tempDic;
        }

        private static Dictionary<string, List<KeyValuePair<string, string>>> ReadCsv(string varCsvPath)
        {
            var tempDic = new Dictionary<string, List<KeyValuePair<string, string>>>();
            var tempInfos = File.ReadAllLines(varCsvPath);
            for (int i = 1; i < tempInfos.Length; ++i)
            {
                var item = tempInfos[i];
                var tempSplitInfo = item.Split(',');
                var tempABName = tempSplitInfo[0];
                var tempMeshName = tempSplitInfo[1];
                var tempchanelInfo = tempSplitInfo.Length == 3 ? tempSplitInfo[2] : string.Empty;

                if (tempDic.TryGetValue(tempABName, out var tempMeshDic))
                {
                    tempMeshDic.Add(new KeyValuePair<string, string>(tempMeshName, tempchanelInfo));
                }
                else
                {
                    tempDic.Add(tempABName, new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>(tempMeshName, tempchanelInfo) });
                }
            }
            var tempKeys = tempDic.Keys.ToList();
            tempKeys.Sort();
            var tempSortDic = new Dictionary<string, List<KeyValuePair<string, string>>>();
            foreach (var tempkvp in tempDic)
            {
                tempSortDic.Add(tempkvp.Key, tempkvp.Value);
            }
            return tempSortDic;
        }

        private static void MeshInfosort()
        {
            var tempDic = new Dictionary<string, List<KeyValuePair<string, string>>>();
            var tempCsvPath = "E:/MeshDataTest/MeshInfosDump_strip.csv";
            var tempCsvPath2 = "E:/MeshDataTest/MeshInfosDump_strip_sort.csv";
            tempDic = ReadCsv(tempCsvPath);
            Debug.Log(tempDic.Count);

            var tempCsv = new List<string>();
            tempCsv.Add("Path,MeshName,MeshChannels");
            foreach (var tempKvp in tempDic)
            {
                foreach (var item in tempKvp.Value)
                {
                    tempCsv.Add(string.Format("{0},{1},{2}", tempKvp.Key, item.Key, item.Value));
                }
            }
            File.WriteAllText(tempCsvPath2, string.Join("\n", tempCsv));
        }



        private static Type _ShaderUtilType;
        private static Type ShaderUtilType
        {
            get
            {
                if (null == _ShaderUtilType)
                {
                    _ShaderUtilType = Assembly.Load("UnityEditor").GetType("UnityEditor.ShaderUtil");
                }
                return _ShaderUtilType;
            }
        }
        private static SObject _ShaderUtilInstance;
        private static SObject ShaderUtil
        {
            get
            {
                if (null == _ShaderUtilInstance)
                {
                    _ShaderUtilInstance = ShaderUtilType.GetConstructor(Type.EmptyTypes).Invoke(new SObject[] { });
                }
                return _ShaderUtilInstance;
            }
        }

        private static Dictionary<string, string> GLESMap = new Dictionary<string, string>
        {
            {"POSITION0","vertices" },
            {"NORMAL0","normals" },
            {"TANGENT0","tangents" },
            {"COLOR0","colors" },
            {"TEXCOORD0","uv" },
            {"TEXCOORD1","uv2" },
            {"TEXCOORD2","uv3" },
            {"TEXCOORD3","uv4" },
            {"TEXCOORD4","uv5" },
            {"TEXCOORD5","uv6" },
            {"TEXCOORD6","uv7" },
            {"TEXCOORD7","uv8" },
        };

        [MenuItem("Test/SomeFun")]
        private static void Test()
        {
            var tempFiles = Directory.GetFiles("E:/PrefabCheckRet/dd/", "*.txt");
            Debug.LogError(tempFiles.Length);
        }

        private static Dictionary<string, HashSet<string>> DumpShaderInfo()
        {
            var tempPattern = "in \\w+ \\w+ in_(\\w+);";

            var tempShaderInfo = new Dictionary<string, HashSet<string>>();
            var tempDir = "C:/WorkSpace/Unity_proj/shader_result";
            var tempFiles = Directory.GetFiles(tempDir, "*.shader", SearchOption.AllDirectories);

            foreach (var tempPath in tempFiles)
            {
                var tempShaderName = Path.GetFileNameWithoutExtension(tempPath).Replace("Compiled-", string.Empty).Replace("-", "/");

                var tempSet = new HashSet<string>();
                var tempStr = File.ReadAllText(tempPath);
                var tempMatched = Regex.Matches(tempStr, tempPattern);
                foreach (Match item in tempMatched)
                {
                    var tempVal = GLESMap[item.Groups[1].Value];
                    tempSet.Add(tempVal);
                }

                tempShaderInfo.Add(tempShaderName, tempSet);
            }

            return tempShaderInfo;
        }

        [MenuItem("MeshOp/ModeMeta")]
        public static void ShaderVexChannels()
        {
            var tempShaderInfo = DumpShaderInfo();

            var tempAllModels = File.ReadAllLines("E:/input.txt");
            var tempModelsSet = new HashSet<string>(tempAllModels);
            //K = fbxPath,v=channelInfo;
            var tempStripInfo = new Dictionary<string, HashSet<string>>();

            var tempPrefabs = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/ABPack" });
            foreach (var tempGUid in tempPrefabs)
            {
                var tempPrefabPath = AssetDatabase.GUIDToAssetPath(tempGUid);
                var tempDeps = AssetDatabase.GetDependencies(tempPrefabPath, true);

                var tempModels = new List<string>();
                var tempShaders = new HashSet<string>();
                foreach (var tempAstPath in tempDeps)
                {
                    if (tempModelsSet.Contains(tempAstPath.ToLower()))
                    {
                        tempModels.Add(tempAstPath);
                    }

                    if (tempAstPath.ToLower().EndsWith(".shader"))
                    {
                        var tempShaderStr = File.ReadAllText(tempAstPath);
                        var tempMatched = Regex.Match(tempShaderStr, "Shader\\s+\"(.+)\"");
                        var tempShaderName = tempMatched.Groups[1].Value;
                        if (tempShaderInfo.TryGetValue(tempShaderName, out var tempC))
                        {
                            foreach (var v in tempC) tempShaders.Add(v);
                        }
                        else
                        {
                            Debug.LogError($"tempShaderInfo can't find [{tempShaderName}]");
                        }
                    }
                }

                foreach (var tempM in tempModels)
                {
                    if (tempStripInfo.TryGetValue(tempM, out var tempVal))
                    {
                        foreach (var v in tempShaders) tempVal.Add(v);
                    }
                    else
                    {
                        tempStripInfo.Add(tempM, tempShaders);
                    }
                }
            }

            var tempStripRetStr = string.Empty;
            foreach (var tempKvp in tempStripInfo)
            {
                tempStripRetStr += $"{tempKvp.Key},{string.Join("|", tempKvp.Value)}\n";
            }
            File.WriteAllText("E:/MetaStripRet.txt", tempStripRetStr);
        }


        [MenuItem("MeshOp/Model_UV2")]
        public static void modelUV_Fun2()
        {
            var tempShaderInfo = DumpShaderInfo();

            var tempAllModels = File.ReadAllLines("E:/input_uv.txt");
            var tempModelsSet = new HashSet<string>(tempAllModels);
            //K1 = fbxPath,K2 = Meshname,v=channelInfo;
            var tempStripInfo = new Dictionary<string, Dictionary<string, HashSet<string>>>();

            var tempPrefabs = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/ABPack" });
            foreach (var tempGUid in tempPrefabs)
            {
                var tempPrefabPath = AssetDatabase.GUIDToAssetPath(tempGUid);
                var tempDeps = AssetDatabase.GetDependencies(tempPrefabPath, true);

                var tempModels = new HashSet<string>();
                var tempShaders = new HashSet<string>();
                foreach (var tempAstPath in tempDeps)
                {
                    if (tempModelsSet.Contains(tempAstPath.ToLower()))
                    {
                        tempModels.Add(tempAstPath);
                    }

                    if (tempAstPath.ToLower().EndsWith(".shader"))
                    {
                        var tempShaderStr = File.ReadAllText(tempAstPath);
                        var tempMatched = Regex.Match(tempShaderStr, "Shader\\s+\"(.+)\"");
                        var tempShaderName = tempMatched.Groups[1].Value;
                        if (tempShaderInfo.TryGetValue(tempShaderName, out var tempC))
                        {
                            foreach (var v in tempC) tempShaders.Add(v);
                        }
                        else
                        {
                            Debug.LogWarning($"tempShaderInfo can't find [{tempShaderName}]");
                        }
                    }
                }

                if (tempModels.Count != 0)
                {
                    var tempPrefabObj = AssetDatabase.LoadAssetAtPath<GameObject>(tempPrefabPath);

                    var tempMeshRders = tempPrefabObj.GetComponentsInChildren<MeshRenderer>();
                    foreach (var tempR in tempMeshRders)
                    {
                        var tempShaderNames = tempR.sharedMaterials.Select(m => m?.shader.name);
                        var tempMesh = tempR.gameObject.GetComponent<MeshFilter>().sharedMesh;

                        if (tempMesh == null) continue;

                        var tempMeshName = tempMesh.name;
                        var tempModelPath = AssetDatabase.GetAssetPath(tempMesh);

                        
                        tempStripInfo.TryGetValue(tempModelPath, out var tempChannel);
                        if (tempChannel == null)
                        {
                            tempChannel = new Dictionary<string, HashSet<string>>();
                            tempStripInfo.Add(tempModelPath, tempChannel);
                        }
                        tempChannel.TryGetValue(tempMeshName, out var tempSet);
                        if (tempSet == null)
                        {
                            tempSet = new HashSet<string>();
                            tempChannel.Add(tempMeshName, tempSet);
                        }
                        
                        foreach (var tempShaderName in tempShaderNames)
                        {
                            if (string.IsNullOrEmpty(tempShaderName))
                            {
                                continue;
                            }

                            if (tempShaderInfo.TryGetValue(tempShaderName, out var tempVal))
                            {
                                foreach (var v in tempVal) tempSet.Add(v);
                            }
                            else
                            {
                                Debug.LogError($"modelChannel can't find [{tempShaderName}]");
                            }
                        }
                    }

                    var tempSkinRders = tempPrefabObj.GetComponentsInChildren<SkinnedMeshRenderer>();
                    foreach (var tempR in tempSkinRders)
                    {
                        var tempShaderNames = tempR.sharedMaterials.Select(m => m?.shader.name);
                        var tempMesh = tempR.sharedMesh;

                        if (tempMesh == null) continue;

                        var tempMeshName = tempMesh.name;
                        var tempModelPath = AssetDatabase.GetAssetPath(tempMesh);


                        tempStripInfo.TryGetValue(tempModelPath, out var tempChannel);
                        if (tempChannel == null)
                        {
                            tempChannel = new Dictionary<string, HashSet<string>>();
                            tempStripInfo.Add(tempModelPath, tempChannel);
                        }
                        tempChannel.TryGetValue(tempMeshName, out var tempSet);
                        if (tempSet == null)
                        {
                            tempSet = new HashSet<string>();
                            tempChannel.Add(tempMeshName, tempSet);
                        }

                        foreach (var tempShaderName in tempShaderNames)
                        {
                            if (string.IsNullOrEmpty(tempShaderName))
                            {
                                continue;
                            }

                            if (tempShaderInfo.TryGetValue(tempShaderName, out var tempVal))
                            {
                                foreach (var v in tempVal) tempSet.Add(v);
                            }
                            else
                            {
                                Debug.LogError($"modelChannel can't find [{tempShaderName}]");
                            }
                        }
                    }

                }
            }

            var tempStripRetStr = string.Empty;
            foreach (var tempKvp in tempStripInfo)
            {
                foreach (var temmpMesh in tempKvp.Value)
                {
                    tempStripRetStr += $"{tempKvp.Key},{temmpMesh.Key},{string.Join("|", temmpMesh.Value)}\n";
                }
            }
            File.WriteAllText("E:/MetaStripRet_UV.txt", tempStripRetStr);

        }

        [MenuItem("MeshOp/ReadUVDiff")]
        public static void ReadUVDiff()
        {
            var tempRealLines = File.ReadAllLines("E:/MeshDataTest/diff_fbx_uv.txt");
            var tempFbxSet = new HashSet<string>();
            foreach (var item in tempRealLines)
            {
                tempFbxSet.Add(item.Split(',')[0]);
            }

            var tempModifyList = new List<string>();
            foreach (var tempModel in tempFbxSet)
            {
                var tempOrgModel = AssetDatabase.LoadAssetAtPath<GameObject>(tempModel);
                var tempOrgMeshs = GetObjMeshs(tempOrgModel);
                var tempOMeshDic = new Dictionary<string, Mesh>();
                foreach (var v in tempOrgMeshs)
                {
                    if (tempOMeshDic.ContainsKey(v.name))
                    {
                        Debug.LogError($"Mesh Dup Name[{tempModel}] - {v.name}");
                        break;
                    }
                    tempOMeshDic.Add(v.name, v);
                }


                var tempModifyModel = AssetDatabase.LoadAssetAtPath<GameObject>(Path.Combine("Assets/AAAA_Test", Path.GetFileName(tempModel)));
                var tempModiMeshs = GetObjMeshs(tempModifyModel);
                var tempMoMeshDic = new Dictionary<string, Mesh>();
                foreach (var v in tempModiMeshs)
                {
                    if (tempMoMeshDic.ContainsKey(v.name))
                    {
                        Debug.LogError($"Mesh-Modify Dup Name[{tempModel}] - {v.name}");
                        break;
                    }
                    tempMoMeshDic.Add(v.name, v);
                }

                if (tempOrgMeshs.Count() != tempOrgMeshs.Count())
                {
                    Debug.LogError($"Count Error:[{tempModel}]");
                    continue;
                }

                foreach (var tempKvp in tempOMeshDic)
                {
                    var tempOSet = DocumentMeshInfos(tempKvp.Value);
                    tempOSet.Remove("normals");
                    tempOSet.Remove("tangents");

                    if (!tempMoMeshDic.ContainsKey(tempKvp.Key))
                    {
                        Debug.LogError($"{tempModel} - {tempKvp.Key}");
                        break;
                    }

                    var tempMSet = DocumentMeshInfos(tempMoMeshDic[tempKvp.Key]);
                    tempMSet.Remove("normals");
                    tempMSet.Remove("tangents");

                    if (tempOSet == tempMSet)
                    {
                        continue;
                    }
                    else
                    {
                        tempModifyList.Add(tempModel);
                        break;
                    }

                }
            }

            File.WriteAllText("E:/UVRealMofiy.txx", string.Join("\n", tempModifyList));

        }

        private static HashSet<Mesh> GetObjMeshs(GameObject varObj)
        {
            var tempOrgMeshSet = new HashSet<Mesh>();
            if (varObj == null) return tempOrgMeshSet;

            var tempMeshs = varObj.GetComponentsInChildren<MeshFilter>().Select(mf => mf.sharedMesh);
            foreach (var v in tempMeshs) tempOrgMeshSet.Add(v);
            tempMeshs = varObj.GetComponentsInChildren<SkinnedMeshRenderer>().Select(smr => smr.sharedMesh);
            foreach (var v in tempMeshs) tempOrgMeshSet.Add(v);
            return tempOrgMeshSet;
        }

        [MenuItem("MeshOp/SubMeshSameName")]
        public static void SubMeshSameName()
        {
            var tempPrefabs = File.ReadAllLines("E:/SubMeshCheck.txt");
            var tempError = new List<string>();
            foreach (var item in tempPrefabs)
            {
                var tempObj = AssetDatabase.LoadAssetAtPath<GameObject>(item);
                var tempMeshs = GetObjMeshs(tempObj);
                var tempSet = new HashSet<string>();
                foreach (var m in tempMeshs)
                {
                    if (m == null) continue;

                    if (tempSet.Contains(m.name))
                    {
                        tempError.Add(item);
                        break;
                    }
                    tempSet.Add(m.name);
                }
            }
            File.WriteAllText("E:/SubMeshSameName.txt", string.Join("\n", tempError));
        }

        [MenuItem("MeshOp/PrefabCheck")]
        public static void PrefabCheck()
        {
            var tempFbxs = new HashSet<string>(File.ReadAllLines("E:/QA_Unkown.txt"));
            var tempPrefabs = File.ReadAllLines("E:/prefabCheck.txt");
            var tempHitPrefab = new HashSet<string>();
            foreach (var item in tempPrefabs)
            {
                var tempDeps = AssetDatabase.GetDependencies(item, true);
                foreach (var tempDep in tempDeps)
                {
                    if (tempFbxs.Contains(tempDep.ToLower()))
                    {
                        tempHitPrefab.Add(item);
                        break;
                    }
                }
            }
            File.WriteAllText("E:/prefabCheck_Ret.txt", string.Join("\n", tempHitPrefab));
        }

        [MenuItem("MeshOp/FbxSkinMode")]
        public static void FbxSkinMode()
        {
            var tempPath = "E:/SkinTest.txt";
            var templines = File.ReadAllLines(tempPath);
            var tempRet = new List<string>();
            foreach (var item in templines)
            {
                var tempObj = AssetDatabase.LoadAssetAtPath<GameObject>(item);
                var tempMeshs = GetObjMeshs(tempObj);
                foreach (var m in tempMeshs)
                {
                    if (m.bindposes.Length != 0 || m.boneWeights.Length != 0)
                    {
                        tempRet.Add(item);
                    }
                    break;
                }
            }
            File.WriteAllText("E:/SkinTest_Res.txt", string.Join("\n", tempRet));
        }

        [MenuItem("AAAAAA/TestTimelineAssetLoad")]
        public static void TestTimelineAssetLoad()
        {
            //Debug.LogError("Load heroshow_veigar_skilla03_idle01_in_04");
            //var tempA1 = AssetBundle.LoadFromFile("C:/WorkSpace/Unity_proj/AssetBundles/StandaloneWindows64/heroshow_veigar_skilla03_idle01_in_04");
            Debug.LogError("Load timelineassets");
            var tempA2 = AssetBundle.LoadFromFile("C:/WorkSpace/Unity_proj/AssetBundles/StandaloneWindows64/timelineassets");

            //if (tempA1 != null) tempA1.Unload(true);
            if (tempA2 != null) tempA2.Unload(true);
        }
    }
}