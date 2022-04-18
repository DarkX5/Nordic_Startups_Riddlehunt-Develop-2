using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSelection.Components;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products;
using riddlehouse_libraries.products.huntProduct;
using riddlehouse_libraries.products.resources;
using UI.UITools;
using UnityEngine;
using Random = System.Random;

[TestFixture]
public class TestCharacterSelectionComponent : MonoBehaviour
{
    private CharacterSelectionComponent.Config _config;

    [SetUp]
    public void Init()
    {
        _config = new CharacterSelectionComponent.Config()
        {
            Resource = new CharacterSelectionComponentResource()
            {
                RoleSelectionComponent = new RoleSelectionComponentResource()
                {
                    RoleButtons = new List<RoleButtonResource>()
                    {
                        new RoleButtonResource(new Mock<IIcon>().Object)
                        {
                            Title = "title",
                        }
                    }
                }
            }
        };
    }

    [TearDown]
    public void TearDown()
    {
        _config = null;
    }
    
    CharacterSelectionComponent.Dependencies CreateDependencies(
        Mock<IRoleSelectionComponent> roleSelectionComponentMock = null,
        Mock<ISelectedCharactersListComponent> selectedCharactersListComponentMock = null)
    {
        roleSelectionComponentMock ??= new Mock<IRoleSelectionComponent>();
        selectedCharactersListComponentMock ??= new Mock<ISelectedCharactersListComponent>();
        return new CharacterSelectionComponent.Dependencies()
        {
            RoleSelectionComponent = roleSelectionComponentMock.Object,
            SelectedCharactersListComponent = selectedCharactersListComponentMock.Object
        };
    }
    [Test]
    public void TestSetDependencies()
    {
        //Arrange
        var sut = new GameObject().AddComponent<CharacterSelectionComponent>();
        var dependencies = CreateDependencies();
        //Act
        sut.SetDependencies(dependencies);
        //Assert
        Assert.AreSame(dependencies, sut._dependencies);
    }

    [Test]
    public void TestConfigure_Configures_RoleSelection_And_SelectedCharactersListComponent()
    {
        //Arrange
        var sut = new GameObject().AddComponent<CharacterSelectionComponent>();
        var selectedCharactersListComponentMock = new Mock<ISelectedCharactersListComponent>();
        selectedCharactersListComponentMock.Setup(x => x.Configure(It.IsAny<SelectedCharactersListComponent.Config>())).Verifiable();

        
        var roleSelectionComponentMock = new Mock<IRoleSelectionComponent>();
        
        var actionMessage = "notCalled";
        Action<string> selectCharacterToAddAction = (msg) =>
        {
            actionMessage = msg;
        };

        roleSelectionComponentMock.Setup(x => x.Configure(It.IsAny<RoleSelectionComponent.Config>()))
            .Callback<RoleSelectionComponent.Config>((theConfig) =>
            {
                theConfig.AddRoleAction.Invoke(theConfig.Resource.RoleButtons[0].Title);
            })
            .Verifiable();
        var dependencies = CreateDependencies(roleSelectionComponentMock, selectedCharactersListComponentMock);
        sut.SetDependencies(dependencies);
 
        _config.SelectCharacterToAdd = selectCharacterToAddAction;
        //Act
        sut.Configure(_config);
        
        //Assert
        roleSelectionComponentMock.Verify(x => x.Configure(It.IsAny<RoleSelectionComponent.Config>()));
        selectedCharactersListComponentMock.Verify(x => x.Configure(It.IsAny<SelectedCharactersListComponent.Config>()));
        Assert.AreEqual(_config.Resource.RoleSelectionComponent.RoleButtons[0].Title, actionMessage);
    }
    
    [Test]
    public void TestEditCharacter()
    {
        //Arrange
        string roleID = "roleA";
        var sut = new GameObject().AddComponent<CharacterSelectionComponent>();
        var selectedCharactersListComponentMock = new Mock<ISelectedCharactersListComponent>();
        selectedCharactersListComponentMock.Setup(
            x => x.Configure(It.IsAny<SelectedCharactersListComponent.Config>()))
            .Callback<SelectedCharactersListComponent.Config>((theConfig) =>
            {
                theConfig.EditCharacterAction.Invoke(roleID);
            })
            .Verifiable();

        var dependencies = CreateDependencies(null, selectedCharactersListComponentMock);
        sut.SetDependencies(dependencies);
        
        var actionMessage = "notCalled";
        Action<string> editCharacterAction = (msg) =>
        {
            actionMessage = msg;
        };
        
        _config.EditCharacterAction = editCharacterAction;
        
        //Act
        sut.Configure(_config);
        
        //Assert
        selectedCharactersListComponentMock.Verify(x => x.Configure(It.IsAny<SelectedCharactersListComponent.Config>()));
        Assert.AreEqual(roleID, actionMessage);
    }
    [Test]
    public void TestRemoveCharacter()
    {
        //Arrange
        var huntPlayerData = new HuntCharacterData();
        var sut = new GameObject().AddComponent<CharacterSelectionComponent>();
        var selectedCharactersListComponentMock = new Mock<ISelectedCharactersListComponent>();
        selectedCharactersListComponentMock.Setup(
                x => x.RemoveCharacterButton(huntPlayerData)).Verifiable();

        var dependencies = CreateDependencies(null, selectedCharactersListComponentMock);
        sut.SetDependencies(dependencies);

        sut.Configure(_config);

        //Act
        sut.RemoveCharacter(huntPlayerData);
        
        //Assert
        selectedCharactersListComponentMock.Setup(
            x => x.RemoveCharacterButton(huntPlayerData)).Verifiable(); 
    }
    
    [Test]
    public void TestRegisterCharacter()
    {
        //Arrange
        var huntPlayerData = new HuntCharacterData();
        var sut = new GameObject().AddComponent<CharacterSelectionComponent>();
        var selectedCharactersListComponentMock = new Mock<ISelectedCharactersListComponent>();
        selectedCharactersListComponentMock.Setup(
            x => x.AddCharacterButtonToList(huntPlayerData)).Verifiable();

        var dependencies = CreateDependencies(null, selectedCharactersListComponentMock);
        sut.SetDependencies(dependencies);
        
        sut.Configure(_config);

        //Act
        sut.RegisterCharacter(huntPlayerData);
        
        //Assert
        selectedCharactersListComponentMock.Setup(
            x => x.AddCharacterButtonToList(huntPlayerData)).Verifiable(); 
    }
    
    
}
