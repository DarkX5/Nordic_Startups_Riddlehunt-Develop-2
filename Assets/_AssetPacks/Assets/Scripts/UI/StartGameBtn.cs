using System;
using UnityEngine;
using Zenject;

public class StartGameBtn : MonoBehaviour
{
    // [Inject] private Video video;
    [SerializeField]
    private VideoBehaviour video;
    public void Start()
    {
        video.videoUpdated += (videoEvent) => VideoIsFullscreen(videoEvent);
    }
    
    public void StartGame()
    {
        Debug.Log("Start the game");
    }

    public void VideoIsFullscreen(VideoEvent videoEvent)
    {
        Debug.Log("video event kaldt");
        switch (videoEvent)
        {
            case VideoEvent.fullscreenOpen:
                this.gameObject.SetActive(false);
                break;
            case VideoEvent.fullscreenClosed:
                this.gameObject.SetActive(true);
                break;
        }
    }

    public void OnDestroy()
    {
        video.videoUpdated -= (videoEvent) => VideoIsFullscreen(videoEvent);    
    }
}
