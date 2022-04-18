using System;
using Answers.MultipleChoice.Buttons.Icon;
using Answers.MultipleChoice.Data.Icon;
using Moq;
using MultipleChoice.Icon.Transitions;
using NUnit.Framework;
using UI.Answers;
using UnityEngine;
using UnityEngine.UI;

[TestFixture]
public class TestMultipleChoiceIconButton
{
    private MultipleChoiceOldIconButton.Dependencies _dependencies;
    private Image _image;
    private Image _iconBackground;
    private Button _button;
    private MultipleChoiceOldIconButton.Config _config;
    private Mock<IMultipleChoiceIconStateTransitions> transitionsMock;
    [SetUp]
    public void Init()
    {
        transitionsMock = new Mock<IMultipleChoiceIconStateTransitions>();
        _image = new GameObject().AddComponent<Image>();
        _iconBackground = new GameObject().AddComponent<Image>();
        _button = new GameObject().AddComponent<Button>();
        _dependencies = new MultipleChoiceOldIconButton.Dependencies()
        {
            Transitions = transitionsMock.Object,
            IconImage = _image,
            IconBackground = _iconBackground,
            Button = _button
        };
        
        _config = new MultipleChoiceOldIconButton.Config()
        {
            iconWithValue = new IconWithValue()
            {
                Correct = false,
                Icon = null,
                Value = null
            },
            SelectionToggleAction = null
        };
    }
    [TearDown]
    public void TearDown()
    {
        _dependencies = null;
        _image = null;
        _config = null;
        transitionsMock = null;
    }

    [Test]
    public void TestSetDependencies_Sets_IconImage()
    {
        //Given a new MultipleChoiceIconButton.
        //When SetDependencies are called with an instance of it's dependencies.
        //Then those dependencies are set correctly.
        
        //Arrange
        var sut = new GameObject().AddComponent<MultipleChoiceOldIconButton>();
        
        //Act
        sut.SetDependencies(_dependencies);
        
        //Assert
        Assert.AreSame(_dependencies, sut._dependencies);
    }
    
    [Test]
    public void TestConfigure_SetsIconAndButtonClickedAction()
    {
        //Given an existing MultipleChoiceIconButton, and a Config instance.
        //When configure is called with that instance.
        //Then the button is configured correctly.
        
        //Arrange
        var sut = new GameObject().AddComponent<MultipleChoiceOldIconButton>();
        
        sut.SetDependencies(_dependencies);
        
        var sprite = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        string value = "a";
        
        _config = new MultipleChoiceOldIconButton.Config()
        {
            iconWithValue = new IconWithValue()
            {
                Correct = false,
                Icon = sprite,
                Value = value
            },
            SelectionToggleAction = null
        };

        //Act
        sut.Configure(_config);
        
        //Assert
        Assert.AreSame(sprite, sut._dependencies.IconImage.sprite);
    }

    [Test]
    public void TestButtonClickedAction_PressesButton_ActionYieldsValue_Correct()
    {
        //Given a configured MultipleChoiceIconButton.
        //When the buttonClickedAction is called.
        //Then the action is invoked with the configured value.
        
        //Arrange
        var sut = new GameObject().AddComponent<MultipleChoiceOldIconButton>();
        
        sut.SetDependencies(_dependencies);
        
        var sprite = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        string value = "correct";
        string callValue = "notCorrect";
        Action<IconWithValue> buttonClickedAction = (theValue) =>
        {
            callValue = theValue.Value;
        };
        
        _config = new MultipleChoiceOldIconButton.Config()
        {
            iconWithValue = new IconWithValue()
            {
                Correct = true,
                Icon = sprite,
                Value = value
            },
            SelectionToggleAction = buttonClickedAction,
            State = MultipleChoiceState.active
        };

        sut.Configure(_config);
        
        //Act
        sut.ButtonClickedAction();
        
        //Assert
        Assert.AreSame(value, callValue);
    }
    
