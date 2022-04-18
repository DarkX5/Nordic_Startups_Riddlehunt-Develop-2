using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UIHelpers;
using riddlehouse_libraries.products.Assets.AnswerAssets;
using UnityEngine;

public class TestWispContainerAnswerView
{
    private WispContainerAnswerView.Dependencies CreateDependencies(
        Color selected,
        Color unselected,
        Mock<IWispAnswerComponent> answerComponentMock = null,
        Mock<IWispFooter> wispFooterMock = null, 
        SetTextElement textElement = null)
    {
        answerComponentMock ??= new Mock<IWispAnswerComponent>();
        wispFooterMock ??= new Mock<IWispFooter>();
        textElement ??= new GameObject().AddComponent<SetTextElement>();
        
        return new WispContainerAnswerView.Dependencies()
        {
            WispAnswerComponent =answerComponentMock.Object,
            WispFooter = wispFooterMock.Object,
            RiddleTextField = textElement,
            SelectedColor = selected,
            IdleColor = unselected
        };
    }

    [Test]
    public void TestConfigure_Configures_ComponentsInView()
    {
        //Given a new WispContainerAnswerView
        //When Configure is called
        //Then the components are configured
        
        //Arrange
        var answerAsset = new Mock<IMultipleChoiceTextAnswerAsset>();
        string riddleText = "text";
        Color characterFrameColor = Color.black;
        Sprite characterIcon = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        Color selected = Color.blue;
        Color unselected = Color.clear;
        
        var sut = new GameObject().AddComponent<WispContainerAnswerView>();

        var wispAnswerComponentMock = new Mock<IWispAnswerComponent>();
        wispAnswerComponentMock.Setup(x => 
            x.Configure(answerAsset.Object, selected, unselected)).Verifiable();

        var wispFooterMock = new Mock<IWispFooter>();
        wispFooterMock.Setup(x=> 
            x.Configure(characterFrameColor, characterIcon, It.IsAny<Action>(), It.IsAny<Action>())).Verifiable();
        
        var dependencies = CreateDependencies(selected, unselected, wispAnswerComponentMock, wispFooterMock);
        sut.SetDependencies(dependencies);
        
        //Act
        sut.Configure(
            answerAsset.Object, 
            riddleText, 
            characterFrameColor, 
            characterIcon, 
            null, 
            null
        );
        
        //Assert
        wispFooterMock.Verify(x=> 
            x.Configure(characterFrameColor, characterIcon, It.IsAny<Action>(), It.IsAny<Action>()));
        wispAnswerComponentMock.Verify(x =>
            x.Configure(answerAsset.Object, selected, unselected));
        Assert.AreEqual(riddleText, sut._dependencies.RiddleTextField.TextField.text);
    }

    [Test]
    public void TestAcceptAction_Invokes_Configured_Action()
    {
        //Given a WispContainerAnswerView
        //When the Accept button is pressed
        //Then the accept action is invoked
        
        //Arrange
        bool actionCalled = false;
        Action action = () =>
        {
            actionCalled = true;
        };

        var wispFooterMock = new Mock<IWispFooter>();
        wispFooterMock.Setup(x=> 
            x.Configure(
                It.IsAny<Color>(), 
                It.IsAny<Sprite>(),
                It.IsAny<Action>(), 
                It.IsAny<Action>()))
            .Callback<Color, Sprite, Action, Action>((color, icon, accept, abort) =>
                {
                    accept.Invoke();
                }).Verifiable();

        var sut = new GameObject().AddComponent<WispContainerAnswerView>();
        var dependencies = CreateDependencies(Color.black, Color.blue, null, wispFooterMock);
        sut.SetDependencies(dependencies);

        //Act -> actual action happens in the callback
        sut.Configure(
           null, 
           "", 
           Color.black, 
           null,
           action, 
           null
        );
        
        //Assert
        Assert.IsTrue(actionCalled);
    }
    
    [Test]
    public void TestAbortAction_Invokes_Configured_Action()
    {
        //Given a WispContainerAnswerView
        //When the Abort button is pressed
        //Then the Abort action is invoked
        //Arrange
        bool actionCalled = false;
        Action action = () =>
        {
            actionCalled = true;
        };

        var wispFooterMock = new Mock<IWispFooter>();
        wispFooterMock.Setup(x=> 
                x.Configure(
                    It.IsAny<Color>(), 
                    It.IsAny<Sprite>(),
                    It.IsAny<Action>(), 
                    It.IsAny<Action>()))
            .Callback<Color, Sprite, Action, Action>((color, icon, accept, abort) =>
            {
                abort.Invoke();
            }).Verifiable();

        var sut = new GameObject().AddComponent<WispContainerAnswerView>();
        var dependencies = CreateDependencies(Color.black, Color.blue,null, wispFooterMock);
        sut.SetDependencies(dependencies);

        //Act -> actual action happens in the callback
        sut.Configure(
            null, 
            "", 
            Color.black, 
            null,
            null, 
            action
        );
        //Assert
        Assert.IsTrue(actionCalled);
    }
}
