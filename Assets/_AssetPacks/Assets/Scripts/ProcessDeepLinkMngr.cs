using System;
using UnityEngine;
using Zenject;

public class ProcessDeepLinkMngr : IInitializable
{
    public static ProcessDeepLinkMngr Instance { get; private set; }
    [Inject] private LoginHandler loginHandler;
    string deeplinkURL;
    public void Initialize()
    {
        if (Instance == null)
        {
            Instance = this;                
            Application.deepLinkActivated += onDeepLinkActivated;
            if (!String.IsNullOrEmpty(Application.absoluteURL))
            {
                // Cold start and Application.absoluteURL not null so process Deep Link.
                onDeepLinkActivated(Application.absoluteURL);
            }
            // Initialize DeepLink Manager global variable.
            else deeplinkURL = "[none]";
        }
    }

    private void onDeepLinkActivated(string url)
    {
        // Update DeepLink Manager global variable, so URL can be accessed from anywhere.
        deeplinkURL = url;

        // Decode the URL to determine action. 
        // In this example, the app expects a link formatted like this:
        // unitydl://auth0callback?code=12345
        string address = url.Split("?"[0])[0];
        if (address == "unitydl://auth0callback")
        {
            loginHandler.UnpackCallback(url);
        }
    }
}

