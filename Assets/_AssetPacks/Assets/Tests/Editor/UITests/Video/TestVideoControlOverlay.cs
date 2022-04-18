using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using riddlehouse.video;
using riddlehouse_libraries.products.models;
using UnityEngine;
using UnityEngine.UI;

public class TestVideoControlOverlay
{
    Mock<ICanvasGroupFader> _faderController;
    Mock<IWaitToExecuteAction> _waitToExecuteAction;
    private Sprite _playIcon;
    private Sprite _pauseIcon;
    
    private Sprite _enterFullscreenIcon;
    private Sprite _exitFullscreenIcon;
    
    private Image _playPauseButtonImage;
    private Image _fullscreenButtonImage;
    private Image _replayButtonImage;
    private Image _skipButtonImage;

    private Mock<IVideoState> _videoState;

    private VideoControlOverlay.Config _config;
    private VideoControlOverlay.Dependencies _dependencies;
    
    [SetUp]
    public void Init()
    {
        _faderController = new Mock<ICanvasGroupFader>();
        _waitToExecuteAction = new Mock<IWaitToExecuteAction>();
        
        _playIcon = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        _pauseIcon = Sprite.Create(Texture2D.grayTexture, Rect.zero, Vector2.down);
        
        _enterFullscreenIcon = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        _exitFullscreenIcon = Sprite.Create(Texture2D.grayTexture, Rect.zero, Vector2.down);

        _playPauseButtonImage = new GameObject().AddComponent<Image>();
        _fullscreenButtonImage = new GameObject().AddComponent<Image>();
        _replayButtonImage = new GameObject().AddComponent<Image>();
        _skipButtonImage = new GameObject().AddComponent<Image>();
        
        _videoState = new Mock<IVideoState>();

        
        _waitToExecuteAction.Setup(x => x.Configure(It.IsAny<Action>(), 1.5f));
        _dependencies = new VideoControlOverlay.Dependencies()
        {
            CanvasGroupFader = _faderController.Object,
            WaitToExecuteAction = _waitToExecuteAction.Object,
            PlayIcon = _playIcon,
            PauseIcon = _pauseIcon,
            EnterFullscreenIcon = _enterFullscreenIcon,
            ExitFullscreenIcon = _exitFullscreenIcon,
            PlayPauseButtonImage = _playPauseButtonImage,
            FullscreenButtonImage = _fullscreenButtonImage,
            ReplayButtonImage = _replayButtonImage,
            SkipButtonImage = _skipButtonImage
        };
        
        _config = new VideoControlOverlay.Config()
        {
            VideoState = _videoState.Object,
            PlayPause = null,
            Replay = null,
            Skip = null,
            FullscreenToggle = null
        };
    }

    [TearDown]
    public void TearDown()
    {
        _faderController = null;
        _waitToExecuteAction = null;
        _playPauseButtonImage = null;
    }

    [Test]
    public void TestSetDependencies()
    {
        //Given a new uninitialized VideoControlOverlay
        //When SetDependencies is called
        //Then dependencies are configured and the VideoControlOverlay is ready to use.
        
        //Arrange
        var sut = new GameObject().AddComponent<VideoControlOverlay>();

        //Act
        sut.SetDependencies(_dependencies);
        
        //Assert
        Assert.AreEqual(_dependencies, sut._dependencies);
    }
    
    [Test]
    public void TestConfigure_Configures_Waiter()
    {
        //Given an initialized VideoControlOverlay
        //When Configure is called
        //Then the controloverlay is configured and the waiter is prepared.
        
        //Arrange
        _waitToExecuteAction.Setup(x => x.Configure(It.IsAny<Action>(), 1.5f)).Verifiable();
        
        var sut = new GameObject().AddComponent<VideoControlOverlay>();
        
        sut.SetDependencies(new VideoControlOverlay.Dependencies()
        {
            CanvasGroupFader = _faderController.Object,
            WaitToExecuteAction = _waitToExecuteAction.Object,
            PlayIcon = _playIcon,
            PauseIcon = _pauseIcon,
            EnterFullscreenIcon = _enterFullscreenIcon,
            ExitFullscreenIcon = _exitFullscreenIcon,
            PlayPauseButtonImage = _playPauseButtonImage,
            FullscreenButtonImage = _fullscreenButtonImage,
            ReplayButtonImage = _replayButtonImage,
            SkipButtonImage = _skipButtonImage
        });

        //Act
        sut.Configure(_config);
        
        //Assert
        _waitToExecuteAction.Verify(x => x.Configure(It.IsAny<Action>(), 1.5f));
    }

