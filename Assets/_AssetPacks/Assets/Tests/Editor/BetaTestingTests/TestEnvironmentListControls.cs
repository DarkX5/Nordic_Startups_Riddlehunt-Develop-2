using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.environments.Models;
using Riddlehunt.Beta.Environment.Controls;
using UnityEngine;

[TestFixture]
public class TestEnvironmentListControls
{
    private List<Target> environmentTargets;

    private Mock<ICreateEnvironmentTargetEntry> environmentTargetCreator;
    [SetUp]
    public void Init()
    {
        environmentTargetCreator = new Mock<ICreateEnvironmentTargetEntry>();
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
        environmentTargetCreator = null;
        environmentTargets = null;
    }

    [Test]
    public void TestSetDependencies()
    {
        //Arrange
        var sut = new GameObject().AddComponent<EnvironmentListControls>();
        var dependencies = new EnvironmentListControls.Dependencies()
        {
            EnvironmentTargetCreator = environmentTargetCreator.Object
        };
        
        //Act
        sut.SetDependencies(dependencies);
        
        //Assert
        Assert.AreEqual(dependencies, sut._dependencies);
    }

    [Test]
    public void TestConfigure_FirstRun_Creates_TargetButtons()
    {
        //Given a new list
        //When the list is configured with 2 options
        //Then the list only displays the elements required.
        
        //Arrange
        Mock<IEnvironmentTargetControls> targetController = new Mock<IEnvironmentTargetControls>();
        targetController.Setup(x => x.Configure(It.IsAny<EnvironmentTargetControls.Config>())).Verifiable();
        targetController.Setup(x => x.Display()).Verifiable();
        
        environmentTargetCreator.Setup(x => x.Create()).Returns(targetController.Object).Verifiable();
        
        var sut = new GameObject().AddComponent<EnvironmentListControls>();
        var dependencies = new EnvironmentListControls.Dependencies()
        {
            EnvironmentTargetCreator = environmentTargetCreator.Object
        };
        sut.SetDependencies(dependencies);

        //Act
        sut.Configure(new EnvironmentListControls.Config()
        {
            EnvironmentTargets = environmentTargets,
            ButtonAction = (value) => {}
        });
        
        //Assert
        environmentTargetCreator.Verify(x => x.Create(), Times.Exactly(2));
        targetController.Verify(x => x.Configure(It.IsAny<EnvironmentTargetControls.Config>()), Times.Exactly(2));
        targetController.Verify(x => x.Display(), Times.Exactly(2));
    }

    [Test]
    public void TestConfigure_SecondRun_Reuses_TargetButtons()
    {
        //Given a list preconfigured with 2 options
        //When the list needs to be regenerated with only 1 option
        //Then the list only displays the elements required.
        
        //Arrange
        Mock<IEnvironmentTargetControls> targetController = new Mock<IEnvironmentTargetControls>();
        targetController.Setup(x => x.Configure(It.IsAny<EnvironmentTargetControls.Config>())).Verifiable();
        targetController.Setup(x => x.Display()).Verifiable();
        targetController.Setup(x => x.Hide()).Verifiable();
        environmentTargetCreator.Setup(x => x.Create()).Returns(targetController.Object).Verifiable();
        
        var sut = new GameObject().AddComponent<EnvironmentListControls>();
        var dependencies = new EnvironmentListControls.Dependencies()
        {
            EnvironmentTargetCreator = environmentTargetCreator.Object
        };
        sut.SetDependencies(dependencies);
        
        //first call
        sut.Configure(new EnvironmentListControls.Config()
        {
            EnvironmentTargets = environmentTargets,
            ButtonAction = (value) => {}
        });

        List<Target> environmentTargets2 = new List<Target>();
        environmentTargets2.Add(environmentTargets[0]);
        
        //Act - second call
        sut.Configure(new EnvironmentListControls.Config()
        {
            EnvironmentTargets = environmentTargets2,
            ButtonAction = (value) => {}
        });
        
        //Assert
        environmentTargetCreator.Verify(x => x.Create(), Times.Exactly(2));
        targetController.Verify(x => x.Configure(It.IsAny<EnvironmentTargetControls.Config>()), Times.Exactly(3));
        targetController.Verify(x => x.Display(), Times.Exactly(3));
        targetController.Verify(x => x.Hide(), Times.Exactly(2));
    }
}
