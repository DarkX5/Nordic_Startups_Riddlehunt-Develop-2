using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestSetImageColorComponent 
{
    [Test]
    public void TestConfigure()
    {
        //Given a SetImageColorComponent
        //When Configure is called with a color
        //Then the image component on the object changes to that color.
        //Arrange
        var sut = new GameObject().AddComponent<SetImageColorComponent>();
        //Act
        sut.Configure(Color.black);
        //Assert
        Assert.IsNotNull(sut._imageComponent);
        Assert.AreEqual(Color.black, sut._imageComponent.color);
    }
}
