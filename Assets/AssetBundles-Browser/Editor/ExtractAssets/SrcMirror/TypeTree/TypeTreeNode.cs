using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundleBrowser.ExtractAssets
{
    /// <summary>
    /// What is this: Defines a node that make up TypeTree's data.
    /// Notes:
    ///     - This struct is serialized in blob so change to it actually always breaks the serialization of TypeTrees.
    ///  See BlobRead()/BlobWrite().
    /// </summary>
    public class TypeTreeNode
    {
        #region [Enum]
        public enum ETypeFlags : int
        {
            kFlagNone = 0,
            kFlagIsArray = (1 << 0),
            kFlagIsManagedReference = (1 << 1),
            kFlagIsManagedReferenceRegistry = (1 << 2),
            kFlagIsArrayOfRefs = (1 << 3)
        }
        #endregion

        #region [Fields]
        /// <summary>
        /// The version of the serialization format as represented by this type tree.  Usually determined by Transfer() functions.
        /// </summary>
        public ushort m_Version;
        /// <summary>
        /// Level in the hierarchy (0 is the root)
        /// </summary>
        public byte m_Level;
        /// <summary>
        /// Possible values see ETypeFlags
        /// </summary>
        public byte m_TypeFlags;

        /// <summary>
        /// The type of the variable (eg. "Vector3f", "int")
        /// </summary>
        public uint m_TypeStrOffset;
        /// <summary>
        /// The name of the property (eg. "m_LocalPosition")
        /// </summary>
        public uint m_NameStrOffset;
        /// <summary>
        /// = -1 if its not determinable (arrays)
        /// </summary>
        public int m_ByteSize;
        /// <summary>
        /// The index of the property (Prefabs use this index in the override bitset);
        /// And Index of TypeTree Offset
        /// </summary>
        public int m_Index;

        /// <summary>
        /// Serialization meta data (eg. to hide variables in the property editor)
        /// Children or their meta flags with their parents!
        /// </summary>
        public int m_MetaFlag;

        /// <summary>
        /// When node is private reference, this holds the 64bit "hash" of the TypeTreeShareableData of the refed type.
        /// stores Hash128::PackToUInt64(). Why? because the Hash128 type is to expensive to initialize cpu wise(memset)
        /// 0 <=> does not reference a type.
        /// note: if this is deamed to much data (tends to always be zero), we could move the hash to TypeTreeShareableData as a vector and just keep a byte index here.
        /// </summary>
        public ulong m_RefTypeHash;


        public string m_Type;
        public string m_Name;
        #endregion

        #region [API]
        public TypeTreeNode Parse(EndianBinaryReader varStream, SerializedFileFormatVersion varFormat)
        {
            m_Version = varStream.ReadUInt16();
            m_Level = varStream.ReadByte();
            m_TypeFlags = varStream.ReadByte();
            m_TypeStrOffset = varStream.ReadUInt32();
            m_NameStrOffset = varStream.ReadUInt32();
            m_ByteSize = varStream.ReadInt32();
            m_Index = varStream.ReadInt32();
            m_MetaFlag = varStream.ReadInt32();
            if (varFormat >= SerializedFileFormatVersion.kTypeTreeNodeWithTypeFlags)
            {
                m_RefTypeHash = varStream.ReadUInt64();
            }
            return this;
        }

        public bool IsArray() => (m_TypeFlags & (int)ETypeFlags.kFlagIsArray) != 0;
        public bool IsManagedReference() => (m_TypeFlags & (int)ETypeFlags.kFlagIsManagedReference) != 0;
        public bool IsManagedReferenceRegistry() => (m_TypeFlags & (int)ETypeFlags.kFlagIsManagedReferenceRegistry) != 0;
        public bool IsArrayOfRefs() => (m_TypeFlags & (int)ETypeFlags.kFlagIsArrayOfRefs) != 0;
        #endregion

        #region [Override]
        public override string ToString()
        {
            return $"m_Version:[{m_Version}] m_Level:[{m_Level}] m_TypeFlags:[{m_TypeFlags}] m_TypeStrOffset:[{m_TypeStrOffset}]" +
                $"m_NameStrOffset:[{m_NameStrOffset}] m_ByteSize:[{m_ByteSize}] m_Index:[{m_Index}] m_MetaFlag:[{m_MetaFlag}]" +
                $"m_RefTypeHash:[{m_RefTypeHash}] m_Type:[{m_Type}] m_Name:[{m_Name}]";
        }
        #endregion
    }
}