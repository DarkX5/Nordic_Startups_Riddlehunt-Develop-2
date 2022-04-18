using System;
using System.Collections.Generic;
using Helpers;
using Hunt;
using RHPackages.Core.Scripts.StepControllers;
using riddlehouse_libraries.products.Assets.Map2DAssets;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.resources.Map;
using riddlehouse_libraries.products.Steps;
using riddlehouse_libraries.products.Steps.Resources;
using riddlehouse_libraries.products.Stops;
using StepControllers;
using UnityEngine;
using Zenject;

public interface IMap2DInstantiator
{
    public IMap2D Create();
}

public class Map2DInstantiator: IMap2DInstantiator
{
    private readonly Map2D _prefab;
    private readonly Transform _parent;

    public Map2DInstantiator(Map2D prefab, Transform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }

    public IMap2D Create()
    {
        return  Map2D.Factory(_prefab, _parent);
    }
}

public interface IMap2DStopController : IStopController
{
    public void ConfigureAndOpenStop(Map2DStopController.Config config);
}

[RequireComponent(typeof(GameObjectDestroyer))]
public class Map2DStopController : MonoBehaviour, IMap2DStopController
{
    public static IMap2DStopController Factory(Map2DStopController prefab, Transform parent)
    {
        Map2DStopController behaviour = Instantiate(prefab, parent);
        behaviour.Initialize();
        return behaviour;
    }
    
    private List<IStopController> _stopControllers;
    private List<IStepController> _stepControllers;
    public class Dependencies
    {
        public IMapCanvasControllerInstantiator MapCanvasControllerInstantiator { get; set; }
        public IMap2DInstantiator Map2DInstantiator { get; set; }
        public IGameObjectDestroyer GOD { get; set; }
        public IMap2D CurrentMap { get; set; }
        public IStepControllerInstantiator StepControllerInstantiator { get; set; }
        public IStopModelConverter StopModelConverter { get; set; }
    }

    public class Config
    {
        public IMap2DStop Stop { get; set; }
        public Action<bool> EndStop { get; set; }
        public IStopControllerInstantiator StopControllerInstantiator { get; set; }
        public MapCanvasController.Config MapCanvasControllerConfig { get; set; }
    }
    public void Initialize()
    {
        SetDependencies(new Dependencies()
        {
            GOD = GetComponent<GameObjectDestroyer>(),
            MapCanvasControllerInstantiator = new MapCanvasControllerInstantiator(_addressableAssetLoader),
            Map2DInstantiator = new Map2DInstantiator(map2DPrefab, this.transform),
            StepControllerInstantiator = new StepControllerInstantiator(_addressableAssetLoader),
            StopModelConverter = new StopModelConverter(),
        });
    }
    [SerializeField] private POIController poiPrefab; 
    [Inject] private IAddressableAssetLoader _addressableAssetLoader;

    [SerializeField] private Map2D map2DPrefab;
    public Dependencies _dependencies { get; private set; }
    public void SetDependencies(Dependencies dependencies)
    {
        _dependencies = dependencies;
        _stopControllers = new List<IStopController>();
        _stepControllers = new List<IStepController>();
    }

    private Sprite _backButtonIcon;
    private Config _config;

    public void ConfigureAndOpenStop(Config config)
    {
        _config = config;
        
        var stop = _config.Stop;
        gameObject.name = stop.Title;

        _dependencies.CurrentMap = _dependencies.Map2DInstantiator.Create();
        _dependencies.CurrentMap.Configure(new Map2D.Config()
        {
            Resource = _config.Stop,
            InitializationComplete = MapCompletedInitialization
        });
    }

    private void MapCompletedInitialization(bool success)
    {

        if (_config.Stop != null)
        {
            if (_config.Stop.Stops != null)
                CreateStopPOIs(_config.Stop.PoiData, _config.Stop.Stops, _config.Stop.MapResource);
            if (_config.Stop.Steps != null)
                CreateStepPOIs(_config.Stop.PoiData, _config.Stop.Steps, _config.Stop.MapResource);
        }
        
        OpenStop();
    }

    private void CreateStopPOIs(POI2DListAsset pois, List<IStop> stops, MapResource mapResource)
    {
        foreach (var childStop in stops)
        {
            var poiData = pois.PoiData.Find(x => x.Id == childStop.Id);
            if (poiData != null)
            {
                var poiResource = mapResource.PoiResources
                    .Find(x => x.ResourceId == poiData.ResourceId);
                
                _dependencies.CurrentMap.CreatePOIForMap(new POIController.Config()
                {
                    ClickedAction = (id) => { StartStop(id); },
                    Resource = poiResource,
                    PoiAsset = poiData,
                });
            }
        }
    }
    
