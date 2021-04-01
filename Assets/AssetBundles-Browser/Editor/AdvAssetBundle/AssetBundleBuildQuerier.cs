using System;
using UnityEditor;
using System.Collections.Generic;

namespace AssetBundleBrowser.AdvAssetBundle
{
    public sealed class AssetBundleBuildQuerier : IAssetDataQuerier
    {
        #region [Fields]
        //Key = ABName;
        private Dictionary<string, AssetBundleBuild> _BuildABMap;
        private string[] _EmptyStrArray = new string[] { };
        #endregion

        #region [Construct]
        public AssetBundleBuildQuerier(IEnumerable<AssetBundleBuild> varABBuild, int varCount)
        {
            _BuildABMap = new Dictionary<string, AssetBundleBuild>(varCount);
            foreach (var item in varABBuild)
            {
                var tempABName = item.assetBundleName;
                if (string.IsNullOrEmpty(tempABName)) continue;
                if (item.assetNames == null || item.assetNames.Length == 0) continue;

                if (_BuildABMap.TryGetValue(tempABName, out var tempBuild))
                {
                    if (tempBuild.Equals(item)) continue;

                    var tempCachingAstStr = string.Join(";", tempBuild.assetNames);
                    var tempNewAstStr = string.Join(";", item.assetNames);
                    throw new Exception($"[Conflict] Same AssetBundleName [{tempABName}] been signed with different assetNames,see deatil below:\n[1]:[{tempCachingAstStr}]\n[2][{tempNewAstStr}]");
                }
                _BuildABMap.Add(tempABName, item);
            }
        }
        #endregion

        #region [IAssetDataQuerier]
        public string[] GetAssetBundleDependencies(string assetBundleName, bool recursive)
        {
            if (!_BuildABMap.TryGetValue(assetBundleName, out var tempBuild)) return _EmptyStrArray;

            var tempDepBundles = new HashSet<string>();
            var tempAstNames = this.GetDependencies(tempBuild.assetNames, recursive);
            foreach (var tempAst in tempAstNames)
            {
                if (!MiscUtils.ValidateAsset(tempAst)) continue;

                var tempAstABName = this.GetAssetBundleName(tempAst);
                if (string.IsNullOrEmpty(tempAstABName)) continue;
                if (tempAstABName == assetBundleName) continue;

                tempDepBundles.Add(tempAstABName);
            }

            if (tempDepBundles.Count == 0) return _EmptyStrArray;

            var tempArray = new string[tempDepBundles.Count];
            tempDepBundles.CopyTo(tempArray);
            return tempArray;
        }

        public string GetAssetBundleName(string path)
            => AssetImporter.GetAtPath(path).assetBundleName;

        public string[] GetAssetPathsFromAssetBundle(string assetBundleName)
        {
            if (_BuildABMap.TryGetValue(assetBundleName, out var tempBuild))
            {
                return tempBuild.assetNames;
            }
            return _EmptyStrArray;
        }

        public string[] GetDependencies(string[] pathNames, bool recursive)
            => AssetDatabase.GetDependencies(pathNames, recursive);

        public void SetAssetBundleName(string path, string assetBundleName)
            => AssetImporter.GetAtPath(path).SetAssetBundleNameAndVariant(assetBundleName, string.Empty);
        #endregion
    }
}