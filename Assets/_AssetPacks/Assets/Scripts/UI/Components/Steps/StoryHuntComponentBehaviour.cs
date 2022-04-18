using System;
using RHPackages.Core.Scripts;
using RHPackages.Core.Scripts.UI;
using Riddlehouse.Core.Helpers.Helpers;
using TMPro;
using UnityEngine;

public interface IStoryComponent
{
    public void Configure(string storyText, string buttonText, Action action);
    public void PerformAction();
    public IStoryComponentActions GetStoryComponentActions();
    public IViewActions GetComponentUIActions();
}

public class StoryComponent : IStoryComponent
{
    public static IStoryComponent Factory(GameObject componentPrefab)
    {
        
        var storyComponentBehaviour = new ComponentHelper<StoryHuntComponentBehaviour>().GetBehaviourIfExists(componentPrefab);
        return new StoryComponent(storyComponentBehaviour, storyComponentBehaviour);
    }

    private readonly IStoryComponentActions _storyComponentActions;
    private readonly IViewActions _viewActions;
    private Action _buttonAction;

    public StoryComponent(IStoryComponentActions storyComponentActions, IViewActions viewActions)
    {
        _storyComponentActions = storyComponentActions;
        _viewActions = viewActions;
        _storyComponentActions.SetStory(this);
    }

    public void Configure(string storyText, string buttonText, Action action)
    {
        _buttonAction = action;
        _storyComponentActions.Configure(storyText, buttonText);
    }

    public void PerformAction()
    {
        _buttonAction.Invoke();
    }

    public IStoryComponentActions GetStoryComponentActions()
    {
        return _storyComponentActions;
    }

    public IViewActions GetComponentUIActions()
    {
        return _viewActions;
    }
}

public interface IStoryComponentActions
{
    public void SetStory(StoryComponent storyComponent);
    public void Configure(string storyText, string buttonText);
}

public class StoryHuntComponentBehaviour : MonoBehaviour, IStoryComponentActions, IViewActions
{
    [SerializeField] private TextMeshProUGUI storyTextField;
    [SerializeField] private ComponentType componentType;
    [SerializeField] private StandardButtonBehaviour button;
    private StoryComponent _storyComponent;
    [SerializeField] private RectTransform rectTransform;
    public void FitInView(RectTransform parent, IUIFitters uiFitters)
    {
        uiFitters.FitToFullscreen(GetRectTransform(), parent);
    }

    public void FitInView(RectTransform parent, IUIFitters uiFitters, int index)
    {
        uiFitters.FitToFullscreen(GetRectTransform(), parent);
        this.transform.SetSiblingIndex(index);
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
        return componentType;
    }

    public RectTransform GetRectTransform()
    {
        return rectTransform;
    }

    public void SetStory(StoryComponent storyComponent)
    {
        _storyComponent = storyComponent;
        //_storyComponent = new ComponentHelper<StoryComponent>().SetLogicInstance(storyComponent, _storyComponent);
    }

    public void Configure(string storyText, string buttonText)
    {
        storyTextField.text = storyText;
        button.Configure(buttonText, ( ) => {
            _storyComponent.PerformAction();
        });
    }
}