using System;
using UnityEngine;
using Zenject;

public interface ILoginBtnActions
{
    public void Login();
}
public class LoginBtn : MonoBehaviour, ILoginBtnActions
{
    [Inject] private LoginHandler _loginHandler;
    [Inject] private CanvasLayerManager clm;
    public void Start()
    {
        if (_loginHandler != null)
        {
            _loginHandler.ServiceUpdated += ReactToUserLogin;
            if (!_loginHandler.IsLoggedInAsUser())
            {
                this.gameObject.SetActive(true);
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
    }
    public void OnDestroy()
    {
        if (_loginHandler != null)
        {
            _loginHandler.ServiceUpdated -= ReactToUserLogin;
        }
    }

    public void ReactToUserLogin(LoginEvents _event)
    {
        switch (_event)
        {
            case LoginEvents.loggedInAsGuest:
                this.gameObject.SetActive(true);
                break;
            case LoginEvents.loggedInAsUser:
                this.gameObject.SetActive(false);
                break;
        }
    }

    public void Login()
    {
        if (_loginHandler != null)
        {
            if (clm != null)
            {
                var videoCanvas = clm.GetVideoCanvas();
                if (videoCanvas != null)
                {
                 videoCanvas.GetVideoController().Stop();   
                }
            }
            _loginHandler.LoginAction();
        }
    }
}
