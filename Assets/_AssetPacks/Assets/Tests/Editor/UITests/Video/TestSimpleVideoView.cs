using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using riddlehouse.video;
using riddlehouse_libraries.products.models;
using UnityEngine;
using UnityEngine.UI;

[TestFixture]
public class TestSimpleVideoView
{
    Mock<IVideoControlOverlay> _videoControlOverlay;
    Mock<IVideoCanvasController> _videoCanvasController;
    Mock<IVideoController> _videoController;

    private RawImage videoDisplay;

    [SetUp]
    public void Init()
    {
        _videoControlOverlay = new Mock<IVideoControlOverlay>();
        _videoCanvasController = new Mock<IVideoCanvasController>();
        _videoController = new Mock<IVideoController>();

        videoDisplay = new GameObject().AddComponent<RawImage>();
    }

    [TearDown]
    public void TearDown()
    {
        _videoCanvasController = null;
        _videoCanvasController = null;
        _videoController = null;
    }
    
    [Test]
    public void TestSetDependencies()
    {
        //Given a new uninitialized SimpleVideoView
        //When SetDependencies is called
        //Then dependencies are configured and the videoView is ready to use.
        
        //Arrange
        _videoControlOverlay.Setup(x => x.Initialize()).Verifiable();
        
        var sut = new GameObject().AddComponent<SimpleVideoView>();

        var dependencies = new SimpleVideoView.Dependencies()
        {
            VideoDisplay = videoDisplay,
            VideoControlOverlay = _videoControlOverlay.Object,
            VideoCanvasController = _videoCanvasController.Object
        };
        
        //Act
        sut.SetDependencies(dependencies);
        
        //Assert
        Assert.AreEqual(dependencies, sut._dependencies);
        _videoControlOverlay.Verify(x => x.Initialize());
    }

    [Test]
    public void TestConfigure_Configures_VideoCanvasController_And_VideoControlOverlay()
    {
        //Given an initialized SimpleVideoView and a videoUri
        //When Configure is called with that uri
        //Then the videoView is configured with that uri.
        
        //Arrange
        string videoUri = "https://videoUri.com";
        
        _videoControlOverlay.Setup(x => x.Initialize());
        
        _videoCanvasController.Setup(x => x.Configure(It.IsAny<VideoCanvasController.Config>())).Verifiable();
        _videoControlOverlay.Setup(x=> x.Configure(It.IsAny<VideoControlOverlay.Config>())).Verifiable();
        var sut = new GameObject().AddComponent<SimpleVideoView>();
        sut.SetDependencies(new SimpleVideoView.Dependencies()
        {
            VideoDisplay = videoDisplay,
            VideoControlOverlay = _videoControlOverlay.Object,
            VideoCanvasController = _videoCanvasController.Object
        });
        
        var config = new SimpleVideoView.Config()
        {
            videoUri = videoUri
        };
        
        //Act
        sut.Configure(config);
        
        //Assert
        _videoCanvasController.Verify(x => x.Configure(It.IsAny<VideoCanvasController.Config>()));
        _videoControlOverlay.Verify(x=> x.Configure(It.IsAny<VideoControlOverlay.Config>()));
    }

    [Test]
    public void TestPlayPause_Is_Not_Playing_Calls_Play()
    {
        //Given a configured SimpleVideoView
        //When PlayPause is called while the video isn't playing
        //Then play is called in the videoController and fullscreen is opened.
        
        //Arrange
        string videoUri = "https://videoUri.com";
        
        _videoControlOverlay.Setup(x => x.Initialize());

        _videoCanvasController.Setup(x => x.Configure(It.IsAny<VideoCanvasController.Config>()));
        _videoControlOverlay.Setup(x => x.Configure(It.IsAny<VideoControlOverlay.Config>()));

        _videoController.Setup(x => x.Play()).Verifiable();
        _videoController.Setup(x => x.IsPlaying()).Returns(false).Verifiable();
        _videoCanvasController.Setup(x => x.GetVideoController()).Returns(_videoController.Object).Verifiable();
        
        var sut = new GameObject().AddComponent<SimpleVideoView>();
        sut.SetDependencies(new SimpleVideoView.Dependencies()
        {
            VideoDisplay = videoDisplay,
            VideoControlOverlay = _videoControlOverlay.Object,
            VideoCanvasController = _videoCanvasController.Object
        });
        
        var config = new SimpleVideoView.Config()
        {
            videoUri = videoUri
        };
        
        sut.Configure(config);
        
        //Act
        sut.PlayPause();
        
        _videoController.Verify(x => x.IsPlaying());
        _videoController.Verify(x => x.Play());
        _videoCanvasController.Verify(x => x.GetVideoController());
    }

