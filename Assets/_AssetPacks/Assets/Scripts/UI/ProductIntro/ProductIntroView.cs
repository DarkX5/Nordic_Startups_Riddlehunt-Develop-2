using System;
using System.Collections;
using System.Collections.Generic;
using riddlehouse.video;
using UnityEngine;
using UnityEngine.Video;
using Zenject;

public interface IProductIntroView
{
    public void Configure(ProductIntroView.Config config);
    public void Display();
    public void Hide();
}
public class ProductIntroView : MonoBehaviour,IProductIntroView
{
    public static IProductIntroView Factory(ProductIntroView prefab, Transform parent)
    {
        var behaviour = Instantiate(prefab, parent);
        behaviour.Initialize();
        return behaviour;
    }
    public class Dependencies
    {
        public IStandardButton ContinueButton { get; set; }
        public IStandardButton BackButton { get; set; }
        public IGenericWebView GenericWebView { get; set; }
        public ILoginHandler LoginHandler { get; set; }
        public ICanvasLayerManager Clm { get; set; }
    }

    public class Config
    {
        public string VideoUrl { get; set; }
        public string Url { get; set; }
        public Action BackAction { get; set; }
        public Action ContinueAction { get; set; }
    }

    [SerializeField] private StandardButtonBehaviour continueButton;
    [SerializeField] private StandardButtonBehaviour backButton;
    [SerializeField] private GenericWebView genericWebView;
    [Inject] private LoginHandler _loginHandler;
    [Inject] private CanvasLayerManager _clm;
    private IVideoCanvasController _videoCanvasController;
    private ILoaderView _loaderView;
    public void Initialize()
    {
        SetDependencies(new Dependencies()
        {
            ContinueButton = continueButton,
            BackButton = backButton,
            GenericWebView = genericWebView,
            LoginHandler = _loginHandler,
            Clm = _clm
        });
        _loginHandler.ServiceUpdated += OnLoginChanged;
        Hide();
    }

    public void OnDestroy()
    {
        if(_dependencies.LoginHandler != null)
            _loginHandler.ServiceUpdated -= OnLoginChanged;
    }

    public Dependencies _dependencies { get; private set; }
    public void SetDependencies(Dependencies dependencies)
    {
        _dependencies = dependencies;
        _videoCanvasController = _dependencies.Clm.GetVideoCanvas();
        _loaderView = _dependencies.Clm.GetLoaderView();
    }

    private Config _config;
    private Action _continueAction;
    public void Configure(Config config)
    {
        _config = config;
        _continueAction = ContinueAction;
        _continueAction += config.ContinueAction;
        _videoCanvasController.GetVideoController().SubscribeToPrepareEvent(PlayVideoAfterPrepare);
        _videoCanvasController.Configure(new VideoCanvasController.Config()
        {
            Url = config.VideoUrl,
            VideoControlsConfig = new VideoControlOverlay.Config()
            {
                FullscreenToggle = null,
                PlayPause = _videoCanvasController.GetVideoController().PlayPause,
                Replay = _videoCanvasController.GetVideoController().Replay,
                Skip = EndVideo,
                VideoState = _videoCanvasController.GetVideoState()
            }
        });
        _videoCanvasController.FullscreenOpen();
        _loaderView.Display();
        _videoCanvasController.GetVideoController().SubscribeToVideoCompletion(VideoComplete);
        
        _dependencies.GenericWebView.Prepare();

        _dependencies.BackButton.Configure("Tilbage", _config.BackAction);
        if (!_dependencies.LoginHandler.IsLoggedInAsUser())
            OnLoginChanged(LoginEvents.loggedInAsGuest);
        else 
            OnLoginChanged(LoginEvents.loggedInAsUser);
    }

    private void ContinueAction()
    {
        _dependencies.GenericWebView.Close();
    }

    private void PlayVideoAfterPrepare(VideoPlayer vp)
    {
        _loaderView.Hide();
        vp.Play();
        _videoCanvasController.GetVideoController().UnsubscribePrepareEvent(PlayVideoAfterPrepare);
    }

    private void VideoComplete()
    {
        EndVideo();
        _videoCanvasController.GetVideoController().UnsubscribeToVideoCompletion(VideoComplete);
    }

    private void EndVideo()
    {
        _videoCanvasController.GetVideoController().Stop();
        _videoCanvasController.FullscreenClose();
        _dependencies.GenericWebView.Load(_config.Url);
        _dependencies.GenericWebView.Open();
    }
    private void OnLoginChanged(LoginEvents loginEvents)
    {
        switch (loginEvents)
        {
            case LoginEvents.loggedInAsGuest:
                _dependencies.ContinueButton.Configure("Log Ind", _dependencies.LoginHandler.LoginAction);
                break;
            case LoginEvents.loggedInAsUser:
                _dependencies.ContinueButton.Configure("Start", _continueAction);
                break;
        }
    }
    public void Display()
    {
        this.gameObject.SetActive(true);
    }
    public void Hide()
    {
        _loaderView.Hide();
        this.gameObject.SetActive(false);
    }


}
