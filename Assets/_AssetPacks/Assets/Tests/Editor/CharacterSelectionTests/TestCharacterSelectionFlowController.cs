using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSelection;
using CharacterSelection.View;
using Helpers;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products;
using riddlehouse_libraries.products.huntProduct;
using riddlehouse_libraries.products.resources;
using UnityEngine;

[TestFixture]
public class TestCharacterSelectionFlowController
{
    private CharacterSelectionFlowController.Dependencies CreateDependencies(
        RectTransform parent, 
        Mock<ICharacterSelectionStyler> styler = null,
        Mock<ISpriteHelper> spriteHelperMock = null,
        Mock<ICharacterSelectionController> characterSelectionMock = null,
        Mock<ITagSelectionController> tagSelectionControllerMock = null,
        Mock<IPlayerInformationController> playerInformationInputView = null
    )
    {
        styler ??= new Mock<ICharacterSelectionStyler>();
        characterSelectionMock ??= new Mock<ICharacterSelectionController>();
        tagSelectionControllerMock ??= new Mock<ITagSelectionController>();
        playerInformationInputView ??= new Mock<IPlayerInformationController>();

        styler.Setup(x =>
                    x.CreateSelectionHome(CharacterSelectionStyles.Standard, parent))
                .Returns(characterSelectionMock.Object)
                .Verifiable();

        styler.Setup(x =>
                    x.CreateTagView(TagSelectionStyles.Standard, parent))
                .Returns(tagSelectionControllerMock.Object)
                .Verifiable();

        styler.Setup(x =>
                    x.CreatePlayerInformationView(PlayerInformationInputStyles.Standard, parent))
                .Returns(playerInformationInputView.Object)
                .Verifiable();

        if (spriteHelperMock == null)
        {
            spriteHelperMock = new Mock<ISpriteHelper>();
            var icon = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
            spriteHelperMock.Setup(x => x.GetSpriteFromByteArray(It.IsAny<Byte[]>())).Returns(icon).Verifiable();
        }

        return new CharacterSelectionFlowController.Dependencies()
        {
            Styler = styler.Object,
            SpriteHelper = spriteHelperMock.Object
        };
    }

    private CharacterSelectionFlowController.Config _config;
    [SetUp]
    public void Init()
    {
        var tagOptionA = new TagOptionResource()
        {
            Title = "titleA",
            Description = "DescriptionA",
            Id = "tagIDA",
            Options = new List<string>() { "A", "B", "C" }
        };
        var tagOptionB = new TagOptionResource()
        {
            Title = "titleB",
            Description = "DescriptionB",
            Id = "tagIDB",
            Options = new List<string>() { "A", "B", "C" }
        };

        var icon = new Mock<IIcon>();
        icon.Setup(x => x.GetIcon(It.IsAny<HuntProductCacheConfig>())).ReturnsAsync(new Byte[2] {55,66});
        var iconImage = new Mock<IIcon>();
        iconImage.Setup(x => x.GetIcon(It.IsAny<HuntProductCacheConfig>())).ReturnsAsync(new Byte[2] { 66, 77 });
        _config = new CharacterSelectionFlowController.Config()
        {
            Resource = new CharacterSelectionFlowResource() {            
                TagOptions = new List<TagOptionResource>() {tagOptionA, tagOptionB},
                CharacterOptions = new List<CharacterOption>()
                {
                    new CharacterOption(iconImage.Object, icon.Object)
                    {
                        CharacterName = "name",
                        CharacterDescription = "description",
                        Id = "A"
                    }
                }
            },
            FlowComplete = null
        };
    }

    [TearDown]
    public void TearDown()
    {
        _config = null;
    }
    
    [Test]
    public void TestSetDependencies_SetsClassDependencies()
    {
        //Given a newly created CharacterSelectionFlowcontroller
        //When SetDependencies is called with an object
        //Then those dependencies are mapped internally in the SUT.
        
        //Arrange
        var go = new GameObject();
        var parent = go.AddComponent<RectTransform>();

        var sut = go.AddComponent<CharacterSelectionFlowController>();
        var dependencies = CreateDependencies(parent);
        //Act
        sut.SetDependencies(dependencies);
        //Assert
        Assert.AreEqual(dependencies, sut._dependencies);
    }

