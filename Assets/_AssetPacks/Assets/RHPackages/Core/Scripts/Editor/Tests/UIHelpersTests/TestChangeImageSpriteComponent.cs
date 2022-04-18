using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestChangeImageSpriteComponent 
{
    [Test]
    public void TestConfigure()
    {
        //Given a ChangeImageSpriteComponent
        //When Configure is called with an icon
        //Then the image sprite changes to that icon.
        
        //Arrange
        var icon = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        var sut = new GameObject().AddComponent<ChangeImageSpriteComponent>();
        //Act
        sut.Configure(icon);
        //Assert
        Assert.IsNotNull(sut._imageComponent);
        Assert.AreEqual(icon, sut._imageComponent.sprite);
    }
}
