using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public enum AssetBundleAssetType {XRImageReferenceLibrary}

public interface IAssetBundleManifest
{
    public AssetBundlePlatformData GetPlatformRelevantData();
}
public class AssetBundleManifest : IAssetBundleManifest
{
    [JsonProperty("android")]
    public AssetBundlePlatformData Android { get; set; }
    [JsonProperty("ios")]
    public AssetBundlePlatformData IOS { get; set; }

    public AssetBundlePlatformData GetPlatformRelevantData()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            return Android;
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return IOS;
        }
        else if (Application.platform == RuntimePlatform.LinuxEditor ||
                 Application.platform == RuntimePlatform.WindowsEditor)
        {
            return Android;
        }
        else if (Application.platform == RuntimePlatform.OSXEditor)
        {
            return IOS;
        }

        throw new ArgumentException("Asset bundle platform not supported: " + Application.platform);
    }
}

public class AssetBundlePlatformData
{
    [JsonProperty("uri")]
    public string Uri { get; set; }
    [JsonProperty("assets")]
    public List<BundleAsset> Assets { get; set; }
    [JsonProperty("version")]
    public uint Version { get; set; }

    public string GetAssetName(AssetBundleAssetType type)
    {
        var idx = Assets.FindIndex(x => x.Type == type.ToString());
        if (idx >= 0)
            return Assets[idx].Name;
        throw new ArgumentException("no asset of type: " + type + " found.");
    }
}

public class BundleAsset
{
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("type")]
    public string Type { get; set; }
}
