using System;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;
using riddlehouse_libraries.products.Assets;
using riddlehouse_libraries.products.AssetTypes;
using TestRiddlehouseLibraries.LibraryTests.Products.Assets;
using UnityEngine;
using Lib_TextAsset = riddlehouse_libraries.products.Assets.TextAsset;

public class TestTextAnswerComponent
{
    private Mock<ITextAnswerComponentActions> _actionComponentMock;
    private Mock<IViewActions> _huntComponentUIActionsMock;

    private string _answer;
    private string _identifier;

    [SetUp]
    public void Init()
    {
        _actionComponentMock = new Mock<ITextAnswerComponentActions>();
        _huntComponentUIActionsMock = new Mock<IViewActions>();
        
        _answer = "theAnswer";
        _identifier = "TextAnswerComponent-answer-identifier";
        PlayerPrefs.DeleteKey(_identifier);
    }

    [TearDown]
    public void TearDown()
    {
        _actionComponentMock = null;
        _huntComponentUIActionsMock = null;
    }

    [Test]
    public void TestConfigure_Answer_Is_Configured_With_An_AnswerType()
    {
        // Given a user receives a riddle
        // When a riddletab is opened
        // Then an answer should be loaded and that answer should be configured to have a type

        // Arrange
        Action btnAction = () => { };

        var textAsset = new Lib_TextAsset(AssetType.TextAnswer, _answer);
        var answerAsset = new TextAnswerAsset(textAsset);

        _actionComponentMock.Setup(x => x.SetAnswerType(AnswerType.InputTextfield)).Verifiable();
        _actionComponentMock.Setup(x => x.Configure(answerAsset)).Verifiable();
        var sut = new TextTextAnswerComponent(_actionComponentMock.Object, _huntComponentUIActionsMock.Object);

        // Act
        sut.Configure(answerAsset, btnAction);

        // Assert
        _actionComponentMock.Verify(x => x.SetAnswerType(AnswerType.InputTextfield));
        _actionComponentMock.Verify(x => x.Configure(answerAsset));

        Assert.AreSame(btnAction,sut.GetBtnAction());
    }

    [Test]
    public void TestConfigure_With_RecordedAnswer()
    {
        //Given a user returns to a step riddle after completing it once
        //When answer is configured
        //Then the recorded answer is inserted instead.
        
        //Arrange
        string recordedAnswer = "recordedAnswer";
        Action btnAction = () => { };
        var textAsset = new Lib_TextAsset(AssetType.TextAnswer, _answer);
        var answerAsset = new TextAnswerAsset(textAsset);
        answerAsset.SetAnswer(recordedAnswer);
        _actionComponentMock.Setup(x => x.SetAnswerType(AnswerType.InputTextfield)).Verifiable();
        _actionComponentMock.Setup(x => x.Configure(answerAsset)).Verifiable();
        var sut = new TextTextAnswerComponent(_actionComponentMock.Object, _huntComponentUIActionsMock.Object);

        //Act
        sut.Configure(answerAsset, btnAction);

        //Assert
        _actionComponentMock.Verify(x => x.Configure(answerAsset));
    }
    
    [Test]
    public void TestConfigure_Answer_Is_Configured_With_An_AnswerValue_Is_Set_With_Value()
    {
        // Given a user receives a riddle
        // When a riddletab is opened
        // Then an answer should be loaded and that answer should be configured to have a type

        // Arrange
        Action btnAction = () => { };
        
        var textAsset = new Lib_TextAsset(AssetType.TextAnswer, _answer);
        var answerAsset = new TextAnswerAsset(textAsset);
        
        _actionComponentMock.Setup(x => x.SetAnswerType(It.IsAny<AnswerType>()));
        _actionComponentMock.Setup(x => x.Configure(answerAsset)).Verifiable();

        var sut = new TextTextAnswerComponent(_actionComponentMock.Object, _huntComponentUIActionsMock.Object);

        // Act
        sut.Configure(answerAsset, btnAction);

        // Assert
        _actionComponentMock.Verify(x => x.Configure(answerAsset));


    }

    [Test]
    public void TestUpdateAnswer_When_TextField_Is_Updated()
    {
        //Given a user is viewing a riddle
        //When the user is editting the answer
        //Then the answer is updated in the answerData
        
        //Arrange
        var textAsset = new Lib_TextAsset(AssetType.TextAnswer, _answer);
        var answerAsset = new TextAnswerAsset(textAsset);
        _actionComponentMock.Setup(x => x.GetAnswer()).Returns("");
        var sut = new TextTextAnswerComponent(_actionComponentMock.Object, _huntComponentUIActionsMock.Object);
       
        void BtnAction()
        {
        }
        
        //Act
        sut.Configure(answerAsset, BtnAction);
        
        sut.UpdateAnswer("new answer");

        //Assert
        Assert.AreEqual("new answer", answerAsset.RecordedAnswer);
    }

    [Test]
    public void TestSubmitAnswer_Submits_Answer()
    {
        // Given a user submits their answer
        // When answering a riddle
        // Then the submit method is called

        // Arrange
        var hasBeenCalled = false;
        const string testAnswer = "Some Answer";
        var textAsset = new Lib_TextAsset(AssetType.TextAnswer, _answer);
        var answerAsset = new TextAnswerAsset(textAsset);
        answerAsset.SetAnswer(testAnswer);
        _actionComponentMock.Setup(x => x.GetAnswer()).Returns(testAnswer);
        var sut = new TextTextAnswerComponent(_actionComponentMock.Object, _huntComponentUIActionsMock.Object);

        void BtnAction()
        {
            hasBeenCalled = true;
        }

        // Act
        sut.Configure(answerAsset, BtnAction);

        sut.PerformAction();

        // Arrange
        Assert.IsTrue(hasBeenCalled);
    }
}