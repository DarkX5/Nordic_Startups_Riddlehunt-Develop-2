using System;
using System.Collections.Generic;
using Hunt;
using Riddlehouse.Core.Helpers.Helpers;
using riddlehouse_libraries.products.models;
using riddlehouse_libraries.Terms;
using riddlehouse_libraries.Terms.models;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(CanvasController))]
public class MainController : MonoBehaviour
{
    public static MainController Factory(MainController prefab, Camera camera)
    {
        var controller = Instantiate(prefab);
        controller.Initialize(camera);
        return controller;
    }

    public class Dependencies
    {
        public ILoadingView StartupPanel { get; set; }
        public LoginHandler LoginHandler { get; set; }
        public GameObject LoginForeground { get; set; }
        public IProductList ProductList { get; set; }
        public ICanvasController CanvasController { get; set; }
        public ITermsService TermsService { get; set; }
    }
    
    public void Initialize(Camera viewCamera)
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        var cvController = GetComponent<CanvasController>();
        cvController.Initialize();
        
        SetDependencies(new Dependencies()
        {
            StartupPanel = _loadingView,
            LoginHandler = _loginHandler,
            LoginForeground = LoginForeground,
            CanvasController = cvController,
            TermsService = new TermsService()
        });
        _dependencies.CanvasController.Configure(new CanvasController.Config()
        {
            ViewCamera = viewCamera
        });
        
        _dependencies.LoginForeground.SetActive(true);
        
        _dependencies.LoginForeground.transform.SetSiblingIndex(1);
        _dependencies.StartupPanel.FitInView((RectTransform)this.transform, new UIFitters(), 2);
        OpenLoading();
    }
    [Inject] public LoginHandler _loginHandler { get; private set; }
    [Inject] public ILoadingView _loadingView { get; private set; }
    [SerializeField] private TermsFlowController termsControllerPrefab;
    private ITermsFlowControllerActions termsFlowController;
    [SerializeField] private ProductListBehaviour productListPanelPrefab;
    [SerializeField] GameObject LoginForeground;

    public Dependencies _dependencies { get; private set; }
    public void SetDependencies(Dependencies dependencies)
    {
        _dependencies = dependencies;
    }

    public void UpdateDependencies(IProductList productList)
    {
        _dependencies.ProductList = productList;
    }
    
    private void CreateAndInitializeProductList()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortraitUpsideDown = false;

        var tmpProductList = ProductListBehaviour.Factory(productListPanelPrefab, (RectTransform)this.transform);
        tmpProductList.SetSiblingIndex(0);
        
        if(_dependencies.ProductList != null)
            _dependencies.ProductList.DestroyGameObject();
        UpdateDependencies(tmpProductList);
    }
    
    public void Start()
    {
        _dependencies.LoginHandler.ServiceUpdated += LoggedInAction;
    }

    public void OpenLoading()
    {
        _dependencies.StartupPanel.Display();
    }

    public void CloseLoading()
    {
        _dependencies.StartupPanel.Hide();
    }
    
    private LoginEvents _loggedInState = LoginEvents.none;
    private void LoggedInAction(LoginEvents loginEvent)
    {
        var previousLoggedInState = _loggedInState;
        OpenLoading();
        switch (loginEvent)
        {
            case LoginEvents.loggedInAsGuest:
                _loggedInState = LoginEvents.loggedInAsGuest;
                _dependencies.LoginForeground.SetActive(true);
                break;
            case LoginEvents.loggedInAsUser:
                _loggedInState = LoginEvents.loggedInAsUser;
                _dependencies.LoginForeground.SetActive(false);
                CheckAndConfigureTerms();
                break;
        }

        if (_loggedInState != previousLoggedInState)
        {
            CreateAndInitializeProductList();
            _dependencies.ProductList.Configure(CloseLoading);
        }
}

    private async void CheckAndConfigureTerms()
    {
        
        var baseTerms = await _dependencies.TermsService.GetPendingTerms();
        CloseLoading();
        if (baseTerms.Count > 0)
        {
            var pendingTerms = new List<TermsInfo>();
            foreach (var info in baseTerms)
            {
                pendingTerms.Add(new TermsInfo(info));
            }
            var termsControllerobj = Instantiate(termsControllerPrefab.gameObject).GetComponent<TermsFlowController>();
            termsFlowController = TermsFlowController.Factory(termsControllerobj);
            termsFlowController.Configure(new TermsFlowController.Config()
            {
                PendingAgreements = pendingTerms,
                FlowComplete = (allTermsAccepted) =>
                {
                    if (!allTermsAccepted)
                    {
                        _loginHandler.Logout();
                    }
                    termsFlowController.Dispose();
                }
            });
        }
    }
}
