using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AssetBundleBrowser.ExtractAssets
{
    public class TypeTree2Class
    {
        #region [Fields]
        /// <summary>
        /// K = Field Name, V = Field Type;
        /// </summary>
        private Dictionary<string, string> _FieldsInfo;
        public string ClassName;
        #endregion

        #region [Construct]
        public TypeTree2Class(string varClsName, Dictionary<string, string> varFields)
        {
            Debug.Assert(!string.IsNullOrEmpty(varClsName));
            Debug.Assert(varFields != null);
            ClassName = varClsName;
            _FieldsInfo = varFields;
        }
        #endregion

        #region [API]
        public static List<TypeTree2Class> Convert(List<TypeTreeNode> varTreeNodes) => Convert(varTreeNodes[0], varTreeNodes);
        public static List<TypeTree2Class> Convert(TypeTreeNode varFieldCls, List<TypeTreeNode> varTreeNodes)
        {
            PPtr.IsPPtr(varFieldCls.m_Type, out var tempClsName);
            if (string.IsNullOrEmpty(tempClsName)) tempClsName = varFieldCls.m_Type;

            var tempTreeClses = new List<TypeTree2Class>();

            var tempFieldNames = new Dictionary<string, string>();
            for (int iT = varFieldCls.m_Index + 1; iT < varTreeNodes.Count; ++iT)
            {
                var tempNode = varTreeNodes[iT];
                if (tempNode.m_Level <= varFieldCls.m_Level) break;
                if (tempNode.m_Level != varFieldCls.m_Level + 1) continue;

                if (tempNode.m_Name.StartsWith("m_DefValue"))
                {
                    int i = 1;
                }

                tempFieldNames.Add(tempNode.m_Name, tempNode.GetNodeCsharpTypeDes(varTreeNodes, out var tempFieldTypes));
                foreach (var item in tempFieldTypes)
                {
                    if (!item.IsVauleType())
                    {
                        tempTreeClses.AddRange(TypeTree2Class.Convert(item, varTreeNodes));
                    }
                }
            }

            tempTreeClses.Add(new TypeTree2Class(tempClsName, tempFieldNames));
            return tempTreeClses;
        }

        public bool VaildIfConflict(TypeTree2Class varVailder)
        {
            if (varVailder.ClassName != this.ClassName) return false;

            if (varVailder._FieldsInfo.Count != this._FieldsInfo.Count) return true;

            foreach (var tempFieldsKvp in varVailder._FieldsInfo)
            {
                if (!this._FieldsInfo.TryGetValue(tempFieldsKvp.Key, out var tempFiledType))
                {
                    Debug.LogErrorFormat("Class Fields Decode [{0}] conflict in different TypeTree,Missing Field [{1}].", this.ClassName, tempFieldsKvp.Key);
                    return true;
                }
                else if (tempFiledType != tempFieldsKvp.Value)
                {
                    Debug.LogErrorFormat("Class Fields Decode [{0}] conflict in different TypeTree,Field [{1}] has different Type {2} vs {3}", this.ClassName, tempFieldsKvp.Key, tempFiledType, tempFieldsKvp.Value);
                    return true;
                }
            }

            return false;
        }
        public string Serialized()
        {
            var tempStrBuilder = new StringBuilder();
            tempStrBuilder.AppendFormat("public class {0}\n", ClassName);
            tempStrBuilder.AppendLine("{");

            var tempEnum = _FieldsInfo.GetEnumerator();
            for (int iF = 0; iF < _FieldsInfo.Count; ++iF)
            {
                tempEnum.MoveNext();
                var tempKvp = tempEnum.Current;

                tempStrBuilder.AppendFormat("   public {0} {1};//{2};\n", tempKvp.Value, tempKvp.Key, iF);
            }

            tempStrBuilder.Append("}\n");

            return tempStrBuilder.ToString();
        }

        #endregion
    }
}