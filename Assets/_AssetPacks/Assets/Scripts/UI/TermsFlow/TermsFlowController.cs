using System;
using System.Collections;
using System.Collections.Generic;
using Riddlehouse.Core.Helpers.Helpers;
using riddlehouse_libraries.Terms;
using UnityEngine;
using UnityEngine.Serialization;

public interface ITermsFlowControllerActions
{
    public void Configure(TermsFlowController.Config config);
    public void Open();
    public void Close(Action animationCompleted = null);
    public void Dispose();
}
public class TermsFlowController : MonoBehaviour, ITermsFlowControllerActions
{
    public class Config
    {
        public List<TermsInfo> PendingAgreements;
        public Action<bool> FlowComplete;
    }

    public class Dependencies
    {
        public ITermsService _termsService;
        public IImageFaderActions _fadeController;
        public ITermsAcceptanceController _acceptanceController;
        public ITermsWebviewController _webviewController;
    }
    
    public static ITermsFlowControllerActions Factory(TermsFlowController behaviour)
    {
        return new ComponentHelper<TermsFlowController>().GetBehaviourIfExists(behaviour.gameObject);
    }
    
    [FormerlySerializedAs("serializedAgreementAcceptanceController")] [SerializeField] private TermsAcceptanceController serializedTermsAcceptanceController;
    [FormerlySerializedAs("serializedAgreementWebViewController")] [SerializeField] private TermsWebViewController serializedTermsWebViewController;
    [SerializeField] private ImageFader serializedFadeController;
    private List<TermsInfo> _pendingAgreements;
    private Action<bool> _flowComplete;
    private WaitForEndOfFrame frameWaiter;

    private ITermsService TermsService { get; set; }
    private ITermsAcceptanceController AcceptanceView  { get; set; }
    private ITermsWebviewController TermsWebview { get; set; }
    private IImageFaderActions FadeController { get; set; }

    public void Awake()
    {
        SetDependencies(new Dependencies()
        {
            _termsService = new TermsService(),
            _fadeController = serializedFadeController,
            _acceptanceController = serializedTermsAcceptanceController,
            _webviewController = serializedTermsWebViewController
        });
        frameWaiter = new WaitForEndOfFrame();
    }
    public void SetDependencies(Dependencies dependencies)
    {
        TermsService = dependencies._termsService;
        FadeController = dependencies._fadeController;
        AcceptanceView = dependencies._acceptanceController;
        TermsWebview = dependencies._webviewController;
    }

    public void Configure(Config config)
    {
        _pendingAgreements = config.PendingAgreements;
        _flowComplete = config.FlowComplete;
        ConfigureForAgreement();
    }

    private void ConfigureForAgreement()
    {
        var idx = FindNextAgreement();
        if (idx != -1)
        {
            var info = _pendingAgreements[idx];
            var config = new TermsAcceptanceController.Config()
            {
                AcceptAgreementEvent = () => AcceptActiveAgreement(idx),
                DeclineAgreementEvent = () => DeclineActiveAgreement(idx),
                ReadAgreementEvent = TransitionToWebView,
                Title = info.Title,
                Reason = info.Reason
            };
            var webviewConfig = new TermsWebViewController.Config()
            {
                ReturnEvent = () => TransitionToAcceptanceScreen(),
                Url = info.Url
            };
            AcceptanceView.Configure(config);
            TermsWebview.Configure(webviewConfig);
            Open();
        }
        else
        {
            _flowComplete.Invoke(true);
        }
    }
    
    private int FindNextAgreement()
    {
        return _pendingAgreements.FindIndex(x => !x.HasBeenAnswered);
    }

    private void AcceptActiveAgreement(int idx)
    {
        try
        {
            TermsService.AcceptPendingTerms(_pendingAgreements[idx].Id);
            Close(() =>
            {
                _pendingAgreements[idx].Accept();
                ConfigureForAgreement();
            });
        }
        catch
        {
            Debug.LogError("error while accepting pending.");
        }
    }

    private void DeclineActiveAgreement(int idx)
    {
        Close(() =>
        {
            _pendingAgreements[idx].Decline();
            _flowComplete.Invoke(false);
        });
    }

    private void TransitionToWebView()
    {
        AcceptanceView.Close(TermsWebview.Open);
    }

    private void TransitionToAcceptanceScreen()
    {
        TermsWebview.Close(() => AcceptanceView.Open());
    }

    private bool IsAnimating()
    {
        return FadeController.IsAnimating();
    }
    
    public void Open()
    {
        if (!IsAnimating())
        {
            FadeController.Open();
            AcceptanceView.Open();
        }
    }

    public void Close(Action animationCompleted = null)
    {
        if(FadeController.IsOpen() && !FadeController.IsAnimating())
            FadeController.Close();
        
        if(AcceptanceView.IsOpen() && !AcceptanceView.IsAnimating())
            AcceptanceView.Close();
        
        if(TermsWebview.IsOpen() && !TermsWebview.IsAnimating())
            TermsWebview.Close();
        
        if(!coroutineRunning)
            StartCoroutine(WaitForViewToClose(animationCompleted));
    }

    public void Dispose()
    {
        Close(() => Destroy(this.gameObject));
    }

    private bool coroutineRunning = false;
    private IEnumerator WaitForViewToClose(Action animationCompleted)
    {
        coroutineRunning = true;
        while (FadeController.IsOpen() || AcceptanceView.IsOpen() || TermsWebview.IsOpen())
        {
            yield return frameWaiter;
        }
        coroutineRunning = false;
        animationCompleted.Invoke();
    }
}
