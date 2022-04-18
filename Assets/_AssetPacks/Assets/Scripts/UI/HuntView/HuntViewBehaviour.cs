using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Riddlehouse.Core.Helpers.Helpers;
using riddlehouse_libraries.analytics;
using riddlehouse_libraries.products;
using riddlehouse_libraries.products.models;
using UnityEngine;
using UnityEngine.Serialization;

// public interface IHuntView
// {
//     public void Configure(StartPanelData startPanelData);
//     public void GoBackToProductList(bool completed);
//     public HuntController GetHuntController();
// }
public enum ViewType {startup, productList, huntView}
public interface IViewUIActions
{
    public void FitInView(RectTransform parent, IUIFitters uiFitters, Camera viewCamera);
    public void Display();
    public void Hide();
    public bool IsShown();
    public ViewType GetViewType();
}

public class HuntView //:IHuntView
{
   //  public static IHuntView Factory(GameObject go, IProductEvents events, Camera viewCamera)
   //  {
   //      var huntViewBehaviour = new ComponentHelper<HuntViewBehaviour>().GetBehaviourIfExists(go);
   //      huntViewBehaviour.FitInView(null, new UIFitters(), viewCamera);
   //      var component = new HuntView(huntViewBehaviour, huntViewBehaviour, events, new HuntController());
   //      huntViewBehaviour.SetLogicInstance(component);
   //      return component;
   //
   //  }
   //  
   //  private IHuntViewActions _huntViewActions;
   //  private IViewUIActions _viewUIActions;
   //  private IProductEvents _events;
   //  private IHuntController _huntController;
   //  private GameObject productlist;
   // // private IStoryComponent _storyComponent; //should have a list of IViewUIActions; populated by the system after it gets the huntflow.
   //  public HuntView(IHuntViewActions huntViewActions, IViewUIActions viewUIActions, IProductEvents events, IHuntController huntController)
   //  {
   //      _huntViewActions = huntViewActions;
   //      _viewUIActions = viewUIActions;
   //      _events = events;
   //      _huntController = huntController;
   //  }
   //  
   //  public void Configure(StartPanelData startPanelData)
   //  {
   //      _huntViewActions.configure(startPanelData, null,new ProductService(Application.persistentDataPath));
   //  }
   //
   //  public void GoBackToProductList(bool completed)
   //  {
   //      _events.ProductAborted();
   //      _huntController.EndHunt(false);
   //      _viewUIActions.Hide();
   //  }
   //
   //  public HuntController GetHuntController()
   //  {
   //      return (HuntController)_huntController;
   //  }
}

