using System;
using System.Collections;
using System.Collections.Generic;
using Map;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products.Assets.Adressables;
using riddlehouse_libraries.products.Assets.Map2DAssets;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.resources;
using riddlehouse_libraries.products.resources.Map;
using riddlehouse_libraries.products.Stops;
using UnityEngine;
[TestFixture]
public class TestMap2D
{
    private Map2D.Dependencies CreateDependencies(
        Mock<IMapCamControllerInstantiator> mapCampControllerInstantiatorMock = null,
        Mock<IMapBorderInstantiator> mapBorderInstantiatorMock = null,
        Mock<IFingersGestureInstantiator> fingersGestureInstantiatorMock = null,
        Mock<IGameObjectDestroyer> godMock = null,
        Mock<IMap2DHelper> mapHelperMock = null,
        Mock<IMap2DPOIPlacement> map2DPOIPlacementMock = null)
    {
        mapCampControllerInstantiatorMock ??= new Mock<IMapCamControllerInstantiator>();
        mapBorderInstantiatorMock ??= new Mock<IMapBorderInstantiator>();
        fingersGestureInstantiatorMock ??= new Mock<IFingersGestureInstantiator>();
        godMock ??= new Mock<IGameObjectDestroyer>();
        mapHelperMock ??= new Mock<IMap2DHelper>();
        map2DPOIPlacementMock ??= new Mock<IMap2DPOIPlacement>();
        return new Map2D.Dependencies()
        {
            SR = new GameObject().AddComponent<SpriteRenderer>(),
            MapCamControllerInstantiator = mapCampControllerInstantiatorMock.Object,
            MapBorderInstantiator = mapBorderInstantiatorMock.Object,
            FingersGestureInstantiator = fingersGestureInstantiatorMock.Object,
            GOD = godMock.Object,
            MapHelper = mapHelperMock.Object,
            POILayer = new GameObject().transform,
            Map2DpoiPlacement = map2DPOIPlacementMock.Object
        };
    }

    private Mock<IMapCamControllerInstantiator> CreateMapCamController(
        Mock<IMapCamController> mapControllerMock)
    {
        var mapCamControllerInstantiatorMock = new Mock<IMapCamControllerInstantiator>();
        mapCamControllerInstantiatorMock.Setup(x => x.CreateXY()).Returns(mapControllerMock.Object).Verifiable();

        return mapCamControllerInstantiatorMock;
    }
    
    private Mock<IImageGetter> CreateImageGetter(Sprite sprite)
    {
        var imageGetterMock = new Mock<IImageGetter>();
        imageGetterMock.Setup(x => x.GetImage(It.IsAny<string>(), false, It.IsAny<Action<Sprite>>()))
            .Callback<string, bool, Action<Sprite>>((theUrl, theCache, theAction) =>
            {
                theAction.Invoke(sprite);  
            })
            .Verifiable();
        return imageGetterMock;
    }

    Sprite _map;
    private Mock<IMap2DStop> mapStopMock;
    private MapResource _mapResource;
    [SetUp]
    public void Init()
    {
        mapStopMock = new Mock<IMap2DStop>();
        
        _map = Resources.Load<Sprite>("sudeley_map");
        var byteMap = _map.texture.GetRawTextureData();
        var mapImageAsset = new Map2DImageAsset(byteMap);
        mapStopMock.Setup(x => x.MapImageAsset).Returns(mapImageAsset).Verifiable();

        var mapMenuPath = "Assets/MapMenu.prefab";
        _mapResource = new MapResource()
        {
            MapCanvasResource = new MapCanvasResource()
            {
                MapCanvasPrefab = new AddressableWithTag("GameCanvas", mapMenuPath)
            }
        };
        
        mapStopMock.Setup(x => x.MapResource).Returns(_mapResource).Verifiable();
    }
    
    [TearDown]
    public void TearDown()
    {
        _map = null;
        mapStopMock = null;
        _mapResource = null;
    }
    
    [Test]
    public void TestSetDependencies()
    {
        //Arrange
        var sut = new GameObject().AddComponent<Map2D>();
        var dependencies = CreateDependencies();
        //Act
        sut.SetDependencies(dependencies);
        //Assert
        Assert.AreEqual(dependencies,sut._dependencies);
    }

