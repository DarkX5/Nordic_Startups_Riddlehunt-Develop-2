using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataPathHelper
{
    public static string PersistentDataPath
    {
        get
        {
            if(Application.platform == RuntimePlatform.Android)
                return  GetPersistantDataPath_Android();
            if(Application.platform == RuntimePlatform.IPhonePlayer)
                return GetPersistantDataPath_Ios();
            return Application.persistentDataPath;
        }
    }

    public static string GetPersistantDataPath_Android()
    {
        return "file:/"+Application.persistentDataPath;
    }
    public static string GetPersistantDataPath_Ios()
    { 
        return Application.persistentDataPath;
    }
}
