using System;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System.Collections.Generic;

public sealed class GUIDHelper : EditorWindow
{
    #region [Fields]
    private string guidStr = string.Empty;
    private string astPathStr = string.Empty;
    private Vector2 _ScrollPos;
    #endregion

    #region [Menu]
    [MenuItem("Window/AssetBundle Browser/GUIDHelper #g")]
    public static void GUIDHelperWindow()
    {
        EditorWindow.GetWindow<GUIDHelper>();
    }
    #endregion

    #region [GUI]
    private void OnFocus()
    {
        PeekCopyBuffer();
    }
    private void OnGUI()
    {
        EditorGUILayout.Space();
        using (var tempSV = new EditorGUILayout.ScrollViewScope(_ScrollPos))
        {
            _ScrollPos = tempSV.scrollPosition;

            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                GUIItem(ref guidStr, "GUIDToAssetPath", "input GUID parse to AssetPath.", "GUID invalid.", AssetDatabase.GUIDToAssetPath);
            EditorGUILayout.Space(10);
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                GUIItem(ref astPathStr, "AssetPathToGUID", "input AssetPath parse to GUID.", "AssetPath invalid.", AssetDatabase.AssetPathToGUID);
            EditorGUILayout.Space(10);
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                GUIDAndLocalFileIdentifierGUI();
        }
    }
    private void GUIItem(ref string varVal, string varTitle, string varTip_1, string varTips_2, Func<string, string> varFunc)
    {
        varVal = EditorGUILayout.TextField(varTitle, varVal);
        var tempPath = string.Empty;
        if (string.IsNullOrEmpty(varVal))
        {
            EditorGUILayout.HelpBox(varTip_1, MessageType.Info);
        }
        else
        {
            tempPath = varFunc(varVal);
            if (string.IsNullOrEmpty(tempPath))
            {
                EditorGUILayout.HelpBox(varTips_2, MessageType.Warning);
            }
            else
            {
                EditorGUILayout.LabelField(tempPath);
            }
        }
        using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(tempPath)))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Log")) Debug.LogError(tempPath);
                if (GUILayout.Button("Copy 2 SystemCopyBuffer"))
                {
                    Debug.LogError(tempPath);
                    GUIUtility.systemCopyBuffer = tempPath;
                }
            }
        }
    }
    private void GUIDAndLocalFileIdentifierGUI()
    {
        var tempSelObjs = Selection.objects;
        bool tempValid = true;
        var tempPaths = new List<string>();
        if (tempSelObjs == null || tempSelObjs.Length == 0)
        {
            tempValid = false;
            EditorGUILayout.HelpBox("Select Assets to parse.", MessageType.Info);
        }
        else
        {
            foreach (var item in tempSelObjs)
            {
                var tempPath = AssetDatabase.GetAssetPath(item);
                tempValid &= !string.IsNullOrEmpty(tempPath);
                tempPaths.Add(tempPath);
            }

            if (tempValid)
            {
                EditorGUILayout.LabelField("Selection Object");
                tempPaths.ForEach(p => EditorGUILayout.LabelField(p));
            }
            else
            {
                EditorGUILayout.HelpBox($"Can't parse [{tempSelObjs.ToString()}] Assets.", MessageType.Warning);
            }
        }
        using (new EditorGUI.DisabledGroupScope(!tempValid))
        {
            if (GUILayout.Button("TryGetGUIDAndLocalFileIdentifier"))
            {
                foreach (var item in tempPaths)
                {
                    if (string.IsNullOrEmpty(item)) continue;
                    var tempStrBuilder = new StringBuilder();
                    var tempObjs = AssetDatabase.LoadAllAssetsAtPath(item);
                    foreach (var tempObj in tempObjs)
                    {
                        string guid; long file;
                        if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(tempObj, out guid, out file))
                        {
                            var tempVal = $"GUID: {guid} Instance ID: {tempObj.GetInstanceID()} FileID: {file} Asset: {tempObj.name}";
                            tempStrBuilder.AppendLine(tempVal);
                        }
                    }
                    Debug.LogError($"{Path.GetFileName(item)} {tempStrBuilder.ToString()}");
                }
            }
        }
    }
    #endregion

    #region [Business]
    private void PeekCopyBuffer()
    {
        var tempStr = GUIUtility.systemCopyBuffer;

        if (tempStr.Replace("\\", "/").StartsWith("Assets/"))
        {
            astPathStr = tempStr;
        }
        else if (tempStr.Length == 32)
        {
            guidStr = tempStr;
        }
    }
    #endregion
}