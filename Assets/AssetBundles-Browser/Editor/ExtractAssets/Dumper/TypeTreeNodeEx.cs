using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundleBrowser.ExtractAssets
{
    public static class TypeTreeNodeEx
    {
        #region [Fields]
        private static Dictionary<string, string> _CppType2CSharp = new Dictionary<string, string>()
        {
            { "SInt8","sbyte"},
            { "UInt8","byte"},
            { "char","byte"},
            { "short","short"},
            { "SInt16","short"},
            { "UInt16","ushort"},
            { "unsigned short","ushort"},
            { "int","int"},
            { "SInt32","int"},
            { "UInt32","uint"},
            { "unsigned int","uint"},
            { "Type*","uint"},
            { "long long","long"},
            { "SInt64","long"},
            { "UInt64","ulong"},
            { "unsigned long long","ulong"},
            { "FileSize","ulong"},
            { "float","float"},
            { "double","double"},
            { "bool","bool"},
            { "string","string"},
        };
        private static Dictionary<string, Func<TypeTreeNode, List<TypeTreeNode>, string>> _CppTypeConvert = new Dictionary<string, Func<TypeTreeNode, List<TypeTreeNode>, string>>
        {
            { "vector" , CppTypeConvert_vector }
        };
        #endregion

        #region [API]
        public static string GetNodeCsharpType(this TypeTreeNode varNode, List<TypeTreeNode> varTreeNodes)
        {
            if (_CppType2CSharp.TryGetValue(varNode.m_Type, out var tempTypeVal))
            {
                return tempTypeVal;
            }

            if (_CppTypeConvert.TryGetValue(varNode.m_Type, out var tempConverter))
            {
                return tempConverter(varNode, varTreeNodes);
            }
            //Debug.LogWarning(varNode.m_Type);
            return varNode.m_Type;
        }
        #endregion

        #region [Business]
        private static string CppTypeConvert_vector(TypeTreeNode varNode, List<TypeTreeNode> varTreeNodes)
        {
            var tempIdx = varTreeNodes.FindIndex(0, n => varNode == n);
            //check
            {

            }
            var tempType = varTreeNodes[tempIdx + 3].GetNodeCsharpType(varTreeNodes);
            Debug.LogWarningFormat("List<{0}>", tempType);
            return string.Format("List<{0}>", tempType);
        }
        #endregion
    }
}