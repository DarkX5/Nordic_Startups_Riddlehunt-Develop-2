using System;
using System.Collections.Generic;
using Helpers;
using Hunt;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.StepControllers;
using riddlehouse_libraries.products.Assets.Adressables;
using riddlehouse_libraries.products.Assets.Map2DAssets;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.resources;
using riddlehouse_libraries.products.resources.Map;
using riddlehouse_libraries.products.Steps;
using riddlehouse_libraries.products.Stops;
using UnityEngine;

[TestFixture]
public class TestMap2DStopController
{
    private Map2DStopController.Config _config;
    private Camera _camera;
    private Mock<IMap2D> _map2DMock;
    private Mock<IMap2DInstantiator> _map2DInstantiatorMock;
    private Mock<IStopControllerInstantiator> _stopControllerInstantiator;
    private Mock<IMap2DStop> _map2DStop;
    private string _mapCanvasPath;
    private AddressableWithTag _mapCanvasAddressable;
    private MapResource _mapResource;
    [SetUp]
    public void Init()
    {
        _map2DStop = new Mock<IMap2DStop>();
        _map2DStop.Setup(x => x.Title).Returns("defaultTitle");
        _map2DStop.Setup(x => x.Id).Returns("defaultId");
        _map2DStop.Setup(x => x.Stops).Returns(new List<IStop>());
        _map2DStop.Setup(x => x.Steps).Returns(new List<IStep>());
        
        _map2DMock = new Mock<IMap2D>();
        _map2DMock.Setup(x => x.Configure(It.IsAny<Map2D.Config>())).Verifiable();
        
        _map2DInstantiatorMock = new Mock<IMap2DInstantiator>();
        _map2DInstantiatorMock.Setup(x => x.Create())
            .Returns(_map2DMock.Object)
            .Verifiable();

        var map2DStopControllerMock = new Mock<IMap2DStopController>();

        _stopControllerInstantiator = new Mock<IStopControllerInstantiator>();
        _stopControllerInstantiator.SetupSequence(x => x.CreateMap2D(null))
            .Returns(map2DStopControllerMock.Object);

        _mapCanvasPath = "Assets/MapMenu.prefab";
        _mapCanvasAddressable = new AddressableWithTag("GameCanvas", _mapCanvasPath);
        _mapResource = new MapResource()
        {
            MapCanvasResource = new MapCanvasResource()
            {
                MapCanvasPrefab = _mapCanvasAddressable
            }
        };
        _map2DStop.Setup(x => x.MapResource).Returns(_mapResource);
        
        _config = new Map2DStopController.Config()
        {
            Stop = _map2DStop.Object,
            StopControllerInstantiator = _stopControllerInstantiator.Object
        };
        
        _camera = new GameObject().AddComponent<Camera>();
    }

    [TearDown]
    public void TearDown()
    {
        _map2DStop = null;
        _map2DMock = null;
        _map2DInstantiatorMock = null;
        _config = null;
    }
    
    Map2DStopController.Dependencies CreateDependencies(
        Mock<IMap2DInstantiator> map2DInstantiatorMock = null,
        Mock<IMapCanvasControllerInstantiator> map2DCanvasControllerInstantiatorMock = null,
        Mock<IGameObjectDestroyer> gameObjectDestroyerMock = null,
        Mock<IStepControllerInstantiator> stepControllerInstantiator = null,
        Mock<IStopModelConverter> stopModelConverter = null)
    {
        map2DInstantiatorMock ??= new Mock<IMap2DInstantiator>();
        stepControllerInstantiator ??= new Mock<IStepControllerInstantiator>();
        if (map2DCanvasControllerInstantiatorMock == null)
        {
            map2DCanvasControllerInstantiatorMock = new Mock<IMapCanvasControllerInstantiator>();
            map2DCanvasControllerInstantiatorMock.Setup(x => x.CreateOrCollectInstance(It.IsAny<AddressableWithTag>()))
                .ReturnsAsync(new Mock<IMapCanvasController>().Object)
                .Verifiable();
        }
        
        if (stopModelConverter == null)
        {
            stopModelConverter = new Mock<IStopModelConverter>();
            stopModelConverter.Setup(x => 
                x.ConvertMap2DModel(It.IsAny<IStop>())).Returns(new Mock<IMap2DStop>().Object);
            stopModelConverter.Setup(x => 
                x.ConvertMapBoxModel(It.IsAny<IStop>())).Returns(new Mock<IMapBoxStop>().Object);
        }

        gameObjectDestroyerMock ??= new Mock<IGameObjectDestroyer>();
        
        return new Map2DStopController.Dependencies()
        { 
            MapCanvasControllerInstantiator = map2DCanvasControllerInstantiatorMock.Object,
            Map2DInstantiator = map2DInstantiatorMock.Object,
            GOD = gameObjectDestroyerMock.Object,
            StepControllerInstantiator = stepControllerInstantiator.Object,
            StopModelConverter = stopModelConverter.Object
        };
    }

