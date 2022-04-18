using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAssetBundleHelper<TType>
{
    public bool ContainsAsset(string assetName, AssetBundle bundle);
    public void RetrieveAsset(string assetName, AssetBundle bundle, Action<TType> assetRetrieved);
}
public class AssetBundleHelper<TType> : IAssetBundleHelper<TType> where TType : class
{
    private MonoBehaviour _monoBehaviour;
    public AssetBundleHelper(MonoBehaviour monoBehaviour)
    {
        _monoBehaviour = monoBehaviour;
    }

    public bool ContainsAsset(string assetName,  AssetBundle bundle)
    {
        return bundle.Contains(assetName);
    }
    public void RetrieveAsset(string assetName, AssetBundle bundle, Action<TType> assetRetrieved)
    {
        if(ContainsAsset(assetName, bundle))
            _monoBehaviour.StartCoroutine(RetrieveAssetFromBundle(assetName, bundle, assetRetrieved));
    }
    
    private IEnumerator RetrieveAssetFromBundle(string assetName,  AssetBundle bundle, Action<TType> AssetRetrieved)
    {
        var assetLoadRequest = bundle.LoadAssetAsync<TType>(assetName);
        yield return assetLoadRequest;
        TType asset = assetLoadRequest.asset as TType;
        if (asset != null)
        {
            AssetRetrieved.Invoke(asset);
        }
    }
}
