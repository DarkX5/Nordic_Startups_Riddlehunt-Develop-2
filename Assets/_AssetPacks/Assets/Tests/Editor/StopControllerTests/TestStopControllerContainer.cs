using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using StopControllers;
using UnityEngine;

public class TestStopControllerContainer
{
    [Test]
    public void TestAdd_AddsControllerToDictionary()
    {
        //Given a stopcontrollercontrain and a stopcontroller
        //when add is called with the stopcontroller
        //Then the controller is added to the dictionary in the container

        //Act
        var a_id = "a";
        var stopController = new Mock<IStopController>();
        stopController.Setup(x => x.GetId()).Returns(a_id).Verifiable();

        var sut = new StopControllerContainer();
        //Act
        sut.Add(stopController.Object);

        stopController.Verify(x => x.GetId());

        //Assert
        var loadedStopController = sut.Get(a_id);
        Assert.AreEqual(stopController.Object, loadedStopController);
    }
    
    [Test]
    public void TestGet_GetsControllerFromDictionary()
    {
        //Given a stopcontrollercontainer and a stopcontroller id
        //when Get is called with the stopcontroller id
        //Then the controller collected from the diction

        //Act
        var a_id = "a";
        var stopController = new Mock<IStopController>();
        stopController.Setup(x => x.GetId()).Returns(a_id);

        var sut = new StopControllerContainer();
        sut.Add(stopController.Object);

        //Act
        var loadedStopController = sut.Get(a_id);
        //Assert
        Assert.AreEqual(stopController.Object, loadedStopController);
    }
    
    [Test]
    public void TestRemoveAndDestroyAllControllers()
    {
        //Given a stopcontrollercontainer with 3 stopcontrollers
        //when RemoveAndDestroyAllControllers is called
        //Then the controllers are removed, and destroyed.

        //Act
        var stopController_a = new Mock<IStopController>();
        stopController_a.Setup(x => x.GetId()).Returns("a_id");
        stopController_a.Setup(x => x.DestroySelf()).Verifiable();

        var stopController_b = new Mock<IStopController>();
        stopController_b.Setup(x => x.GetId()).Returns("b_id");
        stopController_b.Setup(x => x.DestroySelf()).Verifiable();

        var stopController_c = new Mock<IStopController>();
        stopController_c.Setup(x => x.GetId()).Returns("c_id");
        stopController_c.Setup(x => x.DestroySelf()).Verifiable();

        var sut = new StopControllerContainer();
        sut.Add(stopController_a.Object);
        sut.Add(stopController_b.Object);
        sut.Add(stopController_c.Object);

        //Act
        sut.RemoveAndDestroyAllControllers();
        //Assert
        stopController_a.Verify(x => x.DestroySelf());
        stopController_b.Verify(x => x.DestroySelf());
        stopController_c.Verify(x => x.DestroySelf());
    }
}
