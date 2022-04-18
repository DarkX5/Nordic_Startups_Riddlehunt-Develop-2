using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSelection.Components;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products;
using riddlehouse_libraries.products.resources;
using UI.UITools;
using UnityEngine;

[TestFixture]
public class TestRoleSelectionComponent 
{
    private RoleSelectionComponent.Config _config;

    [SetUp]
    public void Init()
    {

        _config = new RoleSelectionComponent.Config()
        {
            Resource = new RoleSelectionComponentResource()
            {
                RoleButtons = new List<RoleButtonResource>()
                {
                    new RoleButtonResource(new Mock<IIcon>().Object)
                    {
                        Title = "title"
                    },
                    new RoleButtonResource(new Mock<IIcon>().Object)
                    {
                        Title = "title2"
                    }
                }
            }
        };
    }

    [TearDown]
    public void TearDown()
    {
        _config = null;
    }
    
    RoleSelectionComponent.Dependencies CreateDependencies(
        Mock<IHorizontalGridComponentResizer> horizontalGridComponentResizer = null, 
        Mock<IRoleButtonInstantiater> roleButtonInstantiater = null,
        Mock<IExecuteActionAtEndOfFixedFrame> executeActionAtTheEndOfFixedFrameMock = null)
    {
        horizontalGridComponentResizer ??= new Mock<IHorizontalGridComponentResizer>();
        roleButtonInstantiater ??= new Mock<IRoleButtonInstantiater>();
        executeActionAtTheEndOfFixedFrameMock ??= new Mock<IExecuteActionAtEndOfFixedFrame>();
        return new RoleSelectionComponent.Dependencies()
        {
            HorizontalGridComponentResizer = horizontalGridComponentResizer.Object,
            RoleButtonInstantiater = roleButtonInstantiater.Object,
            ExecuteActionAtEndOfFixedFrame = executeActionAtTheEndOfFixedFrameMock.Object
        };
    }
    [Test]
    public void TestSetDependencies()
    {
        //Arrange
        var sut = new GameObject().AddComponent<RoleSelectionComponent>();
        var dependencies = CreateDependencies();
        //Act
        sut.SetDependencies(dependencies);
        //Assert
        Assert.AreSame(dependencies.HorizontalGridComponentResizer, sut._dependencies.HorizontalGridComponentResizer);
    }

    [Test]
    public void TestConfigure_FirstCall_CreatesAndConfiguresNecessaryButtons()
    {
        //Given an initialized RoleSelectionComponent
        //When configure is called
        //Then it creates a list of buttons and configures them
        
        //Arrange
        string actionMessage = "notCalled";
        Action<string> addRoleAction = (msg) =>
        {
            actionMessage = msg;
        };


        _config = new RoleSelectionComponent.Config()
        {
            AddRoleAction = addRoleAction,
            Resource = new RoleSelectionComponentResource()
            {
                RoleButtons = new List<RoleButtonResource>()
                {
                    new RoleButtonResource(new Mock<IIcon>().Object)
                    {
                        Title = "Kriger"
                    }
                }
            }
        };
        
        var rolebuttonAMock = new Mock<IRoleButton>();
        rolebuttonAMock.Setup(x => x.Configure(It.IsAny<RoleButton.Config>()))
            .Callback<RoleButton.Config>((theConfig) =>
            {
                theConfig.buttonAction(theConfig.Resource.Title);
            })
            .Verifiable();

        var sut = new GameObject().AddComponent<RoleSelectionComponent>();
        var roleButtonInstantiaterMock = new Mock<IRoleButtonInstantiater>();
        roleButtonInstantiaterMock.Setup(x => x.Create())
            .Returns(rolebuttonAMock.Object).Verifiable();

        var horizontalGridComponentResizerMock = new Mock<IHorizontalGridComponentResizer>();
        horizontalGridComponentResizerMock.Setup(x => x.ResizeGrid(_config.Resource.RoleButtons.Count, 450f)).Verifiable();

        var executeActionAtTheEndOfFixedFrameMock = new Mock<IExecuteActionAtEndOfFixedFrame>();
        executeActionAtTheEndOfFixedFrameMock.Setup(x => x.Configure(It.IsAny<Action>(), It.IsAny<Action>()))
            .Callback<Action, Action>((theWaiter, theAction) =>
            {
                theAction.Invoke();
            })
            .Verifiable();
        
        var dependencies = CreateDependencies(horizontalGridComponentResizerMock, roleButtonInstantiaterMock, executeActionAtTheEndOfFixedFrameMock);
        sut.SetDependencies(dependencies);
        _config.AddRoleAction = addRoleAction;
        //Act
        sut.Configure(_config);
        
        //Assert
        roleButtonInstantiaterMock.Verify(x => x.Create(), Times.Exactly(1));

        Assert.AreEqual(_config.Resource.RoleButtons[0].Title, actionMessage);
        horizontalGridComponentResizerMock.Verify(x => x.ResizeGrid(_config.Resource.RoleButtons.Count, 450f));
        executeActionAtTheEndOfFixedFrameMock.Verify(x => x.Configure(It.IsAny<Action>(), It.IsAny<Action>()));
    }
    
    [Test]
    public void TestConfigure_CreatesNecessaryButtons_DestroysSuperflousEntities()
    {
        //Given a previously configured RoleSelectionComponent
        //When the selector is configured a second time with one button less than the first time
        //Then the function removes 1 button and configures the buttons.
        
        //Arrange
        Action<string> addRoleAction = (msg) => { };
        
        
        var rolebuttonAMock = new Mock<IRoleButton>();
        rolebuttonAMock.Setup(x => x.Configure(It.IsAny<RoleButton.Config>())).Verifiable();
        
        var rolebuttonBMock = new Mock<IRoleButton>();
        rolebuttonBMock.Setup(x => x.Configure(It.IsAny<RoleButton.Config>())).Verifiable();

        var sut = new GameObject().AddComponent<RoleSelectionComponent>();
        var roleButtonInstantiaterMock = new Mock<IRoleButtonInstantiater>();
        roleButtonInstantiaterMock.SetupSequence(x => x.Create())
            .Returns(rolebuttonAMock.Object)
            .Returns(rolebuttonBMock.Object);

        var dependencies = CreateDependencies(null, roleButtonInstantiaterMock);
        sut.SetDependencies(dependencies);
        _config.AddRoleAction = addRoleAction;

        sut.Configure(_config);

        _config.Resource.RoleButtons.RemoveAt(1);
        
        //Act
        sut.Configure(_config);
        
        //Assert
        rolebuttonAMock.Verify(x => x.Configure(It.IsAny<RoleButton.Config>()), Times.Exactly(2));
        rolebuttonAMock.Verify(x => x.DestroySelf(), Times.Never);

        rolebuttonBMock.Verify(x => x.Configure(It.IsAny<RoleButton.Config>()), Times.Once);
        rolebuttonBMock.Verify(x => x.DestroySelf(), Times.Once);
        roleButtonInstantiaterMock.Verify(x => x.Create(), Times.Exactly(2));
    }
}
