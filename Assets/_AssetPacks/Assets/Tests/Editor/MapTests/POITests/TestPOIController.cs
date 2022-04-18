using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products.Assets.Map2DAssets;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.huntProduct;
using riddlehouse_libraries.products.resources;
using riddlehouse_libraries.products.resources.Map;
using riddlehouse_libraries.products.Stops;
using UnityEngine;
using Random = System.Random;

[TestFixture]
public class TestPOIController
{
    private POIController.Dependencies CreateDependencies(
        Mock<IPOIButtonAnimationController> animationController = null, 
        Mock<ISpriteHelper> spriteHelper = null)
    {
        animationController ??= new Mock<IPOIButtonAnimationController>(); 
        spriteHelper ??= new Mock<ISpriteHelper>();

        var frameSR = new GameObject().AddComponent<SpriteRenderer>();
        frameSR.sprite = Sprite.Create(Texture2D.blackTexture, Rect.MinMaxRect(0,0,2,2), Vector2.down);
        return new POIController.Dependencies()
        {
            AnimationController = animationController.Object,
            SpriteHelper = spriteHelper.Object,
            IconSR = new GameObject().AddComponent<SpriteRenderer>(),
            BackgroundSR = new GameObject().AddComponent<SpriteRenderer>(),
            FrameSR = frameSR
        };
    }

    private POIController.Config CreateConfig(POI2DResource resource = null)
    {
        resource ??= _resource;
        return new POIController.Config()
        {
            Resource = resource
        };
    }
    
    private IIcon _iconLink;
    private RHColor _backgroundColor;
    private RHColor _frameColor;
    private RHColor _iconColor;
    
    private Color32 _backgroundColor32;
    private Color32 _frameColor32;
    private Color32 _iconColor32;

    private Sprite _icon;

    private POI2DResource _resource;

    [SetUp]
    public void Init()
    {
        Random rnd = new Random();
        var icon = new Byte[10];
        rnd.NextBytes(icon);

        var iconMock = new Mock<IIcon>();
        iconMock.Setup(x => x.GetIcon(It.IsAny<HuntProductCacheConfig>())).ReturnsAsync(icon);

        _iconLink = iconMock.Object;
        _backgroundColor = new RHColor(44, 44, 44, 44);
        _frameColor = new RHColor(66, 66, 66, 66);
        _iconColor = new RHColor(88, 88, 88, 88);
        
        _backgroundColor32 = new Color32(44,44,44,44);
        _frameColor32 = new Color32(66,66,66,66);
        _iconColor32 = new Color32(88,88,88,88);
        
        _icon = Sprite.Create(
                Texture2D.blackTexture, 
                new Rect(0, 0, Texture2D.blackTexture.width, Texture2D.blackTexture.height), 
                new Vector2(0.5f, 0.5f), 
                5f);
        
        _resource = new POI2DResource(_iconLink)
        {
            IconColor = _iconColor,
            BackgroundColor = _backgroundColor,
            FrameColor = _frameColor,
            PixelsPerUnit = 5,
        };
    }

    [TearDown]
    public void TearDown()
    {
        _icon = null;
        _resource = null;
    }
    
    [Test]
    public void TestSetDependencies()
    {
        //Arrange
        var sut = new GameObject().AddComponent<POIController>();
        var dependencies = CreateDependencies();
        
        //Act
        sut.SetDependencies(dependencies);
        
        //Assert
        Assert.AreEqual(dependencies, sut._dependencies);
    }
        
