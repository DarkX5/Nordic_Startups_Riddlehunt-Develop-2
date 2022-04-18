using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hunt;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using riddlehouse.video;
using riddlehouse_libraries.products;
using riddlehouse_libraries.products.models;
using riddlehouse_libraries.products.models.DTOs;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Zenject;

[TestFixture]
public class TestProductCard
{
    private ProductCardBehavior.Dependencies _dependencies;

    private ProductCardBehavior.Dependencies CreateDependencies(
        Mock<IHuntInstantiater> huntInstantiaterMock = null, 
        Mock<IImageGetter> imageGetterMock = null, 
        Mock<IProductCardStartPanelDataGetter> productcardStartPanelDataGetter = null,
        Mock<IWaitToExecuteAction> waitToExecuteActionMock = null,
        Mock<IExecuteActionAtEndOfFixedFrame> executeActionAtEndOfFixedFrameMock = null,
        Mock<ILoadingView> loadingView = null
        )
    {
        huntInstantiaterMock ??= new Mock<IHuntInstantiater>();
        imageGetterMock ??= new Mock<IImageGetter>();
        productcardStartPanelDataGetter ??= new Mock<IProductCardStartPanelDataGetter>();
        waitToExecuteActionMock ??= new Mock<IWaitToExecuteAction>();
        executeActionAtEndOfFixedFrameMock ??= new Mock<IExecuteActionAtEndOfFixedFrame>();
        loadingView ??= new Mock<ILoadingView>();
        var titleObj = new GameObject();
        var descriptionObj = new GameObject();
        var dependencies = new ProductCardBehavior.Dependencies()
        {
            HuntInstantiater = huntInstantiaterMock.Object,
            ImageGetter = imageGetterMock.Object,
            ProductEvents = null,
            ProductCardStartPanelDataGetter = productcardStartPanelDataGetter.Object,
            Title = titleObj.AddComponent<TextMeshProUGUI>(),
            TitleTransform = titleObj.GetComponent<RectTransform>(),
            Description = descriptionObj.AddComponent<TextMeshProUGUI>(),
            DescriptionTransform = descriptionObj.GetComponent<RectTransform>(),
            PlayBtnTransform = new GameObject().AddComponent<RectTransform>(),
            BannerImage = new GameObject().AddComponent<Image>(),
            CardLayoutElement = new GameObject().AddComponent<LayoutElement>(),
            BodyLayoutElement = new GameObject().AddComponent<LayoutElement>(),
            LayoutContentOwner = new GameObject().AddComponent<RectTransform>(),
            BannerRect = new GameObject().AddComponent<RectTransform>(),
            WaitToExecuteAction = waitToExecuteActionMock.Object,
            ExecuteActionAtEndOfFixedFrame = executeActionAtEndOfFixedFrameMock.Object,
            LoaderView = loadingView.Object
        };
        return dependencies;
    }
    
    [SetUp]
    public void Init()
    {
        _dependencies = CreateDependencies();
    }

    [TearDown]
    public void TearDown()
    {
        _dependencies = null;
    }

    [Test]
    public void TestSetDependencies_Dependencies_Object_Is_Set()
    {
        //Given a new ProductCard and a set of dependencies
        //When SetDependencies is called
        //Then ProductCard.Dependencies is set with those variables.

        //Arrange
        var sut = new GameObject().AddComponent<ProductCardBehavior>();
        //Act
        sut.SetDependencies(_dependencies);
        //Assert
        Assert.AreEqual(_dependencies, sut._dependencies);
    }
    
    [Test]
    public void TestSetParent_IsSetAndScaledToSame()
    {
        // Given the the parent object is scaled differently
        // When SetParent is called
        // Then the card is set to be child of parent and scaled 1:1
        GameObject parent = new GameObject();
        GameObject child = new GameObject();
        child.transform.localScale = new Vector3(2, 2, 2);
        
        var sut = child.AddComponent<ProductCardBehavior>();
        sut.SetDependencies(new ProductCardBehavior.Dependencies()
        {
            ImageGetter = null, 
            ProductEvents = null,
            ProductCardStartPanelDataGetter = null
        });
        sut.SetParent(parent.transform);
        Assert.AreEqual(parent.transform, child.transform.parent);
        Assert.AreEqual(new Vector3(1, 1, 1), child.transform.localScale);
    }

