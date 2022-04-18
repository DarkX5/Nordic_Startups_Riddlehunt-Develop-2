using System;
using System.Collections;
using System.Collections.Generic;
using Riddlehouse.Core.Helpers.Helpers;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;
using riddlehouse_libraries.products.models;
using UnityEngine;

[TestFixture]
public class TestTabComponent
{
    public TabComponent _testTabComponent;
    public Mock<ITabBarController> _tabBarController;
    public Mock<IViewActions> _viewComponentActionsMock;
    public Mock<ITabComponentActions> _tabComponentActionsMock;
    public Mock<ITabBtn> _tabBtnMock;
    public RectTransform _rectTransformParent;
    public RectTransform _rectTransformChild;
    public RectTransform _contentParent;
    public Mock<IUIFitters> _uiFittersMock;
    public ComponentType key = ComponentType.Story;

    [SetUp]
    public void Init()
    {
        key = ComponentType.Story;

        _rectTransformParent = new GameObject().AddComponent<RectTransform>();

        _rectTransformChild = new GameObject().AddComponent<RectTransform>();

        _contentParent = new GameObject().AddComponent<RectTransform>();

        _viewComponentActionsMock = new Mock<IViewActions>();
        _viewComponentActionsMock
            .Setup(x => x.GetComponentType())
            .Returns(key)
            .Verifiable();

        _viewComponentActionsMock
            .Setup(x => x.GetRectTransform())
            .Returns(_rectTransformChild)
            .Verifiable();

        _uiFittersMock = new Mock<IUIFitters>();
        _uiFittersMock.Setup(x => x.FitToFullscreen(_rectTransformChild, _contentParent)).Verifiable();

        _tabBtnMock = new Mock<ITabBtn>();
        _tabBtnMock.Setup(x => x.Configure(key.ToString(), key, It.IsAny<Action<ComponentType>>())).Verifiable();

        _tabBarController = new Mock<ITabBarController>();
        
        _tabComponentActionsMock = new Mock<ITabComponentActions>();
        _tabComponentActionsMock.Setup(x => x.GetContentParent()).Returns(_contentParent).Verifiable();
        _tabComponentActionsMock.Setup(x => x.GetTabBarController()).Returns(_tabBarController.Object).Verifiable();
        _testTabComponent = new TabComponent(_tabComponentActionsMock.Object, _uiFittersMock.Object);
    }

    [Test]
    public void TestFactory_Creates_New_Instance()
    {
        // Given a game object with a TabComponentBehaviour component
        // When Factory() is called
        // Then return a new instance of TabComponent that references the given component

        // Arrange
        var testGo = new GameObject();
        testGo.AddComponent<RectTransform>();
        var componentBehaviour = testGo.AddComponent<TabComponentBehaviour>();
        
        // Act
        var sut = TabComponent.Factory(testGo, _rectTransformParent);

        // Assert
        Assert.IsNotNull(sut);
        Assert.IsNotNull(componentBehaviour._tabComponent);
    }

