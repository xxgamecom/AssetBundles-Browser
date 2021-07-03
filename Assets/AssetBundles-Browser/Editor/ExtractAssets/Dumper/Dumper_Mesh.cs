using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetBundleBrowser.ExtractAssets
{
    public static class Dumper_Mesh
    {
        public static void LoadMeshs(this SerializedFile varSerializedFile)
        {
            var tempObjMap = varSerializedFile.ObjectMap;
            foreach (var tempKvp in tempObjMap)
            {
                var tempIdx = tempKvp.Value.typeID;
                var tempType = varSerializedFile.Types[tempIdx];
                if (tempType.classID == PersistentTypeID.Mesh)
                {
                    var tempIsUserSrcipt = tempType.classID == PersistentTypeID.MonoBehaviour;
                    var tempIsMonoSrcipt = tempType.classID == PersistentTypeID.MonoScript;
                    //var tempObj = 
                }
            }
        }
    }
}