    [Test]
    public void TestButtonClickedAction_PressesButton_StateIsSelected_ActionIs_Not_Invoked()
    {
        //Given a configured MultipleChoiceIconButton, with state of Selected.
        //When the buttonClickedAction is called.
        //Then the action is not invoked.
        
        //Arrange
        var sut = new GameObject().AddComponent<MultipleChoiceOldIconButton>();
        
        
        
        sut.SetDependencies(_dependencies);

        IconWithValue returnedIconWithValue = null;
        Action<IconWithValue> buttonClickedAction = (theValue) =>
        {
            returnedIconWithValue = theValue;
        };

        var iconWithValue = new IconWithValue()
        {
            Correct = true,
            Icon = null,
            Value = "correct"
        };
        _config = new MultipleChoiceOldIconButton.Config()
        {
            iconWithValue = iconWithValue,
            SelectionToggleAction = buttonClickedAction,
            State = MultipleChoiceState.selected
        };

        sut.Configure(_config);
        
        //Act
        sut.ButtonClickedAction();
        
        //Assert
        Assert.AreEqual(iconWithValue, returnedIconWithValue);
    }

    [Test]
    public void TestSelect_GameObject_Inactive_SetsState_Hidden_NoAnimation()
    {
        //Given an inactive MultipleChoiceIconButton.
        //When select is called.
        //Then the program sets state to hidden and stops.
        
        //Arrange
        var sut = new GameObject().AddComponent<MultipleChoiceOldIconButton>();
        
        transitionsMock.Setup(x => x.StartAnimateSelected(It.IsAny<Action>())).Verifiable();
        transitionsMock.Setup(x => x.StartAnimateDeselected(It.IsAny<Action>())).Verifiable();

        _dependencies = new MultipleChoiceOldIconButton.Dependencies()
        {
            Transitions = transitionsMock.Object,
            IconImage = _image
        };
        
        sut.SetDependencies(_dependencies);

        _config = new MultipleChoiceOldIconButton.Config()
        {
            iconWithValue = new IconWithValue()
            {
                Correct = false,
                Icon = null,
                Value = null
            },
            SelectionToggleAction = null,
            State = MultipleChoiceState.active
        };
        sut.gameObject.SetActive(false);

        sut.Configure(_config);
        
        //Act
        sut.Select();
        
        //Assert
        transitionsMock.Verify(x => x.StartAnimateSelected(It.IsAny<Action>()), Times.Never);
        transitionsMock.Verify(x => x.StartAnimateDeselected(It.IsAny<Action>()), Times.Never);

    }
    
    [Test]
    public void TestSelect_GameObject_Active_SetsState_Selected_HasAnimation()
    {
        //Given an active MultipleChoiceIconButton.
        //When Select is called.
        //Then the button animates to the selected state.
        
        //Arrange
        var sut = new GameObject().AddComponent<MultipleChoiceOldIconButton>();
        sut.gameObject.SetActive(true);

        transitionsMock.Setup(x => x.StartAnimateSelected(It.IsAny<Action>())).Verifiable();
        transitionsMock.Setup(x => x.StartAnimateDeselected(It.IsAny<Action>())).Verifiable();

        _dependencies = new MultipleChoiceOldIconButton.Dependencies()
        {
            Transitions = transitionsMock.Object,
            IconImage = _image
        };
        
        sut.SetDependencies(_dependencies);

        _config = new MultipleChoiceOldIconButton.Config()
        {
          iconWithValue = new IconWithValue()
            {
                Correct = false,
                Icon = null,
                Value = null
            },
            SelectionToggleAction = null,
            State = MultipleChoiceState.active
        };

        sut.Configure(_config);
        
        //Act
        sut.Select();
        
        //Assert
        transitionsMock.Verify(x => x.StartAnimateSelected(It.IsAny<Action>()), Times.Once);
        transitionsMock.Verify(x => x.StartAnimateDeselected(It.IsAny<Action>()), Times.Never);
    }
    
    [Test]
    public void TestDeselect_GameObject_Inactive_SetsState_hidden_NoAnimation()
    {
        //Given an active MultipleChoiceIconButton.
        //When Deselect is called with parameter false.
        //Then the program sets state to hidden and stops.
        
        //Arrange
        var sut = new GameObject().AddComponent<MultipleChoiceOldIconButton>();

        transitionsMock.Setup(x => x.StartAnimateSelected(It.IsAny<Action>())).Verifiable();
        transitionsMock.Setup(x => x.StartAnimateDeselected(It.IsAny<Action>())).Verifiable();

        _dependencies = new MultipleChoiceOldIconButton.Dependencies()
        {
            Transitions = transitionsMock.Object,
            IconImage = _image
        };
        
        sut.SetDependencies(_dependencies);

        _config = new MultipleChoiceOldIconButton.Config()
        {
            iconWithValue = new IconWithValue()
            {
                Correct = false,
                Icon = null,
                Value = null
            },
            SelectionToggleAction = null,
            State = MultipleChoiceState.active
        };

        sut.Configure(_config);

        //Act
        sut.Deselect(false);
        
        //Assert
        transitionsMock.Verify(x => x.StartAnimateSelected(It.IsAny<Action>()), Times.Never);
        transitionsMock.Verify(x => x.StartAnimateDeselected(It.IsAny<Action>()), Times.Never);
    }
    
