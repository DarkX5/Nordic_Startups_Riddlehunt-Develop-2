using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSelection;
using CharacterSelection.Components;
using Helpers;
using Moq;
using NUnit.Framework;
using Prefabs.UI.CharacterSelection.StyledCharacterSelection;
using riddlehouse_libraries.products.huntProduct;
using riddlehouse_libraries.products.resources;
using UnityEngine;

public class TestSelectedCharactersListComponent 
{
    SelectedCharactersListComponent.Dependencies CreateDependencies(
        Mock<ISelectedPlayerButtonInstantiater> selectedCharacterButtonInstantiaterMock = null,
        Mock<ISpriteHelper> spriteHelper = null)
    {
        selectedCharacterButtonInstantiaterMock ??= new Mock<ISelectedPlayerButtonInstantiater>();
        spriteHelper ??= new Mock<ISpriteHelper>();
        return new SelectedCharactersListComponent.Dependencies()
        {
            SelectedPlayerButtonInstantiater = selectedCharacterButtonInstantiaterMock.Object,
            SpriteHelper = spriteHelper.Object
        };
    }
    [Test]
    public void TestSetDependencies()
    {
        //Arrange
        var sut = new GameObject().AddComponent<SelectedCharactersListComponent>();
        var dependencies = CreateDependencies();
        //Act
        sut.SetDependencies(dependencies);
        //Assert
        Assert.AreSame(dependencies, sut._dependencies);
    }

    [Test]
    public void TestConfigure_SecondCall_EmptiesList()
    {
        //Given a previously configured SelectedCharactersList with two buttons added
        //When Configure is called a second time
        //Then the buttons are deleted and the list cleared.
        
        //Arrange
        var sut = new GameObject().AddComponent<SelectedCharactersListComponent>();

        var characterButtonMockA = new Mock<ISelectedPlayerButton>();
        characterButtonMockA.Setup(x => x.Configure(It.IsAny<SelectedPlayerButton.Config>())).Verifiable();
        var characterButtonMockB = new Mock<ISelectedPlayerButton>();
        characterButtonMockB.Setup(x => x.Configure(It.IsAny<SelectedPlayerButton.Config>())).Verifiable();

        var selectedCharacterButtonInstantiaterMock = new Mock<ISelectedPlayerButtonInstantiater>();
        selectedCharacterButtonInstantiaterMock.SetupSequence(x => x.Create())
            .Returns(characterButtonMockA.Object)
            .Returns(characterButtonMockB.Object);

        var byteArr = new Byte[0];
        var icon = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        var spriteHelperMock = new Mock<ISpriteHelper>();
        spriteHelperMock.Setup(x => x.GetSpriteFromByteArray(byteArr)).Returns(icon).Verifiable();
        
        var dependencies = CreateDependencies(selectedCharacterButtonInstantiaterMock, spriteHelperMock);
        sut.SetDependencies(dependencies);
        
        var rolebuttonIcon = new Mock<IIcon>();
        rolebuttonIcon.Setup(x => x.GetIcon(null)).ReturnsAsync(byteArr).Verifiable();
        
        var roleSelectionComponentResource = new RoleSelectionComponentResource()
        {
            InitialTag = "role",
            RoleButtons = new List<RoleButtonResource>()
            {
                new RoleButtonResource(rolebuttonIcon.Object)
                {
                    Title = "Anna"
                }
            }
        };
        
        //first configure - empty list.
        sut.Configure(new SelectedCharactersListComponent.Config()
        {
            Resource = roleSelectionComponentResource
        });
        //Add two characters to list.
        var characterA = new HuntCharacterData()
        {
            Id = "idA",
            PlayerName = "name",
            Tags = new Dictionary<string, string>() {{"role", "Anna"}}
        };
        var characterB = new HuntCharacterData()
        {
            Id = "idB",
            PlayerName = "name",
            Tags = new Dictionary<string, string>() {{"role", "Anna"}}
        };
        sut.AddCharacterButtonToList(characterA);
        sut.AddCharacterButtonToList(characterB);
        
        //Act - second confgiure, empties list.
        sut.Configure(new SelectedCharactersListComponent.Config()
        {
            Resource = roleSelectionComponentResource
        });
        
        //Assert
        selectedCharacterButtonInstantiaterMock.Verify(x=> x.Create(), Times.Exactly(2));
        characterButtonMockA.Verify(x => x.Configure(It.IsAny<SelectedPlayerButton.Config>()));
        characterButtonMockB.Verify(x => x.Configure(It.IsAny<SelectedPlayerButton.Config>()));
        
        characterButtonMockA.Verify(x => x.DestroySelf());
        characterButtonMockB.Verify(x => x.DestroySelf());
    }
    [Test]
    public void TestAddCharacterButtonToList_Adds_And_Configures_CharacterButton()
    {
        //Given a configured SelectedCharactersList
        //When AddCharacterButtonToList is called
        //Then a buttons is added and configured
        
        //Arrange
        var sut = new GameObject().AddComponent<SelectedCharactersListComponent>();

        var characterButtonMockA = new Mock<ISelectedPlayerButton>();
        characterButtonMockA.Setup(x => x.Configure(It.IsAny<SelectedPlayerButton.Config>())).Verifiable();
        
        var selectedCharacterButtonInstantiaterMock = new Mock<ISelectedPlayerButtonInstantiater>();
        selectedCharacterButtonInstantiaterMock.Setup(x => x.Create())
                .Returns(characterButtonMockA.Object)
            .Verifiable();

        var byteArr = new Byte[0];
        var icon = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        var spriteHelperMock = new Mock<ISpriteHelper>();
        spriteHelperMock.Setup(x => x.GetSpriteFromByteArray(byteArr)).Returns(icon).Verifiable();
        
        var dependencies = CreateDependencies(selectedCharacterButtonInstantiaterMock, spriteHelperMock);
        sut.SetDependencies(dependencies);
        
        var rolebuttonIcon = new Mock<IIcon>();
        rolebuttonIcon.Setup(x => x.GetIcon(null)).ReturnsAsync(byteArr).Verifiable();
        
        var roleSelectionComponentResource = new RoleSelectionComponentResource()
        {
            InitialTag = "role",
            RoleButtons = new List<RoleButtonResource>()
            {
                new RoleButtonResource(rolebuttonIcon.Object)
                {
                    Title = "Anna"
                }
            }
        };

        
        sut.SetDependencies(dependencies);
        sut.Configure(new SelectedCharactersListComponent.Config()
        {
            Resource = roleSelectionComponentResource
        });
        
        var characterA = new HuntCharacterData()
        {
            Id = "idA",
            PlayerName = "name",
            Tags = new Dictionary<string, string>() {{"role", "Anna"}}
        };

        //Act
        sut.AddCharacterButtonToList(characterA);

        //Assert
        selectedCharacterButtonInstantiaterMock.Verify(x=> x.Create(), Times.Exactly(1));
        characterButtonMockA.Verify(x => x.Configure(It.IsAny<SelectedPlayerButton.Config>()));
    }
    
