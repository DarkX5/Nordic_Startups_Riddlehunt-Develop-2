using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public interface IVideoThumbController
{
    public void Configure(Action playPauseAction, Action fullscreenAction, RenderTexture videoTexture, Canvas cv);
    public void Hide();
    public void Display();
    public IBubbleSlider GetBubbleSlider();
}
[RequireComponent(typeof(BubbleSlider))]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
public class VideoThumbController : MonoBehaviour, IVideoThumbController
{
    public static IVideoThumbController Factory(VideoThumbController prefab, RectTransform parent)
    {
       return Instantiate(prefab, parent).GetComponent<VideoThumbController>();
    }
    
    public class Dependencies
    {
        public RawImage VideoDisplay { get; set; }
        public IBubbleDropController LeftDropController { get; set; }
        public IBubbleDropController RightDropController { get; set; }
    }

    public void Awake()
    {
        _rt = GetComponent<RectTransform>();
        bubbleSlider = GetComponent<BubbleSlider>();
        RectTransform parent = (RectTransform)_rt.parent;
        SetDependencies(bubbleSlider, new Dependencies()
        {
            VideoDisplay = videoDisplay,
            LeftDropController = BubbleDropController.Factory(bubbleDropControllerPrefab, parent, bubbleSlider, BubbleSlideLockType.left),
            RightDropController = BubbleDropController.Factory(bubbleDropControllerPrefab, parent, bubbleSlider, BubbleSlideLockType.right)
        });
    }

    public void SetDependencies(IBubbleSlider animationController, Dependencies thumbnailControllerDependencies)
    {
        _dependencies = thumbnailControllerDependencies;
        _rt = GetComponent<RectTransform>();
        animationController.SetDependencies(new BubbleSlider.Dependencies()
        {
            LeftLockPoint = thumbnailControllerDependencies.LeftDropController.GetRectTransform(),
            RightLockPoint = thumbnailControllerDependencies.RightDropController.GetRectTransform(),
            TargetTransform = _rt,
            Ac = AnimationCurve.EaseInOut(0,0,1,1),
            Cg = GetComponent<CanvasGroup>(),
        });
        bubbleSliderController = animationController;
    }
    
    private Action _playPauseAction;
    private Action _fullscreenAction;
    [SerializeField] private BubbleDropController bubbleDropControllerPrefab;
    private RectTransform _rt;
    [SerializeField] private RawImage videoDisplay;
    [SerializeField] private BubbleSlider bubbleSlider; //don't use this directly, check setDependencies and Awake
    private IBubbleSlider bubbleSliderController;
    public Dependencies _dependencies { get; private set; }

    public void Configure(Action playPauseAction, Action fullscreenAction, RenderTexture videoTexture, Canvas cv)
    {
        _playPauseAction = playPauseAction;
        _fullscreenAction = fullscreenAction;
        _dependencies.VideoDisplay.texture = videoTexture;
        bubbleSliderController.Configure(cv);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
        _dependencies.LeftDropController.DeactivateUI();
        _dependencies.RightDropController.DeactivateUI();

    }

    public void Display()
    {
        this.gameObject.SetActive(true);
        _dependencies.LeftDropController.ActivateUI();
        _dependencies.RightDropController.ActivateUI();
    }
    
    public void PlayPauseAction()
    {
        _playPauseAction?.Invoke();
    }

    public void FullScreenOpen()
    {
        _fullscreenAction?.Invoke();
    }

    public IBubbleSlider GetBubbleSlider()
    {
        return bubbleSliderController;
    }
}
