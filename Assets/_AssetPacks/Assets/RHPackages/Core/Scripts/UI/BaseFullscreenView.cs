using System.Collections;
using System.Collections.Generic;
using RHPackages.Core.Scripts.UI;
using Riddlehouse.Core.Helpers.Helpers;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class BaseFullscreenView : MonoBehaviour, IViewActions
{
    public void FitInView(RectTransform parent, IUIFitters uiFitters)
    {
        uiFitters.FitToFullscreen(GetRectTransform(), parent);
    }

    public void FitInView(RectTransform parent, IUIFitters uiFitters, int index)
    {
        uiFitters.FitToFullscreen(GetRectTransform(), parent);
        this.transform.SetSiblingIndex(index);
    }

    public virtual void Display()
    {
        this.gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public virtual bool IsShown()
    {
        return this.gameObject.activeSelf;
    }

    public virtual ComponentType GetComponentType()
    {
        return ComponentType.FullscreenView;
    }

    public RectTransform GetRectTransform()
    {
        return (RectTransform)this.transform;
    }
}
