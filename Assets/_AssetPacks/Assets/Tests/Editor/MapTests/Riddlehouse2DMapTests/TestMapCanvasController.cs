using Riddlehouse.Core.Helpers.Helpers;
using Hunt;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;
using UnityEngine;

namespace Tests.Editor.MapTests.Riddlehouse2DMapTests
{
    public class TestMapCanvasController
    {
        private MapCanvasController.Dependencies CreateDependencies(
            GameObject go, 
            Mock<IMapMenuController> mapMenuControllerMock = null,
            Mock<ICanvasController> canvasControllerMock = null)
        {
            mapMenuControllerMock ??= new Mock<IMapMenuController>();
            canvasControllerMock ??= new Mock<ICanvasController>();
            return new MapCanvasController.Dependencies()
            {
                MapMenuController = mapMenuControllerMock.Object,
                CanvasController = canvasControllerMock.Object
            };
        }
        [Test]
        public void TestSetDependencies()
        {
            //Arrange
            var go = new GameObject();
            var sut = go.AddComponent<MapCanvasController>();
            var dependencies = CreateDependencies(go);
            //Act
            sut.SetDependencies(dependencies);
            //Assert
            Assert.AreEqual(dependencies, sut._dependencies);
        }

        [Test]
        public void TestConfigure_ConfiguresCanvasController()
        {
            //Given a new Map2DCanvasController
            //When configure is called
            //Then the canvascontroller config is passed to the canvascontroller config.
        
            //Arrange
            var go = new GameObject();
            var sut = go.AddComponent<MapCanvasController>();
            sut.gameObject.SetActive(false);

            
            var mapMenuController = new Mock<IMapMenuController>();
            mapMenuController.Setup(x => x.Display()).Verifiable();
            mapMenuController.Setup(x => x.Configure(null)).Verifiable();
            
            var canvasControllerMock = new Mock<ICanvasController>();
            
            canvasControllerMock.Setup(x => x.Configure()).Verifiable();
        
            var dependencies = CreateDependencies(go, mapMenuController, canvasControllerMock);
            sut.SetDependencies(dependencies);
        
            //Act
            sut.ConfigureAndDisplay(new MapCanvasController.Config()
            {
                BackButtonAction = null
            });
        
            //Assert
            canvasControllerMock.Verify(x => x.Configure());
            mapMenuController.Verify(x => x.Configure(null));
            mapMenuController.Verify(x => x.Display());
            Assert.IsTrue(sut.gameObject.activeSelf);
        }
        
        [Test]
        public void TestAttachViewToCanvas()
        {
            //Arrange
            var go = new GameObject();
            var sut = go.AddComponent<MapCanvasController>();
            var dependencies = CreateDependencies(go);
            sut.SetDependencies(dependencies);
            var huntview = new Mock<IViewActions>();
            int index = 3;
            huntview.Setup(x => x.FitInView((RectTransform)go.transform, It.IsAny<UIFitters>(), index)).Verifiable();
            
            //Act
            sut.AttachViewToCanvas(huntview.Object, 3);
            
            //Assert
            huntview.Verify(x => x.FitInView((RectTransform)go.transform, It.IsAny<UIFitters>(), index));

        }
    }
}
