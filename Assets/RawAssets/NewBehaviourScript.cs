using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var tempBuild = new List<AssetBundleBuild>();
        tempBuild.Add(new AssetBundleBuild());
        MM(tempBuild);
        Debug.LogError(tempBuild.Count);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void MM(List<AssetBundleBuild> varBUilds)
    {
        varBUilds.Clear();
        var tempT = new List<AssetBundleBuild>();
        tempT.Add(new AssetBundleBuild() { assetBundleName = "1"});
        tempT.Add(new AssetBundleBuild() { assetBundleName = "2" });
        varBUilds.AddRange(tempT);
        //varBUilds = new List<AssetBundleBuild>(tempT);
    }
}
