using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using UnityEngine;

[TestFixture]
public class TestResolutionVideoAndEndStep
{
    private Mock<ITextGetter> textGetterMock;
    
    private string _endTextUrl;
    private string _endText;
    
    private string _resolutionUrl;
    
    [SetUp]
    public void Init()
    {
        textGetterMock = new Mock<ITextGetter>();

        _endTextUrl = "https://endtextUrl.com";
        _endText = "end text";

        _resolutionUrl = "https://resolutionVideoUrl.com";
        
    }

    [TearDown]
    public void TearDown()
    {
        textGetterMock = null;
    }

    [Test]
    public void TestConstructor_Gets_EndText_And_ResolutionVideoLink()
    {
        //Given a ResolutionVideoAndEnd Huntstep
        //When a new ResolutionVideoAndEnd step is created
        //Then the assets are downloaded successfully.
        
        //Arrange
        textGetterMock.Setup(x => x.GetText(_endTextUrl, false, It.IsAny<Action<string>>()))
            .Callback<string, bool, Action<string>>((theUrl, theCache, theAction) =>
            {
                theAction.Invoke(_endText);
            }).Verifiable();

        HuntAsset resolutionVideoAsset = new HuntAsset()
        {
            Type = AssetType.VideoToPlay,
            Url = _resolutionUrl
        };

        HuntAsset endtextAsset = new HuntAsset()
        {
            Type = AssetType.EndText,
            Url = _endTextUrl
        };
        
        HuntStep step = new HuntStep() { Type = StepType.ResolutionVideoAndEnd, Assets = new List<HuntAsset>() { resolutionVideoAsset, endtextAsset } };

        bool didSucceed = false;
        
        //Act
        var sut = new ResolutionVideoAndEndStep(step, textGetterMock.Object, succes =>
        {
            didSucceed = succes;
        });
        
        //Assert
        Assert.IsFalse(sut.HasAnswer());
        textGetterMock.Verify(x => x.GetText(_endTextUrl, false, It.IsAny<Action<string>>()));
        Assert.IsTrue(didSucceed);
    }
    
    [Test]
    public void TestConstructor_Fails_ReturnsFalse()
    {
        //Given a failing ResolutionVideoAndEnd Huntstep.
        //When a new ResolutionVideoAndEnd step is created.
        //Then stepmodel quits and returns false.
        
        //Arrange
        textGetterMock.Setup(x => x.GetText(_endTextUrl, false, It.IsAny<Action<string>>()))
            .Throws(new ArgumentException("failed")).Verifiable();

        HuntAsset resolutionVideoAsset = new HuntAsset()
        {
            Type = AssetType.VideoToPlay,
            Url = _resolutionUrl
        };

        HuntAsset endtextAsset = new HuntAsset()
        {
            Type = AssetType.EndText,
            Url = _endTextUrl
        };
        
        HuntStep step = new HuntStep() { Type = StepType.ResolutionVideoAndEnd, Assets = new List<HuntAsset>() { resolutionVideoAsset, endtextAsset } };

        bool didSucceed = true;
        
        //Act
        var sut = new ResolutionVideoAndEndStep(step, textGetterMock.Object, succes =>
        {
            didSucceed = succes;
        });
        
        //Assert
        textGetterMock.Verify(x => x.GetText(_endTextUrl, false, It.IsAny<Action<string>>()));
        Assert.IsFalse(didSucceed);
    }

    [Test]
    public void TestGetResolutionVideoLink()
    {
        //Given a ResolutionVideoAndEndStep
        //When GetResolutionVideoLink is called
        //Then returns the collected videoLink
        
        //Arrange
        textGetterMock.Setup(x => x.GetText(_endTextUrl, false, It.IsAny<Action<string>>()))
            .Callback<string, bool, Action<string>>((theUrl, theCache, theAction) =>
            {
                theAction.Invoke(_endText);
            }).Verifiable();

        HuntAsset resolutionVideoAsset = new HuntAsset()
        {
            Type = AssetType.VideoToPlay,
            Url = _resolutionUrl
        };

        HuntAsset endtextAsset = new HuntAsset()
        {
            Type = AssetType.EndText,
            Url = _endTextUrl
        };
        
        HuntStep step = new HuntStep() { Type = StepType.ResolutionVideoAndEnd, Assets = new List<HuntAsset>() { resolutionVideoAsset, endtextAsset } };

        bool didSucceed = false;
        
        //Act
        var sut = new ResolutionVideoAndEndStep(step, textGetterMock.Object, succes =>
        {
            didSucceed = succes;
        });
        
        //Assert
        Assert.AreEqual(_resolutionUrl, sut.GetResolutionVideoLink());
    }
    
    [Test]
    public void TestGetEndtext()
    {
        //Given a ResolutionVideoAndEndStep
        //When GetEndText is called
        //Then returns the collected endText
        
        //Arrange
        textGetterMock.Setup(x => x.GetText(_endTextUrl, false, It.IsAny<Action<string>>()))
            .Callback<string, bool, Action<string>>((theUrl, theCache, theAction) =>
            {
                theAction.Invoke(_endText);
            }).Verifiable();

        HuntAsset resolutionVideoAsset = new HuntAsset()
        {
            Type = AssetType.VideoToPlay,
            Url = _resolutionUrl
        };

        HuntAsset endtextAsset = new HuntAsset()
        {
            Type = AssetType.EndText,
            Url = _endTextUrl
        };
        
        HuntStep step = new HuntStep() { Type = StepType.ResolutionVideoAndEnd, Assets = new List<HuntAsset>() { resolutionVideoAsset, endtextAsset } };

        bool didSucceed = false;
        
        //Act
        var sut = new ResolutionVideoAndEndStep(step, textGetterMock.Object, succes =>
        {
            didSucceed = succes;
        });
        
        //Assert
        Assert.AreEqual(_endText, sut.GetEndText());
    }

}
