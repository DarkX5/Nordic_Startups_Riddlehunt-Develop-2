using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products.Assets;
using riddlehouse_libraries.products.Assets.AnswerAssets;
using UnityEngine;
using UnityEngine.PlayerLoop;

[TestFixture]
public class TestStringMultipleChoiceAnswerData
{
    private List<string> _options;
    private string _correctAnswer;
    private string _seperator;
    private string _identifier;

    [SetUp]
    public void Init()
    {
        _options = new List<string>()
        {
            "1825",
            "1925",
            "1984"
        };
        _correctAnswer = "1984;";
        _seperator = ";";
        
        _identifier = "stringMultipleChoice-answer-identifier";
        PlayerPrefs.DeleteKey(_identifier);
    }

    [TearDown]
    public void TearDown()
    {
    }
    
    [Test]
    public void TestConstructor_RetrievesAssetFile_SetsCorrectAnswer_HasNoSession_Invokes_True()
    {
        //Given a StringMultipleChoiceAnswerData with a succeeding asset retrieval
        //When the texgetter succeeds
        //Then the json is unpacked and readied.
        
        //Arrange -- see init
        ////Act
        var sut = new StringMultipleChoiceAnswerData(_identifier, _options, _correctAnswer, _seperator, MultipleChoiceLogic.ContainsAll);
        
        //Assert
        Assert.AreEqual(3, sut.PossibleAnswers.Count);
        Assert.AreEqual(_correctAnswer, sut.CorrectAnswers);
        Assert.IsFalse(sut.HasAnswer());        
    }
    
    [Test]
    public void TestConstructor_SetsCorrectAnswer_HasSession_Invokes_True()
    {
        //Given a new StringMultipleChoiceAnswerData and an active session in playerprefs
        //When the texgetter is constructed
        //Then data is configured and the session is retrieved
        
        //Arrange
        PlayerPrefs.SetString(_identifier, _correctAnswer);

        //Act
        var sut = new StringMultipleChoiceAnswerData(_identifier, _options, _correctAnswer, _seperator, MultipleChoiceLogic.ContainsAll);
        
        //Assert
        Assert.AreEqual(3, sut.PossibleAnswers.Count);
        Assert.AreEqual(_correctAnswer, sut.CorrectAnswers);
        Assert.IsTrue(sut.HasAnswer());
        Assert.IsTrue(sut.HasCorrectAnswer());
    }

    [Test]
    public void TestHasAnswer_Returns_True()
    {
        //Given a StringMultipleChoiceAnswerData with an answer.
        //When HasAnswer is called
        //Then the function returns true.
        
        //Arrange
        var sut = new StringMultipleChoiceAnswerData(_identifier, _options, _correctAnswer, _seperator, MultipleChoiceLogic.ContainsAll);
        sut.SetAnswer("theAnswer");
        //Act
        var hasAnswer = sut.HasAnswer();
        
        //Assert
        Assert.IsTrue(hasAnswer);
    }
    
    [Test]
    public void TestHasAnswer_Returns_False()
    {
        //Given a StringMultipleChoiceAnswerData with no answer.
        //When HasAnswer is called
        //Then the function returns false.
        
        //Arrange
        var sut = new StringMultipleChoiceAnswerData(_identifier, _options, _correctAnswer, _seperator, MultipleChoiceLogic.ContainsAll);

        //Act
        var hasAnswer = sut.HasAnswer();
        
        //Assert
        Assert.IsFalse(hasAnswer);
    }
    
    [TestCase("1984", "1925")]
    [TestCase("1984", "1925")]
    [Test]
    public void TestHasCorectAnswer_ContainsAll_Returns_True(string answerA, string answerB)
    {
        //Given a StringMultipleChoiceAnswerData with a correct answerValue and the containsAll logic tag.
        //When HasCorrectAnswer is called
        //Then the function returns true.
        
        //Arrange
        _correctAnswer = "1984;1925;";
        
        var sut = new StringMultipleChoiceAnswerData(_identifier, _options, _correctAnswer, _seperator, MultipleChoiceLogic.ContainsAll);
        sut.AddAnswer(answerA);
        sut.AddAnswer(answerB);
        
        //Act
        var hasCorrectAnswer = sut.HasCorrectAnswer();
        
        //Assert
        Assert.IsTrue(hasCorrectAnswer);
    }

    [Test]
    public void TestHasCorectAnswer_Exact_Returns_True()
    {
        //Given a StringMultipleChoiceAnswerData with a correct answerValue and the exact logic tag.
        //When HasCorrectAnswer is called
        //Then the function returns true.
        
        //Arrange
        var sut = new StringMultipleChoiceAnswerData(_identifier, _options, _correctAnswer, _seperator, MultipleChoiceLogic.Exact);
        sut.SetAnswer(_correctAnswer);
        
        //Act
        var hasCorrectAnswer = sut.HasCorrectAnswer();
        
        //Assert
        Assert.IsTrue(hasCorrectAnswer);
    }
    
