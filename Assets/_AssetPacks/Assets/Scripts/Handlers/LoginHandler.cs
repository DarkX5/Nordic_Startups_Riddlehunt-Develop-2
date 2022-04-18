using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using UnityEngine;
using riddlehouse_libraries.login;
using riddlehouse_libraries;
using UnityEngine.SceneManagement;
using Zenject;

public enum LoginEvents {none, loggedInAsGuest, loggedInAsUser}
public interface ILoginHandler
{
    public void CheckIsUserLoggedIn();
    public void UnpackCallback(string callback);
    public void Logout(string fallbackToken = "");
    public bool IsLoggedInAsUser();
    public void LoginAction();
}
public class LoginHandler : ILoginHandler
{
    private ILoginService _loginService;

    public Action<LoginEvents> ServiceUpdated; //update

    private readonly IStartup _startup;

    private readonly ILoginWebview _loginWebView;
    public LoginHandler(ILoginWebview loginWebView, IStartup startup = null, System.Action<LoginEvents> serviceUpdatedOnStartup = null)
    {
        _loginWebView = loginWebView;
        
        if (startup == null)
            startup = new Startup();
        
        // #if UNITY_EDITOR
        startup.SubscribeToDebug((msg) => { Debug.Log(msg); });
        // #endif

        _startup = startup;

        if (serviceUpdatedOnStartup == null)
        {
            ServiceUpdated = (loginActions) => { };
        }
        else
        {
            ServiceUpdated = serviceUpdatedOnStartup;
        }

        var oldJwt = GetJWTFromPlayerPrefs();
        //update subscription.
        _loginService = _startup.InitializeLibrary(Application.identifier, Application.version, oldJwt);
        SilentLogin();
    }

    private string GetJWTFromPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("Token"))
        {
            var token = PlayerPrefs.GetString("Token");
            return token;
        }
        return "";
    }

    public async void SilentLogin()
    {
        UpdateToken(await _loginService.Login());
    }

    public async void LoginAction()
    {
        var url = await _loginService.LoginAction();
        _loginWebView.DisplayLogin(url, UnpackCallback);
    }

    public async void UnpackCallback(string auth0callbackMessage)
    {
        // const string toBeSearched = "code=";
        // var code = callback.Substring(callback.IndexOf(toBeSearched, StringComparison.Ordinal) + toBeSearched.Length);
        var longLived = await _loginService.ExchangeAuthCodeForToken(auth0callbackMessage);
        UpdateToken(longLived);
    }

    private void UpdateToken(string token)
    {
        Debug.Log("updating token: "+token);
        PlayerPrefs.SetString("Token", token);
        SetFirstToken(token);
        CheckIsUserLoggedIn();
    }

    public void Logout(string fallbackToken = "")
    {
        RestoreJwtToFirstToken();
        var token = PlayerPrefs.GetString("Token");
        if (!string.IsNullOrEmpty(fallbackToken))
            token = fallbackToken;
        _loginService.LogoutAction(token);
        SilentLogin();
    }

    private void SetFirstToken(string token)
    {
        if (PlayerPrefs.HasKey("FirstToken"))
            return;
        PlayerPrefs.SetString("FirstToken", token);
    }
    private void RestoreJwtToFirstToken()
    {
        if(PlayerPrefs.HasKey("Token"))
            PlayerPrefs.DeleteKey("Token");
        if(PlayerPrefs.HasKey("FirstToken"))
            PlayerPrefs.SetString("Token", PlayerPrefs.GetString("FirstToken"));
        else 
            PlayerPrefs.SetString("Token", "");
    }

    public async void CheckIsUserLoggedIn()
    {
        var isLoggedIn = await _loginService.IsCurrentUserLoggedIn();
        if (isLoggedIn)
        {
            Debug.Log("logged in as user");
            _loggedInState = LoginEvents.loggedInAsUser;
            ServiceUpdated.Invoke(LoginEvents.loggedInAsUser);
        }
        else
        {
            Debug.Log("logged in as guest");
            _loggedInState = LoginEvents.loggedInAsGuest;
            ServiceUpdated.Invoke(LoginEvents.loggedInAsGuest);
        }
    }
    private LoginEvents _loggedInState = LoginEvents.none;
    public bool IsLoggedInAsUser()
    {
        switch (_loggedInState)
        {
            case LoginEvents.loggedInAsUser:
                return true;
            case LoginEvents.none:
            case LoginEvents.loggedInAsGuest:
            default: return false;
        }
    }
}