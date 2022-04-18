using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Riddlehouse.Core.Helpers.Helpers;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products;
using riddlehouse_libraries.products.models;
using riddlehouse_libraries.products.models.DTOs;
using UnityEngine;
// using UnityEngine.XR.ARSubsystems;

[Ignore("Deprecated")]
public class TestHuntViewBehaviour
{
//     public Mock<IHuntComponentUIActions> CreateUIActions(ComponentType type)
//     {
//         var mock = new Mock<IHuntComponentUIActions>();
//         mock.Setup(x => x.GetComponentType()).Returns(type);
//         return mock;
//     }
//     public HuntViewBehaviour CreateSUT(
//         ITabComponent iTabComponent = null, 
//         IHuntHomeComponent iHuntHomeComponent = null, 
//         IStoryComponent iStoryComponent = null, 
//         IRiddleTabComponent iRiddleTabComponent = null, 
//         IEndHuntComponent iEndHuntComponent = null, 
//         IScanningCorrectDisplayVideoComponent iScanningCorrectDisplayVideoComponent = null,
//         IResolutionComponent iResolutionComponent = null)
//     {
//         var gameObject = new GameObject();
//         gameObject.AddComponent<RectTransform>();
//         var sut = gameObject.AddComponent<HuntViewBehaviour>();
//         
//         iTabComponent ??= new Mock<ITabComponent>().Object;
//
//         if (iHuntHomeComponent == null)
//         {
//             var ihuntHomeComponentMock = new Mock<IHuntHomeComponent>();
//             ihuntHomeComponentMock
//                 .Setup(x => x.GetComponentUIActions())
//                 .Returns(CreateUIActions(ComponentType.HuntHome).Object);
//             iHuntHomeComponent = ihuntHomeComponentMock.Object;
//         }
//
//         if (iStoryComponent == null)
//         {
//             var iStoryComponentMock = new Mock<IStoryComponent>();
//             iStoryComponentMock
//                 .Setup(x => x.GetComponentUIActions())
//                 .Returns(CreateUIActions(ComponentType.Story).Object);
//             iStoryComponent = iStoryComponentMock.Object;
//         }
//
//         if (iRiddleTabComponent == null)
//         {
//             var iRiddleTabComponentMock = new Mock<IRiddleTabComponent>();
//             iRiddleTabComponentMock
//                 .Setup(x => x.GetComponentUIActions())
//                 .Returns(CreateUIActions(ComponentType.RiddleTab).Object);
//             iRiddleTabComponent = iRiddleTabComponentMock.Object;
//         }
//
//         if (iEndHuntComponent == null)
//         {
//             var iEndHuntComponentMock = new Mock<IEndHuntComponent>();
//             iEndHuntComponentMock
//                 .Setup(x => x.GetComponentUIActions())
//                 .Returns(CreateUIActions(ComponentType.End).Object);
//             iEndHuntComponent = iEndHuntComponentMock.Object;
//         }
//
//         if (iScanningCorrectDisplayVideoComponent == null)
//         {
//             var iScanningCorrectDisplayVideoComponentMock = new Mock<IScanningCorrectDisplayVideoComponent>();
//             iScanningCorrectDisplayVideoComponentMock
//                 .Setup(x => x.GetComponentUIActions())
//                 .Returns(CreateUIActions(ComponentType.Scanning).Object);
//             iScanningCorrectDisplayVideoComponent = iScanningCorrectDisplayVideoComponentMock.Object;
//         }
//
//         if (iResolutionComponent == null)
//         {
//             var iResolutionComponentMock = new Mock<IResolutionComponent>();
//             iResolutionComponentMock.Setup(
//                 x => x.GetComponentUIActions())
//                 .Returns(CreateUIActions(ComponentType.Resolution).Object);
//             iResolutionComponent = iResolutionComponentMock.Object;
//         }
//
//         sut.SetTestDependencies(iTabComponent, iHuntHomeComponent, iStoryComponent, iRiddleTabComponent, iEndHuntComponent, iScanningCorrectDisplayVideoComponent, iResolutionComponent);
//         return sut;
//     } 
// //TODO: AR Element
//     // public XRReferenceImageLibrary lib;
//
//     [SetUp]
//     public void Init()
//     {
//         //collects imagelibrary asset file from streaming assets; if null, then the file is missing.
//         //TODO: AR Element
//         // lib = Resources.Load<XRReferenceImageLibrary>("editor/testLibrary");
//     }
//
//     [Test]
//     public void TestConfigure()
//     {    
//         // Given a user with access to a specific hunt, watches an introduction video of a hunt
//         // When waiting for the video to finish
//         // Then the app downloads the required flow object in the background
//
//         //Arrange
//         var productId = "id";
//         StartPanelData startPanelData = new StartPanelData() { HasAccess = true, Id = productId, Title = "title", VideoUrl = "videoURL", FeedbackLink = "https://google.com"};
//         
//         var stepdata = new InternalDisplayStoryAndDoneHuntStep();
//         stepdata.StoryText = "hint";
//
//         List<IInternalHuntStep> stepdataList = new List<IInternalHuntStep>() {stepdata};
//         HuntSteps huntdata = new HuntSteps(startPanelData.FeedbackLink, "id");
//         huntdata.ConvertInternalStepdata(stepdataList);
//         
//         var storyAssetUrl = "https://some.url";
//         var storyAssetType = AssetType.StoryText;
//         
//         var riddleAssetUrl = "https://some.url";
//         var riddleAssetType = AssetType.RiddleText;
//         
//         var stepType = StepType.RecognizeImageAndPlayVideo;
//         HuntAsset storyAssetDto = new HuntAsset() { Url = storyAssetUrl, Type = storyAssetType};
//         HuntAsset riddleAssetDto = new HuntAsset() { Url = riddleAssetUrl, Type = riddleAssetType};
//
//         HuntStep stepDto = new HuntStep() { Type = stepType, Assets = new List<HuntAsset>(new[] {storyAssetDto, riddleAssetDto})};
//         HuntFlow huntFlow = new HuntFlow() { Steps = new List<HuntStep>(new[]{stepDto})};
//         
//         var flowMock = new Mock<IGetProductFlowData>();
//         flowMock.Setup(x=>x.GetProductFlow(productId)).ReturnsAsync(huntFlow);
//         
//         var callBackCalled = false;
//         var huntAssetGetterMock = new Mock<IHuntAssetGetter>();
//         huntAssetGetterMock
//             .Setup(x => x.GetHuntAssets(huntFlow, productId, startPanelData.FeedbackLink, It.IsAny<Action<IHuntSteps>>()))
//             .Callback<HuntFlow, string, string, Action<IHuntSteps>>((flowDto, theProductId, myFeedbackLink, huntDataIsReady) =>
//             {
//                 huntDataIsReady.Invoke(huntdata);
//                 callBackCalled = true;
//             });
//
//         var huntHomeMock = new Mock<IHuntHomeComponent>();
//         huntHomeMock.Setup(x => x.Configure(startPanelData, It.IsAny<Action<bool>>(), It.IsAny<Action<bool>>()))
//             .Callback<StartPanelData, Action<bool>, Action<bool>>((theStartPanelData, theIntroVideoPrepared, theGoBackFunction ) =>
//             {
//                 theIntroVideoPrepared.Invoke(true);
//             }).Verifiable();
//         huntHomeMock.Setup(x => x.HuntReady(It.IsAny<IHuntController>(), It.IsAny<IConditionalStepList>(), It.IsAny<List<IConditionalStepBtn>>())).Verifiable();
//        
//         var sut = CreateSUT(null, huntHomeMock.Object);
//         var huntViewMock = new Mock<IHuntView>();
//         huntViewMock.Setup(x => x.GetHuntController()).Returns(new HuntController());
//         sut.SetLogicInstance(huntViewMock.Object);
//         //Act
//         var task  = Task.Run(() => sut.configure(startPanelData, huntAssetGetterMock.Object, flowMock.Object));
//         task.Wait();
//         
//         //Assert
//         huntAssetGetterMock.Verify(x => x.GetHuntAssets(huntFlow, productId, startPanelData.FeedbackLink, It.IsAny<Action<IHuntSteps>>()));
//         Assert.IsTrue(callBackCalled);
//         flowMock.Verify(x => x.GetProductFlow(productId));
//         huntHomeMock.Verify(x => x.Configure(startPanelData, It.IsAny<Action<bool>>(), It.IsAny<Action<bool>>()));
//         huntHomeMock.Verify(x =>
//             x.HuntReady(It.IsAny<IHuntController>(), It.IsAny<IConditionalStepList>(), It.IsAny<List<IConditionalStepBtn>>()));
//     }
//
//     [Test]
//     public void Test_FitInView_FitsToFullScreen()
//     {
//         //Given the users starts a new hunt
//         //When the hunt is created, the Huntview is UIfitted.
//         //Then the Huntview is set to fill the entire tab screen.
//
//         var sut = CreateSUT();
//         var parent = new GameObject().AddComponent<RectTransform>();
//
//         var uiFittersMock = new Mock<IUIFitters>();
//         uiFittersMock.Setup(x => x.FitToGlobalView((RectTransform)sut.gameObject.transform)).Verifiable();
//         Camera camera = new Camera();
//
//         sut.FitInView(parent, uiFittersMock.Object, camera);
//
//         uiFittersMock.Verify(x => x.FitToGlobalView((RectTransform)sut.gameObject.transform));
//     }
//     
//     [Test]
//     public void Test_IsShown_false()
//     {
//         //Given that the user changes view
//         //When the system checks if the view is active
//         //Then the function returns false.
//
//         // Arrange
//         var sut = CreateSUT();
//         sut.gameObject.SetActive(false);
//
//         // ACT & Assert
//         Assert.IsFalse(sut.IsShown());
//     }
}