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
public class TestStepControllerHelper
{
    private Mock<IViewActions> _storyUIActions;
    private Mock<IViewActions> _riddleTabUIActions;
    private Mock<IViewActions> _endHuntUIActions;

    public Mock<IViewActions> CreateUIAction(ComponentType type)
    {
        var UIActions = new Mock<IViewActions>();
        UIActions.Setup(x => x.GetComponentType()).Returns(type);
        return UIActions;
    }
    
    [SetUp]
    public void Init()
    {
        _storyUIActions = CreateUIAction(ComponentType.Story);
        _riddleTabUIActions = CreateUIAction(ComponentType.RiddleTab);
        _endHuntUIActions = CreateUIAction(ComponentType.End);
    }

    [TearDown]
    public void Teardown()
    {
        _storyUIActions = null;
        _riddleTabUIActions = null;
        _endHuntUIActions = null;
    }

    [Test]
    public void TestAssureNecessaryStepViews_All_Dependencies_Present()
    {
        //Given a list of dependent componentType, and a view collection with the actual components
        //When AssureNecessaryStepViews is called
        //Then the function succeeds, because all are present.
        
        //Arrange
        var typesInOrder = new List<ComponentType>()
            { ComponentType.Story, ComponentType.RiddleTab, ComponentType.End };
        var views = new Dictionary<ComponentType, IViewActions>();
        
        views.Add(ComponentType.Story, _storyUIActions.Object);
        views.Add(ComponentType.RiddleTab, _riddleTabUIActions.Object);
        views.Add(ComponentType.End, _endHuntUIActions.Object);
        
        var sut = new StepControllerHelper();
        //Act and Assert
        Assert.DoesNotThrow(() => sut.AssureNecessaryStepViews(typesInOrder, views));
    }
    [Test]
    public void TestAssureNecessaryStepViews_Some_Dependencies_Missing()
    {
        //Given a list of dependent componentType, and a view collection with missing components
        //When AssureNecessaryStepViews is called
        //Then the function throws, because riddleTabComponent is missing.
        
        //Arrange
        var typesInOrder = new List<ComponentType>()
            { ComponentType.Story, ComponentType.RiddleTab, ComponentType.End };
        var views = new Dictionary<ComponentType, IViewActions>();
        
        views.Add(ComponentType.Story, _storyUIActions.Object);
        views.Add(ComponentType.End, _endHuntUIActions.Object);
        
        var sut = new StepControllerHelper();
        //Act and Assert
        Assert.Throws<ArgumentException>(() => sut.AssureNecessaryStepViews(typesInOrder, views));
    }
    [Test]
    public void TestAssureHuntStep_StepType_Is_DisplayRiddleAndSubmitAnswer_Succeeds()
    {
        //Given a huntstep of type DisplayRiddleAndSubmitAnswer
        //When that huntstep is being used in DisplayRiddleAndSubmitAnswerStepController
        //Then the function succeeds.

        var sut = new StepControllerHelper();

        Assert.DoesNotThrow(() => sut.AssureHuntStep(StepType.DisplayRiddleAndSubmitAnswer, StepType.DisplayRiddleAndSubmitAnswer));
    }
    [Test]
    public void TestAssureHuntStep_WrongStepType_In_HuntStep_Throws()
    {
        //Given a huntstep of type DisplayStoryAndDone
        //When that huntstep is being used in DisplayRiddleAndSubmitAnswerStepController
        //Then the function throws and error.
        
        //Arrange
        var sut = new StepControllerHelper();

        //Act & Assert
        Assert.Throws<ArgumentException>(() =>
            sut.AssureHuntStep(StepType.DisplayStoryAndDone, StepType.DisplayRiddleAndSubmitAnswer));
    }
}
