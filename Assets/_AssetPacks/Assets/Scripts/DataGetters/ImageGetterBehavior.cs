using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public interface IImageGetter
{
    public void GetImage(string fileLink, bool cache, Action<Sprite> imageRetrieved);
    public void DisposeSelf();
}
public interface IImageGetterActions
{
    public void GetImage(string fileLink, bool cache, Action<Sprite> fileRetrieved);
    public void DisposeSelf();
}

public class ImageGetter : IImageGetter
{
    public static ImageGetter Factory(MonoBehaviour monoBehaviour)
    {
        if (monoBehaviour == null)
        {
            throw new ArgumentException("monobehavior missing in ImageGetter");
        }

        var imageGetterBehavior = new ImageGetterBehavior(monoBehaviour);
        var imageGetter = new ImageGetter(imageGetterBehavior);

        return imageGetter;
    }
    
    private readonly IImageGetterActions _imageGetterActions;
    public ImageGetter(IImageGetterActions imageGetterActions)
    {
        _imageGetterActions = imageGetterActions;
    }
    public void GetImage(string fileLink, bool cache, Action<Sprite> imageRetrieved)
    {
        _imageGetterActions.GetImage(fileLink, cache, imageRetrieved);
    }

    public void DisposeSelf()
    {
        _imageGetterActions.DisposeSelf();
    }
}

public class ImageGetterBehavior : FileGetter, IImageGetterActions
{
    public readonly MonoBehaviour MonoBehaviour;
    private List<UnityWebRequest> _uwrs;
    public ImageGetterBehavior(MonoBehaviour monoBehaviour, string directory = "") : base(directory)
    {
        MonoBehaviour = monoBehaviour;
    }
    
    public void GetImage(string fileLink, bool cache, Action<Sprite> fileRetrieved)
    {
        Debug.Log("Getting image: "+fileLink);
        
        if(_uwrs == null)
            _uwrs = new List<UnityWebRequest>();
        MonoBehaviour.StartCoroutine(RetrieveImage(fileLink, cache, fileRetrieved));
    }

    public void DisposeSelf()
    {
        if(_uwrs != null)
            foreach(UnityWebRequest req in _uwrs)
                req.Dispose();
        _uwrs = null;
    }

    IEnumerator RetrieveImage(string fileLink, bool cache, Action<Sprite> fileRetrieved)
    {
        if (!FileExists(fileLink))
        {
            yield return MonoBehaviour.StartCoroutine(DownloadImageAndCache(fileLink, cache, fileRetrieved));
        }
        else
        {
            yield return MonoBehaviour.StartCoroutine(CollectImageFromCache("file://"+GetFileLocation(fileLink), fileRetrieved));
        }
    }

    IEnumerator DownloadImageAndCache(string fileLink, bool cache, System.Action<Sprite> imageRetrieved)
    {
        var idx = _uwrs.Count;
        _uwrs.Add(UnityWebRequestTexture.GetTexture(fileLink));
        
        yield return _uwrs[idx].SendWebRequest();
        
        if (_uwrs[idx].result == UnityWebRequest.Result.ConnectionError || _uwrs[idx].result == UnityWebRequest.Result.ProtocolError)
        {
            throw new ArgumentException(_uwrs[idx].error);
        }
        var content = DownloadHandlerTexture.GetContent(_uwrs[idx]);
        if(cache)
            File.WriteAllBytes(GetFileLocation(fileLink), content.EncodeToPNG());
        imageRetrieved.Invoke(ConvertTextureToSprite(content));
    }
    
    IEnumerator CollectImageFromCache(string path, Action<Sprite> imageRetrieved)
    {
        var idx = _uwrs.Count;
        _uwrs.Add(UnityWebRequestTexture.GetTexture(path));
        yield return _uwrs[idx].SendWebRequest();

        if (_uwrs[idx].result == UnityWebRequest.Result.ConnectionError || _uwrs[idx].result == UnityWebRequest.Result.ProtocolError)
        {
            throw new ArgumentException(_uwrs[idx].error);
        }

        var content = DownloadHandlerTexture.GetContent(_uwrs[idx]);
        imageRetrieved.Invoke(ConvertTextureToSprite(content));
    }
    private Sprite ConvertTextureToSprite(Texture2D texture)
    {
        try
        {
            Debug.Log("Completed retrieval");
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));
        }
        catch
        {
            throw new ArgumentException("Couldn't convert retrieved texture");
        }
    }
}