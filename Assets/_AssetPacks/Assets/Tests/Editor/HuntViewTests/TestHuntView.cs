using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products;
using riddlehouse_libraries.products.models;
using UnityEngine;

[Ignore("Deprecated")]
public class TestHuntView
{
    // public HuntView CreateSUT(
    //     IHuntViewActions huntViewActionsMock = null, 
    //     IProductEvents productEvents = null, 
    //     IViewUIActions viewUIActionsMock = null,
    //     IHuntController huntControllerMock = null)
    // {
    //     huntViewActionsMock ??= new Mock<IHuntViewActions>().Object;
    //     viewUIActionsMock ??= new Mock<IViewUIActions>().Object;
    //     productEvents ??= new Mock<IProductEvents>().Object;
    //     huntControllerMock ??= new Mock<IHuntController>().Object;
    //     var sut = new HuntView(huntViewActionsMock, viewUIActionsMock, productEvents, huntControllerMock);
    //     return sut;
    // }
    //
    // [Test]
    // public void TestFactory_Creates_New_Instance()
    // {
    //     // Given a game object with an EndHuntComponentBehaviour component
    //     // When Factory() is called
    //     // Then return a new instance of StoryComponent that references the given component
    //         
    //     // Arrange
    //     var testGo = new GameObject();
    //     testGo.AddComponent<RectTransform>();
    //     testGo.AddComponent<HuntViewBehaviour>();
    //     Camera camera = new Camera();
    //     // Act
    //     IHuntView sut = HuntView.Factory(testGo, null, camera);
    //
    //     // Assert
    //     Assert.IsNotNull(sut);
    // }
    //     
    // [Test]
    // public void TestFactory_Throws_ArgumentException_If_GameObject_Does_Not_Have_Expected_Component()
    // {
    //     // Given a game object without a HuntViewBehaviour component
    //     // When Factory() is called
    //     // Then an argument exception is thrown
    //         
    //     // Arrange
    //     GameObject testGo = new GameObject();
    //     testGo.AddComponent<RectTransform>();
    //     Camera camera = new Camera();
    //
    //     // Act & Assert
    //     Assert.Throws<ArgumentException>(() => HuntView.Factory(testGo,null, camera));
    // }
    //
    // [Test]
    // public void TestConfigure()
    // {
    //     StartPanelData startPanelData = new StartPanelData() { HasAccess = true, Id = "id", Title = "title", VideoUrl = "videoURL", FeedbackLink = "feedbackLink" };
    //     var huntViewActionsMock = new Mock<IHuntViewActions>();
    //     huntViewActionsMock.Setup(x => x.configure(startPanelData, null, It.IsAny<ProductService>())).Verifiable();
    //     var sut = CreateSUT(huntViewActionsMock.Object);
    //     sut.Configure(startPanelData);
    //     huntViewActionsMock.Verify(x => x.configure(startPanelData, null, It.IsAny<ProductService>()));
    // }
    //
    // [Test]
    // public void TestGoBackToProductList()
    // {
    //     //Given a huntview
    //     //When a product experience is aborted
    //     //Then GoBackToProductList is called with false.
    //     
    //     //Arrange
    //     var productEventsMock = new Mock<IProductEvents>();
    //     productEventsMock.Setup(x => x.ProductAborted()).Verifiable();
    //     var huntViewActionsMock = new Mock<IHuntViewActions>();
    //     var huntViewUIActionsMock = new Mock<IViewUIActions>();
    //     huntViewUIActionsMock.Setup(x => x.Hide()).Verifiable();
    //     
    //     var huntControllerMock = new Mock<IHuntController>();
    //     huntControllerMock.Setup(x => x.EndHunt(false)).Verifiable();
    //     var sut = CreateSUT(huntViewActionsMock.Object, productEventsMock.Object, huntViewUIActionsMock.Object, huntControllerMock.Object);
    //     //Act
    //     sut.GoBackToProductList(false);
    //     //Assert
    //     huntViewUIActionsMock.Verify(x => x.Hide());
    //     huntControllerMock.Verify(x => x.EndHunt(false));
    //     productEventsMock.Verify(x => x.ProductAborted());
    // }
}
