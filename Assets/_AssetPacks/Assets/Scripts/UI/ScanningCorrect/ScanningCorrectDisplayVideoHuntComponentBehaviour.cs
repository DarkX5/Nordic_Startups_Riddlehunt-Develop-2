using System;
using RHPackages.Core.Scripts;
using RHPackages.Core.Scripts.UI;
using Riddlehouse.Core.Helpers.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public interface IScanningCorrectDisplayVideoComponent
{
    public void Configure(string videoUrl, string buttonText, Action buttonAction);
    public IScanningCorrectDisplayVideoActions GetScanningCorrectDisplayComponentActions();
    public IViewActions GetComponentUIActions();

}
public class ScanningCorrectDisplayVideoComponent : IScanningCorrectDisplayVideoComponent
{
    public static IScanningCorrectDisplayVideoComponent Factory(GameObject go)
    {
        ScanningCorrectDisplayVideoHuntComponentBehaviour scanningCorrectDisplayVideoHuntComponentActions = new ComponentHelper<ScanningCorrectDisplayVideoHuntComponentBehaviour>().GetBehaviourIfExists(go);
        var scanningCorrectDisplayVideo = new ScanningCorrectDisplayVideoComponent(scanningCorrectDisplayVideoHuntComponentActions, scanningCorrectDisplayVideoHuntComponentActions);
        return scanningCorrectDisplayVideo;
    }
    
    private readonly IScanningCorrectDisplayVideoActions _scanningCorrectDisplayVideoActions;
    private readonly IViewActions _viewActions;
    public ScanningCorrectDisplayVideoComponent(IScanningCorrectDisplayVideoActions iScanningCorrectDisplayVideoActions, IViewActions viewActions)
    {
        _scanningCorrectDisplayVideoActions = iScanningCorrectDisplayVideoActions;
        _viewActions = viewActions;
    }

    public void Configure(string videoUrl, string buttonText, Action buttonAction)
    {
        _scanningCorrectDisplayVideoActions.Configure(videoUrl, buttonText, buttonAction);
    }

    public IScanningCorrectDisplayVideoActions GetScanningCorrectDisplayComponentActions()
    {
        return _scanningCorrectDisplayVideoActions;
    }

    public IViewActions GetComponentUIActions()
    {
        return _viewActions;
    }
}

public interface IScanningCorrectDisplayVideoActions
{
    public void Configure(string videoUrl, string buttonText, Action buttonAction);
    public void ScanningSuccessAction();
    public void PerformAction();
}
public class ScanningCorrectDisplayVideoHuntComponentBehaviour : MonoBehaviour, IScanningCorrectDisplayVideoActions, IViewActions
{
    [SerializeField] private TextMeshProUGUI serializedComponentText;
    [SerializeField] private Button serializedBtn; 
    [SerializeField] private VideoBehaviour serializedVideo;
    [SerializeField] private TextMeshProUGUI serializedNextRiddleBtnText;
    private IVideo _video;
    private ITestable<TextMeshProUGUI> _nextRiddlebuttonText;
    private ITestable<Button> _nextButton;
    [SerializeField] RectTransform _rectTransform;
    public void SetDependencies(IVideo video, TextMeshProUGUI nextRiddleButtonText, RectTransform rectTransform, Button nextButton)
    {
        _video = video;
        _nextRiddlebuttonText = new TestableComponent<TextMeshProUGUI>(nextRiddleButtonText);
        _nextButton = new TestableComponent<Button>(nextButton);
        _rectTransform = rectTransform;
    }
    
    private readonly ComponentType _viewType; 
    private Action _buttonAction;
    
    public ScanningCorrectDisplayVideoHuntComponentBehaviour()
    {
        _viewType = ComponentType.Scanning;
    }
    
    public void Configure(string videoUrl, string buttonText, Action buttonAction)
    {
        if(serializedVideo != null && serializedNextRiddleBtnText != null)
            SetDependencies(serializedVideo.Video, serializedNextRiddleBtnText, (RectTransform)this.transform, serializedBtn);

        SetVideo(videoUrl);
        _video.SubscribeToVideoUpdates(ToggleShowBtn);
        _buttonAction = buttonAction;
        _nextRiddlebuttonText.Get().text = buttonText;
    }

    private void SetVideo(string videoUrl)
    {
        // _video.Configure(videoUrl);
        // _video.Pause();
    }

    public void ScanningSuccessAction()
    {
        _video.Play();
        _video.SetVideoForFullScreen();
        ToggleShowBtn(VideoEvent.fullscreenOpen);
    }

    public void PerformAction()
    {
      _buttonAction.Invoke();
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
        return gameObject.activeSelf;
    }

    public ComponentType GetComponentType()
    {
        return _viewType;
    }

    public RectTransform GetRectTransform()
    {
        return _rectTransform;
    }

    private void ToggleShowBtn(VideoEvent videoEvent)
    {
        var isVideoFullScreen = videoEvent == VideoEvent.fullscreenClosed;
        _nextButton.Get().gameObject.SetActive(isVideoFullScreen);
        _nextRiddlebuttonText.Get().gameObject.SetActive(isVideoFullScreen);
    }
}
