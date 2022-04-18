using System;
using System.Collections.Generic;
using Riddlehouse.Core.Helpers.Helpers;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;
using riddlehouse_libraries.products.Assets;
using riddlehouse_libraries.products.Assets.AnswerAssets;
using riddlehouse_libraries.products.AssetTypes;
using UI.Answers;
using UnityEngine;
using Lib_TextAsset = riddlehouse_libraries.products.Assets.TextAsset;

[TestFixture]
public class TestRiddleTabComponent
{
    private GameObject _go;
    private RiddleTabComponent _behaviour;
    private RectTransform _rectTransform;
    
    private Mock<IViewActions> _textUIActionsMock;
    private Mock<IViewActions> _multipleChoiceUIActionsMock;
    private Mock<IViewActions> _numericUIActionsMock;
    
    private Mock<IRiddleComponent> _riddleComponentMock;
    private Mock<ITextAnswerComponent> _textAnswerComponentMock;
    private Mock<IMultipleChoiceAnswerComponent> _multipleChoiceAnswerComponentMock;
    private Mock<INumericAnswerComponent> _numericAnswerComponentMock;
    private Mock<IImageGridComponentActions> _imageGridComponentActionsMock;

    private Sprite _img1;
    private Sprite _img2;
    private Sprite _img3;

    private List<Sprite> _imageList;
    
    private List<string> _multiChoice_options;
    private string _multiChoice_correctAnswer;

    private string String_Answer = "theAnswer";

    private float _numericAnswer = 42;
    private string _identifier;
    private string _seperator;

    [SetUp]
    public void Init()
    {
        _go = new GameObject();
        _rectTransform = _go.AddComponent<RectTransform>();
        _behaviour = _go.AddComponent<RiddleTabComponent>();
        _riddleComponentMock = new Mock<IRiddleComponent>();
        _textAnswerComponentMock = new Mock<ITextAnswerComponent>();
        _multipleChoiceAnswerComponentMock = new Mock<IMultipleChoiceAnswerComponent>();
        _numericAnswerComponentMock = new Mock<INumericAnswerComponent>();
        
        _textUIActionsMock = new Mock<IViewActions>();
        _multipleChoiceUIActionsMock = new Mock<IViewActions>();
        _numericUIActionsMock = new Mock<IViewActions>();
        _imageGridComponentActionsMock = new Mock<IImageGridComponentActions>();
        
        _img1 = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        _img2 = Sprite.Create(Texture2D.normalTexture, Rect.zero, Vector2.up);
        _img3 = Sprite.Create(Texture2D.redTexture, Rect.zero, Vector2.left);

        _imageList = new List<Sprite>() { _img1, _img2, _img3 };

        _multiChoice_options = new List<string>()
        {
            "1825",
            "1925",
            "1984"
        };
        _multiChoice_correctAnswer = "1825";
        
        _seperator = ";";
        _identifier = "test-riddletab-identifier";
        PlayerPrefs.DeleteKey(_identifier);
    }

    [TearDown]
    public void TearDown()
    {
        _go = null;
        _behaviour = null;
        _riddleComponentMock = null;
        _textAnswerComponentMock = null;
        _multipleChoiceAnswerComponentMock = null;
        _numericAnswerComponentMock = null;
        
        _textUIActionsMock = null;
        _multipleChoiceUIActionsMock = null;
        _numericUIActionsMock = null;
        _imageGridComponentActionsMock = null;

        _img1 = null;
        _img2 = null;
        _img3 = null;
        _imageList = null;
    }

