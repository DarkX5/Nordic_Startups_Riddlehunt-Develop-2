using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace riddlehouse.video
{
    public interface IVideoControlOverlay
    {
        public void Initialize();
        public void Configure(VideoControlOverlay.Config config);
        public void UpdateControlUIState();
        public void SetInteractable(bool interactable);
    }
    [RequireComponent(typeof(CanvasGroupFader))]
    public class VideoControlOverlay : MonoBehaviour, IVideoControlOverlay
    {
        public class Dependencies
        {
            public ICanvasGroupFader CanvasGroupFader { get; set; }
            public IWaitToExecuteAction WaitToExecuteAction { get; set; }
            public Sprite PlayIcon { get; set; }
            public Sprite PauseIcon { get; set; }
            public Sprite EnterFullscreenIcon { get; set; }
            public Sprite ExitFullscreenIcon { get; set; }
            public Image PlayPauseButtonImage { get; set; }
            public Image FullscreenButtonImage { get; set; }
            public Image ReplayButtonImage { get; set; }
            public Image SkipButtonImage { get; set; }
        }

        public class Config
        {
            public IVideoState VideoState { get; set; }
            public Action PlayPause { get; set; }
            public Action Replay { get; set; }
            public Action Skip { get; set; }
            public Action FullscreenToggle { get; set; }
        }
        private bool _initialized = false;

        public void Initialize()
        {
            if (!_initialized)
            {
                SetDependencies(new Dependencies()
                {
                    CanvasGroupFader = fader,
                    WaitToExecuteAction = actionTimer,
                    PlayIcon = playIcon,
                    PauseIcon = pauseIcon,
                    EnterFullscreenIcon = enterFullscreenIcon,
                    ExitFullscreenIcon = exitFullscreenIcon,
                    PlayPauseButtonImage = playPauseButtonImage,
                    FullscreenButtonImage = fullscreenButtonImage,
                    ReplayButtonImage = replayButtonImage,
                    SkipButtonImage = skipButtonImage
                });
            }
        }

        public Dependencies _dependencies { get; private set; }
        public void SetDependencies(Dependencies dependencies)
        {
            _dependencies = dependencies;
        }

        [SerializeField] private Sprite playIcon;
        [SerializeField] private Sprite pauseIcon;

        [SerializeField] private Sprite enterFullscreenIcon;
        [SerializeField] private Sprite exitFullscreenIcon;
        
        [SerializeField] private Image playPauseButtonImage;
        [SerializeField] private Image fullscreenButtonImage;

        [SerializeField] private Image replayButtonImage;
        [SerializeField] private Image skipButtonImage;

        [SerializeField] private CanvasGroupFader fader;
        [SerializeField] private WaitToExecuteAction actionTimer;
        
        private Config _config;
        public void Configure(Config config)
        {
            _config = config; 
            _dependencies.ReplayButtonImage.gameObject.SetActive(_config.Replay != null); 
            _dependencies.SkipButtonImage.gameObject.SetActive(_config.Skip != null);
            _dependencies.PlayPauseButtonImage.gameObject.SetActive(_config.PlayPause != null);
            _dependencies.FullscreenButtonImage.gameObject.SetActive(_config.FullscreenToggle != null);
            
            _dependencies.WaitToExecuteAction.Configure(TimedCloseControlsEvent, 1.5f);
        }

        public void SetInteractable(bool interactable)
        {
            _dependencies.CanvasGroupFader.SetInteractable(interactable);
        }
        
        public void PlayPause()
        {
            if (_config != null)
            {
                _config.PlayPause();
                UpdateControlUIState();
            }
        }

        public void UpdateControlUIState()
        {
            if (_config != null)
            {
                if (_config.VideoState.IsPlaying())
                {
                    SetPauseUI();
                }
                else
                {
                    SetPlayUI();
                }
                SetFullscreenUI();
            }
        }
        private void SetPlayUI()
        {
            _dependencies.PlayPauseButtonImage.sprite = _dependencies.PlayIcon;
        }

        private void SetPauseUI()
        {
            _dependencies.PlayPauseButtonImage.sprite = _dependencies.PauseIcon;
        }

        public void Replay()
        {
            if (_config != null)
            {
                _config.Replay();
                SetPauseUI();
                SetFullscreenUI();
            }
        }

        public void Skip()
        {
            if (_config != null)
            {
                _dependencies.CanvasGroupFader.Close(() =>
                {
                    _config.Skip();
                    SetPlayUI();
                    SetFullscreenUI();
                });
                
            }
        }

        public void ToggleFullscreen()
        {
            if (_config != null)
            {
                _config.FullscreenToggle();
                SetFullscreenUI();
            }
        }
        private void SetFullscreenUI()
        {
            if (_config != null)
            {
                if (_config.VideoState.IsFullscreen())
                {
                    _dependencies.FullscreenButtonImage.sprite = _dependencies.ExitFullscreenIcon;
                }
                else
                {
                    _dependencies.FullscreenButtonImage.sprite = _dependencies.EnterFullscreenIcon;
                }
            }
        }

        public void ToggleView()
        {
            if (_dependencies.CanvasGroupFader.IsOpen())
            {
                _dependencies.WaitToExecuteAction.StopWaiting();
                CloseView();
            }
            else
            {
                OpenView();
            }
        }

        private void TimedCloseControlsEvent()
        {
            CloseView();
        }

        private void OpenView()
        {
            UpdateControlUIState();
            if (_config.VideoState.IsPlaying())
            {
                _dependencies.CanvasGroupFader.SetInteractable(true);
            }
            _dependencies.CanvasGroupFader.Open();
            _dependencies.WaitToExecuteAction.BeginWaiting();
        }

        private void CloseView()
        {
            UpdateControlUIState();
            _dependencies.CanvasGroupFader.Close();
        }

        public void OnDisable()
        {
            if(_dependencies != null)
                _dependencies.CanvasGroupFader.ForceClosed();
        }
    }
}