using System;
using System.Collections;
using System.Collections.Generic;
using DigitalRubyShared;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products.resources.Map;
using UnityEngine;

public class TestMapCamController
{
    private MapCamController.Dependencies CreateDependencies(
        Mock<IMapPlayerMoverInstantiator> mapPlayerMoverInstantiatorMock = null,
        Mock<IMapCameraCenterPoint> mapCameraCenterPointMock = null,
        Mock<IGameObjectDestroyer> godMock = null,
        Mock<IMapCamMovementHelper> movementHelperMock = null)
    {
        mapCameraCenterPointMock ??= new Mock<IMapCameraCenterPoint>();
        godMock ??= new Mock<IGameObjectDestroyer>();
        movementHelperMock ??= new Mock<IMapCamMovementHelper>();
        
        if(mapPlayerMoverInstantiatorMock == null) {
            var mapPlayerMock = new Mock<IMapPlayerMover>();

            mapPlayerMoverInstantiatorMock = new Mock<IMapPlayerMoverInstantiator>();
            mapPlayerMoverInstantiatorMock
                .Setup(x => x.CreateMap2DPlayerMover(MapCamNavOrientation.XY))
                .Returns(mapPlayerMock.Object);
        }
        return new MapCamController.Dependencies()
        {
            MapCameraCenterPoint = mapCameraCenterPointMock.Object,
            Physics = new GameObject().AddComponent<Rigidbody>(),
            Camera = new GameObject().AddComponent<Camera>(),
            GOD = godMock.Object,
            MapCamMovementHelper = movementHelperMock.Object,
            MapPlayerMoverInstantiator = mapPlayerMoverInstantiatorMock.Object
        };
    }

    private MapCamController.Config CreateConfig(
        Mock<IScaleGestureRecognizer> scaleRecognizerMock = null, 
        Mock<IPanGestureRecognizer> panGestureRecognizerMock = null,
        float maxZoom = 16f,
        float minZoom = 5f,
        float startZoom = 8f)
    {
        scaleRecognizerMock ??= new Mock<IScaleGestureRecognizer>();
        panGestureRecognizerMock ??= new Mock<IPanGestureRecognizer>();
        return new MapCamController.Config()
        {
            PinchGesture = scaleRecognizerMock.Object,
            PanGestureRecognizer = panGestureRecognizerMock.Object,
            Riddlehouse2DMapCameraResource = new MapCameraResource() {
                ZoomSpeed = 3f,
                StartZoom = startZoom,
                MaxZoom = maxZoom,
                MinZoom = minZoom,
                CameraMoveSpeed = 7f,
                PlayerMoveSpeed = 3f
            }
        };
    }

    [Test]
    public void TestSetDependencies()
    {
        //Arrange
        var sut = new GameObject().AddComponent<MapCamController>();
        var dependencies = CreateDependencies();
        //Act
        sut.SetDependencies(dependencies);
        //Assert
        Assert.AreEqual(dependencies, sut._dependencies);
    }

    [Test]
    public void TestConfigure_Map2D_CreatesMapPlayerMover()
    {
        //Given a new initialized MapCamController, and a config which includes a panGestureRecognizer
        //When Configure is called
        //Then a mapPlayerMover is created and configured, the camera is zoomed, and the gesture events are subscribed.
        
        //Arrange
        var mapPlayerMock = new Mock<IMapPlayerMover>();
        mapPlayerMock.Setup(x => x.Configure(It.IsAny<MapPlayerMover.Config>())).Verifiable();

        var mapPlayerInstantiatorMock = new Mock<IMapPlayerMoverInstantiator>();
        mapPlayerInstantiatorMock
            .Setup(x => x.CreateMap2DPlayerMover(MapCamNavOrientation.XY))
            .Returns(mapPlayerMock.Object)
            .Verifiable();
        
        var scaleGestureRecognizerMock = new Mock<IScaleGestureRecognizer>();
        scaleGestureRecognizerMock.Setup(x => 
                x.SubscribeToStateUpdated(It.IsAny<GestureRecognizerStateUpdatedDelegate>()))
            .Verifiable();

        var panGestureRecognizerMock = new Mock<IPanGestureRecognizer>();
        
        var config = CreateConfig(scaleGestureRecognizerMock,panGestureRecognizerMock);
        
        var sut = new GameObject().AddComponent<MapCamController>();
        var dependencies = CreateDependencies(mapPlayerInstantiatorMock);
        sut.SetDependencies(dependencies);

        //Act
        sut.Configure(config);
        
        //Assert
        Assert.AreEqual(config.Riddlehouse2DMapCameraResource.StartZoom, dependencies.Camera.orthographicSize);
        mapPlayerMock.Verify(x => x.Configure(It.IsAny<MapPlayerMover.Config>()));
        scaleGestureRecognizerMock.Verify(x => 
                x.SubscribeToStateUpdated(It.IsAny<GestureRecognizerStateUpdatedDelegate>()));
        mapPlayerInstantiatorMock.Verify(x => x.CreateMap2DPlayerMover(MapCamNavOrientation.XY));
    }
    
