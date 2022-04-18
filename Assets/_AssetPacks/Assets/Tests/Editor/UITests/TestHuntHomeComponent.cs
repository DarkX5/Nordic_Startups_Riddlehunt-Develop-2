using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;
using riddlehouse_libraries.products;
using riddlehouse_libraries.products.models;
using riddlehouse_libraries.products.models.DTOs;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
// using UnityEngine.XR.ARSubsystems;
using Object = System.Object;

[TestFixture]
public class TestHuntHomeComponent : ZenjectUnitTestFixture
{
    //TODO: AR Element
    // public XRReferenceImageLibrary lib;

     [SetUp]
     public void Init()
     {
         //collects imagelibrary asset file from streaming assets; if null, then the file is missing.
         //TODO: AR Element
         // lib = Resources.Load<XRReferenceImageLibrary>("editor/testLibrary");
     }

    [Test]
    public void TestFactory_PrefabHasHuntHomeComponentBehaviour_Succeeds()
    {
        //Arrange
        GameObject gameObject = new GameObject();
        gameObject.AddComponent<HuntHomeComponentBehaviour>();
        //Assert & Act
        // sut;
        Assert.IsNotNull(HuntHomeComponent.Factory(gameObject));
    }

    [Test]
    public void TestFactoryNoHuntStartBehaviorOnGameObject_Throws()
    {
        // Given the gameobject has no ProductStartBehavior
        // When constructing ProductStartPanel
        // Then an exception is thrown
        
        //Arrange
        GameObject gameObject = new GameObject();

        //Assert & Act
        // sut;
        Assert.Throws<ArgumentException>(() => HuntHomeComponent.Factory(gameObject));
    }

    [Test]
    public void TestConfigure_HuntHomeIsConfigured()
    {
        //Given the user starts a new hunt.
        //When the HuntHome is created.
        //Then the HuntHome is configured.
        
        //Arrange
        var startPanelData = new StartPanelData() {HasAccess =  true, Id = "id", Title = "title"};

        var huntHomecomponentActionsMock = new Mock<IHuntHomeComponentActions>();
        huntHomecomponentActionsMock.Setup(x => x.Configure(startPanelData, It.IsAny<Action<bool>>())).Verifiable();
        
        var huntComponentUIActionsMock = new Mock<IViewActions>();
        //Act
        var sut = new HuntHomeComponent(huntHomecomponentActionsMock.Object, huntComponentUIActionsMock.Object);
        
        sut.Configure(startPanelData, It.IsAny<Action<bool>>(), It.IsAny<Action<bool>>());
        //Assert
        huntHomecomponentActionsMock.Verify(x => x.Configure(startPanelData, It.IsAny<Action<bool>>()));
    }
    
    
    [Test]
    public void TestConfigure_ReturnToAppHome_CallsAction()
    {
        //Given a huntHomeComponent
        //When returnToAppHome is called
        //Then the goBack action is invoked..
        
        //Arrange
        bool hasBeenCalled = false;
        Action<bool> goBack = (success) => { hasBeenCalled = true;};
        var startPanelData = new StartPanelData() {HasAccess =  true, Id = "id", Title = "title"};

        var huntHomecomponentActionsMock = new Mock<IHuntHomeComponentActions>();
        huntHomecomponentActionsMock.Setup(x => x.Configure(startPanelData, It.IsAny<Action<bool>>())).Verifiable();
        
        var huntComponentUIActionsMock = new Mock<IViewActions>();
        var sut = new HuntHomeComponent(huntHomecomponentActionsMock.Object, huntComponentUIActionsMock.Object);
        
        sut.Configure(startPanelData, (introVideoPrepared) =>
        {
        }, goBack);
        //Act
        sut.ReturnToAppHome();
        //Assert
        Assert.IsTrue(hasBeenCalled);
    }

    // [Test]
    // public void TestConfigureARElements_ARSession_IsStarted()
    // {
    //     //Given the user has started a huntStep
    //     //When the step needs ARScanning
    //     //Then the ARElements are configured.
    //     
    //     //Arrange 
    //     var huntStepMock = new Mock<IRecognizeImageAndPlayVideoHuntStep>();
    //     huntStepMock.Setup(x => x.GetImageLibraryReference()).Returns(lib).Verifiable();
    //     Action foundTarget = () => { };
    //     
    //     var morphableARCameraMock = new Mock<IMorphableARCameraActions>();
    //     var huntHomeComponentActionsMock = new Mock<IHuntHomeComponentActions>();
    //     huntHomeComponentActionsMock.Setup(
    //         x => x.ConfigureARElements(morphableARCameraMock.Object, null, foundTarget)).Verifiable();
    //     var huntComponentUIActionsMock = new Mock<IHuntComponentUIActions>();
    //     //Act
    //     var sut = new HuntHomeComponent(huntHomeComponentActionsMock.Object, huntComponentUIActionsMock.Object);
    //     
    //     sut.ConfigureARElements(morphableARCameraMock.Object, null, foundTarget);
    //
    //     //Assert
    //     huntHomeComponentActionsMock.Verify(
    //         x => x.ConfigureARElements(morphableARCameraMock.Object, null, foundTarget));
    // }
   

    [Test]
    public void TestGetHuntHomeComponentActions()
    {
        //Arrange 
        var huntHomecomponentActionsMock = new Mock<IHuntHomeComponentActions>();
        var huntComponentUIActionsMock = new Mock<IViewActions>();
        //Act
        var sut = new HuntHomeComponent(huntHomecomponentActionsMock.Object, huntComponentUIActionsMock.Object);
        
        //Assert
        Assert.AreSame(huntHomecomponentActionsMock.Object, sut.GetHuntHomeComponentActions());
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