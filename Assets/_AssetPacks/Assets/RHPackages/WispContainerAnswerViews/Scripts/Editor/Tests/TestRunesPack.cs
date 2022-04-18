using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestRunesPack
{
    [Test]
    public void TestGetRuneIcon_Returns_Sprite()
    {
        //Given a new RunesPack with 3 ids and 3 sprites
        //When GetRuneIcon is called with one of those ids
        //Then the corresponding sprite is returned.
        
        //Arrange
        var runesIds = new List<string>() { "black", "red", "blue" };

        var spriteBlack = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        var spriteRed = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        var spriteBlue = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);

        var runesIcons = new List<Sprite>()
        {
            spriteBlack,
            spriteRed,
            spriteBlue
        };

        var sut = new RunesPack(runesIds, runesIcons);

        //Act
        var sprite = sut.GetRuneIcon("black");
        //Assert
        Assert.AreEqual(spriteBlack, sprite);
    }
    
    [Test]
    public void TestGetRuneIcon_Unequal_List_Lengths_Throws()
    {
        //Given a new RunesPack with 2 ids and 3 sprites
        //When GetRuneIcon is called with one of those ids
        //Then the function recognizes that there is a misconfiguration and throws an error.
        
        //Arrange
        var runesIds = new List<string>() { "black", "red" };

        var spriteBlack = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        var spriteRed = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        var spriteBlue = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);

        var runesIcons = new List<Sprite>()
        {
            spriteBlack,
            spriteRed,
            spriteBlue
        };
        
        var sut = new RunesPack(runesIds, runesIcons);

        //Act & Assert
        Assert.Throws<ArgumentException>(() => sut.GetRuneIcon("black"));
    }
    
    [Test]
    public void TestGetRuneIcon_No_Such_Rune_Throws()
    {
        //Given a new RunesPack with 1 ids and 1 sprites
        //When GetRuneIcon is called with an id that isn't in the runesIds list.
        //Then the function recognizes that there is a misconfiguration and throws an error.
        
        //Arrange
        var runesIds = new List<string>() { "black"};

        var spriteBlack = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);

        var runesIcons = new List<Sprite>()
        {
            spriteBlack,
        };

        var sut = new RunesPack(runesIds, runesIcons);

        //Act & Assert
        Assert.Throws<ArgumentException>(() => sut.GetRuneIcon("red"));
    }

    [Test]
    public void GetUniqueRuneCount()
    {
        //Arrange
        var runesIds = new List<string>() { "black"};

        var spriteBlack = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);

        var runesIcons = new List<Sprite>()
        {
            spriteBlack,
        };

        var sut = new RunesPack(runesIds, runesIcons);
        
        //Act & Assert
        Assert.AreEqual(1, sut.GetUniqueRuneCount());
    }
}
