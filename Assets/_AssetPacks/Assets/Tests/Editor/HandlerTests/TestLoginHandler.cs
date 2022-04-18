using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries;
using riddlehouse_libraries.login;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;

[TestFixture]
public class TestLoginHandler : ZenjectUnitTestFixture
{
    [Test]
    public void TestInitializeLoginHandler_Guest()
    {
        var loginWebviewMock = new Mock<ILoginWebview>();
        var startupMock = new Mock<IStartup>();
        var loginServiceMock = new Mock<ILoginService>();
        PlayerPrefs.SetString("Token", "");
        loginServiceMock.Setup(x => x.Login()).ReturnsAsync("GuestToken").Verifiable();
        loginServiceMock.Setup(x => x.IsCurrentUserLoggedIn()).ReturnsAsync(false).Verifiable();
        startupMock.Setup(x => x.InitializeLibrary(It.IsAny<string>(),It.IsAny<string>(),"")).Returns(loginServiceMock.Object).Verifiable();
        var loggedInAsGuest = false;
        LoginHandler sut = new LoginHandler(loginWebviewMock.Object, startupMock.Object, (loginActions) =>
        {
            if (loginActions == LoginEvents.loggedInAsGuest)
            {
                loggedInAsGuest = true;
            }
            else
            {
                loggedInAsGuest = false;
            }
        });

        startupMock.Verify(x => x.InitializeLibrary(It.IsAny<string>(), It.IsAny<string>(), ""));
        loginServiceMock.Verify(x => x.Login());
        loginServiceMock.Verify(x => x.IsCurrentUserLoggedIn());
        var tokenStored = PlayerPrefs.GetString("Token");
        Assert.AreEqual("GuestToken",tokenStored);
        Assert.IsTrue(loggedInAsGuest);
    }
    
    [Test]
    public void TestInitializeLoginHandler_User()
    {
        var loginWebviewMock = new Mock<ILoginWebview>();
        var startupMock = new Mock<IStartup>();
        var loginServiceMock = new Mock<ILoginService>();

        PlayerPrefs.SetString("Token", "");
        loginServiceMock.Setup(x => x.Login()).ReturnsAsync("UserToken").Verifiable();
        loginServiceMock.Setup(x => x.IsCurrentUserLoggedIn()).ReturnsAsync(true).Verifiable();
        startupMock.Setup(x => x.InitializeLibrary(It.IsAny<string>(),It.IsAny<string>(),"")).Returns(loginServiceMock.Object).Verifiable();
        var loggedInAsUser = false;
        LoginHandler sut = new LoginHandler(loginWebviewMock.Object, startupMock.Object, (loginActions) =>
        {
            if (loginActions == LoginEvents.loggedInAsUser)
            {
                loggedInAsUser = true;
            }
            else
            {
                loggedInAsUser = false;
            }
        });
        startupMock.Verify(x => x.InitializeLibrary(It.IsAny<string>(),It.IsAny<string>(),""));
        loginServiceMock.Verify(x => x.Login());
        loginServiceMock.Verify(x => x.IsCurrentUserLoggedIn());
        var tokenStored = PlayerPrefs.GetString("Token");
        Assert.AreEqual("UserToken",tokenStored);
        Assert.IsTrue(loggedInAsUser);
    }

    [Test]
    public void TestLoginAction()
    {
        var loginWebviewMock = new Mock<ILoginWebview>();
        loginWebviewMock.Setup(x => x.DisplayLogin(It.IsAny<string>(), It.IsAny<Action<string>>())).Verifiable();
        var startupMock = new Mock<IStartup>();
        var loginServiceMock = new Mock<ILoginService>();
        var code = "uniwebview://auth0Callback?code=shortLivedToken";
        var newJwt = "longlivedToken";
        PlayerPrefs.SetString("Token", "");
        loginServiceMock.Setup(x => x.Login()).ReturnsAsync("UserToken").Verifiable();
        loginServiceMock.Setup(x => x.ExchangeAuthCodeForToken(code)).ReturnsAsync(newJwt);
        startupMock.Setup(x => x.InitializeLibrary(It.IsAny<string>(),It.IsAny<string>(),"")).Returns(loginServiceMock.Object).Verifiable();
        loginServiceMock.Setup(x => x.IsCurrentUserLoggedIn()).ReturnsAsync(true).Verifiable();
        var loggedInAsUser = false;
        LoginHandler sut = new LoginHandler(loginWebviewMock.Object, startupMock.Object, (loginActions) =>
        {
            if (loginActions == LoginEvents.loggedInAsUser)
            {
                loggedInAsUser = true;
            }
            else
            {
                loggedInAsUser = false;
            }
        });
        sut.LoginAction();
        sut.UnpackCallback(code);
        startupMock.Verify(x => x.InitializeLibrary(It.IsAny<string>(),It.IsAny<string>(),""));
        loginServiceMock.Verify(x => x.Login());
        loginServiceMock.Verify(x => x.ExchangeAuthCodeForToken(code));
        loginServiceMock.Verify(x => x.IsCurrentUserLoggedIn());
        loginWebviewMock.Verify(x => x.DisplayLogin(It.IsAny<string>(), It.IsAny<Action<string>>()));
        Assert.AreEqual(PlayerPrefs.GetString("Token"), newJwt);
        Assert.IsTrue(loggedInAsUser);
    }
    
