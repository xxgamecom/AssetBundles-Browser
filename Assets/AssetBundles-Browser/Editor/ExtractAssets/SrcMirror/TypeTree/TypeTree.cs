using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace AssetBundleBrowser.ExtractAssets
{
    public class TypeTree
    {
        #region [Fields]
        public List<TypeTreeNode> Nodes;
        public byte[] m_StringBuffer;
        #endregion

        #region [API]
        public void Parse(EndianBinaryReader varStream, SerializedFileFormatVersion varFormat)
        {
            var tempNodeCount = varStream.ReadInt32();
            var tempStringBufferSize = varStream.ReadInt32();

            var tempStart = varStream.Position;
            
            Nodes = new List<TypeTreeNode>(tempNodeCount);
            for (int i = 0; i < tempNodeCount; ++i)
            {
                var tempNode = new TypeTreeNode();
                tempNode.Parse(varStream, varFormat);
                Nodes.Add(tempNode);
            }
            //Debug.LogError((varStream.Position - tempStart) / tempNodeCount);

            m_StringBuffer = varStream.ReadBytes(tempStringBufferSize);
            using (var stringBufferReader = new BinaryReader(new MemoryStream(m_StringBuffer)))
            {
                for (int i = 0; i < tempNodeCount; ++i)
                {
                    var m_Node = Nodes[i];
                    m_Node.m_Type = ReadString(stringBufferReader, m_Node.m_TypeStrOffset);
                    m_Node.m_Name = ReadString(stringBufferReader, m_Node.m_NameStrOffset);
                }
            }

            string ReadString(BinaryReader stringBufferReader, uint value)
            {
                var isOffset = (value & 0x80000000) == 0;
                if (isOffset)
                {
                    stringBufferReader.BaseStream.Position = value;
                    return stringBufferReader.ReadStringToNull();
                }
                var offset = value & 0x7FFFFFFF;
                if (CommonString.StringBuffer.TryGetValue(offset, out var str))
                {
                    return str;
                }
                return offset.ToString();
            }

        }
        #endregion

        #region [Override]
        public override string ToString()
        {
            return $"Nodes({Nodes.Count}):[{string.Join(",", Nodes)}]" +
                $"m_StringBuffer({m_StringBuffer.Length})";
        }
        #endregion
    }
}