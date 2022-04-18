using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products.models.DTOs;
using UnityEngine;

public class TestConditionalStepList
{
    [Test]
    public void TestConfigureStepList_AllStepConditionsMet_HasAnswer_setsStepToComplete()
    {
        //Given a collection of huntsteps, with 1 step that has an answer and no conditions.
        //When the system evaluates it's state
        //Then the system sets that state to complete.
        
        //Arrange
        Action<string> buttonAction = (index) => { };
        string stepTitle = "StepTitle";
        string stepId = "id";
        var stepBtnList = new List<IConditionalStepBtn>();
        int stepCount = 1;

        var stepBtnMock = new Mock<IConditionalStepButtonActions>();
        var huntStepsMock = new Mock<IHuntSteps>();
        huntStepsMock.Setup(x => x.HasStepConditionsBeenMet(stepId)).Returns(true);
        
        var huntStep = new Mock<IHuntStep>();
        huntStep.Setup(x => x.HasAnswer()).Returns(true);
        huntStep.Setup(x => x.GetStepId()).Returns(stepId);
        
        huntStep.Setup(x => x.GetStepTitle()).Returns(stepTitle).Verifiable();
        huntStepsMock.Setup(x => x.GetLengthOfHunt()).Returns(stepCount);
        huntStepsMock.Setup(x => x.GetElement(0)).Returns(huntStep.Object).Verifiable();
        stepBtnMock
            .Setup(x => 
                x.Configure(
                    ConditionalStepButtonState.Complete,
                    stepId, 
                    stepTitle, 
                    buttonAction
                )
            ).Verifiable();
        var stepBtn = new ConditionalStepBtn(stepBtnMock.Object);
        stepBtnList.Add(stepBtn);
        var sut = new ConditionalStepList();
        
        //Act
        sut.ConfigureStepList(huntStepsMock.Object, stepBtnList, buttonAction);
        
        //Assert
        huntStep.Verify(x => x.GetStepTitle());
        huntStepsMock.Verify(x => x.GetLengthOfHunt());
        huntStepsMock.Verify(x => x.GetElement(0));
        stepBtnMock
            .Verify(x => 
                x.Configure(
                    ConditionalStepButtonState.Complete,
                    stepId, 
                    stepTitle, 
                    buttonAction
                )
            );
    }
    
    [Test]
    public void TestConfigureStepList_AllStepConditionsMet_HasNoAnswer_setsStepToInComplete()
    {
        //Given a collection of huntsteps, with 1 step that has no answer and no conditions.
        //When the system evaluates it's state
        //Then the system sets that state to incomplete.
        
        //Arrange
        Action<string> buttonAction = (index) => { };
        string stepTitle = "StepTitle";
        string stepId = "id";
        var stepBtnList = new List<IConditionalStepBtn>();
        int stepCount = 1;

        var stepBtnMock = new Mock<IConditionalStepButtonActions>();
        var huntStepsMock = new Mock<IHuntSteps>();
        huntStepsMock.Setup(x => x.HasStepConditionsBeenMet(stepId)).Returns(true);
        
        var huntStep = new Mock<IHuntStep>();
        huntStep.Setup(x => x.HasAnswer()).Returns(false);
        huntStep.Setup(x => x.GetStepId()).Returns(stepId);
        
        huntStep.Setup(x => x.GetStepTitle()).Returns(stepTitle).Verifiable();
        huntStepsMock.Setup(x => x.GetLengthOfHunt()).Returns(stepCount);
        huntStepsMock.Setup(x => x.GetElement(0)).Returns(huntStep.Object).Verifiable();
        stepBtnMock
            .Setup(x => 
                x.Configure(
                    ConditionalStepButtonState.Incomplete,
                    stepId, 
                    stepTitle, 
                    buttonAction
                )
            ).Verifiable();
        var stepBtn = new ConditionalStepBtn(stepBtnMock.Object);
        stepBtnList.Add(stepBtn);
        var sut = new ConditionalStepList();
        
        //Act
        sut.ConfigureStepList(huntStepsMock.Object, stepBtnList, buttonAction);
        
        //Assert
        huntStep.Verify(x => x.GetStepTitle());
        huntStepsMock.Verify(x => x.GetLengthOfHunt());
        huntStepsMock.Verify(x => x.GetElement(0));
        stepBtnMock
            .Verify(x => 
                x.Configure(
                    ConditionalStepButtonState.Incomplete,
                    stepId, 
                    stepTitle, 
                    buttonAction
                )
            );
    }
    
