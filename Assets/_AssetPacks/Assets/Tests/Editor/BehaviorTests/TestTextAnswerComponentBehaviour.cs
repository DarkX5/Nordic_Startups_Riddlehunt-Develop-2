using System;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products.Assets;
using riddlehouse_libraries.products.AssetTypes;
using TMPro;
using UnityEngine;
using Lib_TextAsset = riddlehouse_libraries.products.Assets.TextAsset;

[TestFixture]
public class TestTextAnswerComponentBehaviour
{
    private string answer;
    private string identifier;
    private Lib_TextAsset _textAsset;

    [SetUp]
    public void Init()
    {
        answer = "theAnswer";
        _textAsset = new Lib_TextAsset(AssetType.TextAnswer, answer);
      
        identifier = "string-answer-identifier";
        PlayerPrefs.DeleteKey(identifier);
    }
    
    [Test]
    public void TestConfigure_NoAnswerAvailable_SetsState_None()
    {
        //Given a TextAnswerComponentBehaviour, with a StringAnswerData without an answer given.
        //When Configure is called
        //Answer state is set to none.
        
        // Arrange
        var go = new GameObject();
        var sut = go.AddComponent<TextAnswerComponentBehaviour>();
        var tmpInputField = go.AddComponent<TMP_InputField>();
        var tmpAnswerText = go.AddComponent<TextMeshProUGUI>();
        sut.SetDependencies(tmpInputField, tmpAnswerText);
        
        var answerAsset = new TextAnswerAsset(_textAsset);

        // Act
        sut.Configure(answerAsset);
        
        // Assert
        Assert.AreEqual(AnswerState.None, sut._state);
    }
    
    [Test]
    public void TestConfigure_NoAnswerAvailable_SetsState_Incorrect()
    {
        //Given a TextAnswerComponentBehaviour, with a StringAnswerData witg a wrong answer given.
        //When Configure is called
        //Answer state is set to incorrect.
        
        // Arrange
        var go = new GameObject();
        var sut = go.AddComponent<TextAnswerComponentBehaviour>();
        var tmpInputField = go.AddComponent<TMP_InputField>();
        var tmpAnswerText = go.AddComponent<TextMeshProUGUI>();
        sut.SetDependencies(tmpInputField, tmpAnswerText);
        
        var answerAsset = new TextAnswerAsset(_textAsset);
        answerAsset.SetAnswer("incorrect");
        // Act
        sut.Configure(answerAsset);
        
        // Assert
        Assert.AreEqual(AnswerState.Incorrect, sut._state);
    }
    
    [Test]
    public void TestConfigure_NoAnswerAvailable_SetsState_Correct()
    {
        //Given a TextAnswerComponentBehaviour, with a StringAnswerData with a correct answer given.
        //When Configure is called
        //Answer state is set to correct.
        
        // Arrange
        var go = new GameObject();
        var sut = go.AddComponent<TextAnswerComponentBehaviour>();
        var tmpInputField = go.AddComponent<TMP_InputField>();
        var tmpAnswerText = go.AddComponent<TextMeshProUGUI>();
        sut.SetDependencies(tmpInputField, tmpAnswerText);
        
        var answerAsset = new TextAnswerAsset(_textAsset);
        answerAsset.SetAnswer("theAnswer");
        // Act
        sut.Configure(answerAsset);
        
        // Assert
        Assert.AreEqual(AnswerState.Correct, sut._state);
    }
    
    [Test]
    public void TestPerformAction()
    {
        // Given a user submits their answer
        // When answering a riddle
        // Then the action associated with the submit answer button is called!
        
        // Arrange
        var go = new GameObject();
        var sut = go.AddComponent<TextAnswerComponentBehaviour>();
        var answerComponent = new Mock<ITextAnswerComponent>();
        answerComponent.Setup(x => x.PerformAction()).Verifiable();
        
        // Act
        sut.SetAnswerComponent(answerComponent.Object);
        sut.PerformAction();
        
        // Assert
        answerComponent.Verify(x => x.PerformAction());
    }

    [Test]
    public void TestUpdateAnswer()
    {
        //Given a user is viewing a riddle with an answer
        //When the user is editting the answer
        //Then the answer is updated in the component
        
        //Arrange
        string updatedAnswer = "updatedAnswer";
        var go = new GameObject();
        var tmpInputField = go.AddComponent<TMP_InputField>();
        var tmpAnswerText = go.AddComponent<TextMeshProUGUI>();
        var sut = go.AddComponent<TextAnswerComponentBehaviour>();
        var answerComponent = new Mock<ITextAnswerComponent>();
        answerComponent.Setup(x => x.UpdateAnswer(updatedAnswer)).Verifiable();
        sut.SetDependencies(tmpInputField, tmpAnswerText);
        sut.SetAnswerComponent(answerComponent.Object);
        
        //Act
        tmpInputField.text = updatedAnswer;
        sut.UpdateAnswer();
        //Assert
        answerComponent.Verify(x => x.UpdateAnswer(updatedAnswer));
    }
}
