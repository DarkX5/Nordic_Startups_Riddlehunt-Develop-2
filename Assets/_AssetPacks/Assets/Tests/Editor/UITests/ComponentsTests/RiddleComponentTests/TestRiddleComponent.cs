using System;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;

public class TestRiddleComponent
{
    private Mock<IRiddleComponentActions> ActionComponentMock;
    private Mock<IViewActions> HuntComponentUiActionsMock;

    [SetUp]
    public void Init()
    {
        ActionComponentMock = new Mock<IRiddleComponentActions>();
        HuntComponentUiActionsMock = new Mock<IViewActions>();
    }

    [TearDown]
    public void TearDown()
    {
        ActionComponentMock = null;
        HuntComponentUiActionsMock = null;
    }

    [Test]
    public void TestConfigure_RiddleComponent_Is_Configured_With_A_RiddleText()
    {
        // Given a user receives a riddle
        // When a riddletab is opened
        // Then a riddle should be loaded with a given riddle text

        // Arrange
        const string riddleText = "This is a riddle text.";

        ActionComponentMock.Setup(x => x.Configure(riddleText)).Verifiable();
        var sut = new RiddleComponent(ActionComponentMock.Object, HuntComponentUiActionsMock.Object);

        // Act
        sut.Configure(riddleText);

        // Act & Assert
        ActionComponentMock.Verify(x => x.Configure(riddleText));
    }
}