    [Test]
    public void TestFactory_Throws_ArgumentException_If_GameObject_Does_Not_Have_Expected_Component()
    {
        // Given a game object without a TabComponentBehaviour component
        // When Factory() is called
        // Then an argument exception is thrown

        // Arrange
        GameObject testGo = new GameObject();
        testGo.AddComponent<RectTransform>();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => TabComponent.Factory(testGo, _rectTransformParent));
    }

    [TearDown]
    public void Complete()
    {
        _testTabComponent = null;
        _viewComponentActionsMock = null;
        _rectTransformParent = null;
        _rectTransformChild = null;
        _uiFittersMock = null;
    }

    [Test]
    public void TestConfigureTabs_Adds_A_List_Of_Components_To_Tabs()
    {
        //Given a hunt is started.
        //When setting up the tabcomponent
        //Then the ConfigureTabs are called with the needed TabComponents
        // -- and added/fitted to the scene/tab
        
        //Arrange
        var sut = _testTabComponent;
        
        var homeComponentUIActionsMock = new Mock<IViewActions>();
        homeComponentUIActionsMock
            .Setup(x => x.GetComponentType())
            .Returns(ComponentType.HuntHome)
            .Verifiable();
        homeComponentUIActionsMock.Setup(x => x.GetRectTransform()).Returns(_rectTransformChild);

        var endComponentUIActionsMock = new Mock<IViewActions>();
        endComponentUIActionsMock
            .Setup(x => x.GetComponentType())
            .Returns(ComponentType.End)
            .Verifiable();

        endComponentUIActionsMock.Setup(x => x.GetRectTransform()).Returns(_rectTransformChild).Verifiable();
        
        var uiComponentsList = new List<IViewActions>() { _viewComponentActionsMock.Object, homeComponentUIActionsMock.Object, endComponentUIActionsMock.Object};
        
        sut.ConfigureTabs(uiComponentsList);

        Assert.IsTrue(sut.HasTypeInViews(ComponentType.HuntHome));
        Assert.IsTrue(sut.HasTypeInViews(ComponentType.Story));
    }

    [Test]
    public void TestAdd_ViewComponent_To_TabView()
    {
        //Given a hunt is started.
        //When a new ViewComponent is needed to facilitate the step.
        //Then the ViewComponent is added to the ActiveViewComponents,
        //-- and a new tabBtn is added to tabs and configured.

        //Arrange
        var sut = _testTabComponent;

        //Act
        sut.Add(_viewComponentActionsMock.Object);

        //Assert
        Assert.AreSame(_viewComponentActionsMock.Object, sut.GetComponentUIAction(key));
        _viewComponentActionsMock.Verify(x => x.GetComponentType());
        _uiFittersMock.Setup(x => x.FitToFullscreen(_rectTransformChild, _contentParent));
        _viewComponentActionsMock.Verify(x => x.GetRectTransform());
        _tabComponentActionsMock.Verify(x => x.GetContentParent());
        _tabComponentActionsMock.Verify(x => x.GetTabBarController());
        Assert.IsTrue(sut.HasTypeInViews(key));
    }

    [Test]
    public void TestConfigureForStepType()
    {
        //Given a hunt is ongoing
        //When a step is started
        //Then the tabcomponent updates the tabButtons for possible steps.
        
        //Arrange
        var sut = _testTabComponent;
        
        var stepController = new Mock<IOldStepController>();
        List<ComponentType> typesInOrder = new List<ComponentType>()
            {ComponentType.Story, ComponentType.Riddle};
        
        stepController.Setup(x => x.GetTypesInOrder()).Returns(typesInOrder).Verifiable();
        stepController.Setup(x => x.GetFirstStepTypeToShow()).Returns(ComponentType.Story).Verifiable();
        
        _tabBarController.Setup(x => x.ConfigureNeededButtons( typesInOrder)).Verifiable();
        _tabBarController.Setup(x => x.UpdateButtonStates(ComponentType.Story)).Verifiable();
        
        //Act
        sut.ConfigureForStepType(stepController.Object);
        //Assert
        stepController.Verify(x => x.GetTypesInOrder());
        stepController.Verify(x => x.GetFirstStepTypeToShow());

    }

    [Test]
    public void TestAdd_ViewComponent_To_TabView_Twice_Throws_ArgumentException()
    {
        //Given a huntView that already has a ViewComponent of type X.
        //When a new ViewComponent is added of type X.
        //Then system throws and argument exception.

        //Arrange
        var sut = _testTabComponent;
        sut.Add(_viewComponentActionsMock.Object);

        //Act & Assert
        Assert.Throws<ArgumentException>(() => sut.Add(_viewComponentActionsMock.Object));
    }

    [Test]
    public void TestGetComponentUIAction_HasType()
    {
        //Given a huntView with ViewComponent of Type X.
        //When GetComponentUIAction is called
        //Then the function returns that ViewComponent.
        //Arrange
        var sut = _testTabComponent;
        sut.Add(_viewComponentActionsMock.Object);

        //Act & Assert
        Assert.IsNotNull(sut.GetComponentUIAction(key));
        _viewComponentActionsMock.Verify(x => x.GetComponentType());
    }

    [Test]
    public void TestAddHome_Home_Is_Added_On_Hunt_Startup()
    {
        //Given a new hunt is started
        //When the tabcomponent is created
        //The homeComponent is connected to the home button on the UI.
        
        //Arrange
        var homeComponentUIActionsMock = new Mock<IViewActions>();
        homeComponentUIActionsMock
            .Setup(x => x.GetComponentType())
            .Returns(ComponentType.HuntHome)
            .Verifiable();

        homeComponentUIActionsMock.Setup(x => x.GetRectTransform()).Returns(_rectTransformChild).Verifiable();
        
        //Act
        var sut = _testTabComponent;
        sut.AddHome(homeComponentUIActionsMock.Object);
        
        //Assert
        homeComponentUIActionsMock.Verify(x => x.GetRectTransform());
        _uiFittersMock.Verify(x => x.FitToFullscreen(_rectTransformChild, _contentParent));
        Assert.IsTrue(sut.HasTypeInViews(ComponentType.HuntHome));
    }
    
    [Test]
    public void TestHomeAction_CallsDisplayOnHomeScreen()
    {
        //Given a huntView with a tabView
        //When the homeButton is pressed
        //Then the Display action on "home" is called.
        
        //Arrange
        var homeComponentUIActionsMock = new Mock<IViewActions>();
        homeComponentUIActionsMock
            .Setup(x => x.GetComponentType())
            .Returns(ComponentType.HuntHome)
            .Verifiable();
        homeComponentUIActionsMock.Setup(x => x.Display()).Verifiable();

        homeComponentUIActionsMock.Setup(x => x.GetRectTransform()).Returns(_rectTransformChild);
        
        //Act
        var sut = _testTabComponent;
        sut.AddHome(homeComponentUIActionsMock.Object);
        
        sut.HomeAction();
        
        //Assert
        homeComponentUIActionsMock.Verify(x => x.Display());
    }
    [Test]
    public void TestAddEnd_End_Is_Added_On_Hunt_Startup()
    {
        //Given a new hunt is started
        //When the tabcomponent is created
        //The EndCompoment is connected to the end tab on the UI.
        
        //Arrange
        var endComponentUIActionsMock = new Mock<IViewActions>();
        endComponentUIActionsMock
            .Setup(x => x.GetComponentType())
            .Returns(ComponentType.End)
            .Verifiable();

        endComponentUIActionsMock.Setup(x => x.GetRectTransform()).Returns(_rectTransformChild).Verifiable();
        
        //Act
        var sut = _testTabComponent;
        sut.AddEnd(endComponentUIActionsMock.Object);
        
        //Assert
        endComponentUIActionsMock.Verify(x => x.GetRectTransform());
        _uiFittersMock.Verify(x => x.FitToFullscreen(_rectTransformChild, _contentParent));
        Assert.IsTrue(sut.HasTypeInViews(ComponentType.End));
    }

    [Test]
    public void TestAddEnd_Incorrect_Type_Throws()
    {
        //Given a new hunt is started
        //When the tabcomponent is created and a non-end type is added to the end view.
        //Then an argument exception is thrown.
        
        //Arrange
        var homeComponentUIActionsMock = new Mock<IViewActions>();
        homeComponentUIActionsMock
            .Setup(x => x.GetComponentType())
            .Returns(ComponentType.HuntHome)
            .Verifiable();

        homeComponentUIActionsMock.Setup(x => x.GetRectTransform()).Returns(_rectTransformChild).Verifiable();
        
        //Act
        var sut = _testTabComponent;
        //Assert
       Assert.Throws<ArgumentException>(() => sut.AddEnd(homeComponentUIActionsMock.Object));
    }
    [Test]
    public void TestAddHome_Incorrect_Type_Throws()
    {
        //Given a new hunt is started
        //When the tabcomponent is created and a non-home type is added to the home view.
        //Then an argument exception is thrown.
        
        //Arrange
        var endComponentUIActionsMock = new Mock<IViewActions>();
        endComponentUIActionsMock
            .Setup(x => x.GetComponentType())
            .Returns(ComponentType.End)
            .Verifiable();

        endComponentUIActionsMock.Setup(x => x.GetRectTransform()).Returns(_rectTransformChild).Verifiable();
        
        //Act
        var sut = _testTabComponent;
        //Assert
        Assert.Throws<ArgumentException>(() => sut.AddHome(endComponentUIActionsMock.Object));
    }
}