    [TestCase(false)]
    [TestCase(true)]
    [Test]
    public void TestSetInteractable(bool interactable)
    {
        //Given an initialized VideoControlOverlay
        //When SetInteractable is called
        //Then the SetInteractable function inside the canvasGroupFader is called with the same value
        
        //Arrange
        var sut = new GameObject().AddComponent<VideoControlOverlay>();
        
        _faderController.Setup(x => x.SetInteractable(interactable)).Verifiable();
        
        sut.SetDependencies(new VideoControlOverlay.Dependencies()
        {
            CanvasGroupFader = _faderController.Object,
            WaitToExecuteAction = _waitToExecuteAction.Object,
            PlayIcon = _playIcon,
            PauseIcon = _pauseIcon,
            EnterFullscreenIcon = _enterFullscreenIcon,
            ExitFullscreenIcon = _exitFullscreenIcon,
            PlayPauseButtonImage = _playPauseButtonImage,
            FullscreenButtonImage = _fullscreenButtonImage,
            ReplayButtonImage = _replayButtonImage,
            SkipButtonImage = _skipButtonImage
        });

        //Act
        sut.SetInteractable(interactable);
        //Assert
        _faderController.Verify(x => x.SetInteractable(interactable));
    }
    
    [Test]
    public void TestConfigure_Configures_HidesAllButtons()
    {
        //Given an initialized VideoControlOverlay
        //When Configure is called, but no actions are assigned
        //Then the controloverlay is configured and all buttons are hidden
        
        //Arrange
        
        var sut = new GameObject().AddComponent<VideoControlOverlay>();
        
        sut.SetDependencies(_dependencies);

        //Act
        sut.Configure(_config);
        
        //Assert
        Assert.IsFalse(_dependencies.ReplayButtonImage.gameObject.activeSelf);
        Assert.IsFalse(_dependencies.SkipButtonImage.gameObject.activeSelf);
        Assert.IsFalse(_dependencies.PlayPauseButtonImage.gameObject.activeSelf);
        Assert.IsFalse(_dependencies.FullscreenButtonImage.gameObject.activeSelf);
    }
    
    [Test]
    public void TestConfigure_Configures_RevealsAllButtons()
    {
        //Given an initialized VideoControlOverlay
        //When Configure is called, and all actions are assigned
        //Then the controloverlay is configured and all buttons are revealed
        
        //Arrange
        
        var sut = new GameObject().AddComponent<VideoControlOverlay>();
        
        sut.SetDependencies(_dependencies);

        _config = new VideoControlOverlay.Config()
        {
            VideoState = _videoState.Object,
            PlayPause = () => { },
            Replay = () => { },
            Skip = () => { },
            FullscreenToggle = () => { }
        };
        
        //Act
        sut.Configure(_config);
        
        //Assert
        Assert.IsTrue(_dependencies.ReplayButtonImage.gameObject.activeSelf);
        Assert.IsTrue(_dependencies.SkipButtonImage.gameObject.activeSelf);
        Assert.IsTrue(_dependencies.PlayPauseButtonImage.gameObject.activeSelf);
        Assert.IsTrue(_dependencies.FullscreenButtonImage.gameObject.activeSelf);
    }


