using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products.resources.Map;
using UnityEngine;

public class TestMapCamMovementHelper
{
    [TestCase(MapBorderDirection.North)]
    [TestCase(MapBorderDirection.South)]
    [TestCase(MapBorderDirection.East)]
    [TestCase(MapBorderDirection.West)]
    [Test]
    public void TestUpdate_adds_velocityXY_to_camera_basedOn_PlayerPosition(MapBorderDirection direction)
    {
        //Given a configured MapCamController, on the XY axes
        //When Update is called
        //then velocity is added in the direction of the player on the map.
        //-- this will happen at a fixed rate in the update loop, speeding up based on the distance to the player on the x,y plane.
        
        //In this case, the camera is on x=5, Y=4, Z=0
        //and the player is on x=5, y=6, Z=0;
        //the relative vector is therefore (0,2f,0) with the camera being below the player, and expected to move upwards.
        
        //Arrange
        var mapCameraCenterPointMock = new Mock<IMapCameraCenterPoint>();
        mapCameraCenterPointMock.Setup(x => x.GetPosition()).Returns(new Vector3(5f,4f,0f)).Verifiable();
        var mapPlayerMock = new Mock<IMapPlayerMover>();
        mapPlayerMock.Setup(x => x.GetPosition()).Returns(GetPlayerPositionXY(direction)).Verifiable();

        var physics = new GameObject().AddComponent<Rigidbody>();

        var riddlehouse2DMapCameraResource = new MapCameraResource()
        {
            ZoomSpeed = 3f,
            StartZoom = 8f,
            MaxZoom = 5f,
            MinZoom = 16f,
            CameraMoveSpeed = 7f,
            PlayerMoveSpeed = 3f
        };

        var sut = new MapCamMovementHelper(
            MapCamNavOrientation.XY,  
            physics, 
            mapCameraCenterPointMock.Object);
        
        //Act
        sut.MovePlayer(riddlehouse2DMapCameraResource, mapPlayerMock.Object);
        
        //Assert
        var correctDirection = IsCameraHeadedInTheRightDirectionXY(direction, physics.velocity);
        Assert.IsTrue(correctDirection);
    }
    
    [TestCase(MapBorderDirection.North)]
    [TestCase(MapBorderDirection.South)]
    [TestCase(MapBorderDirection.East)]
    [TestCase(MapBorderDirection.West)]
    [Test]
    public void TestUpdate_adds_velocityXZ_to_camera_basedOn_PlayerPosition(MapBorderDirection direction)
    {
        //Given a configured MapCamController, on the XZ axes
        //When Update is called
        //then velocity is added in the direction of the player on the map.
        //-- this will happen at a fixed rate in the update loop, speeding up based on the distance to the player on the x,z plane.
        
        //In this case, the camera is on x=5, Y=0, Z=4,
        //and the player is on x=5, Y=0, Z=6;
        //the relative vector is therefore (0,0,2f) with the camera being below the player, and expected to move upwards.
        
        //Arrange
        var mapCameraCenterPointMock = new Mock<IMapCameraCenterPoint>();
        mapCameraCenterPointMock.Setup(x => x.GetPosition()).Returns(new Vector3(5f,0f,4f)).Verifiable();
        var mapPlayerMock = new Mock<IMapPlayerMover>();
        mapPlayerMock.Setup(x => x.GetPosition()).Returns(GetPlayerPositionXZ(direction)).Verifiable();

        var physics = new GameObject().AddComponent<Rigidbody>();

        var riddlehouse2DMapCameraResource = new MapCameraResource()
        {
            ZoomSpeed = 3f,
            StartZoom = 8f,
            MaxZoom = 5f,
            MinZoom = 16f,
            CameraMoveSpeed = 7f,
            PlayerMoveSpeed = 3f
        };

        var sut = new MapCamMovementHelper(
            MapCamNavOrientation.XZ,  
            physics, 
            mapCameraCenterPointMock.Object);
        
        //Act
        sut.MovePlayer(riddlehouse2DMapCameraResource,mapPlayerMock.Object);
        
        //Assert
        var correctDirection = IsCameraHeadedInTheRightDirectionXZ(direction, physics.velocity);
        Assert.IsTrue(correctDirection);
    }
    
    private Vector3 GetPlayerPositionXY(MapBorderDirection direction)
    {
        switch (direction)
        {
            case MapBorderDirection.North:
                return new Vector3(5f, 6f, 0f);
            case MapBorderDirection.South:
                return new Vector3(5f, 2f, 0f);
            case MapBorderDirection.East:
                return new Vector3(6f,4f,0f);
            case MapBorderDirection.West:
                return new Vector3(3f,4f,0f);
            default:
                throw new ArgumentException("no such case");
        }
    }
    private Vector3 GetPlayerPositionXZ(MapBorderDirection direction)
    {
        switch (direction)
        {
            case MapBorderDirection.North:
                return new Vector3(5f, 0f, 6f);
            case MapBorderDirection.South:
                return new Vector3(5f, 0f, 2f);
            case MapBorderDirection.East:
                return new Vector3(6f,0f,4f);
            case MapBorderDirection.West:
                return new Vector3(3f,0f,4f);
            default:
                throw new ArgumentException("no such case");
        }
    }
    
    private bool IsCameraHeadedInTheRightDirectionXY(MapBorderDirection direction, Vector3 velocity)
    {
        switch (direction)
        {
            case MapBorderDirection.North:
                return velocity.x == 0 && velocity.y > 0;
            case MapBorderDirection.South:
                return velocity.x == 0 && velocity.y < 0;
            case MapBorderDirection.East:
                return velocity.x > 0 && velocity.y == 0;
            case MapBorderDirection.West:
                return velocity.x < 0 && velocity.y == 0;
            default:
                throw new ArgumentException("no such case");
        }
    }
    
    private bool IsCameraHeadedInTheRightDirectionXZ(MapBorderDirection direction, Vector3 velocity)
    {
        switch (direction)
        {
            case MapBorderDirection.North:
                return velocity.x == 0 && velocity.z > 0;
            case MapBorderDirection.South:
                return velocity.x == 0 && velocity.z < 0;
            case MapBorderDirection.East:
                return velocity.x > 0 && velocity.z == 0;
            case MapBorderDirection.West:
                return velocity.x < 0 && velocity.z == 0;
            default:
                throw new ArgumentException("no such case");
        }
    }

}
