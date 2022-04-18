using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;
using riddlehouse_libraries.products.Assets;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using UnityEngine;

public class TestHuntResolutionAndEndStepController
{
    private List<Sprite> _imageList;

    private Mock<ITabComponent> _iTabComponentMock;
    
    private Mock<IViewActions> _iStoryUIActionsMock;
    private Mock<IViewActions> _iRiddleTabUIActionsMock;
    private Mock<IViewActions> _iHuntHomeUIActionsMock;
    private Mock<IViewActions> _iEndHuntUIActionsMock;
    private Mock<IViewActions> _iResolutionUIActionsMock;

    private Mock<IStoryComponent> _iStoryComponent;
    private Mock<IRiddleTabComponent> _iRiddleTabComponent;
    private Mock<IHuntHomeComponent> _iHuntHomeComponent;
    private Mock<IEndHuntComponent> _iEndHuntComponent;
    private Mock<IResolutionComponent> _iResolutionComponent;

    private HuntResolutionAndEndOldStepController _oldStepController;

    private Mock<IHuntResolutionAndEndStep> _huntResolutionAndEndStepMock;

    private StepType _expectedStepType = StepType.HuntResolutionAndEnd;
    
    private string _stepTitle;
    private string _storyText;
    private string _riddleText;
    private string _endText;
    private string _videoUrl;
    
    private Mock<IAnswerAsset> _iAnswerData;

    private Mock<IChristmasHuntController> _iHuntController;
    
    public Mock<IViewActions> CreateUIAction(ComponentType type)
    {
        var UIActions = new Mock<IViewActions>();
        UIActions.Setup(x => x.GetComponentType()).Returns(type).Verifiable();
        return UIActions;
    }

    [SetUp]
    public void Init()
    {
        _iTabComponentMock = new Mock<ITabComponent>();

        _iStoryUIActionsMock = CreateUIAction(ComponentType.Story);
        _iHuntHomeUIActionsMock = CreateUIAction(ComponentType.HuntHome);
        _iEndHuntUIActionsMock = CreateUIAction(ComponentType.End);
        _iResolutionUIActionsMock = CreateUIAction(ComponentType.Resolution);

        _iStoryComponent = new Mock<IStoryComponent>();
        _iStoryComponent.Setup(x => x.GetComponentUIActions()).Returns(_iStoryUIActionsMock.Object).Verifiable();
        _iRiddleTabComponent = new Mock<IRiddleTabComponent>();
        _iRiddleTabComponent.Setup(x => x.GetComponentType()).Returns(ComponentType.RiddleTab).Verifiable();
        _iHuntHomeComponent = new Mock<IHuntHomeComponent>();
        _iHuntHomeComponent.Setup(x => x.GetComponentUIActions()).Returns(_iHuntHomeUIActionsMock.Object).Verifiable();
        _iEndHuntComponent = new Mock<IEndHuntComponent>();
        _iEndHuntComponent.Setup(x => x.GetComponentUIActions()).Returns(_iEndHuntUIActionsMock.Object).Verifiable();
        _iResolutionComponent = new Mock<IResolutionComponent>();
        _iResolutionComponent.Setup(x => x.GetComponentUIActions()).Returns(_iResolutionUIActionsMock.Object).Verifiable();
        
        _oldStepController = new HuntResolutionAndEndOldStepController(
            _iTabComponentMock.Object, 
            _iStoryComponent.Object, 
            _iRiddleTabComponent.Object, 
            _iHuntHomeComponent.Object, 
            _iEndHuntComponent.Object,
            _iResolutionComponent.Object
        );

        _stepTitle = "title";
        _storyText = "storyText";
        _riddleText = "riddleText";
        _endText = "endText";
        _videoUrl = "videoText";
        
        _iAnswerData = new Mock<IAnswerAsset>();
        
        _imageList = new List<Sprite>();

        _huntResolutionAndEndStepMock = new Mock<IHuntResolutionAndEndStep>();
        _huntResolutionAndEndStepMock.Setup(x=>x.GetStepTitle()).Returns(_stepTitle).Verifiable();
        _huntResolutionAndEndStepMock.Setup(x => x.GetStepType()).Returns(_expectedStepType);
        _huntResolutionAndEndStepMock.Setup(x => x.GetStoryText()).Returns(_storyText).Verifiable();
        _huntResolutionAndEndStepMock.Setup(x => x.GetRiddleText()).Returns(_riddleText).Verifiable();
        _huntResolutionAndEndStepMock.Setup(x=>x.GetAnswerData()).Returns(_iAnswerData.Object).Verifiable();
        _huntResolutionAndEndStepMock.Setup(x => x.GetEndText()).Returns(_endText).Verifiable();
        _huntResolutionAndEndStepMock.Setup(x => x.GetVideoUrl()).Returns(_videoUrl).Verifiable();
        _huntResolutionAndEndStepMock.Setup(x => x.GetRiddleImages()).Returns(_imageList).Verifiable();
        
        _iHuntController = new Mock<IChristmasHuntController>();
        
    }

