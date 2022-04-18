using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;
[TestFixture]
public class TestConditionalStepListView
{
    private ConditionalStepListView.Config _config;

    [SetUp]
    public void Init()
    {
        _config = new ConditionalStepListView.Config()
        {
            
        };
    }

    [TearDown]
    public void TearDown()
    {
        _config = null;
    }
    ConditionalStepListView.Dependencies CreateDependencies(
        Mock<IComponentDisplayController> displayControllerMock = null,
        Mock<IConditionalStepBtnInstantiater> conditionalStepBtnInstantiater = null,
        Mock<IConditionalStepList> conditionalStepListMock = null)
    {
        displayControllerMock ??= new Mock<IComponentDisplayController>();
        conditionalStepBtnInstantiater ??= new Mock<IConditionalStepBtnInstantiater>();
        conditionalStepListMock ??= new Mock<IConditionalStepList>();
        return new ConditionalStepListView.Dependencies()
        {
            DisplayController = displayControllerMock.Object,
            ConditionalStepBtnInstantiater = conditionalStepBtnInstantiater.Object,
            ConditionalStepList = conditionalStepListMock.Object
        };
    }
    [Test]
    public void TestSetDependencies()
    {
        //Arrange
        var sut = new GameObject().AddComponent<ConditionalStepListView>();

        var dependencies = CreateDependencies();
        
        //Act
        sut.SetDependencies(dependencies);
        //Assert
        Assert.AreEqual(dependencies, sut._dependencies);
    }

    [Test]
    public void TestConfigure_FirstCall_CreatesAndConfigures_ButtonList()
    {
        //Arrange
        var buttonMock = new Mock<IConditionalStepBtn>();
        
        var conditionalStepBtnInstantiater = new Mock<IConditionalStepBtnInstantiater>();
        conditionalStepBtnInstantiater.SetupSequence(x => x.Create())
            .Returns(buttonMock.Object)
            .Returns(buttonMock.Object)
            .Returns(buttonMock.Object);

            
        var huntStepsMock = new Mock<IHuntSteps>();
        huntStepsMock.Setup(x => x.GetLengthOfHunt()).Returns(3);
        
        var conditionalStepListMock = new Mock<IConditionalStepList>();
        conditionalStepListMock.Setup(x => 
            x.ConfigureStepList(
                huntStepsMock.Object, 
                It.IsAny<List<IConditionalStepBtn>>(), 
                It.IsAny<Action<string>>())
            ).Verifiable();
        
        _config.HuntSteps = huntStepsMock.Object;

        var sut = new GameObject().AddComponent<ConditionalStepListView>();

        var dependencies = CreateDependencies(null, conditionalStepBtnInstantiater, conditionalStepListMock);
        sut.SetDependencies(dependencies);
        
        //Act
        sut.Configure(_config);
        
        //Assert
        conditionalStepBtnInstantiater.Verify(x => x.Create(), Times.Exactly(3));
        conditionalStepListMock.Verify(x => 
            x.ConfigureStepList(
                huntStepsMock.Object, 
                It.IsAny<List<IConditionalStepBtn>>(), 
                It.IsAny<Action<string>>())
        );
    }
    
    [Test]
    public void TestConfigure_SecondCall_HidesAll_Creates_new_AndConfigures_ButtonList()
    {
        //Arrange
        var buttonMock = new Mock<IConditionalStepBtn>();
        buttonMock.Setup(x => x.Hide()).Verifiable();
        
        var conditionalStepBtnInstantiater = new Mock<IConditionalStepBtnInstantiater>();
        conditionalStepBtnInstantiater.SetupSequence(x => x.Create())
            .Returns(buttonMock.Object)
            .Returns(buttonMock.Object)
            .Returns(buttonMock.Object);

            
        var huntStepsMock = new Mock<IHuntSteps>();
        huntStepsMock.SetupSequence(x => x.GetLengthOfHunt()).Returns(2).Returns(3);
        
        _config.HuntSteps = huntStepsMock.Object;
        var conditionalStepListMock = new Mock<IConditionalStepList>();
        conditionalStepListMock.Setup(x => 
            x.ConfigureStepList(
                huntStepsMock.Object, 
                It.IsAny<List<IConditionalStepBtn>>(), 
                It.IsAny<Action<string>>())
        ).Verifiable();

        var sut = new GameObject().AddComponent<ConditionalStepListView>();

        var dependencies = CreateDependencies(null, conditionalStepBtnInstantiater, conditionalStepListMock);
        sut.SetDependencies(dependencies);
        
        sut.Configure(_config);
        //Act
        sut.Configure(_config);

        //Assert
        conditionalStepBtnInstantiater.Verify(x => x.Create(), Times.Exactly(3));
        buttonMock.Verify(x => x.Hide(), Times.Exactly(2));
        conditionalStepListMock.Verify(x => 
            x.ConfigureStepList(
                huntStepsMock.Object, 
                It.IsAny<List<IConditionalStepBtn>>(), 
                It.IsAny<Action<string>>()), Times.Exactly(2)
        );
    }
}
