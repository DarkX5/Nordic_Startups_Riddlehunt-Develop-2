using System;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Editor.UITests.ComponentsTests.StepsTests
{
    [TestFixture]
    public class TestStoryComponent
    {
        [Test]
        public void TestFactory_Creates_New_Instance()
        {
            // Given a game object with a StoryComponentBehaviour component
            // When Factory() is called
            // Then return a new instance of StoryComponent that references the given component
            
            // Arrange
            var testGo = new GameObject();
            testGo.AddComponent<StoryHuntComponentBehaviour>();

            // Act
            IStoryComponent sut = StoryComponent.Factory(testGo);

            // Assert
            Assert.IsNotNull(sut);
        }
        
        [Test]
        public void TestFactory_Throws_ArgumentException_If_GameObject_Does_Not_Have_Expected_Component()
        {
            // Given a game object without a StoryComponentBehaviour component
            // When Factory() is called
            // Then an argument exception is thrown
            
            // Arrange
            GameObject testGo = new GameObject();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => StoryComponent.Factory(testGo));
        }

        [Test]
        public void TestConstructor_Calls_SetStory()
        {
            // Given a new StoryComponent
            // When the constructor is called
            // The function SetStory is called with self 
            
            // Arrange
            var storyComponentActionsMock = new Mock<IStoryComponentActions>();
            storyComponentActionsMock.Setup(x => x.SetStory(It.IsAny<StoryComponent>())).Verifiable();

            // Act
            var sut = new StoryComponent(storyComponentActionsMock.Object, null);

            // Assert
            storyComponentActionsMock.Verify(x => x.SetStory(It.IsAny<StoryComponent>()));
        }


        [Test]
        public void TestConfigure_Configure_Calls_Configure_From_Actions_Layer()
        {
            // Given a story component
            // When calling the Configure function
            // Then the action layer's configure function is called 
            
            // Arrange
            string testStoryText = "This is a test story text";
            string buttonText = "Ok";
            Action testAction = ()=>{};
            
            var storyComponentActionsMock = new Mock<IStoryComponentActions>();
            storyComponentActionsMock.Setup(x => x.Configure(testStoryText, buttonText)).Verifiable();
            
            // Act
            var sut = new StoryComponent(storyComponentActionsMock.Object, null);
            sut.Configure(testStoryText, buttonText, testAction);
            
            // Assert
            storyComponentActionsMock.Verify(x=>x.Configure(testStoryText, buttonText));
        }

        [Test]
        public void TestPerformAction_Invokes_ButtonAction()
        {
            // Given a StoryComponent with a ButtonAction
            // When calling ButtonAction()
            // Then ButtonAction is invoked. 
            
            // Arrange
            bool hasBeenCalled = false;
            string testStoryText = "This is a test story text";
            string buttonText = "Ok";
            Action testAction = () =>
            {
                hasBeenCalled = true;
            };

            var storyComponentActionsMock = new Mock<IStoryComponentActions>();
            storyComponentActionsMock.Setup(x => x.Configure(testStoryText, buttonText)).Verifiable();
            
            // Act
            var sut = new StoryComponent(storyComponentActionsMock.Object, null);
            sut.Configure(testStoryText,buttonText, testAction);
            sut.PerformAction();
            
            // Assert
            Assert.IsTrue(hasBeenCalled);
        }
        
        [Test]
        public void TestPerformAction_Button_Action_Null_Throws_NullReferenceException()
        {
            // Given a StoryComponent with a ButtonAction.
            // When calling ButtonAction().
            // Then ButtonAction is invoked. 
            
            // Arrange
            var storyComponentActionsMock = new Mock<IStoryComponentActions>();

            // Act
            var sut = new StoryComponent(storyComponentActionsMock.Object, null);
            
            // Assert
            Assert.Catch<NullReferenceException>(sut.PerformAction);
        }
    }
}