    [TearDown]
    public void Teardown()
    {
        _iTabComponentMock = null;
        
        _iStoryUIActionsMock = null;
        _iRiddleTabUIActionsMock = null;
        _iEndHuntUIActionsMock = null;

        _oldStepController = null;
        _stepTitle = null;
        _storyText = null;
        _riddleText = null;
        _iAnswerData = null;

        _huntResolutionAndEndStepMock = null;
        
        _iHuntController = null;
    }
    
    [Test]
    public void TestConstructor_Story_Component_Missing()
    {
        //Given the user starts a new hunt.
        //When the HuntResolutionAndEndStepController is created, without a storycomponent
        //Then the system throws a nullReferenceException.
        Assert.Throws<NullReferenceException>(() =>
            new HuntResolutionAndEndOldStepController(
                _iTabComponentMock.Object,
                null,
                _iRiddleTabComponent.Object,
                _iHuntHomeComponent.Object,
                _iEndHuntComponent.Object,
                _iResolutionComponent.Object));
    }
    
    [Test]
    public void TestConstructor_All_Dependencies()
    {
        //Given the user starts a new hunt.
        //When the HuntResolutionAndEndStepController is created, all it's elements.
        //Then the hunt is created.
        Assert.DoesNotThrow(() =>
            new HuntResolutionAndEndOldStepController(
                _iTabComponentMock.Object,
                _iStoryComponent.Object,
                _iRiddleTabComponent.Object,
                _iHuntHomeComponent.Object,
                _iEndHuntComponent.Object,
                _iResolutionComponent.Object));
    }
    
    [Test]
    public void TestStartStep_IncorrectHuntStep_Throws()
    {
        //Given the user starts a step in a hunt, but the system provides the wrong type of huntstep
        //When the step is started
        //Then the system throws and exception.
        
        //Arrange
        var huntStepMock = new Mock<IHuntResolutionAndEndStep>();
        huntStepMock.Setup(x => x.GetStepType()).Returns(StepType.DisplayStoryAndDone);
        var sut = _oldStepController;
        
        //Act and Assert.
        Assert.Throws<ArgumentException>( () => sut.StartStep(huntStepMock.Object, null, false));
        huntStepMock.Verify(x => x.GetStepType());
    }
    
    [Test]
    public void TestStartStep_ShowsFirstAsset_AssetIsShown()
    {
        //Given an existing stepcontroller, with all it's values correctly setup.
        //When step is started.
        //Then it shows the first asset in the step.
        
        //Arrange
        _iTabComponentMock.Setup(x => x.Display(ComponentType.Story)).Verifiable();
        var sut = _oldStepController;
        //Act
        sut.StartStep(_huntResolutionAndEndStepMock.Object, _iHuntController.Object, false);
        //Assert
        _iTabComponentMock.Verify(x => x.Display(sut.GetFirstStepTypeToShow()));
    }

    [TestCase(ComponentType.Story)]
    [TestCase(ComponentType.RiddleTab)]
    [TestCase(ComponentType.End)]
    [TestCase(ComponentType.HuntHome)]
    [TestCase(ComponentType.Resolution)]
    [Test]
    public void TestShowAssetInStep_AssetIsShown(ComponentType type)
    {
        //Given an existing stepcontroller with all it's values correctly setup.
        //When step is started.
        //Then it displays the type passed into the test.
        
        //Arrange
        _iTabComponentMock.Setup(x => x.Display(type)).Verifiable();
        var sut = _oldStepController;
        sut.StartStep(_huntResolutionAndEndStepMock.Object, _iHuntController.Object, false);
        //Act
        sut.ShowAssetInStep(type);
        //Assert
        _iTabComponentMock.Verify(x => x.Display(type));
    }

    [Test]
    public void TestShowAssetInStep_Configures_Story()
    {
        //Given an existing stepcontroller with all it's values correctly setup.
        //When story is displayed.
        //Then it configures the story component
        
        //Arrange
        _iStoryComponent.Setup(x => x.Configure(_storyText, "OK!", It.IsAny<Action>())).Verifiable();
        var sut = _oldStepController;
        sut.StartStep(_huntResolutionAndEndStepMock.Object, _iHuntController.Object, false);
        //Act
        sut.ShowAssetInStep(ComponentType.Story);
        //Assert
        _iStoryComponent.Verify(x => x.Configure(_storyText, "OK!", It.IsAny<Action>()));
    }
    
