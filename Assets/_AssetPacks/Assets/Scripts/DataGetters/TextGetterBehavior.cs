using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public interface ITextGetter
{
    public void GetText(string fileLink, bool cache, Action<string> textRetrieved);
    public void DisposeSelf();
}

public interface ITextGetterActions
{
    public void GetText(string fileLink, bool cache, Action<string> textRetrieved);
    public void DisposeSelf();
}

public class TextGetter : ITextGetter
{
    public static TextGetter Factory(MonoBehaviour monoBehaviour)
    {
        if (monoBehaviour == null)
        {
            throw new ArgumentException("monobehavior missing in textGetter");
        }

        var textGetterBehavior = new TextGetterBehavior(monoBehaviour);
        var textGetter = new TextGetter(textGetterBehavior);

        return textGetter;
    }
    
    private readonly ITextGetterActions _textGetterActions;
    public TextGetter(ITextGetterActions textGetterActions)
    {
        _textGetterActions = textGetterActions;
    }
    public void GetText(string fileLink, bool cache, Action<string> textRetrieved)
    {
        _textGetterActions.GetText(fileLink, cache, textRetrieved);
    }

    public void DisposeSelf()
    {
        _textGetterActions.DisposeSelf();
    }
}
public class TextGetterBehavior : FileGetter, ITextGetterActions
{
    public readonly MonoBehaviour MonoBehaviour;
    private List<UnityWebRequest> _uwrs;

    public TextGetterBehavior(MonoBehaviour monoBehaviour, string directory = "") : base(directory)
    {
        MonoBehaviour = monoBehaviour;
    }
    public void GetText(string fileLink, bool cache, Action<string> textRetrieved)
    {
        if(_uwrs == null)
            _uwrs = new List<UnityWebRequest>();
        MonoBehaviour.StartCoroutine(RetrieveText(fileLink, cache, textRetrieved));
    }

    public void DisposeSelf()
    {
        if(_uwrs != null)
            foreach(UnityWebRequest req in _uwrs)
                req.Dispose();
        _uwrs = null;
    }

    IEnumerator RetrieveText(string fileLink, bool cache, Action<string> fileRetrieved)
    {
        if (!FileExists(fileLink) && cache || !cache)
        {
            yield return MonoBehaviour.StartCoroutine(DownloadTextAndCache(fileLink, cache, fileRetrieved));
        }
        else
        {
            yield return MonoBehaviour.StartCoroutine(CollectTextFromCache("file://"+GetFileLocation(fileLink), fileRetrieved));
        }
    }

    IEnumerator DownloadTextAndCache(string fileLink, bool cache, Action<string> fileRetrieved)
    {
        var idx = _uwrs.Count;
        _uwrs.Add(UnityWebRequest.Get(fileLink));

        yield return _uwrs[idx].SendWebRequest();

        if (_uwrs[idx].result == UnityWebRequest.Result.ConnectionError ||
            _uwrs[idx].result == UnityWebRequest.Result.ProtocolError)
        {
            throw new ArgumentException(_uwrs[idx].error);
        }

        var content = _uwrs[idx].downloadHandler.text;
        if (cache)
            File.WriteAllText(GetFileLocation(fileLink), content);

        fileRetrieved.Invoke(content);
    }

    IEnumerator CollectTextFromCache(string fileLink, Action<string> fileRetrieved)
    {
        var idx = _uwrs.Count;
        _uwrs.Add(UnityWebRequest.Get(fileLink)); 
        
        yield return _uwrs[idx].SendWebRequest();
        if (_uwrs[idx].result == UnityWebRequest.Result.ConnectionError ||
            _uwrs[idx].result == UnityWebRequest.Result.ProtocolError)
        {
            throw new ArgumentException(_uwrs[idx].error);
        }
        var content = _uwrs[idx].downloadHandler.text;
        fileRetrieved.Invoke(content);
    }
}
