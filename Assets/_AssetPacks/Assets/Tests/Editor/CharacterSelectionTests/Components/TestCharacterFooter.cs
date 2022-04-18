using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Prefabs.UI.CharacterSelection.StyledCharacterSelection.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class TestCharacterFooter 
{
    private Footer.Dependencies CreateDependencies(Mock<ICharacterIconDisplay> characterIconDisplay = null)
    {
        characterIconDisplay ??= new Mock<ICharacterIconDisplay>();
        return new Footer.Dependencies()
        {
            CharacterIconDisplay = characterIconDisplay.Object
        };
    }

    [Test]
    public void TestSetDependencies()
    {
        //Arrange
        var sut = new GameObject().AddComponent<Footer>();
        var dependencies = CreateDependencies();
        //Act
        sut.SetDependencies(dependencies);
        //Assert
        Assert.AreEqual(dependencies, sut._dependencies);
    }

    [Test]
    public void TestConfigure()
    {
        //Arrange
        var icon = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        var color = Color.black;
        
        var sut = new GameObject().AddComponent<Footer>();
        var characterIconDisplayMock = new Mock<ICharacterIconDisplay>();
        characterIconDisplayMock.Setup(x => x.Configure(color, icon)).Verifiable();
        var dependencies = CreateDependencies(characterIconDisplayMock);
        sut.SetDependencies(dependencies);

        //Act
        sut.Configure(new Footer.Config()
        {
            Icon = icon,
            IconFrame = color
        });
        
        //Assert
        characterIconDisplayMock.Verify(x => x.Configure(color, icon));

    }

    [Test]
    public void TestApprove()
    {
        //Arrange
        var sut = new GameObject().AddComponent<Footer>();
        var dependencies = CreateDependencies();
        sut.SetDependencies(dependencies);

        var icon = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        var color = Color.black;

        bool hasBeenCalled = false;
        Action approve = () =>
        {
            hasBeenCalled = true;
        };

        sut.Configure(new Footer.Config()
        {
            Icon = icon,
            IconFrame = color,
            Approve = approve
        });
        //Act
        sut.Approve();
        
        //Assert
        Assert.IsTrue(hasBeenCalled);
    }
    
    [Test]
    public void TestAbort()
    {
        //Arrange
        var sut = new GameObject().AddComponent<Footer>();
        var dependencies = CreateDependencies();
        sut.SetDependencies(dependencies);

        var icon = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        var color = Color.black;

        bool hasBeenCalled = false;
        Action abort = () =>
        {
            hasBeenCalled = true;
        };

        sut.Configure(new Footer.Config()
        {
            Icon = icon,
            IconFrame = color,
            Abort = abort
        });
        //Act
        sut.Abort();
        
        //Assert
        Assert.IsTrue(hasBeenCalled);
    }
}
