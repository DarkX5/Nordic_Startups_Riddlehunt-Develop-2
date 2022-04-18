using System;
using System.Collections;
using System.Collections.Generic;
using Answers.MultipleChoice.Buttons.Icon;
using Answers.MultipleChoice.Components;
using Answers.MultipleChoice.Data.Icon;
using NUnit.Framework;
using UnityEngine;
using Moq;
using UnityEngine.UI;

[TestFixture]
public class TestMultipleChoiceIconAnswerComponent
{
    RectTransform _contentTransform;
    private Mock<IIconButtonInstantiater> buttonInstantiater;
    private Button commitAnswerButton;
    private AudioSource objectAudioSource;
    private string identifier;

    private MultipleChoiceIconAnswerOptionsDisplay.Dependencies _dependencies;
    [SetUp]
    public void Init()
    {
        _contentTransform = new GameObject().AddComponent<RectTransform>();
        buttonInstantiater = new Mock<IIconButtonInstantiater>();
        commitAnswerButton = new GameObject().AddComponent<Button>();
        objectAudioSource = new GameObject().AddComponent<AudioSource>();
        _dependencies = new MultipleChoiceIconAnswerOptionsDisplay.Dependencies()
        {
            ButtonInstantiater = buttonInstantiater.Object,
            ContentParent = _contentTransform,
            CommitAnswerButton = commitAnswerButton,
            ObjectAudioSource = objectAudioSource
        };
        identifier = "iconMultipleChoiceAnswerComponent-answer-identifier";
        PlayerPrefs.DeleteKey(identifier);
    }

    [TearDown]
    public void TearDown()
    {
        _contentTransform = null;
        buttonInstantiater = null;
        commitAnswerButton = null;
        objectAudioSource = null;
    }
    