    [Test]
    public void TestConfigure_SetsUpNecessaryComponents()
    {
        //Given a new initialized Riddlehouse2DMap
        //When Configure is called
        //Then the surrounding system is setup, and the map is ready to run.
        
        //Arrange
        var mapStopMock = new Mock<IMap2DStop>();
        var map = Resources.Load<Sprite>("sudeley_map");
        var byteMap = map.texture.GetRawTextureData();

        var mapImageAsset = new Map2DImageAsset(byteMap);
        mapStopMock.Setup(x => x.MapImageAsset).Returns(mapImageAsset).Verifiable();
        var mapCanvasPath = "Assets/MapMenu.prefab";
        var mapResource = new MapResource()
        {
            MapCanvasResource = new MapCanvasResource()
            {
                MapCanvasPrefab = new AddressableWithTag("GameCanvas", mapCanvasPath)
            }
        };
        
        mapStopMock.Setup(x => x.MapResource).Returns(mapResource).Verifiable();

        
        var fingersGestureInstantiatorMock = new Mock<IFingersGestureInstantiator>();
        fingersGestureInstantiatorMock.Setup(x => x.CreatePinchGesture()).Verifiable();
        fingersGestureInstantiatorMock.Setup(x => x.CreatePanSingleTouchGesture()).Verifiable();

        var mapControllerMock = new Mock<IMapCamController>();
        mapControllerMock.Setup(x => x.Configure(It.IsAny<MapCamController.Config>())).Verifiable();
        var mapCamControllerInstantiatorMock = CreateMapCamController(mapControllerMock);

        var mapBorderInstantiatorMock = new Mock<IMapBorderInstantiator>();
        mapBorderInstantiatorMock.Setup(x => 
                x.Create(MapBorderDirection.North, It.IsAny<MapBorder.Dependencies>()))
            .Verifiable();
        mapBorderInstantiatorMock.Setup(x => 
                x.Create(MapBorderDirection.South, It.IsAny<MapBorder.Dependencies>()))
            .Verifiable();
        mapBorderInstantiatorMock.Setup(x => x
                .Create(MapBorderDirection.East, It.IsAny<MapBorder.Dependencies>()))
            .Verifiable();
        mapBorderInstantiatorMock.Setup(x => 
                x.Create(MapBorderDirection.West, It.IsAny<MapBorder.Dependencies>()))
            .Verifiable();

        var sut = new GameObject().AddComponent<Map2D>();
        var dependencies = CreateDependencies(mapCamControllerInstantiatorMock, mapBorderInstantiatorMock, fingersGestureInstantiatorMock);
        sut.SetDependencies(dependencies);
        
        //Act
        sut.Configure(new Map2D.Config()
        {
            Resource = mapStopMock.Object,
            InitializationComplete = (complete) => {}
        });
        
        //Assert
        fingersGestureInstantiatorMock.Verify(x => x.CreatePinchGesture());
        fingersGestureInstantiatorMock.Verify(x => x.CreatePanSingleTouchGesture());
        
        mapControllerMock.Verify(x => x.Configure(It.IsAny<MapCamController.Config>()));
        mapCamControllerInstantiatorMock.Verify(x => x.CreateXY());
        
        mapBorderInstantiatorMock.Verify(x => 
                x.Create(MapBorderDirection.North, It.IsAny<MapBorder.Dependencies>()));
        mapBorderInstantiatorMock.Verify(x => 
                x.Create(MapBorderDirection.South, It.IsAny<MapBorder.Dependencies>()));
        mapBorderInstantiatorMock.Verify(x => x
                .Create(MapBorderDirection.East, It.IsAny<MapBorder.Dependencies>()));
        mapBorderInstantiatorMock.Verify(x => 
                x.Create(MapBorderDirection.West, It.IsAny<MapBorder.Dependencies>()));
        
        mapStopMock.Verify(x => x.MapImageAsset);
        mapStopMock.Verify(x => x.MapResource);
    }
    
      [Test]
    public void TestDestroySelf_DestroysComponents_And_Self()
    {
        //Given a new configured Riddlehouse2DMap
        //When Destroy is called
        //Then the registered components as well as the Map is destroyed.
        
        //Arrange
        var mapControllerMock = new Mock<IMapCamController>();
        mapControllerMock.Setup(x => x.Configure(It.IsAny<MapCamController.Config>())).Verifiable();
        var mapCamControllerInstantiatorMock = CreateMapCamController(mapControllerMock);

        var godMock = new Mock<IGameObjectDestroyer>();
        godMock.Setup(x => x.Destroy()).Verifiable();
        
        var sut = new GameObject().AddComponent<Map2D>();
        var dependencies = CreateDependencies(
            mapCamControllerInstantiatorMock,
            null, 
            null, 
            godMock);
        
        sut.SetDependencies(dependencies);
        
        sut.Configure(new Map2D.Config()
        {
            Resource = mapStopMock.Object,
            InitializationComplete = (complete) => {}
        });
        
        //Act
        sut.DestroySelf();
        
        //Assert
        mapControllerMock.Verify(x => x.DestroySelf());
        godMock.Verify(x => x.Destroy());
    }
    
