using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using riddlehouse_libraries.Terms;
using riddlehouse_libraries.Terms.models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TermsInfo : BaseTermsInfo
{
    public TermsInfo() {}

    public TermsInfo(BaseTermsInfo info)
    {
        Id = info.Id;
        Title = info.Title;
        Reason = info.Reason;
        Url = info.Url;
    }
    public bool HasBeenAccepted { get; private set; } = false;
    public bool HasBeenAnswered { get; private set; } = false;
    public void Accept()
    {
        HasBeenAccepted = true;
        HasBeenAnswered = true;
    }

    public void Decline()
    {
        HasBeenAccepted = false;
        HasBeenAnswered = true;
    }
}

public class TermsDemo : MonoBehaviour
{
    [SerializeField] private TermsFlowController controller;
    private LoginHandler _loginHandler;
    private TermsService _termsService;
    public void Awake()
    {
        _loginHandler = new LoginHandler(null);
        _termsService = new TermsService();
    }
    public void StartDemo()
    {
        var pendingTerms = new List<TermsInfo>()
        {
            new TermsInfo()
            {
                Id = "jeepers",
                Title = "Terms to Accept",
                Reason = "This is a one line description.",
                Url = "https://google.com"
            }
        };

        controller.gameObject.SetActive(true);
        controller.Configure(
            new TermsFlowController.Config()
            {
                PendingAgreements = pendingTerms, 
                FlowComplete = FlowComplete
            });
        this.gameObject.SetActive(false);
    }
    
    public void FlowComplete(bool allAccepted)
    {
        if (!allAccepted)
        {
            _loginHandler.Logout();
        }
        else
        {
            controller.Close(() => Debug.Log("flow completed"));
        }
    }
}