    [Test]
    public void TestConfigure()
    {
        //Given a new POIController
        //When configure is called with a resource
        //Then the resource is unpacked and passed appropriately to the system. (Colors and Sprite set correctly)
        
        //Arrange
        Random rnd = new Random();
        Byte[] image = new Byte[10];
        rnd.NextBytes(image);
        
        var iconMock = new Mock<IIcon>();
        iconMock
            .Setup(x => 
                x.GetIcon(It.IsAny<HuntProductCacheConfig>()))
            .ReturnsAsync(image)
            .Verifiable();

      
        _resource = new POI2DResource(iconMock.Object)
        {
            IconColor = _iconColor,
            BackgroundColor = _backgroundColor,
            FrameColor = _frameColor,
            PixelsPerUnit = 5,
            // StopId = "theStopId",
            // ActionId = "theAction"
        };
        var spriteHelperMock = new Mock<ISpriteHelper>();
        spriteHelperMock.Setup(x => 
            x.GetSpriteFromByteArray(image, _resource.PixelsPerUnit))
            .Returns(_icon).Verifiable();
            
        var sut = new GameObject().AddComponent<POIController>();
        
        var dependencies = CreateDependencies(null, spriteHelperMock);
        sut.SetDependencies(dependencies);
        var config = CreateConfig(_resource);

        //Act
        sut.Configure(config);
        
        //Assert
        Assert.AreEqual(_icon, dependencies.IconSR.sprite);
        Assert.AreEqual(_icon.pixelsPerUnit, dependencies.IconSR.sprite.pixelsPerUnit);

        //Cast to Color32, since the spriterenderer changes the color class
        Assert.AreEqual(_frameColor32, (Color32)dependencies.FrameSR.color);
        Assert.AreEqual(_backgroundColor32, (Color32)dependencies.BackgroundSR.color);
        Assert.AreEqual(_iconColor32,(Color32)dependencies.IconSR.color);
        iconMock.Verify(x => x.GetIcon(It.IsAny<HuntProductCacheConfig>()));
        spriteHelperMock.Verify(x => 
                x.GetSpriteFromByteArray(image, _resource.PixelsPerUnit));
    }
    
    [Test]
    public void TestGetXBounds_Returns_Sprite_SizeX()
    {
        //Given a configured POIController
        //When GetXBounds is called
        //Then the function returns the horizontal size of the frame-sprite
        
        //Arrange
        var sut = new GameObject().AddComponent<POIController>();

        var dependencies = CreateDependencies(null);
        sut.SetDependencies(dependencies);
        var config = CreateConfig(_resource);
        sut.Configure(config);

        //Act
        var xBoundary = sut.GetXBounds();
        //Assert
        Assert.AreEqual(dependencies.FrameSR.bounds.size.x, xBoundary);
    }
    
        
    [Test]
    public void TestGetYBounds_Returns_Sprite_SizeY()
    {
        //Given a configured POIController
        //When GetYBounds is called
        //Then the function returns the vertical size of the frame-sprite
        
        //Arrange
        var sut = new GameObject().AddComponent<POIController>();
        
        var dependencies = CreateDependencies(null);
        sut.SetDependencies(dependencies);
        var config = CreateConfig(_resource);
        sut.Configure(config);

        //Act
        var yBoundary = sut.GetYBounds();
        //Assert
        Assert.AreEqual(dependencies.FrameSR.bounds.size.y, yBoundary);
    }
    
    [Test]
    public void TestGetRealWorldPosition_ReturnsConfigured_RWPosition()
    {
        //Given a configured POIController
        //When GetRealWorldPosition is called
        //Then the function return the resource parameter (x=double.max, y=double.min)
        
        //Arrange
        var poiAsset = new POI2DListAsset.Poi2DAsset()
        {
            RealWorldPosition = new Map2DPosition(double.MaxValue, double.MinValue)
        };

        var sut = new GameObject().AddComponent<POIController>();
        
        var dependencies = CreateDependencies(null);
        sut.SetDependencies(dependencies);
        var config = CreateConfig(_resource);
        config.PoiAsset = poiAsset;
        sut.Configure(config);

        //Act
        var pos = sut.GetRealWorldPosition();
        //Assert
        Assert.AreEqual(poiAsset.RealWorldPosition, pos);
    }
    