    [Test]
    public void TestConfigure_InstantiatesComponentStyles()
    {
        //Given an initialized CharacterSelectionFlowcontroller and a config object
        //When Configure is called.
        //Then the styler styles and configures the views as instructed.
        
        //Arrange
        var go = new GameObject();
        var parent = go.AddComponent<RectTransform>();

        var characterSelectionMock = new Mock<ICharacterSelectionController>();
        characterSelectionMock.Setup(x => x.Configure(It.IsAny<CharacterSelectionView.Config>())).Verifiable();
        characterSelectionMock.Setup(x => x.Display()).Verifiable();
        Mock<ICharacterSelectionStyler> characterSelectionStyler = new Mock<ICharacterSelectionStyler>();
        var dependencies = CreateDependencies(parent, characterSelectionStyler, null, characterSelectionMock, null, null);
       
        var sut = go.AddComponent<CharacterSelectionFlowController>();
        sut.SetDependencies(dependencies);
        
        //Act
        sut.Configure(_config);
        
        //Assert
        characterSelectionStyler.Verify(x => 
                x.CreateSelectionHome(CharacterSelectionStyles.Standard, parent));
        
        characterSelectionStyler.Verify(x => 
                x.CreateTagView(TagSelectionStyles.Standard, parent));
        
        characterSelectionStyler.Verify(x => 
                x.CreatePlayerInformationView(PlayerInformationInputStyles.Standard, parent));

        characterSelectionMock.Verify(x=> x.Configure(It.IsAny<CharacterSelectionView.Config>()));
        characterSelectionMock.Verify(x => x.Display());
    }
    
     [Test]
    public void TestConfigure_SecondCall_SameStyles_Does_Not_Destroy()
    {
        //Given a previously configured CharacterSelectionFlowcontroller and a new config object
        //When Configure is called.
        //Then the styler styles and configures the views as instructed, replacing existing with new ones.
        //--- yet persisting those that aren't a new type.
        
        //Arrange
        var go = new GameObject();
        var parent = go.AddComponent<RectTransform>();

        var characterSelectionMock = new Mock<ICharacterSelectionController>();
        characterSelectionMock.Setup(x=> 
            x.Configure(It.IsAny<CharacterSelectionView.Config>())).Verifiable();
        characterSelectionMock.Setup(x => x.DestroySelf()).Verifiable();
      
        var tagSelectionViewMock = new Mock<ITagSelectionController>();
        tagSelectionViewMock.Setup(x=> x.DestroySelf()).Verifiable();
        tagSelectionViewMock.Setup(x => x.Configure(It.IsAny<TagSelectionView.Config>())).Verifiable();

        var playerInformationInputView = new Mock<IPlayerInformationController>();
        playerInformationInputView.Setup(x=> x.DestroySelf()).Verifiable();
        
        var dependencies = CreateDependencies(parent, null, null, characterSelectionMock, tagSelectionViewMock, null);
        
        var sut = go.AddComponent<CharacterSelectionFlowController>();
        sut.SetDependencies(dependencies);
        sut.Configure(_config);

        //Act
        sut.Configure(_config);
        
        //Assert
        characterSelectionMock.Verify(x=> x.DestroySelf(), Times.Never);
        tagSelectionViewMock.Verify(x=> x.DestroySelf(), Times.Never);
        playerInformationInputView.Verify(x=> x.DestroySelf(), Times.Never);
        
        characterSelectionMock.Verify(x=> x.Configure(It.IsAny<CharacterSelectionView.Config>()), Times.Exactly(2));
    }
    
     [Test]
    public void TestStartAddCharacter_NavigatesTo_And_Configures_TagSelection()
    {
        //Given a configured flowController
        //When StartAddCharacter is called.
        //Then the characterSelection is hidden, and the playerInformation is configured, and shown on screen.
        
        //Arrange
        var go = new GameObject();
        var parent = go.AddComponent<RectTransform>();

        var characterSelectionMock = new Mock<ICharacterSelectionController>();
        characterSelectionMock.Setup(x => x.Hide()).Verifiable();
        var playerInformationViewMock = new Mock<IPlayerInformationController>();
        playerInformationViewMock.Setup(x => x.Configure(It.IsAny<PlayerInformationView.Config>())).Verifiable();
        playerInformationViewMock.Setup(x => x.Display()).Verifiable();
        
        var dependencies = CreateDependencies(parent, null, null, characterSelectionMock, null, playerInformationViewMock );
       
        var sut = go.AddComponent<CharacterSelectionFlowController>();
        sut.SetDependencies(dependencies);
        
        sut.Configure(_config);
        
        //Act
        sut.StartAddCharacter("A");
        
        characterSelectionMock.Verify(x => x.Hide());
        playerInformationViewMock.Verify(x => x.Configure(It.IsAny<PlayerInformationView.Config>()));
        playerInformationViewMock.Verify(x => x.Display());
    }
    
