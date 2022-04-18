using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using Hunt;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products.Assets.Adressables;
using riddlehouse_libraries.products.Assets.Map2DAssets;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.resources;
using riddlehouse_libraries.products.resources.Map;
using riddlehouse_libraries.products.Steps;
using riddlehouse_libraries.products.Stops;
using StopControllers;
using UnityEngine;

[TestFixture]
public class TestMapBoxStopController : MonoBehaviour
{
    
    private string _mapCanvasPath;
    private MapResource _mapResource;
    private AddressableWithTag _mapCanvasAddressable;

    [SetUp]
    public void Init()
    {
        _mapCanvasPath = "Assets/MapMenu.prefab";
        _mapCanvasAddressable = new AddressableWithTag("GameCanvas", _mapCanvasPath);
        _mapResource = new MapResource()
        {
            MapCanvasResource = new MapCanvasResource()
            {
                MapCanvasPrefab = _mapCanvasAddressable
            }
        };
    }
    
    private MapBoxStopController.Dependencies CreateDependencies(
        Mock<IMapboxMapInstantiator> mapBoxMapInstantiator = null,
        Mock<IGameObjectDestroyer> godMock = null,
        Mock<IMapCanvasControllerInstantiator> mapCanvasInstantiatorMock = null,
        Mock<IStepControllerInstantiator> stepControllerInstantiatorMock = null,
        Mock<IStopModelConverter> stopModelConverterMock = null,
        Mock<IStopControllerContainer> stopControllerContainerMock = null)
    {
        mapBoxMapInstantiator ??= new Mock<IMapboxMapInstantiator>();
        godMock ??= new Mock<IGameObjectDestroyer>();
        mapCanvasInstantiatorMock ??= new Mock<IMapCanvasControllerInstantiator>();
        stepControllerInstantiatorMock ??= new Mock<IStepControllerInstantiator>();
        stopModelConverterMock ??= new Mock<IStopModelConverter>();
        stopControllerContainerMock ??= new Mock<IStopControllerContainer>();
        return new MapBoxStopController.Dependencies()
        {
            MapboxMapInstantiator = mapBoxMapInstantiator.Object,
            GOD = godMock.Object,
            MapCanvasControllerInstantiater = mapCanvasInstantiatorMock.Object,
            StepControllerInstantiator = stepControllerInstantiatorMock.Object,
            StopModelConverter = stopModelConverterMock.Object,
            StopControllerContainer = stopControllerContainerMock.Object
        };
    }

    private POI2DListAsset CreatePOI2DListAsset(string stopId1, string stopId2, string stepId, string stopResource, string stepResource)
    {
        return new POI2DListAsset(new List<POI2DListAsset.Poi2DAsset>()
        {
            new POI2DListAsset.Poi2DAsset()
            {
                Id = stopId1,
                ResourceId = stopResource
            },
            new POI2DListAsset.Poi2DAsset()
            {
                Id = stopId2,
                ResourceId = stopResource
            },
            new POI2DListAsset.Poi2DAsset()
            {
                Id = stepId,
                ResourceId = stepResource
            }
        });
    }

    private List<POI2DResource> CreatePoi2DResources(string stopResource, string stepResource)
    {
        return  new List<POI2DResource>()
        {
            new POI2DResource(new Mock<IIcon>().Object)
            {
                ResourceId = stopResource
            },
            new POI2DResource(new Mock<IIcon>().Object)
            {
                ResourceId = stepResource
            }
        };
    }
    
    private Mock<IMapCanvasControllerInstantiator> CreateMap2DCanvasControllerInstantiator(AddressableWithTag addressable, Mock<IMapCanvasController> mapCanvasController = null)
    {
        mapCanvasController ??= new Mock<IMapCanvasController>();
        var map2DCanvasControllerInstantiatorMock = new Mock<IMapCanvasControllerInstantiator>();
        map2DCanvasControllerInstantiatorMock.Setup(x => x.CreateOrCollectInstance(addressable))
            .ReturnsAsync(mapCanvasController.Object)
            .Verifiable();

        return map2DCanvasControllerInstantiatorMock;
    }
    
    [Test]
    public void TestSetDependencies()
    {
        //Arrange
        var sut = new GameObject().AddComponent<MapBoxStopController>();
        var dependencies = CreateDependencies();
        
        //Act
        sut.SetDependencies(dependencies);

        //Assert
        Assert.AreEqual(dependencies, sut._dependencies);
    }

