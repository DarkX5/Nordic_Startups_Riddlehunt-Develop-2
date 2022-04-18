using System;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products.Assets.Map2DAssets;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.resources;
using riddlehouse_libraries.products.resources.Map;
using UnityEngine;

public class TestMapBoxMap
{
    private MapBoxMap.Dependencies CreateDependencies(
        Mock<IMapCamController> mapCamControllerMock = null, 
        Mock<IFingersGestureInstantiator> fingersGestureInstantiatorMock = null,
        Mock<IMapBoxPOIPlacement> poiPlacementMock = null,
        Mock<IGameObjectDestroyer> godMock = null)
    {
        mapCamControllerMock ??= new Mock<IMapCamController>();
        fingersGestureInstantiatorMock ??= new Mock<IFingersGestureInstantiator>();
        poiPlacementMock ??= new Mock<IMapBoxPOIPlacement>();
        godMock ??= new Mock<IGameObjectDestroyer>();

        return new MapBoxMap.Dependencies()
        {
            Map = new GameObject().AddComponent<AbstractMap>(),
            LocationProviderFactory = new GameObject().AddComponent<LocationProviderFactory>(),
            MapCamController = mapCamControllerMock.Object,
            FingersGestureInstantiator = fingersGestureInstantiatorMock.Object,
            MapBoxPoiPlacement = poiPlacementMock.Object,
            GOD = godMock.Object
        };
    }
    [Test]
    public void TestSetDependencies()
    {
        var dependencies = CreateDependencies();
        var sut = new GameObject().AddComponent<MapBoxMap>();
        sut.SetDependencies(dependencies);
        
        Assert.AreEqual(dependencies, sut._dependencies);
    }
    
    [Test]
    public void TestConfigure_InstantiatesMapCamController()
    {
        //Given a new initialized MapBoxMap
        //When configure is called with a mapCameraResource
        //Then the Camera Controller is instantiated and configured with the required components.

        //Arrange
        var mapCamControllerMock = new Mock<IMapCamController>();
        mapCamControllerMock
            .Setup(x => x.Configure(It.IsAny<MapCamController.Config>()))
            .Verifiable();

        var camera = new GameObject().AddComponent<Camera>();
        mapCamControllerMock.Setup(x => x.GetCamera()).Returns(camera).Verifiable();
        
       var fingersGestureInstantiator = new Mock<IFingersGestureInstantiator>();
       fingersGestureInstantiator
           .Setup(x => x.CreatePanSingleTouchGesture())
           .Returns(new Mock<IPanGestureRecognizer>().Object)
           .Verifiable();
       
       fingersGestureInstantiator
           .Setup(x => x.CreatePinchGesture())
           .Returns(new Mock<IScaleGestureRecognizer>().Object)
           .Verifiable();

        var dependencies = CreateDependencies(mapCamControllerMock, fingersGestureInstantiator);
        var sut = new GameObject().AddComponent<MapBoxMap>();
        sut.SetDependencies(dependencies);
        
        //Act
        sut.Configure(new MapBoxMap.Config()
        {
            Resource = new MapCameraResource(),
            MapCoordinate = new Map2DPosition(12.22, 14.22)
        });
        
        //Assert
        mapCamControllerMock.Verify(x => x.Configure(It.IsAny<MapCamController.Config>()));
        
        fingersGestureInstantiator.Verify(x => x.CreatePanSingleTouchGesture());
        fingersGestureInstantiator.Verify(x => x.CreatePinchGesture());
        mapCamControllerMock.Verify(x => x.GetCamera());

    }
    
    [Test]
    public void TestCreatePositionAndConfigurePoi()
    {
        //Given a MapBoxMap, and a poi (asset and resource)
        //When CreatePositionAndConfigurePoi is called with those assets
        //Then the poi is created, configured, and placed on the XZ plane on the map.

        //Arrange
        var poiId = "id";
        var poiData = new POI2DListAsset.Poi2DAsset()
        {
            Id = "id",
            ActionId = "start",
            RealWorldPosition = new Map2DPosition(55.664, 12.379),
            ResourceId = "resource1"
        };

        var poiResource = new POI2DResource(new Mock<IIcon>().Object)
        {
            BackgroundColor = RHColor.Black,
            FrameColor = RHColor.White
        };
        
        var poiMock = new Mock<IPOIController>();
        poiMock.Setup(x => x.Configure(It.IsAny<POIController.Config>())).Verifiable();
        
        var mapBoxPoiPlacementMock = new Mock<IMapBoxPOIPlacement>();
        mapBoxPoiPlacementMock
            .Setup(x => 
                x.CreateAndConfigure(It.IsAny<POIController.Config>(),1f))
            .Verifiable();
        
        var dependencies = CreateDependencies(
            null, 
            null, 
            mapBoxPoiPlacementMock 
        );
        
        var sut = new GameObject().AddComponent<MapBoxMap>();
        sut.SetDependencies(dependencies);
        
        //Act
        sut.CreatePositionAndConfigurePoi(poiData, poiResource, (id) => {});
        
        //Assert
        mapBoxPoiPlacementMock
            .Verify(x => 
                x.CreateAndConfigure(It.IsAny<POIController.Config>(), 1f));
    }
    
