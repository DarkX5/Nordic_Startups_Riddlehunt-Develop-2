using System;
using System.Diagnostics;
using riddlehouse.video;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Video;
using Debug = UnityEngine.Debug;

public interface IVideoController
{
    public RenderTexture GetRenderTexture();
    public void Replay();
    public void PlayPause();
    public void Play();
    public void Pause();
    public void Stop();
    public void SetFullscreenStateToOpen();
    public void SetFullscreenStateToClose();
    public void Prepare(RenderTexture texture, string url);
    public void Prepare(string url);
    public bool IsPlaying();
    public bool IsPrepared();
    public bool IsPaused();
    public void SubscribeToVideoCompletion(Action action);
    public void UnsubscribeToVideoCompletion(Action action);
    public void SubscribeToPrepareEvent(UnityEngine.Video.VideoPlayer.EventHandler prepareEvent);
    public void UnsubscribePrepareEvent(UnityEngine.Video.VideoPlayer.EventHandler prepareEvent);
}

public interface IVideoState
{
    public bool IsPlaying();
    public bool IsPrepared();
    public bool IsPaused();
    public bool IsFullscreen();
}
public class VideoController :IVideoController, IVideoState
{
    private bool fullscreenOpen = false;
    private VideoPlayer VideoPlayer;
    private Action Completed = () => {};
    public VideoController(VideoPlayer player)
    {
        VideoPlayer = player;
        VideoPlayer.loopPointReached += VideoFinishedPlaying;
    }

    public RenderTexture GetRenderTexture()
    {
        return VideoPlayer.targetTexture;
    }

    public void Play()
    {
        VideoPlayer.Play();
    }

    public void Pause()
    {
        VideoPlayer.Pause();
    }

    public void Stop()
    {
        VideoPlayer.Stop();
    }

    public void SetFullscreenStateToOpen()
    {
        fullscreenOpen = true;
    }

    public void SetFullscreenStateToClose()
    {
        fullscreenOpen = false;
    }

    public void Prepare(RenderTexture texture, string url)
    {
        VideoPlayer.targetTexture = texture;
        Prepare(url);
    }

    public void Prepare(string url)
    {
        VideoPlayer.url = url;
        VideoPlayer.Prepare();
    }

    public void SubscribeToPrepareEvent(UnityEngine.Video.VideoPlayer.EventHandler prepareEvent)
    {
        if(prepareEvent != null)
            VideoPlayer.prepareCompleted += prepareEvent;
    }
    public void UnsubscribePrepareEvent(UnityEngine.Video.VideoPlayer.EventHandler prepareEvent)
    {
        if(prepareEvent != null)
            VideoPlayer.prepareCompleted -= prepareEvent;
    }

    public void Replay()
    {
        VideoPlayer.Stop();
        VideoPlayer.frame = 0;
        VideoPlayer.Play();
    }

    public bool IsPlaying()
    {
        return VideoPlayer.isPlaying;
    }

    public bool IsPrepared()
    {
        return VideoPlayer.isPrepared;
    }

    public bool IsPaused()
    {
        return VideoPlayer.isPaused;
    }

    public bool IsFullscreen()
    {
        return fullscreenOpen;
    }

    public void SubscribeToVideoCompletion(Action action)
    {
        Completed += action;
    }

    public void UnsubscribeToVideoCompletion(Action action)
    {
        Completed -= action;
    }
    private void VideoFinishedPlaying(VideoPlayer vp)
    {
        Completed.Invoke();
    }
    public void PlayPause()
    {
        if (!IsPlaying())
        {
            Play();
        }
        else
        {
            Pause();
        }
    }
}

public interface IVideoCanvasController
{
    public VideoCanvasController.Dependencies dependencies { get; }