    [Test]
    public void TestEditCharacter_Calls_Action_From_Config()
    {
        //Given a configured SelectedCharactersList
        //When EditCharacter is called
        //Then action in config is called with the ID.
        
        //Arrange
        var characterId = "idA";
        var sut = new GameObject().AddComponent<SelectedCharactersListComponent>();

        var characterButtonMockA = new Mock<ISelectedPlayerButton>();
        characterButtonMockA.Setup(x => 
            x.Configure(It.IsAny<SelectedPlayerButton.Config>())).Verifiable();
        
        var selectedCharacterButtonInstantiaterMock = new Mock<ISelectedPlayerButtonInstantiater>();
        selectedCharacterButtonInstantiaterMock.SetupSequence(x => x.Create())
            .Returns(characterButtonMockA.Object);

        var dependencies = CreateDependencies(selectedCharacterButtonInstantiaterMock);

        sut.SetDependencies(dependencies);

        string actionMessage = "notCalled";
        Action<string> editCharacterAction = (msg) =>
        {
            actionMessage = msg;
        };
        //first configure - empty list.
        var config = new SelectedCharactersListComponent.Config()
        {
            EditCharacterAction = editCharacterAction
        };
        
        sut.Configure(config);
        var characterA = new HuntCharacterData()
        {
            Id = "idA",
            PlayerName = "name",
            Tags = new Dictionary<string, string>()
        };

        sut.AddCharacterButtonToList(characterA);
        //Act
        sut.EditCharacter(characterId);

        //Assert
        Assert.AreEqual(characterId, actionMessage);
    }
    [Test]
    public void TestRemoveCharacter_Calls_Action_From_Config()
    {
        //Given a configured SelectedCharactersList
        //When RemoveCharacter is called
        //Then action in config is called with the ID.
        
        //Arrange
        var characterId = "idA";
        var sut = new GameObject().AddComponent<SelectedCharactersListComponent>();

        var characterButtonMockA = new Mock<ISelectedPlayerButton>();
        characterButtonMockA.Setup(x => 
            x.Configure(It.IsAny<SelectedPlayerButton.Config>())).Verifiable();
        
        characterButtonMockA.Setup(x => x.DestroySelf()).Verifiable();
        var selectedCharacterButtonInstantiaterMock = new Mock<ISelectedPlayerButtonInstantiater>();
        selectedCharacterButtonInstantiaterMock.SetupSequence(x => x.Create())
            .Returns(characterButtonMockA.Object);

        var byteArr = new Byte[0];
        var icon = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        var spriteHelperMock = new Mock<ISpriteHelper>();
        spriteHelperMock.Setup(x => x.GetSpriteFromByteArray(byteArr)).Returns(icon).Verifiable();
        var dependencies = CreateDependencies(selectedCharacterButtonInstantiaterMock, spriteHelperMock);
        sut.SetDependencies(dependencies);

        string actionMessage = "notCalled";
        Action<string> removeCharacterAction = (msg) =>
        {
            actionMessage = msg;
        };
        //first configure - empty list.
        var rolebuttonIcon = new Mock<IIcon>();
        rolebuttonIcon.Setup(x => x.GetIcon(null)).ReturnsAsync(byteArr).Verifiable();
        var config = new SelectedCharactersListComponent.Config()
        {
            Resource = new RoleSelectionComponentResource()
            {
                InitialTag = "role",
                RoleButtons = new List<RoleButtonResource>()
                {
                    new RoleButtonResource(rolebuttonIcon.Object)
                    {
                        Title = "Anna"
                    }
                }
            },
            RemoveCharacterAction = removeCharacterAction
        };
        
        sut.Configure(config);
        var characterA = new HuntCharacterData()
        {
            Id = characterId,
            PlayerName = "name",
            Tags = new Dictionary<string, string>() {{"role","Anna"}}
        };

        sut.AddCharacterButtonToList(characterA);

        //Act
        sut.RemoveCharacter(characterId);
        
        //Assert
        Assert.AreEqual(characterId, actionMessage);
        characterButtonMockA.Verify(x => x.DestroySelf());
        rolebuttonIcon.Verify(x => x.GetIcon(null));
        spriteHelperMock.Verify(x => x.GetSpriteFromByteArray(byteArr));
    }
}
