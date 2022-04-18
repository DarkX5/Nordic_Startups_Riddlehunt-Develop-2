using System;
using System.Collections;
using System.Collections.Generic;
using Riddlehouse.Core.Helpers.Helpers;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;
using TMPro;
using UnityEngine;

public class TestEndHuntComonentBehaviour
{
        
    [Test]
    public void TestEndHuntFactory_Returns_Correct_EndHunt_Object()
    {
        // Arrange
        var gameObject = new GameObject("EndHuntPanel", typeof(RectTransform));
        var rt = (RectTransform) gameObject.transform;
        gameObject.AddComponent<EndHuntComponentBehaviour>();
        
        // Act
        var result = EndHuntComponent.Factory(gameObject);

        // Assert
        Assert.True(result.GetEndHuntActions() != null);
        Assert.True(result.GetComponentUIActions() != null);
    }
    
    [Test]
    public void TestEndHunt_Display()
    {
        //Given that the user changes view.
        //When the next view is displayed.
        //Then the gameobject is enabled.

        // Arrange
        var gameObject = new GameObject();
        var sut = gameObject.AddComponent<EndHuntComponentBehaviour>();
        gameObject.SetActive(false);
        // Act
        sut.Display();

        // Assert
        Assert.True(sut.gameObject.activeSelf);
    }
    
    [Test]
    public void TestEndHunt_Hide()
    {
        //Given that the user changes view.
        //When the next view is displayed.
        //Then the previous component is hidden.
        
        // Arrange
        var gameObject = new GameObject();
        var sut = gameObject.AddComponent<EndHuntComponentBehaviour>();
        // ACT
        sut.Hide();
        // Assert
        Assert.IsFalse(sut.gameObject.activeSelf);
    }
    [Test]
    public void Test_IsShown_true()
    {
        //Given that the user changes view
        //When the system checks if the view is active
        //Then the function returns true.

        // Arrange
        var gameObject = new GameObject();
        var sut = gameObject.AddComponent<EndHuntComponentBehaviour>();
        gameObject.SetActive(true);

        // ACT & Assert
        Assert.True(sut.IsShown());
    }
    [Test]
    public void Test_IsShown_false()
    {
        //Given that the user changes view
        //When the system checks if the view is active
        //Then the function returns false.

        // Arrange
        var gameObject = new GameObject();
        var sut = gameObject.AddComponent<EndHuntComponentBehaviour>();
        gameObject.SetActive(false);

        // ACT & Assert
        Assert.False(sut.IsShown());
    }
    
    [Test]
    public void TestEndHunt_PerformAction()
    {
        //Given that the user presses the button in the view
        //When the button is pressed
        //Then PerformAction is called the buttonAction is invoked, and the view is hidden.

        // Arrange
        var gameObject = new GameObject();
        var endHuntBehaviour = gameObject.AddComponent<EndHuntComponentBehaviour>();
        var sut = new EndHuntComponent(endHuntBehaviour, endHuntBehaviour);
        bool hasBeenCalled = false;
        Action action = () => { hasBeenCalled = true; };
        string endText = "end";
        endHuntBehaviour.SetDependencies(gameObject.AddComponent<TextMeshProUGUI>(), new Mock<IVideoCanvasController>().Object);
        sut.Configure(endText, action);

        endHuntBehaviour.PerformAction();
        // ACT & Assert
        Assert.False(endHuntBehaviour.IsShown());
        Assert.True(hasBeenCalled);
    }

    [Test]
    public void TestEndHuntBehavior_FitInView_FitsToFullScreen()
    {
        //Given the users starts a new hunt
        //When the hunt is created, the endHuntComponent is UIfitted.
        //Then the endHuntComponent is set to fill the entire screen.

        var gameObject = new GameObject();
        var child = gameObject.AddComponent<RectTransform>();
        var endHuntBehaviour = gameObject.AddComponent<EndHuntComponentBehaviour>();
        var parent = new GameObject().AddComponent<RectTransform>();

        var uiFittersMock = new Mock<IUIFitters>();
        uiFittersMock.Setup(x => x.FitToFullscreen(child, parent)).Verifiable();
        
        endHuntBehaviour.FitInView(parent, uiFittersMock.Object);

        uiFittersMock.Verify(x => x.FitToFullscreen(child, parent));
    }
    
    [TestCase(0)]
    [TestCase(1)]
    [Test]
    public void TestEndHuntBehavior_FitInView_With_SibblingIndex_FitsToFullScreen(int index)
    {
        //Given the user requires an endview
        //When FitInView is called with a parent and an index
        //Then view is fitted to that parent, and set to the given sibbling index.

        var gameObject = new GameObject();
        var child = gameObject.AddComponent<RectTransform>();
        var endHuntBehaviour = gameObject.AddComponent<EndHuntComponentBehaviour>();
        var parent = new GameObject().AddComponent<RectTransform>();
        var otherChild = new GameObject();
        otherChild.transform.SetParent(parent);
        
        var uiFittersMock = new Mock<IUIFitters>();
        uiFittersMock.Setup(x => x.FitToFullscreen(child, parent)).Verifiable();
        
        endHuntBehaviour.FitInView(parent, uiFittersMock.Object, index);

        uiFittersMock.Verify(x => x.FitToFullscreen(child, parent));
        Assert.AreEqual(gameObject.transform.GetSiblingIndex(), index);
    }

    [Test]
    public void TestEndHuntBehaviour_GetComponentType()
    {
        var gameObject = new GameObject();
        var endHuntBehaviour = gameObject.AddComponent<EndHuntComponentBehaviour>();
        Assert.AreEqual(ComponentType.End, endHuntBehaviour.GetComponentType());
    }
}
