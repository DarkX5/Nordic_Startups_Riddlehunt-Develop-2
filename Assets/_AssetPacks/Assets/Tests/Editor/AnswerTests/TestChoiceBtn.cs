using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TMPro;
using UI.Answers;
using UnityEngine;

public class TestChoiceBtn
{
    private ChoiceBtn.Dependencies CreateDependencies(Mock<IChoiceBtnAnimator> choiceBtnAnimatorMock = null)
    {
        choiceBtnAnimatorMock ??= new Mock<IChoiceBtnAnimator>();
        return new ChoiceBtn.Dependencies()
        {
            Animator = choiceBtnAnimatorMock.Object,
        };
    }
    
    [Test]
    public void TestConfigure_CallsConfigure_In_ActionLayer_And_Sets_ButtonAction()
    {
        //Given a riddleview is displayed
        //When choicebtn is configured
        //Then configure is called on the actions layer and the buttonAction is prepped for selection.
        
        //Arrange
        string choice = "choice";
        Action<string> buttonAction = (myChoice) => { }; 
        
        var sut = new GameObject().AddComponent<ChoiceBtn>();
        var animatorMock = new Mock<IChoiceBtnAnimator>();
        animatorMock.Setup(x => x.SetBool("IsHighlighted", false)).Verifiable();
        animatorMock.Setup(x => x.SetBool("IsIdle", true)).Verifiable();

        var dependencies = CreateDependencies(animatorMock);
        sut.SetDependencies(dependencies);
        //Act
        sut.Configure(choice, buttonAction);
        //Assert
        Assert.AreEqual(MultipleChoiceState.active, sut.GetState());
        animatorMock.Verify(x => x.SetBool("IsHighlighted", false));
        animatorMock.Verify(x => x.SetBool("IsIdle", true));
    }

    [Test]
    public void TestSetState_SetsStateToHighlighted()
    {
        //Given a choice is selected, or the view is displayed
        //When the button state is set
        //Then setState is called.
       
        //Arrange
        var sut = new GameObject().AddComponent<ChoiceBtn>();
        
        var animatorMock = new Mock<IChoiceBtnAnimator>();
        animatorMock.Setup(x => x.SetBool("IsHighlighted", true)).Verifiable();
        animatorMock.Setup(x => x.SetBool("IsIdle", false)).Verifiable();

        var dependencies = CreateDependencies(animatorMock);
        sut.SetDependencies(dependencies);
        
        //Act
        sut.SetState(MultipleChoiceState.selected);
        //Assert
        Assert.AreEqual(MultipleChoiceState.selected, sut.GetState());
        animatorMock.Verify(x => x.SetBool("IsHighlighted", true));
        animatorMock.Verify(x => x.SetBool("IsIdle", false));
    }

    [Test]
    public void TestSelect_Invokes_ButtonAction()
    {
        //Given a choice that's been configured
        //When the choice is pressed
        //Then the choice is returned via a lambda action.
        
        //Arrange
        string choice = "choice";
        string chosen = null;
        Action<string> buttonAction = (myChoice) => { chosen = myChoice; }; 
        
        var sut = new GameObject().AddComponent<ChoiceBtn>();
        var dependencies = CreateDependencies();
        sut.SetDependencies(dependencies);
        
        sut.Configure(choice, buttonAction);
        //Act
        sut.Select();
        //Assert
        Assert.AreEqual(choice, chosen);
    }

    [Test]
    public void TestSetState_Hidden()
    {
        //Given a new multiplechoice answer component with 4 possible answers
        //When it is created with 6 buttons
        //then state on the last 2 buttons is set to hidden.
        //-- and the game object is disabled.
        var go = new GameObject();
        go.SetActive(true);
        var sut = new GameObject().AddComponent<ChoiceBtn>();
        sut.SetState(MultipleChoiceState.hidden);
        Assert.IsFalse(sut.gameObject.activeSelf);
    }
    
    [Test]
    public void TestDisplay()
    {
        //Given a choicebtn needs to be displayed
        //When display is called
        //then the gameobject is enabled.
        
        //Arrange
        var go = new GameObject();
        go.SetActive(false);
        var sut = new GameObject().AddComponent<ChoiceBtn>();
        //Act
        sut.Display();
        //Assert
        Assert.IsTrue(sut.gameObject.activeSelf);
    }
    [Test]
    public void TestHide()
    {
        //Given a choicebtn needs to be hidden
        //When display is called
        //then the gameobject is disabled
        
        //Arrange.
        var go = new GameObject();
        go.SetActive(true);
        var sut = new GameObject().AddComponent<ChoiceBtn>();
        //Act
        sut.Hide();
        //Assert
        Assert.IsFalse(sut.gameObject.activeSelf);
    }
}
