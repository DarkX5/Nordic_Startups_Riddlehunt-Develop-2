using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

[TestFixture]
public class TestVideoThumbController
{
    private GameObject go;
    private CanvasGroup cg;
    private Canvas cv;

    private RectTransform targetTransform;
    private RawImage videoDisplay;
    private Mock<IBubbleDropController> leftDropControllerMock;
    private GameObject leftController;
    private RectTransform leftTransform;
    private Mock<IBubbleDropController> rightDropControllerMock;
    private GameObject rightController;
    private RectTransform rightTransform;
    private Mock<IBubbleSlider> bubbleSliderMock;

    [SetUp]
    public void Init()
    {
        go = new GameObject();
        cg = go.AddComponent<CanvasGroup>();
        cv = new GameObject().AddComponent<Canvas>();
        targetTransform = go.AddComponent<RectTransform>();
        leftController = new GameObject();
        leftTransform = leftController.AddComponent<RectTransform>();
        
        rightController = new GameObject();
        rightTransform = rightController.AddComponent<RectTransform>();
        
        leftDropControllerMock = new Mock<IBubbleDropController>();
        rightDropControllerMock = new Mock<IBubbleDropController>();
        videoDisplay = go.AddComponent<RawImage>();
        bubbleSliderMock = new Mock<IBubbleSlider>();
    }

    [TearDown]
    public void TearDown()
    {
        leftDropControllerMock = null;
        rightDropControllerMock = null;
    }

    [Test]
    public void TestSetDependencies()
    {
        var dependencies = new VideoThumbController.Dependencies()
        {
            VideoDisplay = videoDisplay,
            LeftDropController = leftDropControllerMock.Object,
            RightDropController = rightDropControllerMock.Object
        };
        var sut = go.AddComponent<VideoThumbController>();
        
        bubbleSliderMock.Setup(x => x.SetDependencies(It.IsAny<BubbleSlider.Dependencies>())).Verifiable();
        
        sut.SetDependencies(bubbleSliderMock.Object, dependencies);
        
        Assert.AreSame(dependencies, sut._dependencies);
    }

    [Test]
    public void TestConfigure()
    {
 
        var dependencies = new VideoThumbController.Dependencies()
        {
            VideoDisplay = videoDisplay,
            LeftDropController = leftDropControllerMock.Object,
            RightDropController = rightDropControllerMock.Object
        };
        RenderTexture videoTexture = new RenderTexture(1920, 1080, 16, RenderTextureFormat.ARGB32);
        videoDisplay.texture = videoTexture;  

        var sut = go.AddComponent<VideoThumbController>();
        
        bubbleSliderMock.Setup(x => x.Configure(cv)).Verifiable();
        
        sut.SetDependencies(bubbleSliderMock.Object, dependencies);
        
        sut.Configure(null, null, videoTexture, cv);

        bubbleSliderMock.Verify(x => x.Configure(cv));

        Assert.AreEqual(videoDisplay.texture, sut._dependencies.VideoDisplay.texture);
        Assert.AreEqual(bubbleSliderMock.Object, sut.GetBubbleSlider());
    }

    [Test]
    public void TestPlayPauseAction()
    {
        var dependencies = new VideoThumbController.Dependencies()
        {
            VideoDisplay = videoDisplay,
            LeftDropController = leftDropControllerMock.Object,
            RightDropController = rightDropControllerMock.Object
        };
        RenderTexture videoTexture = new RenderTexture(1920, 1080, 16, RenderTextureFormat.ARGB32);
        videoDisplay.texture = videoTexture;  

        var sut = go.AddComponent<VideoThumbController>();
        
        bubbleSliderMock.Setup(x => x.Configure(cv)).Verifiable();
        
        sut.SetDependencies(bubbleSliderMock.Object, dependencies);

        bool hasBeenCalled = false;
        Action playPauseAction = () => { hasBeenCalled = true;};
        
        sut.Configure(playPauseAction, null, videoTexture, cv);
        
        sut.PlayPauseAction();
        
        Assert.IsTrue(hasBeenCalled);
    }
    
    [Test]
    public void FullScreenOpen()
    {
        var dependencies = new VideoThumbController.Dependencies()
        {
            VideoDisplay = videoDisplay,
            LeftDropController = leftDropControllerMock.Object,
            RightDropController = rightDropControllerMock.Object
        };
        RenderTexture videoTexture = new RenderTexture(1920, 1080, 16, RenderTextureFormat.ARGB32);
        videoDisplay.texture = videoTexture;  

        var sut = go.AddComponent<VideoThumbController>();
        
        bubbleSliderMock.Setup(x => x.Configure(cv)).Verifiable();
        
        sut.SetDependencies(bubbleSliderMock.Object, dependencies);

        bool hasBeenCalled = false;
        Action fullscreenAction = () => { hasBeenCalled = true;};
        
        sut.Configure(null, fullscreenAction, videoTexture, cv);
        
        sut.FullScreenOpen();
        
        Assert.IsTrue(hasBeenCalled);
    }
}