using System;
using Helpers;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using RHPackages.Core.Scripts.StepControllers;
using riddlehouse_libraries.products.Assets.Adressables;
using riddlehouse_libraries.products.Assets.AnswerAssets;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.huntProduct;
using riddlehouse_libraries.products.resources;
using riddlehouse_libraries.products.Steps;
using riddlehouse_libraries.products.Steps.Resources;
using UnityEngine;
using lib_TextAsset =  riddlehouse_libraries.products.Assets.TextAsset;
using TextAsset = riddlehouse_libraries.products.Assets.TextAsset;

public class TestDisplayRiddleWithMultipleChoiceStepController
{
    private Mock<ISpriteHelper> _spriteHelperMock;
    private Mock<IViewCollector> _viewCollector;
    private Mock<IStepDataConverter>_stepDataConverter;
    private Mock<IGameViewCanvasController> _gameCanvasController;
    private Mock<IDisplayRiddleWithMultipleChoice> _stepModelMock;
    private string _riddleText;
    private string _buttonText;
    private lib_TextAsset _riddleTextAsset;
    private DisplayRiddleWithMultipleChoiceResource _answerResource;
    private Mock<IMultipleChoiceTextAnswerAsset> _answerAsset;
    private Byte[] rawIcon;
    private Mock<IIcon> iconMock;
    [SetUp]
    public void Init()
    {
        iconMock = new Mock<IIcon>();
        rawIcon = new Byte[3] { 55, 66, 77 };
        iconMock.Setup(x => x.GetIcon(It.IsAny<HuntProductCacheConfig>())).ReturnsAsync(rawIcon).Verifiable();
        _answerResource = new DisplayRiddleWithMultipleChoiceResource(
            iconMock.Object,
            new Addressable("answerAddress", AddressableTypes.GameObject),
            new RHColor(55,55,55,55));
        
        _answerAsset = new Mock<IMultipleChoiceTextAnswerAsset>();
        _riddleTextAsset = new lib_TextAsset(AssetType.RiddleText, "riddle");

        _viewCollector = new Mock<IViewCollector>();
        _stepDataConverter = new Mock<IStepDataConverter>();

        _gameCanvasController = new Mock<IGameViewCanvasController>();
        
        _stepModelMock = new Mock<IDisplayRiddleWithMultipleChoice>();
        _stepModelMock.Setup(x=> x.Resource).Returns(_answerResource).Verifiable();
        _stepModelMock.Setup(x => x.RiddleText).Returns(_riddleTextAsset).Verifiable();
        _stepModelMock.Setup(x=> x.AnswerAsset).Returns(_answerAsset.Object).Verifiable();
        _spriteHelperMock = new Mock<ISpriteHelper>();
    }

    [TearDown]
    public void TearDown()
    {
        _viewCollector = null;
        _stepDataConverter = null;
    }

    [Test]
    public void TestStartStep_CollectsAnwerView_Displays()
    {
        //Given a DisplayRiddleWithMultipleChoiceStepController
        //When StartStep is called
        //Then the view is collected, configured and displayed.
      
        //Arrange
        var characterSprite = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        _spriteHelperMock.Setup(x => x.GetSpriteFromByteArray(rawIcon)).Returns(characterSprite).Verifiable();
        
        var answerViewMock = new Mock<IWispContainerAnswerView>();
        answerViewMock.Setup(x =>
            x.Configure(
                _answerAsset.Object,
                _riddleTextAsset.Text,
                It.IsAny<Color>(),
                characterSprite,
                It.IsAny<Action>(),
                It.IsAny<Action>()));
        answerViewMock.Setup(x => x.Display());

        _stepDataConverter.Setup(x => x.ConvertToDisplayRiddleWithMultipleChoice(_stepModelMock.Object))
            .Returns(_stepModelMock.Object).Verifiable();
        
        _viewCollector.Setup(x=> x.AnswerView(_answerResource.AnswerView.Address)).ReturnsAsync(answerViewMock.Object).Verifiable();

        _gameCanvasController.Setup(x => x.AttachViewToCanvas(answerViewMock.Object, 1));
        
        var sut = new DisplayRiddleWithMultipleChoiceStepController(_viewCollector.Object, _stepDataConverter.Object, _spriteHelperMock.Object);
        
        //Act
        sut.StartStep(_stepModelMock.Object, _gameCanvasController.Object, null);
        
        //Assert
        _spriteHelperMock.Verify(x => x.GetSpriteFromByteArray(rawIcon));
        _viewCollector.Verify(x=> x.AnswerView(_answerResource.AnswerView.Address));
        answerViewMock.Verify(x =>
            x.Configure(
                _answerAsset.Object,
                _riddleTextAsset.Text,
                It.IsAny<Color>(),
                characterSprite,
                It.IsAny<Action>(),
                It.IsAny<Action>()));
        answerViewMock.Verify(x => x.Display());
        iconMock.Verify(x => x.GetIcon(It.IsAny<HuntProductCacheConfig>()));
        _gameCanvasController.Verify(x => x.AttachViewToCanvas(answerViewMock.Object, 1));
    }
    
     [Test]
    public void TestEndStep()
    {
        //Given a started DisplayRiddleWithMultipleChoiceStepController
        //When EndStep is called
        //Then the view is hidden and the stepEnded function is invoked.
      
        //Arrange
        var characterSprite = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        _spriteHelperMock.Setup(x => x.GetSpriteFromByteArray(rawIcon)).Returns(characterSprite).Verifiable();
        
        var answerViewMock = new Mock<IWispContainerAnswerView>();
        answerViewMock.Setup(x =>
            x.Configure(
                _answerAsset.Object,
                _riddleTextAsset.Text,
                It.IsAny<Color>(),
                characterSprite,
                It.IsAny<Action>(),
                It.IsAny<Action>()))
            .Callback<IMultipleChoiceTextAnswerAsset, string, Color, Sprite, Action, Action>(
                (answerAsset, riddleText, frameColor, icon, accept, abort) =>
                {
                    accept.Invoke();
                    abort.Invoke();
                }).Verifiable();
        answerViewMock.Setup(x => x.Hide()).Verifiable();

        _stepDataConverter.Setup(x => x.ConvertToDisplayRiddleWithMultipleChoice(_stepModelMock.Object))
            .Returns(_stepModelMock.Object).Verifiable();
        
        _viewCollector.Setup(x=> x.AnswerView(_answerResource.AnswerView.Address)).ReturnsAsync(answerViewMock.Object).Verifiable();

        _gameCanvasController.Setup(x => x.AttachViewToCanvas(answerViewMock.Object, 1));

        int timesInvoked = 0;
        bool stepEndedValue = false;
        Action stepEnded = () =>
        {
            timesInvoked++;
            stepEndedValue = true;
        };

        var sut = new DisplayRiddleWithMultipleChoiceStepController(_viewCollector.Object, _stepDataConverter.Object, _spriteHelperMock.Object);

        //Act
        sut.StartStep(_stepModelMock.Object, _gameCanvasController.Object, stepEnded);
        
        //Assert
        Assert.AreEqual(2, timesInvoked);
        answerViewMock.Verify(x => x.Hide(), Times.Exactly(2));
        Assert.IsTrue(stepEndedValue);
    }
}
