using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public interface ITermsWebviewController
{
    public void Configure(TermsWebViewController.Config config);
    public void ReturnButtonAction();
    public void Open();
    public void Close(Action closeEvent = null);
    public bool IsAnimating();
    public bool IsOpen();
}

[RequireComponent(typeof(ImageSliderFromBottomToTop))]
public class TermsWebViewController : MonoBehaviour, ITermsWebviewController
{
    public class Config
    {
        public Action ReturnEvent { get; set; }
        public string Url { get; set; }
    }
    
    public class Dependencies
    {
        public IImageSlider slideComponent;
        public RectTransform referenceTransform;
    }

    public void SetDependencies(Dependencies dependencies)
    {
        _slideComponent = dependencies.slideComponent;
        ReferenceTransform = dependencies.referenceTransform;
    }
    
    private IImageSlider _slideComponent;
    [SerializeField] private RectTransform ReferenceTransform;
    [SerializeField] private UniWebView Webview; 
    private Action _returnEvent;

    public void Configure(Config config)
    {
        if(_slideComponent == null)
            _slideComponent = GetComponent<ImageSliderFromBottomToTop>();
        _returnEvent = config.ReturnEvent;

        //The webview can't run in the editor, so we only enable it on those platforms.
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            UniWebViewLogger.Instance.LogLevel = UniWebViewLogger.Level.Debug;
            if (Webview == null)
            {
                Webview = ReferenceTransform.gameObject.AddComponent<UniWebView>();
            }
            Webview.ReferenceRectTransform = ReferenceTransform;
            Webview.SetShowToolbar(false, false, false);
            Webview.Load(config.Url);
        }
    }
    
    public void ReturnButtonAction()
    {
        _returnEvent?.Invoke();
    }
    
    public void Open()
    {
        _slideComponent.Open(() =>
        {
            if (Webview != null)
            {
                Webview.Show();
                Webview.UpdateFrame();
            }
        });
    }

    public void Close(Action closeEvent = null)
    {
        if (Webview != null)
        {
            Webview.Hide();
        }
        _slideComponent.Close(closeEvent);
    }
    
    public bool IsAnimating()
    {
        return _slideComponent.IsAnimating();
    }

    public bool IsOpen()
    {
        return _slideComponent.IsOpen();
    }
}
