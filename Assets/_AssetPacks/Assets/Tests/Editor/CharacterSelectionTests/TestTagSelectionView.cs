using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using CharacterSelection.View;
using Components.Selection.MultipleChoice;
using Helpers;
using Moq;
using NUnit.Framework;
using Prefabs.UI.CharacterSelection.StyledCharacterSelection.Scripts;
using riddlehouse_libraries.products.huntProduct;
using riddlehouse_libraries.products.resources;
using TMPro;
using UnityEngine;

[TestFixture]
public class TestTagSelectionView
{
    private TagSelectionView.Config _config;

    private TagSelectionView.Dependencies CreateDependencies(
        Mock<IComponentDisplayController> displayController = null,
        Mock<ISingleChoiceMultipleOptionsSelectionComponent> tagChoiceListComponent = null,
        Mock<ISpriteHelper> spriteHelper = null,
        Mock<IFooter> footerMock = null)
    {
        displayController ??= new Mock<IComponentDisplayController>();
        tagChoiceListComponent ??= new Mock<ISingleChoiceMultipleOptionsSelectionComponent>();
        footerMock ??= new Mock<IFooter>();
        if (spriteHelper == null)
        {
            var icon = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
            spriteHelper = new Mock<ISpriteHelper>();
            spriteHelper.Setup(x => x.GetSpriteFromByteArray(It.IsAny<Byte[]>())).Returns(icon);
        }
        return new TagSelectionView.Dependencies()
        {
            TitleField = new GameObject().AddComponent<TextMeshProUGUI>(),
            DescriptionField = new GameObject().AddComponent<TextMeshProUGUI>(),
            DisplayController = displayController.Object,
            TagChoiceListComponent = tagChoiceListComponent.Object,
            Footer = footerMock.Object,
            SpriteHelper = spriteHelper.Object
        };
    }

    [SetUp]
    public void Init()
    {
        var characterIcon = new Mock<IIcon>();
        characterIcon.Setup(x => x.GetIcon(It.IsAny<HuntProductCacheConfig>())).ReturnsAsync(new Byte[] { 66, 77 });
        var characterImage = new Mock<IIcon>();
        _config = new TagSelectionView.Config()
        {
            Resource = new TagSelectionViewResource()
            {
            },
            characterOption = new CharacterOption(characterImage.Object, characterIcon.Object)
            {
                RoleColor = new RHColor(55,55,55,55)
            },
            Option = new TagOptionResource()
            {
                Id="role",
                Title = "Rolle",
                Description = "Rolle beskrivelse",
                Options = new List<string>() {"Kriger","Tidstager","Kortstyrer","Ordstyrer"}
            }
        };
    }

    [TearDown]
    public void TearDown()
    {
        _config = null;
    }
    
    [Test]
    public void SetDependencies_sets_Class_Dependencies()
    {
        var sut = new GameObject().AddComponent<TagSelectionView>();
        var dependencies = CreateDependencies();
        sut.SetDependencies(dependencies);
        
        Assert.AreEqual(dependencies, sut._dependencies);
    }

    [Test]
    public void TestConfigure_SetsConfigValuesInUI()
    {
        var tagChoiceListControllerMock = new Mock<ISingleChoiceMultipleOptionsSelectionComponent>();
        tagChoiceListControllerMock.Setup(x => x.DefineOptionChosen(It.IsAny<string>())).Verifiable();
        tagChoiceListControllerMock.Setup(x => x.Configure(It.IsAny<SingleChoiceMultipleOptionsSelectionComponent.Config>())).Verifiable();
        var dependencies = CreateDependencies(null, tagChoiceListControllerMock);
        var sut = new GameObject().AddComponent<TagSelectionView>();
        sut.SetDependencies(dependencies);

        sut.Configure(_config);
        
        Assert.AreEqual(_config.Option.Title, sut._dependencies.TitleField.text);
        Assert.AreEqual(_config.Option.Description, sut._dependencies.DescriptionField.text);
        tagChoiceListControllerMock.Verify(x => x.Configure(It.IsAny<SingleChoiceMultipleOptionsSelectionComponent.Config>()));
        tagChoiceListControllerMock.Verify(x => x.DefineOptionChosen(It.IsAny<string>()), Times.Never);

    }
    [Test]
    public void TestConfigure_SecondCall_HasPreviouslyChosenValue_SetsConfigValuesInUI()
    {
        var tagChoiceListControllerMock = new Mock<ISingleChoiceMultipleOptionsSelectionComponent>();
        tagChoiceListControllerMock.Setup(x => x.DefineOptionChosen("Tidstager")).Verifiable();
        tagChoiceListControllerMock.Setup(x => x.Configure(It.IsAny<SingleChoiceMultipleOptionsSelectionComponent.Config>())).Verifiable();

        var footerMock = new Mock<IFooter>();
        footerMock.Setup(x => x.Configure(It.IsAny<Footer.Config>())).Verifiable();

        var characterIcon = new Mock<IIcon>();
        var characterIconByteArr = new Byte[] { 66, 77 };
        characterIcon.Setup(x => x.GetIcon(It.IsAny<HuntProductCacheConfig>())).ReturnsAsync(characterIconByteArr);
        var characterImage = new Mock<IIcon>();
        
        var icon = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        var spriteHelper = new Mock<ISpriteHelper>();
        spriteHelper.Setup(x => x.GetSpriteFromByteArray(characterIconByteArr)).Returns(icon).Verifiable();

        _config.characterOption = new CharacterOption(characterImage.Object, characterIcon.Object)
        {
            RoleColor = new RHColor(55, 55, 55, 55)
        };
        
        var dependencies = CreateDependencies(null, tagChoiceListControllerMock, spriteHelper, footerMock);
        var sut = new GameObject().AddComponent<TagSelectionView>();
        sut.SetDependencies(dependencies);
        _config.PreviousSelection = "Tidstager";
        sut.Configure(_config);
        
        tagChoiceListControllerMock.Verify(x => x.DefineOptionChosen("Tidstager"));
        footerMock.Verify(x => x.Configure(It.IsAny<Footer.Config>()));
        spriteHelper.Verify(x => x.GetSpriteFromByteArray(characterIconByteArr));
        characterIcon.Verify(x => x.GetIcon(It.IsAny<HuntProductCacheConfig>()));
    }
    
    [Test]
    public void TestCommitTag_CallsAction_In_Configure()
    {
        var sut = new GameObject().AddComponent<TagSelectionView>();
        var tagChoiceListControllerMock = new Mock<ISingleChoiceMultipleOptionsSelectionComponent>();
        tagChoiceListControllerMock.Setup(x => x.GetOptionChosen())
            .Returns(_config.Option.Options[0])
        .Verifiable();
        
        var dependencies = CreateDependencies(null, tagChoiceListControllerMock);
        sut.SetDependencies(dependencies);

        TagSelectionView.TagChoice actionMessage = null;
        Action<TagSelectionView.TagChoice> commitTagAction = (choice) => { actionMessage = choice; };

        _config.CommitTag = commitTagAction;
        
        sut.Configure(_config);
        
        sut.CommitTag();
        
        Assert.AreEqual( _config.Option.Id,actionMessage.TagId);
        Assert.AreEqual( _config.Option.Options[0],actionMessage.TagValue);

        Assert.IsNotNull(actionMessage);
    }
    
}