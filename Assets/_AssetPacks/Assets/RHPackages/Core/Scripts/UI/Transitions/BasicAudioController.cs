using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(AudioSource))]
public class BasicAudioController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public void SetAudio(float value)
    {
        audioSource ??= gameObject.GetComponent<AudioSource>();
        value = Mathf.Clamp(value, 0f, 1f);
        audioSource.volume = value;
        if(audioSource.volume < 0.1f  && audioSource.isPlaying)
            audioSource.Pause();
        if(audioSource.volume >0.1f && !audioSource.isPlaying)
            audioSource.Play();
    }
}
