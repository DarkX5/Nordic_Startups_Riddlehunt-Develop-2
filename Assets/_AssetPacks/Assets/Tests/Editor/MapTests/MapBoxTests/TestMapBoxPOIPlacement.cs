using Mapbox.Unity.Map;
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
public class TestMapBoxPOIPlacement
{
    private POIController.Config _configA;
    private POIController.Config _configB;

    [SetUp]
    public void Init()
    {
        _configA = new POIController.Config()
        {
            ClickedAction = null,
            PoiAsset = new POI2DListAsset.Poi2DAsset()
            {
                ResourceId = "resourceId",
                RealWorldPosition = new Map2DPosition(55.55, 77.77),
                ActionId = "start",
                Id = "theIdA"
            },
            Resource = new POI2DResource((IIcon)null)
            {
                PoiPrefab = new Addressable("addressA", AddressableTypes.GameObject),
                BackgroundColor = new RHColor(55, 55, 55, 55),
                IconColor = new RHColor(44,44,44,44),
                FrameColor = new RHColor(33,33,33,33),
                PixelsPerUnit = 5,
                ResourceId = "resourceId"
            }
        };
        _configB = new POIController.Config()
        {
            ClickedAction = null,
            PoiAsset = new POI2DListAsset.Poi2DAsset()
            {
                ResourceId = "resourceId",
                RealWorldPosition = new Map2DPosition(55.55, 77.77),
                ActionId = "start",
                Id = "theIdB"
            },
            Resource = new POI2DResource((IIcon)null)
            {
                PoiPrefab = new Addressable("addressB", AddressableTypes.GameObject),
                BackgroundColor = new RHColor(55, 55, 55, 55),
                IconColor = new RHColor(44,44,44,44),
                FrameColor = new RHColor(33,33,33,33),
                PixelsPerUnit = 5,
                ResourceId = "resourceId"
            }
        };
    }

    [TearDown]
    public void TearDown()
    {
        _configA = null;
        _configB = null;
    }
    
    [Test]
    public void TestPlaceMapBoxPoi_Creates_And_Positions_POI()
    {
        //Given a poi, a scale and a coordinate
        //When PlaceMapBoxPoi is called
        //Then the poi is placed, scaled and rotated for use on the MapBox viewer.
        //-note that we can't test/assert against the Abstract map against real locations, since it needs to be 'initialized' and at runtime.
        
        //Arrange
        var map = new GameObject().AddComponent<AbstractMap>();
        var parent = new GameObject();
        
        var poiMock = new Mock<IPOIController>();
        
        var poiInstantiatorMock = new Mock<IPOIInstantiator>();
        poiInstantiatorMock.Setup(x => x.Create(_configA.Resource.PoiPrefab)).ReturnsAsync(poiMock.Object).Verifiable();
        var sut = new MapBoxPOIPlacement(poiInstantiatorMock.Object, map, parent.transform);

        poiMock.Setup(x => x.SetLocalPosition(Vector3.up, parent.transform)).Verifiable();
        poiMock.Setup(x => x.SetLocalScale(Vector3.one)).Verifiable();
        poiMock.Setup(x => x.SetLocalRotation(new Vector3(90,0,0))).Verifiable();
        
        //Act
        sut.CreateAndConfigure(_configA, 1f);
        
        //Assert
        poiMock.Verify(x => x.SetLocalPosition(Vector3.up, parent.transform));
        poiMock.Verify(x => x.SetLocalScale(Vector3.one));
        poiMock.Verify(x => x.SetLocalRotation(new Vector3(90,0,0)));
        
        poiInstantiatorMock.Verify(x => x.Create(_configA.Resource.PoiPrefab));
    }
    
    [Test]
    public void TestPlaceMapBoxPoi_No_Duplicate_Creation()
    {
        //Given a poi, a scale and a coordinate
        //When PlaceMapBoxPoi is called, but it already has an instance with that location
        //Then function returns before positioning the poi.
        
        //Arrange
        var map = new GameObject().AddComponent<AbstractMap>();
        var poiMockA = new Mock<IPOIController>();
        poiMockA.Setup(x => x.SetLocalPosition(Vector3.up, null)).Verifiable();
        
        var poiInstantiatorMock = new Mock<IPOIInstantiator>();
        poiInstantiatorMock.Setup(x => x.Create(_configA.Resource.PoiPrefab)).ReturnsAsync(poiMockA.Object).Verifiable();

        var sut = new MapBoxPOIPlacement(poiInstantiatorMock.Object, map, null);
      
        //Act
        sut.CreateAndConfigure(_configA,1f);
        sut.CreateAndConfigure(_configA,1f);

        //Assert
        poiMockA.Verify(x => x.SetLocalPosition(Vector3.up, null), Times.Exactly(1));
    }
    