    [Test]
    public void TestConfigure_CreatesAndConfigures_MapBoxMap()
    {
        //Arrange
        var sut = new GameObject().AddComponent<MapBoxStopController>();
        var mapCameraResource = new MapCameraResource();
        _mapResource.MapCameraResource = mapCameraResource;

        var stopMock = new Mock<IStop>();
        stopMock.Setup(x => x.Stops).Returns(new List<IStop>());
        
        var mapBoxStopModel = new Mock<IMapBoxStop>();
        mapBoxStopModel.Setup(x => x.MapResource)
            .Returns(_mapResource)
            .Verifiable();
        mapBoxStopModel.Setup(x => x.CenterPoint).Returns(new Map2DPosition(12.22,14.22)).Verifiable();
        
        var mapBoxMock = new Mock<IMapBoxMap>();
        mapBoxMock.Setup(x => x.Configure(It.IsAny<MapBoxMap.Config>())).Verifiable();

        var mapBoxInstantiator = new Mock<IMapboxMapInstantiator>();
        mapBoxInstantiator.Setup(x => x.CreateOrCollect(sut.transform)).Returns(mapBoxMock.Object).Verifiable();
        
        var dependencies = CreateDependencies(mapBoxInstantiator);
        sut.SetDependencies(dependencies);

        //Act
        sut.Configure(new MapBoxStopController.Config()
        {
            Stop = mapBoxStopModel.Object
        });
        
        //Assert
        mapBoxStopModel.Verify(x => x.MapResource);
        mapBoxInstantiator.Verify(x => x.CreateOrCollect(sut.transform));
        mapBoxMock.Verify(x => x.Configure(It.IsAny<MapBoxMap.Config>()));
        mapBoxStopModel.Verify(x => x.CenterPoint);
    }

    [Test]
    public void TestConfigure_CreatesAndPositions_POIs()
    {
        //Given a MapBoxStopController and a stopmodel that contains stops and steps
        //When Configure is called
        //Then the stopcontroller will create and configure step and stop buttons on the map
        
        //Arrange
        var sut = new GameObject().AddComponent<MapBoxStopController>();

        var stopId1 = "subStop1";
        var stopId2 = "subStop2";
        var stepId = "step1";

        var stopResource = "resourceId1";
        var stepResource = "resourceId2";

        var poiData = CreatePOI2DListAsset(stopId1, stopId2, stepId, stopResource, stepResource);

        var poiResources = CreatePoi2DResources(stopResource, stepResource);
        
        var mapResource = new MapResource()
        {
            MapCameraResource = new MapCameraResource(),
            PoiResources = poiResources,
            MapCanvasResource = new MapCanvasResource()
            {
                MapCanvasPrefab = new AddressableWithTag("GameCanvas", _mapCanvasPath)
            }
        };
        
        var subStopMockA = new Mock<IStop>();
        subStopMockA.Setup(x => x.Id).Returns(stopId1);
        var subStopMockB = new Mock<IStop>();
        subStopMockB.Setup(x=> x.Id).Returns(stopId2);
        
        var stepMock = new Mock<IStep>();
        stepMock.Setup(x => x.Id).Returns(stepId);
        
        var mapBoxStopModel = new Mock<IMapBoxStop>();
        mapBoxStopModel.Setup(x => x.MapResource)
            .Returns(mapResource)
            .Verifiable();
        mapBoxStopModel.Setup(x => x.PoiData).Returns(poiData).Verifiable();
        mapBoxStopModel.Setup(x => x.Stops).Returns(new List<IStop>()
        {
            subStopMockA.Object,
            subStopMockB.Object
        });
        mapBoxStopModel.Setup(x => x.Steps).Returns(new List<IStep>()
        {
            stepMock.Object
        });
        
        var mapBoxMock = new Mock<IMapBoxMap>();
        mapBoxMock.Setup(x => 
            x.CreatePositionAndConfigurePoi(poiData.PoiData[0], poiResources[0], It.IsAny<Action<string>>()))
            .Verifiable();
        mapBoxMock.Setup(x => 
            x.CreatePositionAndConfigurePoi(poiData.PoiData[1], poiResources[0], It.IsAny<Action<string>>()))
            .Verifiable();
        mapBoxMock.Setup(x => 
            x.CreatePositionAndConfigurePoi(poiData.PoiData[2], poiResources[1], It.IsAny<Action<string>>()))
            .Verifiable();

        var mapBoxInstantiator = new Mock<IMapboxMapInstantiator>();
        mapBoxInstantiator.Setup(x => x.CreateOrCollect(sut.transform)).Returns(mapBoxMock.Object).Verifiable();

        var dependencies = CreateDependencies(mapBoxInstantiator);
        sut.SetDependencies(dependencies);
        
        //Act
        sut.Configure(new MapBoxStopController.Config()
        {
            Stop = mapBoxStopModel.Object
        });
        
        //Assert
        mapBoxStopModel.Verify(x => x.PoiData);
        mapBoxStopModel.Verify(x => x.MapResource);
        mapBoxMock.Verify(x => x.CreatePositionAndConfigurePoi(poiData.PoiData[0], poiResources[0], It.IsAny<Action<string>>()));
        mapBoxMock.Verify(x => x.CreatePositionAndConfigurePoi(poiData.PoiData[1], poiResources[0], It.IsAny<Action<string>>()));
        mapBoxMock.Verify(x => x.CreatePositionAndConfigurePoi(poiData.PoiData[2], poiResources[1], It.IsAny<Action<string>>()));
    }
    
