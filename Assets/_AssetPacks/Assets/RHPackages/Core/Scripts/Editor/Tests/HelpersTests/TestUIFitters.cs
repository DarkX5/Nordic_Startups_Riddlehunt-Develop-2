using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Riddlehouse.Core.Helpers.Helpers;
using UnityEngine;

public class TestUIFitters
{
    [Test]
    public void TestFitToFullScreen_AttachesToParent()
    {
        //Given a new UIFitter
        //When FitToFullScreen is called with a child and a parent
        //Then the child is parented and set to fullscreen settings
        
        //Arrange
        var sut = new UIFitters();
        var child = new GameObject().AddComponent<RectTransform>();
        var parent = new GameObject().AddComponent<RectTransform>();
        
        //Act
        sut.FitToFullscreen(child, parent);
        
        //Assert
        Assert.AreEqual(parent, child.parent);
        Assert.AreEqual(child.localPosition, Vector3.zero);
        Assert.AreEqual(child.localScale, Vector3.one);
        Assert.AreEqual(child.anchorMin, Vector2.zero);
        Assert.AreEqual(child.anchorMax, Vector2.one);
        Assert.AreEqual(child.offsetMin, Vector2.zero);
        Assert.AreEqual(child.offsetMax, Vector2.zero);

    }
    
    [Test]
    public void TestFitToFullScreen_MissingParent_Throws()
    {
        //Given a new UIFitter
        //When FitToFullScreen is called with a child but no parent
        //Then the function throws an error

        //Arrange
        var sut = new UIFitters();
        var child = new GameObject().AddComponent<RectTransform>();
        //Act and Assert
        Assert.Throws<ArgumentException>(() => sut.FitToFullscreen(child, null));
    }
    
    [Test]
    public void TestFitToFullScreen_MissingChildRectTransform_Throws()
    {
        //Given a new UIFitter
        //When FitToFullScreen is called with parent, but no child.
        //Then the function throws an error
        
        //Arrange
        var sut = new UIFitters();
        var parent = new GameObject().AddComponent<RectTransform>();
        //Act and Assert
        Assert.Throws<ArgumentException>(() => sut.FitToFullscreen(null, parent));
    }

    [Test]
    public void TestFitToGlobalView_No_Parent()
    {
        //Given a new UIFitter
        //When FitToGlobalView is called with a child
        //Then the child is parented globally and set to fullscreen settings
        
        //Arrange
        var sut = new UIFitters();
        var child = new GameObject().AddComponent<RectTransform>();
        //Act
        sut.FitToGlobalView(child);
        //Assert
        Assert.AreEqual(null, child.parent);
        Assert.AreEqual(child.localPosition, Vector3.zero);
        Assert.AreEqual(child.localScale, Vector3.one);
        Assert.AreEqual(child.anchorMin, Vector2.zero);
        Assert.AreEqual(child.anchorMax, Vector2.one);
        Assert.AreEqual(child.offsetMin, Vector2.zero);
        Assert.AreEqual(child.offsetMax, Vector2.zero);

    }
    [Test]
    public void TestFitToGlobalView_MissingChildRectTransform_Throws()
    {
        //Given a new UIFitter
        //When FitToGlobalView is called with no child
        //Then the function throws an error

        //Arrange
        var sut = new UIFitters();
        
        //Act & Assert
        Assert.Throws<ArgumentException>(() => sut.FitToGlobalView(null));
    }
}
