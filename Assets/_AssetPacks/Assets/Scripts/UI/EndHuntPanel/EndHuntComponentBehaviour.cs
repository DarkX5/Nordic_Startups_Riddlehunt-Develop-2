using System;
using RHPackages.Core.Scripts;
using RHPackages.Core.Scripts.UI;
using Riddlehouse.Core.Helpers.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public interface IEndHuntComponent
{
    public void Configure(string endText, Action buttonAction);
    public void Configure(string endText, string endVideoUrl, Action buttonAction);
    public IEndHuntComponentActions GetEndHuntActions();
    public IViewActions GetComponentUIActions();
}
public class EndHuntComponent : IEndHuntComponent
{
    public readonly IEndHuntComponentActions _endHuntComponentActions;
    public readonly IViewActions _viewActions;
    public static IEndHuntComponent Factory(GameObject go)
    {
        EndHuntComponentBehaviour endHuntComponentBehaviour = new ComponentHelper<EndHuntComponentBehaviour>().GetBehaviourIfExists(go);
        var endHunt = new EndHuntComponent(endHuntComponentBehaviour, endHuntComponentBehaviour);
        return endHunt;
    }
    public EndHuntComponent(IEndHuntComponentActions endHuntComponentActions, IViewActions viewActions)
    {
        _endHuntComponentActions = endHuntComponentActions;
        _viewActions = viewActions;
    }
    public void Configure(string endText, Action buttonAction)
    {
        _endHuntComponentActions.Configure(endText, buttonAction);
    }
    public void Configure(string endText, string endVideoUrl, Action buttonAction)
    {
        _endHuntComponentActions.Configure(endText, endVideoUrl, buttonAction);
    }

    public IEndHuntComponentActions GetEndHuntActions()
    {
        return _endHuntComponentActions;
    }

    public IViewActions GetComponentUIActions()
    {
        return _viewActions;
    }
}
public interface IEndHuntComponentActions
{
    public void Configure(string endText, Action buttonAction);
    public void Configure(string endText, string endVideoUrl, Action buttonAction);
    public void PerformAction();
}
public class EndHuntComponentBehaviour : MonoBehaviour, IEndHuntComponentActions, IViewActions
{
    [Inject] private CanvasLayerManager _clm;
    [SerializeField] private TextMeshProUGUI serializedEndTextField;
    private Action _buttonAction;

    private ITestable<TextMeshProUGUI> _endTextField;
    private IVideoCanvasController _videoCanvasController;
    public void Start()
    {
        var videoController = _clm.GetVideoCanvas();
        SetDependencies(serializedEndTextField, videoController);
        gameObject.SetActive(false);
    }
    public void SetDependencies(TextMeshProUGUI endTextField, IVideoCanvasController videoCanvasController)
    {
        _endTextField = new TestableComponent<TextMeshProUGUI>(endTextField);
        _videoCanvasController = videoCanvasController;
    }

    private bool _video = false;
    public void Configure(string endText, Action buttonAction)
    {
        _video = false;
        _endTextField.Get().text = endText;
        _buttonAction = buttonAction;
    }
    public void Configure(string endText, string endVideoUrl, Action buttonAction)
    {
        _video = true;
        _videoCanvasController.Configure(new VideoCanvasController.Config()
        {
            Url = endVideoUrl,
        });
        _videoCanvasController.GetVideoController().SubscribeToVideoCompletion(() =>
        {
            _videoCanvasController.FullscreenClose();
        });
        _endTextField.Get().text = endText;
        _buttonAction = buttonAction;
    }

    public void PerformAction()
    {
        _buttonAction.Invoke();
        Hide();
    }
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
        if (_video)
        {
            _videoCanvasController.FullscreenOpen();
            _videoCanvasController.Play();
        }
    }

    public void Hide()
    {
        if (_video)
        {
            _videoCanvasController.FullscreenClose();
            _videoCanvasController.GetVideoController().Stop();
        }
        this.gameObject.SetActive(false);
    }

    public bool IsShown()
    {
        return gameObject.activeSelf;
    }

    public ComponentType GetComponentType()
    {
        return ComponentType.End;
    }

    public RectTransform GetRectTransform()
    {
        return (RectTransform) this.transform;
    }
}
