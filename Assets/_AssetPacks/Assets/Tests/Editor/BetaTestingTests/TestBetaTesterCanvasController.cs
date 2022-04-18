using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.environments;
using riddlehouse_libraries.environments.Models;
using Riddlehunt.Beta.Environment.Controls;
using TMPro;
using UnityEngine;

[TestFixture]
public class TestBetaTesterCanvasController
{
    private List<Target> environmentTargets;
    private Mock<IEnvironmentListControls> _listControlsMock;
    private Mock<IGetEnvironmentTargets> _getEnvironmentTargetsMock;
    private Mock<ISetEnvironmentTarget> _setEnvironmentTargetsMock;
    private Mock<IStandardButton> _resetToDefaultbuttonMock;
    private TextMeshProUGUI _environmentLabel;
    private Mock<ILoginHandler> _loginHandler;
    [SetUp]
    public void Init()
    {
        _loginHandler = new Mock<ILoginHandler>();
        _listControlsMock = new Mock<IEnvironmentListControls>();
        _getEnvironmentTargetsMock = new Mock<IGetEnvironmentTargets>();
        _setEnvironmentTargetsMock = new Mock<ISetEnvironmentTarget>();
        _resetToDefaultbuttonMock = new Mock<IStandardButton>();
        _environmentLabel = new GameObject().AddComponent<TextMeshProUGUI>();
        
        environmentTargets = new List<Target>();
        environmentTargets.Add(new Target()
        {
            Name = "production",
            Url = "prod_url"
        });
        environmentTargets.Add(new Target()
        {
            Name = "development",
            Url = "dev_url"
        });
    }

    [TearDown]
    public void TearDown()
    {
        _loginHandler = null;
        _listControlsMock = null;
        _getEnvironmentTargetsMock = null;
        _setEnvironmentTargetsMock = null;
        _resetToDefaultbuttonMock = null;
        _environmentLabel = null;

    }

    [Test]
    public void TestSetDependencies()
    {
        //Arrange
        var sut = new GameObject().AddComponent<BetaTesterCanvasController>();

        var dependencies = new BetaTesterCanvasController.Dependencies()
        {
            ResetToDefaultButton = _resetToDefaultbuttonMock.Object,
            EnvironmentLabel = _environmentLabel,
            ListControls = _listControlsMock.Object,
            EnvironmentTargetGetter = _getEnvironmentTargetsMock.Object,
            EnvironmentTargetSetter = _setEnvironmentTargetsMock.Object
        };
        
        //Act
        sut.SetDependencies(dependencies);
        
        //Assert
        Assert.AreEqual(dependencies, sut._dependencies);
    }
    
    [Test]
    public void TestConfigure_SetTargetCalled_ButtonCallback()
    {
        //Given a new BetaTesterCanvas 
        //When a button is pressed after its been configured
        //Then the target is set with the url the user selected
        
        //Arrange
        _getEnvironmentTargetsMock.Setup(x => x.CanSwitchEnvironment()).Returns(true).Verifiable();
        _resetToDefaultbuttonMock.Setup(x =>x.Configure("Reset", It.IsAny<Action>())).Verifiable();
        _setEnvironmentTargetsMock.Setup(x => x.SetTarget(environmentTargets[0].Url)).Verifiable();
        _getEnvironmentTargetsMock.Setup(x => x.GetPossibleTargets()).ReturnsAsync(environmentTargets).Verifiable();
        _listControlsMock.Setup(x => 
            x.Configure(It.IsAny<EnvironmentListControls.Config>()))
            .Callback<EnvironmentListControls.Config>((config) =>
            {
                config.ButtonAction.Invoke(environmentTargets[0]);
            })
            .Verifiable();
        
        var sut = new GameObject().AddComponent<BetaTesterCanvasController>();

        var dependencies = new BetaTesterCanvasController.Dependencies()
        {
            ResetToDefaultButton = _resetToDefaultbuttonMock.Object,
            EnvironmentLabel = _environmentLabel,
            ListControls = _listControlsMock.Object,
            EnvironmentTargetGetter = _getEnvironmentTargetsMock.Object,
            EnvironmentTargetSetter = _setEnvironmentTargetsMock.Object
        };
        
        sut.SetDependencies(dependencies);
        
        //Act
        sut.Configure();
        
        //Assert
        _getEnvironmentTargetsMock.Verify(x => x.GetPossibleTargets());
        _setEnvironmentTargetsMock.Verify(x => x.SetTarget(environmentTargets[0].Url));
        _listControlsMock.Verify(x => x.Configure(It.IsAny<EnvironmentListControls.Config>()));
        _resetToDefaultbuttonMock.Verify(x =>x.Configure("Reset", It.IsAny<Action>()));

    }

