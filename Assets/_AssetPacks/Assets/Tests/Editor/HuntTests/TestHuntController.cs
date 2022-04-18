using System;
using System.Collections;
using System.Collections.Generic;
using Hunt;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.huntProduct;
using riddlehouse_libraries.products.Stops;
using UnityEngine;

[TestFixture]
public class TestHuntController
{
    HuntController.Dependencies CreateDependencies(
        Mock<IStopControllerInstantiator> stopControllerInstantiatorMock = null,
        Mock<IGameObjectDestroyer> godMock = null,
        Mock<IStopModelConverter> stopModelConverter = null)
    {
        if (stopControllerInstantiatorMock == null)
        {
            stopControllerInstantiatorMock = new Mock<IStopControllerInstantiator>();
            
            var map2DStopControllerMock = new Mock<IMap2DStopController>();
            map2DStopControllerMock
                .Setup(x => 
                    x.ConfigureAndOpenStop(It.IsAny<Map2DStopController.Config>()));
            
            stopControllerInstantiatorMock.Setup(x => x.CreateMap2D(null)).Returns(map2DStopControllerMock.Object);
        }

        if (stopModelConverter == null)
        {
            stopModelConverter = new Mock<IStopModelConverter>();
            stopModelConverter.Setup(x => 
                x.ConvertMap2DModel(It.IsAny<IStop>())).Returns(new Mock<IMap2DStop>().Object);
            stopModelConverter.Setup(x => 
                x.ConvertMapBoxModel(It.IsAny<IStop>())).Returns(new Mock<IMapBoxStop>().Object);
        }

        
        godMock ??= new Mock<IGameObjectDestroyer>();
        return new HuntController.Dependencies()
        { 
            StopControllerInstantiator = stopControllerInstantiatorMock.Object,
            StopModelConverter =  stopModelConverter.Object,
            GOD = godMock.Object
        };
    }

    HuntController.Config CreateConfig(HuntProductFlow flow = null, Mock<IStop> stop = null)
    {
        flow ??= new HuntProductFlow();
        
        if (stop == null)
        {
            stop = new Mock<IStop>();
            stop.Setup(x=> x.Type).Returns(StopType.MapStop2D);
            flow.Stop = stop.Object;
        }

        return new HuntController.Config
        {
            Ready = () => {},
            Flow = flow
        };
    }
    
    [Test]
    public void SetDependencies()
    {
        //Arrange
        var sut = new GameObject().AddComponent<HuntController>();
        
        var dependencies = CreateDependencies();
        
        //Act
        sut.SetDependencies(dependencies);
        //Assert
        Assert.AreEqual(dependencies, sut._dependencies);
    }
    
    [Test]
    public void TestConfigure_CreatesAndConfigures_Map2DStopController()
    {
        //Arrange
        Mock<IStopModelConverter> modelConverter = new Mock<IStopModelConverter>();
        var flow = new HuntProductFlow();
        var stopMock = new Mock<IStop>();
        stopMock.Setup(x => x.Type).Returns(StopType.MapStop2D);
        var map2DStop = new Mock<IMap2DStop>();
        modelConverter.Setup(x => x.ConvertMap2DModel(stopMock.Object)).Returns(map2DStop.Object);
        flow.Stop = stopMock.Object;
        
        var sut = new GameObject().AddComponent<HuntController>();

        var stopControllerInstantiater = new Mock<IStopControllerInstantiator>();
        var map2DStopControllerMock = new Mock<IMap2DStopController>();
        map2DStopControllerMock
            .Setup(x => 
                x.ConfigureAndOpenStop(It.IsAny<Map2DStopController.Config>()))
            .Verifiable();
        map2DStopControllerMock.Setup(x => x.OpenStop()).Verifiable();

        
        stopControllerInstantiater
            .Setup(x => x.CreateMap2D(null))
            .Returns(map2DStopControllerMock.Object)
            .Verifiable();
        
        var dependencies = CreateDependencies(stopControllerInstantiater,  null, modelConverter);
        sut.SetDependencies(dependencies);

        var config = CreateConfig(flow);
        //Act
        sut.Configure(config);
        
        //Assert
        stopControllerInstantiater.Verify(x => x.CreateMap2D(null));
        map2DStopControllerMock.Verify(x => x.ConfigureAndOpenStop(It.IsAny<Map2DStopController.Config>()));
        map2DStopControllerMock.Verify(x => x.OpenStop(), Times.Never);
    }
    
