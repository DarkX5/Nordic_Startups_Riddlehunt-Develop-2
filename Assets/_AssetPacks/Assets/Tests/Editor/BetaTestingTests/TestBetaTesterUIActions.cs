using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Riddlehunt.Beta.Environment.Controls;
using UnityEngine;

[TestFixture]
public class TestBetaTesterUIActions
{
    private Mock<ITouchCounter> touchCounterMock;
    private Mock<IWaitToExecuteAction> waitToExecuteActionMock;
    private Mock<ICanvasLayerTypes> canvasLayerTypesMock;
    private Mock<IBetaTesterController> betaTesterCanvasControllerMock;
    [SetUp]
    public void Init()
    {
        touchCounterMock = new Mock<ITouchCounter>();
        waitToExecuteActionMock = new Mock<IWaitToExecuteAction>();
        canvasLayerTypesMock = new Mock<ICanvasLayerTypes>();
        betaTesterCanvasControllerMock = new Mock<IBetaTesterController>();
    }

    [TearDown]
    public void TearDown()
    {
        touchCounterMock = null;
        waitToExecuteActionMock = null;
        canvasLayerTypesMock = null;
        betaTesterCanvasControllerMock = null;
    }

    [Test]
    public void TestSetDependencies()
    {
        waitToExecuteActionMock.Setup(x => 
            x.Configure(It.IsAny<Action>(), It.IsAny<float>())).Verifiable();
        canvasLayerTypesMock.Setup(x => 
            x.CreateIBetaTesterCanvasController()).Returns(betaTesterCanvasControllerMock.Object).Verifiable();
        var sut = new GameObject().AddComponent<BetaTesterUIActions>();
        var dependencies = new BetaTesterUIActions.Dependencies()
        {
            Clm = new CanvasLayerManager(canvasLayerTypesMock.Object),
            TouchCounter = touchCounterMock.Object,
            WaitToExecuteAction = waitToExecuteActionMock.Object
        };
        sut.SetDependencies(dependencies);
        
        Assert.AreEqual(dependencies, sut._dependencies);
        canvasLayerTypesMock.Verify(x => x.CreateIBetaTesterCanvasController());
        waitToExecuteActionMock.Verify(x => x.Configure(It.IsAny<Action>(), It.IsAny<float>()));

    }

    [Test]
    public void TestFixedUpdate_TouchCountApproved_BeginsWaiting_AndExecutes()
    {
        //Given a betaTesterCanvasController and a touchCounter
        //when a user places 3 fingers on the screen and waits 0.5 seconds
        //Then the system begins waiting and executes the action.
        betaTesterCanvasControllerMock.Setup(x=> x.Configure()).Verifiable();
        canvasLayerTypesMock.Setup(x => x.CreateIBetaTesterCanvasController()).Returns(betaTesterCanvasControllerMock.Object).Verifiable();
        waitToExecuteActionMock.Setup(x => x.Configure(It.IsAny<Action>(), It.IsAny<float>()))
            .Callback<Action, float>((theAction, thefloat) =>
            {
                theAction.Invoke();
            });
        waitToExecuteActionMock.Setup(x=> x.IsWaiting()).Returns(false).Verifiable();
        waitToExecuteActionMock.Setup(x => x.BeginWaiting()).Verifiable();
        touchCounterMock.Setup(x => x.GetTouchCount()).Returns(3).Verifiable();
        
        var sut = new GameObject().AddComponent<BetaTesterUIActions>();
        var dependencies = new BetaTesterUIActions.Dependencies()
        {
            Clm = new CanvasLayerManager(canvasLayerTypesMock.Object),
            TouchCounter = touchCounterMock.Object,
            WaitToExecuteAction = waitToExecuteActionMock.Object
        };
        sut.SetDependencies(dependencies);

        sut.FixedUpdate();
        
        betaTesterCanvasControllerMock.Verify(x=> x.Configure());
        waitToExecuteActionMock.Verify(x => x.IsWaiting());
        waitToExecuteActionMock.Verify(x => x.BeginWaiting());
        touchCounterMock.Verify(x => x.GetTouchCount());
    }
    
    [Test]
    public void TestFixedUpdate_TouchInterrupted_StopsWaiting_DoesntExecute()
    {
        //Given a user has begun touching the screen with 3 touches
        //When user removes his fingers before the wait is up.
        //Then the coroutine is stopped, and the screen is never shown.
        betaTesterCanvasControllerMock.Setup(x=> x.Display()).Verifiable();
        canvasLayerTypesMock.Setup(x => x.CreateIBetaTesterCanvasController()).Returns(betaTesterCanvasControllerMock.Object).Verifiable();
        waitToExecuteActionMock.Setup(x=> x.StopWaiting()).Verifiable();
        waitToExecuteActionMock.Setup(x=> x.IsWaiting()).Returns(true).Verifiable();
        touchCounterMock.Setup(x => x.GetTouchCount()).Returns(2).Verifiable();
        var sut = new GameObject().AddComponent<BetaTesterUIActions>();
        var dependencies = new BetaTesterUIActions.Dependencies()
        {
            Clm = new CanvasLayerManager(canvasLayerTypesMock.Object),
            TouchCounter = touchCounterMock.Object,
            WaitToExecuteAction = waitToExecuteActionMock.Object
        };
        sut.SetDependencies(dependencies);

        sut.FixedUpdate();
        betaTesterCanvasControllerMock.Verify(x=> x.Display(), Times.Never);

        waitToExecuteActionMock.Verify(x=> x.IsWaiting());
        waitToExecuteActionMock.Verify(x=> x.StopWaiting());
        touchCounterMock.Verify(x => x.GetTouchCount());

    }
    
    [Test]
    public void TestFixedUpdate_NoControllerAvailable_Terminates()
    {
        //Given a user attempts to open the betatester screen
        //When no screen is available
        //Then the process terminates.
        
        waitToExecuteActionMock.Setup(x => x.BeginWaiting()).Verifiable();
        waitToExecuteActionMock.Setup(x => x.StopWaiting()).Verifiable();

        var sut = new GameObject().AddComponent<BetaTesterUIActions>();
        var dependencies = new BetaTesterUIActions.Dependencies()
        {
            Clm = new CanvasLayerManager(canvasLayerTypesMock.Object),
            TouchCounter = touchCounterMock.Object,
            WaitToExecuteAction = waitToExecuteActionMock.Object
        };
        sut.SetDependencies(dependencies);

        sut.FixedUpdate();
        
        waitToExecuteActionMock.Verify(x => x.BeginWaiting(), Times.Never);
        waitToExecuteActionMock.Verify(x => x.StopWaiting(), Times.Never);

    }
}