    [Test]
    public void TestConfigure_CanSwitchEnvironment_True()
    {
        //Given a new BetaTesterCanvas for a user that's allowed to change his environment.
        //When configure is called
        //Then the EnvironmentTargets are collected, and passed to the list controls.
        
        //Arrange
        _getEnvironmentTargetsMock.Setup(x => x.CanSwitchEnvironment()).Returns(true).Verifiable();
        _getEnvironmentTargetsMock.Setup(x => x.GetPossibleTargets()).ReturnsAsync(environmentTargets).Verifiable();
        _listControlsMock.Setup(x => x.Configure(It.IsAny<EnvironmentListControls.Config>())).Verifiable();
        var sut = new GameObject().AddComponent<BetaTesterCanvasController>();

        var dependencies = new BetaTesterCanvasController.Dependencies()
        {
            ResetToDefaultButton = _resetToDefaultbuttonMock.Object,
            EnvironmentLabel = _environmentLabel,
            ListControls = _listControlsMock.Object,
            EnvironmentTargetGetter = _getEnvironmentTargetsMock.Object,
            EnvironmentTargetSetter = _setEnvironmentTargetsMock.Object
        };
        
        sut.SetDependencies(dependencies);
        
        //Act
        sut.Configure();
        
        //Assert
        _getEnvironmentTargetsMock.Verify(x => x.GetPossibleTargets());
        _listControlsMock.Verify(x => x.Configure(It.IsAny<EnvironmentListControls.Config>()));
        _getEnvironmentTargetsMock.Verify(x => x.CanSwitchEnvironment());
    }
    [Test]
    public void TestConfigure_CanSwitchEnvironment_True_PossibleTargetsList_Empty()
    {
        //Given a new BetaTesterCanvas for a session that's still on his default environment.
        //When configure is called and no PossibleTargets are found.
        //Then function terminates.
        
        //Arrange
        _getEnvironmentTargetsMock.Setup(x => x.CanSwitchEnvironment()).Returns(true).Verifiable();
        _getEnvironmentTargetsMock.Setup(x => x.GetPossibleTargets()).ReturnsAsync(new List<Target>()).Verifiable();
        _listControlsMock.Setup(x => x.Configure(It.IsAny<EnvironmentListControls.Config>())).Verifiable();
        var sut = new GameObject().AddComponent<BetaTesterCanvasController>();

        var dependencies = new BetaTesterCanvasController.Dependencies()
        {
            ResetToDefaultButton = _resetToDefaultbuttonMock.Object,
            EnvironmentLabel = _environmentLabel,
            ListControls = _listControlsMock.Object,
            EnvironmentTargetGetter = _getEnvironmentTargetsMock.Object,
            EnvironmentTargetSetter = _setEnvironmentTargetsMock.Object
        };
        
        sut.SetDependencies(dependencies);
        
        //Act
        sut.Configure();
        
        //Assert
        _getEnvironmentTargetsMock.Verify(x => x.GetPossibleTargets());
        _listControlsMock.Verify(x => x.Configure(It.IsAny<EnvironmentListControls.Config>()), Times.Never);
        _getEnvironmentTargetsMock.Verify(x => x.CanSwitchEnvironment());
    }
    [Test]
    public void TestConfigure_CanSwitchEnvironment_False()
    {
        //Given a new BetaTesterCanvas for a user that's allowed to change his environment.
        //When configure is called
        //Then the EnvironmentTargets are collected, and passed to the list controls.
        
        //Arrange
        _getEnvironmentTargetsMock.Setup(x => x.CanSwitchEnvironment()).Returns(false).Verifiable();
        _getEnvironmentTargetsMock.Setup(x => x.GetPossibleTargets()).ReturnsAsync(environmentTargets).Verifiable();
        _listControlsMock.Setup(x => x.Configure(It.IsAny<EnvironmentListControls.Config>())).Verifiable();
        var sut = new GameObject().AddComponent<BetaTesterCanvasController>();

        var dependencies = new BetaTesterCanvasController.Dependencies()
        {
            ResetToDefaultButton = _resetToDefaultbuttonMock.Object,
            EnvironmentLabel = _environmentLabel,
            ListControls = _listControlsMock.Object,
            EnvironmentTargetGetter = _getEnvironmentTargetsMock.Object,
            EnvironmentTargetSetter = _setEnvironmentTargetsMock.Object
        };
        
        sut.SetDependencies(dependencies);
        
        //Act
        sut.Configure();
        
        //Assert
        _getEnvironmentTargetsMock.Verify(x => x.GetPossibleTargets(), Times.Never);
        _listControlsMock.Verify(x => x.Configure(It.IsAny<EnvironmentListControls.Config>()));
        _getEnvironmentTargetsMock.Verify(x => x.CanSwitchEnvironment());
    }

    [Test]
    public void TestResetToDefault_ResetsToDefaultTarget_ResetsLabel()
    {
        //Arrange
        _getEnvironmentTargetsMock.Setup(x => x.CanSwitchEnvironment()).Returns(true).Verifiable();
        _setEnvironmentTargetsMock.Setup(x => x.ResetToDefaultTarget()).Verifiable();

        var sut = new GameObject().AddComponent<BetaTesterCanvasController>();
        
        var dependencies = new BetaTesterCanvasController.Dependencies()
        {
            LoginHandler = _loginHandler.Object,
            ResetToDefaultButton = _resetToDefaultbuttonMock.Object,
            EnvironmentLabel = _environmentLabel,
            ListControls = _listControlsMock.Object,
            EnvironmentTargetGetter = _getEnvironmentTargetsMock.Object,
            EnvironmentTargetSetter = _setEnvironmentTargetsMock.Object
        };
        
        sut.SetDependencies(dependencies);
        
        _resetToDefaultbuttonMock.Setup(x => x.Configure(It.IsAny<string>(), It.IsAny<Action>()))
            .Callback<string, Action>((theTitle, theAction) =>
            {
                theAction.Invoke();
            });
        
        //Act
        sut.ResetToDefault();
        
        //Assert
        _setEnvironmentTargetsMock.Verify(x => x.ResetToDefaultTarget());
        Assert.AreEqual("Current Environment: Default",_environmentLabel.text);
    }
}