    [Test]
    public void TestFirstLogin_SetsFirstToken()
    {
        //Given a client never logged in.
        //when that client logs in as a guest.
        //Then the new token is updated in the playerprefs, and FirstToken is created with the same value.
        
        PlayerPrefs.DeleteKey("FirstToken");
        var loginWebviewMock = new Mock<ILoginWebview>();
        var startupMock = new Mock<IStartup>();
        var loginServiceMock = new Mock<ILoginService>();
        PlayerPrefs.SetString("Token", "");
        loginServiceMock.Setup(x => x.Login()).ReturnsAsync("GuestToken").Verifiable();
        loginServiceMock.Setup(x => x.IsCurrentUserLoggedIn()).ReturnsAsync(false).Verifiable();
        startupMock.Setup(x => x.InitializeLibrary(It.IsAny<string>(),It.IsAny<string>(),"")).Returns(loginServiceMock.Object).Verifiable();
        var loggedInAsGuest = false;
        LoginHandler sut = new LoginHandler(loginWebviewMock.Object, startupMock.Object, (loginActions) =>
        {
            if (loginActions == LoginEvents.loggedInAsGuest)
            {
                loggedInAsGuest = true;
            }
            else
            {
                loggedInAsGuest = false;
            }
        });

        startupMock.Verify(x => x.InitializeLibrary(It.IsAny<string>(), It.IsAny<string>(), ""));
        loginServiceMock.Verify(x => x.Login());
        loginServiceMock.Verify(x => x.IsCurrentUserLoggedIn());
        var tokenStored = PlayerPrefs.GetString("Token");
        var firstTokenStored = PlayerPrefs.GetString("FirstToken");
        Assert.AreEqual("GuestToken",tokenStored);
        Assert.AreEqual(tokenStored, firstTokenStored);
        Assert.IsTrue(loggedInAsGuest);
    }
    
    [Test]
    public void TestSecondLogin_Does_NOT_SetFirstToken()
    {
        //Given a client that's been logged in as a guest
        //when that client logs in as a user.
        //Then the new token is updated in the playerprefs, but FirstToken is unchanged.
        
        //Arrange
        PlayerPrefs.SetString("FirstToken", "GuestToken");
        PlayerPrefs.SetString("Token", "GuestToken");
        var loginWebviewMock = new Mock<ILoginWebview>();
        var startupMock = new Mock<IStartup>();
        var loginServiceMock = new Mock<ILoginService>();

        loginServiceMock.Setup(x => x.Login()).ReturnsAsync("UserToken").Verifiable();
        loginServiceMock.Setup(x => x.IsCurrentUserLoggedIn()).ReturnsAsync(true).Verifiable();
        startupMock.Setup(x => 
            x.InitializeLibrary(
                It.IsAny<string>(),
                It.IsAny<string>(),
                "GuestToken"))
            .Returns(loginServiceMock.Object).Verifiable();
        
        var loggedInAsGuest = false;
        
        //Act
        LoginHandler sut = new LoginHandler(loginWebviewMock.Object, startupMock.Object, (loginActions) =>
        {
            if (loginActions == LoginEvents.loggedInAsGuest)
            {
                loggedInAsGuest = true;
            }
            else
            {
                loggedInAsGuest = false;
            }
        });

        //Assert
        startupMock.Verify(x => 
            x.InitializeLibrary(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                "GuestToken"));
        
        loginServiceMock.Verify(x => x.Login());
        loginServiceMock.Verify(x => x.IsCurrentUserLoggedIn());
        
        var tokenStored = PlayerPrefs.GetString("Token");
        var firstTokenStored = PlayerPrefs.GetString("FirstToken");
        
        Assert.AreEqual("UserToken",tokenStored);
        Assert.AreEqual("GuestToken", firstTokenStored);
        
        Assert.IsFalse(loggedInAsGuest);
    }
    
    [Test]
    public void TestLogout_Restores_FirstToken()
    {
        //Given a client that's been logged in as a user
        //when that client logs out
        //Then the token is updated with the value of the FirstToken slot.
        
        //Arrange
        var firstToken = "GuestToken";
        PlayerPrefs.SetString("FirstToken", firstToken);
        PlayerPrefs.SetString("Token", "UserToken");
        
        var preLogoutTokenStored = PlayerPrefs.GetString("Token");
        
        var loginWebviewMock = new Mock<ILoginWebview>();
        var startupMock = new Mock<IStartup>();
        var loginServiceMock = new Mock<ILoginService>();
        
        loginServiceMock.Setup(x => x.Login()).ReturnsAsync("UserToken").Verifiable(); //first call in constructor.
        loginServiceMock.Setup(x => x.Login()).ReturnsAsync("GuestToken").Verifiable(); //second call after logout.
        loginServiceMock.Setup(x => x.IsCurrentUserLoggedIn()).ReturnsAsync(true).Verifiable();
        
        loginServiceMock.Setup(x => x.LogoutAction(firstToken)).Verifiable();
        
        startupMock.Setup(x => 
                x.InitializeLibrary(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    "UserToken"))
            .Returns(loginServiceMock.Object);
        
        LoginHandler sut = new LoginHandler(loginWebviewMock.Object, startupMock.Object, (loginActions) => { });
        
        //Act
        sut.Logout();
        
        //Assert
        loginServiceMock.Verify(x => x.LogoutAction(firstToken));
 
        var postLogoutTokenStored = PlayerPrefs.GetString("Token");
        var firstTokenStored = PlayerPrefs.GetString("FirstToken");
        
        Assert.AreEqual(firstTokenStored, postLogoutTokenStored);
        Assert.AreNotEqual(preLogoutTokenStored, postLogoutTokenStored);
    }
}

