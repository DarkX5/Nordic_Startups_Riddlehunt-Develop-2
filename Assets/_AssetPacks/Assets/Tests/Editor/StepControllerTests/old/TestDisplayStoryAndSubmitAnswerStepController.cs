using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;
using riddlehouse_libraries.products.Assets;
using riddlehouse_libraries.products.Assets.AnswerAssets;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using UnityEngine;

public class TestDisplayStoryAndSubmitAnswerStepController
{
    private List<Sprite> _imageList;
    private Mock<ITextGetter> textGetterMock;
    private string String_AnswerAssetUrl;
    private string String_AnswerAssetResponse;
    
    private List<string> _multiChoice_options;
    private string _multiChoice_correctAnswer;
    private string _seperator;
    private string identifier;

    [SetUp]
    public void Init()
    {
        _imageList = new List<Sprite>();
        textGetterMock = new Mock<ITextGetter>();
        
        String_AnswerAssetUrl = "https://www.answerAssetUrl,com";
        String_AnswerAssetResponse = "theAnswer";
        textGetterMock = new Mock<ITextGetter>();
        textGetterMock.Setup(x => x.GetText(String_AnswerAssetUrl, false, It.IsAny<Action<string>>()))
            .Callback<string, bool, Action<string>>((theUrl, theCache, theAction) =>
            {
                theAction(String_AnswerAssetResponse);
            });
        
        _multiChoice_options = new List<string>()
        {
            "1825",
            "1925",
            "1984"
        };
        _multiChoice_correctAnswer = "1825";

        _seperator = ";";
        identifier = "displayStoryAndSubmitAnswer-answer-identifier";
        PlayerPrefs.DeleteKey(identifier);
    }

    [TearDown]
    public void TearDown()
    {
        _imageList = null;
        textGetterMock = null;
    }

   public Dictionary<ComponentType, IViewActions> CreateViews(
        IViewActions huntStartMock,
        IViewActions riddleTabMock,
        IViewActions storyUIMock,
        IViewActions endHuntUIMock)
    {
        var Views = new Dictionary<ComponentType, IViewActions>();
        Views = new Dictionary<ComponentType, IViewActions>() {};
        Views.Add(ComponentType.HuntHome, huntStartMock);
        Views.Add(ComponentType.RiddleTab, riddleTabMock);
        Views.Add(ComponentType.Story, storyUIMock);
        Views.Add(ComponentType.End, endHuntUIMock);
        return Views;
    }

    public Mock<IViewActions> CreateComponentUIActionsMock(bool shown, ComponentType type)
    {
        var mock = new Mock<IViewActions>();
        mock.Setup(x => x.Display()).Verifiable();
        mock.Setup(x => x.IsShown()).Returns(shown).Verifiable();
        mock.Setup(x => x.GetComponentType()).Returns(type).Verifiable();
        return mock;
    }

    public void AttachComponentUIActions(Mock<IRiddleTabComponent> mock, bool shown, ComponentType type)
    {
        mock.Setup(x => x.Display()).Verifiable();
        mock.Setup(x => x.IsShown()).Returns(shown).Verifiable();
        mock.Setup(x => x.GetComponentType()).Returns(type).Verifiable();
    }
    
    [Test]
    public void TestShowAssetInStep_Story_Succeeds()
    {
        //Given a DisplayStoryAndSubmitAnswerStep with correct config
        //When we attempt to show the story screen.
        //Then the story screen is shown.
        //Setup
        string storyText = "storyText";
        string storyTextBtn = "OK!";
        StepType stepType = StepType.DisplayRiddleAndSubmitAnswer;

        var answerData = new StringAnswerData(identifier, textGetterMock.Object, String_AnswerAssetUrl, (ready) => { });
        var displayStoryAndSubmitAnswerHuntStepMock = new Mock<IDisplayRiddleAndSubmitAnswerStep>();
        displayStoryAndSubmitAnswerHuntStepMock.Setup(x => x.GetAnswerData()).Returns(answerData).Verifiable();
        displayStoryAndSubmitAnswerHuntStepMock
            .Setup(x => x.GetStoryText())
            .Returns(storyText).Verifiable();

        displayStoryAndSubmitAnswerHuntStepMock
            .Setup(x => x.GetStepType())
            .Returns(stepType)
            .Verifiable();

        var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
        var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
        HuntHomeComponent huntHomeComponentObjectWithMock = new HuntHomeComponent(huntStartActionsMock.Object, huntStartMock.Object);
        
        var storyComponentUIActionsMock = CreateComponentUIActionsMock(true, ComponentType.Story); 
        var storyActionsMock = new Mock<IStoryComponentActions>();
        var storyComponentWithMock = new StoryComponent(storyActionsMock.Object, storyComponentUIActionsMock.Object);
        storyActionsMock.Setup(x => x.Configure(storyText, storyTextBtn)).Verifiable();
        
        var riddleTabComponentMock = new Mock<IRiddleTabComponent>();
        AttachComponentUIActions(riddleTabComponentMock, true, ComponentType.RiddleTab);
        
        var endhuntUIMock = CreateComponentUIActionsMock(false, ComponentType.End);
        var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
        EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);
        
