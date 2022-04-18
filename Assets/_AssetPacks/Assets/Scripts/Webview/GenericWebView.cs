using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public interface IGenericWebView
{
    public void Prepare();
    public void Load(string url);
    public void Open();
    public void Close();

    public void DestroySelf();

    public void SubscribeToMessages(Action<UniWebViewMessage> messageEvent);
    public void UnsubscribeToMessages(Action<UniWebViewMessage> messageEvent);
    public void SubscribeToCloseEvent(Action<UniWebView> closeEvent);
    public void UnsubscribeToCloseEvent(Action<UniWebView> closeEvent);
}

public class GenericWebView : MonoBehaviour, IGenericWebView
{
    public static GenericWebView Factory(GenericWebView prefab, RectTransform parent)
    {
        GenericWebView behaviour = Instantiate(prefab, parent);
        behaviour.SetDependenciesFromFactory();
        return behaviour;
    }
    
    private UniWebView _webview;
    private bool _isPrepared = false;

    public Action<UniWebView> _closeEvent;
    public Action<UniWebViewMessage> _messageEvent;
    
    public void SetDependenciesFromFactory()
    {
        Debug.Log("Dependencies set");
        _closeEvent = (view) => { Debug.Log("blammy"); };
        _messageEvent = (message) => { Debug.Log("Message Recieved"); };
    }

    public void Prepare()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            UniWebViewLogger.Instance.LogLevel = UniWebViewLogger.Level.Debug;
            if (_webview == null)
            {
                _webview = gameObject.AddComponent<UniWebView>();
                _webview.SetUserAgent("riddlehouse-agent");
                _webview.OnShouldClose +=  (view) => { _closeEvent.Invoke(view);  return false;};
                _webview.OnMessageReceived += (view, message) => { _messageEvent.Invoke(message); };
            }
            _webview.ReferenceRectTransform = GetComponent<RectTransform>();
            _webview.SetShowToolbar(false, false, false);
            _isPrepared = true;
        }
        // Close();
    }

    public void Load(string url)
    {
        if (_isPrepared)
        {
            _webview.Load(url);
        }
    }

    public void Open()
    {
        if (_isPrepared)
        {
            _webview.Show();
            _webview.UpdateFrame();
        }
    }

    public void Close()
    {
        if (_isPrepared)
        {
            _webview.Hide();
        }
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    public void SubscribeToMessages(Action<UniWebViewMessage> messageEvent)
    {
        Debug.Log("subscribed message");
        _messageEvent += messageEvent;
    }

    public void UnsubscribeToMessages(Action<UniWebViewMessage> messageEvent)
    {
        Debug.Log("unsubscribed message");
        _messageEvent -= messageEvent;
    }

    public void SubscribeToCloseEvent(Action<UniWebView> closeEvent)
    {
        Debug.Log("subscribed close");
        _closeEvent += closeEvent; 
    }

    public void UnsubscribeToCloseEvent(Action<UniWebView> closeEvent)
    {
        Debug.Log("unsubscribed close");
        _closeEvent -= closeEvent;
    }

    public void OnEnable()
    {
        Prepare();
    }

    public void OnDisable()
    {
        _closeEvent = null;
        _messageEvent = null;
        _isPrepared = false;
        if(_webview != null)
            Destroy(_webview);
    }
}
