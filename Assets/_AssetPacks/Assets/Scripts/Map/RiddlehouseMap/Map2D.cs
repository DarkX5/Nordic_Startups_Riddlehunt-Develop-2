using System;
using DigitalRubyShared;
using Map;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.Stops;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public interface IMapBorderInstantiator
{
    public IMapBorder Create(MapBorderDirection direction,
        MapBorder.Dependencies dependencies);
}

public class MapBorderInstantiator : IMapBorderInstantiator
{
    private MapBorder prefab;
    private Transform parent;

    public MapBorderInstantiator(MapBorder prefab, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;
    }

    public IMapBorder Create( MapBorderDirection direction, MapBorder.Dependencies dependencies)
    {
        return MapBorder.Factory(prefab, parent, direction, dependencies);
    }
}

public interface IScaleGestureRecognizer
{
    public void SubscribeToStateUpdated(GestureRecognizerStateUpdatedDelegate gestureEvent);
    public void UnsubscribeToStateUpdated(GestureRecognizerStateUpdatedDelegate gestureEvent);
    public float GetScaleMultiplier();
}
public interface IPanGestureRecognizer
{
    public void SubscribeToStateUpdated(GestureRecognizerStateUpdatedDelegate gestureEvent);
    public void UnsubscribeToStateUpdated(GestureRecognizerStateUpdatedDelegate gestureEvent);

}
public class RiddlehouseScaleGestureRecognizer: IScaleGestureRecognizer
{
    private ScaleGestureRecognizer gestureRecognizer { get; set; }
    public RiddlehouseScaleGestureRecognizer(ScaleGestureRecognizer gestureRecognizer)
    {
        this.gestureRecognizer = gestureRecognizer;
    }
    public void SubscribeToStateUpdated(GestureRecognizerStateUpdatedDelegate gestureEvent)
    {
        gestureRecognizer.StateUpdated += gestureEvent;
    }
    public void UnsubscribeToStateUpdated(GestureRecognizerStateUpdatedDelegate gestureEvent)
    {
        gestureRecognizer.StateUpdated -= gestureEvent;
    }

    public float GetScaleMultiplier()
    {
        return gestureRecognizer.ScaleMultiplier;
    }
}
public class RiddlehousePanGestureRecognizer: IPanGestureRecognizer
{
    private PanGestureRecognizer gestureRecognizer { get; set; }

    public RiddlehousePanGestureRecognizer(PanGestureRecognizer gestureRecognizer)
    {
        this.gestureRecognizer = gestureRecognizer;
    }

    public void SubscribeToStateUpdated(GestureRecognizerStateUpdatedDelegate gestureEvent)
    {
        gestureRecognizer.StateUpdated += gestureEvent;
    }
    public void UnsubscribeToStateUpdated(GestureRecognizerStateUpdatedDelegate gestureEvent)
    {
        gestureRecognizer.StateUpdated -= gestureEvent;
    }
}

public interface IFingersGestureInstantiator
{
    public IScaleGestureRecognizer CreatePinchGesture();
    public IPanGestureRecognizer CreatePanSingleTouchGesture();
}
public class FingersGestureInstantiator : IFingersGestureInstantiator
{
    public IScaleGestureRecognizer CreatePinchGesture()
    {
        ScaleGestureRecognizer pinchGesture = null;
        foreach (var gesture in FingersScript.Instance.Gestures)
        {
            if (gesture.GetType() == typeof(ScaleGestureRecognizer))
            {
                pinchGesture = (ScaleGestureRecognizer)gesture;
                break;
            }
        }

        if (pinchGesture == null)
        {
            pinchGesture = new ScaleGestureRecognizer();
            pinchGesture.ThresholdUnits = 0.05f;
            FingersScript.Instance.AddGesture(pinchGesture);
        }

        return new RiddlehouseScaleGestureRecognizer(pinchGesture);
    }
    public IPanGestureRecognizer CreatePanSingleTouchGesture()
    {
        PanGestureRecognizer panSingleTouchGesture = null;
        foreach (var gesture in FingersScript.Instance.Gestures)
        {
            if (gesture.GetType() == typeof(PanGestureRecognizer))
            {
                panSingleTouchGesture = (PanGestureRecognizer)gesture;
                break;
            }
        }
        if (panSingleTouchGesture == null)
        {
            panSingleTouchGesture = new PanGestureRecognizer();
            panSingleTouchGesture.MinimumNumberOfTouchesToTrack = panSingleTouchGesture.MaximumNumberOfTouchesToTrack = 1;
            panSingleTouchGesture.ThresholdUnits = 0.05f;
            FingersScript.Instance.AddGesture(panSingleTouchGesture);
        }

        return new RiddlehousePanGestureRecognizer(panSingleTouchGesture);
    }
}

public interface IMap2D
{
    public void Configure(Map2D.Config config);
    public void DestroySelf();
    public Camera GetCamera();
    public void CreatePOIForMap(POIController.Config config);
    public void UpdatePOI(string poiId, PoiStates state);
}
[RequireComponent(typeof(GameObjectDestroyer))]
[RequireComponent(typeof(SpriteRenderer))]
public class Map2D : MonoBehaviour, IMap2D
{
    public static IMap2D Factory(Map2D prefab, Transform parent)
    {
        Map2D behaviour = Instantiate(prefab, parent);
        behaviour.Initialize();
        return behaviour;
    }

