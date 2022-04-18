using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;
using Riddlehouse.Core.Helpers.Helpers;
using UnityEngine;

public class TestBaseFullscreenView
{
    [Test]
    public void TestFitInView_With_Index()
    {
        //Given a BaseFullscreenView
        //When FitInView is called with a desired sibblingIndex
        //Then it is parented, fullscreen fitted, and set to the desired sibblingIndex
        
        //Arrange
        var parent = new GameObject().AddComponent<RectTransform>();
        var otherChild = new GameObject().AddComponent<RectTransform>();
        otherChild.SetParent(parent);
        
        var sut = new GameObject().AddComponent<BaseFullscreenView>();
        sut.gameObject.AddComponent<RectTransform>();

        var uiFittersMock = new Mock<IUIFitters>();
        uiFittersMock.Setup(x => x.FitToFullscreen(sut.GetRectTransform(), parent)).Verifiable();
        //Act
        sut.FitInView(parent, uiFittersMock.Object, 0);
        //Assert
        uiFittersMock.Verify(x => x.FitToFullscreen(sut.GetRectTransform(), parent));
        Assert.AreEqual(0, sut.transform.GetSiblingIndex());
    }

    [Test]
    public void TestFitInView()
    {
        //Given a BaseFullscreenView
        //When FitInView is called
        //Then it is parented and fullscreen fitted
        
        //Arrange
        var parent = new GameObject().AddComponent<RectTransform>();
        var sut = new GameObject().AddComponent<BaseFullscreenView>();
        sut.gameObject.AddComponent<RectTransform>();
        var uiFittersMock = new Mock<IUIFitters>();
        uiFittersMock.Setup(x => x.FitToFullscreen(sut.GetRectTransform(), parent)).Verifiable();
        //Act
        sut.FitInView(parent, uiFittersMock.Object);
        //Assert        
        uiFittersMock.Verify(x => x.FitToFullscreen(sut.GetRectTransform(), parent));
    }

    [Test]
    public void TestDisplay()
    {
        GameObject go = new GameObject();

        var sut = go.AddComponent<BaseFullscreenView>();
        //Pre assertion
        go.SetActive(false);
        Assert.IsFalse(go.activeSelf);
        //Act
        sut.Display();
        //Post assertion
        Assert.IsTrue(go.activeSelf);
    }
    
    [Test]
    public void TestHide()
    {
        //Arrange
        var sut = new GameObject().AddComponent<BaseFullscreenView>();
        //Pre assertion
        Assert.IsTrue(sut.gameObject.activeSelf);
        //Act
        sut.Hide();
        //Post assertion
        Assert.IsFalse(sut.gameObject.activeSelf);
    }

    [Test]
    public void TestIsShown()
    {
        //Arrange
        var sut = new GameObject().AddComponent<BaseFullscreenView>();
        
        //Pre assertion
        Assert.IsTrue(sut.gameObject.activeSelf);
        Assert.AreEqual(sut.gameObject.activeSelf, sut.IsShown());
        
        //Act
        sut.Hide();
        
        //Post assertion
        Assert.IsFalse(sut.gameObject.activeSelf);
        Assert.AreEqual(sut.gameObject.activeSelf, sut.IsShown());
    }

    [Test]
    public void TestGetComponentType()
    {
        //Arrange
        var sut = new GameObject().AddComponent<BaseFullscreenView>();
        //Act & Assert
        Assert.AreEqual(ComponentType.FullscreenView, sut.GetComponentType());
    }

    [Test]
    public void TestGetRectTransform()
    {
        //Given a BaseFullscreenView
        //When GetRectTransform is called
        //Then is returns the component on the gameobject.
        
        //Arrange
        var sut = new GameObject().AddComponent<BaseFullscreenView>();
        var rectTransform = sut.gameObject.GetComponent<RectTransform>();
        //Act & assert
        Assert.AreEqual(rectTransform, sut.GetRectTransform());
    }
}
