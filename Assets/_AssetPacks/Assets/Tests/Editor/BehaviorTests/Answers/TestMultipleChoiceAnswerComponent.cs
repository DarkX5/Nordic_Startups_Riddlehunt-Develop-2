using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products.Assets;
using riddlehouse_libraries.products.Assets.AnswerAssets;
using TMPro;
using UI.Answers;
using UnityEngine;

[TestFixture]
public class TestMultipleChoiceAnswerComponent
{
    private string _seperator;
    private string _identifier;
    [SetUp]
    public void Init()
    {
        _seperator = ";";
        _identifier = "stringMultipleChoice-answer-identifier";
        PlayerPrefs.DeleteKey(_identifier);
    }

    private Mock<ITextChoiceBtn> CreateTextButtonMock(string value)
    {
        var mock = new Mock<ITextChoiceBtn>();
        mock
            .Setup(x => 
                x.Configure(
                    value, 
                    value, 
                    It.IsAny<Action<string>>()))
            .Verifiable();
        return mock;
    }

    private Mock<IIconChoiceBtn> CreateIconButtonMock(string value, Sprite icon)
    {
        var mock = new Mock<IIconChoiceBtn>();
        mock
            .Setup(x => 
                x.Configure(
                    icon, 
                    value, 
                    It.IsAny<Action<string>>()))
            .Verifiable();
        return mock;
    }
    
    private MultipleChoiceAnswerComponent.Dependencies CreateDependencies(
        Mock<IChoiceButtonInstantiator> choiceButtonInstantiatorMock = null,
        Mock<IStandardButton> standardButtonMock = null,
        Mock<ISpriteHelper> spriteHelper = null)
    {
        choiceButtonInstantiatorMock ??= new Mock<IChoiceButtonInstantiator>();
        standardButtonMock ??= new Mock<IStandardButton>();
        spriteHelper ??= new Mock<ISpriteHelper>();
        return new MultipleChoiceAnswerComponent.Dependencies()
        {
            SpriteHelper = spriteHelper.Object,
            ChoiceButtonInstantiator = choiceButtonInstantiatorMock.Object,
            NextButton = standardButtonMock.Object,
            AnswerTextField = new GameObject().AddComponent<TextMeshProUGUI>()
        };
    }

    [Test]
    public void TestSetDependencies()
    {
        //Arrange
        var sut = new GameObject().AddComponent<MultipleChoiceAnswerComponent>();

        var dependencies = CreateDependencies();
        //Act
        sut.SetDependencies(dependencies);
        //Assert
        Assert.AreEqual(dependencies, sut._dependencies);
    }
    
    [Test]
    public void TestChooseAnswer_Chooses_CorrectAnswer()
    {
        //Given a MultipleChoiceAnswerComponent, configured with a list of possible answers
        //When ChooseAnswer is called with the correctAnswer
        //Then the answer is logged and returns as the correct answer.

        //Arrange
        var choiceList = new List<string>() {"Choice A", "Choice B"};
        var correctAnswer = choiceList[0]+_seperator;

        var nextButtonMock = new Mock<IStandardButton>();

        var choiceButtonListMockA = new Mock<ITextChoiceBtn>();
        choiceButtonListMockA.Setup(x => x.GetState()).Returns(MultipleChoiceState.active).Verifiable();
        choiceButtonListMockA.Setup(x => x.SetState(MultipleChoiceState.selected)).Verifiable();
        choiceButtonListMockA.Setup(x => x.GetChoiceValue()).Returns(choiceList[0]).Verifiable();
        var choiceButtonListMockB = new Mock<ITextChoiceBtn>();

        var choiceButtonMockInstantiator = new Mock<IChoiceButtonInstantiator>();
        choiceButtonMockInstantiator
            .SetupSequence(x => x.CreateTextButton(It.IsAny<int>()))
            .Returns(choiceButtonListMockA.Object)
            .Returns(choiceButtonListMockB.Object);
        
        var sut = new GameObject().AddComponent<MultipleChoiceAnswerComponent>();

        var dependencies = CreateDependencies(choiceButtonMockInstantiator, nextButtonMock);
        sut.SetDependencies(dependencies);
        var options = new MultipleChoiceTextAnswerOptions()
        {
            AnswerOptions = choiceList,
            CorrectAnswer = correctAnswer,
            Seperator = _seperator,
            Evaluation = MultipleChoiceLogic.ContainsAll
        };
        var answerAsset = new MultipleChoiceTextAnswerAsset(options);
        sut.Configure(answerAsset, () => {});

        //Act
        sut.ChooseAnswer(choiceList[0]);
        
        //Assert
        choiceButtonListMockA.Verify(x => x.SetState(MultipleChoiceState.selected));
        choiceButtonListMockA.Verify(x => x.GetChoiceValue());
        Assert.IsTrue(answerAsset.HasCorrectAnswer());
    }

