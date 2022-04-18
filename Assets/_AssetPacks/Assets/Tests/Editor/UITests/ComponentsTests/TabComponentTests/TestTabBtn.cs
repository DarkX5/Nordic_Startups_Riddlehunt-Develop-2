using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;
using UnityEngine;
using UnityEngine.PlayerLoop;

[TestFixture]
public class TestTabBtn
{
    public TabBtn _TabBtn;
    public Mock<ITabBtnActions> _tabBtnActionsMock;
    public Mock<ITabBtnUIActions> _tabBtnUIActionsMock;
    public string testBtnText = "ButtonText";
    public RectTransform rectTransformParent;
    public Mock<IViewActions> _componentUIAction;
    private ComponentType _key;

    [SetUp]
    public void Init()
    {
        _key = ComponentType.Story;
        rectTransformParent = new GameObject().AddComponent<RectTransform>();
        _tabBtnActionsMock = new Mock<ITabBtnActions>();
        _tabBtnActionsMock.Setup(x => x.Configure(testBtnText));
        _tabBtnUIActionsMock = new Mock<ITabBtnUIActions>();
        _TabBtn = new TabBtn(_tabBtnActionsMock.Object, _tabBtnUIActionsMock.Object, rectTransformParent);
    }

    [Test]
    public void TestFactory_Creates_New_Instance()
    {
        // Given a game object with a TabBtnBehaviour component
        // When Factory() is called
        // Then return a new instance of TabBtn that references the given component

        // Arrange
        var testGo = new GameObject();
        testGo.AddComponent<TabBtnBehaviour>();

        // Act
        ITabBtn sut = TabBtn.Factory(testGo, rectTransformParent);

        // Assert
        Assert.IsNotNull(sut);
    }

    [Test]
    public void TestFactory_Throws_ArgumentException_If_GameObject_Does_Not_Have_Expected_Component()
    {
        // Given a game object without a TabBtnBehaviour component
        // When Factory() is called
        // Then an argument exception is thrown

        // Arrange
        GameObject testGo = new GameObject();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => TabBtn.Factory(testGo, rectTransformParent));
    }

    [Test]
    public void TestConfigure()
    {
        //Given a TabBtn
        //When configure is called.
        //Then configure on actions mock is called.

        // Arrange
        var sut = _TabBtn;
        Action<ComponentType> buttonAction = (x) => { };


        // Act
        sut.Configure(testBtnText, _key, buttonAction);
        // Assert
        _tabBtnActionsMock.Verify(x => x.Configure(testBtnText));
    }

    [Test]
    public void TestPerformAction()
    {
        //Given a TabBtn
        //When button is pressed.
        //Then action is performed.
        var sut = _TabBtn;
        var actionCalledWithCorrectType = false;
        var returnsComponentType = _key;
        Action<ComponentType> buttonAction = (x) =>
        {
            if (returnsComponentType == x)
                actionCalledWithCorrectType = true;
        };
        sut.Configure(testBtnText, _key, buttonAction);
        sut.PerformAction();
        Assert.IsTrue(actionCalledWithCorrectType);
    }

    [Test]
    public void TestSetButtonState_Sets_Highlighted_State_And_Width()
    {
        // Given a user presses selects another view in the tab component by pressing its related button
        // When the new view is updated
        // Then the button's state is updated to reflect the now active view

        // Arrange
        var sut = _TabBtn;
        const float width = 42f;
        _tabBtnActionsMock.Setup(x => x.SetWidthAndColor(width)).Verifiable();
        // Act
        sut.SetTabButtonState(TabButtonState.Highlighted, width);

        // Assert
        Assert.AreEqual(TabButtonState.Highlighted, sut.State);
        _tabBtnActionsMock.Verify(x => x.SetWidthAndColor(width));
        
    }
}