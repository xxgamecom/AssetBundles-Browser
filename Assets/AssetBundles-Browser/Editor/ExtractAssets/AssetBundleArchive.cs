using System;
using System.Text;
using UnityEngine;
using System.Linq;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

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

                tempBinaryStream.Seek(item.offset, SeekOrigin.Current);

                var tempSF = new SerializedFile();
                tempSF.Parse(tempBinaryStream);

            }
        }

    }
}