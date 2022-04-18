using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Security.Cryptography;
using UnityEngine;
using Answers.MultipleChoice.Data.Icon;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using riddlehouse_libraries.products.models;

[TestFixture]
public class TestIconMultipleChoiceAnswerData
{
    private Mock<ITextGetter> _textGetterMock;
    private Mock<IImageGetter> _imageGetterMock;

    private Sprite _image1;
    private Sprite _image2;
    private Sprite _image3;

    private string _imageUrl1;
    private string _imageUrl2;
    private string _imageUrl3;

    private string _answerAssetUrl;
    private string _answerAssetResponseJson;
    private string identifier;

    [SetUp]
    public void Init()
    {
        _textGetterMock = new Mock<ITextGetter>();
        _imageGetterMock = new Mock<IImageGetter>();
        
        _image1 = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        _image2 = Sprite.Create(Texture2D.redTexture, Rect.zero, Vector2.down);
        _image3 = Sprite.Create(Texture2D.grayTexture, Rect.zero, Vector2.down);

        _imageUrl1 = "https://imageUrl1.com";
        _imageUrl2 = "https://imageUrl2.com";
        _imageUrl3 = "https://imageUrl3.com";

        _answerAssetUrl = "https://assetUrl.com";

        _answerAssetResponseJson = JsonConvert.SerializeObject(new IconMultipleChoiceAnswerData.AnswerCollection()
        {
            Type = MultipleChoiceTypes.Icon,
            Answers = new List<IconMultipleChoiceAnswerData.AnswerOption>()
            {
                new IconMultipleChoiceAnswerData.AnswerOption()
                {
                    Correct = true,
                    IconLink = _imageUrl1,
                    Value = "a"
                },
                new IconMultipleChoiceAnswerData.AnswerOption()
                {
                    Correct = true,
                    IconLink = _imageUrl2,
                    Value = "b"
                },
                new IconMultipleChoiceAnswerData.AnswerOption()
                {
                    Correct = false,
                    IconLink = _imageUrl3,
                    Value = "c"
                }
            }
        });

        _textGetterMock.Setup(x=> x.GetText(_answerAssetUrl, false, It.IsAny<Action<string>>()))            
            .Callback<string, bool, Action<string>>((theUrl, theCache, theAction ) =>
            {
                theAction(_answerAssetResponseJson);
            })
            .Verifiable();
        
        _imageGetterMock.Setup(x => x.GetImage(_imageUrl1, false, It.IsAny<Action<Sprite>>()))
            .Callback<string, bool, Action<Sprite>>((theUrl, theCache, theAction ) =>
            {
                theAction(_image1);
            })
            .Verifiable();
        _imageGetterMock.Setup(x => x.GetImage(_imageUrl2, false, It.IsAny<Action<Sprite>>()))            .Callback<string, bool, Action<Sprite>>((theUrl, theCache, theAction ) =>
            {
                theAction(_image2);
            })
            .Verifiable();
        _imageGetterMock.Setup(x => x.GetImage(_imageUrl3, false, It.IsAny<Action<Sprite>>()))            .Callback<string, bool, Action<Sprite>>((theUrl, theCache, theAction ) =>
            {
                theAction(_image3);
            })
            .Verifiable();
        
        identifier = "iconMultipleChoice-answer-identifier";
        PlayerPrefs.DeleteKey(identifier);
    }

    [TearDown]
    public void TearDown()
    {
        _textGetterMock = null;
        _imageGetterMock = null;
    }
    
    
    [Test]
    public void TestConstructor_Succeeds_DownloadsOwnAssets_AssetReady_HasNoSession_True()
    {
        //Given a new IconMultipleChoiceAnswerData that is called with correct asset URL info
        //When the system initializes
        //Then the assets are downloaded successfully, and the action reports true.
        
        //arrange
        var didSucceed = false;
        //Act
        var sut = new IconMultipleChoiceAnswerData(identifier, _textGetterMock.Object, _imageGetterMock.Object, _answerAssetUrl,
            (success) => { didSucceed = success; });
        //Assert
        Assert.IsFalse(sut.HasAnswer());
        Assert.IsTrue(didSucceed);
    }
    [Test]
    public void TestConstructor_Succeeds_DownloadsOwnAssets_AssetReady_HasSession_True()
    {
        //Given a new IconMultipleChoiceAnswerData that is called with correct asset URL info
        //When the system initializes
        //Then the assets are downloaded successfully, and the action reports true.
        
        //arrange
        PlayerPrefs.SetString(identifier, "a;b");
        
        var didSucceed = false;
        //Act
        var sut = new IconMultipleChoiceAnswerData(identifier, _textGetterMock.Object, _imageGetterMock.Object, _answerAssetUrl,
            (success) => { didSucceed = success; });
        //Assert
        Assert.IsTrue(sut.HasAnswer());
        Assert.IsTrue(sut.HasCorrectAnswer());
        Assert.IsTrue(didSucceed);
    }