    private Mock<IMap2D> CreateMap()
    {
        var map2DMock = new Mock<IMap2D>();
        map2DMock.Setup(x => x.Configure(It.IsAny<Map2D.Config>()))
            .Callback<Map2D.Config>((theConfig) =>
            {
                theConfig.InitializationComplete.Invoke(true);
            })
            .Verifiable();

        map2DMock.Setup(x => x.GetCamera()).Returns(_camera).Verifiable();
        return map2DMock;
    }

    private Mock<IMapCanvasController> CreateMap2DCanvasController(MapCanvasController.Config config)
    {
        var map2DCanvasControllerMock = new Mock<IMapCanvasController>();
        map2DCanvasControllerMock.Setup(x => x.ConfigureAndDisplay(config))
            .Verifiable();
        return map2DCanvasControllerMock;
    }

    private Mock<IMapCanvasControllerInstantiator> CreateMap2DCanvasControllerInstantiator(AddressableWithTag addressable, Mock<IMapCanvasController> mapCanvasController = null)
    {
        var map2DCanvasConfig = new MapCanvasController.Config()
        {
            CanvasConfig = new CanvasController.Config() {},
        };
        mapCanvasController ??= CreateMap2DCanvasController(map2DCanvasConfig);

        var map2DCanvasControllerInstantiatorMock = new Mock<IMapCanvasControllerInstantiator>();
        map2DCanvasControllerInstantiatorMock.Setup(x => x.CreateOrCollectInstance(addressable))
            .ReturnsAsync(mapCanvasController.Object)
            .Verifiable();

        return map2DCanvasControllerInstantiatorMock;
    }
        
   
    private Mock<IMap2DInstantiator> CreateMapInstantiator(Mock<IMap2D> mapMock = null)
    {
        mapMock ??= CreateMap();
        var map2DInstantiatorMock = new Mock<IMap2DInstantiator>();
        map2DInstantiatorMock.Setup(x => x.Create())
            .Returns(mapMock.Object)
            .Verifiable();
        return map2DInstantiatorMock;
    }
    
    [Test]
    public void TestSetDependencies()
    {
        //Arrange
        var sut = new GameObject().AddComponent<Map2DStopController>();
        
        var dependencies = CreateDependencies();
        
        //Act
        sut.SetDependencies(dependencies);
        //Assert
        Assert.AreEqual(dependencies, sut._dependencies);
    }
    
    [Test]
    public void TestConfigure_CreatesAndConfigures_Map()
    {
        //Given a new Map2DStopController
        //When configure is called
        //Then values are configured.
        
        //Arrange
        var sut = new GameObject().AddComponent<Map2DStopController>();

        var map2DMock = new Mock<IMap2D>();
        map2DMock.Setup(x => x.Configure(It.IsAny<Map2D.Config>())).Verifiable();
        
        var map2DInstantiatorMock = new Mock<IMap2DInstantiator>();
        map2DInstantiatorMock.Setup(x => x.Create())
            .Returns(map2DMock.Object)
            .Verifiable();

        var dependencies = CreateDependencies(
            map2DInstantiatorMock, 
            null, 
            null, 
            null,
            null);
        sut.SetDependencies(dependencies);

        //Act
        sut.ConfigureAndOpenStop(new Map2DStopController.Config()
        {
            Stop = _map2DStop.Object
        });
        
        //Assert
        map2DInstantiatorMock.Verify(x => x.Create());
        map2DMock.Verify(x => x.Configure(It.IsAny<Map2D.Config>()));
    }
    
