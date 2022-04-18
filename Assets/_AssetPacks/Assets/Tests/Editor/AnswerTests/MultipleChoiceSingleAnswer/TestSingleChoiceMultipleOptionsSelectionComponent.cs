using System.Collections;
using System.Collections.Generic;
using Components.Selection.MultipleChoice;
using Moq;
using NUnit.Framework;
using Prefabs.UI.CharacterSelection.StyledCharacterSelection.Scripts;
using riddlehouse_libraries.products.resources;
using TMPro;
using UnityEngine;

[TestFixture]
public class TestSingleChoiceMultipleOptionsSelectionComponent
{
    private SingleChoiceMultipleOptionsSelectionComponent.Config _config;
    private RHColor _selectedColor;
    private RHColor _unselectedColor;
    private SingleChoiceMultipleOptionsSelectionComponent.Dependencies CreateDependencies(Mock<ISelectionButtonInstantiater> selectionButtonInstantiaterMock = null)
    {
        selectionButtonInstantiaterMock ??= new Mock<ISelectionButtonInstantiater>();
        return new SingleChoiceMultipleOptionsSelectionComponent.Dependencies()
        {
            SelectionButtonInstantiater = selectionButtonInstantiaterMock.Object
        };
    }

    [SetUp]
    public void Init()
    {
        _selectedColor = RHColor.Black;
        _unselectedColor = RHColor.White;
        _config = new SingleChoiceMultipleOptionsSelectionComponent.Config()
        {
            Options = new List<string> {"A", "B", "C"}
        };
    }

    [TearDown]
    public void TearDown()
    {
        
    }
    [Test]
    public void TestSetDependencies()
    {
        //Arrange
        var go = new GameObject();
        go.AddComponent<RectTransform>();
        var sut = go.AddComponent<SingleChoiceMultipleOptionsSelectionComponent>();
        var dependencies = CreateDependencies();
        //Act
        sut.SetDependencies(dependencies);
        //Assert
        Assert.AreEqual(dependencies, sut._dependencies);
    }

