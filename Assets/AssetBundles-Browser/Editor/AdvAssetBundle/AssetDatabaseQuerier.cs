using UnityEditor;

namespace AssetBundleBrowser.AdvAssetBundle
{
    public sealed class AssetDatabaseQuerier : IAssetDataQuerier
    {
        #region [IAssetDataQuerier]
        public string[] GetAssetBundleDependencies(string assetBundleName, bool recursive) 
            => AssetDatabase.GetAssetBundleDependencies(assetBundleName, recursive);

        public string GetAssetBundleName(string path)
            => AssetImporter.GetAtPath(path).assetBundleName;

        public string[] GetAssetPathsFromAssetBundle(string assetBundleName)
            => AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);

        public string[] GetDependencies(string[] pathNames, bool recursive)
            => AssetDatabase.GetDependencies(pathNames, recursive);

        public void SetAssetBundleName(string path, string assetBundleName)
            => AssetImporter.GetAtPath(path).SetAssetBundleNameAndVariant(assetBundleName, string.Empty);
        #endregion
    }
}