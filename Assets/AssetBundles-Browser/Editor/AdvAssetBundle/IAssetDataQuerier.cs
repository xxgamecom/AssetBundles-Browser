
namespace AssetBundleBrowser.AdvAssetBundle
{
    public interface IAssetDataQuerier
    {
        string GetAssetBundleName(string path);
        void SetAssetBundleName(string path, string assetBundleName);
        string[] GetDependencies(string[] pathNames, bool recursive);
        string[] GetAssetPathsFromAssetBundle(string assetBundleName);
        string[] GetAssetBundleDependencies(string assetBundleName, bool recursive);
    }
}