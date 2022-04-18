using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class TestStringAnswerData
{
    private string answerAsset;

    private Mock<ITextGetter> textGetterMock;

    private string suceedingAnswerAssetUrl;
    private string failingAnswerAssetUrl;
    private string identifier;


    [SetUp]
    public void Init()
    {
        answerAsset = "TheAnswer";
        suceedingAnswerAssetUrl = "www.correctCDNLink.com";
        failingAnswerAssetUrl = "www.failingCDNLink.com";
        
        textGetterMock = new Mock<ITextGetter>();
        textGetterMock
            .Setup(x => 
                x.GetText(suceedingAnswerAssetUrl, false, It.IsAny<Action<string>>()))
            .Callback<string, bool, Action<string>>((theUrl, theCache, theAction) =>
            {
                theAction(answerAsset);
            }).Verifiable();

        textGetterMock
            .Setup(x => 
                x.GetText(failingAnswerAssetUrl, false, It.IsAny<Action<string>>()))
            .Throws<ArgumentException>().Verifiable();

        identifier = "string-answer-identifier";
        PlayerPrefs.DeleteKey(identifier);
    }

    [TearDown]
    public void TearDown()
    {
        textGetterMock = null;
    }

    [Test]
    public void TestConstructor_RetrievesAssetFile_SetsCorrectAnswer_HasNoSession_Invokes_True()
    {
        //Given a textgetter with a succeeding asset retrieval
        //When the texgetter succeeds
        //Then the correct asset is set with the retrieved data.
        
        //Arrange
        bool succeeded = false;
        Action<bool> TextGetterComplete = (success) => { succeeded = success; };

        //Act
        var sut = new StringAnswerData(identifier, textGetterMock.Object, suceedingAnswerAssetUrl, TextGetterComplete);
        
        //Assert
        Assert.AreEqual(answerAsset, sut.CorrectAnswer);
        Assert.IsFalse(sut.HasAnswer());
        Assert.IsTrue(succeeded);
    }
    
    [Test]
    public void TestConstructor_RetrievesAssetFile_SetsCorrectAnswer_Session_Invokes_True()
    {
        //Given a textgetter with a succeeding asset retrieval
        //When the texgetter succeeds
        //Then the correct asset is set with the retrieved data.
        
        //Arrange
        PlayerPrefs.SetString(identifier, answerAsset);
        bool succeeded = false;
        Action<bool> TextGetterComplete = (success) => { succeeded = success; };

        //Act
        var sut = new StringAnswerData(identifier, textGetterMock.Object, suceedingAnswerAssetUrl, TextGetterComplete);
        
        //Assert
        Assert.AreEqual(answerAsset, sut.CorrectAnswer);
        Assert.IsTrue(sut.HasAnswer());
        Assert.IsTrue(sut.HasCorrectAnswer());
        Assert.IsTrue(succeeded);
    }

    [Test]
    public void TestConstructor_CantRetrieveAssetFile_DoesntThrow_Invokes_False()
    { 
        //Given a textgetter with a failing asset retrieval
        //When the textgetter fails
        //Then the exception is handled and the outside system is informed that the asset isn't ready.
        
        //Arrange
        bool succeeded = true;
        Action<bool> TextGetterComplete = (success) => { succeeded = success; };
        
        //Act and Assert
        Assert.DoesNotThrow(() => new StringAnswerData(identifier, textGetterMock.Object, failingAnswerAssetUrl, TextGetterComplete));
        Assert.IsFalse(succeeded);
    }

    [Test]
    public void TestHasAnswer_Returns_True()
    {
        //Given a StringAnswerData with an answer.
        //When HasAnswer is called
        //Then the function returns true.
        
        //Arrange
        bool succeeded = false;
        Action<bool> TextGetterComplete = (success) => { succeeded = success; };

        var sut = new StringAnswerData(identifier, textGetterMock.Object, suceedingAnswerAssetUrl, TextGetterComplete);
        sut.SetAnswer("theAnswer");
        
        //Act
        var hasAnswer = sut.HasAnswer();
        
        //Assert
        Assert.IsTrue(hasAnswer);
    }
    
    [Test]
    public void TestHasAnswer_Returns_False()
    {
        //Given a StringAnswerData with no answer.
        //When HasAnswer is called
        //Then the function returns false.
        
        //Arrange
        Action<bool> TextGetterComplete = (success) => { };

        var sut = new StringAnswerData(identifier, textGetterMock.Object, suceedingAnswerAssetUrl, TextGetterComplete);
        
        //Act
        var hasAnswer = sut.HasAnswer();
        
        //Assert
        Assert.IsFalse(hasAnswer);
    }
    
    [Test]
    public void TestHasCorectAnswer_Returns_True()
    {
        //Given a StringAnswerData with a correct answerValue.
        //When HasCorrectAnswer is called
        //Then the function returns true.
        
        //Arrange
        Action<bool> TextGetterComplete = (success) => {};

        var sut = new StringAnswerData(identifier, textGetterMock.Object, suceedingAnswerAssetUrl, TextGetterComplete);
        sut.SetAnswer(answerAsset);
        
        //Act
        var hasCorrectAnswer = sut.HasCorrectAnswer();
        
        //Assert
        Assert.IsTrue(hasCorrectAnswer);
    }
    
    [Test]
    public void TestHasCorectAnswer_Returns_False()
    {
        //Given a StringAnswerData with an incorrect answerValue.
        //When HasCorrectAnswer is called
        //Then the function returns false.

        //Arrange
        Action<bool> TextGetterComplete = (success) => {};

        var sut = new StringAnswerData(identifier, textGetterMock.Object, suceedingAnswerAssetUrl, TextGetterComplete);
        sut.SetAnswer("IncorrectAnswer");
        
        //Act
        var hasCorrectAnswer = sut.HasCorrectAnswer();
        
        //Assert
        Assert.IsFalse(hasCorrectAnswer);
    }
    
    [Test]
    public void TestSetAnswer_Returns_True()
    {
        //Given a StringAnswerData with no answer.
        //When SetAnswer is called with a string value.
        //Then the recorded answer value is set to that string value.
        
        //Arrange
        Action<bool> TextGetterComplete = (success) => {};

        var sut = new StringAnswerData(identifier, textGetterMock.Object, suceedingAnswerAssetUrl, TextGetterComplete);
        
        //Act
        sut.SetAnswer(answerAsset);
        
        //Assert
        Assert.AreEqual(answerAsset, sut.RecordedAnswer);
    }
    
    [Test]
    public void TestClearSession()
    {
        //Given an instance of StringAnswerData with a correctly set answer.
        //When the ClearSession is answered
        //Then it clears the playerPrefs, and the recorded answer of the answerData

        //Arrange
        PlayerPrefs.SetString(identifier, "answer");
        Action<bool> TextGetterComplete = (success) => {};

        var sut = new StringAnswerData(identifier, textGetterMock.Object, suceedingAnswerAssetUrl, TextGetterComplete);
        
        //Act
        sut.ClearSession();
        
        //Assert
        Assert.IsFalse(sut.HasAnswer());
    }
}
