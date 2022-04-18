using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using UnityEngine;
//TODO: AR Element
// using UnityEngine.XR.ARSubsystems;

[TestFixture]
public class TestChristmasHuntController 
{
    //TODO: AR Element
    // public XRReferenceImageLibrary lib;

    [SetUp]
    public void Init()
    {
        //collects imagelibrary asset file from streaming assets; if null, then the file is missing.
        //TODO: AR Element
        // lib = Resources.Load<XRReferenceImageLibrary>("editor/testLibrary");
    }
    StoryComponent CreateStory(Mock<IStoryComponentActions> storyActionsMock = null, Mock<IViewActions> storyUIMock = null)
    {
        storyUIMock ??= new Mock<IViewActions>();
        storyActionsMock ??= new Mock<IStoryComponentActions>();
        
        StoryComponent storyMock = new StoryComponent(storyActionsMock.Object, storyUIMock.Object);
        return storyMock;
    }
    EndHuntComponent CreateEndHunt(Mock<IEndHuntComponentActions> endHuntActionsMock = null, Mock<IViewActions> endHuntUIMock = null)
    {
        endHuntUIMock ??= new Mock<IViewActions>();
        endHuntActionsMock ??= new Mock<IEndHuntComponentActions>();
        EndHuntComponent endHuntComponentMock = new EndHuntComponent(endHuntActionsMock.Object, endHuntUIMock.Object);
        return endHuntComponentMock;
    }
    ScanningCorrectDisplayVideoComponent CreateScanningCorrectDisplayVideo(Mock<IScanningCorrectDisplayVideoActions> scanningActionsMock = null, Mock<IViewActions> scanningUIMock = null)
    {
        scanningUIMock ??= new Mock<IViewActions>();
        scanningActionsMock ??= new Mock<IScanningCorrectDisplayVideoActions>();
        ScanningCorrectDisplayVideoComponent scanningCorrectMock = new ScanningCorrectDisplayVideoComponent(scanningActionsMock.Object, scanningUIMock.Object);
        return scanningCorrectMock;
    }
    HuntHomeComponent CreateHuntStart(Mock<IHuntHomeComponentActions> huntStartActionsMock = null, Mock<IViewActions> huntStartUIMock = null)
    {
        huntStartUIMock ??= new Mock<IViewActions>();
        huntStartActionsMock ??= new Mock<IHuntHomeComponentActions>();
        HuntHomeComponent huntHomeComponentMock = new HuntHomeComponent(huntStartActionsMock.Object, huntStartUIMock.Object);
        return huntHomeComponentMock;
    }
    IOldStepController CreateDisplayStoryAndDoneController(StoryComponent storyMock, HuntHomeComponent huntHomeComponentMock, EndHuntComponent endHuntComponentMock)
    {
        var tabComponentMock = new Mock<ITabComponent>();
        IOldStepController displayStoryAndDoneOldStepController = 
            new DisplayStoryAndDoneOldStepController(tabComponentMock.Object, storyMock, huntHomeComponentMock, endHuntComponentMock);
        return displayStoryAndDoneOldStepController;
    }
    IOldStepController CreateDisplayAndSumbitAnswerStepController(StoryComponent storyMock, RiddleTabComponent riddleTabComponent, HuntHomeComponent huntHomeComponentMock, EndHuntComponent endHuntComponentMock)
    {
        var tabComponentMock = new Mock<ITabComponent>();
        IOldStepController displayStoryAndDoneOldStepController = 
            new DisplayStoryAndSubmitAnswerOldStepController(tabComponentMock.Object, storyMock, riddleTabComponent, huntHomeComponentMock, endHuntComponentMock);
        return displayStoryAndDoneOldStepController;
    }

    DisplayStoryAndDoneHuntStep createDisplayStoryAndDoneHuntStep(string storyText, string stepId)
    {
        var displayStoryData = new InternalDisplayStoryAndDoneHuntStep();
        displayStoryData.StoryText = storyText;
        displayStoryData.StepId = stepId;
        DisplayStoryAndDoneHuntStep huntStep = new DisplayStoryAndDoneHuntStep(displayStoryData);
        return huntStep;
    }

    [Test]
    public void TestHuntController_NoActiveHunt()
    {
        //Given no hunt started
        //When the user attempts to get the current step.
        //Then the no data is initialized/ready for use.
        var sut = new ChristmasChristmasHuntController();
        AssertHuntIsntActive(sut);
    }

