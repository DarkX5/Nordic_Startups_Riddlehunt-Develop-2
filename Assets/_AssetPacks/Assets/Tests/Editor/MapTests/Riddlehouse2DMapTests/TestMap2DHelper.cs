using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using riddlehouse_libraries.products.Assets.Map2DAssets;
using riddlehouse_libraries.products.resources.Map;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TestMap2DHelper
{
    [TestCase(55.662888251318023, 12.297588969073889, 1f)] //1,1
    [TestCase(55.56288825131802, 12.197588969073888, -1f)] //-1,-1
    [Test]
    public void DesiredPositionCalculator_Min_Max(double x, double y, float target)
    {
        //Given a new Map2DHelper
        //When DesiredPositionCalculator is called with a relativePosition and a boundary rect
        //Then a relative position is returned spanning between (-1,-1) and (1,1)

        //Arrange
        var sut = new Map2DHelper();

        var poiPosition = new Map2DPosition(x, y);

        var boundary = new Map2DRectAsset(
            new Map2DPosition(55.56288825131802,12.197588969073888),
            new Map2DPosition(55.66288825131802, 12.297588969073888)
            );
        
        //Act
       var position = sut.DesiredPositionCalculator(poiPosition, boundary);
       
       //Assert
       
       //Calculate values to compare against,using the same formula as the function - this way we can match expectations.
       var maxDiffX = boundary.EndPoint.XPosition - boundary.OriginPoint.XPosition;
       var internalPosX = poiPosition.XPosition - boundary.OriginPoint.XPosition;
       float relativePosX = (float)(internalPosX / maxDiffX) * 2f;
       float desiredPosX = relativePosX - 1f;
       
       var maxDiffY = boundary.EndPoint.YPosition - boundary.OriginPoint.YPosition;
       var internalPosY = poiPosition.YPosition - boundary.OriginPoint.YPosition;
       float relativePosY = (float)(internalPosY / maxDiffY) * 2f;
       float desiredPosY = relativePosY - 1f;
       
       Assert.AreEqual(new Vector2(desiredPosX, desiredPosY), position);
       Assert.AreEqual(new Vector2(desiredPosX, desiredPosY), new Vector2(target,target));
       Assert.IsTrue(desiredPosX>= -1f && desiredPosX <= 1f);
       Assert.IsTrue(desiredPosY>= -1f && desiredPosY <= 1f);
    }
    
    [Test]
    public void DesiredPositionCalculator_Between()
    {
        //Given a new Map2DHelper
        //When DesiredPositionCalculator is called with a relativePosition and a boundary rect
        //Then a relative position is returned spanning between (-1,-1) and (1,1)

        //Arrange
        var sut = new Map2DHelper();

        var poiPosition = new Map2DPosition(55.61288925131802, 12.2475889607388);

        var boundary = new Map2DRectAsset(new Map2DPosition(55.56288825131802, 12.197588969073888),
            new Map2DPosition(55.66288825131802, 12.297588969073888));

        //Act
       var position = sut.DesiredPositionCalculator(poiPosition, boundary);
       
       //Assert
       
       //Calculate values to compare against,using the same formula as the function - this way we can match expectations.
       var maxDiffX = boundary.EndPoint.XPosition - boundary.OriginPoint.XPosition;
       var internalPosX = poiPosition.XPosition - boundary.OriginPoint.XPosition;
       float relativePosX = (float)(internalPosX / maxDiffX) * 2f;
       float desiredPosX = relativePosX - 1f;
       
       var maxDiffY = boundary.EndPoint.YPosition - boundary.OriginPoint.YPosition;
       var internalPosY = poiPosition.YPosition - boundary.OriginPoint.YPosition;
       float relativePosY = (float)(internalPosY / maxDiffY) * 2f;
       float desiredPosY = relativePosY - 1f;
       
       Assert.AreEqual(new Vector2(desiredPosX, desiredPosY), position);

       Assert.IsTrue(desiredPosX>= -1f && desiredPosX <= 1f);
       Assert.IsTrue(desiredPosY>= -1f && desiredPosY <= 1f);
    }
}
