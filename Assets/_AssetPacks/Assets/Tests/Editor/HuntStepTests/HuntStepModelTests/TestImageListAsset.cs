using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using UnityEngine;

public class TestImageListAsset
{
 [Test]
    public void TestDownloadImage_ImageIsReturned_Succeeds()
    {
        //Given an instance of the ImageGetter and a list of links to the images
        //When a new ImageListAsset is created
        //Then the images are downloaded as sprites, and the isReady returns true.

        //Variables in the sprite are irrelevant, we're just creating a new sprite that's effectively an image.
        Sprite image1 = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        Sprite image2 = Sprite.Create(Texture2D.grayTexture, Rect.zero, Vector2.up);
        Sprite image3 = Sprite.Create(Texture2D.redTexture, Rect.zero, Vector2.left);

        string uri1 = "http://imageLink1.com/get";
        string uri2 = "http://imageLink2.com/get";
        string uri3 = "http://imageLink3.com/get";

        var imageGetterMock = new Mock<IImageGetter>();
        
        imageGetterMock.Setup(x => x.GetImage(uri1, false, It.IsAny<Action<Sprite>>()))
            .Callback<string, bool, Action<Sprite>>((myUri, myCache, myAction) =>
            {
                myAction.Invoke(image1);
            }).Verifiable();
        
        imageGetterMock.Setup(x => x.GetImage(uri2, false, It.IsAny<Action<Sprite>>()))
            .Callback<string, bool, Action<Sprite>>((myUri, myCache, myAction) =>
            {
                myAction.Invoke(image2);
            }).Verifiable();
        
        imageGetterMock.Setup(x => x.GetImage(uri3, false, It.IsAny<Action<Sprite>>()))
            .Callback<string, bool, Action<Sprite>>((myUri, myCache, myAction) =>
            {
                myAction.Invoke(image3);
            }).Verifiable();
        
        bool hasSucceeded = false;
        Action<bool> ready = (success) => { hasSucceeded = success;};
        var sut = new ImageListAsset(imageGetterMock.Object, new List<string>(){uri1, uri2, uri3}, ready);
        
        imageGetterMock.Verify(x => x.GetImage(uri1, false, It.IsAny<Action<Sprite>>()));
        imageGetterMock.Verify(x => x.GetImage(uri2, false, It.IsAny<Action<Sprite>>()));
        imageGetterMock.Verify(x => x.GetImage(uri3, false, It.IsAny<Action<Sprite>>()));

        var images = sut.GetImages();

        Assert.AreEqual(image1, images[0]);
        Assert.AreEqual(image2, images[1]);
        Assert.AreEqual(image3, images[2]);

        Assert.IsTrue(hasSucceeded);
    }

    [Test]
    public void TestDownloadImage_SomethingFails_isReadyReturns_False()
    {
        //Given an instance of the ImageGetter and a failing link to an image.
        //When a new ImageListAsset is created.
        //Then the image isn't downloaded, and the isReady returns false.
        
        string uri = "http://failingImageLink.com/get";
        
        var imageGetterMock = new Mock<IImageGetter>();
        imageGetterMock
            .Setup(x => x.GetImage(uri, false, It.IsAny<Action<Sprite>>()))
            .Throws<ArgumentException>();
        
        bool hasSucceeded = true;
        Action<bool> ready = (success) => { hasSucceeded = success;};
        
        var sut = new ImageListAsset(imageGetterMock.Object, new List<string>() {uri}, ready);
        
        Assert.IsFalse(hasSucceeded);
    }

    [Test]
    public void TestGetAssetType()
    {
        Action<bool> ready = (success) => { };

        var sut = new ImageListAsset(null, new List<string>() {"uri" }, ready);
        var assetType = sut.GetAssetType();
        
        Assert.AreEqual(AssetType.ImageList, assetType);
    }
}