    private IconWithValue CreateOption(string value, bool correct)
    {
        return new IconWithValue()
        {
            Correct = correct,
            Icon = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down),
            Value = value
        };
    }
    [Test]
    public void SetDependencies()
    {
        //Arrange
        var sut = new GameObject().AddComponent<MultipleChoiceIconAnswerOptionsDisplay>();
 
        //Act
        sut.SetDependencies(_dependencies);
        
        //Assert
        Assert.AreSame(_dependencies, sut._dependencies);
    }
    
    [Test]
    public void TestConfigure_CreatesButtons_BasedOnData()
    {
        //Given a new MultipleChoiceIconAnswerComponent and 3 instances of answeroptions
        //When Configure is called
        //Then the system creates 3 buttons configured with those instances.

        //Arrange
        var correctAnswers = new List<IconWithValue>();
        correctAnswers.Add(CreateOption("a", true));
        var incorrectAnswers = new List<IconWithValue>();
        incorrectAnswers.Add(CreateOption("b", false));
        incorrectAnswers.Add(CreateOption("c", false));

        Action<bool> assetReady = (success) => { };
        var answerData = new IconMultipleChoiceAnswerData(identifier, correctAnswers, incorrectAnswers, assetReady);
        var config = new MultipleChoiceIconAnswerOptionsDisplay.Config()
        {
            AnswerData = answerData
        };
        
        var sut = new GameObject().AddComponent<MultipleChoiceIconAnswerOptionsDisplay>();

        buttonInstantiater.Setup(x => x.CreateButton()).Returns(new Mock<IOldIconChoiceBtn>().Object).Verifiable();
        sut.SetDependencies(new MultipleChoiceIconAnswerOptionsDisplay.Dependencies()
        {
            ButtonInstantiater = buttonInstantiater.Object,
            ContentParent = _contentTransform,
            CommitAnswerButton = commitAnswerButton,
            ObjectAudioSource = objectAudioSource
        });
        
        //Act
        sut.Configure(config);
        
        //Assert
        buttonInstantiater.Verify(x => x.CreateButton(), Times.Exactly(3));
    }
    
    [Test]
    public void TestConfigure_SecondConfigure_CreatesAdditionalButtons_BasedOnData()
    {
        //Given a new MultipleChoiceIconAnswerComponent and 3 instances of answeroptions.
        //When Configure is called with 5 new answeroptions.
        //Then the system creates 2 more buttons and reconfigures all existing buttons.

        //Arrange
        var correctAnswers = new List<IconWithValue>();
        correctAnswers.Add(CreateOption(1.ToString(), true));
        var incorrectAnswers = new List<IconWithValue>();
        for(int i = 2; i < 4; i++)
            incorrectAnswers.Add(CreateOption(i.ToString(), false));

        Action<bool> assetReady = (success) => { };
        var answerData = new IconMultipleChoiceAnswerData(identifier, correctAnswers, incorrectAnswers, assetReady);
        var config = new MultipleChoiceIconAnswerOptionsDisplay.Config()
        {
            AnswerData = answerData
        };
        
        var sut = new GameObject().AddComponent<MultipleChoiceIconAnswerOptionsDisplay>();

        buttonInstantiater.Setup(x => x.CreateButton()).Returns(new Mock<IOldIconChoiceBtn>().Object).Verifiable();
        sut.SetDependencies(new MultipleChoiceIconAnswerOptionsDisplay.Dependencies()
        {
            ButtonInstantiater = buttonInstantiater.Object,
            ContentParent = _contentTransform,
            CommitAnswerButton = commitAnswerButton,
            ObjectAudioSource = objectAudioSource
        });
        
        sut.Configure(config);
        
        var correctAnswers2 = new List<IconWithValue>();
        correctAnswers2.Add(CreateOption(1.ToString(), true));
        var incorrectAnswers2 = new List<IconWithValue>();
        for(int i = 2; i < 6; i++)
            incorrectAnswers2.Add(CreateOption(i.ToString(), false));
        
        var answerData2 = new IconMultipleChoiceAnswerData(identifier, correctAnswers2, incorrectAnswers2, assetReady);
        var config2 = new MultipleChoiceIconAnswerOptionsDisplay.Config()
        {
            AnswerData = answerData2
        };
        
        //Act
        sut.Configure(config2);
        
        //Assert
        buttonInstantiater.Verify(x => x.CreateButton(), Times.Exactly(5));
    }
    
    [Test]
    public void TestConfigure_SecondConfigure_Hides_SuperflousButtons_BasedOnData()
    {
        //Given a MultipleChoiceIconAnswerComponent that's been configured with 3 instances of answeroptions
        //When Configure is called with 5 different answeroption
        //Then the original buttons get reconfigured and 2 more buttons are added.

        //Arrange
        var buttonMock = new Mock<IOldIconChoiceBtn>();
        buttonMock.Setup(x => x.Hide()).Verifiable();
        
        var correctAnswers = new List<IconWithValue>();
        correctAnswers.Add(CreateOption(1.ToString(), true));
        var incorrectAnswers = new List<IconWithValue>();
        for(int i = 2; i < 6; i++)
            incorrectAnswers.Add(CreateOption(i.ToString(), false));

        Action<bool> assetReady = (success) => { };
        var answerData = new IconMultipleChoiceAnswerData(identifier, correctAnswers, incorrectAnswers, assetReady);
        var config = new MultipleChoiceIconAnswerOptionsDisplay.Config()
        {
            AnswerData = answerData
        };
        
        var sut = new GameObject().AddComponent<MultipleChoiceIconAnswerOptionsDisplay>();

        buttonInstantiater.Setup(x => x.CreateButton()).Returns(buttonMock.Object).Verifiable();
        sut.SetDependencies(new MultipleChoiceIconAnswerOptionsDisplay.Dependencies()
        {
            ButtonInstantiater = buttonInstantiater.Object,
            ContentParent = _contentTransform,
            CommitAnswerButton = commitAnswerButton,
            ObjectAudioSource = objectAudioSource
        });
        
        sut.Configure(config);
        
        var correctAnswers2 = new List<IconWithValue>();
        correctAnswers2.Add(CreateOption(1.ToString(), true));
        var incorrectAnswers2 = new List<IconWithValue>();
        for(int i = 2; i < 4; i++)
            incorrectAnswers2.Add(CreateOption(i.ToString(), false));
        
        var answerData2 = new IconMultipleChoiceAnswerData(identifier, correctAnswers2, incorrectAnswers2, assetReady);
        var config2 = new MultipleChoiceIconAnswerOptionsDisplay.Config()
        {
            AnswerData = answerData2
        };
        
        //Act
        sut.Configure(config2);
        
        //Assert
        buttonInstantiater.Verify(x => x.CreateButton(), Times.Exactly(5));
        buttonMock.Verify(x => x.Hide(), Times.Exactly(5));
        buttonMock.Verify(x => x.Configure(It.IsAny<MultipleChoiceOldIconButton.Config>()), Times.Exactly(8));
    }

    [Test]
    public void TestCheckAnswer_AnswerIncorrect_ClearsRecord()
    {
        //Arrange
        var correctAnswers = new List<IconWithValue>();
        correctAnswers.Add(CreateOption("a", true));
        var incorrectAnswers = new List<IconWithValue>();
        incorrectAnswers.Add(CreateOption("b", false));
        incorrectAnswers.Add(CreateOption("c", false));

        Action<bool> assetReady = (success) => { };
        var answerData = new IconMultipleChoiceAnswerData(identifier, correctAnswers, incorrectAnswers, assetReady);
        var config = new MultipleChoiceIconAnswerOptionsDisplay.Config()
        {
            AnswerData = answerData
        };
        
        answerData.SetAnswer("bc");
        
        var sut = new GameObject().AddComponent<MultipleChoiceIconAnswerOptionsDisplay>();

        buttonInstantiater.Setup(x => x.CreateButton()).Returns(new Mock<IOldIconChoiceBtn>().Object);
        sut.SetDependencies(new MultipleChoiceIconAnswerOptionsDisplay.Dependencies()
        {
            ButtonInstantiater = buttonInstantiater.Object,
            ContentParent = _contentTransform,
            CommitAnswerButton = commitAnswerButton,
            ObjectAudioSource = objectAudioSource
        });
        sut.Configure(config);

        //Act
        sut.CheckAnswer();
        
        //Assert
        Assert.IsTrue(string.IsNullOrEmpty(sut.collectiveAnswer));
    }
    
    [Test]
    public void TestCheckAnswer_AnswerCorrect()
    {
        //Arrange
        var correctAnswers = new List<IconWithValue>();
        correctAnswers.Add(CreateOption("a", true));
        var incorrectAnswers = new List<IconWithValue>();
        incorrectAnswers.Add(CreateOption("b", false));
        incorrectAnswers.Add(CreateOption("c", false));

        var correctButtonMock = new Mock<IOldIconChoiceBtn>();
        correctButtonMock.Setup(x => x.Configure(It.IsAny<MultipleChoiceOldIconButton.Config>()))
            .Callback<MultipleChoiceOldIconButton.Config>((theConfig) =>
            {
             theConfig.SelectionToggleAction.Invoke(correctAnswers[0]);   
            });
        
        var incorrectButtonMockA = new Mock<IOldIconChoiceBtn>();
        incorrectButtonMockA.Setup(x => x.Configure(It.IsAny<MultipleChoiceOldIconButton.Config>()));
        var incorrectButtonMockB = new Mock<IOldIconChoiceBtn>();
        incorrectButtonMockB.Setup(x => x.Configure(It.IsAny<MultipleChoiceOldIconButton.Config>()));

        Action<bool> assetReady = (success) => { };
        var answerData = new IconMultipleChoiceAnswerData(identifier, correctAnswers, incorrectAnswers, assetReady);
        var config = new MultipleChoiceIconAnswerOptionsDisplay.Config()
        {
            AnswerData = answerData
        };
        
        
        var sut = new GameObject().AddComponent<MultipleChoiceIconAnswerOptionsDisplay>();

        buttonInstantiater.SetupSequence(x => x.CreateButton())
            .Returns(correctButtonMock.Object)
            .Returns(incorrectButtonMockA.Object)
            .Returns(incorrectButtonMockB.Object);

        sut.SetDependencies(new MultipleChoiceIconAnswerOptionsDisplay.Dependencies()
        {
            ButtonInstantiater = buttonInstantiater.Object,
            ContentParent = _contentTransform,
            CommitAnswerButton = commitAnswerButton,
            ObjectAudioSource = objectAudioSource
        });
        sut.Configure(config);

        //Act
        sut.CheckAnswer();
        
        //Assert
        Assert.AreEqual("a", answerData.RecordedAnswer);
    }
}