    [Test]
    public void TestPlayPause_Calls_PlayPause_ActionFromConfig_PlaysVideo_SwapsIcon()
    {
        //Given a configured VideoControlOverlay that's connected to a paused videoplayer.
        //When PlayPause is called
        //Then the playPause action is called, and the Play button icon is set to Pause.
        
        //Arrange
        _videoState.Setup(x => x.IsPlaying()).Returns(true).Verifiable();
        
        var sut = new GameObject().AddComponent<VideoControlOverlay>();
        
        sut.SetDependencies(_dependencies);

        bool hasBeenCalled = false;
        Action PlayPauseAction = () => { hasBeenCalled = true; };
        
        var config = new VideoControlOverlay.Config()
        {
            VideoState = _videoState.Object,
            PlayPause = PlayPauseAction,
            Replay = null,
            Skip = null
        };
        
        sut.Configure(config);
        
        //Act
        sut.PlayPause();
        
        //Assert
        Assert.AreEqual(_pauseIcon, _playPauseButtonImage.sprite);
        Assert.IsTrue(hasBeenCalled);
    }

    [Test]
    public void TestPlayPause_Calls_PlayPause_ActionFromConfig_PausesVideo_SwapsIcon()
    {
        //Given a configured VideoControlOverlay that's connected to a running videoplayer.
        //When PlayPause is called
        //Then the playPause action is called, and the Play button icon is set to play.
        
        //Arrange
        _videoState.Setup(x => x.IsPlaying()).Returns(false).Verifiable();
        
        var sut = new GameObject().AddComponent<VideoControlOverlay>();
        
        sut.SetDependencies(_dependencies);

        bool hasBeenCalled = false;
        Action PlayPauseAction = () => { hasBeenCalled = true; };
        
        var config = new VideoControlOverlay.Config()
        {
            VideoState = _videoState.Object,
            PlayPause = PlayPauseAction,
            Replay = null,
            Skip = null
        };
        
        sut.Configure(config);
        
        //Act
        sut.PlayPause();
        
        //Assert
        Assert.AreEqual(_playIcon, _playPauseButtonImage.sprite);
        Assert.IsTrue(hasBeenCalled);
    }

    [Test]
    public void TestReplay_Calls_ReplayAction_FromConfig_SetsPauseIcon()
    {
        //Given a configured VideoControlOverlay.
        //When Replay is called
        //Then the configured action is invoked and the play icon is set to pause.
        
        //Arrange
        var sut = new GameObject().AddComponent<VideoControlOverlay>();
        
        sut.SetDependencies(_dependencies);

        bool hasBeenCalled = false;
        Action replayAction = () => { hasBeenCalled = true; };
        
        var config = new VideoControlOverlay.Config()
        {
            VideoState = _videoState.Object,
            PlayPause = null,
            Replay = replayAction,
            Skip = null
        };
        
        sut.Configure(config);
        
        //Act
        sut.Replay();
        
        //Assert
        Assert.AreEqual(_pauseIcon, _playPauseButtonImage.sprite);
        Assert.IsTrue(hasBeenCalled);
    }

    [Test]
    public void TestSkip_Calls_Close_In_Fader_Then_Calls_SkipAction()
    {
        //Given a configured VideoControlOverlay.
        //When skip is called
        //Then the configured action is invoked and the play icon is set to play.
        
        //Arrange
        _faderController.Setup(x => x.Close(It.IsAny<Action>())).Callback<Action>((theAction) =>
        {
            theAction.Invoke();
        });
        var sut = new GameObject().AddComponent<VideoControlOverlay>();
        
        sut.SetDependencies(new VideoControlOverlay.Dependencies()
        {
            CanvasGroupFader = _faderController.Object,
            WaitToExecuteAction = _waitToExecuteAction.Object,
            PlayIcon = _playIcon,
            PauseIcon = _pauseIcon,
            EnterFullscreenIcon = _enterFullscreenIcon,
            ExitFullscreenIcon = _exitFullscreenIcon,
            PlayPauseButtonImage = _playPauseButtonImage,
            FullscreenButtonImage = _fullscreenButtonImage,
            ReplayButtonImage = _replayButtonImage,
            SkipButtonImage = _skipButtonImage
        });

        bool hasBeenCalled = false;
        Action skipAction = () => { hasBeenCalled = true; };
        
        var config = new VideoControlOverlay.Config()
        {
            VideoState = _videoState.Object,
            PlayPause = null,
            Replay = null,
            Skip = skipAction
        };
        
        sut.Configure(config);
        
        //Act
        sut.Skip();
        
        //Assert
        Assert.AreEqual(_playIcon, _playPauseButtonImage.sprite);
        Assert.IsTrue(hasBeenCalled);
    }

