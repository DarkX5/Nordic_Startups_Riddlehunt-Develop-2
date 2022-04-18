using System;
using System.Collections.Generic;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using UnityEngine;

public class TestDisplayRiddleAndSubmitAnswerStep
{
    private string _answerString;
    private int? _answerValue;
    private string _answerListJson;

    private string _answer_1;
    private string _answer_2;
    private string _answer_3;
    private List<string> _answerList;

    private Action<bool> readyAction;

    private string _storyUrl;
    private string _riddleUrl;
    private string _answerStringUrl;

    private string _storyText;
    private string _riddleText;
    
    private HuntAsset _storyAsset;
    private HuntAsset _riddleAsset;
    private HuntAsset _answerAsset;
    private HuntAsset _imageListAsset;
    
    private HuntStep _step;
    private Mock<ITextGetter> _textGetterMock;
    private Mock<IImageGetter> _imageGetterMock;

    private string _imageListUrl;
    private string _image1Url;
    private string _image2Url;
    private string _image3Url;

    private Sprite _image1;
    private Sprite _image2;
    private Sprite _image3;

    private List<string> _imageUrls;

    public void SetupTextGetterMocks(HuntAsset answerAsset, string output)
    {
        _storyAsset = new HuntAsset() {Type = AssetType.StoryText, Url = _storyUrl};
        _riddleAsset = new HuntAsset() {Type = AssetType.RiddleText, Url = _riddleUrl};
        _answerAsset = answerAsset;
        _step = new HuntStep() { Type = StepType.DisplayRiddleAndSubmitAnswer, Assets = new List<HuntAsset>() { _storyAsset, _riddleAsset, _answerAsset} };

        _textGetterMock
            .Setup(x => x.GetText(_storyUrl, false, It.IsAny<Action<string>>()))
            .Callback<string, bool, Action<string>>((myStory, myCache, myAction) =>
            {
                myAction.Invoke(_storyText);
            }).Verifiable();
        
        _textGetterMock
            .Setup(x => x.GetText(_riddleUrl, false, It.IsAny<Action<string>>()))
            .Callback<string, bool, Action<string>>((myRiddle, myCache, myAction) =>
            {
                myAction.Invoke(_riddleText);
            }).Verifiable();

        _textGetterMock
            .Setup(x => x.GetText(answerAsset.Url, false, It.IsAny<Action<string>>()))
            .Callback<string, bool, Action<string>>((myAnswer, myCache, myAction) =>
            {
                myAction.Invoke(output);
            }).Verifiable();
    }

    public void SetupImageGetterMocks(List<string> imageUrls)
    {
        _image1 = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        _image2 = Sprite.Create(Texture2D.grayTexture, Rect.zero, Vector2.left);
        _image3 = Sprite.Create(Texture2D.redTexture, Rect.zero, Vector2.right);
        
        _imageUrls = imageUrls;
        var imageSpriteList = new Dictionary<string, Sprite>() {};
        imageSpriteList.Add(_image1Url, _image1);
        imageSpriteList.Add(_image2Url, _image2);
        imageSpriteList.Add(_image3Url, _image3);

        _imageGetterMock = new Mock<IImageGetter>();
        _imageListAsset = new HuntAsset() { Type = AssetType.ImageList, Url = _imageListUrl };
        _step.Assets.Add(_imageListAsset);

        _textGetterMock.Setup(x => x.GetText(_imageListUrl, false, It.IsAny<Action<string>>()))
            .Callback<string, bool, Action<string>>((myUrl, myCache, myAction) =>
            {
                myAction.Invoke(JsonConvert.SerializeObject(imageUrls));
            }).Verifiable();

        foreach (var url in _imageUrls)
        {
            _imageGetterMock
                .Setup(x => x.GetImage(url, false, It.IsAny<Action<Sprite>>()))
                .Callback<string, bool, Action<Sprite>>((myUrl, myCache, myAction) =>
                {
                    myAction.Invoke(imageSpriteList[url]);
                })
                .Verifiable();
        }
    }


