using System;
using System.Collections;
using System.Collections.Generic;
using DigitalRubyShared;
using Moq;
using riddlehouse_libraries.products.resources.Map;
using UnityEngine;
using UnityEngine.Serialization;

public interface IMapPlayerMoverInstantiator
{
    public IMapPlayerMover CreateMapBoxPlayerMover();
    public IMapPlayerMover CreateMap2DPlayerMover(MapCamNavOrientation orientation);
}

public class MapPlayerMoverInstantiator : IMapPlayerMoverInstantiator
{
    public readonly MapBoxPlayerMover _mapBoxPlayerMoverPrefab;
    public readonly MapPlayerMover _mapPlayerPrefab;
    public Transform _parent;
    public MapPlayerMoverInstantiator(MapBoxPlayerMover mapBoxPlayerMoverPrefab, MapPlayerMover mapPlayerPrefab, Transform parent)
    {
        _mapBoxPlayerMoverPrefab = mapBoxPlayerMoverPrefab;
        _mapPlayerPrefab = mapPlayerPrefab;
        _parent = parent;
    }
    public IMapPlayerMover CreateMapBoxPlayerMover()
    {
        return MapBoxPlayerMover.Factory(_mapBoxPlayerMoverPrefab, _parent, 1);
    }

    public IMapPlayerMover CreateMap2DPlayerMover(MapCamNavOrientation orientation)
    {
       return MapPlayerMover.Factory(_mapPlayerPrefab, _parent, 1, orientation);
    }
}

public interface IMapCamController
{
    public void SetPosition(Vector3 origin);
    public void Configure(MapCamController.Config config);
    public void DetachPinchGesture();
    public void DestroySelf();
    public Camera GetCamera();
}

[RequireComponent((typeof(GameObjectDestroyer)))]
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Rigidbody))]
public class MapCamController : MonoBehaviour, IMapCamController
{
    public static IMapCamController Factory(MapCamController prefab, Transform parent, MapCamNavOrientation orientation)
    {
        MapCamController behaviour = Instantiate(prefab, parent);
        behaviour.Initialize(orientation);
        return behaviour;
    }
    public class Dependencies
    {
        public IGameObjectDestroyer GOD { get; set; }
        public IMapPlayerMoverInstantiator MapPlayerMoverInstantiator { get; set; }
        public IMapPlayerMover MapPlayer;
        public IMapCameraCenterPoint MapCameraCenterPoint;
        public IMapCamMovementHelper MapCamMovementHelper;
        public Rigidbody Physics { get; set; }
        public Camera Camera { get; set; }
    }

    public class Config
    {
        public IScaleGestureRecognizer PinchGesture;
        public IPanGestureRecognizer PanGestureRecognizer;
        public MapCameraResource Riddlehouse2DMapCameraResource { get; set; }
    }
    
    public void Initialize(MapCamNavOrientation orientation)
    {
        var mapCamera = GetComponent<Camera>();
        var physics = GetComponent<Rigidbody>();

        // IMapPlayerMover mapPlayerMover;
        // if(orientation == MapCamNavOrientation.XY)
        //     mapPlayerMover = MapPlayerMover.Factory(mapPlayerPrefab, this.transform.parent, 1);
        // else if (orientation == MapCamNavOrientation.XZ)
        //     mapPlayerMover = MapBoxPlayerMover.Factory(mapBoxPlayerMoverPrefab, this.transform.parent, 1);
        // else throw new ArgumentException("no mapPlayer defined");
        
        var mapCameraCenterPoint = MapCameraCenterPoint.Factory(mapCameraCenterPointPrefab, this.transform);
        _orientation = orientation;
        var dependencies = new Dependencies()
        {
            MapPlayer = null,
            MapCameraCenterPoint = mapCameraCenterPoint,
            Physics = physics,
            Camera = mapCamera,
            GOD = GetComponent<GameObjectDestroyer>(),
            MapCamMovementHelper = new MapCamMovementHelper(_orientation, physics, mapCameraCenterPoint),
            MapPlayerMoverInstantiator = new MapPlayerMoverInstantiator(mapBoxPlayerMoverPrefab, mapPlayerPrefab, this.transform.parent)
        };
        SetDependencies(dependencies);
    }