    [Test]
    public void TestConfigureStepList_AllStepConditionsNOTMet_SetsButton_Hidden()
    {
        //Given a collection of huntsteps, with 1 step that conditions that aren't met, and a style Hidden.
        //When the system evaluates it's state
        //Then the system sets that state to hidden.
        
        //Arrange
        Action<string> buttonAction = (index) => { };
        string stepTitle = "StepTitle";
        string stepId = "id";
        var stepBtnList = new List<IConditionalStepBtn>();
        int stepCount = 1;

        var stepBtnMock = new Mock<IConditionalStepButtonActions>();
        var huntStepsMock = new Mock<IHuntSteps>();
        huntStepsMock.Setup(x => x.HasStepConditionsBeenMet(stepId)).Returns(false);
        
        var huntStep = new Mock<IHuntStep>();
        huntStep.Setup(x => x.GetStepId()).Returns(stepId);
        var conditions = new StepCondition() { Ids = new List<string>(), Style = StepBtnStyles.Hidden, Type = StepConditionTypes.Prerequisite};
        huntStep.Setup(x => x.GetCondition()).Returns(conditions);
        
        huntStep.Setup(x => x.GetStepTitle()).Returns(stepTitle).Verifiable();
        huntStepsMock.Setup(x => x.GetLengthOfHunt()).Returns(stepCount);
        huntStepsMock.Setup(x => x.GetElement(0)).Returns(huntStep.Object).Verifiable();
        stepBtnMock
            .Setup(x => 
                x.Configure(
                    ConditionalStepButtonState.Hidden,
                    stepId, 
                    stepTitle, 
                    buttonAction
                )
            ).Verifiable();
        var stepBtn = new ConditionalStepBtn(stepBtnMock.Object);
        stepBtnList.Add(stepBtn);
        var sut = new ConditionalStepList();
        
        //Act
        sut.ConfigureStepList(huntStepsMock.Object, stepBtnList, buttonAction);
        
        //Assert
        huntStep.Verify(x => x.GetStepTitle());
        huntStepsMock.Verify(x => x.GetLengthOfHunt());
        huntStepsMock.Verify(x => x.GetElement(0));
        stepBtnMock
            .Verify(x => 
                x.Configure(
                    ConditionalStepButtonState.Hidden,
                    stepId, 
                    stepTitle, 
                    buttonAction
                )
            );
    }
    
    [Test]
    public void TestConfigureStepList_AllStepConditionsNOTMet_SetsButton_Disabled()
    {
        //Given a collection of huntsteps, with 1 step that conditions that aren't met, and a style disabled.
        //When the system evaluates it's state
        //Then the system sets that state to disabled.
        
        //Arrange
        Action<string> buttonAction = (index) => { };
        string stepTitle = "StepTitle";
        string stepId = "id";
        var stepBtnList = new List<IConditionalStepBtn>();
        int stepCount = 1;

        var stepBtnMock = new Mock<IConditionalStepButtonActions>();
        var huntStepsMock = new Mock<IHuntSteps>();
        huntStepsMock.Setup(x => x.HasStepConditionsBeenMet(stepId)).Returns(false);
        
        var huntStep = new Mock<IHuntStep>();
        huntStep.Setup(x => x.GetStepId()).Returns(stepId);
        var conditions = new StepCondition() { Ids = new List<string>(), Style = StepBtnStyles.Disabled, Type = StepConditionTypes.Prerequisite};
        huntStep.Setup(x => x.GetCondition()).Returns(conditions);
        
        huntStep.Setup(x => x.GetStepTitle()).Returns(stepTitle).Verifiable();
        huntStepsMock.Setup(x => x.GetLengthOfHunt()).Returns(stepCount);
        huntStepsMock.Setup(x => x.GetElement(0)).Returns(huntStep.Object).Verifiable();
        stepBtnMock
            .Setup(x => 
                x.Configure(
                    ConditionalStepButtonState.Disabled,
                    stepId, 
                    stepTitle, 
                    buttonAction
                )
            ).Verifiable();
        var stepBtn = new ConditionalStepBtn(stepBtnMock.Object);
        stepBtnList.Add(stepBtn);
        var sut = new ConditionalStepList();
        
        //Act
        sut.ConfigureStepList(huntStepsMock.Object, stepBtnList, buttonAction);
        
        //Assert
        huntStep.Verify(x => x.GetStepTitle());
        huntStepsMock.Verify(x => x.GetLengthOfHunt());
        huntStepsMock.Verify(x => x.GetElement(0));
        stepBtnMock
            .Verify(x => 
                x.Configure(
                    ConditionalStepButtonState.Disabled,
                    stepId, 
                    stepTitle, 
                    buttonAction
                )
            );
    }

}
