using System;
using RHPackages.Core.Scripts;
using RHPackages.Core.Scripts.UI;
using Riddlehouse.Core.Helpers.Helpers;
using TMPro;
using UnityEngine;

public interface IStandardButton
{
    public void Configure(string buttonText, Action action);
    public void PerformAction();
    public IViewActions GetComponentUIActions();
}
public class StandardButtonBehaviour : MonoBehaviour, IStandardButton, IViewActions
{
    [SerializeField] private TextMeshProUGUI _buttonText;
    private Action _action;

    public void Configure(string buttonText, Action action)
    {
        _buttonText.text = buttonText;
        _action = action;
    }
    public void PerformAction()
    {
        _action.Invoke();
    }

    public void FitInView(RectTransform parent, IUIFitters uiFitters)
    {
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

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public bool IsShown()
    {
        return this.gameObject.activeSelf;
    }

    public ComponentType GetComponentType()
    {
        throw new NotImplementedException();
    }

    public RectTransform GetRectTransform()
    {
        throw new NotImplementedException();
    }

    public IViewActions GetComponentUIActions()
    {
        return this;
    }
}
