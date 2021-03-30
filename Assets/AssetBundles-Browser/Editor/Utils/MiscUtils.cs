using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace AssetBundleBrowser
{
    public static class MiscUtils
    {
        #region [Fields]
        private static HashSet<string> ValidateExtension = new HashSet<string> { ".dll", ".cs", ".meta", ".js", ".boo" };
        private static HashSet<string> AtomAssetExtension = new HashSet<string>
        { ".png", "jpg", ".psd", ".tga", ".exr",
          ".txt", ".bytes", "byte",
          ".so", ".a", ".jar", 
          ".java", ".mm", ".cpp", ".c",
        };
        #endregion

        #region [API]
        public static void ClearManifestByPath(string varOutputPath)
        {
            try
            {
                var tempDireInfo = new DirectoryInfo(varOutputPath);
                var tempManifestFiles = tempDireInfo.GetFiles("*.manifest", SearchOption.AllDirectories);
                for (int i = 0; i < tempManifestFiles.Length; ++i)
                {
                    FileInfo tempFile = tempManifestFiles[i];
                    EditorUtility.DisplayProgressBar("ClearManifest", string.Format("Delete {0}", tempFile.Name), i / (float)tempManifestFiles.Length);

                    if (tempFile.IsReadOnly) tempFile.IsReadOnly = false;
                    File.Delete(tempFile.FullName);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
        public static bool ExportBundleJson(string varOutputPath, List<string> varBundleNames)
        {
            if (string.IsNullOrEmpty(varOutputPath)) return false;
            if (null == varBundleNames || varBundleNames.Count == 0) return false;

            //Key = AssetPath,Val = AssetBundleName;
            var tempABAssetsDic = new Dictionary<string, string>(varBundleNames.Count);
            try
            {
                for (int ABi = 0; ABi < varBundleNames.Count; ++ABi)
                {
                    string tempBundleName = varBundleNames[ABi];
                    EditorUtility.DisplayProgressBar("ClearManifest", string.Format("ExportBundle {0}", tempBundleName), ABi / (float)varBundleNames.Count);

                    var tempAssets = AssetDatabase.GetAssetPathsFromAssetBundle(tempBundleName);
                    foreach (string tempAsset in tempAssets)
                    {
                        tempABAssetsDic.Add(tempAsset, tempBundleName);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
            File.WriteAllText(varOutputPath, JsonFx.Json.JsonWriter.Serialize(tempABAssetsDic));

            return true;
        }
        public static bool ValidateAsset(string varABName)
        {
            if (!varABName.StartsWith("Assets/")) return false;

            var tempExt = Path.GetExtension(varABName).ToLower();
            return !ValidateExtension.Contains(tempExt);
        }
        public static bool IsAtomAsset(string pathName)
        {
            if (!pathName.StartsWith("Assets/")) return false;
            
            var tempExt = Path.GetExtension(pathName).ToLower();
            return AtomAssetExtension.Contains(tempExt);
        }
        #endregion

        #region [GUI]
        public static void GUISignWithTimestamp(Rect varPosition, string varTimestamp)
        {
            EditorGUILayout.Space();

            GUIStyle _creditsStyle = new GUIStyle();
            _creditsStyle.fontStyle = FontStyle.Italic;
            _creditsStyle.alignment = TextAnchor.MiddleCenter;
            _creditsStyle.normal.textColor = new Color(0, 0, 0, 0.5f);
            GUI.Label(new Rect(15, varPosition.height - 20, varPosition.width, 20), string.Format("by 354888562@qq.com update:{0}", varTimestamp), _creditsStyle);
        }
        #endregion
    }
}