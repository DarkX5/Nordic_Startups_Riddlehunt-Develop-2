using System;
using RHPackages.Core.Scripts;
using RHPackages.Core.Scripts.UI;
using Riddlehouse.Core.Helpers.Helpers;
using TMPro;
using UnityEngine;

public interface IRiddleComponent
{
    public void Configure(string riddleText);
    public IRiddleComponentActions GetRiddleComponentActions();
    public IViewActions GetComponentUIActions();
}

public class RiddleComponent : IRiddleComponent
{
    public static IRiddleComponent Factory(GameObject riddleComponentPrefab)
    {
        
        var riddleComponentBehaviour = new ComponentHelper<RiddleHuntComponentBehaviour>().GetBehaviourIfExists(riddleComponentPrefab);
        return new RiddleComponent(riddleComponentBehaviour, riddleComponentBehaviour);
    }

    private readonly IRiddleComponentActions _riddleComponentActions;
    private readonly IViewActions _viewActions;

    public RiddleComponent(IRiddleComponentActions riddleComponentActions, IViewActions viewActions)
    {
        _riddleComponentActions = riddleComponentActions;
        _viewActions = viewActions;
    }

    public void Configure(string riddleText)
    {
        _riddleComponentActions.Configure(riddleText);
    }
    
    public IRiddleComponentActions GetRiddleComponentActions()
    {
        return _riddleComponentActions;
    }

    public IViewActions GetComponentUIActions()
    {
        return _viewActions;
    }
}

public interface IRiddleComponentActions
{
    public void Configure(string riddleText);
}

public class RiddleHuntComponentBehaviour : MonoBehaviour, IRiddleComponentActions, IViewActions
{
    [SerializeField] private RectTransform thisTransform;
    [SerializeField] private TextMeshProUGUI riddleTextField;
    private readonly ComponentType _componentType;

    public RiddleHuntComponentBehaviour()
    {
        _componentType = ComponentType.Riddle;
    }
    public void FitInView(RectTransform parent, IUIFitters uiFitters)
    {
        uiFitters.FitToFullscreen(GetRectTransform(), parent);
    }

    public void FitInView(RectTransform parent, IUIFitters uiFitters, int index)
    {
        throw new NotImplementedException(); //this is currently only designed to be used within the riddleTab.
    }


    public void Display()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        //button.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }

    public bool IsShown()
    {
        return this.gameObject.activeSelf;
    }

    public ComponentType GetComponentType()
    {
        return _componentType;
    }

    public RectTransform GetRectTransform()
    {
        return thisTransform;
    }

    public void Configure(string riddleText)
    {
        riddleTextField.text = riddleText;
    }
    
}