    [Test]
    public void TestPlayPause_Is_Playing_Calls_Pause()
    {
        //Given a configured SimpleVideoView
        //When PlayPause is called while the video is playing
        //Then pause is called in the videoController
        
        //Arrange
        string videoUri = "https://videoUri.com";
        
        _videoControlOverlay.Setup(x => x.Initialize());
        
        _videoCanvasController.Setup(x => x.Configure(It.IsAny<VideoCanvasController.Config>())).Verifiable();
        _videoControlOverlay.Setup(x=> x.Configure(It.IsAny<VideoControlOverlay.Config>())).Verifiable();
       
        _videoController.Setup(x => x.Pause()).Verifiable();
        _videoController.Setup(x => x.IsPlaying()).Returns(true).Verifiable();
        _videoCanvasController.Setup(x => x.GetVideoController()).Returns(_videoController.Object).Verifiable();
        
        var sut = new GameObject().AddComponent<SimpleVideoView>();
        sut.SetDependencies(new SimpleVideoView.Dependencies()
        {
            VideoDisplay = videoDisplay,
            VideoControlOverlay = _videoControlOverlay.Object,
            VideoCanvasController = _videoCanvasController.Object
        });
        
        var config = new SimpleVideoView.Config()
        {
            videoUri = videoUri
        };
        
        sut.Configure(config);
        
        //Act
        sut.PlayPause();
        
        _videoController.Verify(x => x.IsPlaying());
        _videoController.Verify(x => x.Pause());
        _videoCanvasController.Verify(x => x.GetVideoController());
    }

    [Test]
    public void TestReplay_OpensFullscreen_Calls_Replay()
    {
        //Given a configured SimpleVideoView
        //When Replay is called
        //Then Replay is called in the VideoController and the fullscreen is opened.
        
        //Arrange
        string videoUri = "https://videoUri.com";
        
        _videoControlOverlay.Setup(x => x.Initialize());
        
        _videoCanvasController.Setup(x => 
            x.Configure(It.IsAny<VideoCanvasController.Config>()))
            .Verifiable();
        _videoControlOverlay.Setup(x=> 
            x.Configure(It.IsAny<VideoControlOverlay.Config>()))
            .Verifiable();
        
        _videoController.Setup(x => x.Replay()).Verifiable();
        _videoCanvasController.Setup(x => x.GetVideoController()).Returns(_videoController.Object).Verifiable();
        
        var sut = new GameObject().AddComponent<SimpleVideoView>();
        sut.SetDependencies(new SimpleVideoView.Dependencies()
        {
            VideoDisplay = videoDisplay,
            VideoControlOverlay = _videoControlOverlay.Object,
            VideoCanvasController = _videoCanvasController.Object
        });
        
        var config = new SimpleVideoView.Config()
        {
            videoUri = videoUri
        };
        
        sut.Configure(config);
        
        //Act
        sut.Replay();
        
        //Assert
        _videoController.Verify(x => x.Replay());
        _videoCanvasController.Verify(x => x.GetVideoController());
    }

    [Test]
    public void TestSkip_ClosesFullscreen_Calls_Stop()
    {
        //Given a configured SimpleVideoView
        //When Stop is called
        //Then Stop is called in the VideoController and the fullscreen is close.
        
        //Arrange
        string videoUri = "https://videoUri.com";
        
        _videoControlOverlay.Setup(x => x.Initialize());
        
        _videoCanvasController.Setup(x => 
                x.Configure(It.IsAny<VideoCanvasController.Config>()))
            .Verifiable();
        _videoControlOverlay.Setup(x=> 
                x.Configure(It.IsAny<VideoControlOverlay.Config>()))
            .Verifiable();
        
        _videoController.Setup(x => x.Stop()).Verifiable();
        _videoCanvasController.Setup(x => x.GetVideoController()).Returns(_videoController.Object).Verifiable();
        _videoCanvasController.Setup(x => x.FullscreenClose()).Verifiable();
        
        var sut = new GameObject().AddComponent<SimpleVideoView>();
        sut.SetDependencies(new SimpleVideoView.Dependencies()
        {
            VideoDisplay = videoDisplay,
            VideoControlOverlay = _videoControlOverlay.Object,
            VideoCanvasController = _videoCanvasController.Object
        });
        bool hasBeenCalled = false;
        Action onSkip = () => { hasBeenCalled = true; };
        var config = new SimpleVideoView.Config()
        {
            videoUri = videoUri,
            OnSkip = onSkip
        };
        
        sut.Configure(config);
        
        //Act
        sut.Skip();
        
        //Assert
        _videoController.Verify(x => x.Stop());
        _videoCanvasController.Verify(x => x.GetVideoController());
        _videoCanvasController.Verify(x => x.FullscreenClose());
        Assert.IsTrue(hasBeenCalled);
    }
    
}
