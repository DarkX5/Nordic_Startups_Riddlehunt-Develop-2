using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using NUnit.Framework;
using UnityEngine;

public class TestSpriteHelper : MonoBehaviour
{
    [Test]
    public void TestGetSpriteFromByteArray_ConvertsByteArrayIntoSprite_WithPixelsPrUnit()
    {
        //Given a new SpriteHelper and a ByteArray
        //When called GetSpriteFromByteArray with a byte array and a pixels pr unity
        //Then a sprite with those settings is returned

        var byteArray = new Byte[7] { 55, 66, 77, 88, 99, 44, 12 };
        var pixelsPrUnit = 5;
        var sut = new SpriteHelper();

        var sprite = sut.GetSpriteFromByteArray(byteArray, pixelsPrUnit);
        
        Assert.IsNotNull(sprite);
        Assert.AreEqual(pixelsPrUnit, sprite.pixelsPerUnit);        
        //can't test the actual bytevalues since unity adds alot of data on conversion.
    }
    
    [Test]
    public void TestGetSpriteFromByteArray_ConvertsByteArrayIntoSprite()
    {
        //Given a new SpriteHelper and a ByteArray
        //When called GetSpriteFromByteArray with a byte array
        //Then a sprite with those settings is returned

        var byteArray = new Byte[7] { 55, 66, 77, 88, 99, 44, 12 };
        var sut = new SpriteHelper();

        var sprite = sut.GetSpriteFromByteArray(byteArray);
        
        Assert.IsNotNull(sprite);
        //can't test the actual bytevalues since unity adds alot of data on conversion.
    }

    [Test]
    public void TestConvertByteArrayListToSpriteList_ConvertsToSpriteList()
    {
        //Given a new SpriteHelper and a list of ByteArrays
        //When called ConvertByteArrayListToSpriteList
        //Then a list of sprites with those settings is returned
        var byteArray = new Byte[7] { 55, 66, 77, 88, 99, 44, 12 };
        var imageList = new List<Byte[]> { byteArray, byteArray, byteArray };
        var sut = new SpriteHelper();

        var sprites = sut.ConvertByteArrayListToSpriteList(imageList);
        
        //can't test the actual bytevalues since unity adds alot of data on conversion.
        Assert.AreEqual(imageList.Count, sprites.Count);
    }
}
