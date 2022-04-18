using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using Moq;
using NUnit.Framework;
using Prefabs.UI.CharacterSelection.StyledCharacterSelection.Scripts;
using riddlehouse_libraries.products.huntProduct;
using riddlehouse_libraries.products.models;
using riddlehouse_libraries.products.resources;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[TestFixture]
public class TestPlayerInformationView
{
    PlayerInformationView.Dependencies CreateDependencies(
        Mock<IComponentDisplayController> displayController = null, 
        Mock<IFooter> characterFooter = null,
        Mock<ISpriteHelper> spriteHelper = null)
    {
        displayController ??= new Mock<IComponentDisplayController>();
        characterFooter ??= new Mock<IFooter>();
        if (spriteHelper == null)
        {
            spriteHelper = new Mock<ISpriteHelper>();
            var icon = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
            spriteHelper.Setup(x => x.GetSpriteFromByteArray(It.IsAny<Byte[]>())).Returns(icon);
        }
       
        return new PlayerInformationView.Dependencies()
        {
            SpriteHelper = spriteHelper.Object,
            DisplayController = displayController.Object,
            Footer = characterFooter.Object,
            AgeTextInput = new GameObject().AddComponent<TMP_InputField>(),
            NameTextInput = new GameObject().AddComponent<TMP_InputField>(),
            CharacterTitleField = new GameObject().AddComponent<TextMeshProUGUI>(),
            CharacterDescriptionField = new GameObject().AddComponent<TextMeshProUGUI>(),
            CharacterAvatar = new GameObject().AddComponent<Image>()
        };
    }

    private PlayerInformationView.Config _config;
    
    [SetUp]
    public void Init()
    {
        _config = new PlayerInformationView.Config();
    }

    [TearDown]
    public void TearDown()
    {
        _config = null;
    }
    [Test]
    public void TestSetDependencies()
    {
        //Arrange
        var sut = new GameObject().AddComponent<PlayerInformationView>();
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
        var characterFooter = new Mock<IFooter>();
        characterFooter.Setup(x => x.Configure(It.IsAny<Footer.Config>())).Verifiable();
        var sut = new GameObject().AddComponent<PlayerInformationView>();
        
        var rawIcon = new Byte[] { 55, 66 };
        var iconSprite = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        var icon = new Mock<IIcon>();
        icon.Setup(x => x.GetIcon(It.IsAny<HuntProductCacheConfig>())).ReturnsAsync(rawIcon);
        
        var rawIconImage = new Byte[] { 66, 77 };
        var iconImageSprite = Sprite.Create(Texture2D.redTexture, Rect.zero, Vector2.down);
        var iconImage = new Mock<IIcon>();
        iconImage.Setup(x => x.GetIcon(It.IsAny<HuntProductCacheConfig>())).ReturnsAsync(rawIconImage);
        
        var spriteHelper = new Mock<ISpriteHelper>();
        spriteHelper.Setup(x => x.GetSpriteFromByteArray(rawIconImage)).Returns(iconImageSprite);
        spriteHelper.Setup(x => x.GetSpriteFromByteArray(rawIcon)).Returns(iconSprite);
        
        var dependencies = CreateDependencies(null, characterFooter, spriteHelper);
        sut.SetDependencies(dependencies);

        _config.characterOption = new CharacterOption(iconImage.Object, icon.Object)
        {
            RoleColor = new RHColor(55,55,55,55),
            Id = "A",
            CharacterDescription = "description",
            CharacterName = "name"
        };
        _config.PreviousOutput = new PlayerInformationView.Output()
        {
            Name = "name",
            Age = "17"
        };
        //Act
        sut.Configure(_config);
        
        //Assert
        characterFooter.Verify(x => x.Configure(It.IsAny<Footer.Config>()));
        Assert.AreEqual(_config.PreviousOutput.Name, sut._dependencies.NameTextInput.text);
        Assert.AreEqual(_config.PreviousOutput.Age, sut._dependencies.AgeTextInput.text);
        Assert.AreEqual(_config.characterOption.CharacterName, sut._dependencies.CharacterTitleField.text);
        Assert.AreEqual(_config.characterOption.CharacterDescription, sut._dependencies.CharacterDescriptionField.text);
        Assert.AreEqual(iconImageSprite, sut._dependencies.CharacterAvatar.sprite);
    }
    
