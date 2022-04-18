using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products.models;
using UnityEditor;
using UnityEngine;

public class TestCanvasController
{
    CanvasController.Dependencies CreateDependencies(GameObject go, Mock<ICanvasLayerManager> clmMock = null)
    {
        clmMock ??= new Mock<ICanvasLayerManager>();
        return new CanvasController.Dependencies()
        {
            Cv = go.GetComponent<Canvas>(),
            Cg = go.GetComponent<CanvasGroup>(),
            Clm = clmMock.Object
        };
    }
    
    [Test]
    public void TestSetDependencies()
    {
        var go = new GameObject();
        var sut = go.AddComponent<CanvasController>();
        var dependencies = CreateDependencies(go);
        sut.SetDependencies(dependencies);
        
        Assert.AreEqual(dependencies, sut._dependencies);
    }
    
    [Test]
    public void TestConfigure()
    {
        //Arrange
        var go = new GameObject();
        var sut = go.AddComponent<CanvasController>();
        
        var clmMock = new Mock<ICanvasLayerManager>();
        clmMock.Setup(x => x.RegisterCanvas(CanvasLayerTypeNames.none, sut));
        
        var dependencies = CreateDependencies(go, clmMock);
        sut.SetDependencies(dependencies);
        
        //Act
        sut.Configure();
        //Assert
        clmMock.Verify(x => x.RegisterCanvas(CanvasLayerTypeNames.none, sut));
        Assert.AreEqual(dependencies.Cv.renderMode, RenderMode.ScreenSpaceOverlay);

    }
    
    [Test]
    public void TestConfigure_HasCameraType()
    {
        //Arrange
        var go = new GameObject();
        var sut = go.AddComponent<CanvasController>();
        
        var clmMock = new Mock<ICanvasLayerManager>();
        clmMock.Setup(x => x.RegisterCanvas(CanvasLayerTypeNames.none, sut));
        
        var dependencies = CreateDependencies(go, clmMock);
        sut.SetDependencies(dependencies);
        var camera = new GameObject().AddComponent<Camera>();
        //Act
        sut.Configure(new CanvasController.Config()
        {
            ViewCamera = camera
        });
        //Assert
        clmMock.Verify(x => x.RegisterCanvas(CanvasLayerTypeNames.none, sut));
        Assert.AreEqual(RenderMode.ScreenSpaceCamera, dependencies.Cv.renderMode);
        Assert.AreEqual( camera, dependencies.Cv.worldCamera);
    }
    
    [Test]
    public void TestSetLayerOrder()
    {
        //Arrange
        var go = new GameObject();
        var sut = go.AddComponent<CanvasController>();
        var dependencies = CreateDependencies(go);
        sut.SetDependencies(dependencies);
        var order = 5;
        
        //Act
        sut.SetLayerOrder(5);
        
        //Assert
        Assert.AreEqual(order, dependencies.Cv.sortingOrder);
    }
    
    [TestCase(true)]
    [TestCase(false)]
    [Test]
    public void TestSetInteractable(bool interactable)
    {
        //Arrange
        var go = new GameObject();
        var sut = go.AddComponent<CanvasController>();
        var dependencies = CreateDependencies(go);
        sut.SetDependencies(dependencies);
        
        //Act
        sut.SetInteractable(interactable);
        
        //Assert
        Assert.AreEqual(interactable, dependencies.Cg.interactable);
        Assert.AreEqual(interactable, dependencies.Cg.blocksRaycasts);
    }
    
    [TestCase(true)]
    [TestCase(false)]
    [Test]
    public void TestSetActive(bool active)
    {
        //Arrange
        var go = new GameObject();
        go.SetActive(!active);
        var sut = go.AddComponent<CanvasController>();
        var dependencies = CreateDependencies(go);
        sut.SetDependencies(dependencies);
        
        //Act
        sut.SetActive(active);
        
        //Assert
        Assert.AreEqual(active, go.activeSelf);
    }
    [Test]
    public void TestDestroySelf()
    {
        //Given a new CanvasController of type None
        //When DestroySelf is called
        //Then it unregisters itself with the CLM.
        
        //Arrange
        var go = new GameObject();
        var sut = go.AddComponent<CanvasController>();

        var clmMock = new Mock<ICanvasLayerManager>();
        
        clmMock.Setup(x => x.UnregisterCanvas(CanvasLayerTypeNames.none)).Verifiable();
        
        var dependencies = CreateDependencies(go, clmMock);
        sut.SetDependencies(dependencies);
        
        //Act
        sut.DestroySelf();
        
        //Assert
        clmMock.Verify(x => x.UnregisterCanvas(CanvasLayerTypeNames.none));
    }
}