    [Test]
    public void TestDeselect_GameObject_Active_SetsState_active_HasAnimation()
    {
        //Given an active MultipleChoiceIconButton.
        //When Deselect is called.
        //Then the button animates to the active state.
        
        //Arrange
        var sut = new GameObject().AddComponent<MultipleChoiceOldIconButton>();

        transitionsMock.Setup(x => x.StartAnimateSelected(It.IsAny<Action>())).Verifiable();
        transitionsMock.Setup(x => x.StartAnimateDeselected(It.IsAny<Action>())).Verifiable();

        _dependencies = new MultipleChoiceOldIconButton.Dependencies()
        {
            Transitions = transitionsMock.Object,
            IconImage = _image
        };
        
        sut.SetDependencies(_dependencies);

        _config = new MultipleChoiceOldIconButton.Config()
        {
            iconWithValue = new IconWithValue()
            {
                Correct = false,
                Icon = null,
                Value = null
            },
            State = MultipleChoiceState.active
        };

        sut.Configure(_config);

        //Act
        sut.Deselect(true);
        
        //Assert
        transitionsMock.Verify(x => x.StartAnimateSelected(It.IsAny<Action>()), Times.Never);
        transitionsMock.Verify(x => x.StartAnimateDeselected(It.IsAny<Action>()), Times.Once);
        Assert.AreEqual(MultipleChoiceState.active, _config.State);
    }

    [Test]
    public void TestReset_GameObject_Active_SetsState_Active_HasAnimation()
    {
        //Arrange
        var sut = new GameObject().AddComponent<MultipleChoiceOldIconButton>();

        transitionsMock.Setup(x => x.StartAnimateSelected(It.IsAny<Action>())).Verifiable();
        transitionsMock.Setup(x => x.StartAnimateDeselected(It.IsAny<Action>())).Verifiable();

        _dependencies = new MultipleChoiceOldIconButton.Dependencies()
        {
            Transitions = transitionsMock.Object,
            IconImage = _image,
            IconBackground = _iconBackground,
            Button = _button
        };
        
        sut.SetDependencies(_dependencies);

        _config = new MultipleChoiceOldIconButton.Config()
        {
            iconWithValue = new IconWithValue()
            {
                Correct = false,
                Value = null,
                Icon = null
            },
            SelectionToggleAction = null,
            State = MultipleChoiceState.selected
        };

        sut.Configure(_config);

        //Act
        sut.ResetState();
        
        //Assert
        transitionsMock.Verify(x => x.StartAnimateSelected(It.IsAny<Action>()), Times.Never);
        transitionsMock.Verify(x => x.StartAnimateDeselected(It.IsAny<Action>()), Times.Once);
    }
    [Test]
    public void TestReset_GameObject_Inactive_SetsState_Hidden_HasAnimation()
    {
        //Arrange
        
        transitionsMock.Setup(x => x.StartAnimateSelected(It.IsAny<Action>())).Verifiable();
        transitionsMock.Setup(x => x.StartAnimateDeselected(It.IsAny<Action>())).Verifiable();

        _dependencies = new MultipleChoiceOldIconButton.Dependencies()
        {
            Transitions = transitionsMock.Object,
            IconImage = _image,
            IconBackground = _iconBackground,
            Button = _button
        };
        
        var go = new GameObject();
        var sut = go.AddComponent<MultipleChoiceOldIconButton>();
        sut.SetDependencies(_dependencies);
        go.SetActive(false);
        
        sut.SetDependencies(_dependencies);

        _config = new MultipleChoiceOldIconButton.Config()
        {
            iconWithValue = new IconWithValue()
            {
                Correct = false,
                Icon = null,
                Value = null
            },
            SelectionToggleAction = null,
            State = MultipleChoiceState.active
        };

        sut.Configure(_config);

        //Act
        sut.ResetState();
        
        //Assert
        transitionsMock.Verify(x => x.StartAnimateSelected(It.IsAny<Action>()), Times.Never);
        transitionsMock.Verify(x => x.StartAnimateDeselected(It.IsAny<Action>()), Times.Never);
    }
}
