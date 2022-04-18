using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestToggleImageColorComponent
{
    [Test]
    public void TestConfigure()
    {
        //Given an unconfigured ToggleImageColorComponent
        //When Configure is called
        //Then the ImageComponent is collected, and the image component is set to its "unselected" state.
        
        //Arrange
        var sut = new GameObject().AddComponent<ToggleImageColorComponent>();
        
        //Assert
        Assert.IsNull(sut.ImageComponent);
        
        //Act
        sut.Configure(Color.black, Color.blue);
        
        //Assert
        Assert.IsNotNull(sut.ImageComponent);
        Assert.AreEqual(Color.blue, sut.ImageComponent.color);
    }

    [Test]
    public void TestToggleColor()
    {        
        //Given a configured ToggleImageColorComponent
        //When ToggleColor is called
        //Then the color is swapped back and forth between black and blue.
        
        //Arrange
        var sut = new GameObject().AddComponent<ToggleImageColorComponent>();
        sut.Configure(Color.black, Color.blue);

        //Act
        sut.ToggleColor();
        
        //Assert 
        Assert.IsNotNull(sut.ImageComponent);
        Assert.AreEqual(Color.black, sut.ImageComponent.color);
        Assert.IsTrue(sut.IsSelected);
        
        //Act 2
        sut.ToggleColor();
        
        //Assert 2
        Assert.IsNotNull(sut.ImageComponent);
        Assert.AreEqual(Color.blue, sut.ImageComponent.color);
        Assert.IsFalse(sut.IsSelected);
    }

    [Test]
    public void TestSetStateSelected()
    {               
        //Given a configured ToggleImageColorComponent
        //When SetStateSelected is called
        //Then the color is set to the "selected" and the IsSelected attribute is true.
        
        //Arrange
        var sut = new GameObject().AddComponent<ToggleImageColorComponent>();
        sut.Configure(Color.black, Color.blue);

        //Act
        sut.SetStateSelected();
        //Assert
        Assert.IsNotNull(sut.ImageComponent);
        Assert.AreEqual(Color.black, sut.ImageComponent.color);
        Assert.IsTrue(sut.IsSelected);

    }

    [Test]
    public void TestSetStateUnselected()
    {
        //Given a configured ToggleImageColorComponent
        //When SetStateUnselected is called
        //Then the color is set to the "unselected" and the IsSelected attribute is false.
        
        //Arrange
        var sut = new GameObject().AddComponent<ToggleImageColorComponent>();
        sut.Configure(Color.black, Color.blue);
        //Act
        sut.SetStateUnselected();

        //Assert
        Assert.IsNotNull(sut.ImageComponent);
        Assert.AreEqual(Color.blue, sut.ImageComponent.color);
        Assert.IsFalse(sut.IsSelected);
    }
}
