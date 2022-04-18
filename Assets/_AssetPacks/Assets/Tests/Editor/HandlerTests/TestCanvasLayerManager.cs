using Moq;
using NUnit.Framework;
using Riddlehunt.Beta.Environment.Controls;

namespace Tests.Editor.HandlerTests
{
    public class TestCanvasLayerManager
    {
        [Test]
        public void TestGetVideoCanvas_GetsNew()
        {
            //Given there is no existing video canvas controller
            //When GetVideoCanvas is called
            //Then we get a new video canvas controller
            var mockCanvasLayerTypes = new Mock<ICanvasLayerTypes>();
            var mockIVideoCanvasController = new Mock<IVideoCanvasController>();
            
            var canvasControllerMock = new Mock<ICanvasController>();
            canvasControllerMock.Setup(x => x.SetLayerOrder(2)).Verifiable();
            mockIVideoCanvasController.Setup(x => x.dependencies).Returns(new VideoCanvasController.Dependencies()
            {
                CanvasController = canvasControllerMock.Object
            });
            
            mockCanvasLayerTypes.Setup(x => x.CreateVideoCanvasController())
                .Returns(mockIVideoCanvasController.Object).Verifiable();
            var sut = new CanvasLayerManager(mockCanvasLayerTypes.Object);
            IVideoCanvasController videocanvas = sut.GetVideoCanvas();
            Assert.AreSame(mockIVideoCanvasController.Object, videocanvas);
            mockCanvasLayerTypes.Verify(x => x.CreateVideoCanvasController());
        }
        
        [Test]
        public void TestGetVideoCanvas_WhenCanvasExists_GetsExisting()
        {
            //Given there is already a video canvas
            //When GetVideoCanvas is called
            //Then we get the existing canvas
            var mockCanvasLayerTypes = new Mock<ICanvasLayerTypes>();
            var mockIVideoCanvasController = new Mock<IVideoCanvasController>();
            var canvasControllerMock = new Mock<ICanvasController>();
            canvasControllerMock.Setup(x => x.SetLayerOrder(15)).Verifiable();
            mockIVideoCanvasController.Setup(x => x.dependencies).Returns(new VideoCanvasController.Dependencies()
            {
                CanvasController = canvasControllerMock.Object
            });
            mockCanvasLayerTypes.Setup(x => x.CreateVideoCanvasController())
                .Returns(mockIVideoCanvasController.Object).Verifiable();
            var sut = new CanvasLayerManager(mockCanvasLayerTypes.Object);
            IVideoCanvasController firstCanvas = sut.GetVideoCanvas();
            IVideoCanvasController secondCanvas = sut.GetVideoCanvas();
            Assert.AreSame(firstCanvas, secondCanvas);
            mockCanvasLayerTypes.Verify(x => x.CreateVideoCanvasController());
            canvasControllerMock.Verify(x => x.SetLayerOrder(15));
        }
        
        [Test]
        public void TestGetBetaTesterController_GetsNew()
        {
            //Given there is no existing beta tester canvas controller
            //When GetBetaTesterController is called
            //Then we get a new betatester canvas controller
            var mockCanvasLayerTypes = new Mock<ICanvasLayerTypes>();
            var mockBetaTesterController = new Mock<IBetaTesterController>();
            mockCanvasLayerTypes.Setup(x => x.CreateIBetaTesterCanvasController())
                .Returns(mockBetaTesterController.Object).Verifiable();
            var sut = new CanvasLayerManager(mockCanvasLayerTypes.Object);
            IBetaTesterController betaTesterCanvas = sut.GetBetaTesterController();
            Assert.AreSame(mockBetaTesterController.Object, betaTesterCanvas);
            mockCanvasLayerTypes.Verify(x => x.CreateIBetaTesterCanvasController());
        }
        