    [Test]
    public void TestConstructor_Fails_DownloadsOwnAssets_AssetReady_False()
    {
        //Given a new IconMultipleChoiceAnswerData that is called with corruped asset URL info
        //When the system initializes
        //Then the error is caught and the action is called with false.
        
        //Arrange
        _textGetterMock = new Mock<ITextGetter>();
        _textGetterMock.Setup(x=> x.GetText(_answerAssetUrl, false, It.IsAny<Action<string>>()))            
            .Throws(new ArgumentException("something failed"))
            .Verifiable();
        
        var didSucceed = true;
        //Act
        var sut = new IconMultipleChoiceAnswerData(identifier, _textGetterMock.Object, _imageGetterMock.Object, _answerAssetUrl,
            (success) => { didSucceed = success; });
       //Assert
        Assert.IsFalse(didSucceed);
    }

    [Test]
    public void TestHasAnswer_HasAnswer_ReturnsTrue()
    {
        //Given an existing IconMultipleChoiceAnswerData that has a recorded answer
        //When HasAnswer is called
        //Then it returns true.
        
        //Arrange
        var sut = new IconMultipleChoiceAnswerData(identifier, _textGetterMock.Object, _imageGetterMock.Object, _answerAssetUrl,
            (success) => {});
        
        sut.SetAnswer("a");
        //Act and assert
        Assert.IsTrue(sut.HasAnswer());
    }
    
    [Test]
    public void TestHasAnswer_DoesNot_HaveAnswer_ReturnsFalse()
    {
        //Given an existing IconMultipleChoiceAnswerData that doesn't not have an answer
        //When HasAnswer is called
        //Then it returns false.
        //Arrange
        var sut = new IconMultipleChoiceAnswerData(identifier, _textGetterMock.Object, _imageGetterMock.Object, _answerAssetUrl,
            (success) => {});
        //Act and Assert
        Assert.IsFalse(sut.HasAnswer());
    }

    [TestCase("a")]
    [TestCase("a;c")]
    [TestCase("a;b;c")]
    [Test]
    public void TestHasCorrectAnswer_RecordedAnswerIncorrect_Returns_False(string answer)
    {
        //Given an existing IconMultipleChoiceAnswerData with correct answer ab, and recordedAnswer of "abc"
        //When HasCorrectAnswer is called
        //Then the function returns false
        
        //Arrange
        var sut = new IconMultipleChoiceAnswerData(identifier, _textGetterMock.Object, _imageGetterMock.Object, _answerAssetUrl,
            (success) => {});
        
        sut.SetAnswer(answer);
        //Act and Assert
        Assert.IsFalse(sut.HasCorrectAnswer());
    }
    
    [Test]
    public void TestHasCorrectAnswer_RecordedAnswerCorrect_Returns_True()
    {
        //Given an existing IconMultipleChoiceAnswerData with correct answer ab and recordedAnswer of "ab"
        //When HasCorrectAnswer is called with parameter "ab"
        //Then the function returns true
        
        //Arrange
        var sut = new IconMultipleChoiceAnswerData(identifier, _textGetterMock.Object, _imageGetterMock.Object, _answerAssetUrl,
            (success) => {});
        sut.SetAnswer("a;b;");
        //Act and Assert
        Assert.IsTrue(sut.HasCorrectAnswer());
    }

    [Test]
    public void TestSetAnswer()
    {
        //Given an existing IconMultipleChoiceAnswerData.
        //When SetAnswer is called.
        //Then the answer is recorded.
        //Arrange
        var sut = new IconMultipleChoiceAnswerData(identifier, _textGetterMock.Object, _imageGetterMock.Object, _answerAssetUrl,
            (success) => {});
        //Act
        sut.SetAnswer("a;b");
        //Assert
        Assert.AreEqual("a;b", sut.RecordedAnswer);
    }
    [Test]
    public void TestClearSession()
    {
        //Given an instance of IconMultipleChoiceAnswerData with a correctly set answer.
        //When the ClearSession is answered
        //Then it clears the playerPrefs, and the recorded answer of the answerData

        //Arrange
        PlayerPrefs.SetString(identifier, "a;b");

        var sut = new IconMultipleChoiceAnswerData(identifier, _textGetterMock.Object, _imageGetterMock.Object, _answerAssetUrl,
            (success) => {});
        
        //Act
        sut.ClearSession();
        
        //Assert
        Assert.IsFalse(sut.HasAnswer());
    }
}