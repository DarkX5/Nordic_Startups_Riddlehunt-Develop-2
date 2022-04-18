using System;
using System.Collections;
using System.Collections.Generic;
using DigitalRubyShared;
using riddlehouse_libraries.products.resources.Map;
using UnityEngine;

public interface IMapPlayerMovementHelper
{
    public MapCamNavOrientation NavOrientation {get;}
    public void MovePlayer(float moveSpeed, GestureRecognizer gesture);
}

public class MapPlayerMovementHelper: IMapPlayerMovementHelper
{
    public MapCamNavOrientation NavOrientation {get;}

    private Rigidbody _physics;
    public MapPlayerMovementHelper(
        MapCamNavOrientation navOrientation, 
        Rigidbody physics)
    {
        NavOrientation = navOrientation;
        _physics = physics;
    }

    public void MovePlayer(float moveSpeed, GestureRecognizer gesture)
    {
        if (_physics == null)
            return;
        
        switch (NavOrientation)
        {
            case MapCamNavOrientation.XY:
                MovePlayerXY(moveSpeed, gesture);
                break;
            case MapCamNavOrientation.XZ:
                MovePlayerXZ(moveSpeed, gesture);
                break;
            default:
                throw new ArgumentException("navigation not defined");
        }
    }
    private void MovePlayerXZ(float moveSpeed, GestureRecognizer gesture)
    {
        var velocity = _physics.velocity;
        velocity += Vector3.left * ((gesture.DeltaX * moveSpeed) * Time.deltaTime);
        velocity += Vector3.back * ((gesture.DeltaY * moveSpeed) * Time.deltaTime);
        _physics.velocity = velocity;
    }
    
    private void MovePlayerXY(float moveSpeed, GestureRecognizer gesture)
    {
        var velocity = _physics.velocity;
        velocity += Vector3.left * ((gesture.DeltaX * moveSpeed) * Time.deltaTime);
        velocity += Vector3.down * ((gesture.DeltaY * moveSpeed) * Time.deltaTime);
        _physics.velocity = velocity;
    }
}