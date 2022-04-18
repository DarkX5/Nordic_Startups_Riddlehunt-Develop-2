using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TMPro;
using UI.Answers;
using UnityEngine;

public class TestTextChoiceBtn
{
    private TextChoiceBtn.Dependencies CreateDependencies(Mock<IChoiceBtnAnimator> choiceBtnAnimatorMock = null)
    {
        choiceBtnAnimatorMock ??= new Mock<IChoiceBtnAnimator>();
        return new TextChoiceBtn.Dependencies()
        {
            Animator = choiceBtnAnimatorMock.Object,
            TextField = new GameObject().AddComponent<TextMeshProUGUI>()
        };
    }
    
    [Test]
    public void TestConfigure_CallsConfigure_In_ActionLayer_And_Sets_ButtonAction()
    {
        //Given a riddleview is displayed
        //When choicebtn is configured
        //Then configure is called on the actions layer and the buttonAction is prepped for selection.
        
        //Arrange
        string choiceBtnText = "buttonText";
        string choice = "choice";
        Action<string> buttonAction = (myChoice) => { }; 
        
        var sut = new GameObject().AddComponent<TextChoiceBtn>();

        var dependencies = CreateDependencies();
        sut.SetDependencies(dependencies);
        //Act
        sut.Configure(choiceBtnText, choice, buttonAction);
        //Assert
        Assert.AreEqual(choiceBtnText, sut._dependencies.TextField.text);
    }
}