    [Test]
    public void TestConfigure_InputTextField_AnswerType_Calls_In_Mock()
    {
        //Given a user is viewing a riddle with a textanswer
        //When riddleTab is configured
        //Then the answerField is configured.

        //Arrange
        string riddleText = "text";
        var textAsset = new Lib_TextAsset(AssetType.TextAnswer, String_Answer);
        var answerAsset = new TextAnswerAsset(textAsset);
        Action btnAction = () => { };
        
        var sut = _behaviour;
        _riddleComponentMock.Setup(x => x.Configure(riddleText)).Verifiable();
        _textAnswerComponentMock.Setup(x => x.Configure(answerAsset, btnAction)).Verifiable();
       
        _textAnswerComponentMock.Setup(x => x.GetComponentUIActions()).Returns(_textUIActionsMock.Object);
        _multipleChoiceAnswerComponentMock.Setup(x => x.GetComponentUIActions()).Returns(_multipleChoiceUIActionsMock.Object);
        _numericAnswerComponentMock.Setup(x => x.GetComponentUIActions()).Returns(_numericUIActionsMock.Object);
        
        _textUIActionsMock.Setup(x => x.Hide()).Verifiable();
        _multipleChoiceUIActionsMock.Setup(x => x.Hide()).Verifiable();
        _numericUIActionsMock.Setup(x => x.Hide()).Verifiable();
        _imageGridComponentActionsMock.Setup(x => x.Hide()).Verifiable();
        
        _textUIActionsMock.Setup(x => x.Display()).Verifiable();
        
        sut.SetDependencies(_rectTransform, _riddleComponentMock.Object, _textAnswerComponentMock.Object, _multipleChoiceAnswerComponentMock.Object, _numericAnswerComponentMock.Object, _imageGridComponentActionsMock.Object);
        
        //Act
        sut.Configure(riddleText, answerAsset, _imageList, btnAction);
        
        //Assert
        _riddleComponentMock.Verify(x => x.Configure(riddleText));
        _textAnswerComponentMock.Verify(x => x.Configure(answerAsset, btnAction));
        
        _textAnswerComponentMock.Verify(x=>x.GetComponentUIActions());
        _multipleChoiceAnswerComponentMock.Verify(x=>x.GetComponentUIActions());
        _numericAnswerComponentMock.Verify(x=>x.GetComponentUIActions());
        
        _textUIActionsMock.Verify(x => x.Hide());
        _multipleChoiceUIActionsMock.Verify(x => x.Hide());
        _numericUIActionsMock.Verify(x=>x.Hide());
        _textUIActionsMock.Verify(x => x.Display());
    }
    
    [Test]
    public void TestConfigure_NumericAnswer_AnswerType_Calls_In_Mock()
    {
        //Given a user is viewing a riddle with a numeric answer
        //When riddleTab is configured
        //Then the answerField is configured for numeric answer component.
        
        //Arrange
        string riddleText = "text";
        var answerAsset = new NumericAnswerAsset(_numericAnswer);
        Action btnAction = () => { };
        
        var sut = _behaviour;
        _riddleComponentMock.Setup(x => x.Configure(riddleText)).Verifiable();
        _numericAnswerComponentMock.Setup(x => x.Configure(answerAsset, btnAction)).Verifiable();

        _textAnswerComponentMock.Setup(x => x.GetComponentUIActions()).Returns(_textUIActionsMock.Object);
        _multipleChoiceAnswerComponentMock.Setup(x => x.GetComponentUIActions()).Returns(_multipleChoiceUIActionsMock.Object);
        _numericAnswerComponentMock.Setup(x => x.GetComponentUIActions()).Returns(_numericUIActionsMock.Object);
        
        _numericUIActionsMock.Setup(x => x.Display()).Verifiable();
        
        sut.SetDependencies(_rectTransform, _riddleComponentMock.Object, _textAnswerComponentMock.Object, _multipleChoiceAnswerComponentMock.Object, _numericAnswerComponentMock.Object, _imageGridComponentActionsMock.Object);
        //Act
        sut.Configure(riddleText, answerAsset, _imageList, btnAction);
        
        //Assert
        _riddleComponentMock.Verify(x => x.Configure(riddleText));
        _numericAnswerComponentMock.Verify(x => x.Configure(answerAsset, btnAction));
        _numericUIActionsMock.Setup(x => x.Display());
    }
    
