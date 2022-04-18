using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Riddlehouse.Core.Helpers.Helpers;
using riddlehouse.video;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public interface IScreenOrientationSetter
{
    public void SetForPortrait();
    public void SetForAutorotation();
    public void SetForLandscapeWithAutorotation();
}
public class ScreenOrientationSetter :IScreenOrientationSetter
{
    public void SetForPortrait()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
    }

    public void SetForAutorotation()
    {
        Screen.orientation = ScreenOrientation.AutoRotation;
        Screen.autorotateToPortrait = true;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
    }

    public void SetForLandscapeWithAutorotation()
    {
        Screen.orientation = ScreenOrientation.LandscapeRight;
        Screen.autorotateToPortrait = true;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.orientation = ScreenOrientation.AutoRotation;
    }
}

public interface IVideoFullscreenDisplayController
{
    public void Configure(VideoFullscreenDisplayController.Config config);
    public void Display();
    public void Hide();
    public void SetInteractable(bool interactable);

}
public class VideoFullscreenDisplayController : MonoBehaviour, IVideoFullscreenDisplayController
{
    public static IVideoFullscreenDisplayController Factory(VideoFullscreenDisplayController prefab, RectTransform parent)
    {
        return Instantiate(prefab, parent).GetComponent<VideoFullscreenDisplayController>();
    }
    public class Dependencies
    {
        public IScreenOrientationSetter ScreenOrientationSetter { get; set; }
        public IVideoControlOverlay ControlOverlay { get; set; }
        public RawImage VideoDisplay { get; set; }
        public RectTransform FullscreenTransform { get; set; }
        public Button PlayBtn { get; set; }
        public Button FullScreenBtn { get; set; }
    }

    public class Config
    {
        public RenderTexture VideoTexture { get; set; }
        public VideoControlOverlay.Config VideoControlsConfig { get; set; }
    }

    public Dependencies _dependencies { get; private set; }
    
    public void Awake()
    {
        SetDependencies(new Dependencies()
        {
            ScreenOrientationSetter = new ScreenOrientationSetter(),
            ControlOverlay = controlOverlay,
            VideoDisplay = videoDisplay,
            FullscreenTransform = fullscreenTransform,
        });
        Hide();
    }

    public void SetDependencies(Dependencies dependencies)
    { 
        _dependencies = dependencies;   
        _dependencies.ControlOverlay.Initialize();
    }
    
    [SerializeField] private RawImage videoDisplay;
    [SerializeField] private RectTransform fullscreenTransform;
    [SerializeField] private VideoControlOverlay controlOverlay;
    private Config _config;
    public void Configure(Config config)
    {
        _config = config;
        _dependencies.VideoDisplay.texture = _config.VideoTexture;
        _dependencies.ControlOverlay.Configure(_config.VideoControlsConfig);
    }

    public void SetInteractable(bool interactable)
    {
        _dependencies.ControlOverlay.SetInteractable(interactable);
    }
    
    public void Display()
    {
        _dependencies.ScreenOrientationSetter.SetForLandscapeWithAutorotation();
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        _dependencies.ScreenOrientationSetter.SetForPortrait();
        this.gameObject.SetActive(false);
    }

    public void OnEnable()
    {
        controlOverlay.UpdateControlUIState();
    }

    public void OnDisable()
    {
        controlOverlay.UpdateControlUIState();
    }
}
