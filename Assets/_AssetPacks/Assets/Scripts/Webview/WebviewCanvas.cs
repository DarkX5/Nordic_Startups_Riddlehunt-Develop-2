using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public interface ICreateGenericWebView
{
    public IGenericWebView Create();
}

public class CreateGenericWebview : ICreateGenericWebView
{
    private GenericWebView _prefab;
    private RectTransform _parent;
    private GenericWebView _instance;
    public CreateGenericWebview(GenericWebView prefab, RectTransform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }
    public IGenericWebView Create()
    {
        if (_instance == null)
        {
            _instance = GenericWebView.Factory(_prefab, _parent);
        }
        return _instance;
    }
}

public interface IWebviewCanvas
{
    public WebviewCanvas.Dependencies dependencies { get; }

    public void ConfigureAndDisplay(WebviewCanvas.Config config);
    public void Hide();
    public void RemoveSelf();
}

[RequireComponent((typeof(CanvasController)))]
public class WebviewCanvas : MonoBehaviour, IWebviewCanvas
{
    private IGenericWebView webview;

    public static IWebviewCanvas Factory(WebviewCanvas prefab)
    {
        WebviewCanvas instance = Instantiate(prefab);
        instance.Initialize();
        return instance;
    }
    public class Dependencies
    {
        public ICanvasController CanvasController { get; set; }
        public ICreateGenericWebView WebViewCreator { get; set; }
    }

    public class Config
    {
        public string Url { get; set; }
        public Action<UniWebView> closeAction { get; set; }
        public Action<UniWebViewMessage> messageAction { get; set; }
    }
    
    [SerializeField] private GenericWebView webviewPrefab;
    public void Initialize()
    {
        var canvasController = GetComponent<CanvasController>();
        canvasController.Initialize();
        SetDependencies(new Dependencies()
        {
            CanvasController = canvasController,
            WebViewCreator = new CreateGenericWebview(webviewPrefab, (RectTransform)this.transform)
        });
    }
    
    public Dependencies dependencies { get; private set; }
    public void SetDependencies(Dependencies dependencies)
    {
        this.dependencies = dependencies;
        Hide();
    }

    private Config _config;
    public void ConfigureAndDisplay(Config config)
    {
        this.gameObject.SetActive(true);
        _config = config;
        if (webview == null)
        {
            webview = dependencies.WebViewCreator.Create();
        }
        webview.Prepare();
        if(_config.messageAction != null)
            webview.SubscribeToMessages(_config.messageAction);
        if(_config.closeAction != null)
            webview.SubscribeToCloseEvent(_config.closeAction);
        
        webview.Load(_config.Url);
        webview.Open();
    }

    public void Hide()
    {
        if (_config != null && webview != null)
        {
            if (_config.messageAction != null)
                webview.UnsubscribeToMessages(_config.messageAction);
            if (_config.closeAction != null)
                webview.UnsubscribeToCloseEvent(_config.closeAction);
        }
        this.gameObject.SetActive(false);
    }

    public void RemoveSelf()
    {
        Hide();
        dependencies.CanvasController.DestroySelf();
    }
}
