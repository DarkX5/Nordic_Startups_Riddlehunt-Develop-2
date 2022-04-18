using System;
using Riddlehouse.Core.Helpers.Helpers;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public class TestRatingComponentBehaviour
{
    private RatingComponentBehaviour sut;
    private GameObject go;
    private Slider slider;
    
    [SetUp]
    public void Setup()
    {
        go = new GameObject();
        slider = go.AddComponent<Slider>();
        sut = go.AddComponent<RatingComponentBehaviour>();
        slider.wholeNumbers = false;
    }

    [TearDown]
    public void TearDown()
    {
        sut = null;
        go = null;
        slider = null;
    }
    
    [Test]
    public void TestPerformAction()
    {
        // Given a user presses the submit button
        // When giving feedback to a riddlehunt
        // Then the action associated with the submit feedback button is called!

        // Arrange
        var logicComponent = new Mock<IRatingComponent>();
        logicComponent.Setup(x => x.PerformAction()).Verifiable();

        // Act
        sut.SetLogicInstanceOfComponent(logicComponent.Object);
        sut.PerformAction();

        // Assert
        logicComponent.Verify(x => x.PerformAction());
    }

    [Test]
    //[Ignore("Need to figure out dependencies for UIFitters")]
    public void TestFitInView_FitsToFullScreen()
    {
        // Given the users ends a hunt
        // When the feedback view is shown
        // the component's viewport is refitted to its parent

        // Arrange
        var parent = new GameObject().AddComponent<RectTransform>();
        var uiFittersMock = new Mock<IUIFitters>();
        uiFittersMock.Setup(x => x.FitToFullscreen((RectTransform)sut.gameObject.transform, parent)).Verifiable();
        
        // Act
        sut.FitInView(parent, uiFittersMock.Object);

        // Assert
        uiFittersMock.Verify(x => x.FitToFullscreen((RectTransform)sut.gameObject.transform, parent));
    }

    [Test]
    public void TestSetLogicInstanceOfComponent_Sets_Field_To_Instance()
    {
        // Given a user wishes to submit a rating
        // When Creating the rating component
        // Then the component type should be set to rating
        
        // Arrange
        var iRatingComponentMock = new Mock<IRatingComponent>().Object;
        
        // Act
        sut.SetLogicInstanceOfComponent(iRatingComponentMock);

        // Assert
        Assert.AreSame(sut.GetRatingComponent(), iRatingComponentMock);
        
    }
    
    [TestCase(true)]
    [TestCase(false)]
    [Test]
    public void TestIsShown(bool active)
    {
        // Given that the user changes view
        // When the system checks if the view is active
        // Then the function returns whether the view is actually active.

        // Arrange
        sut.gameObject.SetActive(active);

        // ACT & Assert
        Assert.IsTrue(active == sut.IsShown());
    }
    
    [Test]
    public void TestHide()
    {
        // Given that the user changes view
        // When the next view is displayed
        // Then the previous component is hidden.
        
        // Arrange
        sut.gameObject.SetActive(true);
        // Act
        sut.Hide();

        // Assert
        Assert.IsFalse(sut.gameObject.activeSelf);
    }
    
    [Test]
    public void TestDisplay()
    {
        // Given that the user changes view.
        // When the next view is displayed.
        // Then the gameobject is enabled.

        // Arrange
        sut.gameObject.SetActive(false);
        // Act
        sut.Display();

        // Assert
        Assert.True(sut.gameObject.activeSelf);
    }

    [Test]
    public void TestConfigure()
    {
        // Given a user ends a hunt
        // When creating the rating view
        // Then the view is configured to assign a default value to rating
        
        // Arrange
        var expectedRating = 1f;

        // Act
        sut.SetDependencies(slider);
        sut.Configure(expectedRating);
        
        // Assert
        Assert.AreEqual(expectedRating,sut.GetRating());
    }

    [Test]
    public void TestGetComponent_Returns_ComponentType()
    {
        // Given a user ends a hunt
        // When the rating view has been created
        // Then the view is configured to have component type rating
        
        // Arrange
        var expectedComponentType = ComponentType.Rating;

        // Act
        var result = sut.GetComponentType();
        
        // Assert
        Assert.AreEqual(expectedComponentType, result);
    }

    [Test]
    public void TestGetRectTransform_Returns_RectTransform_Of_Object()
    {
        // Given a user ends a hunt
        // When the rating view has been created
        // Then the Rating System Component is able to return it's rect transform
        
        // Arrange
        RectTransform ExpectedType = new GameObject().AddComponent<RectTransform>();

        // Act
        var result = sut.GetRectTransform();

        // Assert
        Assert.NotNull(result);
        Assert.AreSame(ExpectedType.GetType(), result.GetType());
    }
    
    [TestCase(0f, ExpectedResult = 0f)]
    [TestCase(1f, ExpectedResult = 1f)]
    [TestCase(0.8f, ExpectedResult = 0.8f)]
    [Test]
    public float TestGetRating_Gets_Current_Rating(float expectedRating)
    {
        // Given a user wishes to rate a riddlehunt
        // When they submits their rating
        // Then the rating is read before it is send
        
        // Arrange
        sut.SetDependencies(slider);
        sut.Configure(expectedRating);

        // Act
        var result = sut.GetRating();
        
        // Assert
        return result;
    }
}