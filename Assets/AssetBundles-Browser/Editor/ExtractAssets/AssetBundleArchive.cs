using System;
using System.Text;
using UnityEngine;
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

            var tempStream = File.OpenRead(tempFilePath);
            var tempBinaryStream = new EndianBinaryReader(tempStream);
            var tempHead = ArchiveStorageHeader.Parse(tempBinaryStream);
            Debug.LogError(tempHead);
            tempHead.CheckCompressionSupported();
        }

        

    }
}