     [Test]
    public void TestStartStop_StartsMapBox()
    {
        //Given a configured and started MapBoxController
        //When StartStop is called with an Id
        //Then the stopcontroller will create and configure the stop in question.
        
        //Arrange
        var sut = new GameObject().AddComponent<MapBoxStopController>();

        var stopId1 = "subStop1";
        var poiData = new POI2DListAsset(new List<POI2DListAsset.Poi2DAsset>()
        {
            new POI2DListAsset.Poi2DAsset()
            {
                Id = stopId1,
                ResourceId = "stopResource"
            }
        });



        var poiResources = new List<POI2DResource>()
        {
            new POI2DResource(new Mock<IIcon>().Object)
            {
                ResourceId = "stopResource"
            }
        };
        
        var mapResource = new MapResource()
        {
            MapCameraResource = new MapCameraResource(),
            PoiResources = poiResources,
            MapCanvasResource = new MapCanvasResource()
            {
                MapCanvasPrefab = new AddressableWithTag("GameCanvas", _mapCanvasPath)
            }
        };
        
        var substopToStart = new Mock<IStop>();
        substopToStart.Setup(x => x.Id).Returns(stopId1);
        substopToStart.Setup(x => x.Type).Returns(StopType.MapStop2D).Verifiable();

        var mapBoxStopModel = new Mock<IMapBoxStop>();
        mapBoxStopModel.Setup(x => x.MapResource)
            .Returns(mapResource);
        mapBoxStopModel.Setup(x => x.PoiData).Returns(poiData).Verifiable();
        mapBoxStopModel.Setup(x => x.Stops).Returns(new List<IStop>()
        {
            substopToStart.Object,
        });
        mapBoxStopModel.Setup(x => x.Steps).Returns(new List<IStep>());
        
        var mapBoxMock = new Mock<IMapBoxMap>();

        var mapBoxInstantiator = new Mock<IMapboxMapInstantiator>();
        mapBoxInstantiator.Setup(x => x.CreateOrCollect(sut.transform)).Returns(mapBoxMock.Object).Verifiable();
        
        var map2DStopControllerMock = new Mock<IMap2DStopController>();
        map2DStopControllerMock.Setup(x => x.ConfigureAndOpenStop(It.IsAny<Map2DStopController.Config>())).Verifiable();
        
        
        var stopControllerInstantiator = new Mock<IStopControllerInstantiator>();
        stopControllerInstantiator
            .Setup(x => x.CreateMap2D(null))
            .Returns(map2DStopControllerMock.Object)
            .Verifiable();
        
        var stopControllerContainer = new Mock<IStopControllerContainer>();
        stopControllerContainer.Setup(x => x.Add(map2DStopControllerMock.Object)).Verifiable();
        
        var dependencies = CreateDependencies(
            mapBoxInstantiator, 
            null, 
            null, 
            null,
            null,
            stopControllerContainer);
        
        sut.SetDependencies(dependencies);

        sut.Configure(new MapBoxStopController.Config()
        {
            Stop = mapBoxStopModel.Object,
            StopControllerInstantiator = stopControllerInstantiator.Object
        });
        
        //Act
        sut.StartStop(stopId1);
        
        //Assert
        map2DStopControllerMock.Verify(x => x.ConfigureAndOpenStop(It.IsAny<Map2DStopController.Config>()));
        stopControllerInstantiator.Verify(x => x.CreateMap2D(null));
        stopControllerContainer.Verify(x => x.Add(map2DStopControllerMock.Object));
        substopToStart.Verify(x => x.Type);
    }
    
