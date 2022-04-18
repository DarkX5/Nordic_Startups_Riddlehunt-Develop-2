using System;
using Hunt;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Tests.Editor.MapTests.Riddlehouse2DMapTests
{
    public class TestMapMenuController
    {
        private MapMenuController.Dependencies CreateDependencies(Mock<IComponentDisplayController> displayControllerMock = null)
        {
            displayControllerMock ??= new Mock<IComponentDisplayController>();
            return new MapMenuController.Dependencies()
            {
                DisplayController = displayControllerMock.Object,
            };
        }
        
        [Test]
        public void TestSetDependencies()
        {
            //Arrange
            var go = new GameObject();
            var sut = go.AddComponent<MapMenuController>();
            var dependencies = CreateDependencies();
            
            //Act
            sut.SetDependencies(dependencies);
            
            //Assert
            Assert.AreEqual(dependencies, sut._dependencies);
        }
        
        [Test]
        public void TestBackButtonAction_CallsActionFromConfig()
        {  
            //Given a new MapMenuController
            //When BackButtonAction is called
            //Then BackButtonAction from config is invoked.
            
            //Arrange
            bool hasBeenCalled = false;
            Action buttonAction = () => { hasBeenCalled = true; };
            
            var go = new GameObject();
            var sut = go.AddComponent<MapMenuController>();
            var dependencies = CreateDependencies();
            sut.SetDependencies(dependencies);

            sut.Configure(buttonAction);

            //Act
            sut.BackButtonAction();

            //Assert
            Assert.IsTrue(hasBeenCalled);
        }


        [Test]
        public void TestDisplay_CallsDisplayOnController()
        {
            //Arrange
            var go = new GameObject();
            var sut = go.AddComponent<MapMenuController>();

            var displayController = new Mock<IComponentDisplayController>();
            displayController.Setup(x => x.Display()).Verifiable();
        
            var dependencies = CreateDependencies(displayController);
            sut.SetDependencies(dependencies);
        
            //Act
            sut.Display();
            
            //Assert
            displayController.Verify(x => x.Display());

        }
    
        [Test]
        public void TestHide_CallsHideOnController()
        {
            //Arrange
            var go = new GameObject();
            var sut = go.AddComponent<MapMenuController>();

            var displayController = new Mock<IComponentDisplayController>();
            displayController.Setup(x => x.Hide()).Verifiable();
        
            var dependencies = CreateDependencies(displayController);
            sut.SetDependencies(dependencies);
        
            //Act
            sut.Hide();
            
            //Assert
            displayController.Verify(x => x.Hide());
        }
    }
}