    [Test]
    public void TestConfigureHunt_NoHuntActive_Succeeds()
    {
        //Given valid huntData and a huntStart
        //When ConfigureHunt is called
        //Then the huntStart UI is displayed.
        
        //Setup
        string StoryText = "StoryText";
        var Story = CreateStory();
        var end = CreateEndHunt();
        var scanning = CreateScanningCorrectDisplayVideo();
        
        var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
        var huntStart = CreateHuntStart(huntStartActionsMock);

        var displayStoryAndDoneStepController = CreateDisplayStoryAndDoneController(Story, huntStart, end);
        Dictionary<StepType, IOldStepController> stepControllers = new Dictionary<StepType, IOldStepController>();
        stepControllers.Add(StepType.DisplayStoryAndDone, displayStoryAndDoneStepController);

        var huntDataMock = new Mock<IHuntSteps>();
        var StoryStep = createDisplayStoryAndDoneHuntStep(StoryText, "0");
        huntDataMock.Setup(x => x.GetElement(0)).Returns(StoryStep).Verifiable();
        huntDataMock.Setup(x => x.GetLengthOfHunt()).Returns(1).Verifiable();

        //Act
        var sut = new ChristmasChristmasHuntController();
        sut.ConfigureHunt(huntDataMock.Object, stepControllers, null);
        
        //Assert
        Assert.IsNotNull(sut.CurrentHuntSteps);
        Assert.IsNotNull(sut.StepControllers);
        Assert.IsNotNull(sut.GetStepData());
    }
    
    [Test]
    public void TestConfigureAndStartHunt_HuntAlreadyActive_Throws()
    {
        //Given valid huntData and a huntStart
        //When ConfigureHunt is called twice, before the end function has been called.
        //Then huntController throws an error.
        
        //Setup
        string StoryText = "StoryText";
        var Story = CreateStory();
        var end = CreateEndHunt();
        var scanning = CreateScanningCorrectDisplayVideo();
        
        var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
        // var arCamera = new Mock<IMorphableARCameraActions>(); //TODO: AR Element
        // arCamera.Setup(x => x.PrepARCamera(lib, It.IsAny<Action>())); //TODO: AR Element
        // huntStartActionsMock.Setup(x => //TODO: AR Element
        //         x.ConfigureARElements(arCamera.Object, lib, It.IsAny<Action>()))
        //     .Callback<IMorphableARCameraActions,XRReferenceImageLibrary, Action>((_arCamera, huntStep, foundTarget) =>
        //     {
        //         foundTarget.Invoke();
        //     });

        //TODO: AR Element - DELETE ME
        huntStartActionsMock.Setup(x =>
                x.ConfigureARElements(It.IsAny<Action>()))
            .Callback<Action>((foundTarget) =>
            {
                foundTarget.Invoke();
            });
        //TODO: AR Element - END DELETE
        
        var huntStart = CreateHuntStart(huntStartActionsMock);

        var displayStoryAndDoneStepController = CreateDisplayStoryAndDoneController(Story, huntStart, end);
        Dictionary<StepType, IOldStepController> stepControllers = new Dictionary<StepType, IOldStepController>();
        stepControllers.Add(StepType.DisplayStoryAndDone, displayStoryAndDoneStepController);

        var huntDataMock = new Mock<IHuntSteps>();
        var StoryStep = createDisplayStoryAndDoneHuntStep(StoryText, "0");
        huntDataMock.Setup(x => x.GetElement(0)).Returns(StoryStep);
        huntDataMock.Setup(x => x.GetLengthOfHunt()).Returns(1);
        
        var sut = new ChristmasChristmasHuntController();
        
        //Act
        sut.ConfigureHunt(huntDataMock.Object, stepControllers, null);
        
        //ASSERT
        Assert.Throws<ArgumentException>(() => sut.ConfigureHunt(huntDataMock.Object, stepControllers, null));
    }
    
