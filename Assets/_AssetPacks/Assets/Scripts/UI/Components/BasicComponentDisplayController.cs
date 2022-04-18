using System.Collections;
using System.Collections.Generic;
using Riddlehouse.Core.Helpers.Helpers;
using UnityEngine;

public interface IComponentDisplayController
{
    public void FitToScreen(RectTransform parent);
    public void Display();
    
    public void Hide();

    public bool IsShown();
}
[RequireComponent(typeof(RectTransform))]
public class BasicComponentDisplayController : MonoBehaviour, IComponentDisplayController
{
    public class Dependencies
    {
        public IUIFitters UIFitter { get; set; }
    }
    public void Initialize()
    {
        SetDependencies(new Dependencies()
        {
            UIFitter = new UIFitters()
        });
    }
    public Dependencies _dependencies { get; private set; }
    public void SetDependencies(Dependencies dependencies)
    {
        _dependencies = dependencies;
    }
    public void FitToScreen(RectTransform parent)
    {
        _dependencies.UIFitter.FitToFullscreen((RectTransform)this.transform, parent);
    }
    public void Display()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public bool IsShown()
    {
        return this.gameObject.activeSelf;
    }
}