    [Test]
    public void TestConfigure_Configures_MultiChoiceText()
    {
        //Given a new MultipleChoiceAnswerComponent, a StringMultipleChoiceAnswerData and a button action.
        //When Configure is called with those parameters.
        //Then a list of textChoiceButtons is instantiated and configured.
        //--- NextButton Is configured with the buttonAction.
        //------ The current answer is reflected in the view.
        
        //Arrange
        var choiceList = new List<string>() {"Choice A", "Choice B"};
        
        var nextButtonMock = new Mock<IStandardButton>();
        nextButtonMock.Setup(x => x.Configure("Svar", It.IsAny<Action>())).Verifiable();
        
        var choiceButtonListMockA = CreateTextButtonMock(choiceList[0]);

        var choiceButtonListMockB = CreateTextButtonMock(choiceList[1]);

        var choiceButtonMockInstantiator = new Mock<IChoiceButtonInstantiator>();
        choiceButtonMockInstantiator
            .SetupSequence(x => x.CreateTextButton(It.IsAny<int>()))
            .Returns(choiceButtonListMockA.Object)
            .Returns(choiceButtonListMockB.Object);
        
        var sut = new GameObject().AddComponent<MultipleChoiceAnswerComponent>();

        var dependencies = CreateDependencies(choiceButtonMockInstantiator, nextButtonMock);
        sut.SetDependencies(dependencies);
        var options = new MultipleChoiceTextAnswerOptions()
        {
            AnswerOptions = choiceList,
            CorrectAnswer = choiceList[0],
            Seperator = _seperator,
            Evaluation = MultipleChoiceLogic.ContainsAll
        };
        var answerAsset = new MultipleChoiceTextAnswerAsset(options);
        //Act
        sut.Configure(answerAsset, () => {});
        
        //Assert
        Assert.AreEqual(dependencies, sut._dependencies);
        nextButtonMock.Verify(x => x.Configure("Svar", It.IsAny<Action>()));
        
        choiceButtonListMockA.Verify(x => 
                x.Configure(
                    choiceList[0], 
                    choiceList[0], 
                    It.IsAny<Action<string>>()));
        
        choiceButtonListMockB.Verify(x => 
                x.Configure(
                    choiceList[1], 
                    choiceList[1], 
                    It.IsAny<Action<string>>()));
    }
 [Test]
    public void TestConfigure_Configures_MultiChoiceIcon()
    {
        //Given a new MultipleChoiceAnswerComponent, an IconStringMultipleChoiceAnswerData and a button action.
        //When Configure is called with those parameters.
        //Then a list of IconChoiceButtons is instantiated and configured.
        //--- NextButton Is configured with the buttonAction.
        
        //Arrange
        var choiceList = new List<string>() {"A", "B", "C"};
        
        var nextButtonMock = new Mock<IStandardButton>();
        nextButtonMock.Setup(x => x.Configure("Svar", It.IsAny<Action>())).Verifiable();

        var iconA = Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.up);
        var choiceButtonListMockA = CreateIconButtonMock(choiceList[0], iconA);
        var iconB = Sprite.Create(Texture2D.grayTexture, Rect.zero, Vector2.left);
        var choiceButtonListMockB = CreateIconButtonMock(choiceList[1], iconB);
        var iconC = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        var choiceButtonListMockC = CreateIconButtonMock(choiceList[2], iconC);

        var choiceButtonMockInstantiator = new Mock<IChoiceButtonInstantiator>();
        choiceButtonMockInstantiator
            .SetupSequence(x => x.CreateIconButton(It.IsAny<int>()))
            .Returns(choiceButtonListMockA.Object)
            .Returns(choiceButtonListMockB.Object)
            .Returns(choiceButtonListMockC.Object);
        
        var sut = new GameObject().AddComponent<MultipleChoiceAnswerComponent>();

     
        var icons = new List<Sprite>() { iconA, iconB, iconC };
        var rawIconA = new Byte[] { 55 };
        var rawIconB = new Byte[] { 77 };
        var rawIconC = new Byte[] { 99 };
        
        var rawIcons = new List<Byte[]>();
        rawIcons.Add(rawIconA);
        rawIcons.Add(rawIconB);
        rawIcons.Add(rawIconC);

        var answerAssetMock = new Mock<IMultipleChoiceTextIconAnswerAsset>();
        answerAssetMock.Setup(x => x.Icons).Returns(rawIcons).Verifiable();
        answerAssetMock.Setup(x => x.AnswerOptions).Returns(choiceList).Verifiable();
        var spriteHelperMock = new Mock<ISpriteHelper>();
        spriteHelperMock.Setup(x => x.ConvertByteArrayListToSpriteList(rawIcons)).Returns(icons).Verifiable();
        var dependencies = CreateDependencies(choiceButtonMockInstantiator, nextButtonMock, spriteHelperMock);
        sut.SetDependencies(dependencies);
        
        //Act
        sut.Configure(answerAssetMock.Object, () => {});
        