    [Test]
    public void TestConfigure_MultipleChoiceAnswer_AnswerType_Calls_In_Mock()
    {
        //Given a user is viewing a riddle with a multiplechoice text answer
        //When riddleTab is configured
        //Then the answerField is configured.
        
        //Arrange
        string riddleText = "text";
        var options = new MultipleChoiceTextAnswerOptions()
        {
            AnswerOptions = _multiChoice_options,
            CorrectAnswer = _multiChoice_correctAnswer,
            Seperator = _seperator,
            Evaluation = MultipleChoiceLogic.ContainsAll
        };
        var answerAsset = new MultipleChoiceTextAnswerAsset(options);
       
        Action btnAction = () => { };
        
        var sut = _behaviour;
        _riddleComponentMock.Setup(x => x.Configure(riddleText)).Verifiable();
        _multipleChoiceAnswerComponentMock.Setup(x => x.Configure(answerAsset, btnAction)).Verifiable();
        _textAnswerComponentMock.Setup(x => x.GetComponentUIActions()).Returns(_textUIActionsMock.Object);
        _multipleChoiceAnswerComponentMock.Setup(x => x.GetComponentUIActions()).Returns(_multipleChoiceUIActionsMock.Object);
        _numericAnswerComponentMock.Setup(x => x.GetComponentUIActions()).Returns(_numericUIActionsMock.Object);
        _multipleChoiceUIActionsMock.Setup(x => x.Display());
        
        sut.SetDependencies(_rectTransform, _riddleComponentMock.Object, _textAnswerComponentMock.Object, _multipleChoiceAnswerComponentMock.Object, _numericAnswerComponentMock.Object, _imageGridComponentActionsMock.Object);
        //Act
        sut.Configure(riddleText, answerAsset, _imageList, btnAction);
        
        //Assert
        _riddleComponentMock.Verify(x => x.Configure(riddleText));
        _multipleChoiceAnswerComponentMock.Verify(x => x.Configure(answerAsset, btnAction));
        _multipleChoiceUIActionsMock.Verify(x => x.Display());
    }

    [Test]
    public void TestConfigure_WrongAnswerType_Throws()
    {
        //Given a user is viewing a riddle with a non-compatible answer type
        //When riddleTab is configured
        //Then the system throws an exception
        
        //Arrange
        string riddleText = "text";
        BooleanAnswerData answerData = new BooleanAnswerData(_identifier);
        Action btnAction = () => { };
        
        var sut = _behaviour;
        _textAnswerComponentMock.Setup(x => x.GetComponentUIActions()).Returns(_textUIActionsMock.Object);
        _multipleChoiceAnswerComponentMock.Setup(x => x.GetComponentUIActions()).Returns(_multipleChoiceUIActionsMock.Object);
        _numericAnswerComponentMock.Setup(x => x.GetComponentUIActions()).Returns(_numericUIActionsMock.Object);

        _riddleComponentMock.Setup(x => x.Configure(riddleText));
        sut.SetDependencies(_rectTransform, _riddleComponentMock.Object, _textAnswerComponentMock.Object, _multipleChoiceAnswerComponentMock.Object, _numericAnswerComponentMock.Object, _imageGridComponentActionsMock.Object);
        //Act and Assert
        Assert.Throws<ArgumentException>( () =>  sut.Configure(riddleText, answerData, _imageList, btnAction));
    }

    [Test]
    public void TestFitInView()
    {
        //Given a new hunt is created
        //When hunt is configured
        //Then the UI is fitted to fullscreen
        
        //Arrange
        GameObject parentGo = new GameObject();
        var parent = parentGo.AddComponent<RectTransform>();
        var sut = _behaviour;
        var fitterMock = new Mock<IUIFitters>();
        fitterMock.Setup(x => x.FitToFullscreen(_rectTransform, parent)).Verifiable();
        sut.SetDependencies(_rectTransform, _riddleComponentMock.Object, _textAnswerComponentMock.Object, _multipleChoiceAnswerComponentMock.Object, _numericAnswerComponentMock.Object, _imageGridComponentActionsMock.Object);
        //Act
        sut.FitInView(parent, fitterMock.Object);
        //Assert
        fitterMock.Verify(x => x.FitToFullscreen(_rectTransform, parent));
    }
    
