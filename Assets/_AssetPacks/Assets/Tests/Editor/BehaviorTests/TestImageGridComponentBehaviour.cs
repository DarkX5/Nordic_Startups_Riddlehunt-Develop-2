using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class TestImageGridComponentBehaviour
{
    private GameObject _prefab;
    private RectTransform _prefabTransform;
    private RectTransform _parent;

    private Sprite _img1;
    private Sprite _img2;
    private Sprite _img3;

    private List<Sprite> _imageList;

    private Mock<IImageFullScreenCanvasActions> ImageFullscreenCanvasActionsMock;
    
    private Mock<IImageDisplayActions> _imgDisplayMock1;
    private Mock<IImageDisplayActions> _imgDisplayMock2;
    private Mock<IImageDisplayActions> _imgDisplayMock3;

    private List<IImageDisplayActions> AvailableImageMocks;
    [SetUp]
    public void Init()
    {
        _prefab = new GameObject();
        var parentObj = new GameObject();
        _prefabTransform = _prefab.AddComponent<RectTransform>();
        
        _parent = parentObj.AddComponent<RectTransform>();
        
        _img1 = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        _img2 = Sprite.Create(Texture2D.normalTexture, Rect.zero, Vector2.up);
        _img3 = Sprite.Create(Texture2D.redTexture, Rect.zero, Vector2.left);

        _imageList = new List<Sprite>() { _img1, _img2, _img3 };

        ImageFullscreenCanvasActionsMock = new Mock<IImageFullScreenCanvasActions>();

        _imgDisplayMock1 = new Mock<IImageDisplayActions>();
        _imgDisplayMock1.Setup(x => x.Configure(_img1, It.IsAny<Action<Sprite>>())).Verifiable();
        
        _imgDisplayMock2 = new Mock<IImageDisplayActions>();
        _imgDisplayMock2.Setup(x => x.Configure(_img2, It.IsAny<Action<Sprite>>())).Verifiable();
        
        _imgDisplayMock3 = new Mock<IImageDisplayActions>();
        _imgDisplayMock3.Setup(x => x.Configure(_img3, It.IsAny<Action<Sprite>>())).Verifiable();

        AvailableImageMocks = new List<IImageDisplayActions>() { _imgDisplayMock1.Object, _imgDisplayMock2.Object, _imgDisplayMock3.Object };
    }

    [TearDown]
    public void TearDown()
    {
        _prefab = null;
        _parent = null;

        _img1 = null;
        _img2 = null;
        _img3 = null;
        
        _imageList = null;

        ImageFullscreenCanvasActionsMock = null;
    }
    [Test]
    public void TestFactory_Has_ImageGridComponentBehaviour_Succeeds()
    {
        //Given a prefab that has an ImageGridComponentBehaviour connected
        //When the factory is called with that prefab object
        //Then the factory returns a newly created ImageGridComponentBehaviour
        
        //Arrange
        _prefab.AddComponent<ImageGridComponentBehaviour>();
        //Act & Assert
        Assert.DoesNotThrow(() => ImageGridComponentBehaviour.Factory(_prefab, _parent));
    }

    [Test]
    public void TestFactory_DoesNotHave_ImageGridComponentBehaviour_Throws()
    {
        //Given a prefab that has an ImageGridComponentBehaviour connected
        //When the factory is called with that prefab object
        //Then the factory returns a newly created ImageGridComponentBehaviour
        
        //Act & Assert
        Assert.Throws<ArgumentException>(() => ImageGridComponentBehaviour.Factory(_prefab, _parent));
    }
    
    [Test] 
    public void TestConfigure_ConfiguresImageList_PreConfigured_With_2_imageDisplays_Needs_3_Expands()
    {
        //Arrange
        var sut = _prefab.AddComponent<ImageGridComponentBehaviour>();
        var imageDisplayComponent = (new GameObject()).AddComponent<ImageDisplayComponentBehaviour>();
        _imageList = new List<Sprite>() { _img1, _img2 };
        var imageDisplayComponentHelperMock = new Mock<IImageDisplayComponentHelper>();
        sut.SetDependencies(AvailableImageMocks, ImageFullscreenCanvasActionsMock.Object, imageDisplayComponent, imageDisplayComponentHelperMock.Object, _prefabTransform);
        
        //Act
        sut.Configure(_imageList);
        
        //Assert
        _imgDisplayMock1.Verify(x => x.Configure(_img1, It.IsAny<Action<Sprite>>()));
        _imgDisplayMock2.Verify(x => x.Configure(_img2, It.IsAny<Action<Sprite>>()));
        _imgDisplayMock3.Verify(x => x.Configure(_img3, It.IsAny<Action<Sprite>>()), Times.Never());
        Assert.AreEqual(_imageList.Count+1, sut.AvailableImages.Count);
    }

    [Test] 
    public void TestConfigure_ConfiguresImageList_WithMocks_calls_Configure_In_Mock()
    {
        //Arrange
        var sut = _prefab.AddComponent<ImageGridComponentBehaviour>();
        var imageDisplayComponent = (new GameObject()).AddComponent<ImageDisplayComponentBehaviour>();
        var imageDisplayComponentHelperMock = new Mock<IImageDisplayComponentHelper>();
        sut.SetDependencies(AvailableImageMocks, ImageFullscreenCanvasActionsMock.Object, imageDisplayComponent, imageDisplayComponentHelperMock.Object, _prefabTransform);
        
        //Act
        sut.Configure(_imageList);
        
        //Assert
        _imgDisplayMock1.Verify(x => x.Configure(_img1, It.IsAny<Action<Sprite>>()));
        _imgDisplayMock2.Verify(x => x.Configure(_img2, It.IsAny<Action<Sprite>>()));
        _imgDisplayMock3.Verify(x => x.Configure(_img3, It.IsAny<Action<Sprite>>()));
    }
    
    
    [Test] 
    public void TestConfigure_ConfiguresImageList_CreatesNew_ImageDisplays()
    {
        //Arrange
        var sut = _prefab.AddComponent<ImageGridComponentBehaviour>();
        var imageDisplayComponent = (new GameObject()).AddComponent<ImageDisplayComponentBehaviour>();
        var imageDisplayComponentHelperMock = new Mock<IImageDisplayComponentHelper>();
        imageDisplayComponentHelperMock.Setup(x =>
            x.CreateImageDisplayActions(It.IsAny<GameObject>(), It.IsAny<RectTransform>())).Returns(_imgDisplayMock3.Object);
        sut.SetDependencies(new List<IImageDisplayActions>(), ImageFullscreenCanvasActionsMock.Object, imageDisplayComponent, imageDisplayComponentHelperMock.Object, _prefabTransform);
        
        //Act
        sut.Configure(_imageList);
        
        //Assert
        Assert.AreEqual(_imageList.Count, sut.AvailableImages.Count);
    }
}
