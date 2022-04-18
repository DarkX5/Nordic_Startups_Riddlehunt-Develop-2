using System;
using System.Collections;
using System.Collections.Generic;
using Riddlehouse.Core.Helpers.Helpers;
using UnityEngine;
using UnityEngine.UI;

public interface IImageFullScreenCanvasActions
{
    public void DisplayImageFullScreen(Sprite imageToDisplay);
    public void Hide();
}
public class ImageFullscreenCanvasBehaviour : MonoBehaviour, IImageFullScreenCanvasActions
{
    public static IImageFullScreenCanvasActions Factory(GameObject go)
    {
        if (new ComponentHelper<ImageFullscreenCanvasBehaviour>().GetBehaviourIfExists(go) != null)
        {
            var behaviour = Instantiate(go);
            behaviour.transform.localScale = Vector3.one;
            return behaviour.GetComponent<ImageFullscreenCanvasBehaviour>();
        }
        return null;
    }
    
    [SerializeField] private Image serializedDisplayField;
    private Image _displayField;
    
    public void Awake()
    {
        SetDependencies(serializedDisplayField);   
        Hide();
    }
    
    public void SetDependencies(Image displayField)
    {
        _displayField = displayField;
    }
    
    public void DisplayImageFullScreen(Sprite imageToDisplay)
    {
        _displayField.sprite = imageToDisplay;
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
