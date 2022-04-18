using System;
using RHPackages.Core.Scripts;
using RHPackages.Core.Scripts.UI;
using Riddlehouse.Core.Helpers.Helpers;
using UnityEngine;

public interface IResolutionComponent
{
    public void Configure(Action btnAction, string videoLink);
    public void PerformAction();
    public IViewActions GetComponentUIActions();
}

public class ResolutionComponent : IResolutionComponent
{
    public static IResolutionComponent Factory(GameObject go)
    {
        var behaviour = new ComponentHelper<ResolutionComponentBehaviour>().GetBehaviourIfExists(go);
        var component = new ResolutionComponent(behaviour, behaviour);
        behaviour.SetLogicInstance(component);
        return component;
    }

    private IResolutionComponentActions _iResolutionComponentActions;
    private IViewActions _uiActions;
    private Action _btnAction;

    public ResolutionComponent(IResolutionComponentActions iResolutionComponentActions, IViewActions uiActions)
    {
        _iResolutionComponentActions = iResolutionComponentActions;
        _uiActions = uiActions;
    }

    public void Configure(Action btnAction, string videoLink)
    {
        _btnAction = btnAction;
        _iResolutionComponentActions.Configure(videoLink);
    }

    public void PerformAction()
    {
        _btnAction.Invoke();
    }

    public IViewActions GetComponentUIActions()
    {
        return _uiActions;
    }
}

public interface IResolutionComponentActions
{
    public void Configure(string videoLink);
    public void PerformAction();
}
public class ResolutionComponentBehaviour : MonoBehaviour, IResolutionComponentActions, IViewActions
{
    private IResolutionComponent _resolutionComponent;
    
    [SerializeField] private StandardButtonBehaviour serializedBtnBehaviour;
    [SerializeField] private VideoBehaviour serializedVideoPlayerBehaviour;

    private IStandardButton _standardButton;
    private IVideo _testableVideoComponent;

    public const string BtnText = "Videre";

    public void Awake()
    {
        var iVideo = Video.Factory(serializedVideoPlayerBehaviour.gameObject);
        SetDependencies(serializedBtnBehaviour, iVideo);
        _standardButton.GetComponentUIActions().Hide();
    }

    public void SetDependencies(IStandardButton standardButtonBehaviour, IVideo video)
    {
        _testableVideoComponent = video;
        _standardButton = standardButtonBehaviour;
    }

    public void SetLogicInstance(IResolutionComponent value)
    {
        _resolutionComponent =
            new ComponentHelper<IResolutionComponent>().SetLogicInstance(value, _resolutionComponent);
    }
    public void Configure(string videoLink)
    {
        _standardButton.Configure(BtnText, PerformAction);
        // _testableVideoComponent.Configure(videoLink);
        // _testableVideoComponent.Play();
        // _testableVideoComponent.SubscribeToVideoUpdates(ToggleBtn);
        // _testableVideoComponent.SetVideoForFullScreen();
    }

    public void PerformAction()
    {
        _resolutionComponent.PerformAction();
    }
    
    private void ToggleBtn(VideoEvent videoEvent)
    {
        if(videoEvent == VideoEvent.fullscreenClosed)
        {
            _standardButton.GetComponentUIActions().Display();
        }
        else
        {
            _standardButton.GetComponentUIActions().Hide();
        }
    }

    public void FitInView(RectTransform parent, IUIFitters uiFitters)
    {
        uiFitters.FitToFullscreen(GetRectTransform(), parent);
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
        return ComponentType.Resolution;
    }

    public RectTransform GetRectTransform()
    {
        return (RectTransform) this.transform;
    }
}