    [Test]
    public void TestConfigure_CollectsMap2DCanvasController_And_Configures()
    {
        //Given a new Map2DStopController
        //When configure is called
        //Then an instance of the map2DCanvasController is collected and configured.
        
        //Arrange
        var mapCanvasPath = "mapCanvasPath";
        var mapCanvasAddressable = new AddressableWithTag("GameCanvas", mapCanvasPath);
        var mapResource = new MapResource()
        {
            MapCanvasResource = new MapCanvasResource()
            {
                MapCanvasPrefab = mapCanvasAddressable
            }
        };
        _map2DStop.Setup(x=> x.MapResource).Returns(mapResource);
        var sut = new GameObject().AddComponent<Map2DStopController>();

        var map2DCanvasConfig = new MapCanvasController.Config()
        {
            CanvasConfig = new CanvasController.Config() {},
        };
        
        var map2DCanvasControllerMock = new Mock<IMapCanvasController>();
        map2DCanvasControllerMock.Setup(x => x.ConfigureAndDisplay(map2DCanvasConfig))
            .Verifiable();

        
        var map2DCanvasControllerInstantiatorMock = CreateMap2DCanvasControllerInstantiator(mapCanvasAddressable, map2DCanvasControllerMock);
       
        var map2DMock = new Mock<IMap2D>();
        map2DMock.Setup(x => x.Configure(It.IsAny<Map2D.Config>()))
            .Callback<Map2D.Config>((theConfig) =>
            {
                theConfig.InitializationComplete.Invoke(true);
            })
            .Verifiable();

        map2DMock.Setup(x => x.GetCamera()).Returns(_camera).Verifiable();
        var map2DInstantiatorMock = new Mock<IMap2DInstantiator>();
        map2DInstantiatorMock.Setup(x => x.Create())
            .Returns(map2DMock.Object)
            .Verifiable();

        var dependencies = CreateDependencies(
            map2DInstantiatorMock,
            map2DCanvasControllerInstantiatorMock, 
            null);
        
        sut.SetDependencies(dependencies);
        
        //Act
        sut.ConfigureAndOpenStop(new Map2DStopController.Config()
        {
            Stop = _map2DStop.Object,
            MapCanvasControllerConfig = map2DCanvasConfig
        });
        
        //Assert
        map2DCanvasControllerMock.Verify(x => x.ConfigureAndDisplay(map2DCanvasConfig));
        map2DMock.Verify(x => x.GetCamera());
    }

    private Mock<IStop> CreateStop(string id)
    {
        var stopMock = new Mock<IStop>();
        stopMock.Setup(x => x.Id).Returns(id).Verifiable();
        return stopMock;
    }

    private Mock<IStep> CreateStep(string id)
    {
        var stepMock = new Mock<IStep>();
        stepMock.Setup(x => x.Id).Returns(id).Verifiable();
        return stepMock;
    }
    private POI2DListAsset.Poi2DAsset CreatePoi2DAsset(string resourceId, string id)
    {
        var asset = new POI2DListAsset.Poi2DAsset()
        {
            ResourceId = resourceId,
            Id = id
        };
        return asset;
    }

