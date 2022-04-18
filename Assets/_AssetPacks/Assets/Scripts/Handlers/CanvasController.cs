using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface ICanvasController
{
    public void Configure();
    public void Configure(CanvasController.Config config);
    public void SetInteractable(bool interactable);
    public void SetActive(bool visible);
    public void SetLayerOrder(int order);
    public void DestroySelf();
}

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]
public class CanvasController : MonoBehaviour, ICanvasController
{
    [Inject] private CanvasLayerManager clm;
    [SerializeField] private CanvasLayerTypeNames type = CanvasLayerTypeNames.none;
    public class Dependencies
    {
        public Canvas Cv { get; set; }
        public CanvasGroup Cg { get; set; }
        public ICanvasLayerManager Clm { get; set; }
    }

    public class Config
    {
        public Camera ViewCamera;
    }

    public void Initialize()
    {
        var dependencies = new Dependencies()
        {
            Cv = GetComponent<Canvas>(),
            Cg = GetComponent<CanvasGroup>(),
            Clm = clm
        };
        SetDependencies(dependencies);
    }

    public Dependencies _dependencies { get; private set; }
    public void SetDependencies(Dependencies dependencies)
    {
        _dependencies = dependencies;
    }
    
    public void Configure()
    {
        _dependencies.Cv.renderMode = RenderMode.ScreenSpaceOverlay;
        _dependencies.Clm.RegisterCanvas(type, this);
    }
    private Config _config;
    public void Configure(Config config)
    {
        Configure();
        _config = config;
        _dependencies.Cv.renderMode = RenderMode.ScreenSpaceCamera;
        _dependencies.Cv.worldCamera = _config.ViewCamera;
        _dependencies.Cv.planeDistance = 5;
    }

    public void SetLayerOrder(int order)
    {
        _dependencies.Cv.sortingOrder = order;
    }

    public void SetInteractable(bool interactable)
    {
        _dependencies.Cg.interactable = interactable;
        _dependencies.Cg.blocksRaycasts = interactable;
    }

    public void SetActive(bool visible)
    {
        SetInteractable(visible);
        this.gameObject.SetActive(visible);
    }

    public void DestroySelf()
    {
        _dependencies.Clm.UnregisterCanvas(type);
    }
}
