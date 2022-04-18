using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products.models;

public class TestRiddleTextAsset
{
    [Test]
    public void TestConstructor_Gets_RiddleText()
    {
        //Given a user starts a hunt, that contains steps that have a riddleTextAsset
        //When the hunt assets are collected, and the huntstep generated
        //Then this asset is constructed, and the data is collected.
        //- and the isReady is invoked with a value of true;
        
        //Arrange
        bool succeeded = false;
        Action<bool> isReady = (success) => { succeeded = success; };
        
        string riddleText = "riddleText";
        string riddle_uri = "http://uri.com";
        bool cache = false;
        
        var textGetterMock = new Mock<ITextGetter>();
        textGetterMock
            .Setup(x => x.GetText(riddle_uri, cache, It.IsAny<Action<string>>()))
            .Callback<string, bool, Action<string>>((myRiddleURI, myCache, myAction) =>
            {
                myAction.Invoke(riddleText);
            }).Verifiable();
        
        //Act
        var sut = new RiddleTextAsset(textGetterMock.Object, riddle_uri, isReady);
        
        //Assert
        Assert.AreEqual(riddleText, sut.GetText());
        textGetterMock.Verify(x => x.GetText(riddle_uri, cache, It.IsAny<Action<string>>()));
        Assert.IsTrue(succeeded);
    }

    [Test]
    public void TestConstructor_Fails_To_Get_Riddle_Text()
    {
        //Given a user starts a hunt, that contains steps that have a riddleTextAsset
        //When the hunt assets are collected, and the huntstep generated, but something fails in the process.
        //Then the isReady is invoked with a value of "false"
        
        //Arrange
        bool succeeded = true;
        Action<bool> isReady = (success) => { succeeded = success; };
        
        string riddle_uri = null;

        var textGetterMock = new Mock<ITextGetter>();
        textGetterMock
            .Setup(x => x.GetText(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Action<string>>()))
            .Throws(new ArgumentException("some exception"))
            .Verifiable();
        
        //Act
        var sut = new RiddleTextAsset(textGetterMock.Object, riddle_uri, isReady);
        //Assert
        textGetterMock.Verify(x => x.GetText(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Action<string>>()));
        Assert.IsFalse(succeeded);
    }
}