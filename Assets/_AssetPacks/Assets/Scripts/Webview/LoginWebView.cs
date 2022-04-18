using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface ILoginWebview
{
    public void DisplayLogin(string url, Action<string> unpackCallback);
}

public class LoginWebView : ILoginWebview
{
    private ICanvasLayerManager _clm;

    public LoginWebView(ICanvasLayerManager clm)
    {
        _clm = clm;
    }

    private IWebviewCanvas _webviewCanvas;
    private Action<string> UnpackCallback { get; set; }

    public void DisplayLogin(string url, Action<string> unpackCallback)
    {
        UnpackCallback = unpackCallback;
        _webviewCanvas = _clm.GetWebViewCanvas();
        _webviewCanvas.ConfigureAndDisplay(new WebviewCanvas.Config()
        {
            Url = url,
            closeAction = CloseAction,
            messageAction = MessageAction
        });
    }

    void CloseAction(UniWebView view)
    {
        _webviewCanvas.RemoveSelf();
    }

    void MessageAction(UniWebViewMessage message)
    {
        if (message.Path.Equals("auth0callback"))
        {
            UnpackCallback.Invoke(message.RawMessage);
            _webviewCanvas.RemoveSelf();
        }
    }
}