    [Test]
    public void TestConfigure_MapBox_CreatesMapBoxPlayerMover()
    {
        //Given a new initialized MapCamController, and a config which does not include a panGestureRecognizer
        //When Configure is called
        //Then a MapBoxPlayerMover is created and configured, the camera is zoomed, and the gesture events are subscribed.
        
        //Arrange
        var mapPlayerMock = new Mock<IMapPlayerMover>();
        mapPlayerMock.Setup(x => x.Configure(It.IsAny<MapPlayerMover.Config>())).Verifiable();

        var mapPlayerInstantiatorMock = new Mock<IMapPlayerMoverInstantiator>();
        mapPlayerInstantiatorMock
            .Setup(x => x.CreateMapBoxPlayerMover())
            .Returns(mapPlayerMock.Object)
            .Verifiable();
        
        var scaleGestureRecognizerMock = new Mock<IScaleGestureRecognizer>();
        scaleGestureRecognizerMock.Setup(x => 
                x.SubscribeToStateUpdated(It.IsAny<GestureRecognizerStateUpdatedDelegate>()))
            .Verifiable();

        var config = new MapCamController.Config()
        {
            PinchGesture = scaleGestureRecognizerMock.Object,
            PanGestureRecognizer = null,
            Riddlehouse2DMapCameraResource = new MapCameraResource()
            {
                ZoomSpeed = 3f,
                StartZoom = 5f,
                MaxZoom = 15f,
                MinZoom = 5f,
                CameraMoveSpeed = 7f,
                PlayerMoveSpeed = 3f
            }
        };
        
        var sut = new GameObject().AddComponent<MapCamController>();
        var dependencies = CreateDependencies(mapPlayerInstantiatorMock);
        sut.SetDependencies(dependencies);

        //Act
        sut.Configure(config);
        
        //Assert
        Assert.AreEqual(config.Riddlehouse2DMapCameraResource.StartZoom, dependencies.Camera.orthographicSize);
        mapPlayerMock.Verify(x => x.Configure(It.IsAny<MapPlayerMover.Config>()));
        scaleGestureRecognizerMock.Verify(x => 
            x.SubscribeToStateUpdated(It.IsAny<GestureRecognizerStateUpdatedDelegate>()));
        mapPlayerInstantiatorMock.Verify(x => x.CreateMapBoxPlayerMover());
    }

    [Test]
    public void TestDetachPinchGesture()
    {
        //Given a configured MapCamController
        //When DetachPinchGesture is called
        //Then the gesture is unsubscribed.
        
        //Arrange
        var mapPlayerMock = new Mock<IMapPlayerMover>();
        mapPlayerMock.Setup(x => x.Configure(It.IsAny<MapPlayerMover.Config>())).Verifiable();

        var mapPlayerInstantiatorMock = new Mock<IMapPlayerMoverInstantiator>();
        mapPlayerInstantiatorMock
            .Setup(x => x.CreateMap2DPlayerMover(MapCamNavOrientation.XY))
            .Returns(mapPlayerMock.Object);

        var scaleGestureRecognizerMock = new Mock<IScaleGestureRecognizer>();

        scaleGestureRecognizerMock.Setup(x => 
                x.UnsubscribeToStateUpdated(It.IsAny<GestureRecognizerStateUpdatedDelegate>()))
            .Verifiable();
        
        var panGestureRecognizerMock = new Mock<IPanGestureRecognizer>();
        
        var config = CreateConfig(scaleGestureRecognizerMock,panGestureRecognizerMock);
        
        var sut = new GameObject().AddComponent<MapCamController>();
        var dependencies = CreateDependencies(mapPlayerInstantiatorMock);
        sut.SetDependencies(dependencies);
        sut.Configure(config);

        //Act
        sut.DetachPinchGesture();
        
        //Assert
        scaleGestureRecognizerMock.Verify(x => 
            x.UnsubscribeToStateUpdated(It.IsAny<GestureRecognizerStateUpdatedDelegate>()));
    }
    
