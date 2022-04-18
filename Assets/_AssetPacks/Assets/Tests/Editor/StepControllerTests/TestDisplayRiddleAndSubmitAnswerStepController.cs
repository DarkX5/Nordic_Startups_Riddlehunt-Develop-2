using System;
using System.Collections.Generic;
using Hunt;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;
using riddlehouse_libraries.products.Assets;
using riddlehouse_libraries.products.Assets.Adressables;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.Steps;
using riddlehouse_libraries.products.Steps.Resources;
using StepControllers;
using UnityEngine;
using lib_TextAsset =  riddlehouse_libraries.products.Assets.TextAsset;

[TestFixture]
public class TestDisplayRiddleAndSubmitAnswerStepController
{
    private Mock<IDisplayRiddleAndSubmitAnswer> _stepModelMock;
    private Mock<IStoryComponent> _storyTextViewMock;
    private Mock<IRiddleTabComponent> _riddleTabMock;
    private Mock<IViewCollector> _viewCollector;
    private Mock<IStepDataConverter>_stepDataConverter;
    private Mock<IMapCanvasController> _mapCanvasController;
    private string _storyText;
    private string _riddleText;
    private string _buttonText;
    private BooleanAnswerAsset _answerAsset;
    private lib_TextAsset _storyTextAsset;
    private lib_TextAsset _riddleTextAsset;
    private DisplayRiddleAndSubmitAnswerResource _answerResource;

    [SetUp]
    public void Init()
    {
        _answerResource = new DisplayRiddleAndSubmitAnswerResource(
            new Addressable("storyAddress", AddressableTypes.GameObject),
            new Addressable("riddleAddress", AddressableTypes.GameObject));
        
        _storyText = "story";
        _riddleText = "riddle";
        _buttonText = "Videre";
        
        _answerAsset = new BooleanAnswerAsset(true);
        _storyTextAsset = new lib_TextAsset(AssetType.StoryText, _storyText);
        _riddleTextAsset = new lib_TextAsset(AssetType.RiddleText, _riddleText);

        _storyTextViewMock = new Mock<IStoryComponent>();
        _storyTextViewMock.Setup(x => x.Configure(_storyText, _buttonText, It.IsAny<Action>())).Verifiable();
        _riddleTabMock = new Mock<IRiddleTabComponent>();
        _riddleTabMock.Setup(x=> x.Hide()).Verifiable();
        _viewCollector = new Mock<IViewCollector>();
        _viewCollector.Setup(x => x.StoryView(_answerResource.StoryView.Address))
            .ReturnsAsync(_storyTextViewMock.Object).Verifiable();
        _viewCollector.Setup(x => x.RiddleView(_answerResource.RiddleView.Address))
            .ReturnsAsync(_riddleTabMock.Object).Verifiable();
        
        _stepDataConverter = new Mock<IStepDataConverter>();
        
        _stepModelMock = new Mock<IDisplayRiddleAndSubmitAnswer>();
        _stepModelMock.Setup(x => x.Type).Returns(StepType.DisplayRiddleAndSubmitAnswer);
        _stepModelMock.Setup(x => x.Id).Returns("step1");
        _stepModelMock.Setup(x => x.Resource).Returns(_answerResource);
        _stepModelMock.Setup(x => x.StoryText).Returns(_storyTextAsset).Verifiable();
        _stepModelMock.Setup(x => x.RiddleText).Returns(_riddleTextAsset).Verifiable();
        _stepModelMock.Setup(x => x.AnswerAsset).Returns(_answerAsset).Verifiable();
        _mapCanvasController = new Mock<IMapCanvasController>();

    }

    [TearDown]
    public void TearDown()
    {
        _viewCollector = null;
        _stepDataConverter = null;
    }