    [Test]
    public void TestDestroySelf_NoSubControllers()
    {
        //Arrange
        var sut = new GameObject().AddComponent<MapBoxStopController>();

        var godMock = new Mock<IGameObjectDestroyer>();
        godMock.Setup(x => x.Destroy()).Verifiable();
        
        
        var stopMock = new Mock<IStop>();
        stopMock.Setup(x => x.Stops).Returns(new List<IStop>());

        var mapBoxStopModel = new Mock<IMapBoxStop>();
        mapBoxStopModel.Setup(x => x.MapResource)
            .Returns(_mapResource)
            .Verifiable();
        
        var mapBoxMock = new Mock<IMapBoxMap>();
        mapBoxMock.Setup(x => x.DestroySelf()).Verifiable();
        var mapBoxInstantiator = new Mock<IMapboxMapInstantiator>();
        mapBoxInstantiator.Setup(x => x.CreateOrCollect(sut.transform)).Returns(mapBoxMock.Object).Verifiable();

        var mapCanvasController = new Mock<IMapCanvasController>();
        mapCanvasController.Setup(x => x.Hide()).Verifiable();

        var mapCanvasInstantiator = CreateMap2DCanvasControllerInstantiator(_mapCanvasAddressable, mapCanvasController);

        var stopControllerContainerMock = new Mock<IStopControllerContainer>();
        stopControllerContainerMock.Setup(x => x.RemoveAndDestroyAllControllers()).Verifiable();
        var dependencies = CreateDependencies(
            mapBoxInstantiator, 
            godMock, 
            mapCanvasInstantiator,
            null, 
            null,
            stopControllerContainerMock);
        
        sut.SetDependencies(dependencies);

        sut.Configure(new MapBoxStopController.Config()
        {
            Stop = mapBoxStopModel.Object
        });
        
        //Act
        sut.DestroySelf();

        //Assert
        godMock.Verify(x => x.Destroy());
        mapBoxMock.Verify(x => x.DestroySelf());
        mapCanvasController.Verify(x => x.Hide());
        stopControllerContainerMock.Verify(x => x.RemoveAndDestroyAllControllers());
    }

    [Test]
    public void TestGetId()
    {
        //Arrange
        var sut = new GameObject().AddComponent<MapBoxStopController>();

        var theId = "theId";
        var mapBoxStopModel = new Mock<IMapBoxStop>();
        mapBoxStopModel.Setup(x => x.MapResource)
            .Returns(new MapResource())
            .Verifiable();

        mapBoxStopModel.Setup(x => x.Id).Returns(theId).Verifiable();
        
        var mapBoxMock = new Mock<IMapBoxMap>();
        var mapBoxInstantiator = new Mock<IMapboxMapInstantiator>();
        mapBoxInstantiator.Setup(x => x.CreateOrCollect(sut.transform)).Returns(mapBoxMock.Object);
        
        var dependencies = CreateDependencies(mapBoxInstantiator);
        sut.SetDependencies(dependencies);

        sut.Configure(new MapBoxStopController.Config()
        {
            Stop = mapBoxStopModel.Object
        });
        //Act
        var id = sut.GetId();
        
        //Assert
        Assert.AreEqual(theId, id);
    }

