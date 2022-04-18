using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products.Assets.Adressables;
using riddlehouse_libraries.products.Assets.Map2DAssets;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.resources;
using riddlehouse_libraries.products.resources.Map;

public class TestMap2DPOIPlacement 
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
                IconColor = new RHColor(44, 44, 44, 44),
                FrameColor = new RHColor(33, 33, 33, 33),
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
    public async void TestCreateAndConfigure_EmptySpawnedList_InstantiatesAndConfigures()
    {
        //Given a Map2DPoiPlacement instance
        //When CreateAndConfigure is called with an addressable
        //Then the prefab is collected, instantiated, configured and returned.
        
        //Arrange
        var poiMock = new Mock<IPOIController>();
        poiMock.Setup(x=> x.Configure(_configA)).Verifiable();
        var poiInstantiatorMock = new Mock<IPOIInstantiator>();
        poiInstantiatorMock.Setup(x => x.Create(_configA.Resource.PoiPrefab)).ReturnsAsync(poiMock.Object).Verifiable();
        var sut = new Map2DPOIPlacement(poiInstantiatorMock.Object);

        //Act
        var poi = await sut.CreateAndConfigure(_configA, 1f);
        
        //Assert
        Assert.AreEqual(poiMock.Object, poi);
        poiInstantiatorMock.Verify(x => x.Create(_configA.Resource.PoiPrefab));
        poiMock.Verify(x=> x.Configure(_configA));
    }
    
    [Test]
    public async void TestCreateAndConfigure_InstanceAlreadyExists_ReturnsPreviouslyConfiguredInstance()
    {
        //Given a Map2DPoiPlacement instance with 1 poicontroller connected
        //When CreateAndConfigure is called with the same poiAsset ID
        //Then the original instance is returned and nothing new is created.
        
        //Arrange
        var poiMock = new Mock<IPOIController>();
        poiMock.Setup(x=> x.Configure(_configA)).Verifiable();
        var poiInstantiatorMock = new Mock<IPOIInstantiator>();
        poiInstantiatorMock.Setup(x => x.Create(_configA.Resource.PoiPrefab)).ReturnsAsync(poiMock.Object).Verifiable();
        var sut = new Map2DPOIPlacement(poiInstantiatorMock.Object);

        var poiA = await sut.CreateAndConfigure(_configA, 1f);
        
        //Act
        var poiB = await sut.CreateAndConfigure(_configA, 1f);
        
        //Assert
        Assert.AreEqual(poiMock.Object, poiB);
        Assert.AreEqual(poiA, poiB);

        poiInstantiatorMock.Verify(x => x.Create(_configA.Resource.PoiPrefab), Times.Once);
        poiMock.Verify(x=> x.Configure(_configA), Times.Once);
    }
    
    [TestCase(PoiStates.Idle)]
    [TestCase(PoiStates.Completed)]
    [TestCase(PoiStates.Hidden)]
    [TestCase(PoiStates.Highlighted)]
    [TestCase(PoiStates.Disabled)]
    [Test]
    public async void TestUpdatePOIState_Updates_State_With_GivenState(PoiStates newState)
    {
        //Given a Map2DPoiPlacement instance with two poi's in it.
        //When UpdatePOIState is called with an id and a state.
        //Then the given POIController has its state updated.
        
        //Arrange
        var poiMockA = new Mock<IPOIController>();
        poiMockA.Setup(x=> x.Configure(_configA)).Verifiable();
        var poiMockB = new Mock<IPOIController>();
        poiMockB.Setup(x=> x.Configure(_configB)).Verifiable();
        var poiInstantiatorMock = new Mock<IPOIInstantiator>();
        poiInstantiatorMock.Setup(x => x.Create(_configA.Resource.PoiPrefab)).ReturnsAsync(poiMockA.Object).Verifiable();
        poiInstantiatorMock.Setup(x => x.Create(_configB.Resource.PoiPrefab)).ReturnsAsync(poiMockB.Object).Verifiable();
        
        var sut = new Map2DPOIPlacement(poiInstantiatorMock.Object);
        await sut.CreateAndConfigure(_configA, 1f);
        await sut.CreateAndConfigure(_configB, 1f);

        //Act
        sut.UpdatePOIState(_configA.PoiAsset.Id, newState);
        
        //Assert
        poiMockA.Verify(x=> x.UpdatePOIState(newState));
        poiMockB.Verify(x=> x.UpdatePOIState(newState), Times.Never);
    }
    
    [Test]
    public async void TestDestroyAllPois_Destroys_All_POIs()
    {
        //Given a Map2DPoiPlacement instance with two poi's in it.
        //When DestroyAllPois is called with an id and a state.
        //Then the destroySelf is called in the existing Poi's
        
        //Arrange
        var poiMockA = new Mock<IPOIController>();
        poiMockA.Setup(x=> x.Configure(_configA)).Verifiable();
        var poiMockB = new Mock<IPOIController>();
        poiMockB.Setup(x=> x.Configure(_configB)).Verifiable();
        var poiInstantiatorMock = new Mock<IPOIInstantiator>();
        poiInstantiatorMock.Setup(x => x.Create(_configA.Resource.PoiPrefab)).ReturnsAsync(poiMockA.Object).Verifiable();
        poiInstantiatorMock.Setup(x => x.Create(_configB.Resource.PoiPrefab)).ReturnsAsync(poiMockB.Object).Verifiable();
        
        var sut = new Map2DPOIPlacement(poiInstantiatorMock.Object);
        await sut.CreateAndConfigure(_configA, 1f);
        await sut.CreateAndConfigure(_configB, 1f);

        //Act
        sut.DestroyAllPois();
        
        //Assert
        poiMockA.Verify(x=> x.DestroySelf());
        poiMockB.Verify(x=> x.DestroySelf());
    }
}
