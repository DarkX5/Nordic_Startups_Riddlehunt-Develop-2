

using System;
using Riddlehouse.Core.Helpers.Helpers;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;
using UnityEngine;

public class TestRiddleComponentBehaviour
{
    private GameObject TestGo;
    private RectTransform Parent;
    private Mock<IViewActions> HuntComponentUIActionsMock;
    private Mock<IRiddleComponentActions> RiddleComponentActionsMock;

    [OneTimeSetUp]
    public void Init()
    {
        Parent = new GameObject().AddComponent<RectTransform>();
    }
    
    [SetUp]
    public void Setup()
    {
        HuntComponentUIActionsMock = new Mock<IViewActions>();
        RiddleComponentActionsMock = new Mock<IRiddleComponentActions>();
        TestGo = new GameObject();
        TestGo.AddComponent<RiddleHuntComponentBehaviour>();
    }

    [TearDown]
    public void TearDown()
    {
        TestGo = null;
        HuntComponentUIActionsMock = null;
    }

    [Test]
    public void TestFactory_Creates_New_Intsance()
    {
        // Given a game object with a Riddle Component behaviour
        // When Factory() is called
        // Then return a new instance of Riddle Component

        // Arrange
        
        // Act
        var sut = RiddleComponent.Factory(TestGo);

        // Assert
        Assert.IsNotNull(sut);
    }
    
    [Test]
    public void TestFactory_Throws_ArgumentException_If_GameObject_Does_Not_Have_Expected_Component()
    {
        // Given a game object without a Riddle Component behaviourt
        // When Factory() is called
        // Then an argument exception is thrown

        // Arrange
        TestGo = new GameObject();
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => RiddleComponent.Factory(TestGo));
    }
    
    [Test]
    public void TestFitInView_Calls_FitToFullScreen()
    {
        // Given a new Riddle Component is created
        // When it is instantiated
        // Then it is configured to be shown on the UI by calling FitInView
        
        // Arrange
        var uiFitterMock = new Mock<IUIFitters>();
        HuntComponentUIActionsMock.Setup(x => x.FitInView(Parent, uiFitterMock.Object));
        var sut = new RiddleComponent(RiddleComponentActionsMock.Object,HuntComponentUIActionsMock.Object);
        
        // Act
        sut.GetComponentUIActions().FitInView(Parent, uiFitterMock.Object);      
        
        //Assert
        HuntComponentUIActionsMock.Verify(x=>x.FitInView(Parent,uiFitterMock.Object));
    }
}
