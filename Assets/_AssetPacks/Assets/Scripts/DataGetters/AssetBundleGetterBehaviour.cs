using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public interface IAssetBundleGetter
{
    public void GetAssetBundle(IAssetBundleManifest manifest, Action<AssetBundle> bundleRetrieved);
    public void DisposeSelf();
}

public class AssetBundleGetter : IAssetBundleGetter
{
    private readonly IAssetBundleGetterActions _iAssetBundleGetterActions;
    public AssetBundleGetter(IAssetBundleGetterActions iAssetBundleGetterActions)
    {
        _iAssetBundleGetterActions = iAssetBundleGetterActions;
    }

    public static AssetBundleGetter Factory(MonoBehaviour monoBehaviour)
    {
        if (monoBehaviour == null)
        {
            throw new ArgumentException("monobehavior missing in textGetter");
        }

        var assetBundleGetterBehaviour = new AssetBundleGetterBehaviour(monoBehaviour);
        var assetBundleGetter = new AssetBundleGetter(assetBundleGetterBehaviour);

        return assetBundleGetter;
    }
    public void GetAssetBundle(IAssetBundleManifest manifest, Action<AssetBundle> bundleRetrieved)
    {
        _iAssetBundleGetterActions.GetAssetBundle(manifest, bundleRetrieved);
    }

    public void DisposeSelf()
    {
        _iAssetBundleGetterActions.DisposeSelf();
    }
}

public interface IAssetBundleGetterActions
{
    public void GetAssetBundle(IAssetBundleManifest manifest, Action<AssetBundle> bundleRetrieved);
    public void DisposeSelf();
}
public class AssetBundleGetterBehaviour : IAssetBundleGetterActions
{
    private readonly MonoBehaviour _monoBehaviour;
    private List<UnityWebRequest> _uwrs;
    private List<AssetBundle> _bundles;
    public AssetBundleGetterBehaviour(MonoBehaviour monoBehaviour)
    {
        _monoBehaviour = monoBehaviour;
    }

    public void GetAssetBundle(IAssetBundleManifest manifest, Action<AssetBundle> bundleRetrieved)
    {
        if(_uwrs == null)
            _uwrs = new List<UnityWebRequest>();
        if (_bundles == null)
            _bundles = new List<AssetBundle>();
        
        var platformRelevantData = manifest.GetPlatformRelevantData();
        _monoBehaviour.StartCoroutine(RetrieveAssetBundle(platformRelevantData.Uri, platformRelevantData.Version, bundleRetrieved));
    }

    public void DisposeSelf()
    {
        if(_uwrs != null)
            foreach(UnityWebRequest req in _uwrs)
                req.Dispose();
        _uwrs = null;
        
        if(_bundles != null)
            foreach(AssetBundle bundle in _bundles)
                bundle.Unload(false);
        _bundles = null;
    }

    private IEnumerator RetrieveAssetBundle(string fileLink, uint version, Action<AssetBundle> bundleRetrieved)
    {
        var idx = _uwrs.Count;
        _uwrs.Add(UnityWebRequestAssetBundle.GetAssetBundle(fileLink, version, 0));

        yield return _uwrs[idx].SendWebRequest();
        
        if (_uwrs[idx].result == UnityWebRequest.Result.ConnectionError || _uwrs[idx].result == UnityWebRequest.Result.ProtocolError)
        {
            throw new ArgumentException(_uwrs[idx].error);
        }
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(_uwrs[idx]);
        _bundles.Add(bundle);
        bundleRetrieved.Invoke(bundle);
    }
}
