using AssetBundles;
using System.Collections;
using UnityEngine;

public class AssetBundleManagerInit : MonoBehaviour
{
    private static AssetBundleLoadManifestOperation s_LoadOperation;

    public static bool IsInitialised()
    {
        return s_LoadOperation == null || s_LoadOperation.IsDone();
    }

    private IEnumerator Start()
    {
        AssetBundleManager.SetDevelopmentAssetBundleServer();
        s_LoadOperation = AssetBundleManager.Initialize();
        return s_LoadOperation;
    }
}
