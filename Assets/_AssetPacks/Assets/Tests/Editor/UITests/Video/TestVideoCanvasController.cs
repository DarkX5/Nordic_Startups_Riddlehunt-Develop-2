using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class TestVideoCanvasController : MonoBehaviour
{
    private GameObject go;
    private RectTransform sutTransform;
    private Canvas cv;
    
    private Mock<IVideoFullscreenDisplayController> videoFullscreenDisplayMock;
    private Mock<IVideoController> videoControllerMock;

    private string url;
    
    [SetUp]
    public void Init()
    {
        go = new GameObject();
        sutTransform = go.AddComponent<RectTransform>();
        cv = go.AddComponent<Canvas>();
        videoFullscreenDisplayMock = new Mock<IVideoFullscreenDisplayController>();
        videoControllerMock = new Mock<IVideoController>();
        
        url = "https://videoUrl.com";
    }

    [TearDown]
    public void TearDown()
    {
        videoFullscreenDisplayMock = null;
        videoControllerMock = null;
    }

    [Test]
    public void TestSetDependencies()
    {
        var sut = go.AddComponent<VideoCanvasController>();
        var dependencies = new VideoCanvasController.Dependencies()
        {
            VideoFullscreenDisplay = videoFullscreenDisplayMock.Object,
            VideoPlayer = videoControllerMock.Object
        };
        sut.SetDependencies(dependencies);
        
        Assert.AreSame(dependencies, sut.dependencies);
    }

    [Test]
    public void TestConfigure()
    {
        var sut = go.AddComponent<VideoCanvasController>();
        
        videoFullscreenDisplayMock
            .Setup(x => 
                x.Configure( It.IsAny<VideoFullscreenDisplayController.Config>()))
            .Verifiable();
        videoControllerMock.Setup(x => x.Prepare(It.IsAny<RenderTexture>(), url)).Verifiable();
        
        var dependencies = new VideoCanvasController.Dependencies()
        {
            VideoFullscreenDisplay = videoFullscreenDisplayMock.Object,
            VideoPlayer = videoControllerMock.Object
        };
        sut.SetDependencies(dependencies);
        
        sut.Configure(new VideoCanvasController.Config()
        {
            Url = url
        });
        
        videoFullscreenDisplayMock
            .Verify(x => 
                x.Configure(It.IsAny<VideoFullscreenDisplayController.Config>()));
        videoControllerMock.Verify(x => x.Prepare(It.IsAny<RenderTexture>(),url));

    }
    
    [Test]
    public void TestFullscreenOpen()
    {
        var sut = go.AddComponent<VideoCanvasController>();
        
        videoFullscreenDisplayMock
            .Setup(x => 
                x.Configure(It.IsAny<VideoFullscreenDisplayController.Config>()));
        videoControllerMock.Setup(x => x.Prepare(url));
        
        videoFullscreenDisplayMock.Setup(x => x.Display()).Verifiable();
        
        var dependencies = new VideoCanvasController.Dependencies()
        {
            VideoFullscreenDisplay = videoFullscreenDisplayMock.Object,
            VideoPlayer = videoControllerMock.Object
        };
        sut.SetDependencies(dependencies);
        
        sut.Configure(new VideoCanvasController.Config()
        {
            Url = url
        });
        
        sut.FullscreenOpen();
        
        videoFullscreenDisplayMock.Verify(x => x.Display());
    }
    
    [Test]
    public void TestFullscreenClose()
    {
        var sut = go.AddComponent<VideoCanvasController>();
        
        videoFullscreenDisplayMock
            .Setup(x => 
                x.Configure(It.IsAny<VideoFullscreenDisplayController.Config>()));
        videoControllerMock.Setup(x => x.Prepare(url));
        
        videoFullscreenDisplayMock.Setup(x => x.Hide()).Verifiable();
        
        var dependencies = new VideoCanvasController.Dependencies()
        {
            VideoFullscreenDisplay = videoFullscreenDisplayMock.Object,
            VideoPlayer = videoControllerMock.Object
        };
        sut.SetDependencies(dependencies);
        
        sut.Configure(new VideoCanvasController.Config()
        {
            Url = url
        });
        
        sut.FullscreenClose();
        
        videoFullscreenDisplayMock.Verify(x => x.Hide());
    }
}