    [TestCase(0)]
    [TestCase(1)]
    [Test]
    public void TestFitInView_Sets_SibblingIndex(int index)
    {
        //Given the user requires a riddletabView
        //When FitInView is called with a parent and an index
        //Then view is fitted to that parent, and set to the given sibbling index.
        
        //Arrange
        GameObject parentGo = new GameObject();
        var parent = parentGo.AddComponent<RectTransform>();
        var otherChild = new GameObject();
        otherChild.transform.SetParent(parent);
        
        var sut = _behaviour;
        var fitterMock = new Mock<IUIFitters>();
        fitterMock.Setup(x => x.FitToFullscreen(_rectTransform, parent)).Verifiable();
        sut.SetDependencies(_rectTransform, _riddleComponentMock.Object, _textAnswerComponentMock.Object, _multipleChoiceAnswerComponentMock.Object, _numericAnswerComponentMock.Object, _imageGridComponentActionsMock.Object);
        //Act
        sut.FitInView(parent, fitterMock.Object, index);
        //Assert
        fitterMock.Verify(x => x.FitToFullscreen(_rectTransform, parent));
        Assert.AreEqual(sut.gameObject.transform.GetSiblingIndex(), index);
    }

    [Test]
    public void TestDisplay()
    {
        //Given the user navigates to the riddleTab
        //When display is called
        //Then the gameobject is set active.
        
        //Arrange
        var sut = _behaviour;
        sut.gameObject.SetActive(false);
        //Act
        sut.Display();
        //Assert
        Assert.IsTrue(sut.gameObject.activeSelf);
    }

    [Test]
    public void TestHide()
    {
        //Given the user navigates to the riddleTab
        //When hide is called
        //Then the gameobject is hidden.

        //Arrange
        var sut = _behaviour;
        sut.gameObject.SetActive(true);
        //Act
        sut.Hide();
        //Assert
        Assert.IsFalse(sut.gameObject.activeSelf);
    }

    [Test]
    public void TestIsShown_True()
    {
        //Given the user is viewing a riddleTab
        //When isShown is called
        //Then the gameobject is active.
                
        //Arrange
        var sut = _behaviour;
        sut.gameObject.SetActive(true);
        //Act & Assert
        Assert.IsTrue(sut.IsShown());
    }
    
    [Test]
    public void TestIsShown_False()
    {
        //Given the user isn't viewing a riddleTab
        //When isShown is called
        //Then the gameobject isn't active.
        
        //Arrange
        var sut = _behaviour;
        sut.gameObject.SetActive(false);
        //Act & Assert
        Assert.IsFalse(sut.IsShown());
    }

    [Test]
    public void TestGetComponentType_Returns_RiddleTab()
    {
        //Given the componentType is a riddleTab
        //When getter is called
        //Then the riddleTab type is returned
        //Arrange
        var sut = _behaviour;
        //Act & Assert
        Assert.AreEqual(ComponentType.RiddleTab, sut.GetComponentType());
    }

    [Test]
    public void GetRectTransform_Returns_Own_RectTransform()
    {
        //Given a riddletab
        //When GetRectTransform is called
        //Then returns the rectTransform
        //Arrange
        var sut = _behaviour;
        sut.SetDependencies(_rectTransform, _riddleComponentMock.Object, _textAnswerComponentMock.Object, _multipleChoiceAnswerComponentMock.Object, _numericAnswerComponentMock.Object, _imageGridComponentActionsMock.Object);
        //Act & Assert
        Assert.AreSame(_rectTransform, sut.GetRectTransform());
    }
}
