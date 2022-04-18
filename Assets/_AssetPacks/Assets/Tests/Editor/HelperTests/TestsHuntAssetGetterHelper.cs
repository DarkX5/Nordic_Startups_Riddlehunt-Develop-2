using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using UnityEngine;
[TestFixture]
public class TestsHuntAssetGetterHelper
{
    private HuntStep  _step;
    
    private HuntAsset _storyAsset;
    private string _storyUrl = "http://url.com/a";
    
    private HuntAsset _riddleAsset;
    private string _riddleUrl = "http://url.com/b";
    
    private HuntAsset _answerAsset;
    private string _answerUrl = "http://url.com/answer";
    
    [SetUp]
    public void Init()
    {
        _storyAsset = new HuntAsset() { Type = AssetType.StoryText, Url = _storyUrl };
        _riddleAsset = new HuntAsset() { Type = AssetType.RiddleText, Url = _riddleUrl };
        _answerAsset = new HuntAsset() { Type = AssetType.TextAnswer, Url = _answerUrl };

        var huntAssets = new List<HuntAsset>() {_storyAsset, _riddleAsset, _answerAsset};
        _step = new HuntStep() { Assets = huntAssets, Type = StepType.DisplayRiddleAndSubmitAnswer };
    }

    [TearDown]
    public void TearDown()
    {
        _step = null;
        _storyAsset = null;
        _riddleAsset = null;
        _answerAsset = null;
    }

    [Test]
    public void TestGetAssetUrl_Returns_Url()
    { 
        //Given a huntstep with multiple assets.
        //When GetAssetUrl is called with a type.
        //Then the appropriate url is returned.
        
        //arrange
        var huntAssets = new List<HuntAsset>() {_storyAsset, _riddleAsset, _answerAsset};
        _step = new HuntStep() { Assets = huntAssets, Type = StepType.DisplayRiddleAndSubmitAnswer };
        var sut = new AssetGetterHelper();
            
        //act
        var storyUrl = sut.GetAssetUrl(_step, AssetType.StoryText);
        var riddleUrl = sut.GetAssetUrl(_step, AssetType.RiddleText);
        
        //assert
        Assert.AreEqual(_storyUrl, storyUrl);
        Assert.AreEqual(_riddleUrl, riddleUrl);
    }
    
    [Test]
    public void TestGetAssetUrl_No_Such_Asset_Throws()
    { 
        //Given a huntstep without a riddle asset.
        //When GetAssetUrl is called with type RiddleText.
        //Then the function throws.
        
        //arrange
        var huntAssets = new List<HuntAsset>() {_storyAsset, _answerAsset};
        _step = new HuntStep() { Assets = huntAssets, Type = StepType.DisplayRiddleAndSubmitAnswer };
        var sut = new AssetGetterHelper();
            
        //act & assert
        Assert.Throws<ArgumentException>(() => sut.GetAssetUrl(_step, AssetType.RiddleText));
    }

    [Test]
    public void TestGetAnswerHuntAsset_Returns_AnswerAsset()
    {
        //Given a huntstep with an answer asset
        //When GetAnswerHuntAsset is called with a type.
        //Then the huntAsset is returned.
        
        //Arrange
        var huntAssets = new List<HuntAsset>() {_storyAsset, _riddleAsset, _answerAsset};
        _step = new HuntStep() { Assets = huntAssets, Type = StepType.DisplayRiddleAndSubmitAnswer };
        var sut = new AssetGetterHelper();
        //Act
        var huntAnswerAsset = sut.GetAnswerHuntAsset(_step);
        //Assert
        Assert.AreEqual(_answerAsset, huntAnswerAsset);
    }
    [Test]
    public void TestGetAnswerHuntAsset_No_AnswerAsset_Available_Throws()
    {
        //Given a huntstep with no answer assets
        //When GetAnswerHuntAsset is called.
        //Then the function throws an error.
        
        //Arrange
        var huntAssets = new List<HuntAsset>() {_storyAsset, _riddleAsset};
        _step = new HuntStep() { Assets = huntAssets, Type = StepType.DisplayRiddleAndSubmitAnswer };
        var sut = new AssetGetterHelper();
        //Act & Assert
        Assert.Throws<ArgumentException>(() => sut.GetAnswerHuntAsset(_step));
    }
}
