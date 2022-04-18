using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class TestImageFullscreenCanvasBehaviour
{
    private GameObject prefab;
    
    [SetUp]
    public void Init()
    {
        prefab = new GameObject();
    }

    [TearDown]
    public void TearDown()
    {
        prefab = null;
    }
    
    [Test]
    public void TestFactory_Has_ImageFullscreenCanvasBehaviour_Succeeds()
    {
        //Given a prefab that has an ImageFullscreenCanvasBehaviour connected
        //When the factory is called with that prefab object
        //Then the factory returns a newly created ImageFullscreenCanvasBehaviour
        
        //Arrange
        prefab.AddComponent<ImageFullscreenCanvasBehaviour>();
        //Act & Assert
        Assert.DoesNotThrow(() => ImageFullscreenCanvasBehaviour.Factory(prefab));
    }

    [Test]
    public void TestFactory_DoesNotHave_ImageFullscreenCanvasBehaviour_Throws()
    {
        //Given a prefab that has an ImageFullscreenCanvasBehaviour connected
        //When the factory is called with that prefab object
        //Then the factory returns a newly created ImageFullscreenCanvasBehaviour
        
        //Act & Assert
        Assert.Throws<ArgumentException>(() => ImageFullscreenCanvasBehaviour.Factory(prefab));
    }

    [Test]
    public void TestDisplayImageFullScreen()
    {
        //Given an instance of the FullscreenCanvasBehaviour
        //When the Display funciton is called
        //Then the object is set active
        
        //Arrange
        var sut = prefab.AddComponent<ImageFullscreenCanvasBehaviour>();
        Image img = prefab.AddComponent<Image>();
        var sprite = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        sut.SetDependencies(img);
        //Act
        sut.DisplayImageFullScreen(sprite);
        //Assert
        Assert.IsTrue(sut.gameObject.activeSelf);
    }
    [Test]
    public void TestHide()
    {
        //Given an instance of the FullscreenCanvasBehaviour
        //When the hide funciton is called
        //Then the object is set inactive
        
        //Arrange
        var sut = prefab.AddComponent<ImageFullscreenCanvasBehaviour>();
        //Act
        sut.Hide();
        //Assert
        Assert.IsFalse(sut.gameObject.activeSelf);
    }
}
