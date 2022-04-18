using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using riddlehouse_libraries;
using UnityEngine;
using UnityEngine.TestTools;
using riddlehouse_libraries.products;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using riddlehouse_libraries.products.models.DTOs;

public class TestProductList
{
    [Test]
    public void TestInitialize_Configures_And_WaitsFor_AllProductCards()
    {
        //GIVEN cards = null, and two productCards of different types.
        //WHEN Configure is called.
        //THEN cards of the correct type are added to the productlist.
        
        //Arrange
        var productCardList = new List<ProductBasicsDTO>()
        {
            new ProductBasicsDTO {
                ProductCardStyle = ProductCardStyles.Hunt,
                Title = "product A", 
                Description = "beskrivelse", 
                BackgroundPictureUrl = "https://Img.com",
                IntroVideoUrl = "http://introvideo",
                Version = new ProductVersion(Startup.BuildVersion.Major, Startup.BuildVersion.Minor, Startup.BuildVersion.Patch)
            },
            new ProductBasicsDTO {
                ProductCardStyle = ProductCardStyles.ChristmasHunt,
                Title = "product B", 
                Description = "beskrivelse", 
                BackgroundPictureUrl = "https://Img.com",
                IntroVideoUrl = "http://introvideo",
                Version = new ProductVersion(Startup.BuildVersion.Major, Startup.BuildVersion.Minor, Startup.BuildVersion.Patch)
            }
        };
        RectTransform contentParent = new GameObject().AddComponent<RectTransform>();

        var productCardMockA = new Mock<IProductCard>();
        productCardMockA.Setup(x => x.Configure(productCardList[0],It.IsAny<Action>()))
            .Callback<ProductBasicsDTO, Action>((theProductBasics, theAction) =>
            {
                theAction.Invoke();
            })
            .Verifiable();

        var productCardMockB = new Mock<IProductCard>();
        productCardMockB.Setup(x => x.Configure(productCardList[1],It.IsAny<Action>()))
            .Callback<ProductBasicsDTO, Action>((theProductBasics, theAction) =>
            {
                theAction.Invoke();
            })
            .Verifiable();

        var productCardInstantiaterMock = new Mock<IProductCardInstantiater>();
        productCardInstantiaterMock.Setup(x => x.Create(productCardList[0], contentParent, It.IsAny<IProductEvents>()))
            .Returns(productCardMockA.Object)
            .Verifiable();
        productCardInstantiaterMock.Setup(x => x.Create(productCardList[1], contentParent, It.IsAny<IProductEvents>()))
            .Returns(productCardMockB.Object)
            .Verifiable();

        var productDataGetterMock = new Mock<IProductDataGetter>();
        productDataGetterMock
            .Setup(x => x.GetCardList()).ReturnsAsync(productCardList).Verifiable();
        
        var dependencies = new ProductListBehaviour.Dependencies()
        {
            ContentParent = contentParent,
            ProductDataGetter = productDataGetterMock.Object,
            Instantiater = productCardInstantiaterMock.Object
        };
        
        var sut = new GameObject().AddComponent<ProductListBehaviour>();
        sut.SetDependencies(dependencies);
        
        var readyForDisplayWasRun = false;
        //Act
        sut.Configure(() => { readyForDisplayWasRun = true; });
        
        //Assert
        productCardInstantiaterMock.Verify(x => x.Create(productCardList[0], contentParent, sut));
        productCardInstantiaterMock.Verify(x => x.Create(productCardList[1], contentParent, sut));

        productCardMockA.Verify(x => x.Configure(productCardList[0],It.IsAny<Action>()));
        productCardMockB.Verify(x => x.Configure(productCardList[1],It.IsAny<Action>()));
        
        Assert.IsTrue(readyForDisplayWasRun);
    }

    [Test]
    public void TestSetSiblingIndex()
    {
        // Given a productlist that is a child of a gameobject
        // When SetSiblingIndex is called with value 0
        // Then the siblingindex of the productindex becomes 0
        var parent = new GameObject().transform;
        var productListGO = new GameObject();
        productListGO.transform.SetParent(parent);
        var sibling = new GameObject().transform;
        sibling.SetParent(parent);
        var sut = productListGO.AddComponent<ProductListBehaviour>();
        sut.SetSiblingIndex(1);
        Assert.AreEqual(0, sibling.GetSiblingIndex());
        Assert.AreEqual(1, productListGO.transform.GetSiblingIndex());
    }
    
    [Test]
    public void TestDestroyGameObject()
    {
        // Given a productlist that is a child of a gameobject
        // When SetSiblingIndex is called with value 0
        // Then the siblingindex of the productindex becomes 0
        var productListGO = new GameObject();
        ProductListBehaviour sut = productListGO.AddComponent<ProductListBehaviour>();
        sut.DestroyGameObject();
        Assert.IsTrue(sut == null);
        Assert.IsTrue(productListGO == null);
    }
}
