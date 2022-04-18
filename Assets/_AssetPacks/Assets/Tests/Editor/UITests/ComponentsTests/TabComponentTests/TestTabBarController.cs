using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;
using UnityEngine;

namespace Tests.Editor.UITests.ComponentsTests.TabComponentTests
{
    [TestFixture]
    public class TestTabBarController
    {
        Mock<ITabBtn> _tabBtnMock;
        Mock<IViewActions> _componentUIActionsMock;
        Mock<ITabBarControllerActions> _tabBarControllerActionsMock;
        Action<ComponentType> _btnAction;
        TabBarController _tabBarController;
        ComponentType _key;
        private string _buttonTitle;

        
        [SetUp]
        public void Init()
        {
            _buttonTitle = "buttonTitle";

            _key = ComponentType.Story;
            _tabBtnMock = new Mock<ITabBtn>();
            _componentUIActionsMock = new Mock<IViewActions>();
            _componentUIActionsMock.Setup(x => x.GetComponentType()).Returns(_key);
            
            _tabBarControllerActionsMock = new Mock<ITabBarControllerActions>();
            _btnAction = (x) => { };
            _tabBarController = new TabBarController(_tabBarControllerActionsMock.Object);
            _tabBarControllerActionsMock.Setup(x => x.MapButtonTitle(_key)).Returns(_buttonTitle).Verifiable();
            _tabBarControllerActionsMock
                .Setup(x=>x.InstantiateButton(_componentUIActionsMock.Object))
                .Returns(_tabBtnMock.Object)
                .Verifiable();
        }

        [TearDown]
        public void TearDown()
        {
            _tabBtnMock = null;
            _componentUIActionsMock = null;
            _tabBarControllerActionsMock = null;
            _tabBarController = null;
            _btnAction = null;
        }
        
        [Test]
        public void TestAddAndConfigureButton_Adds_Button_To_Dictionary()
        {
            // Given a user starts a new step in a hunt
            // When a new view component is added to the tab component
            // Then a button is added to the dictionary and configured

            float expectedActiveWidth = 85f;
            float expectedHightlightedWidth = 115f;
            
            Vector2 tabBarCurrentSize = new Vector2(300, 15);
            Vector3 layoutSpacingRules = new Vector3(25, 25, 50);
            _tabBarControllerActionsMock.Setup(x => x.TabBarCurrentSize()).Returns(tabBarCurrentSize).Verifiable();
            _tabBarControllerActionsMock.Setup(x => x.GetLayoutSpacingRules()).Returns(layoutSpacingRules);
            // Arrange
            _tabBtnMock.Setup(x => x.SetTabButtonState(TabButtonState.Hidden, expectedActiveWidth)).Verifiable();
            _tabBtnMock.Setup(x => x.Configure(_buttonTitle, _key, _btnAction)).Verifiable();
            var sut = _tabBarController;

            // Act
            sut.AddAndConfigure(_componentUIActionsMock.Object,_btnAction);

            // Assert
            Assert.IsTrue(sut.HasKey(_key));
            _tabBarControllerActionsMock.Verify(x=>x.InstantiateButton(_componentUIActionsMock.Object));
            _tabBtnMock.Verify(x => x.SetTabButtonState(TabButtonState.Hidden, expectedActiveWidth));
            _tabBarControllerActionsMock.Verify(x => x.TabBarCurrentSize());
            _tabBarControllerActionsMock.Verify(x => x.GetLayoutSpacingRules());
            _tabBtnMock.Verify(x => x.Configure(_buttonTitle, _key, _btnAction));
            _tabBarControllerActionsMock.Verify(x => x.MapButtonTitle(_key));

            Assert.AreEqual(expectedActiveWidth, sut.ActiveWidth);
            Assert.AreEqual(expectedHightlightedWidth, sut.HighlightedWidth);
        }

