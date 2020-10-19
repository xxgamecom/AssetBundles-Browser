using System;
using System.IO;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEditor;
using System.Reflection;
using System.Diagnostics;
using UnityEditor.Rendering;


using Object = System.Object;
using Debug = UnityEngine.Debug;

public sealed class ShaderMeasure : EditorWindow
{
    #region [Fields]
    private Shader _shader_src;

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

    private static Object _ShaderUtilInstance;
    private static Object ShaderUtil
    {
        get
        {
            if (null == _ShaderUtilInstance)
            {
                _ShaderUtilInstance = ShaderUtilType.GetConstructor(Type.EmptyTypes).Invoke(new Object[] { });
            }
            return _ShaderUtilInstance;
        }
    }

    private string _CLIVersion = string.Empty;
    #endregion

    #region [Menu]
    [MenuItem("Window/AssetBundle Browser/ShaderMeasure")]
    public static void GUIDHelperWindow()
    {
        EditorWindow.GetWindow<ShaderMeasure>();
    }
    #endregion

    #region [GUI]
    private void OnEnable()
    {
        _CLIVersion = CLIVersion();
    }
    private void OnGUI()
    {
        EditorGUILayout.Space();

        var tempSupport = !string.IsNullOrEmpty(_CLIVersion);
        EditorGUILayout.LabelField(_CLIVersion);
        using (new EditorGUI.DisabledScope(!tempSupport))
        {
            _shader_src = EditorGUILayout.ObjectField("Shader", _shader_src, typeof(Shader), true) as Shader;
            using (new EditorGUI.DisabledScope(!_shader_src))
            {
                if (GUILayout.Button("Measure"))
                {
                    var tempPlatMask = 1 << (int)ShaderCompilerPlatform.GLES3x;
                    var tempESCodePath = OpenCompiledShader(_shader_src, tempPlatMask, true);
                    var tempSrcCode = File.ReadAllText(tempESCodePath);
                }
            }
        }
        if (!tempSupport)
        {
            EditorGUILayout.HelpBox("Malioc env not support!", MessageType.Error);
        }
        //credits
        {
            EditorGUILayout.Space();
            GUIStyle _creditsStyle = new GUIStyle();
            _creditsStyle.fontStyle = FontStyle.Italic;
            _creditsStyle.alignment = TextAnchor.MiddleCenter;
            _creditsStyle.normal.textColor = new Color(0, 0, 0, 0.5f);
            GUI.Label(new Rect(15, position.height - 20, position.width, 20), "by 354888562@qq.com update:2020/10/19", _creditsStyle);
        }
    }
    #endregion

    #region [API]
    public static string OpenCompiledShader(Shader varShader, int varExternPlatformsMask, bool varIncludeAllVariants)
    {
        var OpenShaderCombinations = ShaderUtilType.GetMethod("OpenCompiledShader", BindingFlags.NonPublic | BindingFlags.Static);
        OpenShaderCombinations.Invoke(ShaderUtil, new Object[] { varShader, 3, varExternPlatformsMask, varIncludeAllVariants });
        return Path.Combine(Application.dataPath, "../Temp", $"Compiled-{varShader.name.Replace("/", "-")}.shader");
    }
    #endregion

    #region [Business]
    private string CLIVersion()
    {
        string tempOutput = string.Empty, tempError = string.Empty;

        if (ToolCLICMD(out tempOutput, out tempError, "--version") == 0)
        {
            return Regex.Match(tempOutput, @"v\d+\.\d+\.\d+\s\(Build\s\d+\)").Value;
        }
        return string.Empty;
    }

    private int ToolCLICMD(out string varOutput, out string varError, params string[] varParams)
    {
        using (var tempProcess = new Process())
        {
            tempProcess.StartInfo.FileName = "malioc";
            tempProcess.StartInfo.Arguments = string.Join(" ", varParams);

            tempProcess.StartInfo.UseShellExecute = false;
            tempProcess.StartInfo.CreateNoWindow = true;
            tempProcess.StartInfo.RedirectStandardOutput = true;
            tempProcess.StartInfo.RedirectStandardError = true;

            tempProcess.Start();

            varOutput = tempProcess.StandardOutput.ReadToEnd();
            varError = tempProcess.StandardError.ReadToEnd();
            tempProcess.WaitForExit();

            return tempProcess.ExitCode;
        }
    }
    #endregion
}