    [Test]
    public void TestSetLocalPosition()
    {
        //Given a configured POIController
        //When SetLocalPosition is called
        //Then the object is parented to the parsed transform, and the position is updated locally.
        
        //Arrange
        var parent = new GameObject();
        var poiAsset = new POI2DListAsset.Poi2DAsset()
        {
            RealWorldPosition = new Map2DPosition(double.MaxValue, double.MinValue)
        };
        
        var sut = new GameObject().AddComponent<POIController>();
        
        var dependencies = CreateDependencies(null);
        sut.SetDependencies(dependencies);
        var config = CreateConfig(_resource);
        config.PoiAsset = poiAsset;
        sut.Configure(config);

        //Act
        sut.SetLocalPosition(Vector3.left, parent.transform);
        
        //Assert
        Assert.AreEqual(Vector3.left, sut.transform.localPosition);
        Assert.AreEqual(parent.transform, sut.transform.parent);
    }
    
    [Test]
    public void TestClickedAction_POIEnabled_TriggersAnimation_TriggersConfiguredAction()
    {
        //Given a configured POIController
        //When the poi is clicked while the POI is enabled.
        //Then the animationcontroller clickAnimation is triggered And the configured action is invoked.
        
        //Arrange
        var poiId = "stopID";
        var poiAsset = new POI2DListAsset.Poi2DAsset()
        {
            RealWorldPosition = new Map2DPosition(double.MaxValue, double.MinValue),
            Id = poiId
        };
        
        var sut = new GameObject().AddComponent<POIController>();

        var animationController = new Mock<IPOIButtonAnimationController>();
        animationController.Setup(x => x.ClickAnimation()).Verifiable();
        animationController.Setup(x => x.IsDisabled()).Returns(false).Verifiable();
        var dependencies = CreateDependencies(animationController);

        sut.SetDependencies(dependencies);

        var config = CreateConfig();
        bool hasBeenCalled = false;
        string clickedId = null;
        Action<string> clickedAction = (id) => { 
            hasBeenCalled = true;
            clickedId = id;
        };
        config.ClickedAction = clickedAction;
        config.PoiAsset = poiAsset;
        sut.Configure(config);
        
        //Act
        sut.ClickedAction();
        
        //Assert
        Assert.IsTrue(hasBeenCalled);
        Assert.AreEqual(poiId,clickedId);
        animationController.Verify(x => x.ClickAnimation());
        animationController.Verify(x => x.IsDisabled());
    }
    
    [Test]
    public void TestClickedAction_POIDisabled_NoAnim_NoConfigInvoke()
    {
        //Given a configured POIController
        //When the poi is clicked while the POI is disabled.
        //Then the poi does nothing.
        
        //Arrange
        var sut = new GameObject().AddComponent<POIController>();

        var animationController = new Mock<IPOIButtonAnimationController>();
        animationController.Setup(x => x.ClickAnimation()).Verifiable();
        animationController.Setup(x => x.IsDisabled()).Returns(true).Verifiable();
        var dependencies = CreateDependencies(animationController);

        sut.SetDependencies(dependencies);

        var config = CreateConfig();
        bool hasBeenCalled = false;
        Action<string> clickedAction = (id) => { hasBeenCalled = true; };
        config.ClickedAction = clickedAction;
        sut.Configure(config);
        
        //Act
        sut.ClickedAction();
        
        //Assert
        Assert.IsFalse(hasBeenCalled);
        
        animationController.Verify(x => x.ClickAnimation(), Times.Never);
        animationController.Verify(x => x.IsDisabled());
    }
    [Test]
    public void TestSetLocalScale_ChangesScaleFrom_1_to_3()
    {
        //Given a POIController and a desired scale
        //When SetLocalScale is called with that angle
        //Then the POIcontroller is scaled to that size.

        //Arrange
        var sut = new GameObject().AddComponent<POIController>();
        sut.transform.localScale = Vector3.one;
        var dependencies = CreateDependencies();
        sut.SetDependencies(dependencies);

        //Act
        sut.SetLocalScale(Vector3.one*3);
        
        //Assert
        Assert.AreEqual(Vector3.one*3, sut.transform.localScale);
    }