        [Test]
        public void TestGetBetaTesterController_WhenCanvasExists_GetsExisting()
        {
            //Given there is already a video canvas
            //When GetVideoCanvas is called
            //Then we get the existing canvas
            var mockCanvasLayerTypes = new Mock<ICanvasLayerTypes>();
            var mockBetaTesterController = new Mock<IBetaTesterController>();
            mockCanvasLayerTypes.Setup(x => x.CreateIBetaTesterCanvasController())
                .Returns(mockBetaTesterController.Object).Verifiable();
            var sut = new CanvasLayerManager(mockCanvasLayerTypes.Object);
            IBetaTesterController firstCanvas = sut.GetBetaTesterController();
            IBetaTesterController secondCanvas = sut.GetBetaTesterController();
            Assert.AreSame(firstCanvas, secondCanvas);
            mockCanvasLayerTypes.Verify(x => x.CreateIBetaTesterCanvasController());
        }
        
        [Test]
        public void TestGetWebViewCanvas_GetsNew()
        {
            //Given there is no existing webview canvas
            //When GetWebViewCanvas is called
            //Then we get a new webview canvas
            var mockCanvasLayerTypes = new Mock<ICanvasLayerTypes>();
            var mockWebviewCanvas = new Mock<IWebviewCanvas>();
            
            var canvasControllerMock = new Mock<ICanvasController>();
            canvasControllerMock.Setup(x => x.SetLayerOrder(20)).Verifiable();
            mockWebviewCanvas.Setup(x => x.dependencies).Returns(new WebviewCanvas.Dependencies()
            {
                CanvasController = canvasControllerMock.Object
            });
            mockCanvasLayerTypes.Setup(x => x.CreateWebviewCanvas())
                .Returns(mockWebviewCanvas.Object).Verifiable();
            var sut = new CanvasLayerManager(mockCanvasLayerTypes.Object);
            IWebviewCanvas webviewCanvas = sut.GetWebViewCanvas();
            Assert.AreSame(mockWebviewCanvas.Object, webviewCanvas);
            mockCanvasLayerTypes.Verify(x => x.CreateWebviewCanvas());
            canvasControllerMock.Verify(x => x.SetLayerOrder(20));
        }
        
        [Test]
        public void TestGetWebViewCanvas_WhenCanvasExists_GetsExisting()
        {
            //Given there is already a webview canvas
            //When GetWebViewCanvas is called
            //Then we get the existing canvas
            var mockCanvasLayerTypes = new Mock<ICanvasLayerTypes>();
            var mockWebviewCanvas = new Mock<IWebviewCanvas>();
            
            var canvasControllerMock = new Mock<ICanvasController>();
            canvasControllerMock.Setup(x => x.SetLayerOrder(20)).Verifiable();
            mockWebviewCanvas.Setup(x => x.dependencies).Returns(new WebviewCanvas.Dependencies()
            {
                CanvasController = canvasControllerMock.Object
            });
            
            mockCanvasLayerTypes.Setup(x => x.CreateWebviewCanvas())
                .Returns(mockWebviewCanvas.Object).Verifiable();
            var sut = new CanvasLayerManager(mockCanvasLayerTypes.Object);
            
            IWebviewCanvas firstCanvas = sut.GetWebViewCanvas();
            //Act
            IWebviewCanvas secondCanvas = sut.GetWebViewCanvas();
            
            Assert.AreSame(firstCanvas, secondCanvas);
            mockCanvasLayerTypes.Verify(x => x.CreateWebviewCanvas());
            canvasControllerMock.Verify(x => x.SetLayerOrder(20));
        }

