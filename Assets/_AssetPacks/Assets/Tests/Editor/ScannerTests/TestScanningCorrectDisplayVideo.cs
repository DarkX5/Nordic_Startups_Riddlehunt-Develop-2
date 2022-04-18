using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class TestScanningCorrectDisplayVideo
{
    [Test]
    public void TestFactory_Succeeds()
    {
        //Given a prefab to instantiate from
        //When factory is called
        //Creates and configures the UI.
        var parentGo = new GameObject();
        var parent = parentGo.AddComponent<RectTransform>();
        
        var gameObject = new GameObject();
        gameObject.AddComponent<RectTransform>();
        gameObject.AddComponent<ScanningCorrectDisplayVideoHuntComponentBehaviour>();
        
        var didCatch = false;

        var sut = ScanningCorrectDisplayVideoComponent.Factory(gameObject);
        
        Assert.IsTrue((didCatch == false));
        Assert.IsNotNull(sut);
    }
    
    [Test]
    public void TestConfigure()
    {
        string expectedVideoUrl = "video URL";
        string buttonText = "buttonText";
        Action buttonAction = () => {};

        var scanningCorrectDisplayVideoActionsMock = new Mock<IScanningCorrectDisplayVideoActions>();
        scanningCorrectDisplayVideoActionsMock.Setup(x => x.Configure(expectedVideoUrl, buttonText, buttonAction)).Verifiable();

        var huntComponentUIActionsMock = new Mock<IViewActions>();
        
        var sut = new ScanningCorrectDisplayVideoComponent(scanningCorrectDisplayVideoActionsMock.Object, huntComponentUIActionsMock.Object);
        
        sut.Configure(expectedVideoUrl, buttonText, buttonAction);
        scanningCorrectDisplayVideoActionsMock.Verify(x => x.Configure(expectedVideoUrl, buttonText, buttonAction));
    }
    [Test]
    public void TestGetHuntHomeComponentActions()
    {
        //Arrange 
        var scanningCorrectDisplayVideoActionsMock = new Mock<IScanningCorrectDisplayVideoActions>();
        var huntComponentUIActionsMock = new Mock<IViewActions>();
        //Act
        var sut = new ScanningCorrectDisplayVideoComponent(scanningCorrectDisplayVideoActionsMock.Object, huntComponentUIActionsMock.Object);
        
        //Assert
        Assert.AreSame(scanningCorrectDisplayVideoActionsMock.Object, sut.GetScanningCorrectDisplayComponentActions());
    }
    [Test]
    public void TestGetComponentUIActions()
    {
        //Arrange 
        var huntHomecomponentActionsMock = new Mock<IHuntHomeComponentActions>();
        var huntComponentUIActionsMock = new Mock<IViewActions>();
        //Act
        var sut = new HuntHomeComponent(huntHomecomponentActionsMock.Object, huntComponentUIActionsMock.Object);
        
        //Assert
        Assert.AreSame(huntComponentUIActionsMock.Object, sut.GetComponentUIActions());
    }
}