    [Test]
    public void TestUpdatePoiPositions_UpdatesThePOIPositions()
    {
        //Given MapBoxPOIPlacement with a list of POI's
        //When UpdatePoiPositions is called
        //Then all existing POI's have their localPositions updated.
        
        //Arrange
        var map = new GameObject().AddComponent<AbstractMap>();
        var parent = new GameObject();

        var poiMockA = new Mock<IPOIController>();
        poiMockA.Setup(x => x.SetLocalPosition(Vector3.up, parent.transform)).Verifiable();

        var poiMockB = new Mock<IPOIController>();
        poiMockB.Setup(x => x.SetLocalPosition(Vector3.up,  parent.transform)).Verifiable();
        
        poiMockA.Setup(x => x.SetLocalPosition(Vector3.up)).Verifiable();
        poiMockB.Setup(x => x.SetLocalPosition(Vector3.up)).Verifiable();

        var poiInstantiatorMock = new Mock<IPOIInstantiator>();
        poiInstantiatorMock.Setup(x => x.Create(_configA.Resource.PoiPrefab)).ReturnsAsync(poiMockA.Object).Verifiable();
        poiInstantiatorMock.Setup(x => x.Create(_configB.Resource.PoiPrefab)).ReturnsAsync(poiMockB.Object).Verifiable();

        
        var sut = new MapBoxPOIPlacement(poiInstantiatorMock.Object, map, parent.transform);
        
        sut.CreateAndConfigure(_configA,1f);
        sut.CreateAndConfigure(_configB,1f);

        //Act
        sut.UpdatePoiPositions();
        
        //Assert
        poiMockA.Verify(x => x.SetLocalPosition(Vector3.up, parent.transform), Times.Exactly(1));
        poiMockB.Verify(x => x.SetLocalPosition(Vector3.up, parent.transform), Times.Exactly(1));
        poiMockA.Verify(x => x.SetLocalPosition(Vector3.up), Times.Exactly(1));
        poiMockB.Verify(x => x.SetLocalPosition(Vector3.up), Times.Exactly(1));
    }

    [Test]
    public void TestDestroyAllPois()
    {
        var map = new GameObject().AddComponent<AbstractMap>();
        
        var poiMockA = new Mock<IPOIController>();
        var poiMockB = new Mock<IPOIController>();
        poiMockA.Setup(x => x.DestroySelf()).Verifiable();
        poiMockB.Setup(x => x.DestroySelf()).Verifiable();
        
        var poiInstantiatorMock = new Mock<IPOIInstantiator>();
        poiInstantiatorMock.Setup(x => x.Create(_configA.Resource.PoiPrefab)).ReturnsAsync(poiMockA.Object).Verifiable();
        poiInstantiatorMock.Setup(x => x.Create(_configB.Resource.PoiPrefab)).ReturnsAsync(poiMockB.Object).Verifiable();

        var sut = new MapBoxPOIPlacement(poiInstantiatorMock.Object, map, null);

        sut.CreateAndConfigure(_configA,1f);
        sut.CreateAndConfigure(_configB,1f);
        
        //Act
        sut.DestroyAllPois();
        
        poiMockA.Verify(x => x.DestroySelf());
        poiMockB.Verify(x => x.DestroySelf());
    }
    
    [Test]
    public void TestUpdatePOIState_Updates_State()
    {
        //Given a MapBoxPOIPlacement with two POI's
        //When UpdatePOIState is called with the ID of one of those
        //Then the function call is propagated to the right instance.
        
        var map = new GameObject().AddComponent<AbstractMap>();
        var parent = new GameObject();
        
        var poiMockA = new Mock<IPOIController>();
        var poiMockB = new Mock<IPOIController>();
        poiMockA.Setup(x => x.UpdatePOIState(PoiStates.Idle)).Verifiable();
        poiMockB.Setup(x => x.UpdatePOIState(It.IsAny<PoiStates>())).Verifiable();
        
        var poiInstantiatorMock = new Mock<IPOIInstantiator>();
        poiInstantiatorMock.Setup(x => x.Create(_configA.Resource.PoiPrefab)).ReturnsAsync(poiMockA.Object).Verifiable();
        poiInstantiatorMock.Setup(x => x.Create(_configB.Resource.PoiPrefab)).ReturnsAsync(poiMockB.Object).Verifiable();
        
        var sut = new MapBoxPOIPlacement(poiInstantiatorMock.Object, map, parent.transform);
        
        sut.CreateAndConfigure(_configA,1f);
        sut.CreateAndConfigure(_configB,1f);

        //Act
        sut.UpdatePOIState("theIdA", PoiStates.Idle);
        
        //Assert
        poiMockA.Verify(x => x.UpdatePOIState(PoiStates.Idle));
        poiMockB.Verify(x => x.UpdatePOIState(It.IsAny<PoiStates>()), Times.Never());
    }
    
        
    [Test]
    public void TestUpdatePOIState_No_Such_POI()
    {
        //Given a MapBoxPOIPlacement with no POI's
        //When UpdatePOIState is called with an ID
        //Then the function call will abort.
        
        var map = new GameObject().AddComponent<AbstractMap>();
        var parent = new GameObject();
        var poiInstantiatorMock = new Mock<IPOIInstantiator>();

        var sut = new MapBoxPOIPlacement(poiInstantiatorMock.Object, map, parent.transform);
        //Act && Assert
        Assert.DoesNotThrow( () => sut.UpdatePOIState("A", PoiStates.Idle));
    }
}