    [Test]
    public void TestCommitTag_NavigatesTo_And_Configures_PlayerInformationView()
    {
        //Given a configured flowController
        //When CommitTag is called.
        //Then the TagSelection is hidden, and the characterselection is displayed.
        
        //Arrange
        var go = new GameObject();
        var parent = go.AddComponent<RectTransform>();
        
        var tagSelectionViewMock = new Mock<ITagSelectionController>();
        tagSelectionViewMock.Setup(x => x.Hide()).Verifiable();

        var characterSelectionView = new Mock<ICharacterSelectionController>();
        characterSelectionView.Setup(x => x.Configure(It.IsAny<CharacterSelectionView.Config>())).Verifiable();
        characterSelectionView.Setup(x => x.Display()).Verifiable();

        var dependencies = CreateDependencies(parent, null, null, characterSelectionView, tagSelectionViewMock, null);
       
        var sut = go.AddComponent<CharacterSelectionFlowController>();
        sut.SetDependencies(dependencies);
        
        sut.Configure(_config);
        TagSelectionView.TagChoice tagChosen = new TagSelectionView.TagChoice()
        {
            TagId = "tagIDA",
            TagValue = "A"
        };
        sut.StartAddCharacter("playerId");

        //Act
        sut.CommitTag(tagChosen);
        
        tagSelectionViewMock.Verify(x => x.Hide());
        characterSelectionView.Verify(x => x.Configure(It.IsAny<CharacterSelectionView.Config>()));
        characterSelectionView.Verify(x => x.Display());
    }
    
      [Test]
    public void TestCompleteAddCharacter_NavigatesTo_CharacterSelection()
    {
        //Given a configured flowController
        //When CompleteAddCharacter is called.
        //Then the TagSelection is hidden, the CompleteAddCharacter is shown on screen.
        
        //Arrange
        var go = new GameObject();
        var parent = go.AddComponent<RectTransform>();
        
        var characterSelectionMock = new Mock<ICharacterSelectionController>();
        characterSelectionMock.Setup(x => x.Display()).Verifiable();
        
        var tagSelectionViewMock = new Mock<ITagSelectionController>();
        tagSelectionViewMock.Setup(x => x.Hide()).Verifiable();
        
        var dependencies = CreateDependencies(parent, null, null, characterSelectionMock, tagSelectionViewMock, null);
       
        var sut = go.AddComponent<CharacterSelectionFlowController>();
        sut.SetDependencies(dependencies);
        
        sut.Configure(_config);
        sut.StartAddCharacter("id");

        //Act
        sut.CompleteAddCharacter();
        
        tagSelectionViewMock.Verify(x => x.Hide());
        characterSelectionMock.Verify(x => x.Display());
    }

    [Test]
    public void TestEditCharacter_ReplacesExisting_WithNewValues()
    {
        var go = new GameObject();
        var parent = go.AddComponent<RectTransform>();
        
        var characterSelectionMock = new Mock<ICharacterSelectionController>();
        characterSelectionMock.Setup(x => x.Display()).Verifiable();
        characterSelectionMock.Setup(x => x.RegisterCharacter(It.IsAny<HuntCharacterData>())).Verifiable();
        characterSelectionMock.Setup(x => x.RemoveCharacter(It.IsAny<HuntCharacterData>())).Verifiable();
        
        var tagSelectionViewMock = new Mock<ITagSelectionController>();
        tagSelectionViewMock.Setup(x =>
                x.Configure(It.IsAny<TagSelectionView.Config>()))
            .Callback<TagSelectionView.Config>((theConfig) =>
            {
                theConfig.CommitTag.Invoke(new TagSelectionView.TagChoice()
                {
                    TagId = "tagIDA",
                    TagValue = "A"
                });
            });
        
        var characterName = "name";
        var characterAge = "age";
        var playerOutput = new PlayerInformationView.Output()
        {
            Name = characterName,
            Age = characterAge
        };
        
        var playerInformationInputView = new Mock<IPlayerInformationController>();
        playerInformationInputView.Setup(x => x.Hide()).Verifiable();
        playerInformationInputView.Setup(x => x.Configure(It.IsAny<PlayerInformationView.Config>()))
            .Callback<PlayerInformationView.Config>((theConfig) =>
            {
                theConfig.Approve.Invoke(playerOutput);
            });
  
        var dependencies = CreateDependencies(parent, null, null, characterSelectionMock, tagSelectionViewMock, playerInformationInputView);
        
        var sut = go.AddComponent<CharacterSelectionFlowController>();
        sut.SetDependencies(dependencies);
        sut.Configure(_config);
        sut.StartAddCharacter("A");
        //Act
        sut.EditCharacter(sut.RegisteredCharacters[0].Id);
        
        characterSelectionMock.Verify(x => 
            x.RegisterCharacter(It.IsAny<HuntCharacterData>()), Times.Exactly(2));
        characterSelectionMock.Verify(x =>
            x.RemoveCharacter(It.IsAny<HuntCharacterData>()), Times.Once);
    }
}