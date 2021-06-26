using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AssetBundleBrowser.ExtractAssets
{
    public partial class SerializedFile
    {
        /// <summary>
        /// This header is always in BigEndian when in file Metadata follows directly after the header
        /// </summary>
        public class SerializedFileHeader
        {
            #region [Fields]
            public const int kHeaderSize_Ver8 = 12;

            public uint MetadataSize;
            public uint FileSize;
            public uint Version;
            public uint DataOffset;
            public byte Endianess;
            public byte[] Reserved = new byte[3];
            #endregion

            #region [API]
            public static SerializedFileHeader Parse(EndianBinaryReader varStream)
            {
                var tempHeader = new SerializedFileHeader
                {
                    MetadataSize = varStream.ReadUInt32(),
                    FileSize = varStream.ReadUInt32(),
                    Version = varStream.ReadUInt32(),
                };

                if (tempHeader.Version < (int)SerializedFileFormatVersion.kLargeFilesSupport)
                {
                    tempHeader.DataOffset = varStream.ReadUInt32();
                    tempHeader.Endianess = varStream.ReadByte();
                    tempHeader.Reserved = varStream.ReadBytes(tempHeader.Reserved.Length);
                }
                else
                {
                    varStream.Seek(4, SeekOrigin.Current);

                    varStream.Seek(4, SeekOrigin.Current);
                    tempHeader.MetadataSize = varStream.ReadUInt32();

                    varStream.Seek(4, SeekOrigin.Current);
                    tempHeader.FileSize = varStream.ReadUInt32();

                    varStream.Seek(4, SeekOrigin.Current);
                    tempHeader.DataOffset = varStream.ReadUInt32();

                    varStream.Seek(4, SeekOrigin.Current);
                    tempHeader.Endianess = varStream.ReadByte();
                    tempHeader.Reserved = varStream.ReadBytes(tempHeader.Reserved.Length);
                }

                return tempHeader;
            }

            public override string ToString()
            {
                var tempVerStr = Version.ToString();
                if (Enum.IsDefined(typeof(SerializedFileFormatVersion), Version))
                {
                    tempVerStr = ((SerializedFileFormatVersion)Version).ToString();
                }
                return $"MetadataSize:[{MetadataSize}] FileSize:[{FileSize}] Version:[{Version}({tempVerStr})] DataOffset:[{DataOffset}] Endianess:[{Endianess}] Reserved:[{string.Join("", Reserved)}]";
            }
            #endregion
        }
    }
}