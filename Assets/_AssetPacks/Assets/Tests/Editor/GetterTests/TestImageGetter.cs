using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class TestImageGetter
{
    [Test]
    public void TestFactory_Throws()
    {
        // Given no monobehavior
        // When constructing the imageGetter
        // Then an exception is thrown
        ImageGetter sut;
        Assert.Throws<ArgumentException>(() => sut = ImageGetter.Factory(null));
    }
    [Test]
    public void TestFactory_Succeeds()
    {
        // Given a monobehavior
        // When constructing the imageGetter
        // Then an imageGetter is returned
        GameObject go = new GameObject();
        var mono = go.AddComponent<HuntHomeComponentBehaviour>();
        ImageGetter sut = ImageGetter.Factory(mono);
        Assert.IsNotNull(sut);
    }
    [Test]
    public void TestLoadFile()
    {
        string imgLink = "thisIsALink.com";
        bool performCache = false;
        Action<Sprite> imageRetrievedAction = (sprite) => { };
        var imageGetterBehaviorMock = new Mock<IImageGetterActions>();
        imageGetterBehaviorMock
            .Setup(x => x.GetImage(imgLink, performCache, It.IsAny<Action<Sprite>>()))
            .Callback<string, bool, Action<Sprite>>((link, cache, imageRetrieved) =>
            {
                imageRetrieved.Invoke(null);
            });
        var SUT = new ImageGetter(imageGetterBehaviorMock.Object);
        SUT.GetImage(imgLink, performCache, imageRetrievedAction);
        imageGetterBehaviorMock.Verify(x => x.GetImage(imgLink, performCache, imageRetrievedAction));
    }
    [Test]
    public void TestDisposeSelf()
    {
        //Given a ImageGetter
        //When the ImageGetter has completed its purpose.
        //Then the ImageGetter is disposed. Releasing its resources.
        
        var imageGetterBehaviorMock = new Mock<IImageGetterActions>();
        imageGetterBehaviorMock.Setup(x => x.DisposeSelf()).Verifiable();

        var sut = new ImageGetter(imageGetterBehaviorMock.Object);
        sut.DisposeSelf();
        imageGetterBehaviorMock.Verify(x => x.DisposeSelf());
    }
}
