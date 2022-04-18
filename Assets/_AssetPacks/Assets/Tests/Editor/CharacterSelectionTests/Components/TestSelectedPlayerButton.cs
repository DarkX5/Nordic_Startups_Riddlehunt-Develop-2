using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSelection.Components;
using NUnit.Framework;
using Prefabs.UI.CharacterSelection.StyledCharacterSelection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestSelectedPlayerButton
{
    public SelectedPlayerButton.Dependencies CreateDependencies()
    {
        return new SelectedPlayerButton.Dependencies()
        {
            TextField = new GameObject().AddComponent<TextMeshProUGUI>(),
            IconField = new GameObject().AddComponent<Image>()
        };
    }

    [Test]
    public void TestSetDependencies()
    {
        //Arrange
        var sut = new GameObject().AddComponent<SelectedPlayerButton>();
        var dependencies = CreateDependencies();
        //Act
        sut.SetDependencies(dependencies);
        //Assert
        Assert.AreSame(dependencies, sut._dependencies);
    }

    [Test]
    public void TestConfigure_Sets_ButtonText_And_LabelText()
    {
        //Given a new SelectedCharacterButton
        //When Configure is called with ID and Tag list
        //Then the TextField and iconField is configured with those values.
        
        //Arrange
        var icon = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        var sut = new GameObject().AddComponent<SelectedPlayerButton>();
        var dependencies = CreateDependencies();
        sut.SetDependencies(dependencies);

        string playerName = "name";
        string tagA = "Rolle";
        string tagB = "Let";
        string expectedLabelText = playerName + " | Rolle | Let";
        var tagList = new List<string>() { tagA, tagB };
        
        var config = new SelectedPlayerButton.Config()
        {
            Id = "id",
            Label = playerName,
            Tags = tagList,
            Icon = icon
        };
        
        //Act
        sut.Configure(config);
        
        //Assert
        Assert.AreEqual(expectedLabelText, sut._dependencies.TextField.text);
        Assert.AreEqual(icon, sut._dependencies.IconField.sprite);
    }
    
    [Test]
    public void TestEditCharacter_InvokesAction_From_Config()
    {
        //Given a configured SelectedPlayerButton
        //When Edit is called
        //Then the EditCharacterAction is invoked.
        
        //Arrange
        var sut = new GameObject().AddComponent<SelectedPlayerButton>();
        var dependencies = CreateDependencies();
        sut.SetDependencies(dependencies);

        var icon = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        string playerName = "name";
        string tagA = "Rolle";
        string tagB = "Let";
        var tagList = new List<string>() { tagA, tagB };


        string actionMessage = "notCalled";
        Action<string> editCharacterAction = (msg) =>
        {
            actionMessage = msg;
        };
        
        var config = new SelectedPlayerButton.Config()
        {
            Id = "id",
            Label = playerName,
            Tags = tagList,
            Icon = icon,
            Edit = editCharacterAction
        };
        sut.Configure(config);

        //Act
        sut.Edit();
        
        //Assert
        Assert.AreEqual("id", actionMessage);
    }
    
    [Test]
    public void TestRemoveCharacter_InvokesAction_From_Config()
    {
        //Given a configured SelectedPlayerButton
        //When Remove is called
        //Then the RemoveCharacterAction is invoked.
        
        //Arrange
        var sut = new GameObject().AddComponent<SelectedPlayerButton>();
        var dependencies = CreateDependencies();
        sut.SetDependencies(dependencies);

        string playerName = "name";
        string tagA = "Rolle";
        string tagB = "Let";
        var tagList = new List<string>() { tagA, tagB };

        string actionMessage = "notCalled";
        Action<string> removeCharacterAction = (msg) =>
        {
            actionMessage = msg;
        };
        
        var config = new SelectedPlayerButton.Config()
        {
            Id = playerName,
            Tags = tagList,
            Remove = removeCharacterAction
        };
        sut.Configure(config);

        //Act
        sut.Remove();
        
        //Assert
        Assert.AreEqual(playerName, actionMessage);
    }
    
    
    [Test]
    public void TestDestroySelf()
    {
        //Arrange
        var sut = new GameObject().AddComponent<SelectedPlayerButton>();
        var dependencies = CreateDependencies();
        sut.SetDependencies(dependencies);

        //Act
        sut.DestroySelf();
        //Assert
        Assert.IsTrue(sut == null);
    }
}