    [Test]
    public void TestGetCamera_ReturnMapCamController()
    {
        //Arrange
        var camera = new GameObject().AddComponent<Camera>();
        var sut = new GameObject().AddComponent<Map2D>();

        var mapControllerMock = new Mock<IMapCamController>();
        mapControllerMock.Setup(x => x.GetCamera()).Returns(camera).Verifiable();
        var mapCamControllerInstantiatorMock = CreateMapCamController(mapControllerMock);        
        
        var dependencies = CreateDependencies(mapCamControllerInstantiatorMock);
        sut.SetDependencies(dependencies);

        sut.Configure(new Map2D.Config()
        {
            Resource = mapStopMock.Object,
            InitializationComplete = (complete) => {}
        });
        
        //Act
        var theCamera = sut.GetCamera();
        //Assert
        Assert.AreEqual(camera,theCamera);
    }
    
    [Test]
    public void TestCreatePOIForMap_Creates_Configures_And_Sets_the_POI_At_DesiredPosition()
    {
        //Given a new Map2D and a POI.
        //When the SetPOIOnMap is called.
        //Then setLocalPosition is called on the POI, with that desired position.
        
        //Arrange
        var sut = new GameObject().AddComponent<Map2D>();
        
        var map2DHelperMock = new Mock<IMap2DHelper>();
        map2DHelperMock.Setup(x => 
            x.DesiredPositionCalculator(It.IsAny<Map2DPosition>(), It.IsAny<Map2DRectAsset>())).Returns(Vector2.one)
            .Verifiable();
        
        var mapControllerMock = new Mock<IMapCamController>();
        mapControllerMock.Setup(x => x.DestroySelf()).Verifiable();        
        var mapCamControllerInstantiatorMock = CreateMapCamController(mapControllerMock);

        var poiControllerMock = new Mock<IPOIController>();

        var _config = new POIController.Config()
        {
            ClickedAction = null,
            PoiAsset = new POI2DListAsset.Poi2DAsset()
            {
                ResourceId = "resourceId",
                RealWorldPosition = new Map2DPosition(55.55, 77.77),
                ActionId = "start",
                Id = "theId"
            },
            Resource = new POI2DResource((IIcon)null)
            {
                PoiPrefab = new Addressable("address", AddressableTypes.GameObject),
                BackgroundColor = new RHColor(55, 55, 55, 55),
                IconColor = new RHColor(44,44,44,44),
                FrameColor = new RHColor(33,33,33,33),
                PixelsPerUnit = 5,
                ResourceId = "resourceId"
            }
        };

        
        var map2dPoiPlacementMock = new Mock<IMap2DPOIPlacement>();
        map2dPoiPlacementMock.Setup(x=> 
            x.CreateAndConfigure(_config, 1f))
            .ReturnsAsync(poiControllerMock.Object)
            .Verifiable();
        
        var dependencies = CreateDependencies(
            mapCamControllerInstantiatorMock,
            null,
            null,
            null,
            map2DHelperMock,
            map2dPoiPlacementMock);
        
        sut.SetDependencies(dependencies);
        
        poiControllerMock.Setup(x => x.SetLocalPosition(It.IsAny<Vector3>(), dependencies.POILayer)).Verifiable();
        poiControllerMock.Setup(x => x.GetYBounds()).Returns(5f).Verifiable();
        poiControllerMock.Setup(x => x.GetXBounds()).Returns(5f).Verifiable();

        sut.Configure(new Map2D.Config()
        {
            Resource = mapStopMock.Object,
            InitializationComplete = (complete) => {}
        });
        
        var bounds = dependencies.SR.sprite.bounds;
        var expectedPosition = new Vector3(
            (bounds.size.x * 10f) -(5f/2f)-1f, //expected edgePosition calculation based on returned variables from getBounds.
            (bounds.size.y * 10f) -(5f/2f)-1f, 
            sut.transform.localPosition.z);
        
        
        //Act
        sut.CreatePOIForMap(_config);
        
        //Assert
        poiControllerMock.Verify(x => x.SetLocalPosition(expectedPosition, dependencies.POILayer));
        poiControllerMock.Verify(x => x.GetYBounds());
        poiControllerMock.Verify(x => x.GetXBounds());
        map2DHelperMock.Verify(x => 
            x.DesiredPositionCalculator(It.IsAny<Map2DPosition>(), It.IsAny<Map2DRectAsset>()));
        map2dPoiPlacementMock.Verify(x=> 
                x.CreateAndConfigure(It.IsAny<POIController.Config>(), 1f));
    }
}