    private MapCamNavOrientation _orientation;
    [SerializeField] private MapCameraCenterPoint mapCameraCenterPointPrefab;
    [SerializeField] private MapPlayerMover mapPlayerPrefab;
    [SerializeField] private MapBoxPlayerMover mapBoxPlayerMoverPrefab;
    public Dependencies _dependencies { get; private set; }
    public void SetDependencies(Dependencies dependencies)
    {
        _dependencies = dependencies;
    }

    private Vector2 PlayerStartingPoint { get; set; } = Vector2.zero;
    private bool _subscribed = false;
    private Config _config;
    public void Configure(Config config)
    {
        _config = config;

        if (_dependencies.MapPlayer != null)
        {
            _dependencies.MapPlayer.DestroySelf();
        }
        
        if (_config.PanGestureRecognizer == null)
        {
            _dependencies.MapPlayer = _dependencies.MapPlayerMoverInstantiator.CreateMapBoxPlayerMover();
        }
        else
        {
            _dependencies.MapPlayer = _dependencies.MapPlayerMoverInstantiator.CreateMap2DPlayerMover(_orientation);
        }

        PlayerStartingPoint = _dependencies.MapPlayer.GetPosition();

        _dependencies.MapPlayer.Configure(new MapPlayerMover.Config()
        {
            PanGestureRecognizer = _config.PanGestureRecognizer,
            MoveSpeed = _config.Riddlehouse2DMapCameraResource.PlayerMoveSpeed
        });
        _dependencies.Camera.orthographicSize = _config.Riddlehouse2DMapCameraResource.StartZoom;

        AttachPinchGesture();
    }
    private void AttachPinchGesture()
    {
        if (_config != null && !_subscribed)
        {
            _config.PinchGesture.SubscribeToStateUpdated(PinchGestureCallback);
            _subscribed = true;
        }
    }
    public void DetachPinchGesture()
    {
        if (_config != null && _subscribed)
        {
            _config.PinchGesture.UnsubscribeToStateUpdated(PinchGestureCallback);
            _subscribed = false;
        }
    }

    public void OnDisable()
    {
        DetachPinchGesture();
    }
    public void OnEnable()
    {
        AttachPinchGesture();
    }

    private void PinchGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Executing)
        {
            //Scalemultiplier is 0-2 based, subtracting 1, makes it go from -1 to 1
            var zoomFactor =
                (_config.PinchGesture.GetScaleMultiplier() - 1f) *
                -1f; //invert the value to get the right pinch direction.
            Zoom(zoomFactor * _config.Riddlehouse2DMapCameraResource.ZoomSpeed);
        }
    }
    
    // Update is called once per frame
    public void Update()
    {
        if(_dependencies.MapCamMovementHelper != null)
            _dependencies.MapCamMovementHelper.MovePlayer(_config.Riddlehouse2DMapCameraResource, _dependencies.MapPlayer);
    }
    
    public void Zoom(float zoomFactor)
    {
        if(zoomFactor > 0)
            ZoomIn(zoomFactor);
        else if (zoomFactor == 0)
            return;
        else 
            ZoomOut(zoomFactor);
    }
    private void ZoomIn(float zoomFactor)
    {
        float zoomed = _dependencies.Camera.orthographicSize + zoomFactor;
        if (zoomed < _config.Riddlehouse2DMapCameraResource.MaxZoom)
        {
            _dependencies.Camera.orthographicSize = zoomed;
        }
        else
        {
            _dependencies.Camera.orthographicSize = _config.Riddlehouse2DMapCameraResource.MaxZoom;
        }
    }

    private void ZoomOut(float zoomFactor)
    {
        float zoomed = _dependencies.Camera.orthographicSize + zoomFactor;
        if (zoomed > _config.Riddlehouse2DMapCameraResource.MinZoom)
        {
            _dependencies.Camera.orthographicSize = zoomed;
        }
        else
        {
            _dependencies.Camera.orthographicSize = _config.Riddlehouse2DMapCameraResource.MinZoom;
        }
    }
    
    public void SetPosition(Vector3 origin)
    {
        transform.position = new Vector3(origin.x, origin.y,
            origin.z - 10f);
    }
    public void DestroySelf()
    {
        _dependencies.MapPlayer.DestroySelf();
        _dependencies.MapCameraCenterPoint.DestroySelf();
        _dependencies.GOD.Destroy();
    }

    public Camera GetCamera()
    {
        return _dependencies.Camera;
    }
}