    [Test]
    public void TestShowAssetInStep_Configures_Riddle()
    {
        //Given an existing stepcontroller with all it's values correctly setup.
        //When riddletab is displayed.
        //Then it configures the riddletab component
        
        //Arrange
        _iRiddleTabComponent.Setup(x => x.Configure(_riddleText, _iAnswerData.Object, _imageList, It.IsAny<Action>())).Verifiable();
        var sut = _oldStepController;
        sut.StartStep(_huntResolutionAndEndStepMock.Object, _iHuntController.Object, false);
        //Act
        sut.ShowAssetInStep(ComponentType.RiddleTab);
        //Assert
        _iRiddleTabComponent.Verify(x => x.Configure(_riddleText, _iAnswerData.Object, _imageList, It.IsAny<Action>()));
    }
    
    [Test]
    public void TestShowAssetInStep_Configures_Resolution()
    {
        //Given an existing stepcontroller with all it's values correctly setup.
        //When Resolution is displayed.
        //Then it configures the Resolution component
        
        //Arrange
        _iResolutionComponent.Setup(x=>x.Configure(It.IsAny<Action>(), _videoUrl)).Verifiable();
        var sut = _oldStepController;
        sut.StartStep(_huntResolutionAndEndStepMock.Object, _iHuntController.Object, false);
        //Act
        sut.ShowAssetInStep(ComponentType.Resolution);
        //Assert
        _iResolutionComponent.Verify(x=>x.Configure(It.IsAny<Action>(), _videoUrl));
        _huntResolutionAndEndStepMock.Verify(x => x.GetVideoUrl());
    }
    
    [Test]
    public void TestShowAssetInStep_Configures_End()
    {
        //Given an existing stepcontroller with all it's values correctly setup.
        //When end is displayed.
        //Then it configures the end component
        
        //Arrange
        _iEndHuntComponent.Setup(x => x.Configure(_endText, It.IsAny<Action>())).Verifiable();
        var sut = _oldStepController;
        sut.StartStep(_huntResolutionAndEndStepMock.Object, _iHuntController.Object, false);
        //Act
        sut.ShowAssetInStep(ComponentType.End);
        //Assert
        _iEndHuntComponent.Verify(x => x.Configure(_endText,It.IsAny<Action>()));
        _huntResolutionAndEndStepMock.Verify(x => x.GetEndText());
    }
    
    [Test]
    public void TestShowAssetInStep_No_Such_Asset_In_Step_Throws()
    {
        //Given an existing stepController, with all it's values correctly setup.
        //When showAssetInStep is called with an incorrect componentType
        //Then the system shows an error.
        
        //Arrange
        var sut = _oldStepController;
        sut.StartStep(_huntResolutionAndEndStepMock.Object, _iHuntController.Object, false);
        //Act and Assert
        Assert.Throws<ArgumentException>( () => sut.ShowAssetInStep(ComponentType.Scanning));
    }

    [Test]
    public void TestEndStep_Is_LastRiddle_ShowsEnd()
    {
        //Given a step has been completed by the user.
        //When it's the lastRiddle.
        //Then the system displays the end screen.
        
        //Arrange
        _iTabComponentMock.Setup(x => x.Display(ComponentType.End)).Verifiable();
        var sut = _oldStepController;
        sut.StartStep(_huntResolutionAndEndStepMock.Object, _iHuntController.Object, true);
        //Act
        sut.EndStep();
        //Assert
        _iTabComponentMock.Verify(x => x.Display(ComponentType.End));
    }
    
    [Test]
    public void TestEndStep_Not_LastRiddle_ShowsHuntHome()
    {
        //Given a step has been completed by the user.
        //When it's not the lastRiddle
        //Then the system displays the huntHome screen.
        
        //Arrange
        _iTabComponentMock.Setup(x => x.Display(ComponentType.HuntHome)).Verifiable();
        var sut = _oldStepController;
        sut.StartStep(_huntResolutionAndEndStepMock.Object, _iHuntController.Object, false);
        //Act
        sut.EndStep();
        //Assert
        _iTabComponentMock.Verify(x => x.Display(ComponentType.HuntHome));
    }

    [Test]
    public void TestGetTypesInOrder()
    {
         //Given a preconfigured HuntResolutionStepController
         //When the GetTypesInOrder is called
         //Then the returned list is equal to that stepController's recipe.
         
         //Arrange
         List<ComponentType> expectedTypesInOrder = new List<ComponentType>()
         {
             ComponentType.Story,
             ComponentType.RiddleTab,
             ComponentType.Resolution
         };
         //Act
         var sut = _oldStepController;
         //Assert
         Assert.AreEqual(expectedTypesInOrder, sut.GetTypesInOrder());
    }
    
    [Test]
    public void GetFirstStepTypeToShow()
    {
        //Given a preconfigured HuntResolutionStepController
        //When the GetTypesInOrder is called
        //Then the returned list is equal to the first value in that stepController's recipe.
        
        //Arrange
        var firstStep = ComponentType.Story; 
        //Act
        var sut = _oldStepController;
        //Assert
        Assert.AreEqual(firstStep, sut.GetFirstStepTypeToShow());
    }
}