    [Test]
    public void TestConfigure_FirstCall_CreateOptionButton_ForEveryOption_In_Config()
    {
        //Arrange
        var go = new GameObject();
        go.AddComponent<RectTransform>();
        var sut = go.AddComponent<SingleChoiceMultipleOptionsSelectionComponent>();

        var buttonMockA = new Mock<ISelectionButton>();
        buttonMockA.Setup(x => x.Hide()).Verifiable();
        buttonMockA.Setup(x => x.Configure(It.IsAny<SelectableTagButton.Config>())).Verifiable();
        
        var buttonMockB = new Mock<ISelectionButton>();
        buttonMockB.Setup(x => x.Hide()).Verifiable();
        buttonMockB.Setup(x => x.Configure(It.IsAny<SelectableTagButton.Config>())).Verifiable();
        
        var buttonMockC = new Mock<ISelectionButton>();
        buttonMockC.Setup(x => x.Hide()).Verifiable();
        buttonMockC.Setup(x => x.Configure(It.IsAny<SelectableTagButton.Config>())).Verifiable();
        
        var buttonInstantiatorMock = new Mock<ISelectionButtonInstantiater>();
        buttonInstantiatorMock.SetupSequence(x => x.Create())
            .Returns(buttonMockA.Object)
            .Returns(buttonMockB.Object)
            .Returns(buttonMockC.Object);
            
        var dependencies = CreateDependencies(buttonInstantiatorMock);
        sut.SetDependencies(dependencies);
        
        //Act
        sut.Configure(_config);
        
        //Assert
        buttonMockA.Verify(x => x.Hide(), Times.Once);
        buttonMockA.Verify(x => x.Configure(It.IsAny<SelectableTagButton.Config>()), Times.Once);
        
        buttonMockB.Verify(x => x.Hide(), Times.Once);
        buttonMockB.Verify(x => x.Configure(It.IsAny<SelectableTagButton.Config>()), Times.Once);
        
        buttonMockC.Verify(x => x.Hide(), Times.Once);
        buttonMockC.Verify(x => x.Configure(It.IsAny<SelectableTagButton.Config>()), Times.Once);
        
        buttonInstantiatorMock.Verify(x => x.Create(), Times.Exactly(3));
    }
    [Test]
    public void TestConfigure_SecondCall_Create_new_OptionButtons_For_AdditionalOptions_In_Config()
    {
        //Arrange
        var go = new GameObject();
        go.AddComponent<RectTransform>();
        var sut = go.AddComponent<SingleChoiceMultipleOptionsSelectionComponent>();

        var buttonMockA = new Mock<ISelectionButton>();
        buttonMockA.Setup(x => x.Hide()).Verifiable();
        buttonMockA.Setup(x => x.Configure(It.IsAny<SelectableTagButton.Config>())).Verifiable();
        
        var buttonMockB = new Mock<ISelectionButton>();
        buttonMockB.Setup(x => x.Hide()).Verifiable();
        buttonMockB.Setup(x => x.Configure(It.IsAny<SelectableTagButton.Config>())).Verifiable();
        
        var buttonMockC = new Mock<ISelectionButton>();
        buttonMockC.Setup(x => x.Hide()).Verifiable();
        buttonMockC.Setup(x => x.Configure(It.IsAny<SelectableTagButton.Config>())).Verifiable();
        
        var buttonMockD = new Mock<ISelectionButton>();
        buttonMockD.Setup(x => x.Hide()).Verifiable();
        buttonMockD.Setup(x => x.Configure(It.IsAny<SelectableTagButton.Config>())).Verifiable();
        
        var buttonInstantiatorMock = new Mock<ISelectionButtonInstantiater>();
        buttonInstantiatorMock.SetupSequence(x => x.Create())
            .Returns(buttonMockA.Object)
            .Returns(buttonMockB.Object)
            .Returns(buttonMockC.Object)
            .Returns(buttonMockD.Object);
            
            
        var dependencies = CreateDependencies(buttonInstantiatorMock);
        sut.SetDependencies(dependencies);
        
        sut.Configure(_config);
        _config.Options.Add("D");
        //Act
        sut.Configure(_config);

        //Assert
        buttonMockA.Verify(x => x.Hide(), Times.Exactly(2));
        buttonMockA.Verify(x => x.Configure(It.IsAny<SelectableTagButton.Config>()), Times.Exactly(2));
        
        buttonMockB.Verify(x => x.Hide(), Times.Exactly(2));
        buttonMockB.Verify(x => x.Configure(It.IsAny<SelectableTagButton.Config>()), Times.Exactly(2));
        
        buttonMockC.Verify(x => x.Hide(), Times.Exactly(2));
        buttonMockC.Verify(x => x.Configure(It.IsAny<SelectableTagButton.Config>()), Times.Exactly(2));
        
        buttonMockD.Verify(x => x.Hide(), Times.Once);
        buttonMockD.Verify(x => x.Configure(It.IsAny<SelectableTagButton.Config>()), Times.Once);
        
        buttonInstantiatorMock.Verify(x => x.Create(), Times.Exactly(4));
    }
    [Test]
    public void TestConfigure_SecondCall_HideSuperflous_OptionButtons_BasedOn_Options_In_Config()
    {
        //Arrange
        var go = new GameObject();
        go.AddComponent<RectTransform>();
        var sut = go.AddComponent<SingleChoiceMultipleOptionsSelectionComponent>();

        var buttonMockA = new Mock<ISelectionButton>();
        buttonMockA.Setup(x => x.Hide()).Verifiable();
        buttonMockA.Setup(x => x.Configure(It.IsAny<SelectableTagButton.Config>())).Verifiable();
        
        var buttonMockB = new Mock<ISelectionButton>();
        buttonMockB.Setup(x => x.Hide()).Verifiable();
        buttonMockB.Setup(x => x.Configure(It.IsAny<SelectableTagButton.Config>())).Verifiable();
        
        var buttonMockC = new Mock<ISelectionButton>();
        buttonMockC.Setup(x => x.Hide()).Verifiable();
        buttonMockC.Setup(x => x.Configure(It.IsAny<SelectableTagButton.Config>())).Verifiable();
        
        var buttonInstantiatorMock = new Mock<ISelectionButtonInstantiater>();
        buttonInstantiatorMock.SetupSequence(x => x.Create())
            .Returns(buttonMockA.Object)
            .Returns(buttonMockB.Object)
            .Returns(buttonMockC.Object);
            
        var dependencies = CreateDependencies(buttonInstantiatorMock);
        sut.SetDependencies(dependencies);
        
        sut.Configure(_config);
        _config.Options.RemoveAt(2);
        //Act
        sut.Configure(_config);

        //Assert
        buttonMockA.Verify(x => x.Hide(), Times.Exactly(2));
        buttonMockA.Verify(x => x.Configure(It.IsAny<SelectableTagButton.Config>()), Times.Exactly(2));
        
        buttonMockB.Verify(x => x.Hide(), Times.Exactly(2));
        buttonMockB.Verify(x => x.Configure(It.IsAny<SelectableTagButton.Config>()), Times.Exactly(2));
        
        buttonMockC.Verify(x => x.Hide(), Times.Exactly(2));
        buttonMockC.Verify(x => x.Configure(It.IsAny<SelectableTagButton.Config>()), Times.Once());
        
        buttonInstantiatorMock.Verify(x => x.Create(), Times.Exactly(3));
    }

    [Test]
    public void TestDefineOptionChosen_HasOption_CallsSelectButtonInButton_ResetsAllNon_Options()
    {
        //Arrange
        var go = new GameObject();
        go.AddComponent<RectTransform>();
        var sut = go.AddComponent<SingleChoiceMultipleOptionsSelectionComponent>();

        var buttonMockA = new Mock<ISelectionButton>();
        buttonMockA.Setup(x => x.Hide()).Verifiable();
        buttonMockA.Setup(x => x.Configure(It.IsAny<SelectableTagButton.Config>()));
        buttonMockA.Setup(x => x.SelectButton()).Verifiable();
    
        var buttonMockB = new Mock<ISelectionButton>();
        buttonMockB.Setup(x => x.Hide()).Verifiable();
        buttonMockB.Setup(x => x.Configure(It.IsAny<SelectableTagButton.Config>()));
        buttonMockA.Setup(x => x.ResetButton()).Verifiable();

        var buttonMockC = new Mock<ISelectionButton>();
        buttonMockC.Setup(x => x.Hide()).Verifiable();
        buttonMockC.Setup(x => x.Configure(It.IsAny<SelectableTagButton.Config>()));
        buttonMockA.Setup(x => x.ResetButton()).Verifiable();

        var buttonInstantiatorMock = new Mock<ISelectionButtonInstantiater>();
        buttonInstantiatorMock.SetupSequence(x => x.Create())
            .Returns(buttonMockA.Object)
            .Returns(buttonMockB.Object)
            .Returns(buttonMockC.Object);
        
        var dependencies = CreateDependencies(buttonInstantiatorMock);
        sut.SetDependencies(dependencies);
        sut.Configure(_config);
        //Act
        sut.DefineOptionChosen(_config.Options[0]);
    
        //Assert
        buttonMockA.Verify(x => x.SelectButton());
        buttonMockB.Verify(x => x.ResetButton());
        buttonMockC.Verify(x => x.ResetButton());

    }
}