    [Test]
    public void TestConfigure_CreatesAndConfigures_MapBoxStopController()
    {
        //Arrange
        Mock<IStopModelConverter> modelConverter = new Mock<IStopModelConverter>();
        var flow = new HuntProductFlow();
        var stopMock = new Mock<IStop>();
        stopMock.Setup(x => x.Type).Returns(StopType.MapBox);

        var mapBoxStop = new Mock<IMapBoxStop>();
        modelConverter.Setup(x => x.ConvertMapBoxModel(stopMock.Object)).Returns(mapBoxStop.Object);
        flow.Stop = stopMock.Object;

        var sut = new GameObject().AddComponent<HuntController>();

        var stopControllerInstantiater = new Mock<IStopControllerInstantiator>();

        var mapBoxStopControllerMock = new Mock<IMapBoxStopController>();
        mapBoxStopControllerMock
            .Setup(x => 
                x.Configure(It.IsAny<MapBoxStopController.Config>()))
            .Verifiable();
        mapBoxStopControllerMock.Setup(x => x.OpenStop()).Verifiable();

        stopControllerInstantiater
            .Setup(x => x.CreateMapBox(null))
            .Returns(mapBoxStopControllerMock.Object)
            .Verifiable();

        var dependencies = CreateDependencies(stopControllerInstantiater,  null, modelConverter);
        sut.SetDependencies(dependencies);
        var config = CreateConfig(flow, stopMock);

        //Act
        sut.Configure(config);
        
        //Assert
        stopControllerInstantiater.Verify(x => x.CreateMapBox(null));
        mapBoxStopControllerMock
            .Verify(x => 
                x.Configure(It.IsAny<MapBoxStopController.Config>()));
        mapBoxStopControllerMock.Verify(x => x.OpenStop());
    }

    [TestCase(true)]
    [TestCase(false)]
    [Test]
    public void TestEndHunt_EndHuntCalled_From_Config(bool completed)
    {
        //Given a configured HuntController
        //When EndHunt is called
        //Then the action in config is called.
        
        //Arrange
        var sut = new GameObject().AddComponent<HuntController>();

       
        var dependencies = CreateDependencies();
        sut.SetDependencies(dependencies);

        var config = CreateConfig();
        
        bool hasBeenCalled = false;
        bool value = !completed;
        config.EndHunt = (completed) =>
        {
            hasBeenCalled = true;
            value = completed;
        };
        
        sut.Configure(config);
        
        //Act
        sut.EndHunt(completed);

        //Assert
        Assert.IsTrue(hasBeenCalled);
        Assert.AreEqual(completed, value);
    }
    
    [Test]
    public void TestConfigure_StopController_InitializeComplete_Invoked_Successful()
    {
        //Given a flow object, and a new HuntController
        //When configure is called and the stopController reports that it is initialized properly.
        //Then the HuntInitialized callback calls OpenStop on the StopController.
        
        //Arrange
        var flow = new HuntProductFlow();
        var stopMock = new Mock<IStop>();
        stopMock.Setup(x=> x.Type).Returns(StopType.MapStop2D).Verifiable();
        flow.Stop = stopMock.Object;

        var sut = new GameObject().AddComponent<HuntController>();

        var stopcontrollerInstantiater = new Mock<IStopControllerInstantiator>();
        var map2DStopControllerMock = new Mock<IMap2DStopController>();
        map2DStopControllerMock.Setup(x => x.ConfigureAndOpenStop(It.IsAny<Map2DStopController.Config>()))
            .Verifiable();
        map2DStopControllerMock.Setup(x => x.OpenStop()).Verifiable();

        stopcontrollerInstantiater.Setup(x => x.CreateMap2D(null))
            .Returns(map2DStopControllerMock.Object).Verifiable();
        
        var dependencies = CreateDependencies(stopcontrollerInstantiater);
        sut.SetDependencies(dependencies);
        var config = CreateConfig(flow, stopMock);

        //Act
        sut.Configure(config);
        
        //Assert
        map2DStopControllerMock.Verify(x => x.ConfigureAndOpenStop(It.IsAny<Map2DStopController.Config>()));
        stopMock.Verify(x=> x.Type);
    }
    
    [Test]
    public void TestDestroySelf_Destroys_GameObject_And_StopController()
    {
        //Arrange
        var flow = new HuntProductFlow();
        var stopMock = new Mock<IStop>();
        stopMock.Setup(x => x.Type).Returns(StopType.MapStop2D);

        flow.Stop = stopMock.Object;

        var sut = new GameObject().AddComponent<HuntController>();

        var map2DStopControllerMock = new Mock<IMap2DStopController>();
        map2DStopControllerMock.Setup(x => x.DestroySelf()).Verifiable();

        var stopControllerInstantiater = new Mock<IStopControllerInstantiator>();
        stopControllerInstantiater.Setup(x => x.CreateMap2D(null))
            .Returns(map2DStopControllerMock.Object);

        var godMock = new Mock<IGameObjectDestroyer>();
        godMock.Setup(x => x.Destroy()).Verifiable();
        
        var dependencies = CreateDependencies(stopControllerInstantiater, godMock);
        sut.SetDependencies(dependencies);
        var config = CreateConfig(flow);
        sut.Configure(config);
        
        //Act
        sut.DestroySelf();
        
        //Assert
        map2DStopControllerMock.Verify(x => x.DestroySelf());
        godMock.Verify(x => x.Destroy());
    }
}
