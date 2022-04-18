using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public interface ICanvasGroupFaderWithAudioFade : ICanvasGroupFader
{
    public void Configure();
}
[RequireComponent(typeof(BasicAudioController))]
public class CanvasGroupFaderWithAudioFade : CanvasGroupFader, ICanvasGroupFaderWithAudioFade
{
    [SerializeField] private BasicAudioController audioController;
    public void Configure()
    {
        audioController ??= gameObject.GetComponent<BasicAudioController>();
        RepeatingEventValue += audioController.SetAudio;
    }
}