    [Test]
    public void TestOnDisable()
    {
        //Given a configured MapCamController
        //When Object is disabled
        //Then the gesture is unsubscribed.
        
        //Arrange
        var mapPlayerMock = new Mock<IMapPlayerMover>();
        mapPlayerMock.Setup(x => x.Configure(It.IsAny<MapPlayerMover.Config>())).Verifiable();
        
        var mapPlayerInstantiatorMock = new Mock<IMapPlayerMoverInstantiator>();
        mapPlayerInstantiatorMock
            .Setup(x => x.CreateMap2DPlayerMover(MapCamNavOrientation.XY))
            .Returns(mapPlayerMock.Object);

        var scaleGestureRecognizerMock = new Mock<IScaleGestureRecognizer>();

        scaleGestureRecognizerMock.Setup(x => 
                x.UnsubscribeToStateUpdated(It.IsAny<GestureRecognizerStateUpdatedDelegate>()))
            .Verifiable();
        
        var panGestureRecognizerMock = new Mock<IPanGestureRecognizer>();
        
        var config = CreateConfig(scaleGestureRecognizerMock,panGestureRecognizerMock);
        
        var sut = new GameObject().AddComponent<MapCamController>();
        var dependencies = CreateDependencies(mapPlayerInstantiatorMock);
        sut.SetDependencies(dependencies);
        sut.Configure(config);

        //Act
        sut.OnDisable();
        
        //Assert
        scaleGestureRecognizerMock.Verify(x => 
            x.UnsubscribeToStateUpdated(It.IsAny<GestureRecognizerStateUpdatedDelegate>()));
    }
    
    [Test]
    public void TestOnEnable()
    {
        //Given a configured MapCamController
        //When Object is enabled
        //Then the gesture is subscribed.
        
        //Arrange
        var mapPlayerMock = new Mock<IMapPlayerMover>();
        mapPlayerMock.Setup(x => x.Configure(It.IsAny<MapPlayerMover.Config>())).Verifiable();

        var mapPlayerInstantiatorMock = new Mock<IMapPlayerMoverInstantiator>();
        mapPlayerInstantiatorMock
            .Setup(x => x.CreateMap2DPlayerMover(MapCamNavOrientation.XY))
            .Returns(mapPlayerMock.Object);
        
        var scaleGestureRecognizerMock = new Mock<IScaleGestureRecognizer>();

        scaleGestureRecognizerMock.Setup(x => 
                x.SubscribeToStateUpdated(It.IsAny<GestureRecognizerStateUpdatedDelegate>()))
            .Verifiable();
        
        var panGestureRecognizerMock = new Mock<IPanGestureRecognizer>();
        
        var config = CreateConfig(scaleGestureRecognizerMock,panGestureRecognizerMock);
        
        var sut = new GameObject().AddComponent<MapCamController>();
        var dependencies = CreateDependencies(mapPlayerInstantiatorMock);
        sut.SetDependencies(dependencies);
        sut.Configure(config);

        //Act
        sut.OnEnable();
        
        //Assert
        scaleGestureRecognizerMock.Verify(x => 
            x.SubscribeToStateUpdated(It.IsAny<GestureRecognizerStateUpdatedDelegate>()));
    }
    
