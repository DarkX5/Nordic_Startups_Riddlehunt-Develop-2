using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

public class TestTextAsset
{
    [Test]
    public void TestConstructor_Gets_Text()
    {
        //Given a hunt needs a textAsset
        //When the hunt assets are collected, and the huntstep generated
        //Then this asset is constructed, and the data is collected.
        //- and the isReady is invoked with a value of true;
        
        bool succeeded = false;
        Action<bool> isReady = (success) => { succeeded = success; };
        
        string text = "text";
        string uri = "http://uri.com";
        bool cache = false;
        
        var textGetterMock = new Mock<ITextGetter>();
        textGetterMock
            .Setup(x => x.GetText(uri, cache, It.IsAny<Action<string>>()))
            .Callback<string, bool, Action<string>>((myStoryURI, myCache, myAction) =>
            {
                myAction.Invoke(text);
            }).Verifiable();
        
        //Act
        var sut = new TextAsset(textGetterMock.Object, uri, isReady);
        
        //Assert
        Assert.AreEqual(text, sut.GetText());
        textGetterMock.Verify(x => x.GetText(uri, cache, It.IsAny<Action<string>>()));
        Assert.IsTrue(succeeded);
        Assert.AreEqual(text, sut.Text);
    }
    [Test]
    public void TestConstructor_Fails_To_Get_Story_Text()
    {
        //Given a user starts a hunt, that need a textAsset
        //When the hunt assets are collected, and the huntstep generated, but something fails in the process.
        //Then the isReady is invoked with a value of "false"
        
        //Arrange
        bool succeeded = true;
        Action<bool> isReady = (success) => { succeeded = success; };
        
        string uri = null;
        
        var textGetterMock = new Mock<ITextGetter>();
        textGetterMock
            .Setup(x => x.GetText(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Action<string>>()))
            .Throws(new ArgumentException("some exception"))
            .Verifiable();
        //Act
        var sut = new TextAsset(textGetterMock.Object, uri, isReady);
        //Assert
        textGetterMock.Verify(x => x.GetText(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Action<string>>()));
        Assert.IsFalse(succeeded);
    }
}
