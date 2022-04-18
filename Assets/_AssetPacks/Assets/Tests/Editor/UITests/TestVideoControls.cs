using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products;
using riddlehouse_libraries.products.models;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;
[TestFixture]
[Ignore("Deprecated")]
public class TestVideoControls: ZenjectUnitTestFixture
{
    [Test]
    public void TestFactoryNoVideoBehaviorOnGameObject_Throws()
    {
        // Given the gameobject has no VideoBehaviour
        // When constructing Video
        // Then an exception is thrown
        GameObject go = new GameObject();
        //SUT = Video.Factory; returns SUT.
        Assert.Throws<ArgumentException>(() => Video.Factory(go));
    }
    [Test]
    public void TestConfigureVideo()
    {
        // GIVEN a new video object
        // When constructing Video
        // Then configure is called in the videoBehavior.
        GameObject go = new GameObject();
        go.AddComponent<RectTransform>();
        var videoActionsMock = new Mock<IVideoActions>();
        videoActionsMock.Setup(x => x.Configure("https://link.com")).Verifiable();
        Video sut = new Video(videoActionsMock.Object, go);
        sut.Configure("https://link.com");
        videoActionsMock.Verify(x => x.Configure("https://link.com"));
        Assert.AreEqual(((RectTransform)go.transform).rect.height, sut.portraitModeHeight);
    }
    
    [Test]
    public void TestPlay()
    {
        // GIVEN a configured video object
        // When playing a Video
        // Then play is called in the videoBehavior.
        GameObject go = new GameObject();
        go.AddComponent<RectTransform>();
        var videoActionsMock = new Mock<IVideoActions>();
        videoActionsMock.Setup(x => x.Play()).Verifiable();
        Video sut = new Video(videoActionsMock.Object, go);
        sut.Configure("https://link.com");
        sut.Play();
        videoActionsMock.Verify(x => x.Play());
    }

    [Test]
    public void TestReplay()
    {
        // GIVEN a configured video object
        // When replaying a Video
        // Then replay is called in the videoBehavior.
        GameObject go = new GameObject();
        go.AddComponent<RectTransform>();
        var videoActionsMock = new Mock<IVideoActions>();
        videoActionsMock.Setup(x => x.Replay()).Verifiable();
        Video sut = new Video(videoActionsMock.Object, go);
        sut.Configure("https://link.com");
        sut.Replay();
        videoActionsMock.Verify(x => x.Replay());
    }

    [Test]
    public void TestPause()
    {
        // GIVEN a configured video object
        // WHEN pausing a Video
        // THEN pause is called in the videoBehavior.
        GameObject go = new GameObject();
        go.AddComponent<RectTransform>();
        var videoActionsMock = new Mock<IVideoActions>();
        videoActionsMock.Setup(x => x.Pause()).Verifiable();
        Video sut = new Video(videoActionsMock.Object, go);
        sut.Configure("https://link.com");
        sut.Play();
        sut.Pause();
        videoActionsMock.Verify(x => x.Pause());
    }
    
    [Test]
    public void TestSetFullScreenMode_On()
    {
        // GIVEN a configured video object
        // WHEN pressing fullscreen
        // THEN screen is moved to fullscreen.
        var go = new GameObject();
        var goParent = new GameObject();
        go.AddComponent<RectTransform>();
        goParent.AddComponent<RectTransform>();
        go.transform.SetParent(goParent.transform);
        
        var videoActionsMock = new Mock<IVideoActions>();
        Video sut = new Video(videoActionsMock.Object, go);
        sut.Configure("https://link.com");
        var preFullscreen = sut.isFullScreen();
        sut.SwapFullScreenMode();
        var postFullscreen = sut.isFullScreen();
        Assert.IsTrue(!preFullscreen); //not full screen.
        Assert.IsTrue(postFullscreen); //is full screen.
    }

    [Test]
    public void TestSetFullScreenMode_Off()
    {
        // GIVEN a configured video object
        // WHEN pressing fullscreen
        // THEN screen is moved to fullscreen.
        var go = new GameObject();
        var goParent = new GameObject();
        go.AddComponent<RectTransform>();
        goParent.AddComponent<RectTransform>();
        go.transform.SetParent(goParent.transform);
        
        var videoActionsMock = new Mock<IVideoActions>();
        Video sut = new Video(videoActionsMock.Object, go);
        sut.Configure("https://link.com");
        sut.SwapFullScreenMode();
        var post_fullscreen = sut.isFullScreen();
        sut.SwapFullScreenMode();
        var pre_fullscreen = sut.isFullScreen();
        Assert.IsTrue(!pre_fullscreen); //not full screen.
        Assert.IsTrue(post_fullscreen); //is full screen.
    }
}
