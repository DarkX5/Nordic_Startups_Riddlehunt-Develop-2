using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
public class LoginMenuExtension : MonoBehaviour
{
    [MenuItem("Riddlehouse/Authorization/Login")]
    static void Auto_Login()
    {
        var token = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "localJwt.txt"));
        PlayerPrefs.SetString("Token",token);
    }

    [MenuItem("Riddlehouse/Testing/LoginAsUser")]
    static void LoginAsUser()
    {
        var token = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "localJwt.txt"));
        PlayerPrefs.SetString("Token",token);
        var main = GameObject.FindWithTag("UITop");
        if(main != null)
            main.GetComponent<MainController>()._loginHandler.Logout(token);
    }
    
    [MenuItem("Riddlehouse/Testing/LoginAsGuest")]
    static void LoginAsGuest()
    {
        PlayerPrefs.SetString("Token","");
        var main = GameObject.FindWithTag("UITop");
        if(main != null)
            main.GetComponent<MainController>()._loginHandler.Logout();
    }
    
    [MenuItem("Riddlehouse/Authorization/Empty PlayerPrefs")]
    static void RemovePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
