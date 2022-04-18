using System;
using System.Collections.Generic;
using Helpers;
using Hunt;
using RHPackages.Core.Scripts.StepControllers;
using riddlehouse_libraries.products.Assets.Map2DAssets;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.resources.Map;
using riddlehouse_libraries.products.Steps;
using riddlehouse_libraries.products.Stops;
using StepControllers;
using StopControllers;
using UnityEngine;
using Zenject;

public interface IMapBoxStopController : IStopController
{
    public void Configure(MapBoxStopController.Config config);
}

public class MapBoxStopController : MonoBehaviour, IMapBoxStopController
{
    public static IMapBoxStopController Factory(MapBoxStopController prefab, Transform parent)
    {
        MapBoxStopController behaviour = Instantiate(prefab, parent);
        behaviour.Initialize();
        return behaviour;
    }
    private List<IStepController> _stepControllers;
    
    public class Dependencies
    {
        public IMapCanvasControllerInstantiator MapCanvasControllerInstantiater { get; set; }
        public IStepControllerInstantiator StepControllerInstantiator { get; set; }

        public IMapboxMapInstantiator MapboxMapInstantiator;
        public IGameObjectDestroyer GOD;
        public IStopModelConverter StopModelConverter { get; set; }
        public IStopControllerContainer StopControllerContainer { get; set; }
    }
    
    public class Config
    {
        public IMapBoxStop Stop { get; set; }
        public Action<bool> EndStop { get; set; }
        public IStopControllerInstantiator StopControllerInstantiator { get; set; }
        public MapCanvasController.Config MapCanvasControllerConfig { get; set; }
    }

    public void Initialize()
    {
        var dependencies = new Dependencies()
        {
            MapCanvasControllerInstantiater = new MapCanvasControllerInstantiator(_addressableAssetLoader),
            MapboxMapInstantiator = _mapboxMapInstantiator,
            StepControllerInstantiator = new StepControllerInstantiator(_addressableAssetLoader),
            GOD = gameObject.AddComponent<GameObjectDestroyer>(),
            StopModelConverter = new StopModelConverter(),
            StopControllerContainer = new StopControllerContainer()
        };
        SetDependencies(dependencies);
    }

    [Inject] private IMapboxMapInstantiator _mapboxMapInstantiator;
    [Inject] private IAddressableAssetLoader _addressableAssetLoader;
    public Dependencies _dependencies { get; private set; }

    public void SetDependencies(Dependencies dependencies)
    {
        _stepControllers = new List<IStepController>();
        _dependencies = dependencies;
    }