    [Test]
    public void TestCreatePositionAndConfigurePoi_POIIsClicked_ClickedAction_Invoked()
    {
        //Given placed poi, and a configured action
        //When the poi is clicked
        //Then configured action is called.

        //Arrange
        string theId = "theId";
        
        var poiData = new POI2DListAsset.Poi2DAsset()
        {
            RealWorldPosition = new Map2DPosition(55.664, 12.379),
        };

        var poiResource = new POI2DResource(new Mock<IIcon>().Object);

        var mapboxPOIPlacementMock = new Mock<IMapBoxPOIPlacement>();
        mapboxPOIPlacementMock.Setup(x => x.CreateAndConfigure(It.IsAny<POIController.Config>(), 1f))
            .Callback<POIController.Config, float>((theConfig, theSpawnScale) =>
            {
                theConfig.ClickedAction.Invoke(theId);
            });

        var dependencies = CreateDependencies(
            null, 
            null, 
            mapboxPOIPlacementMock
        );
        
        var sut = new GameObject().AddComponent<MapBoxMap>();
        sut.SetDependencies(dependencies);

        var callbackString = "notTheId";
        Action<string> clickedAction = (id) => { callbackString = id; };
        
        //Act
        sut.CreatePositionAndConfigurePoi(poiData, poiResource, clickedAction);
        
        //Assert
        Assert.AreEqual(theId, callbackString);
        mapboxPOIPlacementMock.Verify(x => x.CreateAndConfigure(It.IsAny<POIController.Config>(), 1f));
    }
    
    [Test]
    public void TestDestroySelf()
    {
        //Given a MapBoxMap
        //When DestroySelf is called
        //Then it destroys itself and related dependencies
        var mapCamControllerMock = new Mock<IMapCamController>();
        mapCamControllerMock.Setup(x => x.DestroySelf()).Verifiable();

        var godMock = new Mock<IGameObjectDestroyer>();
        godMock.Setup(x => x.Destroy()).Verifiable();

        var mapBoxPoiPlacement = new Mock<IMapBoxPOIPlacement>();
        mapBoxPoiPlacement.Setup(x => x.DestroyAllPois()).Verifiable();
        
        var dependencies = CreateDependencies(
            mapCamControllerMock, 
            null, 
            mapBoxPoiPlacement, 
            godMock);
        var sut = new GameObject().AddComponent<MapBoxMap>();
        sut.SetDependencies(dependencies);
        
        //Act
        sut.DestroySelf();
        
        //Assert
        mapCamControllerMock.Verify(x => x.DestroySelf());
        mapBoxPoiPlacement.Verify(x => x.DestroyAllPois());
    }
    
    [Test]
    public void TestUpdatePOIState_CallsMapBoxPlacement()
    {
        //Given a MapBoxMap, and a poi (asset and resource)
        //When UpdatePOIState is called with an ID and a state
        //Then call is propagated.

        //Arrange
        var poiId = "id";
        var poiState = PoiStates.Idle;

        var mapBoxPoiPlacementMock = new Mock<IMapBoxPOIPlacement>();
        mapBoxPoiPlacementMock.Setup(x => x.UpdatePOIState(poiId, poiState)).Verifiable();
        var dependencies = CreateDependencies(
            null, 
            null, 
            mapBoxPoiPlacementMock
        );
        
        var sut = new GameObject().AddComponent<MapBoxMap>();
        sut.SetDependencies(dependencies);
        
        //Act
        sut.UpdatePOIState(poiId, poiState);
        
        //Assert
        mapBoxPoiPlacementMock.Verify(x => x.UpdatePOIState(poiId, poiState));

    }
}
