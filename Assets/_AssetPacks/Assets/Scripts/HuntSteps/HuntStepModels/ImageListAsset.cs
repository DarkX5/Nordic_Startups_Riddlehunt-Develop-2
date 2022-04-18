using System;
using System.Collections;
using System.Collections.Generic;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using UnityEngine;

public interface IImageCarousselAsset
{
    public List<Sprite> GetImages();
    public AssetType GetAssetType();
}
public class ImageListAsset :IImageCarousselAsset, IAsset
{
    private List<Sprite> images;
    private readonly Action<bool> _isReady;
    private readonly int targetCount;
    private bool hasFailed = false;
    public ImageListAsset(IImageGetter imageGetter, List<string> uris, Action<bool> isReady)
    {
        targetCount = uris.Count;
        images = new List<Sprite>();
        _isReady = isReady;
        try
        {
            if (uris.Count < 1)
            {
                AllImagesDownloaded(true);
                return;
            }

            foreach (string uri in uris)
            {
                imageGetter.GetImage(uri, false, SingleImageDownloadComplete);
            }
        }
        catch
        {
            hasFailed = true;
            AllImagesDownloaded(false);
        }
    }

    private void SingleImageDownloadComplete(Sprite image)
    {
        images.Add(image);
        if(images.Count == targetCount && !hasFailed)
            AllImagesDownloaded(true);
    }

    private void AllImagesDownloaded(bool success)
    {
        _isReady.Invoke(success);
    }
    
    public List<Sprite> GetImages()
    {
        return images;
    }

    public AssetType GetAssetType()
    {
        return AssetType.ImageList;
    }
}

