using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class AnimatedNarratorText : MonoBehaviour
{
    [Header("Text Config")]
    // [SerializeField] private GameObject narrationBgImage = null;
    [SerializeField] private AudioSource narrationTextAudio = null;
    [SerializeField] private TMP_Text narrationText = null;
    [SerializeField] private float timePerCharacter = 1f;
    [SerializeField] private float narrationTextSpeedStep = 0.025f;
    [SerializeField] private bool enableTextBouncyness = false;
    [SerializeField] private float bounceDistance = 25f;
    [SerializeField] private float bounceDuration = 0.5f;

    [Header("DEBUG vars (Auto Set)")]
    [SerializeField] private Vector2 narrationTextSpeedMinMax = new Vector2(0f, 1f);
    [SerializeField] private string textToWrite;
    [SerializeField] private int charactedIdx;
    [SerializeField] private float timer;
    [SerializeField] private bool isBouncing = false;
    Vector3 textboxStartPosition;
    float timeElapsed;
    bool bouncingUp = false;
    float bouncingValueLerp;
    private bool isFinishedWriting = false;

    public bool IsFinishedWriting { get { return isFinishedWriting; } }

    // Start is called before the first frame update
    void Start()
    {
        if (narrationText == null)
            narrationText = GetComponentInChildren<TMP_Text>();
        if (narrationTextAudio == null)
            narrationTextAudio = GetComponentInChildren<AudioSource>();

        textboxStartPosition = transform.position;
        // narrationBgImage = GetComponentInChildren<Image
    }

    private void Update()
    {
        // once the text is written stop text updates and start bouncing :) 
        if (narrationText == null || textToWrite.Length < charactedIdx)
        {
            BounceText();
            // pause playing narrator mumble audio (pause instead of stop makes short clips seem longer)
            if (narrationTextAudio.isPlaying)
                narrationTextAudio.Pause();

            isFinishedWriting = true;
            return;
        }

        transform.position = textboxStartPosition;
        // start playing narrator mumble audio
        if (!narrationTextAudio.isPlaying)
            narrationTextAudio.Play();

        if (timer <= 0f)
        {
            // write next character
            timer += timePerCharacter;
            narrationText.text = textToWrite.Substring(0, charactedIdx);
            charactedIdx += 1;
        }
        else
        {
            timer -= Time.deltaTime;
        }
        isFinishedWriting = false;
    }
    private void BounceText()
    {
        if (!enableTextBouncyness) { return; }

        if (timeElapsed < bounceDuration)
        {
            if(bouncingUp) {
                bouncingValueLerp = Mathf.Lerp(textboxStartPosition.y - bounceDistance, textboxStartPosition.y, timeElapsed / bounceDuration);
                timeElapsed += Time.deltaTime;
            } else {
                bouncingValueLerp = Mathf.Lerp(textboxStartPosition.y, textboxStartPosition.y - bounceDistance, timeElapsed / bounceDuration);
                timeElapsed += Time.deltaTime;
            }
            transform.position = new Vector3(transform.position.x, bouncingValueLerp, transform.position.y);
        } else {
            bouncingUp = !bouncingUp;
            timeElapsed = 0f;
        }

        // throw new NotImplementedException();
    }

    public void WriteText(string newTextToWrite)
    {
        textToWrite = newTextToWrite;
        charactedIdx = 0;
    }
    public void WriteText(string newTextToWrite, float newTimePerCharacter)
    {
        textToWrite = newTextToWrite;
        timePerCharacter = newTimePerCharacter;
        charactedIdx = 0;
    }

    public void SetNarrationSpeed(float newTimePerCharacter)
    {
        timePerCharacter = newTimePerCharacter;
    }
    public void IncreaseNarrationSpeed()
    { // called from UI
        if (timePerCharacter < narrationTextSpeedMinMax.y)
        {
            timePerCharacter += narrationTextSpeedStep;
        }
        else
        {
            timePerCharacter = narrationTextSpeedMinMax.y;
        }
    }
    public void DecreaseNarrationSpeed()
    { // called from UI
        if (timePerCharacter > narrationTextSpeedMinMax.x)
        {
            timePerCharacter -= narrationTextSpeedStep;
        }
        else
        {
            timePerCharacter = narrationTextSpeedMinMax.x;
        }
    }
}
