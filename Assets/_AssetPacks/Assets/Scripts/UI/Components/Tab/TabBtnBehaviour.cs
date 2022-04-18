using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using RHPackages.Core.Scripts;
using RHPackages.Core.Scripts.UI;
using Riddlehouse.Core.Helpers.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public interface ITabBtn
{
    public void Configure(string buttonText, ComponentType componentType, Action<ComponentType> action);
    public void PerformAction();
    public void SetTabButtonState(TabButtonState state, float width);
    public void SetSiblingIndex(int siblingIndex);
    public int GetSiblingIndex();
}

public class TabBtn : ITabBtn
{
    public static ITabBtn Factory(GameObject go, RectTransform parent)
    {
        var actions = new ComponentHelper<TabBtnBehaviour>().GetBehaviourIfExists(go);
        TabBtn tabBtn = new TabBtn(actions, actions, parent);
        
        return tabBtn;
    }

    private ComponentType _componentType;
    private Action<ComponentType> _action;
    private readonly ITabBtnActions _tabBtnActions;
    private readonly ITabBtnUIActions _tabBtnUIActions;
    public TabButtonState State { get; private set; }
    public TabBtn(ITabBtnActions tabBtnActions, ITabBtnUIActions tabBtnUIActions, RectTransform parent)
    {
        _tabBtnActions = tabBtnActions;
        _tabBtnUIActions = tabBtnUIActions;
        tabBtnActions.SetBtn(this, parent);
    }

    public void Configure(string buttonText, ComponentType componentType, Action<ComponentType> action)
    {
        _componentType = componentType;
        _action = action;
        _tabBtnActions.Configure((buttonText));
    }

    public void PerformAction()
    {
        _action.Invoke(_componentType);
    }

    public void SetTabButtonState(TabButtonState state, float width)
    {
        State = state;
        switch(state)
        {
            case TabButtonState.Highlighted :
                _tabBtnUIActions.Display();
                _tabBtnActions.SetWidthAndColor(width);
                break;
            case TabButtonState.Active :
                _tabBtnUIActions.Display();
                _tabBtnActions.SetWidthAndColor(width);
                break;
            case TabButtonState.Disabled :
                _tabBtnUIActions.Display();
                _tabBtnUIActions.Disable();
                _tabBtnActions.SetWidthAndColor(width);
                break;
            case TabButtonState.Hidden:
                _tabBtnUIActions.Hide();
                break;
            default:
                throw new ArgumentException("TabButtonState does not exist!");
        }
    }

    public void SetSiblingIndex(int siblingIndex)
    {
        _tabBtnUIActions.SetSiblingIndex(siblingIndex);
    }

    public int GetSiblingIndex()
    {
        return _tabBtnUIActions.GetSiblingIndex();
    }
}

public interface ITabBtnActions
{
    public void Configure(string buttonText);
    public void SetBtn(TabBtn value,  RectTransform parent);
    public void SetWidthAndColor(float width);
}

public interface ITabBtnUIActions
{
    public void Display();
    public void Hide();
    public void Disable();
    public void SetSiblingIndex(int siblingIndex);
    public int GetSiblingIndex();
}
public enum TabButtonState
{
    Highlighted,
    Active,
    Disabled,
    Hidden,
}
public class TabBtnBehaviour : MonoBehaviour, ITabBtnActions, ITabBtnUIActions
{
    // Colors
    [SerializeField] private Color highlighted;
    [SerializeField] private Color active;
    [SerializeField] private Color disabled;
    
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private LayoutElement buttonLayoutElement;
    [SerializeField] private GameObject underLine;
    [SerializeField] private bool interactable;
    private TabBtn _tabBtn;
    
    public void SetBtn(TabBtn value, RectTransform parent)
    {
        _tabBtn = new ComponentHelper<TabBtn>().SetLogicInstance(value, _tabBtn);
        Transform thisTransform = this.transform;
        thisTransform.SetParent(parent);
        thisTransform.localScale = Vector3.one;
    }
    public void Configure(string btnText)
    {
        buttonText.text = btnText;
    }
    
    //connected in unity event trigger.
    public void PerformAction()
    {
        if(interactable)
        _tabBtn.PerformAction();
    }

    public void SetWidthAndColor( float width)
    {
        SetWidth(width);
        this.gameObject.SetActive(_tabBtn.State != TabButtonState.Hidden);
        underLine.gameObject.SetActive(_tabBtn.State == TabButtonState.Highlighted);
        switch(_tabBtn.State)
        {
            case TabButtonState.Highlighted :
                SetTextColor(highlighted);
                break;
            case TabButtonState.Active :
                SetTextColor(active);
                break;
            case TabButtonState.Disabled :
                SetTextColor(disabled);
                break;
            default:
                throw new ArgumentException("TabButtonState does not exist!");
        }
    }

    private void SetTextColor(Color color)
    {
        buttonText.color = color;
    }

    private void SetWidth(float width)
    {
        buttonLayoutElement.preferredWidth = width;
    }

    public void Display()
    {
        interactable = true;
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void Disable()
    {
        interactable = false;
    }

    public void SetSiblingIndex(int siblingIndex)
    {
        this.transform.SetSiblingIndex(siblingIndex);
    }

    public int GetSiblingIndex()
    {
        return this.transform.GetSiblingIndex();
    }
}
