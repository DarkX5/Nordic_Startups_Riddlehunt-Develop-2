using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Moq;
using NUnit.Framework;
using riddlehouse.video;
using riddlehouse_libraries.analytics;
using UnityEngine;
using UnityEngine.UI;

public class TestVideoFullscreenDisplayController : MonoBehaviour
{
    private Mock<IScreenOrientationSetter> _screenOrientationSetter;
    private Mock<IVideoControlOverlay> _videoControlOverlay;
    private GameObject go;
    private RectTransform _rectTransform;
    private RawImage _videoDisplay;
    private Button _playBtn;
    private Button _fullscreenBtn;
    [SetUp]
    public void Init()
    {
        _screenOrientationSetter = new Mock<IScreenOrientationSetter>();
        _videoControlOverlay = new Mock<IVideoControlOverlay>();
        go = new GameObject();
        _rectTransform = go.AddComponent<RectTransform>();
        _videoDisplay = go.AddComponent<RawImage>();
        _playBtn = new GameObject().AddComponent<Button>();
        _fullscreenBtn = new GameObject().AddComponent<Button>();
    }

    [TearDown]
    public void TearDown()
    {
        go = null;
    }

    [Test]
    public void TestSetDependencies()
    {
        var sut = go.AddComponent<VideoFullscreenDisplayController>();
        var dependencies = new VideoFullscreenDisplayController.Dependencies()
        {
            ScreenOrientationSetter = _screenOrientationSetter.Object,
            ControlOverlay = _videoControlOverlay.Object,
            VideoDisplay = _videoDisplay,
            FullscreenTransform = _rectTransform,
            PlayBtn = _playBtn,
            FullScreenBtn = _fullscreenBtn
        };
        sut.SetDependencies(dependencies);
        
        Assert.AreSame(dependencies, sut._dependencies);
    }

    [Test]
    public void TestConfigure_Configures_RawImage_AndControlsOverlay()
    {
        //Given a new videoFullscreenDisplay
        //When configure is called
        //Then the fullscreen view is configured with controls and UI.
        
        //Arrange
        RenderTexture videoTexture = new RenderTexture(1920, 1080, 16, RenderTextureFormat.ARGB32);
        _videoControlOverlay.Setup(x => x.Configure(It.IsAny<VideoControlOverlay.Config>())).Verifiable();
        var sut = go.AddComponent<VideoFullscreenDisplayController>();
        var dependencies = new VideoFullscreenDisplayController.Dependencies()
        {
            VideoDisplay = _videoDisplay,
            ControlOverlay = _videoControlOverlay.Object,
            FullscreenTransform = _rectTransform,
            PlayBtn = _playBtn,
            FullScreenBtn = _fullscreenBtn
        };
        sut.SetDependencies(dependencies);

        var videoControls = new VideoControlOverlay.Config()
        {
            PlayPause = null,
            Replay = null,
            Skip = null
        };
        
        //Act
        sut.Configure(new VideoFullscreenDisplayController.Config()
        {
            VideoControlsConfig = videoControls,
            VideoTexture = videoTexture
        });
        
        //Arrange
        Assert.AreSame(videoTexture, dependencies.VideoDisplay.texture);
        _videoControlOverlay.Verify(x => x.Configure(It.IsAny<VideoControlOverlay.Config>()));
    }

    [Test]
    public void Display()
    {
        //Arrange
        _screenOrientationSetter.Setup(x => x.SetForLandscapeWithAutorotation()).Verifiable();
        
        RenderTexture videoTexture = new RenderTexture(1920, 1080, 16, RenderTextureFormat.ARGB32);
        
        var sut = go.AddComponent<VideoFullscreenDisplayController>();
        var dependencies = new VideoFullscreenDisplayController.Dependencies()
        {
            VideoDisplay = _videoDisplay,
            ScreenOrientationSetter = _screenOrientationSetter.Object,
            ControlOverlay = _videoControlOverlay.Object,
            FullscreenTransform = _rectTransform,
            PlayBtn = _playBtn,
            FullScreenBtn = _fullscreenBtn
        };
        sut.SetDependencies(dependencies);

        var videoControls = new VideoControlOverlay.Config()
        {
            PlayPause = null,
            Replay = null,
            Skip = null
        };
        
        sut.Configure(new VideoFullscreenDisplayController.Config()
        {
            VideoControlsConfig = videoControls,
            VideoTexture = videoTexture
        });
        //Act
        sut.Display();
        
        //Arrange
        _screenOrientationSetter.Verify(x => x.SetForLandscapeWithAutorotation());
        Assert.IsTrue(sut.gameObject.activeSelf);
    }

    [Test]
    public void Hide()
    {
        //Arrange
        _screenOrientationSetter.Setup(x => x.SetForPortrait()).Verifiable();

        RenderTexture videoTexture = new RenderTexture(1920, 1080, 16, RenderTextureFormat.ARGB32);
        
        var sut = go.AddComponent<VideoFullscreenDisplayController>();
        var dependencies = new VideoFullscreenDisplayController.Dependencies()
        {
            VideoDisplay = _videoDisplay,
            ScreenOrientationSetter = _screenOrientationSetter.Object,
            ControlOverlay = _videoControlOverlay.Object,
            FullscreenTransform = _rectTransform,
            PlayBtn = _playBtn,
            FullScreenBtn = _fullscreenBtn
        };
        sut.SetDependencies(dependencies);

        var videoControls = new VideoControlOverlay.Config()
        {
            PlayPause = null,
            Replay = null,
            Skip = null
        };
        
        sut.Configure(new VideoFullscreenDisplayController.Config()
        {
            VideoControlsConfig = videoControls,
            VideoTexture = videoTexture
        });
        
        //Act
        sut.Hide();
        
        //Arrange
        _screenOrientationSetter.Verify(x => x.SetForPortrait());
        Assert.IsFalse(sut.gameObject.activeSelf);
    }

    [TestCase(true)]
    [TestCase(false)]
    [Test]
    public void TestSetInteractable(bool interactable)
    {
        //Given an initialized videoFullscreenDisplay
        //When SetInteractable is called
        //Then it called setInteractable inside the videoControlOverlay.
        
        //Arrange
        var sut = go.AddComponent<VideoFullscreenDisplayController>();
        
        _videoControlOverlay.Setup(x => x.SetInteractable(interactable)).Verifiable();
        
        var dependencies = new VideoFullscreenDisplayController.Dependencies()
        {
            VideoDisplay = _videoDisplay,
            ScreenOrientationSetter = _screenOrientationSetter.Object,
            ControlOverlay = _videoControlOverlay.Object,
            FullscreenTransform = _rectTransform,
            PlayBtn = _playBtn,
            FullScreenBtn = _fullscreenBtn
        };
        sut.SetDependencies(dependencies);
        //Act
        sut.SetInteractable(interactable);
        //Assert
        _videoControlOverlay.Verify(x => x.SetInteractable(interactable));

    }
}
