using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestHuntSessionPersistor : MonoBehaviour
{
    public string identifier;
    [SetUp]
    public void Init()
    {
        identifier = "testHuntPersistor";
    }
    
    [Test]
    public void TestClearAnswerInSession()
    {
        //Arrange
        var sut = new HuntSessionPersistor();
        sut.SetStringAnswer(identifier, "theValue");
        Assert.IsTrue(sut.HasAnswerInSession(identifier));

        //Act
        sut.ClearAnswerInSession(identifier);

        //Assert
        Assert.IsFalse(sut.HasAnswerInSession(identifier));
    }
    
    [Test]
    public void TestSetStringAnswer()
    {
        //Arrange
        var sut = new HuntSessionPersistor();
        sut.ClearAnswerInSession(identifier);
        Assert.IsFalse(sut.HasAnswerInSession(identifier));
        //Act
        sut.SetStringAnswer(identifier, "theValue");
        //Assert
        Assert.AreEqual(sut.GetStringAnswer(identifier), "theValue");
        Assert.IsTrue(sut.HasAnswerInSession(identifier));
    }
    
    [Test]
    public void TestSetNumericAnswer()
    {
        //Arrange
        var sut = new HuntSessionPersistor();
        sut.ClearAnswerInSession(identifier);
        Assert.IsFalse(sut.HasAnswerInSession(identifier));
        //Act
        sut.SetNumericAnswer(identifier, 42);
        //Assert
        Assert.AreEqual(sut.GetNumericAnswer(identifier), 42);
        Assert.IsTrue(sut.HasAnswerInSession(identifier));
    }
    
    [Test]
    public void TestSetMultipleChoiceAnswerIconsString()
    {
        //Arrange
        var sut = new HuntSessionPersistor();
        sut.ClearAnswerInSession(identifier);
        Assert.IsFalse(sut.HasAnswerInSession(identifier));
        //Act
        sut.SetMultipleChoiceAnswerIconsString(identifier, "A;B;C;");
        //Assert
        Assert.AreEqual(sut.GetMultipleChoiceAnswerIconsString(identifier), "A;B;C;");
        Assert.IsTrue(sut.HasAnswerInSession(identifier));
    }
    
    [Test]
    public void TestTickBooleanAnswer()
    {
        //Arrange
        var sut = new HuntSessionPersistor();
        sut.ClearAnswerInSession(identifier);
        Assert.IsFalse(sut.HasAnswerInSession(identifier));
        //Act
        sut.TickBooleanAnswer(identifier);
        //Assert
        Assert.AreEqual(sut.GetBooleanAnswer(identifier), true);
        Assert.IsTrue(sut.HasAnswerInSession(identifier));
    }
}
