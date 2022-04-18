using System;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;
using UnityEngine;

namespace Tests.Editor.UITests.ComponentsTests
{
    public class TestRatingSystem
    {
        private IRatingComponent sut;
        private Mock<IRatingComponentComponentActions> iRatingComponentActionsMock;
        private Mock<IViewActions> iHuntComponentUIActionsMock;

        [OneTimeSetUp]
        public void Init()
        {
            iRatingComponentActionsMock = new Mock<IRatingComponentComponentActions>();
            iHuntComponentUIActionsMock = new Mock<IViewActions>();
        }

        [SetUp]
        public void Setup()
        {
            sut = new RatingComponent(iRatingComponentActionsMock.Object, iHuntComponentUIActionsMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            sut = null;
        }

        [Test]
        public void TestFactory_Returns_RatingComponent()
        {
            // Given a user ends a hunt
            // When creating the rating component
            // Then a RatingComponent is created through the factory

            // Arrange
            var testGo = new GameObject();
            testGo.AddComponent<RatingComponentBehaviour>();

            // Act
            var localSut = RatingComponent.Factory(testGo);

            // Assert
            Assert.NotNull(localSut);
        }

        [Test]
        public void TestFactory_GameObject_Does_not_haveBehaviour()
        {
            // Given a user ends a hunt
            // When creating the rating component with a rating component behaviour
            // Then an ArgumentException is thrown

            // Arrange
            var testGo = new GameObject();

            // Act and Assert
            Assert.Throws<ArgumentException>(() => RatingComponent.Factory(testGo));
        }

        [Test]
        public void TestPerformAction_Performs_Action()
        {
            // Given a user wishes to submit their rating of the hunt experience
            // When pressing the button
            // The PerformAction associated with the button is called

            // Arrange

            var actionPerformedAsExpected = false;
            void BtnAction(int rating) {
                if (rating == -1)
                {
                    actionPerformedAsExpected = true;
                }
            }
            iRatingComponentActionsMock.Setup(x => x.SliderHasBeenDragged()).Returns(false).Verifiable();


            // Act
            sut.Configure(BtnAction);
            sut.PerformAction();

            // Assert
            Assert.IsTrue(actionPerformedAsExpected);
            iRatingComponentActionsMock.Verify(x => x.SliderHasBeenDragged());
        }

        [Test]
        public void TestGetComponentUIActions()
        {
            // Given a user wishes to submit their rating of a riddlehunt experience that has just ended
            // When creating the rating system component
            // Then the rest of the system needs to be able to get the rating systems's IHuntComponentUIActions Object
            
            // Arrange
            var expectedType = iHuntComponentUIActionsMock.Object.GetType().Name;
            
            // Act
            var result = sut.GetComponentUIActions();
            
            // Assert
            Assert.NotNull(expectedType);
            Assert.NotNull(result); // Redundant? The type for a null literal is 'null'.
            Assert.AreEqual(expectedType, result.GetType().Name);
        }

        [Test]
        public void TestConfigure_Calls_Configure_In_BehaviourLayer()
        {
            // Given a user wishes to submit their rating of a riddlehunt experience that has just ended
            // When creating the rating system component
            // The rating system is configured
            
            // Arrange
            const float ratingValue = 2f;
            iRatingComponentActionsMock.Setup(x => x.Configure(ratingValue)).Verifiable();

            // Act
            sut.Configure(null);
            
            // Assert
            iRatingComponentActionsMock.Verify(x => x.Configure(ratingValue));
        }

        [Test]
        public void TestConfigure_SmileysStartAt3()
        {
            // Given a user rates the app
            // When creating the rating view
            // Then the initial slider value starts in the middle - 3.
            
            // Arrange
            const short startValue = 2;
            iRatingComponentActionsMock.Setup(x => x.Configure(startValue)).Verifiable();
            
            // Act
            sut.Configure(null);
            
            // Assert
            iRatingComponentActionsMock.Verify(x => x.Configure(startValue));
        }
    }
}