    [Test]
    public void TestSetLocalRotation_ChangeRotationFrom_0_to_90()
    {
        //Given a POIController and a desired angle
        //When SetLocalRotation is called with that angle
        //Then the POIcontroller is rotated to that angle.
        //-Note. It is not recommended to use Euler transformations, as this can lead to gimbal locks.
        //-- we chose the simpler solution to avoid complicated math, in a case where gimbal locks shouldn't happen.
        
        //Arrange
        var sut = new GameObject().AddComponent<POIController>();
        sut.transform.localRotation = Quaternion.Euler(0, 0, 0);
        var dependencies = CreateDependencies();
        sut.SetDependencies(dependencies);

        //Act
        sut.SetLocalRotation(new Vector3(0,0,90));
        
        //Assert
        Assert.AreEqual(Quaternion.Euler(0, 0, 90).eulerAngles, sut.transform.localRotation.eulerAngles);
    }
    
    [TestCase(PoiStates.Idle)]
    [TestCase(PoiStates.Idle)]
    [TestCase(PoiStates.Disabled)]
    [TestCase(PoiStates.Completed)]
    [Test]
    public void TestUpdatePOIState_SetsIdleState_In_Animator(PoiStates stateToTest)
    {
        //Given a POIController
        //When UpdatePOIState is called with a state
        //Then the required state is set in the animator.
        
        //Arrange
        var sut = new GameObject().AddComponent<POIController>();
        sut.gameObject.SetActive(false);
        var animationController = new Mock<IPOIButtonAnimationController>();
        animationController.Setup(x => x.SetIdle()).Verifiable();
        animationController.Setup(x => x.SetHighlighted()).Verifiable();
        animationController.Setup(x => x.SetDisabled()).Verifiable();
        animationController.Setup(x => x.SetCompleted()).Verifiable();
        
        var dependencies = CreateDependencies(animationController);
        sut.SetDependencies(dependencies);
        
        //Act
        sut.UpdatePOIState(stateToTest);
        
        //Assert
        AssertAnimatorTestCase(animationController, stateToTest);
        Assert.IsTrue(sut.gameObject.activeSelf);
    }
    
    private void AssertAnimatorTestCase(Mock<IPOIButtonAnimationController> animationController, PoiStates state)
    {
        switch (state)
        {
            case PoiStates.Idle:
                animationController.Verify(x => x.SetIdle());
                animationController.Verify(x => x.SetHighlighted(), Times.Never);
                animationController.Verify(x => x.SetDisabled(), Times.Never);
                animationController.Verify(x => x.SetCompleted(), Times.Never);
                break;
            case PoiStates.Highlighted:
                animationController.Verify(x => x.SetHighlighted());
                animationController.Verify(x => x.SetIdle(), Times.Never);
                animationController.Verify(x => x.SetDisabled(), Times.Never);
                animationController.Verify(x => x.SetCompleted(), Times.Never);
                break;
            case PoiStates.Disabled:
                animationController.Verify(x => x.SetDisabled());
                animationController.Verify(x => x.SetIdle(), Times.Never);
                animationController.Verify(x => x.SetHighlighted(), Times.Never);
                animationController.Verify(x => x.SetCompleted(), Times.Never);
                break;
            case PoiStates.Completed:
                animationController.Verify(x => x.SetCompleted());
                animationController.Verify(x => x.SetIdle(), Times.Never);
                animationController.Verify(x => x.SetHighlighted(), Times.Never);
                animationController.Verify(x => x.SetDisabled(), Times.Never);
                break;
            default:
                throw new ArgumentException("No such case defined");
        }
    }
    
    [Test]
    public void TestUpdatePOIState_SetsHiddenState_In_Animator()
    {
        //Given a POIController
        //When UpdatePOIState is called with state hidden
        //Then the gameObject is deactivated.

        //Arrange
        var sut = new GameObject().AddComponent<POIController>();
        
        var animationController = new Mock<IPOIButtonAnimationController>();

        var dependencies = CreateDependencies(animationController);
        sut.SetDependencies(dependencies);
        
        //Act
        sut.UpdatePOIState(PoiStates.Hidden);
        
        //Assert
        Assert.IsFalse(sut.gameObject.activeSelf);
    }
}