    private POI2DResource CreatePoi2DResource(string resourceId)
    {
        return new POI2DResource(new Mock<IIcon>().Object)
        {
            ResourceId = resourceId,
        };
    }
    [Test]
    public void TestConfigure_SpawnsPOIs_On_Map()
    {
        //Given a new Map2DStopController
        //When configure is called
        //Then after the map is created, POI's are spawned and mapped on top.

        //Arrange
        _map2DStop = new Mock<IMap2DStop>();
        _map2DStop.Setup(x => x.Title).Returns("defaultTitle");
        _map2DStop.Setup(x => x.Id).Returns("defaultId");

        var stop1Index0 = CreateStop("stop1");
        var stop1Index1 = CreateStop("stop2");

        var stopList = new List<IStop>() { stop1Index0.Object, stop1Index1.Object };
        _map2DStop.Setup(x => x.Stops).Returns(stopList).Verifiable();

        var step1 = CreateStep("step1");
        var step2 = CreateStep("step2");
        var stepList = new List<IStep>() { step1.Object, step2.Object };
        _map2DStop.Setup(x => x.Steps).Returns(stepList).Verifiable();

        var stopPoiData1 = CreatePoi2DAsset("resource1", "stop1");
        var stopPoiData2 = CreatePoi2DAsset("resource2", "stop2");
        var stopPoiData3 = CreatePoi2DAsset("resource3", "step1");
        var stopPoiData4 = CreatePoi2DAsset("resource4", "step2");

        _map2DStop.Setup(x => x.PoiData).Returns(
            new POI2DListAsset(
                new List<POI2DListAsset.Poi2DAsset>()
                {
                    stopPoiData1,stopPoiData2, stopPoiData3, stopPoiData4
                })
            );

        var stopPoiResource = CreatePoi2DResource("resource1");
        var step1PoiResource = CreatePoi2DResource("resource2");
        var step2PoiResource = CreatePoi2DResource("resource3");

        MapResource mapResource = new MapResource()
        {
            MapCanvasResource = _mapResource.MapCanvasResource,
            PoiResources = new List<POI2DResource>()
            {
                stopPoiResource, step1PoiResource, step2PoiResource
            },
        };
        _map2DStop.Setup(x => x.MapResource).Returns(mapResource).Verifiable();


        var mapMock = new Mock<IMap2D>();
        mapMock.Setup(x=> x.CreatePOIForMap(It.IsAny<POIController.Config>())).Verifiable();
        mapMock.Setup(x => x.Configure(It.IsAny<Map2D.Config>()))
            .Callback<Map2D.Config>((theConfig) =>
            {
                theConfig.InitializationComplete.Invoke(true);
            });
        
        var sut = new GameObject().AddComponent<Map2DStopController>();
        var dependencies = CreateDependencies(
            CreateMapInstantiator(mapMock), 
            CreateMap2DCanvasControllerInstantiator(_mapCanvasAddressable), 
            null);
        sut.SetDependencies(dependencies);
        
        //Act
        sut.ConfigureAndOpenStop(
            new Map2DStopController.Config()
            {
                Stop = _map2DStop.Object,
                MapCanvasControllerConfig = new MapCanvasController.Config(),
            });

        //Assert
        _map2DStop.Verify(x => x.Steps);
        _map2DStop.Verify(x => x.Stops);
        mapMock.Verify(x=> x.CreatePOIForMap(It.IsAny<POIController.Config>()), Times.Exactly(4));

    }

    [TestCase(true)]
    [TestCase(false)]
    [Test]
    public void TestEndStop_EndStopCalled_From_Config(bool completed)
    {
        //Given a configured Map2DStopController
        //When endStop is called
        //Then the action in config is called.
        
        //Arrange
        var sut = new GameObject().AddComponent<Map2DStopController>();
        sut.gameObject.SetActive(true);

        var dependencies = CreateDependencies(_map2DInstantiatorMock);
        sut.SetDependencies(dependencies);

        bool hasBeenCalled = false;
        bool value = !completed;
        _config.EndStop = (completed) =>
        {
            hasBeenCalled = true;
            value = completed;
        };
        
        sut.ConfigureAndOpenStop(_config);
        //Act
        sut.EndStop(completed);
        
        //Assert
        Assert.IsTrue(hasBeenCalled);
        Assert.AreEqual(completed, value);
        Assert.AreEqual(false, sut.gameObject.activeSelf);
    }
    
    [Test]
    public void TestOpenStop_EnablesGameObject()
    {
        //Given a Map2DStopController
        //When OpenStop is called
        //Then the gameobject is set Active
        
        //Arrange
        MapCanvasController.Config Map2DCanvasControllerConfig = new MapCanvasController.Config();
        
        var map2DCanvasController = new Mock<IMapCanvasController>();
        map2DCanvasController.Setup(x => x.ConfigureAndDisplay(Map2DCanvasControllerConfig)).Verifiable();
        var map2DCanvasControllerInstantiatorMock = new Mock<IMapCanvasControllerInstantiator>();
        map2DCanvasControllerInstantiatorMock.Setup(x => x.CreateOrCollectInstance(_mapCanvasAddressable))
            .ReturnsAsync(map2DCanvasController.Object)
            .Verifiable();

        var camera = new GameObject().AddComponent<Camera>();
        var map = new Mock<IMap2D>();
        map.Setup(x=> x.GetCamera()).Returns(camera).Verifiable();
        var map2DInstantiatorMock = new Mock<IMap2DInstantiator>();
        map2DInstantiatorMock.Setup(x => x.Create())
            .Returns(map.Object)
            .Verifiable();
        
        var sut = new GameObject().AddComponent<Map2DStopController>();
        sut.gameObject.SetActive(false);

        var dependencies = CreateDependencies(
            map2DInstantiatorMock, 
            map2DCanvasControllerInstantiatorMock,
            null,
            null);
        sut.SetDependencies(dependencies);
        _config.MapCanvasControllerConfig = Map2DCanvasControllerConfig;
        sut.ConfigureAndOpenStop(_config);
        //Act
        sut.OpenStop();
        //Assert
        Assert.AreEqual(true, sut.gameObject.activeSelf);
        map2DCanvasController.Verify(x => x.ConfigureAndDisplay(Map2DCanvasControllerConfig));
        map.Verify(x=> x.GetCamera());

    }