    [Test]
    public void TestStartStep_CollectsStoryView_Displays() {
    
        //Given a stepcontroller and a loaded stepmodel
        //When StartStep is called with that stepmodel
        //Then the storyView is collected and configured with relevant data.

        //Arrange
        var story_ViewUIActions = new Mock<IViewActions>();
        story_ViewUIActions.Setup(x => x.Display()).Verifiable();
        _storyTextViewMock.Setup(x=> x.GetComponentUIActions()).Returns(story_ViewUIActions.Object);

        _mapCanvasController.Setup(x => x.AttachViewToCanvas(story_ViewUIActions.Object, 1)).Verifiable();
        _stepDataConverter.Setup(x => x.ConvertToDisplayRiddleAndSubmitAnswer(_stepModelMock.Object))
            .Returns(_stepModelMock.Object).Verifiable();

        var sut = new DisplayRiddleAndSubmitAnswerStepController(_viewCollector.Object, _stepDataConverter.Object);
        
        //Act
        sut.StartStep(_stepModelMock.Object, _mapCanvasController.Object, null);
        
        //Assert
        _viewCollector.Verify(x => x.StoryView(_answerResource.StoryView.Address));
        _storyTextViewMock.Verify(x => x.Configure(_storyText, _buttonText, It.IsAny<Action>()));
        _stepDataConverter.Verify(x => x.ConvertToDisplayRiddleAndSubmitAnswer(_stepModelMock.Object));
        
        _stepModelMock.Verify(x => x.StoryText);
        story_ViewUIActions.Verify(x => x.Display());
        
        _mapCanvasController.Verify(x => x.AttachViewToCanvas(story_ViewUIActions.Object, 1));
        _riddleTabMock.Verify(x=> x.Hide());
    }

    [Test]
    public void TestStartStep_NoStoryText_Skips_StoryView()
    {
        //Given a DisplayRiddleAndSubmitAnswerStepController, with a model that has an empty storyTextAsset
        //When StartStep is called
        //Then the system skips the storyView and goes directly to the RiddleView
        
        //Arrange
        _storyTextAsset = new lib_TextAsset(AssetType.StoryText, "");

        _stepModelMock = new Mock<IDisplayRiddleAndSubmitAnswer>();
        _stepModelMock.Setup(x => x.Type).Returns(StepType.DisplayRiddleAndSubmitAnswer);
        _stepModelMock.Setup(x => x.Id).Returns("step1");
        _stepModelMock.Setup(x => x.Resource).Returns(_answerResource);
        _stepModelMock.Setup(x => x.StoryText).Returns(_storyTextAsset).Verifiable();
        _stepModelMock.Setup(x => x.RiddleText).Returns(_riddleTextAsset).Verifiable();
        _stepModelMock.Setup(x => x.AnswerAsset).Returns(_answerAsset).Verifiable();

        var story_ViewUIActions = new Mock<IViewActions>(); //refactor this.
        story_ViewUIActions.Setup(x => x.Display()).Verifiable();
        _storyTextViewMock.Setup(x=> x.GetComponentUIActions()).Returns(story_ViewUIActions.Object);

        var riddleTextViewMock = new Mock<IRiddleTabComponent>();
        riddleTextViewMock.Setup(x => x.Display()).Verifiable();

        _viewCollector.Setup(x => x.RiddleView(_answerResource.RiddleView.Address))
            .ReturnsAsync(riddleTextViewMock.Object).Verifiable();
        
        _stepDataConverter.Setup(x => x.ConvertToDisplayRiddleAndSubmitAnswer(_stepModelMock.Object))
            .Returns(_stepModelMock.Object).Verifiable();
        
        var sut = new DisplayRiddleAndSubmitAnswerStepController(_viewCollector.Object, _stepDataConverter.Object);

        //Act (the action happens in the callback on the storyTextViewMock.Configure)
        sut.StartStep(_stepModelMock.Object, _mapCanvasController.Object, null);
        
        //Assert
        riddleTextViewMock.Verify(x => x.Display());
        story_ViewUIActions.Verify(x => x.Display(), Times.Never);
    }