        [Test]
        public void TestSetLayerInteractable()
        {
            //Given a canvasLayerManager with home and product registered
            //When home is then set interactable
            //Then home is reset from non-interactable to interactable.

            //Arrange
            var canvasControllerMock = new Mock<ICanvasController>();
            canvasControllerMock.Setup(x => x.SetLayerOrder(0)).Verifiable();
            canvasControllerMock.Setup(x => x.SetInteractable(false)).Verifiable();
            canvasControllerMock.Setup(x => x.SetInteractable(true)).Verifiable();

            var productCanvasControllerMock = new Mock<ICanvasController>();

            var mockCanvasLayerTypes = new Mock<ICanvasLayerTypes>();
            var sut = new CanvasLayerManager(mockCanvasLayerTypes.Object);
            
            sut.RegisterCanvas(CanvasLayerTypeNames.home, canvasControllerMock.Object);
            sut.RegisterCanvas(CanvasLayerTypeNames.product, productCanvasControllerMock.Object);
            
            //Act
            sut.SetLayerInteractable(CanvasLayerTypeNames.home);
            
            //Assert
            canvasControllerMock.Verify(x => x.SetInteractable(false), Times.Once());
            canvasControllerMock.Verify(x => x.SetInteractable(true), Times.Exactly(2));
        }

        [Test]
        public void TestRegisterCanvas_RegistersCanvasOfType_Home_SetsLayer_0_Then_Sets_Home_Interactable()
        {            
            //Given a canvasLayerManage
            //When home canvas is registered
            //Then the home canvas is registered as layer 0, and then set interactable.

            //Arrange
            var canvasControllerMock = new Mock<ICanvasController>();
            canvasControllerMock.Setup(x => x.SetLayerOrder(0)).Verifiable();
            canvasControllerMock.Setup(x => x.SetInteractable(true)).Verifiable();
            
            var mockCanvasLayerTypes = new Mock<ICanvasLayerTypes>();
            var sut = new CanvasLayerManager(mockCanvasLayerTypes.Object);
            
            //Act
            sut.RegisterCanvas(CanvasLayerTypeNames.home, canvasControllerMock.Object);
            
            //Assert
            canvasControllerMock.Verify(x => x.SetLayerOrder(0));
            canvasControllerMock.Verify(x => x.SetInteractable(true));
        }

        [Test]
        public void TestRegisterCanvas_RegistersCanvasOfType_Home_And_Type_Product_SetsHome_Interactable_False()
        {      
            //Given a canvasLayerManage with a HomeCanvas registered
            //When product canvas is registered
            //Then the product canvas takes control, and home is set non-interactable.
            //- layers are also set 0, 1
            
            //Arrange
            var canvasControllerMock = new Mock<ICanvasController>();
            canvasControllerMock.Setup(x => x.SetLayerOrder(0)).Verifiable();
            canvasControllerMock.Setup(x => x.SetInteractable(false)).Verifiable();
            
            var productCanvasControllerMock = new Mock<ICanvasController>();
            productCanvasControllerMock.Setup(x => x.SetLayerOrder(5)).Verifiable();
            productCanvasControllerMock.Setup(x => x.SetInteractable(true)).Verifiable();
            
            var mockCanvasLayerTypes = new Mock<ICanvasLayerTypes>();
            var sut = new CanvasLayerManager(mockCanvasLayerTypes.Object);
            
            sut.RegisterCanvas(CanvasLayerTypeNames.home, canvasControllerMock.Object);

            //Act
            sut.RegisterCanvas(CanvasLayerTypeNames.product, productCanvasControllerMock.Object);

            //Assert
            canvasControllerMock.Verify(x => x.SetInteractable(false));
            productCanvasControllerMock.Verify(x => x.SetLayerOrder(5));
            productCanvasControllerMock.Verify(x => x.SetInteractable(true));
        }

