using System.Collections;
using System.Collections.Generic;
using DigitalRubyShared;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products.models;
using UnityEngine;

public class TestMapPlayerMover
{

  private MapPlayerMover.Dependencies CreateDependencies()
  {
    return new MapPlayerMover.Dependencies()
    {
    };
  }

  private MapPlayerMover.Config CreateConfig(Mock<IPanGestureRecognizer> panGestureRecognizer = null)
  {
    panGestureRecognizer ??= new Mock<IPanGestureRecognizer>();
    return new MapPlayerMover.Config()
    {
      PanGestureRecognizer = panGestureRecognizer.Object,
      MoveSpeed = 5f
    };
  }
  
  [Test]
  public void TestSetDependencies()
  {
    //Arrange
    var sut = new GameObject().AddComponent<MapPlayerMover>();
    var dependencies = CreateDependencies();
    //Act
    sut.SetDependencies(dependencies);
    //Assert
    Assert.AreEqual(dependencies, sut._dependencies);
  }

  [Test]
  public void TestConfigure()
  {
    //Given a new initialized MapPlayerMover
    //When COnfigure is called
    //Then the config variables are stored, and the gesturerecognize event is attached. (previous is unsubscribed, to avoid double subs)
    
    //Arrange
    var panGestureRecognizerMock = new Mock<IPanGestureRecognizer>();
    panGestureRecognizerMock.Setup(x => 
        x.SubscribeToStateUpdated(It.IsAny<GestureRecognizerStateUpdatedDelegate>()))
      .Verifiable();

    var sut = new GameObject().AddComponent<MapPlayerMover>();
    var dependencies = CreateDependencies();
    sut.SetDependencies(dependencies);

    var config = CreateConfig(panGestureRecognizerMock);
    //Act
    sut.Configure(config);

    //Assert
    panGestureRecognizerMock.Verify(x => 
        x.SubscribeToStateUpdated(It.IsAny<GestureRecognizerStateUpdatedDelegate>()));
  }
  
  [Test]
  public void TestDetachPanGesture()
  {
    //Given a configured MapPlayerMover
    //When DetachPanGesture is called
    //Then the gesture is unsubscribed.
    
    //Arrange
    var panGestureRecognizerMock = new Mock<IPanGestureRecognizer>();
    panGestureRecognizerMock.Setup(x => 
        x.UnsubscribeToStateUpdated(It.IsAny<GestureRecognizerStateUpdatedDelegate>()))
      .Verifiable();

    var sut = new GameObject().AddComponent<MapPlayerMover>();
    var dependencies = CreateDependencies();
    sut.SetDependencies(dependencies);

    var config = CreateConfig(panGestureRecognizerMock);
    sut.Configure(config);

    //Act
    sut.DetachPanGesture();

    //Assert
    panGestureRecognizerMock.Verify(x => 
      x.UnsubscribeToStateUpdated(It.IsAny<GestureRecognizerStateUpdatedDelegate>()));
  }
  
  [Test]
  public void TestOnDisable()
  {
    //Given a configured MapPlayerMover
    //When Object is disabled
    //Then the gesture is unsubscribed.

    //Arrange
    var panGestureRecognizerMock = new Mock<IPanGestureRecognizer>();
    panGestureRecognizerMock.Setup(x => 
        x.UnsubscribeToStateUpdated(It.IsAny<GestureRecognizerStateUpdatedDelegate>()))
      .Verifiable();

    var sut = new GameObject().AddComponent<MapPlayerMover>();
    var dependencies = CreateDependencies();
    sut.SetDependencies(dependencies);

    var config = CreateConfig(panGestureRecognizerMock);
    sut.Configure(config);

    //Act
    sut.OnDisable();

    //Assert
    panGestureRecognizerMock.Verify(x => 
      x.UnsubscribeToStateUpdated(It.IsAny<GestureRecognizerStateUpdatedDelegate>()));
  }
  
  [Test]
  public void TestOnEnable()
  {
    //Given a configured MapPlayerMover
    //When Object is enabled
    //Then the gesture is subscribed.

    //Arrange
    var panGestureRecognizerMock = new Mock<IPanGestureRecognizer>();
    panGestureRecognizerMock.Setup(x => 
        x.SubscribeToStateUpdated(It.IsAny<GestureRecognizerStateUpdatedDelegate>()))
      .Verifiable();

    var sut = new GameObject().AddComponent<MapPlayerMover>();
    var dependencies = CreateDependencies();
    sut.SetDependencies(dependencies);

    var config = CreateConfig(panGestureRecognizerMock);
    sut.Configure(config);

    //Act
    sut.OnEnable();

    //Assert
    panGestureRecognizerMock.Verify(x => 
      x.SubscribeToStateUpdated(It.IsAny<GestureRecognizerStateUpdatedDelegate>()));
  }

  [Test]
  public void TestPositionPlayer_And_GetPosition() //testing both positionPlayer and GetPosition.
  {
    //Given a new MapPlayerMover positioned at 0,0.
    //Outcome PositionPlayer is called, it is repositioned to 3,3 and GetPosition returns 3f,3f
    
    //Arrange
    var sut = new GameObject().AddComponent<MapPlayerMover>();
    var dependencies = CreateDependencies();
    sut.SetDependencies(dependencies);

    Vector3 newPos = Vector3.one*3f;
    newPos.z = 0f;
    sut.PositionPlayer(newPos);
    //Act
    var pos = sut.GetPosition();
    //Assert
    Assert.AreEqual(newPos, pos);
  }
}