    [SetUp]
    public void Init()
    {
        readyAction = (success) => { };
        
        _answerString = "answer";
        _answerValue = 42;
        _answer_1 = "1825";
        _answer_2 = "1925";
        _answer_3 = "1984";
        _answerListJson =  "{\"type\": \"Text\",\"answers\": [{\"value\": \""+_answer_1+"\",\"correct\": false},{\"value\": \""+_answer_2+"\",\"correct\": false},{\"value\": \""+_answer_3+"\",\"correct\": true}]}";
        _answerList = new List<string>() { _answer_1, _answer_2, _answer_3 };
        _storyUrl = "http://www.storyUrl.com";
        _riddleUrl = "http://www.riddleUrl.com";
        _answerStringUrl = "http://www.answerUrl.com/string";

        _storyText = "story text";
        _riddleText = "riddle text";
        
        _textGetterMock = new Mock<ITextGetter>();

        _answerAsset = new HuntAsset() { Type = AssetType.TextAnswer, Url = _answerStringUrl };
        SetupTextGetterMocks(_answerAsset, _answerString);
        
        
        _imageListUrl = "http://www.imageUrl.com/list";
        
        _image1Url = "http://www.imageUrl.com/1";
        _image2Url = "http://www.imageUrl.com/2";
        _image3Url = "http://www.imageUrl.com/3";
        
        SetupImageGetterMocks(new List<string>() { _image1Url, _image2Url, _image3Url });
    }

    [TearDown]
    public void TearDown()
    {
        _answerString = null;
        _answerValue = null;
        _answerListJson = null;

        readyAction = null;

        _storyUrl = null;
        _riddleUrl = null;

        _storyText = null;
        _riddleText = null;
        
        _textGetterMock = null;

        _step = null;

        _imageGetterMock = null;
        
        _imageListUrl = null;
        
        _image1Url = null;
        _image2Url = null;
        _image3Url = null;
        
        _image1 = null;
        _image2 = null;
        _image3 = null;
    }
    
    [Test]
    public void TestNewInstance_wrong_type_throws()
    {
        //Given an unexpected type of huntStep
        //When constructor is called.
        //Then the constructor throws an error.
        
        //Arrange
        _step = new HuntStep() { Type = StepType.HuntResolutionAndEnd, Assets = new List<HuntAsset>() { _storyAsset, _riddleAsset } };

        //Act and Assert
        Assert.Throws<ArgumentException>(() => new DisplayRiddleAndSubmitAnswerStep(_step, _textGetterMock.Object, _imageGetterMock.Object, readyAction));
    }

    [Test]
    public void TestGetStepType()
    {
        //Given a new DisplayRiddleAndSubmitAnswerStep with corresponding type
        //When GetStepType is called
        //returns correctStepType
        
        //Arrange
        var sut = new DisplayRiddleAndSubmitAnswerStep(_step, _textGetterMock.Object, _imageGetterMock.Object, readyAction);
        //Act
        var stepType = sut.GetStepType();
        //Assert
        Assert.AreEqual(StepType.DisplayRiddleAndSubmitAnswer, stepType);
    }

