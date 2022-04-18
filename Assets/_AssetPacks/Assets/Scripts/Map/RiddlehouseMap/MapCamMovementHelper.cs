using System;
using System.Collections;
using System.Collections.Generic;
using riddlehouse_libraries.products.resources.Map;
using UnityEngine;

public enum MapCamNavOrientation {XY, XZ}
public interface IMapCamMovementHelper
{
    public MapCamNavOrientation NavOrientation {get;}
    public void MovePlayer(MapCameraResource resource, IMapPlayerMover playerMover);
}

public class MapCamMovementHelper: IMapCamMovementHelper
{
    public MapCamNavOrientation NavOrientation {get;}
    private Rigidbody _physics;
    private IMapCameraCenterPoint _centerPoint;
    public MapCamMovementHelper(
        MapCamNavOrientation navOrientation, 
        Rigidbody physics, 
        IMapCameraCenterPoint centerPoint)
    {
        NavOrientation = navOrientation;
        _physics = physics;
        _centerPoint = centerPoint;
    }

    public void MovePlayer(MapCameraResource resource, IMapPlayerMover playerMover)
    {
        switch (NavOrientation)
        {
            case MapCamNavOrientation.XY:
                MoveCameraXY(resource,playerMover);
                break;
            case MapCamNavOrientation.XZ:
                MoveCameraXZ(resource,playerMover);
                break;
            default:
                throw new ArgumentException("navigation not defined");
        }
    }
    private void MoveCameraXZ(MapCameraResource resource, IMapPlayerMover playerMover)
    {
        var detectorPosition = _centerPoint.GetPosition();
        var playerPosition = playerMover.GetPosition();
        Vector3 velocity = _physics.velocity;
        
        var horizontalDistance = Vector2.Distance(new Vector2(playerPosition.x, 0f), new Vector2(detectorPosition.x, 0f));
        var verticalDistance = Vector2.Distance(new Vector2(0f, playerPosition.z), new Vector2(0f, detectorPosition.z));
        
        if(playerPosition.x > detectorPosition.x)
            velocity += (Vector3.right*resource.CameraMoveSpeed*horizontalDistance)*Time.deltaTime;
        
        if(playerPosition.x < detectorPosition.x)
            velocity += (Vector3.left*resource.CameraMoveSpeed*horizontalDistance)*Time.deltaTime;
        
        if(playerPosition.z > detectorPosition.z)
            velocity += (Vector3.forward*resource.CameraMoveSpeed*verticalDistance)*Time.deltaTime;
        
        if(playerPosition.z < detectorPosition.z)
            velocity += (Vector3.back*resource.CameraMoveSpeed*verticalDistance)*Time.deltaTime;
        
        _physics.velocity = velocity;
    }
    
    private void MoveCameraXY(MapCameraResource resource, IMapPlayerMover playerMover)
    {
        var detectorPosition = _centerPoint.GetPosition();
        var playerPosition = playerMover.GetPosition();
        Vector3 velocity = _physics.velocity;
        
        var horizontalDistance = Vector2.Distance(new Vector2(playerPosition.x, 0f), new Vector2(detectorPosition.x, 0f));
        var verticalDistance = Vector2.Distance(new Vector2(0f, playerPosition.y), new Vector2(0f, detectorPosition.y));
        
        if(playerPosition.x > detectorPosition.x)
            velocity += (Vector3.right*resource.CameraMoveSpeed*horizontalDistance)*Time.deltaTime;
        
        if(playerPosition.x < detectorPosition.x)
            velocity += (Vector3.left*resource.CameraMoveSpeed*horizontalDistance)*Time.deltaTime;
        
        if(playerPosition.y > detectorPosition.y)
            velocity += (Vector3.up*resource.CameraMoveSpeed*verticalDistance)*Time.deltaTime;
        
        if(playerPosition.y < detectorPosition.y)
            velocity += (Vector3.down*resource.CameraMoveSpeed*verticalDistance)*Time.deltaTime;
        
        _physics.velocity = velocity;
    }
}