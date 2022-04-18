using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RHPackages.Core.Scripts;
using RHPackages.Core.Scripts.UI;
using Riddlehouse.Core.Helpers.Helpers;
using riddlehouse.video;
using riddlehouse_libraries.products;
using riddlehouse_libraries.products.models;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Video;
// using UnityEngine.XR.ARSubsystems; //TODO: AR Element
using Zenject;

public interface IHuntHomeComponent
{
    public IHuntHomeComponentActions GetHuntHomeComponentActions();
    public IViewActions GetComponentUIActions();
    public void Configure(StartPanelData startPanelData, Action<bool> introVideoPrepared, Action<bool> goBack);
    //TODO: AR Element
    public void ConfigureARElements(/*IMorphableARCameraActions morphableARCamera,*/ /*RReferenceImageLibrary imageLibrary,*/ Action foundTarget);
    public void HuntReady(IChristmasHuntController christmasHuntController, IConditionalStepList stepList, List<IConditionalStepBtn> buttons);
    public void ReturnToAppHome();
}
public class HuntHomeComponent : IHuntHomeComponent
{
    private Action<bool> _goBack;
    public static IHuntHomeComponent Factory(GameObject go)
    {
        var huntHomeBehaviour = new ComponentHelper<HuntHomeComponentBehaviour>().GetBehaviourIfExists(go);

        var hunt = new HuntHomeComponent(huntHomeBehaviour, huntHomeBehaviour);
        huntHomeBehaviour.SetLogic(hunt);
        return hunt;
    }

    private readonly IHuntHomeComponentActions _huntHomeComponentActions;
    private readonly IViewActions _viewActions;
    
    public HuntHomeComponent(IHuntHomeComponentActions huntHomeComponentActions, IViewActions viewActions)
    {
        _huntHomeComponentActions = huntHomeComponentActions;
        _viewActions = viewActions;

    }
    public void Configure(StartPanelData startPanelData, Action<bool> introVideoPrepared, Action<bool> goBack)
    {
        _goBack = goBack;
        _huntHomeComponentActions.Configure(startPanelData, introVideoPrepared);
    }
    
    //TODO: AR Element
    public void ConfigureARElements(/*IMorphableARCameraActions arCamera,*/ /*XRReferenceImageLibrary imageLibrary,*/ Action foundTarget)
    {
        //TODO: AR Element
        _huntHomeComponentActions.ConfigureARElements(/*arCamera,*/ /*imageLibrary,*/ foundTarget);
    }

    public void HuntReady(IChristmasHuntController christmasHuntController, IConditionalStepList stepList, List<IConditionalStepBtn> buttons)
    {
        _huntHomeComponentActions.HuntReady(christmasHuntController, stepList, buttons);
    }

    public void ReturnToAppHome()
    {
        _goBack.Invoke(false);
    }
    
    public IHuntHomeComponentActions GetHuntHomeComponentActions()
    {
        return _huntHomeComponentActions;
    }

    public IViewActions GetComponentUIActions()
    {
        return _viewActions;
    }
}

public interface IHuntHomeComponentActions
{
    public void Configure(StartPanelData startPanelData, Action<bool> introVideoPrepared);
    //TODO: AR Element
    public void ConfigureARElements(/*IMorphableARCameraActions morphableARCamera,*/ /*XRReferenceImageLibrary imageLibrary,*/ Action foundTarget);
    public void HuntReady(IChristmasHuntController christmasHuntController, IConditionalStepList stepList, List<IConditionalStepBtn> buttons);
    public void ReturnToAppHome();
}

public class HuntHomeComponentBehaviour : MonoBehaviour, IHuntHomeComponentActions, IViewActions
{
    private IHuntHomeComponent _component;
    