        [Test]
        public void TestUnregisterCanvas_RemovesTypeFromActiveLayers_ActivatesLayerBelow()
        {
            //Arrange
            var canvasControllerMock = new Mock<ICanvasController>();
            canvasControllerMock.Setup(x => x.SetLayerOrder(0)).Verifiable();
            canvasControllerMock.Setup(x => x.SetInteractable(false)).Verifiable();
            
            var productCanvasControllerMock = new Mock<ICanvasController>();
            productCanvasControllerMock.Setup(x => x.SetLayerOrder(5)).Verifiable();
            productCanvasControllerMock.Setup(x => x.SetInteractable(true)).Verifiable();
            
            var videoCanvasControllerMock = new Mock<ICanvasController>();
            videoCanvasControllerMock.Setup(x => x.SetLayerOrder(15)).Verifiable();
            videoCanvasControllerMock.Setup(x => x.SetInteractable(true)).Verifiable();
            
            var mockCanvasLayerTypes = new Mock<ICanvasLayerTypes>();
            var sut = new CanvasLayerManager(mockCanvasLayerTypes.Object);
            
            sut.RegisterCanvas(CanvasLayerTypeNames.home, canvasControllerMock.Object);

            sut.RegisterCanvas(CanvasLayerTypeNames.product, productCanvasControllerMock.Object);
            //map inbetween
            sut.RegisterCanvas(CanvasLayerTypeNames.video, videoCanvasControllerMock.Object);

            //Act
            sut.UnregisterCanvas(CanvasLayerTypeNames.video);

            //Assert
            canvasControllerMock.Verify(x => x.SetLayerOrder(0));
            canvasControllerMock.Verify(x => x.SetInteractable(false));
            productCanvasControllerMock.Verify(x => x.SetInteractable(false), Times.Exactly(1));
            productCanvasControllerMock.Verify(x => x.SetInteractable(true), Times.Exactly(2));
            videoCanvasControllerMock.Verify(x => x.SetLayerOrder(15));
            videoCanvasControllerMock.Verify(x => x.SetInteractable(true));

        }
        
        [Test]
        public void TestUnregisterCanvas_RemovesTypeFromActiveLayers_ActivatesPrioritizedLayerBelow()
        {
            //Arrange
            var canvasControllerMock = new Mock<ICanvasController>();
            canvasControllerMock.Setup(x => x.SetLayerOrder(0)).Verifiable();
            canvasControllerMock.Setup(x => x.SetInteractable(false)).Verifiable();
            
            var productCanvasControllerMock = new Mock<ICanvasController>();
            productCanvasControllerMock.Setup(x => x.SetLayerOrder(5)).Verifiable();
            productCanvasControllerMock.Setup(x => x.SetInteractable(true)).Verifiable();
            
            var mapCanvasControllerMock = new Mock<ICanvasController>();
            productCanvasControllerMock.Setup(x => x.SetLayerOrder(5)).Verifiable();
            productCanvasControllerMock.Setup(x => x.SetInteractable(true)).Verifiable();
            
            var videoCanvasControllerMock = new Mock<ICanvasController>();
            videoCanvasControllerMock.Setup(x => x.SetLayerOrder(15)).Verifiable();
            videoCanvasControllerMock.Setup(x => x.SetInteractable(true)).Verifiable();
            
            var mockCanvasLayerTypes = new Mock<ICanvasLayerTypes>();
            var sut = new CanvasLayerManager(mockCanvasLayerTypes.Object);
            
            sut.RegisterCanvas(CanvasLayerTypeNames.home, canvasControllerMock.Object);

            sut.RegisterCanvas(CanvasLayerTypeNames.product, productCanvasControllerMock.Object);
            
            sut.RegisterCanvas(CanvasLayerTypeNames.map, mapCanvasControllerMock.Object);
            
            sut.RegisterCanvas(CanvasLayerTypeNames.video, videoCanvasControllerMock.Object);

            //Act
            sut.UnregisterCanvas(CanvasLayerTypeNames.video); //expects it to skip map and go straight to product.

            //Assert
            canvasControllerMock.Verify(x => x.SetLayerOrder(0));
            canvasControllerMock.Verify(x => x.SetInteractable(false));
          
            productCanvasControllerMock.Verify(x => x.SetInteractable(false), Times.Exactly(2));
            productCanvasControllerMock.Verify(x => x.SetInteractable(true), Times.Exactly(2));
            
            mapCanvasControllerMock.Verify(x => x.SetInteractable(true), Times.Exactly(2));
            mapCanvasControllerMock.Verify(x => x.SetInteractable(false), Times.Exactly(1));
            
            videoCanvasControllerMock.Verify(x => x.SetLayerOrder(15));
            videoCanvasControllerMock.Verify(x => x.SetInteractable(true));

        }
    }
}