    [Test]
    public void OpenStop_EnablesGameObject_DisplaysMapCanvas()
    {
        //Given a configured MapBoxStopController
        //When OpenStop is called
        //Then the view is made ready to display.
        
        //Arrange
        var sut = new GameObject().AddComponent<MapBoxStopController>();
        
        var mapBoxStopModel = new Mock<IMapBoxStop>();
        mapBoxStopModel.Setup(x => x.MapResource)
            .Returns(_mapResource)
            .Verifiable();
        
        var mapCanvasControllerconfig = new MapCanvasController.Config();
        
        var mapCanvasController = new Mock<IMapCanvasController>();
        mapCanvasController.Setup(x => x.ConfigureAndDisplay(mapCanvasControllerconfig)).Verifiable();

        var mapCanvasInstantiator = CreateMap2DCanvasControllerInstantiator(_mapCanvasAddressable, mapCanvasController);
        
        var camera = new GameObject().AddComponent<Camera>();
        
        var mapBoxMock = new Mock<IMapBoxMap>();
        var mapBoxInstantiator = new Mock<IMapboxMapInstantiator>();
        mapBoxInstantiator.Setup(x => x.CreateOrCollect(sut.transform)).Returns(mapBoxMock.Object);
        mapBoxMock.Setup(x => x.GetCamera()).Returns(camera).Verifiable();
        
        var dependencies = CreateDependencies(mapBoxInstantiator, null, mapCanvasInstantiator);
        sut.gameObject.SetActive(false);
        sut.SetDependencies(dependencies);
        
        sut.Configure(new MapBoxStopController.Config()
        {
            Stop = mapBoxStopModel.Object,
            MapCanvasControllerConfig = mapCanvasControllerconfig
        });
        
        //Act
        sut.OpenStop();
        
        //Assert
        Assert.IsTrue(sut.gameObject.activeSelf);
        mapCanvasController.Verify(x => x.ConfigureAndDisplay(mapCanvasControllerconfig));
        mapBoxMock.Verify(x => x.GetCamera());
    }
    
        [Test]
    public void OpenStop_UpdatesPOIStates()
    {
        //Given a configured MapBoxStopController, with two POIs
        //When OpenStop is called
        //Then the MapBox is called to update states.
        
        //Arrange
        var stopId1 = "subStop1";
        var stopId2 = "subStop2";
        var stepId = "step1";

        var resourceId1 = "resourceId1";
        var resourceId2 = "resourceId2";

        var poiData = CreatePOI2DListAsset(stopId1, stopId2, stepId, resourceId1, resourceId2);

        var poiResources = CreatePoi2DResources(resourceId1, resourceId2);
        
        var sut = new GameObject().AddComponent<MapBoxStopController>();
        
        var mapResource = new MapResource()
        {
            MapCanvasResource = new MapCanvasResource()
            {
                MapCanvasPrefab = new AddressableWithTag("GameCanvas", _mapCanvasPath)
            },
            PoiResources = poiResources,
        };
        
        var camera = new GameObject().AddComponent<Camera>();
        var mapBoxMock = new Mock<IMapBoxMap>();
        var mapBoxInstantiator = new Mock<IMapboxMapInstantiator>();
        mapBoxInstantiator.Setup(x => x.CreateOrCollect(sut.transform)).Returns(mapBoxMock.Object);
        mapBoxMock.Setup(x => x.GetCamera()).Returns(camera).Verifiable();
        
        var stopMock = new Mock<IStop>();
        stopMock.Setup(x => x.Stops).Returns(new List<IStop>());
        
        var mapBoxStopModel = new Mock<IMapBoxStop>();
        mapBoxStopModel.Setup(x => x.MapResource)
            .Returns(mapResource)
            .Verifiable();
        mapBoxStopModel.Setup(x => x.PoiData).Returns(poiData).Verifiable();
        mapBoxStopModel.Setup(x => x.MapResource).Returns(_mapResource);
        var idA = "A";
        var stateA = PoiStates.Idle;
        var subStopA = CreatePOISubStopAndAttachVerify(idA);
        mapBoxStopModel.Setup(x => x.GetState(idA)).Returns(stateA).Verifiable();

        var idB = "B";
        var stateB = PoiStates.Completed;
        var subStopB = CreatePOISubStopAndAttachVerify(idB);
        mapBoxStopModel.Setup(x => x.GetState(idB)).Returns(stateB).Verifiable();

        var stops = new List<IStop>()
        {
            subStopA.Object,
            subStopB.Object
        };
        
        var idStepA = "StepA";
        var stateStepA = PoiStates.Idle;
        var stepA = CreateStepInStopAndAttachVerify(mapBoxStopModel, idStepA, stateStepA);
        
        var idStepB = "StepB";
        var stateStepB = PoiStates.Completed;
        var stepB = CreateStepInStopAndAttachVerify(mapBoxStopModel, idStepB, stateStepB);

        var steps = new List<IStep>()
        {
            stepA.Object,
            stepB.Object
        };
        
        mapBoxStopModel.Setup(x => x.Stops).Returns(stops).Verifiable();
        mapBoxStopModel.Setup(x => x.Steps).Returns(steps).Verifiable();

        mapBoxMock.Setup(x => x.UpdatePOIState(idA, stateA)).Verifiable();
        mapBoxMock.Setup(x => x.UpdatePOIState(idB, stateB)).Verifiable();

        var mapCanvasControllerconfig = new MapCanvasController.Config();
        
        var mapCanvasInstantiator = CreateMap2DCanvasControllerInstantiator(_mapCanvasAddressable);

        var dependencies = CreateDependencies(mapBoxInstantiator, null, mapCanvasInstantiator);
        sut.gameObject.SetActive(false);
        sut.SetDependencies(dependencies);
        
        sut.Configure(new MapBoxStopController.Config()
        {
            Stop = mapBoxStopModel.Object,
            MapCanvasControllerConfig = mapCanvasControllerconfig
        });
        
        //Act
        sut.OpenStop();
        
        //Assert
        mapBoxStopModel.Verify(x => x.PoiData);
        
        mapBoxStopModel.Verify(x => x.Stops);
        mapBoxStopModel.Verify(x => x.Steps);
        subStopA.Verify(x => x.Id);
        subStopB.Verify(x => x.Id);
        
        VerifyPOIStep(mapBoxStopModel, idStepA, stepA);
        VerifyPOIStep(mapBoxStopModel, idStepB, stepB);
        mapBoxStopModel.Verify(x => x.GetState(idA));
        mapBoxStopModel.Verify(x => x.GetState(idB));
        mapBoxMock.Verify(x => x.UpdatePOIState(idA, stateA));
        mapBoxMock.Verify(x => x.UpdatePOIState(idB, stateB));
    }
    