    [Test]
    public void TestCloseStop_DisablesGameObject()
    {
        //Given a Map2DStopController
        //When CloseStop is called
        //Then the gameobject is set InActive
        
        //Arrange
        var sut = new GameObject().AddComponent<Map2DStopController>();
        sut.gameObject.SetActive(true);
        var dependencies = CreateDependencies();
        sut.SetDependencies(dependencies);

        //Act
        sut.CloseStop();
        //Assert
        Assert.AreEqual(false, sut.gameObject.activeSelf);
    }

    [Test]
    public void TestStartStop_StartsStopOfSpecificID()
    {
        //Given a Map2DStopController, and a stop structure with two stops inside it.
        //When StartStop is called with the ID of one of them.
        //Then the stop with that Id is created, configured and started.
        //--- The current stop is stopped.

        var stopModelConverter = new Mock<IStopModelConverter>();
        
        var stop1Id = "stop1";
        
        _map2DStop = new Mock<IMap2DStop>();
        _map2DStop.Setup(x => x.Title).Returns("defaultTitle");
        _map2DStop.Setup(x => x.Id).Returns("defaultId");

        var stop1Index0 = CreateStop(stop1Id);
        var stop1Index1 = CreateStop("stop2");

        var stopList = new List<IStop>() { stop1Index0.Object, stop1Index1.Object };
        _map2DStop.Setup(x => x.Stops).Returns(stopList);

        var stopPoiData1 = CreatePoi2DAsset("resource1", stop1Id);
        var stopPoiData2 = CreatePoi2DAsset("resource1", "stop2");
        
        _map2DStop.Setup(x => x.PoiData).Returns(
            new POI2DListAsset(
                new List<POI2DListAsset.Poi2DAsset>()
                {
                    stopPoiData1,stopPoiData2
                })
            );

        var stopPoiResource = CreatePoi2DResource("resource1");
        MapResource mapResource = new MapResource()
        {
            MapCanvasResource = _mapResource.MapCanvasResource,
            PoiResources = new List<POI2DResource>()
            {
                stopPoiResource
            }
        };
        _map2DStop.Setup(x => x.MapResource).Returns(mapResource);

        var stopPoiMockA = new Mock<IPOIController>();
        stopPoiMockA.Setup( X=> X.Configure(It.IsAny<POIController.Config>())).Verifiable();
        var stopPoiMockB = new Mock<IPOIController>();
        stopPoiMockB.Setup( X=> X.Configure(It.IsAny<POIController.Config>())).Verifiable();
        
        var stop1MapStopController = new Mock<IMap2DStopController>();
        stop1MapStopController
            .Setup(x => x.ConfigureAndOpenStop(It.IsAny<Map2DStopController.Config>()))
            .Verifiable();

        stopModelConverter.Setup(x => x.ConvertMap2DModel(stop1Index0.Object)).Returns(new Mock<IMap2DStop>().Object);
        
        var map2DstopControllerInstantiator = new Mock<IStopControllerInstantiator>();
        map2DstopControllerInstantiator.Setup(x => x.CreateMap2D(null)).Returns(stop1MapStopController.Object).Verifiable();
        
        var sut = new GameObject().AddComponent<Map2DStopController>();
        var dependencies = CreateDependencies(
            CreateMapInstantiator(), 
            CreateMap2DCanvasControllerInstantiator(_mapCanvasAddressable), 
            null, 
            null,
            stopModelConverter);
        
        sut.SetDependencies(dependencies);
        
        sut.ConfigureAndOpenStop(
            new Map2DStopController.Config()
            {
                Stop = _map2DStop.Object,
                StopControllerInstantiator = map2DstopControllerInstantiator.Object,
                MapCanvasControllerConfig = new MapCanvasController.Config(),
            });
        
        //Act
        sut.StartStop(stop1Id);
        
        //Assert
        stop1MapStopController.Verify(x => x.ConfigureAndOpenStop(It.IsAny<Map2DStopController.Config>()));
        stopModelConverter.Verify(x => x.ConvertMap2DModel(stop1Index0.Object));
        Assert.IsFalse(sut.gameObject.activeSelf);
    }
    