    [Test]
    public void TestGetStoryText()
    {
        //Given a new DisplayRiddleAndSubmitAnswerStep
        //When GetStoryText is called
        //Then the storyText element is returned.
        
        //Arrange
        var sut = new DisplayRiddleAndSubmitAnswerStep(_step, _textGetterMock.Object, _imageGetterMock.Object, readyAction);
        //Act
        var storyText = sut.GetStoryText();
        //Assert
        Assert.AreEqual(_storyText, storyText);
    }
    [Test]
    public void TestGetRiddleText()
    {
        //Given a new DisplayRiddleAndSubmitAnswerStep
        //When GetRiddleText is called
        //Then the riddleText element is returned.
        //Arrange
        var sut = new DisplayRiddleAndSubmitAnswerStep(_step, _textGetterMock.Object, _imageGetterMock.Object, readyAction);
        //Act
        var riddleText = sut.GetRiddleText();
        //Assert
        Assert.AreEqual(_riddleText, riddleText);
    }
    [Test]
    public void TestGetAnswerData_TextAnswer_Returns_AnswerString()
    {
        //Given a new DisplayRiddleAndSubmitAnswerStep
        //When GetAnswerData is called
        //Then the AnswerText is returned
        
        //Arrange
        _answerAsset = new HuntAsset() { Type = AssetType.TextAnswer, Url = _answerStringUrl };
        SetupTextGetterMocks(_answerAsset, _answerString);
        SetupImageGetterMocks(new List<string>(){_image1Url});

        var sut = new DisplayRiddleAndSubmitAnswerStep(_step, _textGetterMock.Object, _imageGetterMock.Object, readyAction);
        //ACct
        var data = (StringAnswerData)sut.GetAnswerData();
        //Assert
        Assert.AreEqual(_answerString, data.CorrectAnswer);
    }
    [Test]
    public void TestGetAnswerData_NumericAnswer_Returns_AnswerInt()
    {
        //Given a new DisplayRiddleAndSubmitAnswerStep
        //When GetAnswerData is called
        //Then the answerInt element is returned.

        //Arrange
        var answerValueUrl = "http://www.answerUrl.com/int";
        _answerAsset = new HuntAsset() { Type = AssetType.NumericAnswer, Url = answerValueUrl };
        var output = _answerValue.ToString();
        SetupTextGetterMocks(_answerAsset, output);
        SetupImageGetterMocks(new List<string>(){_image1Url});

        var sut = new DisplayRiddleAndSubmitAnswerStep(_step, _textGetterMock.Object, _imageGetterMock.Object, readyAction);
        //Act
        var data = (NumericAnswerData)sut.GetAnswerData();
        //Assert
        Assert.AreEqual(_answerValue, data.CorrectAnswer);
    }
    [Test]
    public void TestGetAnswerData_MultipleChoiceTextAnswer_Returns_AnswerStringList()
    {
        //Given a new DisplayRiddleAndSubmitAnswerStep
        //When GetAnswerData is called
        //Then the answerStringList element is returned.
        
        //NOTE -> we've replaced the system using multiplechoicetextanswers,
        //and are now using library step model -> we're therefore deprecating this test and expecting it to throw an error.
        
        //Arrange
        var answerListUrl = "http://www.answerUrl.com/list<string>";
        _answerAsset = new HuntAsset() { Type = AssetType.MultipleChoiceTextAnswer, Url = answerListUrl };
        SetupTextGetterMocks(_answerAsset, _answerListJson);
        SetupImageGetterMocks(new List<string>(){_image1Url});
        
        //Act & Assert
        Assert.Throws<ArgumentException>(() => new DisplayRiddleAndSubmitAnswerStep(_step, _textGetterMock.Object, _imageGetterMock.Object, readyAction));

    }

    [Test]
    public void TestGetImages_SingleImageLoaded_Returns_List_Count_1()
    {
        //Given a new DisplayRiddleAndSubmitAnswerStep and a huntstep containing an imagelist huntAsset with 1 imagelink
        //When GetImages is called
        //Then the Image list is returned with 1 sprite in it.
        
        //Arrange
        SetupImageGetterMocks(new List<string>(){_image1Url});
        //Act
        var sut = new DisplayRiddleAndSubmitAnswerStep(_step, _textGetterMock.Object, _imageGetterMock.Object, readyAction);
        
        //Assert
        Assert.AreEqual(1, sut.GetRiddleImages().Count);
        Assert.AreEqual(_image1, sut.GetRiddleImages()[0]);
    }
    [Test]
    public void TestGetImages_SingleImageLoaded_Returns_List_Count_3()
    {
        //Given a new DisplayRiddleAndSubmitAnswerStep and a huntstep containing an imagelist huntAsset with 1 imagelink
        //When GetImages is called
        //Then the Image list is returned with 1 sprite in it.
        
        //Arrange
        SetupImageGetterMocks(new List<string>(){_image1Url, _image2Url, _image3Url});
        //Act
        var sut = new DisplayRiddleAndSubmitAnswerStep(_step, _textGetterMock.Object, _imageGetterMock.Object, readyAction);
        
        //Assert
        Assert.AreEqual(3, sut.GetRiddleImages().Count);
        Assert.AreEqual(_image1, sut.GetRiddleImages()[0]);
        Assert.AreEqual(_image2, sut.GetRiddleImages()[1]);
        Assert.AreEqual(_image3, sut.GetRiddleImages()[2]);

    }
}
