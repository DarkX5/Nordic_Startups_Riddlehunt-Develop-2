using System;
using Map;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using riddlehouse_libraries.products.Assets.Adressables;
using riddlehouse_libraries.products.Assets.Map2DAssets;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.resources.Map;
using riddlehouse_libraries.products.Stops;
using UnityEngine;
using Zenject;

public interface IMapBoxMap
{
    public void Configure(MapBoxMap.Config config);
    public void CreatePositionAndConfigurePoi(
        POI2DListAsset.Poi2DAsset poiData, POI2DResource poiResource, Action<string> clickedAction);

    public void UpdatePOIState(string poiId, PoiStates state);
    public Camera GetCamera();

    public void DestroySelf();
}
public class MapBoxMap : MonoBehaviour, IMapBoxMap
{
    public static MapBoxMap Factory(MapBoxMap prefab, Transform parent)
    {
        var behaviour = Instantiate(prefab, parent);
        behaviour.Initialize();
        return behaviour;
    }
    
    public class Dependencies
    {
        public AbstractMap Map { get; set; }
        public LocationProviderFactory LocationProviderFactory { get; set; }
        public IMapCamController MapCamController { get; set; }
        public IFingersGestureInstantiator FingersGestureInstantiator { get; set; }
        public IMapBoxPOIPlacement MapBoxPoiPlacement { get; set; }
        public IGameObjectDestroyer GOD { get; set; }
    }

    public class Config
    {
        public MapCameraResource Resource { get; set; }
        public Map2DPosition MapCoordinate { get; set; }
    }
    
    public void Initialize()
    {
        var parent = this.transform.parent;
        var mapCamControllerInstantiator = new MapCamControllerInstantiator(mapboxCamPrefab, parent);
        SetDependencies(new Dependencies()
        {
            Map = map,
            MapCamController = mapCamControllerInstantiator.CreateXZ(),
            LocationProviderFactory = location,
            FingersGestureInstantiator = new FingersGestureInstantiator(),
            MapBoxPoiPlacement = new MapBoxPOIPlacement(new POIInstantiator(_addressableAssetLoader), map, parent),
            GOD = gameObject.AddComponent<GameObjectDestroyer>()
        });
        _dependencies.LocationProviderFactory.mapManager = _dependencies.Map;
    }

    [SerializeField] private AbstractMap map;
    [SerializeField] private LocationProviderFactory location;
    [SerializeField] private MapCamController mapboxCamPrefab;
    [Inject] private IAddressableAssetLoader _addressableAssetLoader;
    public Dependencies _dependencies { get; private set; }
    public void SetDependencies(Dependencies dependencies)
    {
        _dependencies = dependencies;
    }


    public void Configure(Config config)
    {
        var panGesture = _dependencies.FingersGestureInstantiator.CreatePanSingleTouchGesture();
        var pinchGesture = _dependencies.FingersGestureInstantiator.CreatePinchGesture();
        
         if (config.Resource.CameraBehaviour == MapCameraBehaviour.Floating)
         {
         }
         else if(config.Resource.CameraBehaviour == MapCameraBehaviour.BoundToLocation)
         {
             panGesture = null;
         }
         else
         {
             throw new ArgumentException("no such behaviour mapped");
        }

        _dependencies.MapCamController.Configure(new MapCamController.Config()
        {
            PinchGesture = pinchGesture,
            PanGestureRecognizer = panGesture,
            Riddlehouse2DMapCameraResource = config.Resource
        });
        var position = Conversions.StringToLatLon(config.MapCoordinate.StringCoord());
        _dependencies.Map.Initialize(position, 15);
        _dependencies.Map.Options.extentOptions.extentType = MapExtentType.CameraBounds;
        _dependencies.Map.SetExtentOptions(new CameraBoundsTileProviderOptions()
        {
            camera = GetCamera()
        });
    }

    public void CreatePositionAndConfigurePoi(
        POI2DListAsset.Poi2DAsset poiData, 
        POI2DResource poiResource, 
        Action<string> clickedAction)
    {
        _dependencies.MapBoxPoiPlacement.CreateAndConfigure(
            new POIController.Config()
            {
                ClickedAction = clickedAction,
                Resource = poiResource,
                PoiAsset = poiData,
            },
            1f);
    }

    public void UpdatePOIState(string poiId, PoiStates state)
    {
        _dependencies.MapBoxPoiPlacement.UpdatePOIState(poiId, state);
    }

    public Camera GetCamera()
    {
        return _dependencies.MapCamController.GetCamera();
    }

    public void DestroySelf()
    {
        _dependencies.MapCamController.DestroySelf();
        _dependencies.MapBoxPoiPlacement.DestroyAllPois();
        _dependencies.GOD.Destroy();
    }

    public void Update()
    {
        _dependencies?.MapBoxPoiPlacement?.UpdatePoiPositions();
    }
}
