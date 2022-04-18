using System;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Editor.BehaviorTests
{
    public class TestResolutionComponentBehaviour
    {
        private GameObject go;
        private ResolutionComponentBehaviour sut;
        private Mock<IVideo> iVideoMock;
        private Mock<IStandardButton> iStandardButtonBehaviourMock;
        private Mock<IResolutionComponent> iResolutionComponentMock; 
        private const string videoLink = "https://some.url";

        [SetUp]
        public void SetUp()
        {
            go = new GameObject();
            iStandardButtonBehaviourMock = new Mock<IStandardButton>();
            iVideoMock = new Mock<IVideo>();
            iResolutionComponentMock = new Mock<IResolutionComponent>();
            sut = go.AddComponent<ResolutionComponentBehaviour>();
        }
        
        [TearDown]
        public void TearDown()
        {
            go = null;
            sut = null;
        }
        [Ignore("not in use - new video system required.")]
        [Test]
        public void TestConfigure()
        {
            // Arrange
            iVideoMock.Setup(x => x.Configure(videoLink)).Verifiable();
            iStandardButtonBehaviourMock.Setup(x => x.Configure(ResolutionComponentBehaviour.BtnText,It.IsAny<Action>())).Verifiable();
            sut.SetDependencies(iStandardButtonBehaviourMock.Object, iVideoMock.Object);
            
            // Act
            sut.Configure(videoLink);
            
            // Assert
            iStandardButtonBehaviourMock.Verify(x => x.Configure(ResolutionComponentBehaviour.BtnText,It.IsAny<Action>()));
            iVideoMock.Verify(x=>x.Configure(videoLink));
        }

        [Test]
        public void PerformAction()
        { 
            // Arrange
            iResolutionComponentMock.Setup(x=>x.PerformAction()).Verifiable();
            sut.SetLogicInstance(iResolutionComponentMock.Object);
            
            // Act
            sut.PerformAction();
            
            // Assert
            iResolutionComponentMock.Verify(x=>x.PerformAction());
        }
    }
}