        var tabComponentMock = new Mock<ITabComponent>();
        tabComponentMock.Setup(x => x.Display(ComponentType.Story)).Verifiable();

        DisplayStoryAndSubmitAnswerOldStepController sut = new DisplayStoryAndSubmitAnswerOldStepController(
            tabComponentMock.Object,
            storyComponentWithMock,
            riddleTabComponentMock.Object,
            huntHomeComponentObjectWithMock, 
            endHuntComponentObjectWithMock);
  
        var huntControllerMock = new Mock<IChristmasHuntController>();
        sut.StartStep(displayStoryAndSubmitAnswerHuntStepMock.Object, huntControllerMock.Object, false);
        //Act
        sut.ShowAssetInStep(ComponentType.Story);
        //Assert
        storyActionsMock.Verify(x => x.Configure(storyText, storyTextBtn));

        displayStoryAndSubmitAnswerHuntStepMock.Verify(x => x.GetStoryText());
        displayStoryAndSubmitAnswerHuntStepMock.Verify(x => x.GetStepType());
        displayStoryAndSubmitAnswerHuntStepMock.Verify(x => x.GetAnswerData());
        tabComponentMock.Verify(x => x.Display(ComponentType.Story));
    }
    [Test]
    public void TestShowAssetInStep_Riddle_With_Text_Answer()
    {
        //Given a DisplayStoryAndSubmitAnswerStep with an text answer type
        //When we attempt to show the riddle screen.
        //Then the riddle screen is shown with that answercomponent.
        
        //Setup
        string riddleText = "riddleText";
        StepType stepType = StepType.DisplayRiddleAndSubmitAnswer;
        var answerData = new StringAnswerData(identifier, textGetterMock.Object, String_AnswerAssetUrl, (ready) => { });
        
        var displayStoryAndSubmitAnswerHuntStepMock = new Mock<IDisplayRiddleAndSubmitAnswerStep>();
        
        displayStoryAndSubmitAnswerHuntStepMock
            .Setup(x => x.GetRiddleText())
            .Returns(riddleText).Verifiable();
        
        displayStoryAndSubmitAnswerHuntStepMock
            .Setup(x => x.GetAnswerData())
            .Returns(answerData).Verifiable();
        
        displayStoryAndSubmitAnswerHuntStepMock.Setup(x => x.GetRiddleImages()).Returns(_imageList).Verifiable();

        displayStoryAndSubmitAnswerHuntStepMock
            .Setup(x => x.GetStepType())
            .Returns(stepType)
            .Verifiable();

        var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
        var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
        HuntHomeComponent huntHomeComponentObjectWithMock = new HuntHomeComponent(huntStartActionsMock.Object, huntStartMock.Object);
        
        var storyComponentUIActionsMock = CreateComponentUIActionsMock(true, ComponentType.Story); 
        var storyActionsMock = new Mock<IStoryComponentActions>();
        var storyComponentWithMock = new StoryComponent(storyActionsMock.Object, storyComponentUIActionsMock.Object);
        
        var riddleTabComponentMock = new Mock<IRiddleTabComponent>();
        AttachComponentUIActions(riddleTabComponentMock, true, ComponentType.RiddleTab);
        riddleTabComponentMock.Setup(x =>
            x.Configure(riddleText, answerData, _imageList, It.IsAny<Action>())).Verifiable();
        
        var endhuntUIMock = CreateComponentUIActionsMock(false, ComponentType.End);
        var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
        EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);
        var tabComponentMock = new Mock<ITabComponent>();
        
        DisplayStoryAndSubmitAnswerOldStepController sut = new DisplayStoryAndSubmitAnswerOldStepController(
            tabComponentMock.Object,
            storyComponentWithMock,
            riddleTabComponentMock.Object,
            huntHomeComponentObjectWithMock, 
            endHuntComponentObjectWithMock);
  
        var huntControllerMock = new Mock<IChristmasHuntController>();
        sut.StartStep(displayStoryAndSubmitAnswerHuntStepMock.Object, huntControllerMock.Object, false);
        
        //Act
        sut.ShowAssetInStep(ComponentType.RiddleTab);
        
        //Assert
        riddleTabComponentMock.Verify(x =>
            x.Configure(riddleText, answerData, _imageList, It.IsAny<Action>()));
        displayStoryAndSubmitAnswerHuntStepMock.Verify(x => x.GetRiddleText());
        displayStoryAndSubmitAnswerHuntStepMock.Verify(x => x.GetStepType());
    }
    [Test]
    public void TestShowAssetInStep_Riddle_With_MultipleChoice_Answer()
    {
        //Given a DisplayStoryAndSubmitAnswerStep with a multiplechoice answer type
        //When we attempt to show the riddle screen.
        //Then the riddle screen is shown with that answercomponent.
        
        //Setup
        string riddleText = "riddleText";
        StepType stepType = StepType.DisplayRiddleAndSubmitAnswer;
        var answerData = new StringMultipleChoiceAnswerData(identifier, _multiChoice_options, _multiChoice_correctAnswer, _seperator, MultipleChoiceLogic.ContainsAll);
        
        var displayStoryAndSubmitAnswerHuntStepMock = new Mock<IDisplayRiddleAndSubmitAnswerStep>();
        
        displayStoryAndSubmitAnswerHuntStepMock
            .Setup(x => x.GetRiddleText())
            .Returns(riddleText).Verifiable();
        
        displayStoryAndSubmitAnswerHuntStepMock
            .Setup(x => x.GetAnswerData())
            .Returns(answerData).Verifiable();

        displayStoryAndSubmitAnswerHuntStepMock.Setup(x => x.GetRiddleImages()).Returns(_imageList).Verifiable();
        
        displayStoryAndSubmitAnswerHuntStepMock
            .Setup(x => x.GetStepType())
            .Returns(stepType)
            .Verifiable();

        var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
        var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
        HuntHomeComponent huntHomeComponentObjectWithMock = new HuntHomeComponent(huntStartActionsMock.Object, huntStartMock.Object);
        
        var storyComponentUIActionsMock = CreateComponentUIActionsMock(true, ComponentType.Story); 
        var storyActionsMock = new Mock<IStoryComponentActions>();
        var storyComponentWithMock = new StoryComponent(storyActionsMock.Object, storyComponentUIActionsMock.Object);
        
        var riddleTabComponentMock = new Mock<IRiddleTabComponent>();
        AttachComponentUIActions(riddleTabComponentMock, true, ComponentType.RiddleTab);
        riddleTabComponentMock.Setup(x =>
            x.Configure(riddleText, answerData, _imageList, It.IsAny<Action>())).Verifiable();
        
        var endhuntUIMock = CreateComponentUIActionsMock(false, ComponentType.End);
        var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
        EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);
        var tabComponentMock = new Mock<ITabComponent>();
        
        DisplayStoryAndSubmitAnswerOldStepController sut = new DisplayStoryAndSubmitAnswerOldStepController(
            tabComponentMock.Object,
            storyComponentWithMock,
            riddleTabComponentMock.Object,
            huntHomeComponentObjectWithMock, 
            endHuntComponentObjectWithMock);
  
        var huntControllerMock = new Mock<IChristmasHuntController>();
        sut.StartStep(displayStoryAndSubmitAnswerHuntStepMock.Object, huntControllerMock.Object, false);
        
        //Act
        sut.ShowAssetInStep(ComponentType.RiddleTab);
        
        //Assert
        riddleTabComponentMock.Verify(x =>
            x.Configure(riddleText, answerData, _imageList, It.IsAny<Action>()));
        displayStoryAndSubmitAnswerHuntStepMock.Verify(x => x.GetRiddleText());
        displayStoryAndSubmitAnswerHuntStepMock.Verify(x => x.GetRiddleImages());
        displayStoryAndSubmitAnswerHuntStepMock.Verify(x => x.GetStepType());
    }
    [Test]
    public void TestShowAssetInStep_End_Suceeds()
    {
        //Given a DisplayStoryAndSubmitAnswerStep with correct config
        //When we attempt to show the end screen.
        //Then the end screen is shown.
        
        //Setup
        var answerData = new StringAnswerData(identifier, textGetterMock.Object, String_AnswerAssetUrl, (ready) => { });
        var displayStoryAndSubmitAnswerHuntStepMock = new Mock<IDisplayRiddleAndSubmitAnswerStep>();
        displayStoryAndSubmitAnswerHuntStepMock.Setup(x => x.GetAnswerData()).Returns(answerData).Verifiable();
        displayStoryAndSubmitAnswerHuntStepMock.Setup(x => x.GetStepType())
            .Returns(StepType.DisplayRiddleAndSubmitAnswer);
        var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
        var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
        HuntHomeComponent huntHomeComponentObjectWithMock = new HuntHomeComponent(huntStartActionsMock.Object, huntStartMock.Object);

        var storyComponentUIActionsMock = CreateComponentUIActionsMock(true, ComponentType.Story); //past screen, now going to show huntStart
        var storyActionsMock = new Mock<IStoryComponentActions>();
        var storyComponentWithMock = new StoryComponent(storyActionsMock.Object, storyComponentUIActionsMock.Object);

        var riddleTabComponentMock = new Mock<IRiddleTabComponent>();
        AttachComponentUIActions(riddleTabComponentMock, true, ComponentType.RiddleTab);

        var endHuntUIActionsMock = CreateComponentUIActionsMock(false, ComponentType.End);
        var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
        endHuntActionsMock.Setup(x => x.Configure("",It.IsAny<Action>())).Verifiable();
        EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endHuntUIActionsMock.Object);
        
        var tabComponentMock = new Mock<ITabComponent>();
        
        DisplayStoryAndSubmitAnswerOldStepController sut = new DisplayStoryAndSubmitAnswerOldStepController(
            tabComponentMock.Object,
            storyComponentWithMock,
            riddleTabComponentMock.Object,
            huntHomeComponentObjectWithMock, 
            endHuntComponentObjectWithMock);
        var huntControllerMock = new Mock<IChristmasHuntController>();
        
        sut.StartStep(displayStoryAndSubmitAnswerHuntStepMock.Object, huntControllerMock.Object, false);
        //Act
        sut.ShowAssetInStep(ComponentType.End);
        //Assert
        endHuntActionsMock.Verify(x => x.Configure("",It.IsAny<Action>()));
    }
    [Test]
    public void TestShowAssetInStep_NoSuchView_Throws()
    {
        //Given a DisplayStoryAndDoneStepController with X and Y screens
        //When we try to show screen Z
        //Then the function throws an error
        //Setup
        var answerData = new StringAnswerData(identifier, textGetterMock.Object, String_AnswerAssetUrl, (ready) => { });
        var displayStoryAndSubmitAnswerHuntStepMock = new Mock<IDisplayRiddleAndSubmitAnswerStep>();
        displayStoryAndSubmitAnswerHuntStepMock.Setup(x => x.GetAnswerData()).Returns(answerData).Verifiable();
        displayStoryAndSubmitAnswerHuntStepMock.Setup(x => x.GetStepType())
            .Returns(StepType.DisplayRiddleAndSubmitAnswer);
        
        var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
        var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
        HuntHomeComponent huntHomeComponentObjectWithMock = new HuntHomeComponent(huntStartActionsMock.Object, huntStartMock.Object);
        
        var storyComponentUIActionsMock = CreateComponentUIActionsMock(true, ComponentType.Story); //past screen, now going to show huntStart
        var storyActionsMock = new Mock<IStoryComponentActions>();
        var storyComponentWithMock = new StoryComponent(storyActionsMock.Object, storyComponentUIActionsMock.Object);
        
        var riddleTabComponentMock = new Mock<IRiddleTabComponent>();
        AttachComponentUIActions(riddleTabComponentMock, true, ComponentType.RiddleTab);

        var endhuntUIMock = CreateComponentUIActionsMock(false, ComponentType.End);
        var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
        EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);
        
        var tabComponentMock = new Mock<ITabComponent>();
        
        DisplayStoryAndSubmitAnswerOldStepController sut = new DisplayStoryAndSubmitAnswerOldStepController(
            tabComponentMock.Object,
            storyComponentWithMock,
            riddleTabComponentMock.Object,
            huntHomeComponentObjectWithMock, 
            endHuntComponentObjectWithMock);
        var huntControllerMock = new Mock<IChristmasHuntController>();
        sut.StartStep(displayStoryAndSubmitAnswerHuntStepMock.Object, huntControllerMock.Object, false);
        //Act & Assert
        Assert.Throws<ArgumentException>(() =>sut.ShowAssetInStep(ComponentType.Scanning));
    }
    [Test]
    public void TestConstructor_MissingStory_Throws()
    {
        //Given a DisplayStoryAndDoneStepController and a null StoryScreen
        //When constructor is called
        //Then the function throws an error
        
        //Assert
        var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
        var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
        HuntHomeComponent huntHomeComponentObjectWithMock = 
            new HuntHomeComponent(huntStartActionsMock.Object, huntStartMock.Object);
        
        var riddleTabComponentMock = new Mock<IRiddleTabComponent>();
        AttachComponentUIActions(riddleTabComponentMock, true, ComponentType.Riddle);

        var endhuntUIMock = CreateComponentUIActionsMock(false, ComponentType.End);
        var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
        EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);
        //Act & Assert
        Assert.Throws<ArgumentException>(() =>new DisplayStoryAndSubmitAnswerOldStepController(
            null,
            null,
            riddleTabComponentMock.Object,
            huntHomeComponentObjectWithMock, 
            endHuntComponentObjectWithMock));
    }
    [Test]
    public void TestConstructor_MissingHuntStart_Throws()
    {
        //Given a DisplayStoryAndDoneStepController and a null HuntStart
        //When constructor is called
        //Then the function throws an error
        
        //Setup
        var storyComponentUIActionsMock = CreateComponentUIActionsMock(true, ComponentType.Story); //past screen, now going to show huntStart
        var storyActionsMock = new Mock<IStoryComponentActions>();
        var storyComponentWithMock = new StoryComponent(storyActionsMock.Object, storyComponentUIActionsMock.Object);
        
        var riddleTabComponentMock = new Mock<IRiddleTabComponent>();
        AttachComponentUIActions(riddleTabComponentMock, true, ComponentType.Riddle);

        var endhuntUIMock = CreateComponentUIActionsMock(false, ComponentType.End);
        var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
        EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);
        //Act & Assert
        Assert.Throws<ArgumentException>(() => new DisplayStoryAndSubmitAnswerOldStepController(
            null,
            storyComponentWithMock,
            riddleTabComponentMock.Object,
            null, 
            endHuntComponentObjectWithMock));
    }
    [Test]
    public void TestConstructor_MissingEnd_Throws()
    {   
        //Given a DisplayStoryAndDoneStepController and a null End screen
        //When constructor is called
        //Then the function throws an error

        //setup
        var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
        var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
        HuntHomeComponent huntHomeComponentObjectWithMock = new HuntHomeComponent(huntStartActionsMock.Object, huntStartMock.Object);

        var storyComponentUIActionsMock = CreateComponentUIActionsMock(true, ComponentType.Story); //past screen, now going to show huntStart
        var storyActionsMock = new Mock<IStoryComponentActions>();
        var storyComponentWithMock = new StoryComponent(storyActionsMock.Object, storyComponentUIActionsMock.Object);
 
        var riddleTabComponentMock = new Mock<IRiddleTabComponent>();
        AttachComponentUIActions(riddleTabComponentMock, true, ComponentType.RiddleTab);

        //Act & Assert
        Assert.Throws<ArgumentException>(() => new DisplayStoryAndSubmitAnswerOldStepController(
            null,
            storyComponentWithMock,
            riddleTabComponentMock.Object,
            huntHomeComponentObjectWithMock, 
            null));
    }
    [Test]
    public void TestConstructor_MissingRiddle_Throws()
    {   
        //Given a DisplayStoryAndDoneStepController and a null End screen
        //When constructor is called
        //Then the function throws an error

        //setup
        var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
        var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
        HuntHomeComponent huntHomeComponentObjectWithMock = new HuntHomeComponent(huntStartActionsMock.Object, huntStartMock.Object);

        var storyComponentUIActionsMock = CreateComponentUIActionsMock(true, ComponentType.Story); //past screen, now going to show huntStart
        var storyActionsMock = new Mock<IStoryComponentActions>();
        var storyComponentWithMock = new StoryComponent(storyActionsMock.Object, storyComponentUIActionsMock.Object);
 
        var endhuntUIMock = CreateComponentUIActionsMock(false, ComponentType.End);
        var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
        EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);
        
        //Act & Assert
        Assert.Throws<ArgumentException>(() => new DisplayStoryAndSubmitAnswerOldStepController(
            null,
            storyComponentWithMock,
            null,
            huntHomeComponentObjectWithMock, 
            endHuntComponentObjectWithMock));
    }
    [Test]
    public void TestShowAssetInStep_WrongHuntStep_Throws()
    {
        //Given a DisplayStoryAndDoneStepController and a wrong huntstep
        //When showAssetInStep is called
        //Then the function throws an error
        
        //Setup
        var displayStoryAndSubmitAnswerHuntStepMock = new Mock<IDisplayRiddleAndSubmitAnswerStep>();
        displayStoryAndSubmitAnswerHuntStepMock.Setup(x => x.GetStepType())
            .Returns(StepType.RecognizeImageAndPlayVideo);

        var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
        var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
        HuntHomeComponent huntHomeComponentObjectWithMock = 
            new HuntHomeComponent(huntStartActionsMock.Object, huntStartMock.Object);

        var storyComponentUIActionsMock = CreateComponentUIActionsMock(true, ComponentType.Story); //past screen, now going to show huntStart
        var storyActionsMock = new Mock<IStoryComponentActions>();
        var storyComponentWithMock = new StoryComponent(storyActionsMock.Object, storyComponentUIActionsMock.Object);

        var riddleTabComponentMock = new Mock<IRiddleTabComponent>();
        AttachComponentUIActions(riddleTabComponentMock, true, ComponentType.Riddle);

        var endhuntUIMock = CreateComponentUIActionsMock(false, ComponentType.End);
        var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
        EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);

        var tabComponentMock = new Mock<ITabComponent>();
        var sut = new DisplayStoryAndSubmitAnswerOldStepController(
                tabComponentMock.Object,
                storyComponentWithMock,
                riddleTabComponentMock.Object,
                huntHomeComponentObjectWithMock, 
                endHuntComponentObjectWithMock
        );
        var huntControllerMock = new Mock<IChristmasHuntController>();

        //Act & Assert
        Assert.Throws<ArgumentException>(()=>sut.StartStep(displayStoryAndSubmitAnswerHuntStepMock.Object, huntControllerMock.Object, false));
        Assert.Throws<ArgumentException>(() => sut.ShowAssetInStep(ComponentType.Story));
        displayStoryAndSubmitAnswerHuntStepMock.Verify(x => x.GetStepType());
    }
    [Test]
    public void TestStartStep_Succeeds()
    {
        //Given a DisplayStoryAndDoneStepController and a huntstep.
        //When StartStep is called
        //Then the function shows the first screen.
        
        //Setup
        string storyText = "story";
        string storyTextBtn = "OK!";
        var answerData = new StringAnswerData(identifier, textGetterMock.Object, String_AnswerAssetUrl, (ready) => { });
        var displayStoryAndSubmitAnswerHuntStepMock = new Mock<IDisplayRiddleAndSubmitAnswerStep>();
        displayStoryAndSubmitAnswerHuntStepMock.Setup(x => x.GetAnswerData()).Returns(answerData).Verifiable();
        displayStoryAndSubmitAnswerHuntStepMock.Setup(x => x.GetStepType())
            .Returns(StepType.DisplayRiddleAndSubmitAnswer);
        
        displayStoryAndSubmitAnswerHuntStepMock.Setup(x => x.GetStoryText()).Returns(storyText).Verifiable();
        var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
        var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
        HuntHomeComponent huntHomeComponentObjectWithMock = 
            new HuntHomeComponent (huntStartActionsMock.Object, huntStartMock.Object);

        var storyComponentUIActionsMock = CreateComponentUIActionsMock(true, ComponentType.Story); //past screen, now going to show huntStart
        var storyActionsMock = new Mock<IStoryComponentActions>();
        var storyComponentWithMock = new StoryComponent(storyActionsMock.Object, storyComponentUIActionsMock.Object);
        storyActionsMock.Setup(x => x.Configure(storyText, storyTextBtn)).Verifiable();
        
        var riddleTabComponentMock = new Mock<IRiddleTabComponent>();
        AttachComponentUIActions(riddleTabComponentMock, true, ComponentType.RiddleTab);

        var endhuntUIMock = CreateComponentUIActionsMock(false, ComponentType.End);
        var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
        EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);
        var huntControllerMock = new Mock<IChristmasHuntController>();

        var tabComponentMock = new Mock<ITabComponent>();
        DisplayStoryAndSubmitAnswerOldStepController sut = new DisplayStoryAndSubmitAnswerOldStepController(
            tabComponentMock.Object,
            storyComponentWithMock,
            riddleTabComponentMock.Object,
            huntHomeComponentObjectWithMock,
            endHuntComponentObjectWithMock);
        tabComponentMock.Setup(x => x.ConfigureForStepType(sut)).Verifiable();
        
        //Act 
        sut.StartStep(displayStoryAndSubmitAnswerHuntStepMock.Object, huntControllerMock.Object, false);
        //Assert
        Assert.IsNotNull(sut.HuntStep);
        storyActionsMock.Verify(x => x.Configure(storyText, storyTextBtn));
        //storyActionsMock.Verify(x => x.Configure(storyText, storyTextBtn, It.IsAny<Action>()));
        displayStoryAndSubmitAnswerHuntStepMock.Verify(x => x.GetStoryText());
        tabComponentMock.Verify(x => x.ConfigureForStepType(sut));
    } 
    [Test]
    public void TestStartStep_Throws()
    {
        //Given a recognizeImageAndPlayVideoStepController and a null huntstep.
        //When StartStep is called
        //Then the function throws an error
        
        //Setup
        var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
        var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
        HuntHomeComponent huntHomeComponentObjectWithMock = 
            new HuntHomeComponent(huntStartActionsMock.Object, huntStartMock.Object);

        var storyComponentUIActionsMock = CreateComponentUIActionsMock(true, ComponentType.Story); //past screen, now going to show huntStart
        var storyActionsMock = new Mock<IStoryComponentActions>();
        var storyComponentWithMock = new StoryComponent(storyActionsMock.Object, storyComponentUIActionsMock.Object);
        
        var riddleTabComponentMock = new Mock<IRiddleTabComponent>();
        AttachComponentUIActions(riddleTabComponentMock, true, ComponentType.RiddleTab);

        var endhuntUIMock = CreateComponentUIActionsMock(false, ComponentType.End);
        var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
        EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);
        
        var sut = new DisplayStoryAndSubmitAnswerOldStepController(
            null,
            storyComponentWithMock,
            riddleTabComponentMock.Object,
            huntHomeComponentObjectWithMock,
            endHuntComponentObjectWithMock);
        //Act & Assert
        Assert.Throws<ArgumentException>(() => sut.StartStep(null, null, false));
    }
    [Test]
    public void TestStartStep_SecondRun_ShowsRiddle_And_RecordedAnswer()
    {
        //Given a DisplayStoryAndSubmitAnswerStep with a recorded text answer
        //When we start the step
        //Then the riddle screen is shown.
        
        //Setup
        string riddleText = "riddleText";
        StepType stepType = StepType.DisplayRiddleAndSubmitAnswer;
        var answerData = new StringAnswerData(identifier, textGetterMock.Object, String_AnswerAssetUrl, (ready) => { });
        answerData.SetAnswer("GivenAnswer");
        var displayStoryAndSubmitAnswerHuntStepMock = new Mock<IDisplayRiddleAndSubmitAnswerStep>();
        
        displayStoryAndSubmitAnswerHuntStepMock
            .Setup(x => x.GetRiddleText())
            .Returns(riddleText).Verifiable();
        
        displayStoryAndSubmitAnswerHuntStepMock
            .Setup(x => x.GetAnswerData())
            .Returns(answerData).Verifiable();
        
        displayStoryAndSubmitAnswerHuntStepMock
            .Setup(x => x.GetStepType())
            .Returns(stepType)
            .Verifiable();

        var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
        var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
        HuntHomeComponent huntHomeComponentObjectWithMock = new HuntHomeComponent(huntStartActionsMock.Object, huntStartMock.Object);
        
        var storyComponentUIActionsMock = CreateComponentUIActionsMock(true, ComponentType.Story); 
        var storyActionsMock = new Mock<IStoryComponentActions>();
        var storyComponentWithMock = new StoryComponent(storyActionsMock.Object, storyComponentUIActionsMock.Object);
        
        var riddleTabComponentMock = new Mock<IRiddleTabComponent>();
        AttachComponentUIActions(riddleTabComponentMock, true, ComponentType.RiddleTab);

        riddleTabComponentMock.Setup(x =>
            x.Configure(riddleText, answerData, _imageList, It.IsAny<Action>())).Verifiable();
        
        var endhuntUIMock = CreateComponentUIActionsMock(false, ComponentType.End);
        var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
        EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);
        var tabComponentMock = new Mock<ITabComponent>();
        tabComponentMock.Setup(x => x.Display(ComponentType.RiddleTab)).Verifiable();
        DisplayStoryAndSubmitAnswerOldStepController sut = new DisplayStoryAndSubmitAnswerOldStepController(
            tabComponentMock.Object,
            storyComponentWithMock,
            riddleTabComponentMock.Object,
            huntHomeComponentObjectWithMock, 
            endHuntComponentObjectWithMock);
  
        var huntControllerMock = new Mock<IChristmasHuntController>();
        //Act
        sut.StartStep(displayStoryAndSubmitAnswerHuntStepMock.Object, huntControllerMock.Object, false);
        
        //Assert
        tabComponentMock.Verify(x => x.Display(ComponentType.RiddleTab));

    }
    [Test]
    public void TestGetTypesInOrder()
    {
        var tabComponentMock = new Mock<ITabComponent>();
        var homeComponentMock = new Mock<IHuntHomeComponent>();
        var storyComponentMock = new Mock<IStoryComponent>();
        var riddleTabComponentMock = new Mock<IRiddleTabComponent>();
        var endHuntComponentMock = new Mock<IEndHuntComponent>();
        DisplayStoryAndSubmitAnswerOldStepController sut = new DisplayStoryAndSubmitAnswerOldStepController(
            tabComponentMock.Object,
            storyComponentMock.Object,
            riddleTabComponentMock.Object,
            homeComponentMock.Object,
            endHuntComponentMock.Object);

        var componentOrder = sut.GetTypesInOrder();
        Assert.AreEqual(ComponentType.Story, componentOrder[0]);
        Assert.AreEqual(ComponentType.RiddleTab, componentOrder[1]);
        Assert.AreEqual(2, componentOrder.Count);
    }
    [Test]
    public void TestGetFirstStepTypeToShow()
    {
        var tabComponentMock = new Mock<ITabComponent>();
        var homeComponentMock = new Mock<IHuntHomeComponent>();
        var storyComponentMock = new Mock<IStoryComponent>();
        var riddleTabComponentMock = new Mock<IRiddleTabComponent>();
        var endHuntComponentMock = new Mock<IEndHuntComponent>();
        DisplayStoryAndSubmitAnswerOldStepController sut = new DisplayStoryAndSubmitAnswerOldStepController(
            tabComponentMock.Object,
            storyComponentMock.Object,
            riddleTabComponentMock.Object,
            homeComponentMock.Object,
            endHuntComponentMock.Object);

        Assert.AreEqual(ComponentType.Story, sut.GetFirstStepTypeToShow());
    }
}