    [Test]
    public void TestGetCurrentStepData()
    {
        //Given a configuredHuntController
        //When GetCurrentStepData is called
        //Then returns the current IHuntStep instance
        
        //Setup
        
        string StoryText = "StoryText";
        Action foundTarget = () => { };
        var Story = CreateStory();
        var end = CreateEndHunt();
        var scanning = CreateScanningCorrectDisplayVideo();
        
        var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
        // var arCamera = new Mock<IMorphableARCameraActions>(); //TODO: AR ELEMENT
        // arCamera.Setup(x => x.PrepARCamera(lib, foundTarget)); //TODO: AR ELEMENT

        // huntStartActionsMock.Setup(x =>
        //         x.ConfigureARElements(arCamera.Object, lib, It.IsAny<Action>()))
        //     .Callback<IMorphableARCameraActions, XRReferenceImageLibrary, Action>((_arCamera, huntStep, _foundTarget) =>
        //     {
        //         _foundTarget.Invoke();
        //     });
        
        //TODO: AR ELEMENT - DELETE ME
        huntStartActionsMock.Setup(x =>
                x.ConfigureARElements(It.IsAny<Action>()))
            .Callback<Action>((_foundTarget) =>
            {
                _foundTarget.Invoke();
            });
        //TODO: AR ELEMENT - DELETE END

        var huntStart = CreateHuntStart(huntStartActionsMock);

        var displayStoryAndDoneStepController = CreateDisplayStoryAndDoneController(Story, huntStart, end);
        Dictionary<StepType, IOldStepController> stepControllers = new Dictionary<StepType, IOldStepController>();
        stepControllers.Add(StepType.DisplayStoryAndDone, displayStoryAndDoneStepController);

        var huntDataMock = new Mock<IHuntSteps>();
        var StoryStep = createDisplayStoryAndDoneHuntStep(StoryText, "0");
        huntDataMock.Setup(x => x.GetElement(0)).Returns(StoryStep).Verifiable();
        huntDataMock.Setup(x => x.GetLengthOfHunt()).Returns(1).Verifiable();
        
        var sut = new ChristmasChristmasHuntController();
        sut.ConfigureHunt(huntDataMock.Object, stepControllers, null);
        
        //Act
        var stepData = sut.GetStepData();
        //Assert
        Assert.AreEqual(StoryStep, stepData);
        huntDataMock.Verify(x => x.GetElement(0));
    }

    [Test]
    public void TestConfigureNextRiddle_StartSpecificStep()
    {
        //Given existing riddlehunt
        //When configureNextRiddle is called
        //Then the StoryUI is called
        //Setup
        string storyText = "StoryText";
        var storyStep = createDisplayStoryAndDoneHuntStep(storyText, "0");
        var storyStep2 = createDisplayStoryAndDoneHuntStep(storyText+"1", "1");

        var storyActionsMock = new Mock<IStoryComponentActions>();

        var story = CreateStory(storyActionsMock);
        var end = CreateEndHunt();
        var scanning = CreateScanningCorrectDisplayVideo();
        var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
        var huntStart = CreateHuntStart(huntStartActionsMock);

        var displayStoryAndDoneStepController = CreateDisplayStoryAndDoneController(story, huntStart, end);
        Dictionary<StepType, IOldStepController> stepControllers = new Dictionary<StepType, IOldStepController>();
        stepControllers.Add(StepType.DisplayStoryAndDone, displayStoryAndDoneStepController);

        var huntDataMock = new Mock<IHuntSteps>();
        huntDataMock.Setup(x => x.GetElement("0")).Returns(storyStep).Verifiable();
        huntDataMock.Setup(x => x.GetElement("1")).Returns(storyStep2).Verifiable();
        huntDataMock.Setup(x => x.GetLengthOfHunt()).Returns(2).Verifiable();
        
        var sut = new ChristmasChristmasHuntController();
        sut.ConfigureHunt(huntDataMock.Object, stepControllers, null);
        
        //Act
        sut.ConfigureRiddle("1");
        
        //ASSERT
        huntDataMock.Verify(x => x.GetElement("1"));
    }
    
    [Test]
    public void TestConfigureNextRiddle_NoSuchStep_Throws()
    {
        //Given existing riddlehunt
        //When configureNextRiddle is called
        //Then the StoryUI is called
        //Setup
        string storyText = "StoryText";
        var storyStep = createDisplayStoryAndDoneHuntStep(storyText, "0");

        var storyActionsMock = new Mock<IStoryComponentActions>();

        var story = CreateStory(storyActionsMock);
        var end = CreateEndHunt();
        var scanning = CreateScanningCorrectDisplayVideo();
        var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
        var huntStart = CreateHuntStart(huntStartActionsMock);

        var displayStoryAndDoneStepController = CreateDisplayStoryAndDoneController(story, huntStart, end);
        Dictionary<StepType, IOldStepController> stepControllers = new Dictionary<StepType, IOldStepController>();
        stepControllers.Add(StepType.DisplayStoryAndDone, displayStoryAndDoneStepController);

        var huntDataMock = new Mock<IHuntSteps>();
        huntDataMock.Setup(x => x.GetElement(0)).Returns(storyStep).Verifiable();
        var sut = new ChristmasChristmasHuntController();
        sut.ConfigureHunt(huntDataMock.Object, stepControllers, null);
        
        //Act & Assert
        Assert.Throws<ArgumentException>(() => sut.ConfigureRiddle("5"));
    }