    [Test]
    public void TestStartStep_StartsStepOfSpecificID()
    {
        //Given a Map2DStopController, and a stop structure with steps and stops inside it.
        //When StartStep is called with the ID of a contained step.
        //Then a stepcontroller with that model is created, configured and started.

        //Arrange
        var stepToStartID = "step1";
        
        _map2DStop = new Mock<IMap2DStop>();
        _map2DStop.Setup(x => x.Title).Returns("defaultTitle");
        _map2DStop.Setup(x => x.Id).Returns("defaultId");

        var stop1Index0 = CreateStop("stop1");
        var stop1Index1 = CreateStop("stop2");

        var stopList = new List<IStop>() { stop1Index0.Object, stop1Index1.Object };
        _map2DStop.Setup(x => x.Stops).Returns(stopList).Verifiable();

        var step1 = CreateStep("step1");
        step1.Setup(x => x.Id).Returns(stepToStartID).Verifiable();
        step1.Setup(x => x.Type).Returns(StepType.DisplayRiddleAndSubmitAnswer);

        var step2 = CreateStep("step2");
        step2.Setup(x => x.Id).Returns("step2").Verifiable();
        var stepList = new List<IStep>() { step1.Object, step2.Object };
        _map2DStop.Setup(x => x.Steps).Returns(stepList).Verifiable();

        var stopPoiData1 = CreatePoi2DAsset("resource1", "stop1"); 
        var stopPoiData2 = CreatePoi2DAsset("resource1", "stop2");
        var stopPoiData3 = CreatePoi2DAsset("resource2", "step1");
        var stopPoiData4 = CreatePoi2DAsset("resource3", "step2");

        _map2DStop.Setup(x => x.PoiData).Returns(
            new POI2DListAsset(
                new List<POI2DListAsset.Poi2DAsset>()
                {
                    stopPoiData1,stopPoiData2, stopPoiData3, stopPoiData4
                })
            );

        var stopPoiResource = CreatePoi2DResource("resource1");
        var step1PoiResource = CreatePoi2DResource("resource2");
        var step2PoiResource = CreatePoi2DResource("resource3");

        MapResource mapResource = new MapResource()
        {
            MapCanvasResource = _mapResource.MapCanvasResource,
            PoiResources = new List<POI2DResource>()
            {
                stopPoiResource, step1PoiResource, step2PoiResource
            }
        };
        _map2DStop.Setup(x => x.MapResource).Returns(mapResource).Verifiable();
        
        var stepControllerMock = new Mock<IStepController>();
        stepControllerMock
            .Setup(x => x.StartStep(step1.Object, It.IsAny<IMapCanvasController>(), It.IsAny<Action>()))
            .Verifiable();

        stepControllerMock.Setup(x => x.GetModelId()).Returns("dette er det rigtige objekt");
        
        var stepControllerInstantiatorMock = new Mock<IStepControllerInstantiator>();
        stepControllerInstantiatorMock
            .Setup(x => 
                x.CreateDisplayRiddleAndSubmitAnswerStepController())
            .Returns(stepControllerMock.Object);
        
        
        var sut = new GameObject().AddComponent<Map2DStopController>();
        var dependencies = CreateDependencies(
            CreateMapInstantiator(), 
            CreateMap2DCanvasControllerInstantiator(_mapCanvasAddressable), 
            null, 
            stepControllerInstantiatorMock);
        
        sut.SetDependencies(dependencies);
        
        sut.ConfigureAndOpenStop(
            new Map2DStopController.Config()
            {
                Stop = _map2DStop.Object,
                MapCanvasControllerConfig = new MapCanvasController.Config(),
            });
        
        //Act
        sut.StartStep(stepToStartID);
        
        //Assert
        step1.Verify(x => x.Id);
        stepControllerInstantiatorMock
            .Verify(x => x.CreateDisplayRiddleAndSubmitAnswerStepController());
        stepControllerMock
            .Verify(x => x.StartStep(step1.Object, It.IsAny<IMapCanvasController>(), It.IsAny<Action>()));
    }
}