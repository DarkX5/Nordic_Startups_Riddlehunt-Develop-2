using System;
using System.Collections;
using System.Collections.Generic;
using Answers.MultipleChoice.Components;
using Answers.MultipleChoice.Data.Icon;
using Answers.MultipleChoice.Icon;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class VideoBasedRiddleViewController : MonoBehaviour
{
    public static VideoBasedRiddleViewController Factory(VideoBasedRiddleViewController prefab, Transform parent)
    {
        return Instantiate(prefab, parent);
    }
    public class Config
    {
        public IconMultipleChoiceAnswerData AnswerData { get; set; }
        public string IntroVideo { get; set; }
        public string OutroVideo { get; set; }
        public Action GoBack { get; set; }
        public Action StepCompleted { get; set; }
    }
    
    [Inject] CanvasLayerManager _clm;
    private IVideoCanvasController _videoCanvasController;

    [FormerlySerializedAs("multipleChoiceIconAnswerView")] [SerializeField] private MultipleChoiceIconVideoView multipleChoiceIconVideoView;
    [SerializeField] private MultipleChoiceIconAnswerOptionsDisplay multipleChoiceOptionsDisplay;
    
    public void Start()
    {
        _videoCanvasController = _clm.GetVideoCanvas();
        Hide();
    }
    
    #if UNITY_EDITOR //developer cheats
    public void Update()
    {
        if (Input.GetKey(KeyCode.F1) && Input.GetKeyDown(KeyCode.F4))
        {
            _config.AnswerData.FulfillAnswer();
            RiddleComplete();
        }
    }
    #endif

    private Config _config;
    public void Configure(Config config)
    {
        _config = config;
        
        multipleChoiceOptionsDisplay.Configure(new MultipleChoiceIconAnswerOptionsDisplay.Config()
        {
            RiddleComplete = RiddleComplete,
            AnswerData = config.AnswerData
        });

        multipleChoiceIconVideoView.Configure(new MultipleChoiceIconVideoView.Config()
        {
            VideoCanvasController = _videoCanvasController,
            videoUri = config.IntroVideo,
            TransmissionAccepted = TransmissionAccepted,
            GoBack = GoBack,
            OutroSkipped = OutroComplete
        });
        _videoCanvasController.GetVideoController().SubscribeToVideoCompletion(IntroComplete);
    }

    private void GoBack()
    {
        if(_videoCanvasController.GetVideoController().IsPlaying())
            _videoCanvasController.GetVideoController().Stop();
        _config.GoBack.Invoke();
    }

    private void RiddleComplete()
    {
        multipleChoiceIconVideoView.DisplayIncomingTransmission();
        _videoCanvasController.GetVideoController().Prepare(_config.OutroVideo);
    }

    private void TransmissionAccepted()
    {
        multipleChoiceIconVideoView.DisplayVideo();
        _videoCanvasController.Play();
        _videoCanvasController.FullscreenOpen();
        _videoCanvasController.GetVideoController().SubscribeToVideoCompletion(OutroComplete);
    }

    private void IntroComplete()
    {
        _videoCanvasController.FullscreenClose();
        _videoCanvasController.GetVideoController().UnsubscribeToVideoCompletion(IntroComplete);
    }

    private void OutroComplete()
    {
        _config.StepCompleted.Invoke();
        _videoCanvasController.FullscreenClose();
        _videoCanvasController.GetVideoController().UnsubscribeToVideoCompletion(OutroComplete);
    }

    public void Display()
    {
        this.gameObject.SetActive(true);
    }
    
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
