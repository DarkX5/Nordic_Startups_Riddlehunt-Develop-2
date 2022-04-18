using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public interface IVideo
{
   public void SetVideoForFullScreen();
   public void SetVideoForPortrait();
   public void SwapFullScreenMode();
   public void Configure(RenderTexture texture, Action openFullscreen, Action playPause);
   public void Configure(string link);
   public void Play();
   public void Replay();
   public void Pause();
   public bool isFullScreen();
   public void SubscribeToVideoUpdates(Action<VideoEvent> actionToSubscribe);
   

}
public enum VideoEvent {fullscreenOpen, fullscreenClosed, prepareCompleted}
public class Video : IVideo
{
   private IVideoActions _videoActions;
   private GameObject _go;
   
   public bool _isFullScreen { get; private set; } = false;
   public float portraitModeHeight { get; private set; }
   public static IVideo Factory(GameObject gameObject)
   {
      var videoActions = gameObject.GetComponent<VideoBehaviour>();
        
      if (videoActions == null)
      {
         throw new ArgumentException("VideoBehaviour missing on prefab");
      }
      var video = new Video(videoActions, gameObject);
      videoActions.setVideo(video);
      
      return video;
   }
   public Video(IVideoActions videoActions, GameObject go)
   {
      _videoActions = videoActions;
      _go = go;
   }
   public void SetVideoForFullScreen()
   {
      _isFullScreen = true;
   }
   
   public void SetVideoForPortrait()
   {
      _isFullScreen = false;
   }

   public void SwapFullScreenMode()
   {
      if (!isFullScreen())
      {
         SetVideoForFullScreen();
      }
      else
      {
         SetVideoForPortrait();
      }
   }
   public void Configure(RenderTexture texture, Action openFullscreen, Action playPause)
   {
      RectTransform rt = (RectTransform)_go.transform;
      portraitModeHeight = rt.rect.height;
      _videoActions.Configure(texture, openFullscreen, playPause);
   }

   public void Configure(string link)
   {
      throw new NotImplementedException();
   }

   public void Play()
   {
      _videoActions.Play();
   }
   public void Replay()
   {
      _videoActions.Replay();
   }
   public void Pause()
   {
      _videoActions.Pause();
   }

   public bool isFullScreen()
   {
      return _isFullScreen;
   }

   public void SubscribeToVideoUpdates(Action<VideoEvent> actionToSubscribe)
   {
      _videoActions.SubscribeToVideoUpdates(actionToSubscribe);
   }
   public void UnsubscribeToVideoUpdates(Action<VideoEvent> actionToSubscribe)
   {
      _videoActions.UnsubscribeToVideoUpdates(actionToSubscribe);
   }
}
public interface IVideoActions
{
   public void Configure(RenderTexture texture, Action openFullscreen, Action playPause);
   public void Configure(string link);
   public void Play();
   public void Replay();
   public void Pause();
   public void SubscribeToVideoUpdates(Action<VideoEvent> actionToSubscribe);
   public void UnsubscribeToVideoUpdates(Action<VideoEvent> actionToSubscribe);
}
public class VideoBehaviour : MonoBehaviour, IVideoActions
{
   public Video Video { get; private set; }
   public System.Action<VideoEvent> videoUpdated;

   public void Awake()
   {
      setVideo(new Video(this, this.gameObject));
      videoUpdated = (videoEvent) => { };
   }

   public void setVideo(Video video)
   {
      if(Video == null)
         Video = video;
      else
      {
         Debug.LogWarning("Can only be set once");
      }
   }

   [SerializeField]
   private VideoPlayer videoPlayer;
   
   [SerializeField]
   private Image FullScreenBtnImage;
   [SerializeField] private Sprite fullscreenEnter;
   [SerializeField] private Sprite fullscreenExit;
   
   [SerializeField]
   private Image playBtnImage;
   [SerializeField] private Sprite play;
   [SerializeField] private Sprite pause;

   [SerializeField] private GameObject replayBtn;
   [SerializeField] private GameObject optionsUI;
   [SerializeField] private RawImage imageUI;
   private Action _openFullscreen;
   private Action _playPause;
   public void Configure(RenderTexture texture, Action openFullscreen, Action playPause)
   {
      imageUI.texture = texture;
      _openFullscreen = openFullscreen;
      _playPause = playPause;
      //videoPlayer.url = videoLink;
      // videoPlayer.loopPointReached += DisplayReplay;
      // videoPlayer.prepareCompleted += VideoIsPrepared;
      replayBtn.SetActive(false);
      optionsUI.SetActive(true);
   }

   public void Configure(string link)
   {
      throw new NotImplementedException();
   }

   private void VideoIsPrepared(VideoPlayer source)
   {
      videoUpdated.Invoke(VideoEvent.prepareCompleted);
   }
   
   public void PlayPause()
   {
      _playPause.Invoke();
      // if (videoPlayer.isPlaying)
      //    Pause();
      // else
      //    Play();
   }
   public void Play()
   {
      optionsUI.SetActive(false);
      replayBtn.SetActive(false);
      playBtnImage.sprite = play;
      videoPlayer.Play();
   }

   public void Replay()
   {
      replayBtn.SetActive(false);
      videoPlayer.Stop();
      videoPlayer.Play();
   }

   private void DisplayReplay(VideoPlayer vp)
   {
      replayBtn.SetActive(true);
      Video.SetVideoForPortrait();
   }

   public void Pause()
   {
      optionsUI.SetActive(true);
      playBtnImage.sprite = pause;
      videoPlayer.Pause();
   }

   public void SubscribeToVideoUpdates(Action<VideoEvent> actionToSubscribe)
   {
      videoUpdated += actionToSubscribe;
   }

   public void UnsubscribeToVideoUpdates(Action<VideoEvent> actionToSubscribe)
   {
      videoUpdated -= actionToSubscribe;
   }

   public void ChangeFullscreen()
   {
      Video.SwapFullScreenMode();
      if (Video.isFullScreen())
      {
         _openFullscreen.Invoke();
        // FullScreenBtnImage.sprite = fullscreenExit;
         videoUpdated.Invoke(VideoEvent.fullscreenOpen);
      }
      else
      {
        // FullScreenBtnImage.sprite = fullscreenEnter;
         videoUpdated.Invoke(VideoEvent.fullscreenClosed);
      }
   }
}
