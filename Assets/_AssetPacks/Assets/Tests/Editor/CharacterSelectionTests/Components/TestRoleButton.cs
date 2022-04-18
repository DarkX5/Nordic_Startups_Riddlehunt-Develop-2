using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using CharacterSelection.Components;
using Components.Buttons;
using Helpers;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products;
using riddlehouse_libraries.products.huntProduct;
using riddlehouse_libraries.products.resources;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class TestRoleButton
{
    private RoleButton.Config CreateRoleButtonConfig(Mock<IIcon> iconMock = null)
    {
        if (iconMock == null)
        {
            Random rnd = new Random();
            var icon = new Byte[10];
            rnd.NextBytes(icon);

            iconMock = new Mock<IIcon>();
            iconMock.Setup(x => x.GetIcon(It.IsAny<HuntProductCacheConfig>())).ReturnsAsync(icon);
        }
        return new RoleButton.Config()
        {
            Resource = new RoleButtonResource(iconMock.Object)
            {
                Title = "buttonTitle"
            }
        };
    }

    private RoleButton.Dependencies CreateDependencies(Mock<ISpriteHelper> spriteHelperMock = null)
    {
        spriteHelperMock ??= new Mock<ISpriteHelper>();
        return new RoleButton.Dependencies()
        {
            SpriteHelper = spriteHelperMock.Object,
            ButtonIcon = new GameObject().AddComponent<Image>()
        };
    }
    
    [Test]
    public void TestSetDependencies()
    {
        //Arrange
        var sut = new GameObject().AddComponent<RoleButton>();
        var dependencies = CreateDependencies();
        //Act
        sut.SetDependencies(dependencies);
        //Assert
        Assert.AreSame(dependencies, sut._dependencies);
    }

    [Test]
    public void TestSetConfigure_SetsConfigurableVariables()
    {
        //Given a new and initialized RoleButton
        //When Configure is called with a sprite and a title
        //Then that sprite is set within the associated image.
        
        //Arrange
        var sprite = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);

        var iconArr = new byte[3] {55, 55, 55};
        var iconMock = new Mock<IIcon>();
        iconMock.Setup(x => x.GetIcon(It.IsAny<HuntProductCacheConfig>())).ReturnsAsync(iconArr).Verifiable();

        var config = new RoleButton.Config()
        {
            Resource = new RoleButtonResource(iconMock.Object)
            {
                Title = "buttonTitle"
            }
        };
        
        var sut = new GameObject().AddComponent<RoleButton>();

        var spriteHelperMock = new Mock<ISpriteHelper>();
        spriteHelperMock.Setup(x => x.GetSpriteFromByteArray(iconArr)).Returns(sprite).Verifiable();
        var dependencies = CreateDependencies(spriteHelperMock);
        sut.SetDependencies(dependencies);

        //Act
        sut.Configure(config);
        
        //Assert
        Assert.AreSame(sprite, sut._dependencies.ButtonIcon.sprite);
        iconMock.Verify( x => x.GetIcon(It.IsAny<HuntProductCacheConfig>()));
        spriteHelperMock.Verify(x => x.GetSpriteFromByteArray(iconArr));

    }
    
    [Test]
    public void TestPerformAction_Invokes_ConfiguredAction()
    {
        //Given a configured RoleButton
        //When PerformAction is called
        //Then the configured action is invoked with the button title as payload.
        
        //Arrange
        var icon = new GameObject().AddComponent<Image>();
        var sut = new GameObject().AddComponent<RoleButton>();
        
        sut.SetDependencies(new RoleButton.Dependencies()
        {
            ButtonIcon = icon
        });

        var config = CreateRoleButtonConfig();
        
        string actionMessage = "notCalled";
        Action<string> buttonAction = (msg) =>
        {
            actionMessage = msg;
        };
        config.buttonAction = buttonAction;
        
        sut.Configure(config);
        
        //Act
        sut.PerformAction();
        //Assert
        Assert.AreEqual(config.Resource.Title, actionMessage);
    }
    
    [Test]
    public void TestDestroySelf()
    {
        //Arrange
        var sutGo = new GameObject();
        var icon = sutGo.AddComponent<Image>();
        var sut = sutGo.AddComponent<RoleButton>();
        sut.SetDependencies(new RoleButton.Dependencies()
        {
            ButtonIcon = icon
        });
        //Act
        sut.DestroySelf();
        //Assert
        Assert.IsTrue(sut == null);
        Assert.IsTrue(icon == null);
    }
}
