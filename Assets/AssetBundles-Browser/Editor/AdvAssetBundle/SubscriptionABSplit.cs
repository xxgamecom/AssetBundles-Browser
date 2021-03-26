using System;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class SubscriptionABSplitAttribute : Attribute
{
    #region [Construct]
    public SubscriptionABSplitAttribute()
    {

    }
    #endregion
}

public sealed class AssetDsc
{
    #region [Fields]
    public string AssetsGUID;
    public string AssetBundleName { get; private set; }
    #endregion

    #region [Construct]
    public AssetDsc() { }
    public AssetDsc(string varGUID) { AssetsGUID = varGUID; }
    #endregion

    #region [API]
    public void TagAssetBundleName(string varAssetBundleName)
    {
        if (string.IsNullOrEmpty(varAssetBundleName)) throw new ArgumentException("Can't tag NullOrEmpty string to AssetBundle.");
        AssetBundleName = varAssetBundleName.ToLower();
    }
    #endregion

    #region [Over]
    public override int GetHashCode() => base.GetHashCode();
    public override bool Equals(object varObj) => this == (varObj as AssetDsc);
    public override string ToString() => string.Format("GUID[{0}]-[{1}]", AssetsGUID, AssetBundleName);
    public static bool operator !=(AssetDsc lhs, AssetDsc rhs) => !(lhs == rhs);
    public static bool operator ==(AssetDsc lhs, AssetDsc rhs)
    {
        if (lhs is null) return rhs is null;
        if (rhs is null) return false;
        return lhs.AssetsGUID == rhs.AssetsGUID;
    }
    #endregion
}

public class test
{
    [SubscriptionABSplit]
    public static void API()
    {

    }

    [SubscriptionABSplit]
    public void API2()
    {

    }

    [SubscriptionABSplit]
    public static List<AssetDsc> API32()
    {
        return new List<AssetDsc>();
    }


    [MenuItem("Test/ss")]
    public static void TestMenu()
    {

        var tempVals = new List<AssetDsc>();
        var tempSubSplitType = typeof(SubscriptionABSplitAttribute);
        var tempReturnType = typeof(List<AssetDsc>);
        var tempAssemblys = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var tempAssembly in tempAssemblys)
        {
            var tempTypes = tempAssembly.GetExportedTypes();
            foreach (var tempType in tempTypes)
            {
                var tempMethods = tempType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                foreach (var tempMethod in tempMethods)
                {
                    if (tempMethod.ReturnType != tempReturnType) continue;
                    if (!tempMethod.IsDefined(tempSubSplitType, false)) continue;

                    var tempTypeVals = tempMethod.Invoke(null, null) as List<AssetDsc>;
                    if (tempTypeVals == null || tempTypeVals.Count == 0) continue;

                    tempVals.AddRange(tempTypeVals);
                }
            }
        }
    }
}