    [Test]
    public void TestFullscreenToggle_Calls_FullscreenToggle_In_Config_Then_SetsFullscreenIcon_Exit()
    {
        //Given a fullscreen VideoControlOverlay.
        //When FullscreenToggle is called
        //Then the configured action is invoked and the fullscreen icon is updated with the exit icon.
        
        //Arrange
        _faderController.Setup(x => x.Close(It.IsAny<Action>())).Callback<Action>((theAction) =>
        {
            theAction.Invoke();
        });
        
        _videoState.Setup(x => x.IsFullscreen()).Returns(false).Verifiable();
        
        var sut = new GameObject().AddComponent<VideoControlOverlay>();
        
        sut.SetDependencies(new VideoControlOverlay.Dependencies()
        {
            CanvasGroupFader = _faderController.Object,
            WaitToExecuteAction = _waitToExecuteAction.Object,
            PlayIcon = _playIcon,
            PauseIcon = _pauseIcon,
            EnterFullscreenIcon = _enterFullscreenIcon,
            ExitFullscreenIcon = _exitFullscreenIcon,
            PlayPauseButtonImage = _playPauseButtonImage,
            FullscreenButtonImage = _fullscreenButtonImage,
            ReplayButtonImage = _replayButtonImage,
            SkipButtonImage = _skipButtonImage
        });

        bool hasBeenCalled = false;
        Action fullscreenToggleAction = () => { hasBeenCalled = true; };
        
        var config = new VideoControlOverlay.Config()
        {
            VideoState = _videoState.Object,
            PlayPause = null,
            Replay = null,
            Skip = null,
            FullscreenToggle = fullscreenToggleAction
        };
        
        sut.Configure(config);
        
        //Act
        sut.ToggleFullscreen();
        
        //Assert
        Assert.AreEqual(_enterFullscreenIcon, _fullscreenButtonImage.sprite);
        Assert.IsTrue(hasBeenCalled);
        _videoState.Verify(x => x.IsFullscreen());
    }
    [Test]
    public void TestFullscreenToggle_Calls_FullscreenToggle_In_Config_Then_SetsFullscreenIcon_Enter()
    {
        //Given a fullscreen VideoControlOverlay.
        //When FullscreenToggle is called
        //Then the configured action is invoked and the fullscreen icon is updated with the enter icon.
        
        //Arrange
        _faderController.Setup(x => x.Close(It.IsAny<Action>())).Callback<Action>((theAction) =>
        {
            theAction.Invoke();
        });
        
        _videoState.Setup(x => x.IsFullscreen()).Returns(true).Verifiable();
        
        var sut = new GameObject().AddComponent<VideoControlOverlay>();
        
        sut.SetDependencies(new VideoControlOverlay.Dependencies()
        {
            CanvasGroupFader = _faderController.Object,
            WaitToExecuteAction = _waitToExecuteAction.Object,
            PlayIcon = _playIcon,
            PauseIcon = _pauseIcon,
            EnterFullscreenIcon = _enterFullscreenIcon,
            ExitFullscreenIcon = _exitFullscreenIcon,
            PlayPauseButtonImage = _playPauseButtonImage,
            FullscreenButtonImage = _fullscreenButtonImage,
            ReplayButtonImage = _replayButtonImage,
            SkipButtonImage = _skipButtonImage
        });

        bool hasBeenCalled = false;
        Action fullscreenToggleAction = () => { hasBeenCalled = true; };
        
        var config = new VideoControlOverlay.Config()
        {
            VideoState = _videoState.Object,
            PlayPause = null,
            Replay = null,
            Skip = null,
            FullscreenToggle = fullscreenToggleAction
        };
        
        sut.Configure(config);
        
        //Act
        sut.ToggleFullscreen();
        
        //Assert
        Assert.AreEqual(_exitFullscreenIcon, _fullscreenButtonImage.sprite);
        Assert.IsTrue(hasBeenCalled);
        _videoState.Verify(x => x.IsFullscreen());
    }

    
    [Test]
    public void TestToggleView_CanvasIsOpen_StopsWaiting_ClosesView()
    {
        //Given a configured videoControlOverlay that's open
        //When ToggleView is called
        //Then the view closes, and stops waiting to close.
        
        //Arrange
        _faderController.Setup(x=> x.IsOpen()).Returns(true).Verifiable();
        _waitToExecuteAction.Setup(x=> x.StopWaiting()).Verifiable();
        _faderController.Setup(x=> x.Close(It.IsAny<Action>())).Verifiable();

        _videoState.Setup(x => x.IsPlaying()).Returns(true).Verifiable();
        
        var sut = new GameObject().AddComponent<VideoControlOverlay>();
        
        sut.SetDependencies(new VideoControlOverlay.Dependencies()
        {
            CanvasGroupFader = _faderController.Object,
            WaitToExecuteAction = _waitToExecuteAction.Object,
            PlayIcon = _playIcon,
            PauseIcon = _pauseIcon,
            EnterFullscreenIcon = _enterFullscreenIcon,
            ExitFullscreenIcon = _exitFullscreenIcon,
            PlayPauseButtonImage = _playPauseButtonImage,
            FullscreenButtonImage = _fullscreenButtonImage,
            ReplayButtonImage = _replayButtonImage,
            SkipButtonImage = _skipButtonImage
        });

        sut.Configure(_config);
        
        //Act
        sut.ToggleView();
        
        //Assert
        _faderController.Verify(x=> x.IsOpen());
        _waitToExecuteAction.Verify(x=> x.StopWaiting());
        _faderController.Verify(x=> x.Close(It.IsAny<Action>()));

        _videoState.Verify(x => x.IsPlaying());

    }