    [TestCase(2f, 16f, 5f, 8f, 10f)] //this will zoomIn and stay between min and max values.
    [TestCase(-2f, 16f, 5f, 8f, 6f)] //this will zoomOut and stay between min and max values.
    [TestCase(-30f, 16f, 5f, 8f, 5f)] //this will zoomOut and stop at the minValue
    [TestCase(30f, 16f, 5f, 8f, 16f)] //this will zoomIn and stop at the maxValue
    [Test]
    public void TestZoom_StaysWithinBoundaries(float zoomValue, float maxZoom, float minZoom, float startZoom, float expected)
    {
        //Given a configured MapCamController
        //When Zoom is called with a zoomfactor
        //Then the camera is zoomed in or out based on the zoomfactor; zoom is capped by a min and max value.

        //Arrange
        var config = CreateConfig(null, null, maxZoom, minZoom, startZoom);
        
        var sut = new GameObject().AddComponent<MapCamController>();
        var dependencies = CreateDependencies();
        sut.SetDependencies(dependencies);

        sut.Configure(config);

        //Act
        sut.Zoom(zoomValue);
        
        //Assert
        Assert.AreEqual(expected, dependencies.Camera.orthographicSize);
        
    }

    [Test]
    public void TestConfigurePosition()
    {
        //Given a new MapCamController
        //When SetPosition is called with a vector position
        //Then the object is moved there.
        
        //Arrange
        var config = CreateConfig(null, new Mock<IPanGestureRecognizer>());
        
        var sut = new GameObject().AddComponent<MapCamController>();
        var dependencies = CreateDependencies();
        sut.SetDependencies(dependencies);

        sut.Configure(config);

        var newPosition = new Vector3(5f, 5f, 0f);
        var expectedPosition = new Vector3(5f, 5f, -10f);
        //Act
        sut.SetPosition(newPosition);

        //Assert
        Assert.AreEqual(expectedPosition, sut.transform.position);
    }


    [Test]
    public void TestUpdate_Calls_CameraMover()
    {
        //Given a configured MapCamController.
        //When Update is called.
        //Then the mover is called to generate movement.
        
        //Arrange
        var mapCameraMover = new Mock<IMapCamMovementHelper>();

        var config = CreateConfig();
        mapCameraMover.Setup(x => x.MovePlayer(config.Riddlehouse2DMapCameraResource, It.IsAny<IMapPlayerMover>())).Verifiable();
        
        var dependencies = CreateDependencies(null, null, null, mapCameraMover);

        var sut = new GameObject().AddComponent<MapCamController>();
        sut.SetDependencies(dependencies);

        sut.Configure(config);
        
        //Act
        sut.Update();
        
        //Assert
        mapCameraMover.Verify(x => x.MovePlayer(config.Riddlehouse2DMapCameraResource, It.IsAny<IMapPlayerMover>()));
    }

    [Test]
    public void TestDestroySelf_CleansUpSelf_And_MapPlayer()
    {
        //Given a new MapCamController.
        //When DestroySelf is called.
        //Then MapPlayer and the Camera is destroyed.
        
        //Arrange
        var mapCameraCenterPointMock = new Mock<IMapCameraCenterPoint>();
        mapCameraCenterPointMock.Setup(x => x.DestroySelf()).Verifiable();
        var mapPlayerMock = new Mock<IMapPlayerMover>();
        mapPlayerMock.Setup(x => x.DestroySelf()).Verifiable();
        
        var mapPlayerInstantiatorMock = new Mock<IMapPlayerMoverInstantiator>();
        mapPlayerInstantiatorMock
            .Setup(x => x.CreateMap2DPlayerMover(MapCamNavOrientation.XY))
            .Returns(mapPlayerMock.Object);

        var config = CreateConfig();

        var godMock = new Mock<IGameObjectDestroyer>();
        godMock.Setup(x => x.Destroy()).Verifiable();
        
        var sut = new GameObject().AddComponent<MapCamController>();
        var dependencies = CreateDependencies(mapPlayerInstantiatorMock, mapCameraCenterPointMock, godMock);
        sut.SetDependencies(dependencies);

        sut.Configure(config);

        //Act
        sut.DestroySelf();
        
        //Assert
        mapCameraCenterPointMock.Verify(x => x.DestroySelf());
        mapPlayerMock.Verify(x => x.DestroySelf());
        godMock.Verify(x => x.Destroy());
    }

    [Test]
    public void TestGetCamera_ReturnsCamera_InUse()
    {
        //Given an initialized MapCamController
        //When GetCamera is called
        //Then the camera used by the controller is returned
        
        //Arrange
        var sut = new GameObject().AddComponent<MapCamController>();
        var dependencies = CreateDependencies();
        sut.SetDependencies(dependencies);
        //Act
        var camera = sut.GetCamera();
        //Assert
        Assert.AreEqual(dependencies.Camera, camera);
    }
}