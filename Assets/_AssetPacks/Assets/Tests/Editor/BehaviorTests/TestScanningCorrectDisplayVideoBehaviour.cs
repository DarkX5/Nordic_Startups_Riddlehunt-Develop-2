using System;
using System.Collections;
using System.Collections.Generic;
using Riddlehouse.Core.Helpers.Helpers;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;
using TMPro;
using UnityEditor.TestTools.TestRunner;
using UnityEngine;
using UnityEngine.UI;

public class TestScanningCorrectDisplayVideoBehaviour
{
    public ScanningCorrectDisplayVideoHuntComponentBehaviour CreateSUT(IVideo video = null, TextMeshProUGUI riddleButtonText = null, RectTransform rectTransform = null)
    {
        var gameobject = new GameObject();
        var behaviour = gameobject.AddComponent<ScanningCorrectDisplayVideoHuntComponentBehaviour>();
        var button = new GameObject().AddComponent<Button>();
        rectTransform = rectTransform == null ? gameobject.AddComponent<RectTransform>() : rectTransform;
        riddleButtonText = riddleButtonText == null ? gameobject.AddComponent<TextMeshProUGUI>() : riddleButtonText;
        video = video == null ? new Mock<IVideo>().Object : video;
        
        behaviour.SetDependencies(video, riddleButtonText, rectTransform, button);
        return behaviour;
    }

    [Ignore("not in use - new video system required.")]
    [Test]
    public void TestConfigure()
    {
        string expectedVideoUrl = "videoURL";
        string expectedButtonText = "buttonText";
        Action buttonAction = () => { };
        var textDisplay = new GameObject().AddComponent<TextMeshProUGUI>();

        var videoMock = new Mock<IVideo>();
        videoMock.Setup(x => x.Configure(expectedVideoUrl)).Verifiable();
        videoMock.Setup(x => x.Pause()).Verifiable();
        var sut = CreateSUT(videoMock.Object, textDisplay);
        sut.Configure(expectedVideoUrl, expectedButtonText, buttonAction);
        videoMock.Verify(x => x.Configure(expectedVideoUrl));
        videoMock.Verify(x => x.Pause());
        
        Assert.AreEqual(expectedButtonText, textDisplay.text);
    }

    [Test]
    public void TestScanningSuccessAction()
    {
        string expectedVideoUrl = "videoURL";
        string expectedButtonText = "buttonText";
        Action buttonAction = () => { };
        var textDisplay = new GameObject().AddComponent<TextMeshProUGUI>();

        var videoMock = new Mock<IVideo>();
        videoMock.Setup(x => x.Configure(expectedVideoUrl));
        videoMock.Setup(x => x.Play()).Verifiable();
        var sut = CreateSUT(videoMock.Object, textDisplay);
        sut.Configure(expectedVideoUrl, expectedButtonText, buttonAction);
        sut.ScanningSuccessAction();
        videoMock.Verify(x => x.Play());
    }
    
    [Test]
    public void TestNextRiddle()
    {
        string expectedVideoUrl = "videoURL";
        string expectedButtonText = "buttonText";
        var hasBeenCalled = false;
        Action buttonAction = () => { hasBeenCalled = true; };
     
        var sut = CreateSUT();
        sut.Configure(expectedVideoUrl, expectedButtonText, buttonAction);
        sut.PerformAction();
        Assert.IsTrue(hasBeenCalled);
    }
   
    [Test]
    public void Test_FitInView_FitsToFullScreen()
    {
        //Given the users starts a new hunt
        //When the hunt is created, the scanningCorrentDisplayVideoBehaviour is UIfitted.
        //Then the scanningCorrentDisplayVideoBehaviour is set to fill the entire screen.

        var gameObject = new GameObject();
        var child = gameObject.AddComponent<RectTransform>();
        var sut = CreateSUT(null, null, child);
        var parent = new GameObject().AddComponent<RectTransform>();

        var uiFittersMock = new Mock<IUIFitters>();
        uiFittersMock.Setup(x => x.FitToFullscreen(child, parent)).Verifiable();
        
        sut.FitInView(parent, uiFittersMock.Object);

        uiFittersMock.Verify(x => x.FitToFullscreen(child, parent));
    }

    [Test]
    public void Test_GetComponentType()
    {
        var sut = CreateSUT();
        Assert.AreEqual(ComponentType.Scanning, sut.GetComponentType());
    }
    [Test]
    public void Test_Display()
    {
        //Given that the user changes view.
        //When the next view is displayed.
        //Then the gameobject is enabled.

        // Arrange
        var sut = CreateSUT();
        sut.gameObject.SetActive(false);
        // Act
        sut.Display();

        // Assert
        Assert.True(sut.gameObject.activeSelf);
    }
    
    [Test]
    public void Test_Hide()
    {
        //Given that the user changes view.
        //When the next view is displayed.
        //Then the previous component is hidden.
        
        // Arrange
        var sut = CreateSUT();
        sut.gameObject.SetActive(true);
        // Act
        sut.Hide();

        // Assert
        Assert.IsFalse(sut.gameObject.activeSelf);
    }
    [Test]
    public void Test_IsShown_true()
    {
        //Given that the user changes view
        //When the system checks if the view is active
        //Then the function returns true.

        // Arrange
        var sut = CreateSUT();
        sut.gameObject.SetActive(true);

        // ACT & Assert
        Assert.True(sut.IsShown());
    }
    [Test]
    public void Test_IsShown_false()
    {
        //Given that the user changes view
        //When the system checks if the view is active
        //Then the function returns false.

        // Arrange
        var sut = CreateSUT();
        sut.gameObject.SetActive(false);

        // ACT & Assert
        Assert.IsFalse(sut.IsShown());
    }
    [Test]
    public void Test_GetRectTransform()
    {
        var gameObject = new GameObject();
        var child = gameObject.AddComponent<RectTransform>();
        var sut = CreateSUT(null, null, child);
        Assert.AreSame(child, sut.GetRectTransform());
    }
}