    [FormerlySerializedAs("stepBtnPrefab")] [SerializeField] private GameObject serializedStepBtnPrefab;
    [FormerlySerializedAs("productGraphics")] [SerializeField] private GameObject serializedProductGraphics;
    [FormerlySerializedAs("productTitle")] [SerializeField] private TextMeshProUGUI serializedProductTitle;
    [FormerlySerializedAs("ScrollViewContent")] [SerializeField] private RectTransform serializedScrollViewContent;
    [FormerlySerializedAs("video")] [SerializeField] private SimpleVideoView serializedVideo;
    [SerializeField] private GameObject serializedGoBackPanel;

    private ITestable<GameObject> _goBackPanel;
    private ITestable<GameObject> _stepBtn;
    private ITestable<GameObject> _productGraphicsGameObject;
    private ITestable<TextMeshProUGUI> _productTitle;
    private ITestable<RectTransform> _scrollViewContent;
    
    public readonly ComponentType _viewType;
    private List<IConditionalStepBtn> _stepButtons;
    private IConditionalStepList _stepList;
    private ISimpleVideoView _video;
    private IChristmasHuntController _christmasHuntController;

    [Inject] private CanvasLayerManager _canvasLayerManager;
    public void Start()
    {
        SetDependencies(serializedStepBtnPrefab, serializedProductGraphics, serializedProductTitle, serializedScrollViewContent, serializedVideo, serializedGoBackPanel);
    }

    public void SetDependencies(GameObject stepBtnPrefab, GameObject productGraphics, TextMeshProUGUI productTitle, RectTransform scrollViewContent, ISimpleVideoView video, GameObject goBackPanel)
    {
        _stepBtn = new TestableGameObject(stepBtnPrefab);
        _productGraphicsGameObject = new TestableGameObject(productGraphics);
        _productTitle = new TestableComponent<TextMeshProUGUI>(productTitle);
        _scrollViewContent = new TestableComponent<RectTransform>(scrollViewContent);
        _video = video;
        _goBackPanel = new TestableGameObject(goBackPanel);
        if (_canvasLayerManager != null)
        {
            var videoCanvas = _canvasLayerManager.GetVideoCanvas();
            if(videoCanvas != null)
                _video.Initialize(videoCanvas);

            videoCanvas.GetVideoController().SubscribeToVideoCompletion(VideoCompletion);
        }
    }

    public void OnDestroy()
    {
        if (_canvasLayerManager != null)
        {
            var videoController = _canvasLayerManager.GetVideoCanvas();
            videoController.GetVideoController().UnsubscribeToVideoCompletion(VideoCompletion);
        }
    }

    public void SetLogic(IHuntHomeComponent huntHomeComponent)
    {
        _component = new ComponentHelper<IHuntHomeComponent>().SetLogicInstance(huntHomeComponent, _component);
    }
    
    public HuntHomeComponentBehaviour()
    {
        _viewType = ComponentType.HuntHome;
    }

    private bool _hasBeenConfigured = false;
    private StartPanelData _startPanelData;
    private Action<bool> _introVideoPrepared;
    public void Configure(StartPanelData startPanelData, Action<bool> introVideoPrepared)
    {
        _canvasLayerManager.GetLoaderView().Display();
        _introVideoPrepared = introVideoPrepared;
        _startPanelData = startPanelData;
        if (!_hasBeenConfigured)
        {
            _video.SubscribeToVideoPrepareEvent(IntroVideoPrepareEvent);
            _hasBeenConfigured = true;
        }
        _video.Configure(new SimpleVideoView.Config()
        {
            videoUri = startPanelData.StartVideoUrl,
            OnSkip = () =>
            {
                if (_stepButtons == null)
                {
                    _introVideoPrepared.Invoke(true);
                    _video.UnsubscribeToVideoPrepareEvent(IntroVideoPrepareEvent);
                    _canvasLayerManager.GetLoaderView().Display();
                }
            }
        });
        _video.FullscreenToggle();

        _productTitle.Get().text = startPanelData.Title;
        _goBackPanel.Get().SetActive(true);
        _productGraphicsGameObject.Get().SetActive(true);
    }

