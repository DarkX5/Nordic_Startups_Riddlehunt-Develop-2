using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Riddlehouse.Core.Helpers.Helpers;
using riddlehouse_libraries.products;
using riddlehouse_libraries.products.models;
using UnityEngine;
using riddlehouse_libraries.analytics;
using riddlehouse_libraries.products.AssetTypes;
using TMPro;
using Zenject;
public interface IHuntView
{
    public Task Configure(StartPanelData startPanelData, IHuntAssetGetter assetGetter, IGetProductFlowData flowGetter);
    public void EndProduct(bool completed);
}
public interface ITimerobberProductActions
{
    public Task Configure(StartPanelData startPanelData, IHuntAssetGetter assetGetter, IGetProductFlowData flowGetter);
}

[RequireComponent(typeof(QuestionnaireWebview))]
public class TimerobberProductBehaviour : MonoBehaviour, ITimerobberProductActions, IHuntView
{
    public static ITimerobberProductActions Factory(TimerobberProductBehaviour go, IProductEvents events, Camera viewCamera)
    {
        var huntViewBehaviour = Instantiate(go);
        huntViewBehaviour.transform.SetSiblingIndex(0);
        huntViewBehaviour.SetDependencies(null, new UIFitters(), viewCamera, events);
        return huntViewBehaviour;
    }

    private ChristmasChristmasHuntController _christmasChristmasHuntController;
    [SerializeField] private Canvas viewCanvasComponent;
    [Inject] private LoginHandler _loginHandler;
    [Inject] private CanvasLayerManager _clm;
    
    [SerializeField] private HuntHomeComponentBehaviour huntHomeComponentPrefab;
    [SerializeField] private EndHuntComponentBehaviour endScreenPanelPrefab;
    [SerializeField] private RatingComponentBehaviour ratingComponentPrefab;
    [SerializeField] private VideoBasedRiddleViewController videoBasedRiddleViewControllerPrefab;

    [SerializeField] private ProductIntroView IntroViewPrefab;
    public IHuntHomeComponent HomeComponent{ get; private set; }
    public IEndHuntComponent EndHuntComponent{ get; private set; }
    public IRatingComponent RatingComponent { get; private set; }
    public IProductIntroView IntroView { get; private set; }
    public VideoBasedRiddleViewController _videoBasedRiddleViewController { get; private set; }

    private IProductEvents _events;

    public void Start()
    {
        if (_loginHandler != null)
            _loginHandler.ServiceUpdated += ReconfigureHuntHomeViewOnUserLogin;
    }

    public void OnDestroy()
    {
        if(_loginHandler != null)
            _loginHandler.ServiceUpdated -= ReconfigureHuntHomeViewOnUserLogin;
    }

    public void SetDependencies(
        RectTransform parent, 
        IUIFitters uiFitters, 
        Camera viewCamera, 
        IProductEvents events, 
        IProductIntroView introView = null)
    {
        _events = events;
        FitInView(parent, uiFitters, viewCamera);
        
        //new steplist
        //new step view
        //end screen
        //rating component
        
        CreateHomeView();
        CreateVideoBasedViewController();
        CreateEndView();
        CreateRatingView();

        if (introView == null)
            IntroView = ProductIntroView.Factory(IntroViewPrefab, transform);
        else
            IntroView = introView;
    }
    private void FitInView(RectTransform parent, IUIFitters uiFitters, Camera viewCamera)
    {
        if(viewCanvasComponent!=null) {
            viewCanvasComponent.renderMode = RenderMode.ScreenSpaceCamera;
            viewCanvasComponent.worldCamera = viewCamera;
        }
        uiFitters.FitToGlobalView((RectTransform)this.transform);
    }

    private string huntID = "";
    private IHuntAssetGetter _huntAssetGetter = null;
    private IGetProductFlowData _flowGetter = null;
    private StartPanelData _startPanelData;
    public async Task Configure(StartPanelData startPanelData, IHuntAssetGetter assetGetter, IGetProductFlowData flowGetter)
    {
        _startPanelData = startPanelData;
        huntID = startPanelData.Id;
        
        if (assetGetter == null)
            assetGetter = HuntAssetGetter.Factory(this);
        
        _huntAssetGetter = assetGetter;
        _flowGetter = flowGetter;
        
        IntroView.Display();
        IntroView.Configure(new ProductIntroView.Config()
        {
            VideoUrl = startPanelData.VideoUrl,
            Url = startPanelData.InfoPopUp.InfoUrl,
            BackAction = IntroViewBackAction,
            ContinueAction = IntroViewContinueAction
        });

        // HomeComponent.GetComponentUIActions().Display();
        // ConfigureHomeComponent(startPanelData, _huntAssetGetter, _flowGetter);
    }

    private void IntroViewBackAction()
    {
        IntroView.Hide();
        EndProduct(false);
    }

    private async void IntroViewContinueAction()
    {
        IntroView.Hide();
        HomeComponent.GetComponentUIActions().Display();
        ProductService productService = new ProductService(DataPathHelper.PersistentDataPath);
        _startPanelData = await productService.GetStartPanelData(huntID);
        ConfigureHomeComponent(_startPanelData, _huntAssetGetter, _flowGetter);
    }
    
