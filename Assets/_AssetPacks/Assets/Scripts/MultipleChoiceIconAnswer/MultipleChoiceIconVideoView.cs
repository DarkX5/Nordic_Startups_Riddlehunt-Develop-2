using System;
using System.Collections;
using System.Collections.Generic;
using Answers.MultipleChoice.Data.Icon;
using MultipleChoice.Icon.Display;
using riddlehouse.video;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Answers.MultipleChoice.Icon
{
    public interface IMultipleChoiceIconAnswerView
    {
        public void Configure(MultipleChoiceIconVideoView.Config config);

        public void DisplayIncomingTransmission();

        public void DisplayVideo();
    }

    public class MultipleChoiceIconVideoView : MonoBehaviour, IMultipleChoiceIconAnswerView
    {
        [SerializeField] private SimpleVideoView simpleVideoView;
        [SerializeField] private GameObject incomingCallView;
        public class Config
        {
            public IVideoCanvasController VideoCanvasController { get; set; }
            public string videoUri { get; set; }
            public Action GoBack  { get; set; }
            public Action TransmissionAccepted { get; set; }
            public Action OutroSkipped { get; set; }
        }

        private Config _config;
        public void Configure(Config config)
        {
            _hasAcceptedTransmission = false;
            _config = config;
            simpleVideoView.Initialize(_config.VideoCanvasController);
            simpleVideoView.Configure(new SimpleVideoView.Config()
            {
                OnSkip = () => { if(_hasAcceptedTransmission) _config.OutroSkipped();},
                videoUri = _config.videoUri
            });
            DisplayVideo();
            simpleVideoView.PlayPause();
            simpleVideoView.FullscreenToggle();
        }

        private bool _hasAcceptedTransmission = false;
        public void DisplayIncomingTransmission()
        {
            incomingCallView.SetActive(true);
            simpleVideoView.gameObject.SetActive(false);
        }
        
        public void DisplayVideo()
        {
            incomingCallView.SetActive(false);
            simpleVideoView.gameObject.SetActive(true);
        }
        
        public void AcceptTransmission()
        {
            _hasAcceptedTransmission = true;
            _config.TransmissionAccepted.Invoke();
        }
        
        public void GoBack()
        {
            _config.GoBack();
        }
    }
}