        //Assert
        Assert.AreEqual(dependencies, sut._dependencies);
        nextButtonMock.Verify(x => x.Configure("Svar", It.IsAny<Action>()));
        
        choiceButtonListMockA.Verify(x => 
                x.Configure(
                    iconA, 
                    choiceList[0], 
                    It.IsAny<Action<string>>()));
        
        choiceButtonListMockB.Verify(x => 
                x.Configure(
                    iconB, 
                    choiceList[1], 
                    It.IsAny<Action<string>>()));
        
        choiceButtonListMockC.Verify(x => 
            x.Configure(
                iconC, 
                choiceList[2], 
                It.IsAny<Action<string>>()));
        answerAssetMock.Verify(x => x.AnswerOptions);
        answerAssetMock.Verify(x => x.Icons);
        spriteHelperMock.Verify(x => x.ConvertByteArrayListToSpriteList(rawIcons));
    }

    [Test]
    public void TestConfigure_ThirdConfiguration_HidesExcessButtons()
    {
        //Given a previously configured MultipleChoiceAnswerComponent
        //When Configure is called for the third time, with fewer options than first time, but same as second time.
        //Then the excess buttons are hidden

        //Arrange
        var secondChoiceList = new List<string>() {"Choice A", "Choice B"};

        var choiceList = new List<string>() {"Choice A", "Choice B", "Choice C"};
  
        var choiceButtonListMockA = CreateTextButtonMock(choiceList[0]);
        choiceButtonListMockA.Setup(x => x.SetState(MultipleChoiceState.hidden)).Verifiable(); //never

        var choiceButtonListMockB = CreateTextButtonMock(choiceList[1]);
        choiceButtonListMockB.Setup(x => x.SetState(MultipleChoiceState.hidden)).Verifiable(); //never

        var choiceButtonListMockC = CreateTextButtonMock(choiceList[2]);
        choiceButtonListMockC.Setup(x => x.SetState(MultipleChoiceState.hidden)).Verifiable(); //once

        var choiceButtonMockInstantiator = new Mock<IChoiceButtonInstantiator>();
        choiceButtonMockInstantiator
            .SetupSequence(x => x.CreateTextButton(It.IsAny<int>()))
            .Returns(choiceButtonListMockA.Object)
            .Returns(choiceButtonListMockB.Object)
            .Returns(choiceButtonListMockC.Object);
        
        var sut = new GameObject().AddComponent<MultipleChoiceAnswerComponent>();

        var dependencies = CreateDependencies(choiceButtonMockInstantiator);
        sut.SetDependencies(dependencies);
        var options = new MultipleChoiceTextAnswerOptions()
        {
            AnswerOptions = choiceList,
            CorrectAnswer = choiceList[0],
            Seperator = _seperator,
            Evaluation = MultipleChoiceLogic.ContainsAll
        };
        var answerAsset = new MultipleChoiceTextAnswerAsset(options);
        sut.Configure(answerAsset, () => {});
        
        //Assert that this wasn't called during the first configuration.
        choiceButtonListMockC.Verify(x => x.SetState(MultipleChoiceState.hidden), Times.Never);;

        var secondOptions = new MultipleChoiceTextAnswerOptions()
        {
            AnswerOptions = secondChoiceList,
            CorrectAnswer = secondChoiceList[0],
            Seperator = _seperator,
            Evaluation = MultipleChoiceLogic.ContainsAll
        };
        var secondAnswerAsset = new MultipleChoiceTextAnswerAsset(secondOptions);
        
        sut.Configure(secondAnswerAsset, () => {});
        //Assert that this was called once during the second configuration.
        choiceButtonListMockC.Verify(x => x.SetState(MultipleChoiceState.hidden), Times.Exactly(1));;
        //Act
        sut.Configure(secondAnswerAsset, () => {});
        
        //Assert
        Assert.AreEqual(dependencies, sut._dependencies);
        
        choiceButtonListMockA.Verify(x => 
                x.Configure(
                    choiceList[0], 
                    choiceList[0], 
                    It.IsAny<Action<string>>()), Times.Exactly(3));
        
        choiceButtonListMockB.Verify(x => 
                x.Configure(
                    choiceList[1], 
                    choiceList[1], 
                    It.IsAny<Action<string>>()), Times.Exactly(3));
        
        choiceButtonListMockC
            .Verify(x => 
                x.Configure(
                    choiceList[2], 
                    choiceList[2], 
                    It.IsAny<Action<string>>()), Times.Exactly(1));
        
        choiceButtonListMockA.Verify(x => x.SetState(MultipleChoiceState.hidden), Times.Exactly(2));
        choiceButtonListMockB.Verify(x => x.SetState(MultipleChoiceState.hidden), Times.Exactly(2));
        choiceButtonListMockC.Verify(x => x.SetState(MultipleChoiceState.hidden), Times.Exactly(2));;
    }
}