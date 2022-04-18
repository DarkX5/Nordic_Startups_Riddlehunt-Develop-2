using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class TestBooleanAnswerData
{
    private string identifier;
    [SetUp]
    public void Init()
    {
        identifier = "boolean-answer-identifier";
        PlayerPrefs.DeleteKey(identifier);
    }

    [Test]
    public void TestConstructor_Default_Value_False()
    {
        //Given a need for a true/false answer
        //When the answerdata is created
        //Then the default value is false.
        
        //Arrange

        //Act
        var sut = new BooleanAnswerData(identifier);
        
        //Assert
        Assert.IsFalse(sut.HasAnswer());
    }

    [Test]
    public void TestConstructor_NoSession_Has_No_Answer()
    {
        //Arrange

        //Act
        var sut = new BooleanAnswerData(identifier);
        
        //Assert
        Assert.IsFalse(sut.HasAnswer());
    }
    
    [Test]
    public void TestConstructor_HasActiveSession_Has_Answer()
    {
        //Arrange
        PlayerPrefs.SetString(identifier, "answer");

        //Act
        var sut = new BooleanAnswerData(identifier);
        
        //Assert
        Assert.IsTrue(sut.HasAnswer());
    }
    
    [Test]
    public void TestHasAnswer_Returns_True()
    {
        //Given an instance of booleanAnswerData with a wrongfully set answer.
        //When the hasAnswer is called
        //Then it returns true.
        
        //Arrange

        var sut = new BooleanAnswerData(identifier);
        sut.SetAnswer(false);
        
        //Act
        var hasAnswer = sut.HasAnswer();
        
        //Assert
        Assert.IsTrue(hasAnswer);
        Assert.IsFalse(sut.HasCorrectAnswer());
    }
    
    [Test]
    public void TestHasAnswer_Returns_False()
    {
        //Given an instance of booleanAnswerData wiouth an answer.
        //When the hasAnswer is called
        //Then it returns false.
        
        //Arrange

        var sut = new BooleanAnswerData(identifier);
        
        //Act
        var hasAnswer = sut.HasAnswer();
        
        //Assert
        Assert.IsFalse(hasAnswer);
    }
    
    [Test]
    public void TestHasCorectAnswer_Returns_True()
    {
        //Given an instance of booleanAnswerData with a correctly set answer.
        //When the hasCorrectAnswer is called
        //Then it returns true.
        
        //Arrange

        var sut = new BooleanAnswerData(identifier);
        sut.SetAnswer(true);
        
        //Act
        var hasCorrectAnswer = sut.HasCorrectAnswer();
        
        //Assert
        Assert.IsTrue(sut.HasAnswer());
        Assert.IsTrue(hasCorrectAnswer);
    }

    [Test]
    public void TestClearSession()
    {
        //Given an instance of booleanAnswerData with a correctly set answer.
        //When the ClearSession is answered
        //Then it clears the playerPrefs, and the recorded answer of the answerData

        //Arrange
        PlayerPrefs.SetString(identifier, "answer");

        var sut = new BooleanAnswerData(identifier);
        
        //Act
        sut.ClearSession();
        
        //Assert
        Assert.IsFalse(sut.HasAnswer());
    }
}