    private IMapBoxMap _mapBoxMap;
    private Config _config;
    public void Configure(Config config)
    {
        _config = config;
        _mapBoxMap = _dependencies.MapboxMapInstantiator.CreateOrCollect(this.transform);
        _mapBoxMap.Configure(new MapBoxMap.Config()
        {
            Resource = _config.Stop.MapResource.MapCameraResource,
            MapCoordinate = _config.Stop.CenterPoint
        });
        
        if (_config.Stop != null)
        {
            if (_config.Stop.Stops != null)
                CreateStopPOIs(_config.Stop.PoiData, _config.Stop.Stops, _config.Stop.MapResource);
            if (_config.Stop.Steps != null)
                CreateStepPOIs(_config.Stop.PoiData, _config.Stop.Steps, _config.Stop.MapResource);
        }
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
                Debug.Log("Creating STOP: "+poiData.Id);
                _mapBoxMap.CreatePositionAndConfigurePoi(poiData, poiResource, StartStop);
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
                Debug.Log("Creating STEP: "+poiData.Id);
                _mapBoxMap.CreatePositionAndConfigurePoi(poiData, poiResource, StartStep);
            }
        }
    }
    
    public void EndStop(bool completed)
    {
        _config?.EndStop.Invoke(completed);
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

            var mapCanvasController =
                await _dependencies.MapCanvasControllerInstantiater.CreateOrCollectInstance(_config.Stop.MapResource
                    .MapCanvasResource.MapCanvasPrefab);
            await controller.StartStep(stepToStart, mapCanvasController, StepEnded);
        }
        Debug.Log("Clicked to start step with ID: "+id);
    }

    private void StepEnded()
    {
        UpdateStepAndStopPOIStates();
    }

    public void StartStop(string id)
    {
        var stopController = _dependencies.StopControllerContainer.Get(id);
        if (stopController == null)
        {
             var stops = _config.Stop.Stops;
             var stop = stops.Find(x => x.Id == id);
             if (stop != null)
             {
                 switch (stop.Type)
                 {
                     case StopType.MapStop2D:
                         _dependencies.StopControllerContainer.Add(CreateMap2D(stop));
                         break;
                     case StopType.MapBox:
                         _dependencies.StopControllerContainer.Add(CreateMapBox(stop));
                         break;
                     default:
                         throw new ArgumentException("no such type found.");
                 }
             }
             CloseStop();
             return;
        }
        stopController.OpenStop();
        CloseStop();
    }
    
    private IStopController CreateMap2D(IStop stop) //Todo: only tested CreateMap2D - Move the instantiation code to a helper, and use it in HuntController as well. 
    {
        var map2DStopController = _config.StopControllerInstantiator.CreateMap2D(null);
        var map2D = _dependencies.StopModelConverter.ConvertMap2DModel(stop);
        map2DStopController.ConfigureAndOpenStop(new Map2DStopController.Config()
        {
            Stop = map2D,
            EndStop = (completed) => OpenStop(),
            StopControllerInstantiator = _config.StopControllerInstantiator,
            MapCanvasControllerConfig = new MapCanvasController.Config(),
        });
        return map2DStopController;
    }
    private IStopController CreateMapBox(IStop stop) //Todo: only tested CreateMap2D - Move the instantiation code to a helper, and use it in HuntController as well. 
    {
        var mapBoxStopController = _config.StopControllerInstantiator.CreateMapBox(null);
        var mapBoxModel = _dependencies.StopModelConverter.ConvertMapBoxModel(stop);
        mapBoxStopController.Configure(new MapBoxStopController.Config()
        {
            Stop = mapBoxModel,
            MapCanvasControllerConfig = new MapCanvasController.Config(),
            StopControllerInstantiator = _config.StopControllerInstantiator,
            EndStop = (completed) => OpenStop(),
        });
        mapBoxStopController.OpenStop();
        return mapBoxStopController;
    }

    public async void OpenStop()
    {
        this.gameObject.SetActive(true);
        _config.MapCanvasControllerConfig.CanvasConfig = new CanvasController.Config()
        {
            ViewCamera = _mapBoxMap.GetCamera()
        };
        _config.MapCanvasControllerConfig.BackButtonAction = () => { EndStop(_config.Stop.HasBeenCompleted()); };
        var mapCanvasController =
            await _dependencies.MapCanvasControllerInstantiater.CreateOrCollectInstance(_config.Stop.MapResource
                .MapCanvasResource.MapCanvasPrefab);
        mapCanvasController.ConfigureAndDisplay(_config.MapCanvasControllerConfig);
        UpdateStepAndStopPOIStates();
    }

    private void UpdateStepAndStopPOIStates()
    {
        if (_config.Stop?.Stops != null)
            foreach (var stop in _config.Stop.Stops)
            {
                _mapBoxMap.UpdatePOIState(stop.Id, _config.Stop.GetState(stop.Id));
            }

        if (_config.Stop?.Steps != null)
            foreach (var step in _config.Stop.Steps)
            {
                _mapBoxMap.UpdatePOIState(step.Id, _config.Stop.GetStepState(step.Id));
            }
    }

    public void CloseStop()
    {
        this.gameObject.SetActive(false);
    }

    public async void DestroySelf()
    {
        _mapBoxMap?.DestroySelf();
        _dependencies.GOD.Destroy();
        var mapCanvasController =
            await _dependencies.MapCanvasControllerInstantiater.CreateOrCollectInstance(_config.Stop.MapResource
                .MapCanvasResource.MapCanvasPrefab);
        mapCanvasController.Hide();
        _dependencies.StopControllerContainer.RemoveAndDestroyAllControllers();
    }

    public string GetId()
    {
        return _config.Stop.Id;
    }
}
