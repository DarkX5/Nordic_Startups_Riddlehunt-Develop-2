using System;
using System.Collections;
using System.Collections.Generic;
using riddlehouse.video;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class VideoDemo : MonoBehaviour
{
    [SerializeField] private SimpleVideoView _video;
    
    [SerializeField] private CanvasLayerTypes clt;
    public void Start()
    {
        CanvasLayerManager clm = new CanvasLayerManager(clt);

        _video.Initialize(clm.GetVideoCanvas());
        
        _video.Configure(new SimpleVideoView.Config()
        {
            videoUri = "https://cdn.escaperoom-riddlehouse.dk/riddlehunt/det_glemte_test_produkt_022/steps/shared/intro.mp4",
        });
    }
}