    private void CreateStepPOIs(POI2DListAsset pois, List<IStep> steps, MapResource mapResource)
    {
        foreach (var childStep in steps)
        {
            var poiData = pois.PoiData.Find(x => x.Id == childStep.Id);
            if (poiData != null)
            {
                var poiResource = mapResource.PoiResources
                    .Find(x => x.ResourceId == poiData.ResourceId);

                _dependencies.CurrentMap.CreatePOIForMap(new POIController.Config()
                {
                    ClickedAction = (id) => { StartStep(id); },
                    Resource = poiResource,
                    PoiAsset = poiData,
                });
            }
        }
    }

    public void StartStop(string id)
    {
        var stopController = _stopControllers.Find(x => x.GetId() == id);
        if (stopController == null)
        {
            var stops = _config.Stop.Stops;
            var stop = stops.Find(x => x.Id == id);
            if (stop != null)
            {
                var nextLayerStopController = _config.StopControllerInstantiator.CreateMap2D(null);

                _config.MapCanvasControllerConfig.BackButtonAction = () =>
                {
                    EndStop(_config.Stop.HasBeenCompleted());
                };
                
                nextLayerStopController.ConfigureAndOpenStop(new Config()
                {
                    Stop = _dependencies.StopModelConverter.ConvertMap2DModel(stop),
                    EndStop = (completed) =>
                    {
                        OpenStop();
                    },
                    StopControllerInstantiator = _config.StopControllerInstantiator,
                    MapCanvasControllerConfig = _config.MapCanvasControllerConfig
                });
                _stopControllers.Add(nextLayerStopController);
            }
            CloseStop();
            return;
        }
        stopController.OpenStop();
        CloseStop();
    }

    public async void StartStep(string id)
    {
       var stepToStart = _config.Stop.Steps.Find(x => x.Id == id);
       if (stepToStart != null)
       {
           var controller = _stepControllers.Find(x => x.GetModelId() == id);
           if (controller == null)
           {
               if (stepToStart.Type == StepType.DisplayRiddleWithMultipleChoice)
                   controller = _dependencies.StepControllerInstantiator
                       .CreateDisplayRiddleWithMultipleChoiceStepController();
               else
                   controller = _dependencies.StepControllerInstantiator
                       .CreateDisplayRiddleAndSubmitAnswerStepController();
               _stepControllers.Add(controller);
           }
           var mapCanvasController = await _dependencies.MapCanvasControllerInstantiator
               .CreateOrCollectInstance(_config.Stop.MapResource.MapCanvasResource.MapCanvasPrefab);
           await controller.StartStep(stepToStart, mapCanvasController, StepEnded);
       }
       Debug.Log("Clicked to start step with ID: "+id);
    }

    private void StepEnded()
    {
        UpdateStepAndStopPOIStates();
    }
    public void EndStop(bool completed)
    {
        _config?.EndStop.Invoke(completed);
        CloseStop();
    }

    public async void OpenStop()
    {
        this.gameObject.SetActive(true);
        _config.MapCanvasControllerConfig.CanvasConfig = new CanvasController.Config()
        {
            ViewCamera = _dependencies.CurrentMap.GetCamera()
        };
        _config.MapCanvasControllerConfig.BackButtonAction = () => { EndStop(_config.Stop.HasBeenCompleted()); };
        var mapCanvasController = await _dependencies.MapCanvasControllerInstantiator
            .CreateOrCollectInstance(_config.Stop.MapResource.MapCanvasResource.MapCanvasPrefab);
        mapCanvasController.ConfigureAndDisplay(_config.MapCanvasControllerConfig);
        UpdateStepAndStopPOIStates();
    }
    
    private void UpdateStepAndStopPOIStates()
    {
        foreach (var stop in _config.Stop.Stops)
        { 
            _dependencies.CurrentMap.UpdatePOI(stop.Id, _config.Stop.GetState(stop.Id));
        }

        foreach (var step in _config.Stop.Steps)
        {
            _dependencies.CurrentMap.UpdatePOI(step.Id, _config.Stop.GetStepState(step.Id));
        }
    }
    
    public void CloseStop()
    {
        this.gameObject.SetActive(false);
    }

    public async void DestroySelf()
    {
        _dependencies.CurrentMap?.DestroySelf();
        foreach (var controller in _stopControllers)
        {
            controller.DestroySelf();
        }
        
        if (_dependencies.MapCanvasControllerInstantiator.HasInstance())
        {
            var mapCanvasController =
                await _dependencies.MapCanvasControllerInstantiator.CreateOrCollectInstance(_config.Stop.MapResource
                    .MapCanvasResource.MapCanvasPrefab);
            mapCanvasController?.Hide();
        }
        _dependencies.GOD.Destroy();
    }

    public string GetId()
    {
        return _config.Stop.Id;
    }
}