    private Mock<IStop> CreatePOISubStopAndAttachVerify(string id)
    {
        var subStop = new Mock<IStop>();
        subStop.Setup(x => x.Id).Returns(id).Verifiable();
        return subStop;
    }

    private Mock<IStep> CreateStepInStopAndAttachVerify(Mock<IMapBoxStop> stop, string id, PoiStates state)
    {
        var subStep = new Mock<IStep>();
        stop.Setup(x => x.GetStepState(id)).Returns(state).Verifiable();
        subStep.Setup(x => x.Id).Returns(id).Verifiable();
        return subStep;
    }
    private void VerifyPOIStep(Mock<IMapBoxStop> stop, string id, Mock<IStep> stepMock)
    {
        stop.Verify(x => x.GetStepState(id));
        stepMock.Verify(x => x.Id);
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
        var sut = new GameObject().AddComponent<MapBoxStopController>();
        sut.gameObject.SetActive(true);
        
        var mapBoxStopModel = new Mock<IMapBoxStop>();
        mapBoxStopModel.Setup(x => x.MapResource)
            .Returns(new MapResource() {MapCameraResource = new MapCameraResource()})
            .Verifiable();
        mapBoxStopModel.Setup(x => x.Stops).Returns(new List<IStop>());

        var mapBoxMock = new Mock<IMapBoxMap>();
        var mapBoxInstantiator = new Mock<IMapboxMapInstantiator>();
        mapBoxInstantiator.Setup(x => x.CreateOrCollect(sut.transform)).Returns(mapBoxMock.Object);

        bool hasBeenCalled = false;
        bool value = !completed;
        
        var mapCanvasControllerconfig = new MapCanvasController.Config();
        var config = new MapBoxStopController.Config()
        {
            Stop = mapBoxStopModel.Object,
            MapCanvasControllerConfig = mapCanvasControllerconfig
        };
        
        config.EndStop = (completed) =>
        {
            hasBeenCalled = true;
            value = completed;
        };
        
        var dependencies = CreateDependencies(mapBoxInstantiator);
        sut.SetDependencies(dependencies);
        
        sut.Configure(config);
        //Act
        sut.EndStop(completed);
        
        //Assert
        Assert.IsTrue(hasBeenCalled);
        Assert.AreEqual(completed, value);
        Assert.AreEqual(false, sut.gameObject.activeSelf);
    }
}
