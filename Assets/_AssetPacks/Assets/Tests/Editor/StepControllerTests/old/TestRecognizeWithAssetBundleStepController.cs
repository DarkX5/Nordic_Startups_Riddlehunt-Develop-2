//TODO: AR Element
// using System;
// using System.Collections.Generic;
// using Moq;
// using NUnit.Framework;
// using riddlehouse_libraries.products.models;
// using UnityEngine;
// using UnityEngine.XR.ARSubsystems;
//
// [TestFixture]
// public class TestRecognizeWithAssetBundleStepController 
// {
//     public Mock<IHuntComponentUIActions> CreateComponentUIActionsMock(bool shown, ComponentType type)
//     {
//         var mock = new Mock<IHuntComponentUIActions>();
//         mock.Setup(x => x.Display()).Verifiable();
//         mock.Setup(x => x.IsShown()).Returns(shown).Verifiable();
//         mock.Setup(x => x.GetComponentType()).Returns(type).Verifiable();
//         return mock;
//     }
//     
//     public Dictionary<ComponentType, IHuntComponentUIActions> CreateViews(
//         IHuntComponentUIActions huntHomeMock,
//         IHuntComponentUIActions storyUIMock,
//         IHuntComponentUIActions endHuntUIMock,
//         IHuntComponentUIActions scanningUIMock
//         )
//     {
//         var Views = new Dictionary<ComponentType, IHuntComponentUIActions>();
//         Views = new Dictionary<ComponentType, IHuntComponentUIActions>() {};
//         Views.Add(ComponentType.HuntHome, huntHomeMock);
//         Views.Add(ComponentType.Story, storyUIMock);
//         Views.Add(ComponentType.Scanning, scanningUIMock);
//         Views.Add(ComponentType.End, endHuntUIMock);
//         return Views;
//     }
//     
//     private XRReferenceImageLibrary _lib;
//     private string _storyText;
//
//     private Mock<IRecognizeWithAssetsBundleStep> _stepMock;
//     
//     [SetUp]
//     public void Init()
//     {
//         _lib = Resources.Load<XRReferenceImageLibrary>("editor/testLibrary");
//         _stepMock = new Mock<IRecognizeWithAssetsBundleStep>();
//         _stepMock.Setup(x => x.GetStepType()).Returns(StepType.RecognizeWithAssetBundle).Verifiable();
//         _stepMock.Setup(x => x.GetStepTitle()).Returns("title");
//         _stepMock.Setup(x => x.GetStoryText()).Returns(_storyText).Verifiable();
//         _stepMock.Setup(x => x.GetImageLibraryReference()).Returns(_lib).Verifiable();
//         _stepMock.Setup(x => x.GetVideoToPlay()).Returns("link").Verifiable();
//         _stepMock.Setup(x => x.GetAnswerData()).Returns(new BooleanAnswerData());
//     }
//
//     [TearDown]
//     public void TearDown()
//     {
//         _lib = null;
//         _storyText = null;
//
//         _stepMock = null;
//     }
//     
//      [Test]
//     public void TestShowAssetInStep_Story_Succeeds()
//     {
//         //Given a RecognizeWithAssetBundleStep with correct config
//         //When we attempt to show the story screen.
//         //Then the story screen is shown.
//         
//         //Setup
//         var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
//         var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
//         HuntHomeComponent huntHomeComponentObjectWithMock = new HuntHomeComponent(huntStartActionsMock.Object, huntStartMock.Object);
//         
//         var storyComponentUIActionsMock = CreateComponentUIActionsMock(true, ComponentType.Story); 
//         var storyActionsMock = new Mock<IStoryComponentActions>();
//         var storyComponentWithMock = new StoryComponent(storyActionsMock.Object, storyComponentUIActionsMock.Object);
//         storyActionsMock.Setup(x => x.Configure(_storyText, It.IsAny<string>())).Verifiable();
//         
//         var endhuntUIMock = CreateComponentUIActionsMock(false, ComponentType.End);
//         var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
//         EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);
//         
//         var scanningCorrectUIMock = CreateComponentUIActionsMock(false, ComponentType.Scanning);
//         var scanningCorrectActionsMock = new Mock<IScanningCorrectDisplayVideoActions>();
//         ScanningCorrectDisplayVideoComponent ScanningHuntComponentObjectWithMock = new ScanningCorrectDisplayVideoComponent(scanningCorrectActionsMock.Object, scanningCorrectUIMock.Object);
//         
//         var tabComponentMock = new Mock<ITabComponent>();
//         tabComponentMock.Setup(x => x.Display(ComponentType.Story)).Verifiable();
//
//         RecognizeWithAssetBundleStepController sut = new RecognizeWithAssetBundleStepController(
//             tabComponentMock.Object,
//             storyComponentWithMock,
//             huntHomeComponentObjectWithMock, 
//             endHuntComponentObjectWithMock,
//             ScanningHuntComponentObjectWithMock);
//   
//         var huntControllerMock = new Mock<IHuntController>();
//         sut.StartStep(_stepMock.Object, huntControllerMock.Object, false);
//         //Act
//         sut.ShowAssetInStep(ComponentType.Story);
//         //Assert
//         storyActionsMock.Verify(x => x.Configure(_storyText, It.IsAny<string>()));
//
//         _stepMock.Verify(x => x.GetStoryText());
//         _stepMock.Verify(x => x.GetStepType());
//         _stepMock.Verify(x => x.GetAnswerData());
//         tabComponentMock.Verify(x => x.Display(ComponentType.Story));
//     }
//  
//  
//     [Test]
//     public void TestShowAssetInStep_End_Suceeds()
//     {
//         //Given a RecognizeWithAssetBundleStep with correct config
//         //When we attempt to show the end screen.
//         //Then the end screen is shown.
//         
//         //Setup
//         var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
//         var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
//         HuntHomeComponent huntHomeComponentObjectWithMock = new HuntHomeComponent(huntStartActionsMock.Object, huntStartMock.Object);
//
//         var storyComponentUIActionsMock = CreateComponentUIActionsMock(true, ComponentType.Story); //past screen, now going to show huntStart
//         var storyActionsMock = new Mock<IStoryComponentActions>();
//         var storyComponentWithMock = new StoryComponent(storyActionsMock.Object, storyComponentUIActionsMock.Object);
//         
//         var endHuntUIActionsMock = CreateComponentUIActionsMock(false, ComponentType.End);
//         var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
//         endHuntActionsMock.Setup(x => x.Configure("",It.IsAny<Action>())).Verifiable();
//         EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endHuntUIActionsMock.Object);
//         
//         var scanningCorrectUIMock = CreateComponentUIActionsMock(false, ComponentType.Scanning);
//         var scanningCorrectActionsMock = new Mock<IScanningCorrectDisplayVideoActions>();
//         ScanningCorrectDisplayVideoComponent ScanningHuntComponentObjectWithMock = new ScanningCorrectDisplayVideoComponent(scanningCorrectActionsMock.Object, scanningCorrectUIMock.Object);
//         
//         var tabComponentMock = new Mock<ITabComponent>();
//         
//         RecognizeWithAssetBundleStepController sut = new RecognizeWithAssetBundleStepController(
//             tabComponentMock.Object,
//             storyComponentWithMock,
//             huntHomeComponentObjectWithMock, 
//             endHuntComponentObjectWithMock,
//             ScanningHuntComponentObjectWithMock);
//         var huntControllerMock = new Mock<IHuntController>();
//         
//         sut.StartStep(_stepMock.Object, huntControllerMock.Object, false);
//         //Act
//         sut.ShowAssetInStep(ComponentType.End);
//         //Assert
//         tabComponentMock.Verify(x => x.Display(ComponentType.End));
//         endHuntActionsMock.Verify(x => x.Configure("",It.IsAny<Action>()));
//     }
//      [Test]
//     public void TestShowAssetInStep_scanning_Suceeds()
//     {
//         //Given a RecognizeWithAssetBundleStep with correct config
//         //When we attempt to show the scanning screen.
//         //Then the scanning screen is shown.
//         
//         //Setup
//         var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
//         var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
//         HuntHomeComponent huntHomeComponentObjectWithMock = new HuntHomeComponent(huntStartActionsMock.Object, huntStartMock.Object);
//
//         var storyComponentUIActionsMock = CreateComponentUIActionsMock(true, ComponentType.Story); //past screen, now going to show huntStart
//         var storyActionsMock = new Mock<IStoryComponentActions>();
//         var storyComponentWithMock = new StoryComponent(storyActionsMock.Object, storyComponentUIActionsMock.Object);
//         
//         var endHuntUIActionsMock = CreateComponentUIActionsMock(false, ComponentType.End);
//         var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
//         EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endHuntUIActionsMock.Object);
//         
//         var scanningCorrectUIMock = CreateComponentUIActionsMock(false, ComponentType.Scanning);
//         var scanningCorrectActionsMock = new Mock<IScanningCorrectDisplayVideoActions>();
//         scanningCorrectActionsMock.Setup(x => x.Configure(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Action>())).Verifiable();
//         ScanningCorrectDisplayVideoComponent ScanningHuntComponentObjectWithMock = new ScanningCorrectDisplayVideoComponent(scanningCorrectActionsMock.Object, scanningCorrectUIMock.Object);
//         
//         var tabComponentMock = new Mock<ITabComponent>();
//         
//         RecognizeWithAssetBundleStepController sut = new RecognizeWithAssetBundleStepController(
//             tabComponentMock.Object,
//             storyComponentWithMock,
//             huntHomeComponentObjectWithMock, 
//             endHuntComponentObjectWithMock,
//             ScanningHuntComponentObjectWithMock);
//         var huntControllerMock = new Mock<IHuntController>();
//         
//         sut.StartStep(_stepMock.Object, huntControllerMock.Object, false);
//         //Act
//         sut.ShowAssetInStep(ComponentType.Scanning);
//         //Assert
//         tabComponentMock.Verify(x => x.Display(ComponentType.Scanning));
//         scanningCorrectActionsMock.Verify(x => x.Configure(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Action>()));
//     }
//     [Test]
//     public void TestShowAssetInStep_NoSuchView_Throws()
//     {
//         //Given a RecognizeWithAssetBundleStep with X and Y screens
//         //When we try to show screen Z
//         //Then the function throws an error
//         //Setup
//         var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
//         var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
//         HuntHomeComponent huntHomeComponentObjectWithMock = new HuntHomeComponent(huntStartActionsMock.Object, huntStartMock.Object);
//         
//         var storyComponentUIActionsMock = CreateComponentUIActionsMock(true, ComponentType.Story); //past screen, now going to show huntStart
//         var storyActionsMock = new Mock<IStoryComponentActions>();
//         var storyComponentWithMock = new StoryComponent(storyActionsMock.Object, storyComponentUIActionsMock.Object);
//
//         var endhuntUIMock = CreateComponentUIActionsMock(false, ComponentType.End);
//         var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
//         EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);
//         
//         var scanningCorrectUIMock = CreateComponentUIActionsMock(false, ComponentType.Scanning);
//         var scanningCorrectActionsMock = new Mock<IScanningCorrectDisplayVideoActions>();
//         ScanningCorrectDisplayVideoComponent scanningHuntComponentObjectWithMock = new ScanningCorrectDisplayVideoComponent(scanningCorrectActionsMock.Object, scanningCorrectUIMock.Object);
//
//         var tabComponentMock = new Mock<ITabComponent>();
//         
//         RecognizeWithAssetBundleStepController sut = new RecognizeWithAssetBundleStepController(
//             tabComponentMock.Object,
//             storyComponentWithMock,
//             huntHomeComponentObjectWithMock, 
//             endHuntComponentObjectWithMock,
//             scanningHuntComponentObjectWithMock);
//         
//         var huntControllerMock = new Mock<IHuntController>();
//         sut.StartStep(_stepMock.Object, huntControllerMock.Object, false);
//         //Act & Assert
//         Assert.Throws<ArgumentException>(() =>sut.ShowAssetInStep(ComponentType.Resolution));
//     }
//     [Test]
//     public void TestConstructor_MissingStory_Throws()
//     {
//         //Given a RecognizeWithAssetBundleStep and a null StoryScreen
//         //When constructor is called
//         //Then the function throws an error
//         
//         //Assert
//         var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
//         var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
//         HuntHomeComponent huntHomeComponentObjectWithMock = 
//             new HuntHomeComponent(huntStartActionsMock.Object, huntStartMock.Object);
//         
//         var endhuntUIMock = CreateComponentUIActionsMock(false, ComponentType.End);
//         var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
//         EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);
//         
//         var scanningCorrectUIMock = CreateComponentUIActionsMock(false, ComponentType.Scanning);
//         var scanningCorrectActionsMock = new Mock<IScanningCorrectDisplayVideoActions>();
//         ScanningCorrectDisplayVideoComponent scanningHuntComponentObjectWithMock = new ScanningCorrectDisplayVideoComponent(scanningCorrectActionsMock.Object, scanningCorrectUIMock.Object);
//
//         //Act & Assert
//         Assert.Throws<NullReferenceException>(() =>new RecognizeWithAssetBundleStepController(
//             null,
//             null,
//             huntHomeComponentObjectWithMock, 
//             endHuntComponentObjectWithMock,
//             scanningHuntComponentObjectWithMock));
//     }
//
//     [Test]
//     public void TestConstructor_MissingScanning_Throws()
//     {   
//         //Given a RecognizeWithAssetBundleStep and a null scanning screen
//         //When constructor is called
//         //Then the function throws an error
//
//         //setup
//         var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
//         var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
//         HuntHomeComponent huntHomeComponentObjectWithMock = new HuntHomeComponent(huntStartActionsMock.Object, huntStartMock.Object);
//
//         var storyComponentUIActionsMock = CreateComponentUIActionsMock(true, ComponentType.Story); //past screen, now going to show huntStart
//         var storyActionsMock = new Mock<IStoryComponentActions>();
//         var storyComponentWithMock = new StoryComponent(storyActionsMock.Object, storyComponentUIActionsMock.Object);
//         
//         var endhuntUIMock = CreateComponentUIActionsMock(false, ComponentType.End);
//         var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
//         EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);
//
//         //Act & Assert
//         Assert.Throws<NullReferenceException>(() => new RecognizeWithAssetBundleStepController(
//             null,
//             storyComponentWithMock,
//             huntHomeComponentObjectWithMock, 
//             endHuntComponentObjectWithMock,
//             null));
//     }
//    
//     [Test]
//     public void TestShowAssetInStep_WrongHuntStep_Throws()
//     {
//         //Given a RecognizeWithAssetBundleStepController and a wrong huntstep
//         //When showAssetInStep is called
//         //Then the function throws an error
//         
//         //Setup
//         var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
//         var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
//         HuntHomeComponent huntHomeComponentObjectWithMock = 
//             new HuntHomeComponent(huntStartActionsMock.Object, huntStartMock.Object);
//
//         var storyComponentUIActionsMock = CreateComponentUIActionsMock(true, ComponentType.Story); //past screen, now going to show huntStart
//         var storyActionsMock = new Mock<IStoryComponentActions>();
//         var storyComponentWithMock = new StoryComponent(storyActionsMock.Object, storyComponentUIActionsMock.Object);
//
//         var endhuntUIMock = CreateComponentUIActionsMock(false, ComponentType.End);
//         var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
//         EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);
//         
//         var scanningCorrectUIMock = CreateComponentUIActionsMock(false, ComponentType.Scanning);
//         var scanningCorrectActionsMock = new Mock<IScanningCorrectDisplayVideoActions>();
//         ScanningCorrectDisplayVideoComponent scanningHuntComponentObjectWithMock = new ScanningCorrectDisplayVideoComponent(scanningCorrectActionsMock.Object, scanningCorrectUIMock.Object);
//
//         var tabComponentMock = new Mock<ITabComponent>();
//         var sut = new RecognizeWithAssetBundleStepController(
//                 tabComponentMock.Object,
//                 storyComponentWithMock,
//                 huntHomeComponentObjectWithMock, 
//                 endHuntComponentObjectWithMock,
//                 scanningHuntComponentObjectWithMock
//         );
//         var huntControllerMock = new Mock<IHuntController>();
//
//         _stepMock.Setup(x => x.GetStepType()).Returns(StepType.DisplayStoryAndDone).Verifiable();
//         //Act & Assert
//         Assert.Throws<ArgumentException>(()=>sut.StartStep(_stepMock.Object, huntControllerMock.Object, false));
//         Assert.Throws<ArgumentException>(() => sut.ShowAssetInStep(ComponentType.Story));
//         _stepMock.Verify(x => x.GetStepType());
//     }
//     [Test]
//     public void TestStartStep_Succeeds()
//     {
//         //Given a RecognizeWithAssetBundleStepController and a huntstep.
//         //When StartStep is called
//         //Then the function shows the first screen.
//         
//         //Setup
//         _stepMock.Setup(x => x.GetStoryText()).Returns(_storyText).Verifiable();
//         var answerDataMock = new Mock<IAnswerData>();
//         answerDataMock.Setup(x => x.HasAnswer()).Returns(false);
//         _stepMock.Setup(x => x.GetAnswerData()).Returns(answerDataMock.Object);
//         var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
//         var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
//         HuntHomeComponent huntHomeComponentObjectWithMock = 
//             new HuntHomeComponent (huntStartActionsMock.Object, huntStartMock.Object);
//
//         var storyComponentUIActionsMock = CreateComponentUIActionsMock(true, ComponentType.Story); //past screen, now going to show huntStart
//         var storyActionsMock = new Mock<IStoryComponentActions>();
//         var storyComponentWithMock = new StoryComponent(storyActionsMock.Object, storyComponentUIActionsMock.Object);
//         storyActionsMock.Setup(x => x.Configure(_storyText, It.IsAny<string>())).Verifiable();
//
//         var endhuntUIMock = CreateComponentUIActionsMock(false, ComponentType.End);
//         var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
//         EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);
//         
//         var scanningCorrectUIMock = CreateComponentUIActionsMock(false, ComponentType.Scanning);
//         var scanningCorrectActionsMock = new Mock<IScanningCorrectDisplayVideoActions>();
//         ScanningCorrectDisplayVideoComponent scanningHuntComponentObjectWithMock = new ScanningCorrectDisplayVideoComponent(scanningCorrectActionsMock.Object, scanningCorrectUIMock.Object);
//
//         var huntControllerMock = new Mock<IHuntController>();
//
//         var tabComponentMock = new Mock<ITabComponent>();
//         RecognizeWithAssetBundleStepController sut = new RecognizeWithAssetBundleStepController(
//             tabComponentMock.Object,
//             storyComponentWithMock,
//             huntHomeComponentObjectWithMock,
//             endHuntComponentObjectWithMock,
//             scanningHuntComponentObjectWithMock);
//         tabComponentMock.Setup(x => x.ConfigureForStepType(sut)).Verifiable();
//         
//         //Act 
//         sut.StartStep(_stepMock.Object, huntControllerMock.Object, false);
//         
//         //Assert
//         Assert.IsNotNull(sut.HuntStep);
//         storyActionsMock.Verify(x => x.Configure(_storyText, It.IsAny<string>()));
//         _stepMock.Verify(x => x.GetStoryText());
//         tabComponentMock.Verify(x => x.ConfigureForStepType(sut));
//     } 
//     [Test]
//     public void TestStartStep_Throws()
//     {
//         //Given a recognizeImageAndPlayVideoStepController and a null huntstep.
//         //When StartStep is called
//         //Then the function throws an error
//         
//         //Setup
//         var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
//         var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
//         HuntHomeComponent huntHomeComponentObjectWithMock = 
//             new HuntHomeComponent(huntStartActionsMock.Object, huntStartMock.Object);
//
//         var storyComponentUIActionsMock = CreateComponentUIActionsMock(true, ComponentType.Story); //past screen, now going to show huntStart
//         var storyActionsMock = new Mock<IStoryComponentActions>();
//         var storyComponentWithMock = new StoryComponent(storyActionsMock.Object, storyComponentUIActionsMock.Object);
//
//         var endhuntUIMock = CreateComponentUIActionsMock(false, ComponentType.End);
//         var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
//         EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);
//         
//         var scanningCorrectUIMock = CreateComponentUIActionsMock(false, ComponentType.Scanning);
//         var scanningCorrectActionsMock = new Mock<IScanningCorrectDisplayVideoActions>();
//         ScanningCorrectDisplayVideoComponent scanningHuntComponentObjectWithMock = new ScanningCorrectDisplayVideoComponent(scanningCorrectActionsMock.Object, scanningCorrectUIMock.Object);
//
//         var sut = new RecognizeWithAssetBundleStepController(
//             null,
//             storyComponentWithMock,
//             huntHomeComponentObjectWithMock,
//             endHuntComponentObjectWithMock,
//             scanningHuntComponentObjectWithMock);
//         //Act & Assert
//         Assert.Throws<ArgumentException>(() => sut.StartStep(null, null, false));
//     }
//     [Test]
//     public void TestStartStep_SecondRun_ShowsScanning_And_RecordedAnswer()
//     {
//         //Given a RecognizeWithAssetBundleStep with a recorded bool answer
//         //When we start the step
//         //Then the riddle screen is shown.
//         
//         //Setup
//
//         var answerData = new BooleanAnswerData();
//         answerData.SetAnswer(true);
//         _stepMock.Setup(x => x.GetAnswerData()).Returns(answerData);
//
//         var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
//         var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
//         HuntHomeComponent huntHomeComponentObjectWithMock = new HuntHomeComponent(huntStartActionsMock.Object, huntStartMock.Object);
//         
//         var storyComponentUIActionsMock = CreateComponentUIActionsMock(false, ComponentType.Story); 
//         var storyActionsMock = new Mock<IStoryComponentActions>();
//         var storyComponentWithMock = new StoryComponent(storyActionsMock.Object, storyComponentUIActionsMock.Object);
//         
//         var scanningCorrectUIMock = CreateComponentUIActionsMock(false, ComponentType.Scanning);
//         var scanningCorrectActionsMock = new Mock<IScanningCorrectDisplayVideoActions>();
//         ScanningCorrectDisplayVideoComponent scanningHuntComponentObjectWithMock = new ScanningCorrectDisplayVideoComponent(scanningCorrectActionsMock.Object, scanningCorrectUIMock.Object);
//
//         var endhuntUIMock = CreateComponentUIActionsMock(true, ComponentType.End);
//         var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
//         EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);
//         
//         var tabComponentMock = new Mock<ITabComponent>();
//         tabComponentMock.Setup(x => x.Display(ComponentType.Scanning)).Verifiable();
//         RecognizeWithAssetBundleStepController sut = new RecognizeWithAssetBundleStepController(
//             tabComponentMock.Object,
//             storyComponentWithMock,
//             huntHomeComponentObjectWithMock, 
//             endHuntComponentObjectWithMock,
//             scanningHuntComponentObjectWithMock);
//
//         var huntControllerMock = new Mock<IHuntController>();
//         //Act
//         sut.StartStep(_stepMock.Object, huntControllerMock.Object, false);
//         
//         //Assert
//         tabComponentMock.Verify(x => x.Display(ComponentType.Scanning));
//
//     }
//     [Test]
//     public void TestGetTypesInOrder()
//     {
//         var tabComponentMock = new Mock<ITabComponent>();
//         var homeComponentMock = new Mock<IHuntHomeComponent>();
//         homeComponentMock.Setup(x => x.GetComponentUIActions())
//             .Returns(CreateComponentUIActionsMock(false, ComponentType.HuntHome).Object);
//         
//         var storyComponentMock = new Mock<IStoryComponent>();
//         storyComponentMock.Setup(x => x.GetComponentUIActions())
//             .Returns(CreateComponentUIActionsMock(false, ComponentType.Story).Object);
//         
//         var scanningComponentMock = new Mock<IScanningCorrectDisplayVideoComponent>();
//         scanningComponentMock.Setup(x => x.GetComponentUIActions())
//             .Returns(CreateComponentUIActionsMock(false, ComponentType.Scanning).Object);
//         
//         var endHuntComponentMock = new Mock<IEndHuntComponent>();
//         endHuntComponentMock.Setup(x => x.GetComponentUIActions())
//             .Returns(CreateComponentUIActionsMock(false, ComponentType.End).Object);
//         
//         RecognizeWithAssetBundleStepController sut = new RecognizeWithAssetBundleStepController(
//             tabComponentMock.Object,
//             storyComponentMock.Object,
//             homeComponentMock.Object,
//             endHuntComponentMock.Object,
//             scanningComponentMock.Object);
//
//         var componentOrder = sut.GetTypesInOrder();
//         Assert.AreEqual(ComponentType.Story, componentOrder[0]);
//         Assert.AreEqual(ComponentType.Scanning, componentOrder[1]);
//         Assert.AreEqual(2, componentOrder.Count);
//     }
//     [Test]
//     public void TestGetFirstStepTypeToShow()
//     {
//         var tabComponentMock = new Mock<ITabComponent>();
//         
//         var homeComponentMock = new Mock<IHuntHomeComponent>();
//         homeComponentMock.Setup(x => x.GetComponentUIActions())
//             .Returns(CreateComponentUIActionsMock(false, ComponentType.HuntHome).Object);
//         
//         var storyComponentMock = new Mock<IStoryComponent>();
//         storyComponentMock.Setup(x => x.GetComponentUIActions())
//             .Returns(CreateComponentUIActionsMock(false, ComponentType.Story).Object);
//         
//         var scanningComponentMock = new Mock<IScanningCorrectDisplayVideoComponent>();
//         scanningComponentMock.Setup(x => x.GetComponentUIActions())
//             .Returns(CreateComponentUIActionsMock(false, ComponentType.Scanning).Object);
//         
//         var endHuntComponentMock = new Mock<IEndHuntComponent>();
//         endHuntComponentMock.Setup(x => x.GetComponentUIActions())
//             .Returns(CreateComponentUIActionsMock(false, ComponentType.End).Object);
//         
//         RecognizeWithAssetBundleStepController sut = new RecognizeWithAssetBundleStepController(
//             tabComponentMock.Object,
//             storyComponentMock.Object,
//             homeComponentMock.Object,
//             endHuntComponentMock.Object,
//             scanningComponentMock.Object);
//
//         Assert.AreEqual(ComponentType.Story, sut.GetFirstStepTypeToShow());
//     }
// }
