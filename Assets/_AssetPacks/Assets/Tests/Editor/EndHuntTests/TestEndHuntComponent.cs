using System;
using Riddlehouse.Core.Helpers.Helpers;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;
using UnityEditor.TestTools.TestRunner;
using UnityEngine;

[TestFixture]
public class TestEndHuntComponent
{
    [Test]
    public void TestFactory_Creates_New_Instance()
    {
        // Given a game object with an EndHuntComponentBehaviour component
        // When Factory() is called
        // Then return a new instance of StoryComponent that references the given component
            
        // Arrange
        var testGo = new GameObject();
        testGo.AddComponent<EndHuntComponentBehaviour>();

        // Act
        IEndHuntComponent sut = EndHuntComponent.Factory(testGo);

        // Assert
        Assert.IsNotNull(sut);
    }
        
    [Test]
    public void TestFactory_Throws_ArgumentException_If_GameObject_Does_Not_Have_Expected_Component()
    {
        // Given a game object without a EndHuntComponentBehaviour component
        // When Factory() is called
        // Then an argument exception is thrown
            
        // Arrange
        GameObject testGo = new GameObject();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => EndHuntComponent.Factory(testGo));
    }
    
    [Test]
    public void TestConfigure()
    {
        //Given a new Endhunt
        //When Configure is called with an action
        //That action is set in the EndhuntActions

        //Arrange
        var endhuntActionsMock = new Mock<IEndHuntComponentActions>();
        Action buttonAction = () => { };
        string endText = "end";
        endhuntActionsMock.Setup(x => x.Configure(endText, buttonAction)).Verifiable();
        EndHuntComponent endhunt = new EndHuntComponent(endhuntActionsMock.Object,null);
        //Act
        endhunt.Configure(endText, buttonAction);
        //Assert
        endhuntActionsMock.Verify(x => x.Configure(endText, buttonAction));
    }

    [Test]
    public void TestGetEndHuntActions()
    {
        //Given an endhunt component
        //When EndhuntActions are requested
        //Then the components EndhuntActions are returned.
        
        //Arrange
        var endhuntActionsMock = new Mock<IEndHuntComponentActions>();
        var sut = new EndHuntComponent(endhuntActionsMock.Object,null);
        
        //Act & Assert
        Assert.AreSame(endhuntActionsMock.Object, sut.GetEndHuntActions());

    }
    
    [Test]
    public void GetComponentUIActions()
    {
        //Given an endhunt component
        //When GetComponentUIActions are requested
        //Then the components UIActions are returned.
        
        //Arrange
        var componentUIActions = new Mock<IViewActions>();
        var sut = new EndHuntComponent(null, componentUIActions.Object);
        
        //Act & Assert
        Assert.AreSame(componentUIActions.Object, sut.GetComponentUIActions());

    }
}