    public void Configure(VideoCanvasController.Config config);
    public void FullscreenOpen();
    public void FullscreenClose();
    public void Play();
    public void Pause();
    public void Replay();
    public void Stop();
    public IVideoController GetVideoController();
    public IVideoState GetVideoState();
    public RenderTexture GetTexture();
}

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(RectTransform))]
[RequireComponent((typeof(CanvasController)))]
public class VideoCanvasController : MonoBehaviour, IVideoCanvasController
{
    public static IVideoCanvasController Factory(VideoCanvasController prefab)
    {
        var behaviour = Instantiate(prefab.gameObject).GetComponent<VideoCanvasController>();
        behaviour.Initialize();
        return behaviour;
    }
    public class Config
    {
        public string Url { get; set; }
        public VideoControlOverlay.Config VideoControlsConfig { get; set; }
    }

    public class Dependencies
    {
        public IVideoFullscreenDisplayController VideoFullscreenDisplay { get; set; }
        public IVideoController VideoPlayer { get; set; }
        public IVideoState VideoState { get; set; }
        public ICanvasController CanvasController { get; set; }
    }

    Dependencies _dependencies { get; set; }
    public Dependencies dependencies
    {
        get { return _dependencies; }
    }

    [SerializeField] private VideoFullscreenDisplayController videoFullscreenDisplayPrefab;
    [SerializeField] private CanvasController _canvasController;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private Canvas cv;
    public void Initialize()
    {
        var iVideoPlayer = new VideoController(videoPlayer);
        _canvasController.Initialize();
        SetDependencies(new Dependencies()
        {
            VideoFullscreenDisplay = VideoFullscreenDisplayController.Factory(videoFullscreenDisplayPrefab, (RectTransform)this.transform),
            VideoPlayer = iVideoPlayer,
            VideoState = iVideoPlayer,
            CanvasController = _canvasController
        });
        _dependencies.VideoFullscreenDisplay.Hide();
    }

    public void SetDependencies(Dependencies dependencies)
    {
        cv = GetComponent<Canvas>();
        _dependencies = dependencies;
        _texture = new RenderTexture(1920, 1080, 16, RenderTextureFormat.ARGB32);
    }

    private RenderTexture _texture;
    private Config _config;
    public void Configure(Config config)
    {
        _config = config;

        if (_config.VideoControlsConfig == null)
            _config.VideoControlsConfig = new VideoControlOverlay.Config()
            {
                PlayPause = _dependencies.VideoPlayer.PlayPause,
                FullscreenToggle = null,
                Replay = Replay,
                Skip = () =>
                {
                    Stop();
                    FullscreenClose();
                },
                VideoState = _dependencies.VideoState
            };
        
        _dependencies.VideoFullscreenDisplay.Configure(new VideoFullscreenDisplayController.Config()
        {
            VideoControlsConfig = config.VideoControlsConfig,
            VideoTexture = _texture
        });
        _dependencies.VideoPlayer.SubscribeToPrepareEvent(OnPrepared);
        _dependencies.VideoFullscreenDisplay.SetInteractable(false);
        _dependencies.VideoPlayer.Prepare(_texture, config.Url);
    }

    private void OnPrepared(VideoPlayer vp)
    {
        _dependencies.VideoFullscreenDisplay.SetInteractable(true);
        _dependencies.VideoPlayer.UnsubscribePrepareEvent(OnPrepared);
    }

    public IVideoController GetVideoController()
    {
        return _dependencies.VideoPlayer;
    }

    public IVideoState GetVideoState()
    {
        return _dependencies.VideoState;
    }

    public RenderTexture GetTexture()
    {
        return _texture;
    }

    public void FullscreenOpen()
    {
       _dependencies.VideoFullscreenDisplay.Display();
       _dependencies.VideoPlayer.SetFullscreenStateToOpen(); //only updates a bool.
    }
    public void FullscreenClose()
    {
        _dependencies.VideoFullscreenDisplay.Hide();
        _dependencies.VideoPlayer.SetFullscreenStateToClose(); //only updates a bool.
    }

    public void Play()
    {
        _dependencies.VideoPlayer.Play();
    }

    public void Pause()
    {
        _dependencies.VideoPlayer.Pause();
    }

    public void Replay()
    {
        _dependencies.VideoPlayer.Replay();
    }

    public void Stop()
    {
        _dependencies.VideoPlayer.Stop();
    }
}