    public class Dependencies
    {
        public SpriteRenderer SR { get; set; }
        public IMapCamControllerInstantiator MapCamControllerInstantiator { get; set; }
        public IMapBorderInstantiator MapBorderInstantiator { get; set; }
        public IFingersGestureInstantiator FingersGestureInstantiator { get; set; }
        public IGameObjectDestroyer GOD { get; set; }
        public IMap2DPOIPlacement Map2DpoiPlacement { get; set; }
        public IMap2DHelper MapHelper { get; set; }
        public Transform POILayer { get; set; }
    }

    public class Config
    {
        public Action<bool> InitializationComplete { get; set; }
        public IMap2DStop Resource { get; set; }
    }
    
    [SerializeField] private MapCamController camControllerPrefab;
    [FormerlySerializedAs("prefab")] [SerializeField] private MapBorder mapBorderPrefab;
    [FormerlySerializedAs("sr")] [SerializeField] private SpriteRenderer mapSR;
    [Inject] private IAddressableAssetLoader _addressableAssetLoader;
    private IScaleGestureRecognizer _pinchGesture;
    private IPanGestureRecognizer _panSingleTouchGesture;
    public void Initialize()
    {
        var thisTransform = transform;
        var dependencies = new Dependencies()
        {
            SR = mapSR,
            MapCamControllerInstantiator = new MapCamControllerInstantiator(camControllerPrefab, thisTransform.parent),
            MapBorderInstantiator = new MapBorderInstantiator(mapBorderPrefab, thisTransform),
            FingersGestureInstantiator = new FingersGestureInstantiator(),
            GOD = GetComponent<GameObjectDestroyer>(),
            MapHelper = new Map2DHelper(),
            POILayer = poiLayer,
            Map2DpoiPlacement = new Map2DPOIPlacement(new POIInstantiator(_addressableAssetLoader))
        };
        SetDependencies(dependencies);
    }
    
    public Dependencies _dependencies { get; private set; }

    public void SetDependencies(Dependencies dependencies)
    {
        _dependencies = dependencies;
    }
    
    [SerializeField] private Transform poiLayer;
    private IMapCamController _mapCamController;
    private Config _config;
    public void Configure(Config config)
    {
        _config = config;
        _pinchGesture = _dependencies.FingersGestureInstantiator.CreatePinchGesture();
        _panSingleTouchGesture = _dependencies.FingersGestureInstantiator.CreatePanSingleTouchGesture();

        _mapCamController = _dependencies.MapCamControllerInstantiator.CreateXY();
        _mapCamController.Configure(new MapCamController.Config()
        {
            PinchGesture = _pinchGesture,
            PanGestureRecognizer = _panSingleTouchGesture,
            Riddlehouse2DMapCameraResource = _config.Resource.MapResource.MapCameraResource
        });
        
        var mapTexture = new Texture2D(2, 2);
        mapTexture.LoadImage(_config.Resource.MapImageAsset.Image);
        _dependencies.SR.sprite =  Sprite.Create(mapTexture, new Rect(0, 0, mapTexture.width, mapTexture.height),
            new Vector2(0.5f, 0.5f));
        
        var mapBorderDependencies = new MapBorder.Dependencies()
        {
            Map = _dependencies.SR
        };
        var position = this.transform.position;

        _mapCamController.SetPosition(position);
        
        _dependencies.MapBorderInstantiator.Create(MapBorderDirection.North, mapBorderDependencies);
        _dependencies.MapBorderInstantiator.Create(MapBorderDirection.South, mapBorderDependencies);
        _dependencies.MapBorderInstantiator.Create(MapBorderDirection.East, mapBorderDependencies);
        _dependencies.MapBorderInstantiator.Create(MapBorderDirection.West, mapBorderDependencies);
        
        _config.InitializationComplete.Invoke(true);
    }
    
    public void DestroySelf()
    {
        _mapCamController?.DestroySelf();
        _dependencies.GOD.Destroy();
    }

    public Camera GetCamera()
    {
        return _mapCamController.GetCamera();
    }

    public async void CreatePOIForMap(POIController.Config config)
    {
        var poi = await _dependencies.Map2DpoiPlacement.CreateAndConfigure(config, 1f);
        SetPOIOnMap(poi);
    }

    public void UpdatePOI(string poiId, PoiStates state)
    {
        _dependencies.Map2DpoiPlacement.UpdatePOIState(poiId, state);
    }
    
    private void SetPOIOnMap(IPOIController poi)
    {
        var topEdgePosition = (_dependencies.SR.sprite.bounds.size.y * 10f)- (poi.GetYBounds()/2f)-1f;
        var rightEdgePosition = (_dependencies.SR.sprite.bounds.size.x * 10f)-(poi.GetXBounds()/2f)-1f;
       
        var desiredRelativePos = 
            _dependencies.MapHelper.DesiredPositionCalculator(
                poi.GetRealWorldPosition(), 
                _config.Resource.MapGpsRect
            );
        
        topEdgePosition = topEdgePosition * desiredRelativePos.y;
        rightEdgePosition = rightEdgePosition * desiredRelativePos.x;

        poi.SetLocalPosition(new Vector3(rightEdgePosition, topEdgePosition, this.transform.localPosition.z), _dependencies.POILayer);
    }
}