    [Test]
    public void TestEndHunt()
    {
        //Given a user has an active hunt
        //When hunt is ended
        //Then the HuntController goes back to pre-config.
        
        //Setup
        string storyText = "StoryText";
        string testUrl = "https://some.url";
        Action foundTarget = () => { };
        var story = CreateStory();
        var end = CreateEndHunt();
        var scanning = CreateScanningCorrectDisplayVideo();

        var iHuntStepsMock = new Mock<IHuntSteps>();
        iHuntStepsMock.Setup(x => x.GetFeedbackLink()).Returns(testUrl);
        
        var iHuntComponentUIActionsMock = new Mock<IViewActions>();
        iHuntComponentUIActionsMock.Setup(x => x.Display()).Verifiable();
     
        var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
        // var arCamera = new Mock<IMorphableARCameraActions>(); //TODO: AR ELEMENT
        // arCamera.Setup(x => x.PrepARCamera(lib, foundTarget)); //TODO: AR ELEMENT
       
        var huntStart = CreateHuntStart(huntStartActionsMock);

        var displayStoryAndDoneStepController = CreateDisplayStoryAndDoneController(story, huntStart, end);
        Dictionary<StepType, IOldStepController> stepControllers = new Dictionary<StepType, IOldStepController>();
        stepControllers.Add(StepType.DisplayStoryAndDone, displayStoryAndDoneStepController);

        var huntDataMock = new Mock<IHuntSteps>();
        var storyStep = createDisplayStoryAndDoneHuntStep(storyText, "0");
        huntDataMock.Setup(x => x.GetElement(0)).Returns(storyStep).Verifiable();
        huntDataMock.Setup(x => x.GetLengthOfHunt()).Returns(1).Verifiable();
        
        var sut = new ChristmasChristmasHuntController();

        Action huntHasEnded = () => { };
        sut.ConfigureHunt(huntDataMock.Object, stepControllers, huntHasEnded);

        //Act
        sut.EndHunt(true);
        
        //Assert
        AssertHuntIsntActive(sut);
    }

    /// <summary>
    /// Moved this to reuse the same code for TestEndHunt as in TestNoActiveHunt.
    /// </summary>
    /// <param name="sut">HuntFlowHandler</param>
    public void AssertHuntIsntActive(ChristmasChristmasHuntController sut)
    {
        //Assert
        Assert.IsNull(sut.CurrentHuntSteps);
        Assert.IsNull(sut.StepControllers);
        Assert.IsNull(sut.GetStepData());
    }

    [Test]
    public void TestConfigureHunt_noStepData_NoStepController_Throws()
    {
        //Given a hunt is starting without a huntDataCollection or stepcontroller
        //When the user attempts to start a new hunt.
        //Then the HuntController throws and exception.
        
        //Setup
        var sut = new ChristmasChristmasHuntController();
        //act & assert
        Assert.Throws<ArgumentException>(() =>sut.ConfigureHunt(null, null, null));
    }
    [Test]
    public void TestConfigureHunt_NoStepController_Throws()
    {
        //Given a hunt is starting without a stepcontroller
        //When the user attempts to start a new hunt.
        //Then the HuntController throws an exception.
        
        //Setup
        var huntDataMock = new Mock<IHuntSteps>();
        var sut = new ChristmasChristmasHuntController();
        //act & assert
        Assert.Throws<ArgumentException>(() =>sut.ConfigureHunt(huntDataMock.Object, null, null));
    }
    [Test]
    public void TestConfigureHunt_EmptyStepController_Throws()
    {
        //Given a hunt is starting with an empty stepController
        //When the user attempts to start a new hunt.
        //Then the HuntController throws an exception.
        
        //Setup
        var huntDataMock = new Mock<IHuntSteps>();
        var dictMock = new Dictionary<StepType, IOldStepController>();
        var sut = new ChristmasChristmasHuntController();
        //act & assert
        Assert.Throws<ArgumentException>(() =>sut.ConfigureHunt(huntDataMock.Object, dictMock, null));
    }
}
