using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class TestStepList
{
    [Test]
    public void TestConfigureStepList()
    {
        //Arrange
        Action<int> buttonAction = (index) => { };
        string stepTitle = "StepTitle";
        int stepIdx = 0;
        int stepCount = 1;
        int reached = 0;
        var stepBtnList = new List<IStepBtn>();

        var stepBtnMock = new Mock<IStepButtonActions>();
        var huntStepsMock = new Mock<IHuntSteps>();
        var huntStep = new Mock<IHuntStep>();
        huntStep.Setup(x => x.GetStepTitle()).Returns(stepTitle).Verifiable();
        huntStepsMock.Setup(x => x.GetLengthOfHunt()).Returns(stepCount);
        huntStepsMock.Setup(x => x.GetElement(stepIdx)).Returns(huntStep.Object).Verifiable();
        stepBtnMock
            .Setup(x => 
                x.Configure(
                    StepButtonState.next,
                    stepIdx, 
                    stepTitle, 
                    buttonAction
                    )
                ).Verifiable();
        var stepBtn = new StepBtn(stepBtnMock.Object);
        stepBtnList.Add(stepBtn);
        var sut = new StepList();
        
        //Act
        sut.ConfigureStepList(reached, huntStepsMock.Object, stepBtnList, buttonAction);
        
        //Assert
        huntStep.Verify(x => x.GetStepTitle());
        huntStepsMock.Verify(x => x.GetLengthOfHunt());
        huntStepsMock.Verify(x => x.GetElement(stepIdx));
        stepBtnMock
            .Verify(x => 
                x.Configure(
                    StepButtonState.next,
                    stepIdx, 
                    stepTitle, 
                    buttonAction
                )
            );
    }
}