    [TestCase("1984")]
    [TestCase("1925")]
    [Test]
    public void TestHasCorectAnswer_ContainsAll_Returns_False(string answerOption)
    {
        //Given a StringMultipleChoiceAnswerData with an incorrect answerValue.
        //When HasCorrectAnswer is called
        //Then the function returns false.
        
        //Arrange
        _correctAnswer = "1984;1925;";

        var sut = new StringMultipleChoiceAnswerData(_identifier, _options, _correctAnswer, _seperator, MultipleChoiceLogic.ContainsAll);
        sut.SetAnswer(answerOption);
        
        //Act
        var hasCorrectAnswer = sut.HasCorrectAnswer();
        
        //Assert
        Assert.IsFalse(hasCorrectAnswer);
    }
    [Test]
    public void TestHasCorectAnswer_Exact_Returns_False()
    {
        //Given a StringMultipleChoiceAnswerData with an incorrect answerValue.
        //When HasCorrectAnswer is called
        //Then the function returns false.
        
        //Arrange
        var sut = new StringMultipleChoiceAnswerData(_identifier, _options, _correctAnswer, _seperator, MultipleChoiceLogic.Exact);
        sut.SetAnswer("1884;");
        
        //Act
        var hasCorrectAnswer = sut.HasCorrectAnswer();
        
        //Assert
        Assert.IsFalse(hasCorrectAnswer);
    }
    
    [Test]
    public void TestAddAnswer()
    {
        //Given a StringMultipleChoiceAnswerData
        //When AddAnswer is called
        //Then the recordedAnswer contains the answer added, and hasAnswer returns true.
        
        //Arrange
        var seperator = ";";
        var sut = new StringMultipleChoiceAnswerData(_identifier, _options, _correctAnswer, seperator, MultipleChoiceLogic.ContainsAll);
        var answerValueToAdd = "1984";
        //Act
        sut.AddAnswer(answerValueToAdd);

        //Assert
        var hasAnswer = sut.HasAnswer();
        Assert.IsTrue(hasAnswer);
        Assert.AreEqual(answerValueToAdd+seperator, sut.RecordedAnswer);
    }
    
    [Test]
    public void TestRemoveAnswer()
    {
        //Given a StringMultipleChoiceAnswerData with an answerData Added to it
        //When RemoveAnswer is called
        //Then the recordedAnswer no longer contains that answervalue, and if it's empty, sets the answer to null.
        
        //Arrange
        var seperator = ";";
        var sut = new StringMultipleChoiceAnswerData(_identifier, _options, _correctAnswer, seperator, MultipleChoiceLogic.ContainsAll);
        var answerValue = "1984";
        sut.AddAnswer(answerValue);

        //Act
        sut.RemoveAnswer(answerValue);

        //Assert
        var hasAnswer = sut.HasAnswer();
        Assert.IsFalse(hasAnswer);
        Assert.AreEqual(null, sut.RecordedAnswer);
    }
    
    [Test]
    public void TestGetRecordedAnswers()
    {
        //Given a StringMultipleChoiceAnswerData with two answers added
        //When GetRecordedAnswers is called
        //Then the recordedAnswer contains the answer added, and hasAnswer returns true.
        
        //Note, the nature of the split method yields an empty entry.
        
        //Arrange
        var seperator = ";";
        var sut = new StringMultipleChoiceAnswerData(_identifier, _options, _correctAnswer, seperator, MultipleChoiceLogic.ContainsAll);
        sut.AddAnswer("1984");
        sut.AddAnswer("1925");
        //Act
        var answersList = sut.GetRecordedAnswers();

        //Assert
        Assert.AreEqual(3, answersList.Length);
        Assert.AreEqual("1984", answersList[0]);
        Assert.AreEqual("1925", answersList[1]);
        Assert.AreEqual("", answersList[2]);
    }
    
    [Test]
    public void TestSetAnswer_Sets_Recorded_Answer()
    {
        //Given a StringMultipleChoiceAnswerData with no answer.
        //When SetAnswer is called with a string value.
        //Then the recorded answer value is set to that string value.
        
        //Arrange
        var sut = new StringMultipleChoiceAnswerData(_identifier, _options, _correctAnswer, _seperator, MultipleChoiceLogic.ContainsAll);
        //Act
        sut.SetAnswer("1884;");
        
        //Assert
        Assert.AreEqual("1884;", sut.RecordedAnswer);
    }
    [Test]
    public void TestClearSession()
    {
        //Given an instance of StringMultipleChoiceAnswerData with a correctly set answer.
        //When the ClearSession is answered
        //Then it clears the playerPrefs, and the recorded answer of the answerData

        //Arrange
        PlayerPrefs.SetString(_identifier, "answer");
        
        var sut = new StringMultipleChoiceAnswerData(_identifier, _options, _correctAnswer, _seperator, MultipleChoiceLogic.ContainsAll);
        //Act
        sut.ClearSession();
        
        //Assert
        Assert.IsFalse(sut.HasAnswer());
        Assert.IsFalse(PlayerPrefs.HasKey(_identifier));
    }
}
