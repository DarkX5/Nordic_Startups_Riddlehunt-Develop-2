using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Riddlehunt.Beta.Environment.Controls;
using UnityEngine;
using UnityEngine.Serialization;

public interface ICanvasLayerManager
{
    public IVideoCanvasController GetVideoCanvas();
    public IBetaTesterController GetBetaTesterController();
    public IWebviewCanvas GetWebViewCanvas();
    public ILoaderView GetLoaderView();
    public void SetLayerInteractable(CanvasLayerTypeNames layerType);
    public void RegisterCanvas(CanvasLayerTypeNames type, ICanvasController controller);
    public void UnregisterCanvas(CanvasLayerTypeNames type);

}

public enum CanvasLayerTypeNames {none, home, product, map, video, webview, loader, beta }
public class CanvasLayerManager :ICanvasLayerManager
{
    private ICanvasLayerTypes _canvasLayerTypes;

    private Dictionary<CanvasLayerTypeNames, ICanvasController> activeLayers; //created canvasses

    private IVideoCanvasController _videoCanvasController;
    private IWebviewCanvas _webviewCanvas;
    private ILoaderView _loaderView;
    private IBetaTesterController _betaTesterController;

    private List<CanvasLayerTypeNames> layers;
    
    public CanvasLayerManager(ICanvasLayerTypes canvasLayerTypes)
    {
        _canvasLayerTypes = canvasLayerTypes;
        activeLayers = new Dictionary<CanvasLayerTypeNames, ICanvasController>();
        layers = new List<CanvasLayerTypeNames>()
        {
            CanvasLayerTypeNames.home, //prioritize
            CanvasLayerTypeNames.product, //prioritize
            CanvasLayerTypeNames.map,
            CanvasLayerTypeNames.video,
            CanvasLayerTypeNames.webview,
            CanvasLayerTypeNames.loader,
            CanvasLayerTypeNames.beta
        };
    }

    private bool IsLayerAPriority(CanvasLayerTypeNames type)
    {
        if (type == CanvasLayerTypeNames.home || type == CanvasLayerTypeNames.product)
            return true;
        return false;
    }

    public void SetLayerInteractable(CanvasLayerTypeNames layerType)
    {
        if (activeLayers.ContainsKey(layerType))
        {
            var idx = layers.FindIndex(x => x == layerType);
            for (int i = idx; i < layers.Count; i++) //every layer above, including itself.
            {
                if (activeLayers.ContainsKey(layers[i]))
                    activeLayers[layers[i]].SetInteractable(true);
            }

            for (int i = idx - 1; i >= 0; i--) // every layer below.
            {
                if(activeLayers.ContainsKey(layers[i]))
                    activeLayers[layers[i]].SetInteractable(false);
            }
        }
    }

    public void RegisterCanvas(CanvasLayerTypeNames type, ICanvasController controller)
    {
        if(!activeLayers.ContainsKey(type))
            activeLayers.Add(type, controller);
        var idx = layers.FindIndex(x => x == type);
        controller.SetLayerOrder(idx*5);
        SetLayerInteractable(type);
    }

    public void UnregisterCanvas(CanvasLayerTypeNames type)
    {
        if(activeLayers.ContainsKey(type))
            activeLayers.Remove(type);
        
        var idx = layers.FindIndex(x => x == type);

        var newIdx = idx-1;
        while (true) //could be converted to an inverse for-loop.
        {
            if (newIdx >= 0)
            {
                var previousType = layers[newIdx];
                if (activeLayers.ContainsKey(previousType))
                {
                    if (IsLayerAPriority(previousType))
                    {
                        SetLayerInteractable(previousType);
                        return;
                    }
                }
            }
            if (newIdx < 0) return;
            newIdx = newIdx - 1;
        }
    }

    public IVideoCanvasController GetVideoCanvas()
    {
        if (_videoCanvasController == null)
        {
            _videoCanvasController = _canvasLayerTypes.CreateVideoCanvasController();
            RegisterCanvas(CanvasLayerTypeNames.video, _videoCanvasController.dependencies.CanvasController);
        }
        return _videoCanvasController;
    }

    public IBetaTesterController GetBetaTesterController()
    {
        if (_betaTesterController == null)
        {
            _betaTesterController = _canvasLayerTypes.CreateIBetaTesterCanvasController();
        }
        return _betaTesterController;
    }

    public IWebviewCanvas GetWebViewCanvas()
    {
        if (_webviewCanvas == null)
        {
            _webviewCanvas = _canvasLayerTypes.CreateWebviewCanvas();
            RegisterCanvas(CanvasLayerTypeNames.webview, _webviewCanvas.dependencies.CanvasController);
        }
        return _webviewCanvas;
    }

    public ILoaderView GetLoaderView()
    {
        if (_loaderView == null)
        {
            _loaderView = _canvasLayerTypes.CreateLoaderView();
        }

        return _loaderView;
    }
}
