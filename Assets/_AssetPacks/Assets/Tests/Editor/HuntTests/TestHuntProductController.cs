using System.Collections;
using System.Collections.Generic;
using CharacterSelection;
using Hunt;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Riddlehouse.Core.Helpers.Helpers;
using riddlehouse_libraries.products;
using riddlehouse_libraries.products.Assets.Adressables;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.huntProduct;
using riddlehouse_libraries.products.resources;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class TestHuntProductController
{
    ProductController.Dependencies CreateDependencies(
        Mock<ICharacterSelectionFlowController> characterSelectionFlowControllerMock = null,
        Mock<IProductResourceService> productResourceServiceMock = null,
        Mock<ICanvasController> canvasControllerMock = null,
        Mock<IHuntControllerInstantiator> huntControllerInstantiaterMock = null,
        Mock<IGetProductFlowData> getProductFlowDataMock = null,
        Mock<ICanvasLayerManager> canvasLayerManagerMock = null,
        Mock<IGameObjectDestroyer> godMock = null,
        Mock<IAdressableViewInstantiator> adressableViewInstantiatorMock = null)
    {
        characterSelectionFlowControllerMock ??= new Mock<ICharacterSelectionFlowController>();
        productResourceServiceMock ??= new Mock<IProductResourceService>();
        canvasControllerMock ??= new Mock<ICanvasController>();
        huntControllerInstantiaterMock ??= new Mock<IHuntControllerInstantiator>();
        getProductFlowDataMock ??= new Mock<IGetProductFlowData>();
        canvasLayerManagerMock ??= new Mock<ICanvasLayerManager>();
        godMock ??= new Mock<IGameObjectDestroyer>();
        adressableViewInstantiatorMock ??= new Mock<IAdressableViewInstantiator>();
        return new ProductController.Dependencies()
        { 
            CharacterSelectionFlowController = characterSelectionFlowControllerMock.Object,
            ProductResourceService = productResourceServiceMock.Object,
            CanvasController = canvasControllerMock.Object,
            HuntControllerInstantiator = huntControllerInstantiaterMock.Object,
            GetProductFlowData = getProductFlowDataMock.Object,
            Clm = canvasLayerManagerMock.Object,
            GOD = godMock.Object,
            AdressableViewInstantiator = adressableViewInstantiatorMock.Object
        };
    }

    private ProductController.Config _config;
    
    [SetUp]
    public void Init()
    {
        _config = new ProductController.Config()
        {
            ProductId = "productId"
        };
    }

    [TearDown]
    public void TearDown()
    {
        _config = null;
    }

    [Test]
    public void SetDependencies()
    {
        //Arrange
        var sut = new GameObject().AddComponent<ProductController>();

        var canvasControllerMock = new Mock<ICanvasController>();
        canvasControllerMock.Setup(x => x.Configure(It.IsAny<CanvasController.Config>())).Verifiable();
        var dependencies = CreateDependencies(null, null, canvasControllerMock);
        
        //Act
        sut.SetDependencies(dependencies);
        //Assert
        Assert.AreEqual(dependencies, sut._dependencies);
        canvasControllerMock.Verify(x => x.Configure(It.IsAny<CanvasController.Config>()));
    }
    
    [Test]
    public void TestConfigure_ConfiguresCharacterSelectionFlowController()
    {
        //Given a new HuntProductController
        //When configure is called
        //Then character selection is configured with necessary resources and an action to perform once the flow is complete.
        
        //Arrange
        var sut = new GameObject().AddComponent<ProductController>();
        var characterSelectionFlowControllerMock = new Mock<ICharacterSelectionFlowController>();
        characterSelectionFlowControllerMock.Setup(x=> 
            x.Configure(It.IsAny<CharacterSelectionFlowController.Config>())).Verifiable();
        
        var loaderViewAddress = "loaderViewAddress";
        var productResourceServiceMock = new Mock<IProductResourceService>();
        productResourceServiceMock.Setup(x => x.CharacterSelectionResources)
            .Returns(new CharacterSelectionResources()
            {
                LoadingView = new Addressable(loaderViewAddress, AddressableTypes.GameObject),
                CharacterSelectionFlowResource = new CharacterSelectionFlowResource()
            }).Verifiable();
        
        var loaderViewMock = new Mock<ILoadingView>();
        var adressableViewInstantiatorMock = new Mock<IAdressableViewInstantiator>();
        adressableViewInstantiatorMock.Setup(x=> x.GetLoaderView(loaderViewAddress))
            .ReturnsAsync(loaderViewMock.Object).Verifiable();

        var dependencies = CreateDependencies(
            characterSelectionFlowControllerMock, 
            productResourceServiceMock, 
            null, 
            null, 
            null, 
            null, 
            null, 
            adressableViewInstantiatorMock);

        sut.SetDependencies(dependencies);
        
        //Act
        sut.Configure(_config);
        
        //Assert
        characterSelectionFlowControllerMock.Verify(
            x=> x.Configure(It.IsAny<CharacterSelectionFlowController.Config>()));
        
        productResourceServiceMock.Verify(x => x.CharacterSelectionResources);

    }

    [Test]
    public void TestConfigure_CharacterSelectionFlowComplete_Action_Invoked_CreatesAndConfigures_HuntController()
    {
        //Given a configured HuntProductController
        //When the configured FlowComplete Action in the CharacterSelectionController is invoked
        //Then the product flow is collected, a huntcontroller is created, and configured
        
        //Arrange
        var sut = new GameObject().AddComponent<ProductController>();
        
        var characterSelectionFlowControllerMock = new Mock<ICharacterSelectionFlowController>();
        var huntCharacterData = new List<HuntCharacterData>();
        
        characterSelectionFlowControllerMock.Setup(x=> 
            x.Configure(It.IsAny<CharacterSelectionFlowController.Config>()))
            .Callback<CharacterSelectionFlowController.Config>((theConfig) =>
                {
                  theConfig.FlowComplete.Invoke(huntCharacterData);
                })
            .Verifiable();

        var loaderViewAddress = "loaderViewAddress";
        var productResourceServiceMock = new Mock<IProductResourceService>();
        productResourceServiceMock.Setup(x => x.CharacterSelectionResources)
            .Returns(new CharacterSelectionResources()
            {
                LoadingView = new Addressable(loaderViewAddress, AddressableTypes.GameObject),
                CharacterSelectionFlowResource = new CharacterSelectionFlowResource()
            }).Verifiable();
        
        var flow = new HuntProductFlow();

        var huntControllerMock = new Mock<IHuntController>();
        huntControllerMock.Setup(x => x.Configure(It.IsAny<HuntController.Config>())).Verifiable();

        var huntControllerInstantiaterMock = new Mock<IHuntControllerInstantiator>();
        huntControllerInstantiaterMock.Setup(x => x.Create())
            .Returns(huntControllerMock.Object).Verifiable();

        var getProductFlowMock = new Mock<IGetProductFlowData>();
        getProductFlowMock.Setup(x => x.GetHuntProductFlow(_config.ProductId, huntCharacterData, It.IsAny<HuntSessionPersistor>()))
            .ReturnsAsync(flow)
            .Verifiable();

        var loaderViewMock = new Mock<ILoadingView>();
        loaderViewMock.Setup(x => x.Display()).Verifiable();
        loaderViewMock.Setup(x =>
            x.FitInView((RectTransform)sut.transform, It.IsAny<UIFitters>(), sut.transform.childCount + 1));
        var adressableViewInstantiatorMock = new Mock<IAdressableViewInstantiator>();
        adressableViewInstantiatorMock.Setup(x=> x.GetLoaderView(loaderViewAddress))
            .ReturnsAsync(loaderViewMock.Object).Verifiable();

        var dependencies = CreateDependencies(
            characterSelectionFlowControllerMock, 
            productResourceServiceMock, 
            null, 
            huntControllerInstantiaterMock, 
            getProductFlowMock,
            null, 
            null,
            adressableViewInstantiatorMock);

        sut.SetDependencies(dependencies);
        
        //Act (the actual action happens in the callback, invoked on characterSelectionFlowControllerMock)
        sut.Configure(_config);
        
        //Assert
        productResourceServiceMock.Verify(x => x.CharacterSelectionResources);
        huntControllerInstantiaterMock.Verify(x => x.Create());
        huntControllerMock.Verify(x => x.Configure(It.IsAny<HuntController.Config>()));
        getProductFlowMock.Verify(x => x.GetHuntProductFlow(_config.ProductId, huntCharacterData, It.IsAny<HuntSessionPersistor>()));
        adressableViewInstantiatorMock.Verify(x=> x.GetLoaderView(loaderViewAddress));
        loaderViewMock.Verify(x =>
            x.FitInView((RectTransform)sut.transform, It.IsAny<UIFitters>(), sut.transform.childCount + 1));
        loaderViewMock.Verify(x => x.Display());
    }
    [TestCase(true)]
    [TestCase(false)]
    public void TestGoBackToProductList(bool completed)
    {
        //Arrange
        var sut = new GameObject().AddComponent<ProductController>();
        var productResourceServiceMock = new Mock<IProductResourceService>();
        productResourceServiceMock.Setup(x => x.CharacterSelectionResources)
            .Returns(new CharacterSelectionResources()
            {
                CharacterSelectionFlowResource = new CharacterSelectionFlowResource()
            }).Verifiable();

        var dependencies = CreateDependencies(null, productResourceServiceMock);
        sut.SetDependencies(dependencies);

                
        bool hasBeenCalled = false;
        bool value = !completed;
        _config.EndProduct = (completed) =>
        {
            hasBeenCalled = true;
            value = completed;
        };

        
        sut.Configure(_config);

        //Act
        sut.EndProduct(completed);
        
        //Assert
        Assert.IsTrue(hasBeenCalled);
        Assert.AreEqual(completed, value);
    }
    
     [Test]
    public void TestDestroySelf_Destroys_HuntController_And_Itself_Sets_CanvasLayerToHome()
    {
        //Given a configured HuntProductController
        //When the DestroySelf is called
        //Then HuntController is destroyed as well as the productcontroller, and the homeCanvas is set interactable.
        
        //Arrange
        var sut = new GameObject().AddComponent<ProductController>();
        
        var characterSelectionFlowControllerMock = new Mock<ICharacterSelectionFlowController>();
        var huntCharacterData = new List<HuntCharacterData>();
        
        characterSelectionFlowControllerMock.Setup(x=> 
            x.Configure(It.IsAny<CharacterSelectionFlowController.Config>()))
            .Callback<CharacterSelectionFlowController.Config>((theConfig) =>
                {
                  theConfig.FlowComplete.Invoke(huntCharacterData);
                })
            .Verifiable();
        
        var loaderViewAddress = "loaderViewAddress";
        var productResourceServiceMock = new Mock<IProductResourceService>();
        productResourceServiceMock.Setup(x => x.CharacterSelectionResources)
            .Returns(new CharacterSelectionResources()
            {
                LoadingView = new Addressable(loaderViewAddress, AddressableTypes.GameObject),
                CharacterSelectionFlowResource = new CharacterSelectionFlowResource()
            }).Verifiable();
        
        var loaderViewMock = new Mock<ILoadingView>();
        var adressableViewInstantiatorMock = new Mock<IAdressableViewInstantiator>();
        adressableViewInstantiatorMock.Setup(x=> x.GetLoaderView(loaderViewAddress))
            .ReturnsAsync(loaderViewMock.Object).Verifiable();
        
        var flow = new HuntProductFlow();

        var huntControllerMock = new Mock<IHuntController>();
        huntControllerMock.Setup(x => x.DestroySelf()).Verifiable();

        var huntControllerInstantiaterMock = new Mock<IHuntControllerInstantiator>();
        huntControllerInstantiaterMock.Setup(x => x.Create())
            .Returns(huntControllerMock.Object).Verifiable();

        var getProductFlowMock = new Mock<IGetProductFlowData>();
        getProductFlowMock.Setup(x => x.GetHuntProductFlow(_config.ProductId, huntCharacterData, It.IsAny<HuntSessionPersistor>()))
            .ReturnsAsync(flow)
            .Verifiable();
        
        var canvasLayerControllerMock = new Mock<ICanvasController>();
        canvasLayerControllerMock.Setup(x => x.DestroySelf()).Verifiable();
        
        var godMock = new Mock<IGameObjectDestroyer>();
        godMock.Setup(x => x.Destroy()).Verifiable();
        
        var dependencies = CreateDependencies(
            characterSelectionFlowControllerMock, 
            productResourceServiceMock, 
            canvasLayerControllerMock, 
            huntControllerInstantiaterMock, 
            getProductFlowMock,
            null,
            godMock,
            adressableViewInstantiatorMock);

        sut.SetDependencies(dependencies);
        sut.Configure(_config);

        //Act
        sut.DestroySelf();
        
        //Assert
        huntControllerMock.Verify(x => x.DestroySelf());
        godMock.Verify(x => x.Destroy());
        canvasLayerControllerMock.Verify(x => x.DestroySelf());
    }
}