    [Test]
    public void TestConfigure_CollectsImageFromLink_ConfiguresProductCard_And_CallsCardReadyAtion()
    {
        // Given the user is logged in and is generating a productList
        // When the productCard is configured
        // Then system configures the productCard and calles the cardReady action
        
        //Setup
        var executeActionAtEndOfFixedFrameMock = new Mock<IExecuteActionAtEndOfFixedFrame>();

        Sprite sprite = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        var cardData = new ProductDTO
            {Title = "product A", Description = "Description", BackgroundPictureUrl = "https://Img.com"};
        
        var ImageGetterMock = new Mock<IImageGetter>();
        ImageGetterMock
            .Setup(x => x.GetImage(
                cardData.BackgroundPictureUrl, 
                false, 
                It.IsAny<Action<Sprite>>())
            )
            .Callback<string, bool, Action<Sprite>>((link, cache, collectedSprite) =>
            {
                collectedSprite.Invoke(sprite);
            }).Verifiable();
        var hasBeenCalled = false;
        var actionFunc = new Action(() => { hasBeenCalled = true; });
        
        var descriptionExcerpt = 150 > cardData.Description.Length
            ? cardData.Description
            : cardData.Description.Substring(0, 150) + " (...)";

        //Act
        var sut = new GameObject().AddComponent<ProductCardBehavior>();
        sut.SetDependencies(CreateDependencies(
            null, 
            ImageGetterMock, 
            null, 
            null, 
            executeActionAtEndOfFixedFrameMock));               
        sut.Configure(cardData, actionFunc);
        
        //Assert
        ImageGetterMock.Verify(x => 
            x.GetImage(cardData.BackgroundPictureUrl, false, It.IsAny<Action<Sprite>>()));
        Assert.AreEqual(cardData.Title, sut._dependencies.Title.text);
        Assert.AreEqual(cardData.Description, descriptionExcerpt);
        Assert.IsTrue(hasBeenCalled);
    }
    
     [Test]
    public void TestConfigure_ConfiguresDependentComponents()
    {
        // Given the user is logged in and is generating a productList
        // When the productCard is configured
        // Then system configures the productCard and calles the cardReady action
        
        //Setup
        var executeActionAtEndOfFixedFrameMock = new Mock<IExecuteActionAtEndOfFixedFrame>();
        executeActionAtEndOfFixedFrameMock
            .Setup(x => 
                x.Configure(It.IsAny<Action>(), It.IsAny<Action>()))
            .Verifiable();

        var cardData = new ProductDTO
            {Title = "product A", Description = "Description", BackgroundPictureUrl = "https://Img.com"};
        
        var actionFunc = new Action(() => { });

        //Act
        var sut = new GameObject().AddComponent<ProductCardBehavior>();
        sut.SetDependencies(CreateDependencies(
            null,
            null,
            null,
            null,
            executeActionAtEndOfFixedFrameMock));
        sut.Configure(cardData, actionFunc);
        
        //Assert
        executeActionAtEndOfFixedFrameMock.Verify(x =>
                x.Configure(It.IsAny<Action>(), It.IsAny<Action>()));
    }

