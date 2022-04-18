using System.Collections;
using System.Collections.Generic;
using riddlehouse_libraries.products.Assets.Map2DAssets;
using riddlehouse_libraries.products.resources.Map;
using UnityEngine;

public interface IMap2DHelper
{
    public Vector2 DesiredPositionCalculator(Map2DPosition realworldPosition, Map2DRectAsset bounds);
}

public class Map2DHelper : IMap2DHelper
{
    public Vector2 DesiredPositionCalculator(Map2DPosition realworldPosition, Map2DRectAsset bounds)
    {
        //to calcuate the relative position, we need to adjust our values to the bounds of the min/max scope.
        //min(x-3, y-5), max(x-5,y-7)

        //diff(2,2)
        var maxDiffX = bounds.EndPoint.XPosition - bounds.OriginPoint.XPosition;

        //realworldPosition(3,7) //actual position within the bounds of the map rect.
        //InternalPosition(0,2) //actual position from 0 to max inside the bounds.
        var internalPosX = realworldPosition.XPosition - bounds.OriginPoint.XPosition;

        //relativePosition(0,1) //calcualted relative distance based on the maximum distance from 0 to max.
        float relativePosX = (float)(internalPosX / maxDiffX) * 2f;

        //desiredPosition(-1,1) //converted to a -1,1 space to accommodate centered pivot point.
        float desiredPosX = relativePosX - 1f;

        //repeat above process but for the Y axis.
        var maxDiffY = bounds.EndPoint.YPosition - bounds.OriginPoint.YPosition;
        var internalPosY = realworldPosition.YPosition - bounds.OriginPoint.YPosition;
        float relativePosY = (float)(internalPosY / maxDiffY) * 2f;
        float desiredPosY = relativePosY - 1f;

        return new Vector2(desiredPosX, desiredPosY);
    }
}