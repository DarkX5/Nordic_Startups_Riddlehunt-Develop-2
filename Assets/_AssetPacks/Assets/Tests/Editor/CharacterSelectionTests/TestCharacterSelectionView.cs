using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSelection;
using CharacterSelection.Components;
using CharacterSelection.View;
using Components.Buttons;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products;
using riddlehouse_libraries.products.huntProduct;
using riddlehouse_libraries.products.models;
using riddlehouse_libraries.products.resources;
using TMPro;
using UnityEngine;

[TestFixture]
public class TestCharacterSelectionView
{
    private CharacterSelectionView.Config _config;

    [SetUp]
    public void Init()
    {
        _config = new CharacterSelectionView.Config()
        {
            Resource = new CharacterSelectionViewResource()
            {
                TitleText = "title"
            }
        };
    }

    [TearDown]
    public void TearDown()
    {
        _config = null;
    }

    private CharacterSelectionView.Dependencies CreateDependencies(
        Mock<ICharacterSelectionComponent> characterSelectionComponentMock = null)
    {
        characterSelectionComponentMock ??= new Mock<ICharacterSelectionComponent>();
        return new CharacterSelectionView.Dependencies()
        {
            Title = new GameObject().AddComponent<TextMeshProUGUI>(),
            CharacterSelectionComponent = characterSelectionComponentMock.Object
        };
    }
    
    [Test]
    public void TestSetDependencies_SetsClassDependencies()
    {
        //Given a newly created CharacterSelectionView
        //When SetDependencies is called with an object
        //Then those dependencies are mapped internally in the SUT.
        
        //Arrange
        var sut = new GameObject().AddComponent<CharacterSelectionView>();
        var dependencies = CreateDependencies();
        //Act
        sut.SetDependencies(dependencies);
        //Assert
        Assert.AreEqual(dependencies, sut._dependencies);
    }

    [Test]
    public void TestConfigure_SetsValuesFromConfig()
    {
        //Arrange
        var sut = new GameObject().AddComponent<CharacterSelectionView>();

        var dependencies = CreateDependencies();
        
        sut.SetDependencies(dependencies);

        sut.Configure(_config);
        
        //Assert
        Assert.AreEqual(_config.Resource.TitleText, dependencies.Title.text);
    }
    [Test]
    public void TestConfigure_Configures_CharacterSelectionComponent()
    {
        //Arrange
        var sut = new GameObject().AddComponent<CharacterSelectionView>();
        
        var characterSelectionComponentMock = new Mock<ICharacterSelectionComponent>();
        characterSelectionComponentMock.Setup(x => x.Configure(It.IsAny<CharacterSelectionComponent.Config>())).Verifiable();

        var dependencies = CreateDependencies( characterSelectionComponentMock);
        
        sut.SetDependencies(dependencies);
 
        //Act
        sut.Configure(_config);
        
        //Assert
        characterSelectionComponentMock.Verify(x => x.Configure(It.IsAny<CharacterSelectionComponent.Config>()));

    }

    [Test]
    public void TestRegisterCharacter_Calls_In_CharacterSelectionComponent()
    {
        //Arrange
        string id = "idA";
        var characterA = new HuntCharacterData()
        {
            Id = id,
            PlayerName = "name",
            Tags = new Dictionary<string, string>()
        };

        var sut = new GameObject().AddComponent<CharacterSelectionView>();
        var characterSelectionComponentMock = new Mock<ICharacterSelectionComponent>();
        characterSelectionComponentMock.Setup(x => x.RegisterCharacter(characterA)).Verifiable();
        var dependencies = CreateDependencies( characterSelectionComponentMock);
        
        sut.SetDependencies(dependencies);

        sut.Configure(_config);

        //Act
        sut.RegisterCharacter(characterA);
        
        //Assert
        characterSelectionComponentMock.Verify(x => x.RegisterCharacter(characterA));
    }
    
    [Test]
    public void TestRemoveCharacter_Calls_In_CharacterSelectionComponent()
    {
        //Arrange
        string id = "idA";
        var characterA = new HuntCharacterData()
        {
            Id = id,
            PlayerName = "name",
            Tags = new Dictionary<string, string>()
        };

        var sut = new GameObject().AddComponent<CharacterSelectionView>();
        var characterSelectionComponentMock = new Mock<ICharacterSelectionComponent>();
        characterSelectionComponentMock.Setup(x => x.RemoveCharacter(characterA)).Verifiable();
        var dependencies = CreateDependencies( characterSelectionComponentMock);
        
        sut.SetDependencies(dependencies);

        sut.Configure(_config);

        //Act
        sut.RemoveCharacter(characterA);
        
        //Assert
        characterSelectionComponentMock.Verify(x => x.RemoveCharacter(characterA));
    }
    [Test]
    public void TestEditCharacterAction_CallsAction_From_Configure()
    {
        //Arrange
        string roleTitle = "roleA";
        
        var sut = new GameObject().AddComponent<CharacterSelectionView>();
        var characterSelectionComponentMock = new Mock<ICharacterSelectionComponent>();
        characterSelectionComponentMock.Setup(x => x.Configure(It.IsAny<CharacterSelectionComponent.Config>()))
            .Callback<CharacterSelectionComponent.Config>((theConfig) =>
            {
                theConfig.EditCharacterAction.Invoke(roleTitle);
            })
            .Verifiable();
        var dependencies = CreateDependencies( characterSelectionComponentMock);
        
        sut.SetDependencies(dependencies);

        string actionMessage = "notCalled";
        Action<string> editCharacterAction = (msg) =>
        {
            actionMessage = msg;
        };
        _config.EditCharacterAction = editCharacterAction;
        //Act
        sut.Configure(_config);
        
        //Assert
        Assert.AreEqual(roleTitle, actionMessage);
    }
    [Test]
    public void TestRemoveCharacterAction_CallsAction_From_Configure()
    {
        //Arrange
        string roleTitle = "roleA";
        
        var sut = new GameObject().AddComponent<CharacterSelectionView>();
        var characterSelectionComponentMock = new Mock<ICharacterSelectionComponent>();
        characterSelectionComponentMock.Setup(x => x.Configure(It.IsAny<CharacterSelectionComponent.Config>()))
            .Callback<CharacterSelectionComponent.Config>((theConfig) =>
            {
                theConfig.RemoveCharacterAction.Invoke(roleTitle);
            })
            .Verifiable();
        var dependencies = CreateDependencies(characterSelectionComponentMock);
        
        sut.SetDependencies(dependencies);

        string actionMessage = "notCalled";
        Action<string> removeCharacterAction = (msg) =>
        {
            actionMessage = msg;
        };
        _config.RemoveCharacterAction = removeCharacterAction;
        //Act
        sut.Configure(_config);
        
        //Assert
        Assert.AreEqual(roleTitle, actionMessage);
    }
}
