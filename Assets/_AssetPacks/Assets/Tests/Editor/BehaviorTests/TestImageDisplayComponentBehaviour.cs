using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class TestImageDisplayComponentBehaviour
{
    private GameObject prefab;
    private RectTransform parent;
    
    [SetUp]
    public void Init()
    {
        prefab = new GameObject();
        var parentObj = new GameObject();
        parent = parentObj.AddComponent<RectTransform>();
    }

    [TearDown]
    public void TearDown()
    {
        prefab = null;
        parent = null;
    }
    [Test]
    public void TestFactory_Has_ImageDisplayComponentBehaviour_Succeeds()
    {
        //Given a prefab that has an ImageDisplayComponentBehaviour connected
        //When the factory is called with that prefab object
        //Then the factory returns a newly created ImageDisplayComponentBehaviour
        
        //Arrange
        prefab.AddComponent<ImageDisplayComponentBehaviour>();
        //Act & Assert
        Assert.IsNotNull(ImageDisplayComponentBehaviour.Factory(prefab, parent));
    }

    [Test]
    public void TestFactory_DoesNotHave_ImageDisplayComponentBehaviour_Throws()
    {
        //Given a prefab that has an ImageDisplayComponentBehaviour connected
        //When the factory is called with that prefab object
        //Then the factory returns a newly created ImageDisplayComponentBehaviour
        
        //Act & Assert
        Assert.Throws<ArgumentException>(() => ImageDisplayComponentBehaviour.Factory(prefab, parent));
    }

    [Test]
    public void TestConfigure_SetsSpriteInImage_SetsActionToInvoke_SetsGameObjectActive()
    {
        //Given an ImageDisplayComponentBehaviour object, a sprite to display and a fullscreen action to invoke
        //When configure is called
        //Then the sprite is set, the object is active, and the action can be invoked without a null error.
        
        //Arrange
        Image img = prefab.AddComponent<Image>();
        
        var sprite = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        Sprite returnedSprite = null;
        Action<Sprite> action = (image) => { returnedSprite = image; };
        var sut = prefab.AddComponent<ImageDisplayComponentBehaviour>();
        
        sut.SetDependencies(img);
        
        //act
        sut.Configure(sprite, action);
        
        //Assert
        Assert.DoesNotThrow(() => sut.Pressed()); //pressed is tested implicitly here.
        Assert.IsTrue(sut.gameObject.activeSelf);
        Assert.AreSame(sprite,returnedSprite);
    }

    [Test]
    public void TestHide_SetsObject_Inactive()
    {
        //Arrange
        var sut = prefab.AddComponent<ImageDisplayComponentBehaviour>();

        //Act
        sut.Hide();
        
        //Assert
        Assert.IsFalse(sut.gameObject.activeSelf);

    }
}
