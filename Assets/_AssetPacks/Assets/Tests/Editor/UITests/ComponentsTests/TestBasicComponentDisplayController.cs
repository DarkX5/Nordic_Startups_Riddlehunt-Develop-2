using System.Collections;
using System.Collections.Generic;
using Riddlehouse.Core.Helpers.Helpers;
using Moq;
using NUnit.Framework;
using UnityEngine;

public class TestBasicComponentDisplayController
{
    [Test]
    public void SetDependencies()
    {
        var sut = new GameObject().AddComponent<BasicComponentDisplayController>();
        var uiFitterMock = new Mock<IUIFitters>();
        var dependencies = new BasicComponentDisplayController.Dependencies()
        {
          UIFitter = uiFitterMock.Object
        };
        sut.SetDependencies(dependencies);
        sut.gameObject.SetActive(false);
        sut.Display();
        Assert.AreEqual(dependencies, sut._dependencies);
    }
    [Test]
    public void TestFitToScreen()
    {
        //Arrange
        var parent = new GameObject().AddComponent<RectTransform>();
        var sut = new GameObject().AddComponent<BasicComponentDisplayController>();
        var uiFitterMock = new Mock<IUIFitters>();
        uiFitterMock.Setup(x => x.FitToFullscreen((RectTransform)sut.transform, parent)).Verifiable();
        var dependencies = new BasicComponentDisplayController.Dependencies()
        {
            UIFitter = uiFitterMock.Object
        };
        sut.SetDependencies(dependencies);
        //Act
        sut.FitToScreen(parent);
        //Assert
        uiFitterMock.Verify(x => x.FitToFullscreen((RectTransform)sut.transform, parent));
    }
    [Test]
    public void TestDisplay()
    {
     var sut = new GameObject().AddComponent<BasicComponentDisplayController>();
     sut.gameObject.SetActive(false);
     sut.Display();
     Assert.IsTrue(sut.gameObject.activeSelf);
    }

    [Test]
    public void TestHide()
    {
     var sut = new GameObject().AddComponent<BasicComponentDisplayController>();
     sut.gameObject.SetActive(true);
     sut.Hide();
     Assert.IsFalse(sut.gameObject.activeSelf);
    } 
    [TestCase(false)]
    [TestCase(true)]
    [Test]
    public void TestDisplay(bool active)
    {
     var sut = new GameObject().AddComponent<BasicComponentDisplayController>();
     sut.gameObject.SetActive(active);
     var shown = sut.IsShown();
     Assert.AreEqual(active, shown);
    }
}
