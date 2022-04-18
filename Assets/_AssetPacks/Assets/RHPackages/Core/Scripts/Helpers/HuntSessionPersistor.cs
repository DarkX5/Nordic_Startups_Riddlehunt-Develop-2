using System.Collections;
using System.Collections.Generic;
using TestRiddlehouseLibraries.LibraryTests.Products.Assets;
using UnityEngine;

public class HuntSessionPersistor : IHuntSessionPersistor
{
    public void SetStringAnswer(string identifier, string value)
    {
        // Debug.Log("Setting string: "+_identifier + "value: "+value);
        PlayerPrefs.SetString(identifier, value);
    }

    public void SetNumericAnswer(string identifier, float value)
    {
        // Debug.Log("Setting string: "+_identifier + "value: "+value);
        PlayerPrefs.SetFloat(identifier, value);
    }

    public void SetMultipleChoiceAnswerIconsString(string identifier, string value)
    {
        // Debug.Log("Setting string: "+_identifier + "value: "+value);
        PlayerPrefs.SetString(identifier, value);
    }

    public void TickBooleanAnswer(string identifier)
    {
        // Debug.Log("Setting string: "+_identifier);
        PlayerPrefs.SetInt(identifier, 0);
    }
    
    public string GetStringAnswer(string identifier)
    {
        var answer = PlayerPrefs.GetString(identifier);
        // Debug.Log("Found string answer in identifer: "+_identifier);
        // Debug.Log(answer);
        return answer;
    }

    public float GetNumericAnswer(string identifier)
    {
        var answer = PlayerPrefs.GetFloat(identifier);
        // Debug.Log("Found numeric answer in identifer: "+_identifier);
        // Debug.Log(answer);
        return answer;
    }

    public string GetMultipleChoiceAnswerIconsString(string identifier)
    {
        var answer = PlayerPrefs.GetString(identifier);
        // Debug.Log("Found MCA_Icon answer in identifer: "+_identifier);
        // Debug.Log(answer);
        return answer;
    }

    public bool GetBooleanAnswer(string identifier)
    {
        var answer = PlayerPrefs.HasKey(identifier);
        // Debug.Log("Found Boolean answer in identifer: "+_identifier);
        // Debug.Log(answer);
        return answer;
    }

    public bool HasAnswerInSession(string identifier)
    {
        var hasAnswer = PlayerPrefs.HasKey(identifier);
        // Debug.Log("Found answer in identifer: "+_identifier);
        // Debug.Log(hasAnswer);
        return hasAnswer;
    }
    public void ClearAnswerInSession(string identifier)
    {
        PlayerPrefs.DeleteKey(identifier);
    }
}