    private bool attemptedConfigure = false;
    private async void ReconfigureHuntHomeViewOnUserLogin(LoginEvents loginEvents)
    {
        // if (loginEvents == LoginEvents.loggedInAsUser && !_attemptedReconfigure)
        // {
        //     _attemptedReconfigure = true;
        //     ProductService productService = new ProductService();
        //     var startPanelData = await productService.GetStartPanelData(huntID);
        //     ConfigureHomeComponent(startPanelData, _huntAssetGetter, _flowGetter);
        // } else if (loginEvents == LoginEvents.loggedInAsGuest && _attemptedReconfigure)
        // {
        //     GoBackToProductList(false);
       //}
    }

    private void ConfigureHomeComponent(StartPanelData startPanelData, IHuntAssetGetter assetGetter,
        IGetProductFlowData flowGetter)
    {
        HomeComponent.Configure(startPanelData, async (introVidepPrepared) =>
            {
                if (!attemptedConfigure)
                {
                    if (startPanelData.HasAccess)
                    {
                        attemptedConfigure = true;
                        var huntFlow = await flowGetter.GetProductFlow(startPanelData.Id);

                        assetGetter.GetHuntAssets(huntFlow, startPanelData.Id, startPanelData.FeedbackLink, (data) =>
                        {
                            Dictionary<StepType, IOldStepController> stepControllers =
                                new Dictionary<StepType, IOldStepController>();
                            stepControllers.Add(StepType.MultipleAnswersVideoInAndOut,
                                new MultipleAnswersVideoInAndOutOldStepController(_videoBasedRiddleViewController,
                                    () =>
                                    {
                                        _videoBasedRiddleViewController.gameObject.SetActive(false);
                                        HomeComponent.GetComponentUIActions().Display();
                                    }));
                            stepControllers.Add(StepType.ResolutionVideoAndEnd,
                                new ResolutionVideoAndEndOldStepController(EndHuntComponent));

                            _christmasChristmasHuntController = new ChristmasChristmasHuntController();
                            _christmasChristmasHuntController.ConfigureHunt(data, stepControllers,
                                () => { InitRatingComponent(startPanelData.FeedbackLink, startPanelData.Id); });
                            HomeComponent.HuntReady(_christmasChristmasHuntController, new ConditionalStepList(),
                                new List<IConditionalStepBtn>());
                        });
                    }
                }
            }, EndProduct);
        }

    private void InitRatingComponent(string feedbackLink, string productID)
    {
        new Analytics().ProductEnd(productID);
        Action<int> BtnAction = (rating) =>
        {
            RatingComponent.GetComponentUIActions().Hide();
            EndProduct(true);
            if (rating != -1)
            {
                new Analytics().ProductFeedback(productID, rating);
            }

            if (!string.IsNullOrEmpty(feedbackLink))
            {
                GetComponent<QuestionnaireWebview>().DisplayQuestionnaire(feedbackLink);
            }
        };
        
        RatingComponent.Configure(BtnAction);
        RatingComponent.GetComponentUIActions().Display();
    }

    public void EndProduct(bool completed)
    {
        if (_clm != null)
        {
            _clm.GetLoaderView().Hide();
            var videoController = _clm.GetVideoCanvas();
            if (videoController != null)
            {
                videoController.GetVideoController().Stop();
            }
        }
        if(_christmasChristmasHuntController != null)
            _christmasChristmasHuntController.EndHunt(completed);
        
        _clm.SetLayerInteractable(CanvasLayerTypeNames.home);
        
        Destroy(this.gameObject);
    }
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    private void CreateHomeView()
    {
        var homeComponentGo = Instantiate(huntHomeComponentPrefab.gameObject);
        HomeComponent = HuntHomeComponent.Factory(homeComponentGo);
        HomeComponent.GetComponentUIActions().FitInView((RectTransform)this.transform, new UIFitters());
        homeComponentGo.transform.SetSiblingIndex(0);
    }

    private void CreateVideoBasedViewController()
    {
        var videoBasedRiddleViewController = Instantiate(videoBasedRiddleViewControllerPrefab, this.transform);
        videoBasedRiddleViewController.transform.SetSiblingIndex(5);
        _videoBasedRiddleViewController = videoBasedRiddleViewController;
    }
    
    private void CreateEndView()
    {
        var endScreenComponentGo = Instantiate(endScreenPanelPrefab.gameObject);
        EndHuntComponent = global::EndHuntComponent.Factory(endScreenComponentGo);
        EndHuntComponent.GetComponentUIActions().FitInView((RectTransform)this.transform, new UIFitters());
    }
    
    private void CreateRatingView()
    {
        var ratingComponentGo = Instantiate(ratingComponentPrefab.gameObject);
        RatingComponent = global::RatingComponent.Factory(ratingComponentGo);
        RatingComponent.GetComponentUIActions().FitInView((RectTransform)this.transform, new UIFitters());
        ratingComponentGo.SetActive(false);
    }
}
