using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using UnityEngine;

[TestFixture]
public class TestDisplayStoryAndDoneStepController
{
    public Dictionary<ComponentType, IViewActions> CreateViews(
        IViewActions huntStartMock,
        IViewActions storyUIMock,
        IViewActions endHuntUIMock)
    {
        var Views = new Dictionary<ComponentType, IViewActions>();
        Views = new Dictionary<ComponentType, IViewActions>() {};
        Views.Add(ComponentType.HuntHome, huntStartMock);
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
    }//IDisplayStoryAndDoneHuntStep
    [Test]
    public void TestShowAssetInStep_Story_Succeeds()
    {
        //Given a DisplayStoryAndDoneStepController with correct config
        //When we attempt to show the story screen.
        //Then the story screen is shown.
        //Setup
        string storyText = "storyText";
        string storyTextBtn = "OK!";
        StepType stepType = StepType.DisplayStoryAndDone;

        var displayStoryAndDoneHuntStepMock = new Mock<IDisplayStoryAndDoneHuntStep>();
        displayStoryAndDoneHuntStepMock
            .Setup(x => x.GetStoryText())
            .Returns(storyText).Verifiable();
        
        displayStoryAndDoneHuntStepMock
            .Setup(x => x.GetStepType())
            .Returns(stepType)
            .Verifiable();

        var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
        var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
        HuntHomeComponent huntHomeComponentObjectWithMock = new HuntHomeComponent(huntStartActionsMock.Object, huntStartMock.Object);
        
        var storyComponentUIActionsMock = CreateComponentUIActionsMock(true, ComponentType.Story); //past screen, now going to show huntStart
        var storyActionsMock = new Mock<IStoryComponentActions>();
        var storyComponentWithMock = new StoryComponent(storyActionsMock.Object, storyComponentUIActionsMock.Object);
        storyActionsMock.Setup(x => x.Configure(storyText, storyTextBtn)).Verifiable();
        
        var endhuntUIMock = CreateComponentUIActionsMock(false, ComponentType.End);
        var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
        EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);
        var tabComponentMock = new Mock<ITabComponent>();
        DisplayStoryAndDoneOldStepController sut = new DisplayStoryAndDoneOldStepController(
            tabComponentMock.Object,
            storyComponentWithMock,
            huntHomeComponentObjectWithMock, 
            endHuntComponentObjectWithMock);


  
        var huntControllerMock = new Mock<IChristmasHuntController>();
        sut.StartStep(displayStoryAndDoneHuntStepMock.Object, huntControllerMock.Object, false);
        //Act
        sut.ShowAssetInStep(ComponentType.Story);
        //Assert
        storyActionsMock.Verify(x => x.Configure(storyText, storyTextBtn));
        //storyActionsMock.Verify(x => x.Configure(storyText, StoryTextBtn, It.IsAny<Action>()));
        displayStoryAndDoneHuntStepMock.Verify(x => x.GetStoryText());
        displayStoryAndDoneHuntStepMock.Verify(x => x.GetStepType());
    }
    [Test]
    public void TestShowAssetInStep_End_Suceeds()
    {
        //Given a DisplayStoryAndDoneStepController with correct config
        //When we attempt to show the end screen.
        //Then the end screen is shown.
        
        //Arrange
        var displayStoryAndDoneHuntStepMock = new Mock<IDisplayStoryAndDoneHuntStep>();
        displayStoryAndDoneHuntStepMock.Setup(x => x.GetStepType()).Returns(StepType.DisplayStoryAndDone);
        var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
        var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
        HuntHomeComponent huntHomeComponentObjectWithMock = new HuntHomeComponent(huntStartActionsMock.Object, huntStartMock.Object);

        var storyComponentUIActionsMock = CreateComponentUIActionsMock(true, ComponentType.Story); //past screen, now going to show huntStart
        var storyActionsMock = new Mock<IStoryComponentActions>();
        var storyComponentWithMock = new StoryComponent(storyActionsMock.Object, storyComponentUIActionsMock.Object);
        
        var endhuntUIMock = CreateComponentUIActionsMock(false, ComponentType.End);
        var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
        endHuntActionsMock.Setup(x => x.Configure("",It.IsAny<Action>())).Verifiable();
        EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);
        var tabComponentMock = new Mock<ITabComponent>();
        DisplayStoryAndDoneOldStepController sut = new DisplayStoryAndDoneOldStepController(
            tabComponentMock.Object,
            storyComponentWithMock,
            huntHomeComponentObjectWithMock, 
            endHuntComponentObjectWithMock);
        var huntControllerMock = new Mock<IChristmasHuntController>();
        sut.StartStep(displayStoryAndDoneHuntStepMock.Object, huntControllerMock.Object, false);
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
        var displayStoryAndDoneHuntStepMock = new Mock<IDisplayStoryAndDoneHuntStep>();
        displayStoryAndDoneHuntStepMock.Setup(x => x.GetStepType()).Returns(StepType.DisplayStoryAndDone);

        var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
        var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
        HuntHomeComponent huntHomeComponentObjectWithMock = new HuntHomeComponent(huntStartActionsMock.Object, huntStartMock.Object);
        
        var storyComponentUIActionsMock = CreateComponentUIActionsMock(true, ComponentType.Story); //past screen, now going to show huntStart
        var storyActionsMock = new Mock<IStoryComponentActions>();
        var storyComponentWithMock = new StoryComponent(storyActionsMock.Object, storyComponentUIActionsMock.Object);
        
        var endhuntUIMock = CreateComponentUIActionsMock(false, ComponentType.End);
        var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
        EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);
        var tabComponentMock = new Mock<ITabComponent>();
        DisplayStoryAndDoneOldStepController sut = new DisplayStoryAndDoneOldStepController(
            tabComponentMock.Object,
            storyComponentWithMock,
            huntHomeComponentObjectWithMock, 
            endHuntComponentObjectWithMock);
        var huntControllerMock = new Mock<IChristmasHuntController>();
        sut.StartStep(displayStoryAndDoneHuntStepMock.Object, huntControllerMock.Object, false);
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
        
      
        var endhuntUIMock = CreateComponentUIActionsMock(false, ComponentType.End);
        var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
        EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);
        //Act & Assert
        Assert.Throws<ArgumentException>(() =>new DisplayStoryAndDoneOldStepController(
            null,
            null,
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
        
        var endhuntUIMock = CreateComponentUIActionsMock(false, ComponentType.End);
        var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
        EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);
        //Act & Assert
        Assert.Throws<ArgumentException>(() => new DisplayStoryAndDoneOldStepController(
            null,
            storyComponentWithMock,
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
 
        //Act & Assert
        Assert.Throws<ArgumentException>(() => new DisplayStoryAndDoneOldStepController(
            null,
            storyComponentWithMock,
            huntHomeComponentObjectWithMock, 
            null));
    }
    [Test]
    public void TestShowAssetInStep_WrongHuntStep_Throws()
    {
        //Given a DisplayStoryAndDoneStepController and a wrong huntstep
        //When showAssetInStep is called
        //Then the function throws an error
        
        //Setup
        var displayStoryAndDoneStepMock = new Mock<IDisplayStoryAndDoneHuntStep>();
        displayStoryAndDoneStepMock
            .Setup(x => x.GetStepType())
            .Returns(StepType.RecognizeImageAndPlayVideo).Verifiable();

        var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
        var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
        HuntHomeComponent huntHomeComponentObjectWithMock = 
            new HuntHomeComponent(huntStartActionsMock.Object, huntStartMock.Object);

        var storyComponentUIActionsMock = CreateComponentUIActionsMock(true, ComponentType.Story); //past screen, now going to show huntStart
        var storyActionsMock = new Mock<IStoryComponentActions>();
        var storyComponentWithMock = new StoryComponent(storyActionsMock.Object, storyComponentUIActionsMock.Object);
        
        var endhuntUIMock = CreateComponentUIActionsMock(false, ComponentType.End);
        var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
        EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);

        var tabComponentMock = new Mock<ITabComponent>();
        var sut = new DisplayStoryAndDoneOldStepController(
                tabComponentMock.Object,
                storyComponentWithMock,
                huntHomeComponentObjectWithMock, 
                endHuntComponentObjectWithMock
        );
        var huntControllerMock = new Mock<IChristmasHuntController>();

        //Act & Assert
        Assert.Throws<ArgumentException>(()=>sut.StartStep(displayStoryAndDoneStepMock.Object, huntControllerMock.Object, false));
        Assert.Throws<ArgumentException>(() => sut.ShowAssetInStep(ComponentType.Story));
        displayStoryAndDoneStepMock.Verify(x => x.GetStepType());
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
        var displayStoryAndDoneStepMock = new Mock<IDisplayStoryAndDoneHuntStep>();
        displayStoryAndDoneStepMock
            .Setup(x => x.GetStepType())
            .Returns(StepType.DisplayStoryAndDone).Verifiable();
        displayStoryAndDoneStepMock.Setup(x => x.GetStoryText()).Returns(storyText).Verifiable();
        var huntStartMock = CreateComponentUIActionsMock(false, ComponentType.HuntHome);
        var huntStartActionsMock = new Mock<IHuntHomeComponentActions>();
        HuntHomeComponent huntHomeComponentObjectWithMock = 
            new HuntHomeComponent (huntStartActionsMock.Object, huntStartMock.Object);

        var storyComponentUIActionsMock = CreateComponentUIActionsMock(true, ComponentType.Story); //past screen, now going to show huntStart
        var storyActionsMock = new Mock<IStoryComponentActions>();
        var storyComponentWithMock = new StoryComponent(storyActionsMock.Object, storyComponentUIActionsMock.Object);
        storyActionsMock.Setup(x => x.Configure(storyText, storyTextBtn)).Verifiable();
        
        var endhuntUIMock = CreateComponentUIActionsMock(false, ComponentType.End);
        var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
        EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);
        var huntControllerMock = new Mock<IChristmasHuntController>();

        var tabComponentMock = new Mock<ITabComponent>();
        DisplayStoryAndDoneOldStepController sut = new DisplayStoryAndDoneOldStepController(
            tabComponentMock.Object,
            storyComponentWithMock,
            huntHomeComponentObjectWithMock,
            endHuntComponentObjectWithMock);
        tabComponentMock.Setup(x => x.ConfigureForStepType(sut)).Verifiable();
        
        //Act 
        sut.StartStep(displayStoryAndDoneStepMock.Object, huntControllerMock.Object, false);
        //Assert
        Assert.IsNotNull(sut.HuntStep);
        storyActionsMock.Verify(x => x.Configure(storyText, storyTextBtn));
        displayStoryAndDoneStepMock.Verify(x => x.GetStoryText());
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
    
        var endhuntUIMock = CreateComponentUIActionsMock(false, ComponentType.End);
        var endHuntActionsMock = new Mock<IEndHuntComponentActions>();
        EndHuntComponent endHuntComponentObjectWithMock = new EndHuntComponent(endHuntActionsMock.Object, endhuntUIMock.Object);
        
        var sut = new DisplayStoryAndDoneOldStepController(
            null,
            storyComponentWithMock,
            huntHomeComponentObjectWithMock,
            endHuntComponentObjectWithMock);
        //Act & Assert
        Assert.Throws<ArgumentException>(() => sut.StartStep(null, null, false));
    }
    [Test]
    public void TestGetTypesInOrder()
    {
        var tabComponentMock = new Mock<ITabComponent>();
        var homeComponentMock = new Mock<IHuntHomeComponent>();
        var storyComponentMock = new Mock<IStoryComponent>();
        var endHuntComponentMock = new Mock<IEndHuntComponent>();
        DisplayStoryAndDoneOldStepController sut = new DisplayStoryAndDoneOldStepController(
            tabComponentMock.Object,
            storyComponentMock.Object,
            homeComponentMock.Object,
            endHuntComponentMock.Object);

        var componentOrder = sut.GetTypesInOrder();
        Assert.AreEqual(ComponentType.Story, componentOrder[0]);
        Assert.AreEqual(1, componentOrder.Count);
    }
    [Test]
    public void TestGetFirstStepTypeToShow()
    {
        var tabComponentMock = new Mock<ITabComponent>();
        var homeComponentMock = new Mock<IHuntHomeComponent>();
        var storyComponentMock = new Mock<IStoryComponent>();
        var endHuntComponentMock = new Mock<IEndHuntComponent>();
        DisplayStoryAndDoneOldStepController sut = new DisplayStoryAndDoneOldStepController(
            tabComponentMock.Object,
            storyComponentMock.Object,
            homeComponentMock.Object,
            endHuntComponentMock.Object);

        Assert.AreEqual(ComponentType.Story, sut.GetFirstStepTypeToShow());
    }
}