    [Test]
    public void TestStoryViewComplete_ShowsRiddle()
    {
        //Given a stepcontroller that's been configured to show the story screen.
        //When the StoryTextView returns as completed
        //Then the riddleView is collected and configured with relevant data.
        
        //Arrange
        var story_ViewUIActions = new Mock<IViewActions>();
        story_ViewUIActions.Setup(x => x.Hide()).Verifiable(); //refactor this.
        
        _storyTextViewMock.Setup(x => x.Configure(_storyText, _buttonText, It.IsAny<Action>()))
            .Callback<string, string, Action>((storyText, riddleText, action) =>
            {
                action.Invoke();
            }).Verifiable();
        _storyTextViewMock.Setup(x => x.GetComponentUIActions()).Returns(story_ViewUIActions.Object);
        
        var riddleTextViewMock = new Mock<IRiddleTabComponent>();

        riddleTextViewMock.Setup(x => 
            x.Configure(_riddleText, It.IsAny<IAnswerAsset>(), It.IsAny<List<Sprite>>(), It.IsAny<Action>()))
            .Verifiable();
        riddleTextViewMock.Setup(x => x.Display()).Verifiable();

        _mapCanvasController.Setup(x => x.AttachViewToCanvas(riddleTextViewMock.Object, 2)).Verifiable();

        _viewCollector.Setup(x => x.RiddleView(_answerResource.RiddleView.Address))
            .ReturnsAsync(riddleTextViewMock.Object).Verifiable();
        _stepDataConverter.Setup(x => x.ConvertToDisplayRiddleAndSubmitAnswer(_stepModelMock.Object))
            .Returns(_stepModelMock.Object).Verifiable();
        
        var sut = new DisplayRiddleAndSubmitAnswerStepController(_viewCollector.Object, _stepDataConverter.Object);

        //Act (the action happens in the callback on the storyTextViewMock.Configure)
        sut.StartStep(_stepModelMock.Object, _mapCanvasController.Object, null);
        
        //Assert
        _viewCollector.Verify(x => x.RiddleView(_answerResource.RiddleView.Address));
        _stepModelMock.Verify(x => x.StoryText);
        _stepModelMock.Verify(x => x.RiddleText);
        _stepModelMock.Verify(x => x.AnswerAsset);
        riddleTextViewMock.Verify(x => 
                x.Configure(_riddleText, It.IsAny<IAnswerAsset>(), It.IsAny<List<Sprite>>(), It.IsAny<Action>()));
        
        story_ViewUIActions.Verify(x => x.Hide());
        riddleTextViewMock.Verify(x => x.Display());
        
        _mapCanvasController.Verify(x => x.AttachViewToCanvas(riddleTextViewMock.Object, 2));
    }

    [Test]
    public void TestRiddleViewComplete_Calls_StepEnded()
    {
        //Arrange
        _storyTextAsset = new lib_TextAsset(AssetType.StoryText, "");
        
        var story_ViewUIActions = new Mock<IViewActions>(); //refactor this.
        story_ViewUIActions.Setup(x => x.Display()).Verifiable();
        _storyTextViewMock.Setup(x=> x.GetComponentUIActions()).Returns(story_ViewUIActions.Object);
        _storyTextViewMock.Setup(x => x.Configure(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Action>()))
            .Callback<string, string, Action>((story, buttonTxt, theAction) =>
            {
                theAction.Invoke();
            }).Verifiable();
        _viewCollector.Setup(x => x.StoryView(_answerResource.RiddleView.Address))
            .ReturnsAsync(_storyTextViewMock.Object).Verifiable();
        
        var riddleTextViewMock = new Mock<IRiddleTabComponent>();

        riddleTextViewMock.Setup(x => 
                x.Configure(_riddleTextAsset.Text, _answerAsset, It.IsAny<List<Sprite>>(), It.IsAny<Action>()))
            .Callback<string, IAnswerAsset, List<Sprite>, Action>((text, answerAsset, images, endAction) =>
            {
                endAction.Invoke();
            })
            .Verifiable();
        riddleTextViewMock.Setup(x => x.Display()).Verifiable();

        _viewCollector.Setup(x => x.RiddleView(_answerResource.RiddleView.Address))
            .ReturnsAsync(riddleTextViewMock.Object).Verifiable();
        
        _stepDataConverter.Setup(x => x.ConvertToDisplayRiddleAndSubmitAnswer(_stepModelMock.Object))
            .Returns(_stepModelMock.Object).Verifiable();
        
        var sut = new DisplayRiddleAndSubmitAnswerStepController(_viewCollector.Object, _stepDataConverter.Object);

        bool stepEndedValue = false;
        Action stepEnded = () =>
        {
            stepEndedValue = true;
        };

        //Act (the action happens in the callback on the riddleTextViewMock.Configure)
        sut.StartStep(_stepModelMock.Object, _mapCanvasController.Object, stepEnded);

        //Assert
        _storyTextViewMock.Verify(x => x.Configure(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Action>()));
        riddleTextViewMock.Verify(x => 
            x.Configure(_riddleTextAsset.Text, _answerAsset, It.IsAny<List<Sprite>>(), It.IsAny<Action>()));
        Assert.IsTrue(stepEndedValue);
    }
}