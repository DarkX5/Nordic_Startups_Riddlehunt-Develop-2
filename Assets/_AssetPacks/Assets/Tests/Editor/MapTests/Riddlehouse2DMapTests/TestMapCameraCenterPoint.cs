using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestMapCameraCenterPoint
{
    [Test]
    public void TestSetToCenterAndScale()
    {
        //Given a new GameObject of size 2,2, localPosition 5,5.
        //When SetToCenterAndScale is called.
        //Then the size is reset to 1,1 and position 0,0.
        //Arrange
        var go = new GameObject();
        var sut = go.AddComponent<MapCameraCenterPoint>();

        go.transform.localPosition = Vector3.one * 5f;
        go.transform.localScale = Vector3.one * 2f;
        
        //Act
        sut.SetToCenterAndScale();
        //Assert
        Assert.AreEqual(go.transform.localPosition, Vector3.zero);
        Assert.AreEqual(go.transform.localScale, Vector3.one);
    }

    [Test]
    public void TestGetPosition()
    {
        //Arrange
        var go = new GameObject();
        var sut = go.AddComponent<MapCameraCenterPoint>();

        go.transform.localPosition = Vector3.one * 5f;
        
        //Act
        var pos = sut.GetPosition();
        
        //Assert
        Assert.AreEqual(Vector3.one * 5f, pos);
    }
}