    [Test]
    public void TestPlay_ActionIsCalledAndStartGame_Succeeds()
    {
        //GIVEN user with access to product in productcard.
        //WHEN play is called
        //THEN fetch product data, then instantiate and configure the huntasset
        
        //Arrange
        var cardData = new ProductDTO
            {Id = "productId", Title = "product A", Description = "Description", BackgroundPictureUrl = "https://Img.com"};

        var startPanelData = new StartPanelData() {Id = "productId", HasAccess = true, VideoUrl = "https://www.rmp-streaming.com/media/big-buck-bunny-360p.mp4"};
        var productCardStartPanelDataGetterMock = new Mock<IProductCardStartPanelDataGetter>();
        productCardStartPanelDataGetterMock.Setup(x => x.GetPanelData(cardData.Id))
         .ReturnsAsync(startPanelData).Verifiable();

        var huntViewMock = new Mock<IProductController>();
        huntViewMock.Setup(x => 
                x.Configure(It.IsAny<ProductController.Config>()))
            .Verifiable();
        
        var huntInstantiater = new Mock<IHuntInstantiater>();
        huntInstantiater.Setup(x =>
                 x.Create(startPanelData.ProductType,  cardData.Id))
             .Returns(huntViewMock.Object)
             .Verifiable();

        var waitToExecuteActionMock = new Mock<IWaitToExecuteAction>();
        waitToExecuteActionMock.Setup(x => x.StopWaiting()).Verifiable();
        waitToExecuteActionMock.Setup(x => 
                x.Configure(It.IsAny<Action>(), 0.1f))
            .Callback<Action, float>((theAction, theTime) =>
            {
                theAction.Invoke();
            }).Verifiable();

        var loaderViewMock = new Mock<ILoadingView>();
        loaderViewMock.Setup(x => x.Display()).Verifiable();

        var actionFunc = new Action(() => { });
        var sut = new GameObject().AddComponent<ProductCardBehavior>();
        sut.SetDependencies(
            CreateDependencies(huntInstantiater, null, productCardStartPanelDataGetterMock, waitToExecuteActionMock, null, loaderViewMock)
        );
        sut.Configure(cardData, actionFunc);
        //Act
        sut.Play();
        //Assert
        productCardStartPanelDataGetterMock.Verify(x => x.GetPanelData(cardData.Id));
        huntInstantiater.Verify(x =>
            x.Create(startPanelData.ProductType, cardData.Id));
        waitToExecuteActionMock.Verify(x => x.Configure(It.IsAny<Action>(), 0.1f));
        huntViewMock.Verify(x =>
            x.Configure(It.IsAny<ProductController.Config>()));
        loaderViewMock.Verify(x => x.Display());
    }

    [Test]
    public void ToggleExcerpt_Unfolds_The_ProductCard_CardObject_Active()
    {
        //Arrange
        var cardData = new ProductDTO
            {Title = "product A", Description = "Description", BackgroundPictureUrl = "https://Img.com"};
        for (int i = 0; i < 5; i++)
            cardData.Description += cardData.Description; //make a very long string
        
        var executeActionAtEndOfFixedFrameMock = new Mock<IExecuteActionAtEndOfFixedFrame>();
        executeActionAtEndOfFixedFrameMock
            .Setup(x => 
                x.BeginWaiting())
            .Verifiable();
        
        var sut = new GameObject().AddComponent<ProductCardBehavior>();
        sut.SetDependencies(
            CreateDependencies(
                null, 
                null, 
                null, 
                null, 
                executeActionAtEndOfFixedFrameMock)
        );
        var actionFunc = new Action(() => { });

        sut.Configure(cardData, actionFunc);
        
        //Act
        sut.ToggleExcerpt();
        executeActionAtEndOfFixedFrameMock
            .Verify(x =>
                x.BeginWaiting());
    }
    
    
    [Test]
    public void ToggleExcerpt_Unfolds_The_ProductCard_CardObject_InActive()
    {
        //Arrange
        var cardData = new ProductDTO
            {Title = "product A", Description = "Description", BackgroundPictureUrl = "https://Img.com"};
        for (int i = 0; i < 5; i++)
            cardData.Description += cardData.Description; //make a very long string
        
        var executeActionAtEndOfFixedFrameMock = new Mock<IExecuteActionAtEndOfFixedFrame>();
        executeActionAtEndOfFixedFrameMock
            .Setup(x => 
                x.BeginWaiting())
            .Verifiable();
        
        var sut = new GameObject().AddComponent<ProductCardBehavior>();
        sut.gameObject.SetActive(false);
        sut.SetDependencies(
            CreateDependencies(null, null, null, null)
        );
        var actionFunc = new Action(() => { });

        sut.Configure(cardData, actionFunc);
        
        //Act
        sut.ToggleExcerpt();
        executeActionAtEndOfFixedFrameMock
            .Verify(x =>
                x.BeginWaiting(), Times.Never);
    }
}

