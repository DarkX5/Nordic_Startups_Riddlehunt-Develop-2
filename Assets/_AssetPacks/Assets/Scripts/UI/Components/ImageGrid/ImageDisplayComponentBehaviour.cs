using System;
using System.Collections;
using System.Collections.Generic;
using Riddlehouse.Core.Helpers.Helpers;
using UnityEngine;
using UnityEngine.UI;

public interface IImageDisplayComponentHelper
{
    public IImageDisplayActions CreateImageDisplayActions(GameObject prefab, RectTransform parent);
}
public class ImageDisplayComponentHelper : IImageDisplayComponentHelper 
{
    public IImageDisplayActions CreateImageDisplayActions(GameObject prefab, RectTransform parent)
    {
       return ImageDisplayComponentBehaviour.Factory(prefab, parent);
    }
}

public interface IImageDisplayActions
{
    public void Configure(Sprite displayValue, Action<Sprite> fullscreen);
    public void Hide();
}

public class ImageDisplayComponentBehaviour : MonoBehaviour, IImageDisplayActions
{
    public static IImageDisplayActions Factory(GameObject go, RectTransform parent)
    {
        new ComponentHelper<ImageDisplayComponentBehaviour>().GetBehaviourIfExists(go);
        var _go = Instantiate(go);
        _go.transform.SetParent(parent);
        _go.transform.localScale = Vector3.one;
        var behaviour = _go.GetComponent<ImageDisplayComponentBehaviour>();
        return behaviour;
    }
    
    [SerializeField] private Image serializedDisplayField;
    private Image _displayField;
    private Action<Sprite> _fullscreenAction;

    public void Awake()
    {
        SetDependencies(serializedDisplayField);
        _displayField.type = Image.Type.Simple;
        _displayField.preserveAspect = true;
    }
    
    public void SetDependencies(Image displayField)
    {
        _displayField = displayField;
    }
    
    public void Configure(Sprite displayValue, Action<Sprite> fullscreen)
    {
        _displayField.sprite = displayValue;
        _fullscreenAction = fullscreen;
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void Pressed()
    {
        _fullscreenAction.Invoke(_displayField.sprite);
    }
}