    private void IntroVideoPrepareEvent(VideoPlayer player)
    {
        _video.PlayPause();
        _canvasLayerManager.GetLoaderView().Hide();
        _introVideoPrepared.Invoke(true);
        _video.UnsubscribeToVideoPrepareEvent(IntroVideoPrepareEvent);
    }

    private void VideoCompletion()
    {
        if (_canvasLayerManager != null)
        {
            var videoController = _canvasLayerManager.GetVideoCanvas();
            videoController.FullscreenClose();
        }
    }
//TODO: AR Element
    public void ConfigureARElements(/*IMorphableARCameraActions morphableARCamera,*/ /*XRReferenceImageLibrary imageLibrary,*/ Action foundTarget)
    {
        //TODO: AR Element
        //morphableARCamera.PrepARCamera(/*imageLibrary,*/ foundTarget);
    }

    public void HuntReady(IChristmasHuntController christmasHuntController, IConditionalStepList stepList, List<IConditionalStepBtn> buttons)
    {
        if (_canvasLayerManager != null)
        {
            _canvasLayerManager.GetLoaderView().Hide();
        }
        _stepButtons = buttons;
        _stepList = stepList;
        _christmasHuntController = christmasHuntController;
        InstantiateStepBtns(_christmasHuntController);
        _stepList.ConfigureStepList(_christmasHuntController.GetCurrentHuntSteps(), _stepButtons, 
            (stepIndex) =>
            {
                _christmasHuntController.ConfigureRiddle(stepIndex);
                Hide();
            });
    }
    
    public void ReturnToAppHome()
    {
        if (_canvasLayerManager != null)
        {
            _video.UnsubscribeToVideoPrepareEvent(IntroVideoPrepareEvent);
            _canvasLayerManager.GetLoaderView().Hide();
            var videoController = _canvasLayerManager.GetVideoCanvas();
            if (videoController != null)
            {
                if (videoController.GetVideoController().IsPlaying())
                    videoController.GetVideoController().Stop();
            }
        }
        _component.ReturnToAppHome();
    }

    private void InstantiateStepBtns(IChristmasHuntController christmasHuntController)
    {
        if (_stepButtons == null)
        {
            _stepButtons = new List<IConditionalStepBtn>();
        }

        var lengthOfHunt = christmasHuntController.GetCurrentHuntSteps().GetLengthOfHunt();
        for (int i = 0; i < lengthOfHunt; i++)
        {
            var go = Instantiate(_stepBtn.Get(), Vector3.zero, Quaternion.identity);
            IConditionalStepBtn stepBtn = ConditionalStepBtn.Factory(go);
            go.transform.SetParent(_scrollViewContent.Get());
            go.transform.localScale = Vector3.one;
            _stepButtons.Add(stepBtn);
        }
    }

    public void FitInView(RectTransform parent, IUIFitters uiFitters)
    {
        uiFitters.FitToFullscreen(GetRectTransform(), parent);
    }

    public void FitInView(RectTransform parent, IUIFitters uiFitters, int index)
    {
        throw new NotImplementedException();
    }

    public void Display()
    {
        this.gameObject.SetActive(true);
        if (_startPanelData != null)
        {
            if (_canvasLayerManager != null)
            {
                var videoCanvas = _canvasLayerManager.GetVideoCanvas();
                videoCanvas.GetVideoController().Prepare(_startPanelData.VideoUrl);
            }
        }

        if (_stepButtons != null)
        {
            _stepList.ConfigureStepList(
                _christmasHuntController.GetCurrentHuntSteps(), 
                _stepButtons,
                (stepIndex) => _christmasHuntController.ConfigureRiddle(stepIndex)
            );
        }
    }
    
    public void Hide()
    {
        if (_canvasLayerManager != null)
        {
            _canvasLayerManager.GetLoaderView().Hide();
        }

        this.gameObject.SetActive(false);
    }

    public bool IsShown()
    {
        return gameObject.activeSelf;
    }

    public ComponentType GetComponentType()
    {
        return _viewType;
    }

    public RectTransform GetRectTransform()
    {
        return (RectTransform)this.transform;
    }
}