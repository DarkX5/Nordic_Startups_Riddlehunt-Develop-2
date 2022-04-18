using System;
using System.Collections;
using System.Collections.Generic;
using riddlehouse.video;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Zenject;

namespace riddlehouse.video
{
    public interface ISimpleVideoView
    {
        public void Initialize(IVideoCanvasController videoCanvas);
        public void Configure(SimpleVideoView.Config config);
        public void PlayPause();
        public void FullscreenToggle();
        public void SubscribeToVideoPrepareEvent(UnityEngine.Video.VideoPlayer.EventHandler eventToFire);
        public void UnsubscribeToVideoPrepareEvent(UnityEngine.Video.VideoPlayer.EventHandler eventToFire);
    }
    public class SimpleVideoView : MonoBehaviour, ISimpleVideoView
    {
        public class Dependencies
        {
            public RawImage VideoDisplay { get; set; }
            public IVideoControlOverlay VideoControlOverlay { get; set; }
            public IVideoCanvasController VideoCanvasController { get; set; }
        }

        public class Config
        {
            public string videoUri { get; set; }
            public Action OnSkip { get; set; } = null;
            // public Action OnReplay { get; set; } = null; //potential future actions.
            // public Action OnPlay { get; set; } = null;
            // public Action OnPause { get; set; } = null;
        }

        [SerializeField] private RawImage videoDisplay;
        [SerializeField] private VideoControlOverlay controls;

        private bool _initialized = false;
        public void Initialize(IVideoCanvasController videoCanvas)
        {
            if (!_initialized)
            {
                _initialized = true;
                SetDependencies(new Dependencies()
                {
                    VideoDisplay = videoDisplay,
                    VideoControlOverlay = controls,
                    VideoCanvasController = videoCanvas
                });
            }
        }
        public Dependencies _dependencies { get; private set; }
        public void SetDependencies(Dependencies dependencies)
        {
            _dependencies = dependencies;
            _dependencies.VideoControlOverlay.Initialize();
        }

        private Config _config;
        public void Configure(Config config)
        {
            _config = config;
            var videoControls
                = new VideoControlOverlay.Config()
                {
                    VideoState = _dependencies.VideoCanvasController.GetVideoState(),
                    PlayPause = PlayPause,
                    Replay = Replay,
                    Skip = Skip,
                    FullscreenToggle = FullscreenToggle
                };
            _dependencies.VideoCanvasController.Configure(new VideoCanvasController.Config()
            {
                Url = _config.videoUri,
                VideoControlsConfig = videoControls
            });
            _dependencies.VideoControlOverlay.Configure(videoControls);
            _dependencies.VideoDisplay.texture = _dependencies.VideoCanvasController.GetTexture();
        }

        public void PlayPause()
        {
            if (!_dependencies.VideoCanvasController.GetVideoController().IsPlaying())
            {
                Play();
            }
            else
            {
                Pause();
            }
        }

        private void Play()
        {
            //_dependencies.VideoCanvasController.FullscreenOpen();
            _dependencies.VideoCanvasController.GetVideoController().Play();
            _dependencies.VideoControlOverlay.UpdateControlUIState();
        }

        private void Pause()
        {
            _dependencies.VideoCanvasController.GetVideoController().Pause();
            _dependencies.VideoControlOverlay.UpdateControlUIState();
        }

        public void Replay()
        {
            // _dependencies.VideoCanvasController.FullscreenOpen();
            _dependencies.VideoCanvasController.GetVideoController().Replay();
            _dependencies.VideoControlOverlay.UpdateControlUIState();
        }

        public void Skip()
        {
            _dependencies.VideoCanvasController.GetVideoController().Stop();
            _dependencies.VideoCanvasController.FullscreenClose();
            _config.OnSkip?.Invoke();
            _dependencies.VideoControlOverlay.UpdateControlUIState();
            _dependencies.VideoControlOverlay.SetInteractable(true);
        }

        public void FullscreenToggle()
        {
            if(_dependencies.VideoCanvasController.GetVideoState().IsFullscreen())
            {
                _dependencies.VideoCanvasController.FullscreenClose();
            }
            else
            {
                _dependencies.VideoCanvasController.FullscreenOpen();
            }
            _dependencies.VideoControlOverlay.UpdateControlUIState();
        }

        public void SubscribeToVideoPrepareEvent(VideoPlayer.EventHandler eventToFire)
        {
            _dependencies.VideoCanvasController.GetVideoController().SubscribeToPrepareEvent(eventToFire);
        }

        public void UnsubscribeToVideoPrepareEvent(VideoPlayer.EventHandler eventToFire)
        {
            _dependencies.VideoCanvasController.GetVideoController().UnsubscribePrepareEvent(eventToFire);
        }
    }
}