    [Test]
    public void TestApprove()
    {
        var sut = new GameObject().AddComponent<PlayerInformationView>();
        var dependencies = CreateDependencies();
        sut.SetDependencies(dependencies);
        PlayerInformationView.Output outputMsg = null;
        
        var icon = new Mock<IIcon>();
        icon.Setup(x => x.GetIcon(It.IsAny<HuntProductCacheConfig>())).ReturnsAsync(new Byte[] { 55, 66 });
        var iconImage = new Mock<IIcon>();
        iconImage.Setup(x => x.GetIcon(It.IsAny<HuntProductCacheConfig>())).ReturnsAsync(new Byte[] { 66, 77 });

        _config.characterOption = new CharacterOption(iconImage.Object, icon.Object)
        {
            RoleColor = new RHColor(55,55,55,55),
            Id = "A",
            CharacterDescription = "description",
            CharacterName = "name"
        };
        _config.PreviousOutput = new PlayerInformationView.Output()
        {
            Name = "name",
            Age = "17"
        };
        _config.Approve = (output) =>
        {
            outputMsg = output;
        };
        sut.Configure(_config);
        //Act
        sut.Approve();
        //Assert
        Assert.AreEqual(sut._dependencies.NameTextInput.text, outputMsg.Name);
        Assert.AreEqual(sut._dependencies.AgeTextInput.text, outputMsg.Age);

    }
    [Test]
    public void TestNameTextUpdated_AgeTextUpdated()
    {
        //Arrange
        var sut = new GameObject().AddComponent<PlayerInformationView>();
        var dependencies = CreateDependencies();
        sut.SetDependencies(dependencies);
        _config.PreviousOutput = new PlayerInformationView.Output()
        {
            Name = "name",
            Age = "17"
        };
        
        var icon = new Mock<IIcon>();
        icon.Setup(x => x.GetIcon(It.IsAny<HuntProductCacheConfig>())).ReturnsAsync(new Byte[] { 55, 66 });
        var iconImage = new Mock<IIcon>();
        iconImage.Setup(x => x.GetIcon(It.IsAny<HuntProductCacheConfig>())).ReturnsAsync(new Byte[] { 66, 77 });

        _config.characterOption = new CharacterOption(iconImage.Object, icon.Object)
        {
            RoleColor = new RHColor(55,55,55,55),
            Id = "A",
            CharacterDescription = "description",
            CharacterName = "name"
        };
        PlayerInformationView.Output outputMsg = null;
        _config.Approve = (output) =>
        {
            outputMsg = output;
        };
        sut.Configure(_config);

        var newName = "another name";
        var newAge = "19";
        sut._dependencies.AgeTextInput.text = newAge;
        sut._dependencies.NameTextInput.text = newName;
        //Act
        sut.NameTextUpdated();
        sut.AgeTextUpdated();

        //Assert
        sut.Approve();
        Assert.AreEqual(newName, outputMsg.Name);
        Assert.AreEqual(newAge, outputMsg.Age);
    }
    
    [Test]
    public void TestHide()
    {
        var displayController = new Mock<IComponentDisplayController>();
        displayController.Setup(x => x.Hide()).Verifiable();
        var sut = new GameObject().AddComponent<PlayerInformationView>();
        var dependencies = CreateDependencies(displayController);
        sut.SetDependencies(dependencies);

        //Act
        sut.Hide();
        //Assert
        displayController.Verify(x => x.Hide());

    }

    [Test]
    public void TestDisplay()
    {
        var displayController = new Mock<IComponentDisplayController>();
        displayController.Setup(x => x.Display()).Verifiable();
        var sut = new GameObject().AddComponent<PlayerInformationView>();
        var dependencies = CreateDependencies(displayController);
        sut.SetDependencies(dependencies);

        //Act
        sut.Display();
        //Assert
        displayController.Verify(x => x.Display());
    }
}