public interface IHuntViewActions
{
    public Task configure(StartPanelData startPanelData, IHuntAssetGetter assetGetter, IGetProductFlowData flowGetter);
}
public class HuntViewBehaviour : MonoBehaviour//, IHuntViewActions, IViewUIActions
{
    // private IHuntView _huntView;
    //
    // [SerializeField] private TabComponentBehaviour tabComponentPrefab;
    // [SerializeField] private StoryHuntComponentBehaviour storyComponentPrefab;
    // [SerializeField] private RiddleTabComponentBehaviour riddleTabComponentPrefab;
    // [SerializeField] private HuntHomeComponentBehaviour huntHomeComponentPrefab;
    // [SerializeField] private ScanningCorrectDisplayVideoHuntComponentBehaviour screeningCorrectPanelPrefab; 
    // [SerializeField] private EndHuntComponentBehaviour endScreenPanelPrefab;
    // [SerializeField] private RatingComponentBehaviour ratingComponentPrefab;
    // [SerializeField] private ResolutionComponentBehaviour resolutionComponentPrefab;
    // [SerializeField] private Canvas viewCanvasComponent;
    // public ITabComponent TabComponent{ get; private set; }
    // public IHuntHomeComponent HomeComponent{ get; private set; }
    // public IStoryComponent StoryComponent{ get; private set; }
    // public IRiddleTabComponent RiddleTabComponent{ get; private set; }
    // public IEndHuntComponent EndHuntComponent{ get; private set; }
    // public IScanningCorrectDisplayVideoComponent ScanningCorrectDisplayVideoComponent{ get; private set; }
    // public IRatingComponent RatingComponent { get; private set; }
    // public IResolutionComponent ResolutionComponent { get; private set; }
    //
    // public void Awake()
    // {
    //     var tabComponentGo = Instantiate(tabComponentPrefab);
    //     TabComponent = global::TabComponent.Factory(tabComponentGo.gameObject, (RectTransform)this.transform);
    //     
    //     //Flows, views, components, ComponentHelpers, Overlay (Popup/Warning)
    //     
    //     //flows are a declared logic to handle views.
    //     //Views are a declared logic to handle components.
    //     //components are the base material to create the user experience.
    //     //ComponentHelper, make component management easier (UIActions, ComponentHelper, UIFitter)
    //     
    //     CreateHomeComponent();
    //     CreateStoryComponent();
    //     CreateEndComponent();
    //     CreateScanningCorrectScreen();
    //     CreateRiddleComponent();
    //     CreateResolutionComponent();
    //     
    //     TabComponent.ConfigureTabs(
    //         new List<IHuntComponentUIActions>()
    //         {
    //             HomeComponent.GetComponentUIActions(), 
    //             StoryComponent.GetComponentUIActions(), 
    //             RiddleTabComponent.GetComponentUIActions(), 
    //             EndHuntComponent.GetComponentUIActions(), 
    //             ScanningCorrectDisplayVideoComponent.GetComponentUIActions(),
    //             ResolutionComponent.GetComponentUIActions()
    //         });
    //     
    //     //not a part of the huntflow.
    //     CreateRatingComponent();
    // }
    //
    // //this code is inserted for testing purposes.
    // public void SetTestDependencies(
    //     ITabComponent tabComponent, 
    //     IHuntHomeComponent home, 
    //     IStoryComponent story, 
    //     IRiddleTabComponent riddle, 
    //     IEndHuntComponent end, 
    //     IScanningCorrectDisplayVideoComponent scanning,
    //     IResolutionComponent resolution)
    // {
    //     TabComponent = tabComponent;
    //     HomeComponent = home;
    //     StoryComponent = story;
    //     RiddleTabComponent = riddle;
    //     EndHuntComponent = end;
    //     ScanningCorrectDisplayVideoComponent = scanning;
    //     ResolutionComponent = resolution;
    // }
    //
    // public void SetLogicInstance(IHuntView huntView)
    // {
    //     _huntView = new ComponentHelper<IHuntView>().SetLogicInstance(huntView, _huntView);
    // }
    //
    // public void FitInView(RectTransform parent, IUIFitters uiFitters, Camera viewCamera)
    // {
    //     if(viewCanvasComponent!=null) {
    //         viewCanvasComponent.renderMode = RenderMode.ScreenSpaceCamera;
    //         viewCanvasComponent.worldCamera = viewCamera;
    //     }
    //     uiFitters.FitToGlobalView((RectTransform)this.transform);
    // }
    // public async Task configure(StartPanelData startPanelData, IHuntAssetGetter assetGetter, IGetProductFlowData flowGetter)
    // {
    //     if (assetGetter == null)
    //         assetGetter = HuntAssetGetter.Factory(this);
    //     
    //     HomeComponent.Configure(startPanelData, async (introVidepPrepared) =>
    //     {
    //         if (startPanelData.HasAccess)
    //         {
    //             var huntFlow = await flowGetter.GetProductFlow(startPanelData.Id);
    //         
    //             assetGetter.GetHuntAssets(huntFlow,  startPanelData.Id, startPanelData.FeedbackLink, (data) =>
    //             {
    //                 DisplayStoryAndDoneStepController displayStoryAndDoneStepController =
    //                     new DisplayStoryAndDoneStepController(TabComponent, StoryComponent, HomeComponent, EndHuntComponent);
    //                 
    //                 DisplayStoryAndSubmitAnswerStepController displayStoryAndDoneSubmitAnswerStepController =
    //                     new DisplayStoryAndSubmitAnswerStepController(TabComponent, StoryComponent, RiddleTabComponent, HomeComponent, EndHuntComponent);
    //                 
    //                 HuntResolutionAndEndStepController huntResolutionAndEndStepController =
    //                     new HuntResolutionAndEndStepController(TabComponent, StoryComponent, RiddleTabComponent, HomeComponent, EndHuntComponent, ResolutionComponent);
    //                 //TODO: AR Element
    //                 // RecognizeWithAssetBundleStepController recognizeWithAssetBundleStepController =
    //                 //     new RecognizeWithAssetBundleStepController(TabComponent, StoryComponent, HomeComponent, EndHuntComponent, ScanningCorrectDisplayVideoComponent );
    //                 //
    //                 Dictionary<StepType, IStepController> stepControllers = new Dictionary<StepType, IStepController>();
    //                 stepControllers.Add(StepType.DisplayStoryAndDone, displayStoryAndDoneStepController);
    //                 stepControllers.Add(StepType.DisplayRiddleAndSubmitAnswer, displayStoryAndDoneSubmitAnswerStepController);
    //                 stepControllers.Add(StepType.HuntResolutionAndEnd, huntResolutionAndEndStepController);
    //                 // stepControllers.Add(StepType.RecognizeWithAssetBundle, recognizeWithAssetBundleStepController);
    //                 //TODO: AR Element
    //                 _huntView.GetHuntController().ConfigureHunt(data, stepControllers, () =>
    //                 {
    //                     InitRatingComponent(startPanelData.FeedbackLink, startPanelData.Id);
    //                 });
    //                 HomeComponent.HuntReady(_huntView.GetHuntController(), new ConditionalStepList(), new List<IConditionalStepBtn>());
    //             });
    //         }
    //     },_huntView.GoBackToProductList);
    // }
    //
    // private void InitRatingComponent(string feedbackLink, string productID)
    // {
    //     new Analytics().ProductEnd(productID);
    //     Action<int> BtnAction = (rating) =>
    //     {
    //         RatingComponent.GetComponentUIActions().Hide();
    //         _huntView.GoBackToProductList(true);
    //         if (rating != -1)
    //         {
    //             new Analytics().ProductFeedback(productID, rating);
    //         }
    //
    //         if (!string.IsNullOrEmpty(feedbackLink))
    //         {
    //             Application.OpenURL(feedbackLink);
    //         }
    //     };
    //     
    //     RatingComponent.Configure(BtnAction);
    //     RatingComponent.GetComponentUIActions().Display();
    // }
    //
    // private void CreateHomeComponent()
    // {
    //     var homeComponentGo = Instantiate(huntHomeComponentPrefab.gameObject);
    //     HomeComponent = HuntHomeComponent.Factory(homeComponentGo);
    //     homeComponentGo.SetActive(false);
    // }
    // private void CreateStoryComponent()
    // {
    //     var storeComponentGo = Instantiate(storyComponentPrefab.gameObject);
    //     StoryComponent = global::StoryComponent.Factory(storeComponentGo);
    //     storeComponentGo.SetActive(false);
    // }
    // private void CreateScanningCorrectScreen()
    // {
    //     var scanningCorrectDisplayVideoGo = Instantiate(screeningCorrectPanelPrefab.gameObject);
    //     ScanningCorrectDisplayVideoComponent = global::ScanningCorrectDisplayVideoComponent.Factory(scanningCorrectDisplayVideoGo);
    //     scanningCorrectDisplayVideoGo.SetActive(false);
    // }
    // private void CreateEndComponent()
    // {
    //     var endScreenComponentGo = Instantiate(endScreenPanelPrefab.gameObject);
    //     EndHuntComponent = global::EndHuntComponent.Factory(endScreenComponentGo);
    //     endScreenComponentGo.SetActive(false);
    // }
    // private void CreateRiddleComponent()
    // {
    //     var riddleComponentGo = Instantiate(riddleTabComponentPrefab.gameObject);
    //     RiddleTabComponent = global::RiddleTabComponent.Factory(riddleComponentGo);
    //     riddleComponentGo.SetActive(false);
    // }
    //
    // private void CreateRatingComponent()
    // {
    //     var ratingComponentGo = Instantiate(ratingComponentPrefab.gameObject);
    //     RatingComponent = global::RatingComponent.Factory(ratingComponentGo);
    //     RatingComponent.GetComponentUIActions().FitInView((RectTransform)this.transform, new UIFitters());
    //     ratingComponentGo.SetActive(false);
    // }
    //
    // private void CreateResolutionComponent()
    // {
    //     var resolutionComponentGo = Instantiate(resolutionComponentPrefab.gameObject);
    //     ResolutionComponent = global::ResolutionComponent.Factory(resolutionComponentGo);
    //     resolutionComponentGo.SetActive(false);
    // }
    //
    // public void Display()
    // {
    //     this.viewCanvasComponent.gameObject.SetActive(true);
    //     this.gameObject.SetActive(true);
    // }
    //
    // public void Hide()
    // {
    //     this.viewCanvasComponent.gameObject.SetActive(false);
    //     this.gameObject.SetActive(false);
    // }
    //
    // public bool IsShown()
    // {
    //     return this.gameObject.activeSelf;
    // }
    //
    // public ViewType GetViewType()
    // {
    //     return ViewType.huntView;
    // }
}
