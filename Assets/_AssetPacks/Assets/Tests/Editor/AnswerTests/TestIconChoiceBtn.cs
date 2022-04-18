using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TMPro;
using UI.Answers;
using UnityEngine;
using UnityEngine.UI;

public class TestIconChoiceBtn
{
    private IconChoiceBtn.Dependencies CreateDependencies(Mock<IChoiceBtnAnimator> choiceBtnAnimatorMock = null)
    {
        choiceBtnAnimatorMock ??= new Mock<IChoiceBtnAnimator>();
        return new IconChoiceBtn.Dependencies()
        {
            Animator = choiceBtnAnimatorMock.Object,
            IconField = new GameObject().AddComponent<Image>()
        };
    }
    
    [Test]
    public void TestConfigure_CallsConfigure_In_ActionLayer_And_Sets_ButtonAction()
    {
        //Given a riddleview is displayed
        //When choicebtn is configured
        //Then configure is called on the actions layer and the buttonAction is prepped for selection.
        
        //Arrange
        Sprite icon = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        string choice = "choice";
        Action<string> buttonAction = (myChoice) => { }; 
        
        var sut = new GameObject().AddComponent<IconChoiceBtn>();

        var dependencies = CreateDependencies();
        sut.SetDependencies(dependencies);
        //Act
        sut.Configure(icon, choice, buttonAction);
        //Assert
        Assert.AreEqual(icon, sut._dependencies.IconField.sprite);
    }
}