    [Test]
    public void TestToggleView_CanvasIsClosed_StartsWaiting_OpenView()
    {
        //Given a configured videoControlOverlay that's closed
        //When ToggleView is called
        //Then the view opens, and starts waiting to close.
        
        //Arrange
        _faderController.Setup(x=> x.IsOpen()).Returns(false).Verifiable();
        _faderController.Setup(x=> x.Close(It.IsAny<Action>())).Verifiable();
        _waitToExecuteAction.Setup(x => x.BeginWaiting()).Verifiable();

        _videoState.Setup(x => x.IsPlaying()).Returns(true).Verifiable();
        
        var sut = new GameObject().AddComponent<VideoControlOverlay>();
        
        sut.SetDependencies(new VideoControlOverlay.Dependencies()
        {
            CanvasGroupFader = _faderController.Object,
            WaitToExecuteAction = _waitToExecuteAction.Object,
            PlayIcon = _playIcon,
            PauseIcon = _pauseIcon,
            EnterFullscreenIcon = _enterFullscreenIcon,
            ExitFullscreenIcon = _exitFullscreenIcon,
            PlayPauseButtonImage = _playPauseButtonImage,
            FullscreenButtonImage = _fullscreenButtonImage,
            ReplayButtonImage = _replayButtonImage,
            SkipButtonImage = _skipButtonImage
        });

        sut.Configure(_config);
        
        //Act
        sut.ToggleView();
        
        //Assert
        _faderController.Verify(x=> x.IsOpen());
        _faderController.Verify(x=> x.Open(It.IsAny<Action>()));
        _waitToExecuteAction.Verify(x => x.BeginWaiting());
        _videoState.Verify(x => x.IsPlaying());
    }
}
