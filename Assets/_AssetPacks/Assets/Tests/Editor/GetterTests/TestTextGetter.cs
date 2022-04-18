using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;
[TestFixture]
public class TestTextGetter 
{
    [Test]
    public void TestFactory_Throws()
    {
        // Given no monobehavior
        // When constructing the TextGetter
        // Then an exception is thrown
        TextGetter sut;
        Assert.Throws<ArgumentException>(() => sut = TextGetter.Factory(null));
    }
    [Test]
    public void TestFactory_Succeeds()
    {
        // Given a monobehavior
        // When constructing the TextGetter
        // Then a TextGetter is returned
        GameObject go = new GameObject();
        var mono = go.AddComponent<HuntHomeComponentBehaviour>();
        TextGetter sut = TextGetter.Factory(mono);
        Assert.IsNotNull(sut);
    }

    [Test]
    public void TestGetText()
    {
        string textLink = "thisIsALink.com";
        bool performCache = false;
        Action<string> textRetrievedAction = (text) => { };
        var textGetterBehaviorMock = new Mock<ITextGetterActions>();
        textGetterBehaviorMock
            .Setup(x => x.GetText(textLink, performCache, It.IsAny<Action<string>>()))
            .Callback<string, bool, Action<string>>((link, cache, imageRetrieved) =>
            {
                imageRetrieved.Invoke(null);
            });
        var SUT = new TextGetter(textGetterBehaviorMock.Object);
        SUT.GetText(textLink, performCache, textRetrievedAction);
        textGetterBehaviorMock.Verify(x => x.GetText(textLink, performCache, textRetrievedAction));
    }

    [Test]
    public void TestDisposeSelf()
    {
        //Given a textGetter
        //When the textGetter has completed its purpose.
        //Then the textGetter is disposed. Releasing its resources.
        
        var textGetterBehaviorMock = new Mock<ITextGetterActions>();
        textGetterBehaviorMock.Setup(x => x.DisposeSelf()).Verifiable();

        var sut = new TextGetter(textGetterBehaviorMock.Object);
        sut.DisposeSelf();
        textGetterBehaviorMock.Verify(x => x.DisposeSelf());
    }
}