        [Test]
        public void TestHasKey_Returns_True_If_Key_Is_Present()
        {
            // Given a user starts a new step in a hunt
            // When adding and configuring view components to the tab component and requesting a key from the active buttons dictionary
            // Then returns whether the key exists or not
            
            // Arrange
            var sut = _tabBarController;

            // Act
            sut.AddAndConfigure(_componentUIActionsMock.Object, _btnAction);
            var result = sut.HasKey(_key);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void TestCalulateWidth_Calulates_Width_Of_Button_Based_On_State()
        {
            // Given a user starts a new hunt step with two required components (scanning and story)
            // When a component is added or removed from the tap view
            // Then the width is calculated based on button state and how many should be displayed
            
            // Arrange
            var expectedActiveWidth = 0f;
            var expectedHighlightedWidth = 0f;
            
            var storyTabBtnMock = new Mock<ITabBtn>();
            var scanningTabBtnMock = new Mock<ITabBtn>();

            var storyComponentUIActionsMock = new Mock<IViewActions>();
            storyComponentUIActionsMock
                .Setup(x => x.GetComponentType()).Returns(ComponentType.Story);
            
            var scanningComponentUIActionsMock = new Mock<IViewActions>();
            scanningComponentUIActionsMock
                .Setup(x => x.GetComponentType()).Returns(ComponentType.Scanning);
            
            var tabBarActionsMock = new Mock<ITabBarControllerActions>();
            Action<ComponentType> btnAction = (x) => { };
            var sut = new TabBarController(tabBarActionsMock.Object);
            tabBarActionsMock
                .Setup(x=>x.InstantiateButton(storyComponentUIActionsMock.Object))
                .Returns(storyTabBtnMock.Object)
                .Verifiable();

            tabBarActionsMock
                .Setup(x => x.InstantiateButton(scanningComponentUIActionsMock.Object))
                .Returns(scanningTabBtnMock.Object)
                .Verifiable();
            

            //Act
            sut.AddAndConfigure(storyComponentUIActionsMock.Object, _btnAction);
            sut.AddAndConfigure(scanningComponentUIActionsMock.Object, _btnAction);
            
            //Assert
            Assert.AreEqual(expectedActiveWidth, sut.ActiveWidth);
            Assert.AreEqual(expectedHighlightedWidth, sut.ActiveWidth);
        }

        [Test]
        public void TestUpdateButtonState_Updates_Based_On_SiblingIndex()
        {
            //Arrange
            ComponentType typeToHighlight = ComponentType.Story;
            var homeComponentUIActionsMock = new Mock<IViewActions>();
            homeComponentUIActionsMock
                .Setup(x => x.GetComponentType()).Returns(ComponentType.HuntHome);
            
            var storyComponentUIActionsMock = new Mock<IViewActions>();
            storyComponentUIActionsMock
                .Setup(x => x.GetComponentType()).Returns(ComponentType.Story);
            
            var endComponentUIActionsMock = new Mock<IViewActions>();
            endComponentUIActionsMock
                .Setup(x => x.GetComponentType()).Returns(ComponentType.End);
            
            var homeTabBtnMock = new Mock<ITabBtn>();
            homeTabBtnMock.Setup(x => x.GetSiblingIndex()).Returns(1).Verifiable();
            homeTabBtnMock.Setup(x => x.SetTabButtonState(TabButtonState.Active, It.IsAny<float>()));
            
            var storyTabBtnMock = new Mock<ITabBtn>();
            storyTabBtnMock.Setup(x => x.GetSiblingIndex()).Returns(2).Verifiable();
            homeTabBtnMock.Setup(x => x.SetTabButtonState(TabButtonState.Highlighted, It.IsAny<float>()));

            var endTabBtnMock = new Mock<ITabBtn>();
            endTabBtnMock.Setup(x => x.GetSiblingIndex()).Returns(3).Verifiable();
            homeTabBtnMock.Setup(x => x.SetTabButtonState(TabButtonState.Hidden, It.IsAny<float>()));
            
            _tabBarControllerActionsMock.Setup(x => x.MapButtonTitle(ComponentType.Story)).Returns(_buttonTitle);
            _tabBarControllerActionsMock.Setup(x => x.MapButtonTitle(ComponentType.HuntHome)).Returns(_buttonTitle);
            _tabBarControllerActionsMock.Setup(x => x.MapButtonTitle(ComponentType.End)).Returns(_buttonTitle);

            var sut = new TabBarController(_tabBarControllerActionsMock.Object);
            _tabBarControllerActionsMock
                .Setup(x=>x.InstantiateButton(homeComponentUIActionsMock.Object))
                .Returns(homeTabBtnMock.Object)
                .Verifiable();
            _tabBarControllerActionsMock
                .Setup(x=>x.InstantiateButton(storyComponentUIActionsMock.Object))
                .Returns(storyTabBtnMock.Object)
                .Verifiable();
            _tabBarControllerActionsMock
                .Setup(x=>x.InstantiateButton(endComponentUIActionsMock.Object))
                .Returns(endTabBtnMock.Object)
                .Verifiable();
            
            //Act
            sut.AddAndConfigure(homeComponentUIActionsMock.Object, _btnAction);
            sut.AddAndConfigure(storyComponentUIActionsMock.Object, _btnAction);
            sut.AddAndConfigure(endComponentUIActionsMock.Object, _btnAction);
            
            sut.UpdateButtonStates(typeToHighlight);
            
            //Assert
            Assert.IsTrue(sut.HasKey(typeToHighlight));
            Assert.AreEqual(typeToHighlight, sut.GetHighlighted());
            
            homeTabBtnMock.Verify(x => x.GetSiblingIndex());
            storyTabBtnMock.Verify(x => x.GetSiblingIndex());
            endTabBtnMock.Verify(x => x.GetSiblingIndex());
            
            homeTabBtnMock.Verify(x => x.SetTabButtonState(TabButtonState.Active, It.IsAny<float>()));
            storyTabBtnMock.Verify(x => x.SetTabButtonState(TabButtonState.Highlighted, It.IsAny<float>()));
            endTabBtnMock.Verify(x => x.SetTabButtonState(TabButtonState.Hidden, It.IsAny<float>()));
        }

        [Test]
        public void TestConfigureNeededButtons()
        {
            List<ComponentType> possibleSteps = new List<ComponentType>()
                {ComponentType.Story, ComponentType.Scanning};

            var storyComponentUIActionsMock = new Mock<IViewActions>();
            storyComponentUIActionsMock
                .Setup(x => x.GetComponentType()).Returns(ComponentType.Story);
            
            var scanningComponentUIActionsMock = new Mock<IViewActions>();
            scanningComponentUIActionsMock
                .Setup(x => x.GetComponentType()).Returns(ComponentType.Scanning);

   
            var storyTabBtnMock = new Mock<ITabBtn>();
            var scanningTabBtnMock = new Mock<ITabBtn>();

            var tabBarActionsMock = new Mock<ITabBarControllerActions>();
            var sut = new TabBarController(tabBarActionsMock.Object);
            tabBarActionsMock
                .Setup(x=>x.InstantiateButton(storyComponentUIActionsMock.Object))
                .Returns(storyTabBtnMock.Object)
                .Verifiable();
            
            tabBarActionsMock
                .Setup(x=>x.InstantiateButton(scanningComponentUIActionsMock.Object))
                .Returns(scanningTabBtnMock.Object)
                .Verifiable();
            
            sut.AddAndConfigure(scanningComponentUIActionsMock.Object, _btnAction);
            sut.AddAndConfigure(storyComponentUIActionsMock.Object, _btnAction);

            sut.ConfigureNeededButtons(possibleSteps);

            foreach (var step in possibleSteps)
            {
                Assert.IsTrue(sut.HasKey(step));
            }
        }
    }
}