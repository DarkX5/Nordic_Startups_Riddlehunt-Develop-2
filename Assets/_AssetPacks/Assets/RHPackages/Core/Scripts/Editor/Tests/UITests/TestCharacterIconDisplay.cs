using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestCharacterIconDisplay
{
    private CharacterIconDisplay.Dependencies CreateDependencies()
    {
        return new CharacterIconDisplay.Dependencies()
        {
            SpriteUpdater = new GameObject().AddComponent<ChangeImageSpriteComponent>(),
            ColorUpdater = new GameObject().AddComponent<SetImageColorComponent>()
        };
    }
    
    [Test]
    public void TestSetDependencies()
    {
        //Arrange
        var sut = new GameObject().AddComponent<CharacterIconDisplay>();
        var dependencies = CreateDependencies();
        //Act
        sut.SetDependencies(dependencies);
        //Assert
        Assert.AreEqual(dependencies, sut._dependencies);
    }

    [Test]
    public void TestConfigure()
    {
        //Given a new CharacterIconDisplay, an icon and a color
        //When Configure is called with those values
        //Then the framecolor is set to red, and the icon is set to icon.

        //Arrange
        var sut = new GameObject().AddComponent<CharacterIconDisplay>();
        var dependencies = CreateDependencies();
        sut.SetDependencies(dependencies);

        var icon = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        var color = Color.red;
        //Act
        sut.Configure(color, icon);
        //Assert
        Assert.AreEqual(color, sut._dependencies.ColorUpdater._imageComponent.color);
        Assert.AreEqual(icon, sut._dependencies.SpriteUpdater._imageComponent.sprite);
    }
}
