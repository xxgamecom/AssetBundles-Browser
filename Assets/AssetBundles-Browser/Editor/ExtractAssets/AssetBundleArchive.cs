using System.IO;
using UnityEngine;
using UnityEditor;

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
            }
        }

    }
}