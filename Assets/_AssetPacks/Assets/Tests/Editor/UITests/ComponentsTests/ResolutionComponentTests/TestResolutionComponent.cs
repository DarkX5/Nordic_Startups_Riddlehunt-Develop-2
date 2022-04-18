using System;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;

namespace Tests.Editor.UITests.ComponentsTests.ResolutionComponentTests
{
    public class TestResolutionComponent
    {
        private ResolutionComponent sut;
        private const string videoLink = "https://some.url";
        private Mock<IResolutionComponentActions> iResolutionComponentActionsMock;
        private Mock<IViewActions> iUIActions;

        [SetUp]
        public void SetUp()
        {
            iResolutionComponentActionsMock = new Mock<IResolutionComponentActions>();
            iUIActions = new Mock<IViewActions>();
            iUIActions.Setup(x => x.GetComponentType()).Returns(ComponentType.Resolution);
            sut = new ResolutionComponent(iResolutionComponentActionsMock.Object, iUIActions.Object);
        }

        [TearDown]
        public void TearDown()
        {
            sut = null;
        }
        
        [Test]
        public void TestConfigure()
        {
            // Arrange
            Action btnAction = () => { };
            iResolutionComponentActionsMock.Setup(x => x.Configure(videoLink)).Verifiable();
            
            // Act
            sut.Configure(btnAction, videoLink);
            
            // Assert
            iResolutionComponentActionsMock.Verify(x => x.Configure(videoLink));
        }
        
        [Test]
        public void TestPerformAction()
        {
            // Arrange
            var hasBeenCalled = false;
            Action btnAction = () => { hasBeenCalled = true; };
            sut.Configure(btnAction, videoLink);

            // Act
            sut.PerformAction();
            
            // Assert
            Assert.IsTrue(hasBeenCalled);
        }
        
    }
}