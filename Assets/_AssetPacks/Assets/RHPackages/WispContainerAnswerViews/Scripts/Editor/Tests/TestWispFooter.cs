using System;
using Moq;
using NUnit.Framework;
using UnityEngine;

public class TestWispFooter 
{
  private WispFooter.Dependencies CreateDependencies(Mock<ICharacterIconDisplay> characterIconDisplayMock = null)
  {
    characterIconDisplayMock ??= new Mock<ICharacterIconDisplay>();
    return new WispFooter.Dependencies()
    {
      CharacterIconDisplay = characterIconDisplayMock.Object
    };
  }

  [Test]
  public void TestSetDependencies()
  {
      var sut = new GameObject().AddComponent<WispFooter>();
      var dependencies = CreateDependencies();
      sut.SetDependencies(dependencies);
      
      Assert.AreEqual(dependencies, sut._dependencies);
  }

  [Test]
  public void TestConfigure()
  {
    //Given a new WispFooter, a frameColor, an icon, and two actions
    //When configure is called
    //Then the icon and framecolor is passed on, and the actions are stored for later use.
    
    var sut = new GameObject().AddComponent<WispFooter>();
    var color = Color.black;
    var icon = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);

    var characterIconDisplayMock = new Mock<ICharacterIconDisplay>();
    characterIconDisplayMock.Setup(x => x.Configure(color, icon)).Verifiable();
    
    var dependencies = CreateDependencies(characterIconDisplayMock);
    sut.SetDependencies(dependencies);
    sut.Configure(color, icon, null, null);
    
    characterIconDisplayMock.Verify(x => x.Configure(color, icon));
  }
  [Test]
  public void TestAcceptbtn()
  {
    //Given a configured wispfooter
    //When AcceptBtn is called
    //Then the configured action is invoked
    
    //Arrange
    var sut = new GameObject().AddComponent<WispFooter>();
    
    var dependencies = CreateDependencies();
    sut.SetDependencies(dependencies);

    bool actionInvoked = false;
    Action acceptAction = () =>
    {
      actionInvoked = true;
    };
    
    sut.Configure(Color.black, null, acceptAction, null);
    
    //Act
    sut.AcceptBtn();
    
    //Assert
    Assert.IsTrue(actionInvoked);
  }
  
  [Test]
  public void TestAbortBtn()
  {
      //Given a configured wispfooter
      //When AbortBtn is called
      //Then the configured action is invoked

      //Arrange
      var sut = new GameObject().AddComponent<WispFooter>();
    
      var dependencies = CreateDependencies();
      sut.SetDependencies(dependencies);

      bool actionInvoked = false;
      Action abortAction = () =>
      {
        actionInvoked = true;
      };
    
      sut.Configure(Color.black, null, null, abortAction);
    
      //Act
      sut.AbortBtn();
    
      //Assert
      Assert.IsTrue(actionInvoked);
  }
}
