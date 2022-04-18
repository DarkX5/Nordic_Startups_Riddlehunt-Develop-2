using System;
using System.Collections;
using System.Collections.Generic;
using RHPackages.Core.Scripts;
using RHPackages.Core.Scripts.UI;
using Riddlehouse.Core.Helpers.Helpers;
using UnityEngine;

public interface IImageGridComponentActions
{
    public void Configure(List<Sprite> imageList);
    public void DisplayImageFullscreen(Sprite imageToDisplay);
    public void Hide();
    public IViewActions GetComponentUIActions();

}

public class ImageGridComponentBehaviour : MonoBehaviour, IImageGridComponentActions, IViewActions
{
    public static IImageGridComponentActions Factory(GameObject go, RectTransform parent)
    {
        new ComponentHelper<ImageGridComponentBehaviour>().GetBehaviourIfExists(go);
        var behaviour = Instantiate(go, parent, true);
        behaviour.transform.localScale = Vector3.one;
        return behaviour.GetComponent<ImageGridComponentBehaviour>();
    }
    
    [SerializeField] private ImageFullscreenCanvasBehaviour fullscreenPrefab;
    [SerializeField] private ImageDisplayComponentBehaviour serializedImageDisplayComponentPrefab;
    [SerializeField] private RectTransform serializedRectTransform;
    public IImageFullScreenCanvasActions FullscreenCanvas  { get; private set; }
    public List<IImageDisplayActions> AvailableImages { get; private set; }
    private ImageDisplayComponentBehaviour _imageDisplayComponentPrefab;
    private IImageDisplayComponentHelper _helper;
    private RectTransform _thisRectTransform;
    public void Awake()
    {
        IImageFullScreenCanvasActions fullscreenObj = ImageFullscreenCanvasBehaviour.Factory(fullscreenPrefab.gameObject);
        SetDependencies(new List<IImageDisplayActions>(), fullscreenObj, serializedImageDisplayComponentPrefab, new ImageDisplayComponentHelper(), serializedRectTransform);
    }
    
    public void SetDependencies(
        List<IImageDisplayActions> availableImages, 
        IImageFullScreenCanvasActions fullscreenObject, 
        ImageDisplayComponentBehaviour imageDisplayComponentPrefab, 
        IImageDisplayComponentHelper helper,
        RectTransform thisRectTransform)
    {
        AvailableImages = availableImages;
        FullscreenCanvas = fullscreenObject;
        _imageDisplayComponentPrefab = imageDisplayComponentPrefab;
        _helper = helper;
        _thisRectTransform = thisRectTransform;
    }

    public void Configure(List<Sprite> imageList)
    {
        ConfigureImages(imageList);
    }
    
    private void ConfigureImages(List<Sprite> imageList)
    {
        if (imageList.Count > AvailableImages.Count)
        {
            for(int i = AvailableImages.Count; i<imageList.Count; i++)
                AddImage();
        }
        else if (imageList.Count < AvailableImages.Count)
        {
            for(int i = imageList.Count; i<AvailableImages.Count; i++)
                AvailableImages[i].Hide();
        }
        
        var idx = 0;
        foreach (var image in imageList)
        {
            var imageDisplayInstance = AvailableImages[idx];
            imageDisplayInstance.Configure(image, DisplayImageFullscreen);
            idx++;
        }
        
        if (imageList.Count > 0)
            Display();
        else
            Hide();
    }

    private void AddImage()
    {
        AvailableImages.Add(_helper.CreateImageDisplayActions(_imageDisplayComponentPrefab.gameObject, (RectTransform)this.transform));
    }
    
    public void DisplayImageFullscreen(Sprite imageToDisplay)
    {
        FullscreenCanvas.DisplayImageFullScreen(imageToDisplay);
    }

    public void Hide()
    {
        foreach (IImageDisplayActions imageDisplayActions in AvailableImages)
        {
            imageDisplayActions.Hide();
        }

        this.gameObject.SetActive(false);
        FullscreenCanvas?.Hide();
    }

    public IViewActions GetComponentUIActions()
    {
        return this;
    }

    public bool IsShown()
    {
        return this.gameObject.activeSelf;
    }

    public ComponentType GetComponentType()
    {
        //the components are currently intermixed with views, and should be cleaned up; this is a COMPONENT so we leave this until we've cleaned the logic abit more.
        throw new NotImplementedException();
    }

    public RectTransform GetRectTransform()
    {
        return _thisRectTransform;
    }

    public void FitInView(RectTransform parent, IUIFitters uiFitters)
    {
        //Due to a shift in the way we handle the instantiation logic, I'm not sure this is logical anymore... Leaving it notimplemented for now.
        //30th sept. 2021 - Philip haugaard.
        throw new NotImplementedException();
    }

    public void FitInView(RectTransform parent, IUIFitters uiFitters, int index)
    {
        throw new NotImplementedException();
    }

    public void Display()
    {
        this.gameObject.SetActive(true);
    }
}