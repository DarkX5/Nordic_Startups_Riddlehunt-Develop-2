using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;

public class TestEndTextAsset
{
    [Test]
    public void TestConstructor_Gets_endText()
    {
        //Given a user starts a hunt, that contains steps that have an endTextAsset
        //When the hunt assets are collected, and the huntstep generated
        //Then this asset is constructed, and the data is collected.
        //- and the isReady is invoked with a value of true;

        //Arrange
        bool succeeded = false;
        Action<bool> isReady = (success) => { succeeded = success; };
        
        string endText = "endText";
        string end_uri = "http://uri.com";
        bool cache = false;
        
        var textGetterMock = new Mock<ITextGetter>();
        textGetterMock
            .Setup(x => x.GetText(end_uri, cache, It.IsAny<Action<string>>()))
            .Callback<string, bool, Action<string>>((myendURI, myCache, myAction) =>
            {
                myAction.Invoke(endText);
            }).Verifiable();
        //Act
        var sut = new EndTextAsset(textGetterMock.Object, end_uri, isReady);
        //Assert
        Assert.AreEqual(endText, sut.GetText());
        textGetterMock.Verify(x => x.GetText(end_uri, cache, It.IsAny<Action<string>>()));
        Assert.IsTrue(succeeded);
    }

    [Test]
    public void TestConstructor_Fails_To_Get_end_Text()
    {
        //Given a user starts a hunt, that contains steps that have a endTextAsset
        //When the hunt assets are collected, and the huntstep generated, but something fails in the process.
        //Then the isReady is invoked with a value of "false"
        
        //Arrange
        bool succeeded = true;
        Action<bool> isReady = (success) => { succeeded = success; };
        
        string end_uri = null;
        
        var textGetterMock = new Mock<ITextGetter>();
        textGetterMock
            .Setup(x => x.GetText(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Action<string>>()))
            .Throws(new ArgumentException("some exception"))
            .Verifiable();
        //Act
        var sut = new EndTextAsset(textGetterMock.Object, end_uri, isReady);
        //Assert
        textGetterMock.Verify(x => x.GetText(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Action<string>>()));
        Assert.IsFalse(succeeded);
    }
}
