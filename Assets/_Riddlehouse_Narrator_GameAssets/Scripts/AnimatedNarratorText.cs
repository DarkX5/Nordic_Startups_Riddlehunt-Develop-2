using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class AnimatedNarratorText : MonoBehaviour
{
#region Serialized Fields Values
    [Header("Text Config")]
    // [SerializeField] private GameObject narrationBgImage = null;
    [SerializeField] private AudioSource narrationTextAudio = null;
    [SerializeField] private TMP_Text narrationText = null;
    [SerializeField] private float timePerCharacter = 1f;
    [SerializeField] private bool enableTextWriting = true;
    [SerializeField] private bool enableTextBouncyness = false;
    [SerializeField] private Transform narrationTextContainer = null;
    [SerializeField] private float bounceDistance = 25f;
    [SerializeField] private float bounceDuration = 0.5f;

    [Header("Buttons")]
    [SerializeField] private Button narrationNextParagraphButton = null;
    [SerializeField] private Button narrationPlayAgainButton = null;
    [SerializeField] private Button narrationFinishGameButton = null;
#endregion

#region Internal Vars
    private string textToWrite;
    private int charactedIdx;
    private float timer;
    private bool isBouncing = false;
    private Vector3 textboxStartPosition;
    private float timeElapsed;
    private bool bouncingUp = false;
    private float bouncingValueLerp;
    private bool isNarrationFinished = false;
    private bool isFinishedWriting = true;
#endregion

#region  Event Actions
    // button subscription events
    public static event Action nextParagraph;
    public static event Action playAgain;
    public static event Action finishGame;
    public static event Action writingStarted;
    public static event Action writingFinished;
#endregion

    // Start is called before the first frame update
    void Start()
    {
        if (narrationText == null)
            narrationText = GetComponentInChildren<TMP_Text>();
        if (narrationTextAudio == null)
            narrationTextAudio = GetComponent<AudioSource>(); // GetComponentInChildren<AudioSource>();

        textboxStartPosition = narrationTextContainer.transform.position;

        // hide finish / replay buttons 
        narrationFinishGameButton.gameObject.SetActive(false);
        narrationPlayAgainButton.gameObject.SetActive(false);

        // check all needed visual elements are visible
        narrationNextParagraphButton.gameObject.SetActive(true);

        // subscribe to the narration finished event
        AnimatedNarratorController.finishNarration += FinishNarration;
    }

    private void Update()
    {
        // once the text is written stop text updates and start bouncing :) 
        if (textToWrite == null || textToWrite.Length < charactedIdx)
        {
            BounceText();
            // pause playing narrator mumble audio (pause instead of stop makes short clips seem longer)
            if (narrationTextAudio.isPlaying)
                narrationTextAudio.Pause();

            if (!isFinishedWriting) {
                isFinishedWriting = true;
                writingFinished?.Invoke();
            }

            if (isNarrationFinished)
                UpdateNarrationFinishUI();
            return;
        }

        narrationTextContainer.transform.position = textboxStartPosition;

        // start playing narrator mumble audio
        if (!narrationTextAudio.isPlaying)
            narrationTextAudio.Play();

        // write all text at once if writing is disabled
        if (!enableTextWriting) {
            narrationText.text = textToWrite;
            charactedIdx = textToWrite.Length;
        }

        // check cooldown for typed character
        if (timer <= 0f)
        {
            // write next character & reset timer
            timer += timePerCharacter;
            narrationText.text = textToWrite.Substring(0, charactedIdx);
            charactedIdx += 1;
        }
        else
        {
            // cooldown not complete -> continue counting
            timer -= Time.deltaTime;
        }
    }
    private void BounceText()
    {
        if (!enableTextBouncyness) { return; }

        // Lerp textbox position either up or down depending on distance to end point (textboxStartPosition.y - bounceDistance)
        if (timeElapsed < bounceDuration)
        {
            if(bouncingUp) {
                bouncingValueLerp = Mathf.Lerp(textboxStartPosition.y - bounceDistance, textboxStartPosition.y, timeElapsed / bounceDuration);
                timeElapsed += Time.deltaTime;
            } else {
                bouncingValueLerp = Mathf.Lerp(textboxStartPosition.y, textboxStartPosition.y - bounceDistance, timeElapsed / bounceDuration);
                timeElapsed += Time.deltaTime;
            }

            // "bounce" container
            narrationTextContainer.transform.position = new Vector3(narrationTextContainer.transform.position.x, 
                                                                    bouncingValueLerp, 
                                                                    narrationTextContainer.transform.position.y);
        } else {
            bouncingUp = !bouncingUp;
            timeElapsed = 0f;
        }
    }
    private void UpdateNarrationFinishUI()
    {
        // show finish / replay buttons 
        narrationFinishGameButton.gameObject.SetActive(true);
        narrationPlayAgainButton.gameObject.SetActive(true);

        // hide next paragraph button
        narrationNextParagraphButton.gameObject.SetActive(false);
    }

#region Subscriptions Methods
    private void FinishNarration()
    {
        isNarrationFinished = true;
    }
#endregion

#region Public Methods
    public void WriteText(string newTextToWrite, float newTimePerCharacter)
    {
        textToWrite = newTextToWrite;
        timePerCharacter = newTimePerCharacter;
        charactedIdx = 0;
        // set text writing to "Instant" when speed reaches 0 
        if (timePerCharacter <= 0.0001f) {
            enableTextWriting = false;
        } else {
            enableTextWriting = true;
        }

        if (isFinishedWriting)
        {
            isFinishedWriting = false;
            writingStarted?.Invoke();
        }
    }

    // public void WriteText(string newTextToWrite)
    // {
    //     textToWrite = newTextToWrite;
    //     charactedIdx = 0;

    //     if (isFinishedWriting)
    //     {
    //         isFinishedWriting = false;
    //         writingStarted?.Invoke();
    //     }
    // }

    // public void SetNarrationSpeed(float newTimePerCharacter)
    // {
    //     timePerCharacter = newTimePerCharacter;
    // }
    #endregion

    #region Button Events
    public void NextParagraph() {
        if(isNarrationFinished) {
            narrationText.text = textToWrite;
            charactedIdx = textToWrite.Length;
            return;
        }
            
        nextParagraph?.Invoke();
    }
    public void PlayAgain()
    {
        // hide finish / replay buttons 
        narrationFinishGameButton.gameObject.SetActive(false);
        narrationPlayAgainButton.gameObject.SetActive(false);

        // show next paragraph button
        narrationNextParagraphButton.gameObject.SetActive(true);

        // reset finish narration
        isNarrationFinished = false;

        playAgain?.Invoke();
    }
    public void FinishGame()
    {
        /* v TODO - 4 editor testing - remove before prod for quit speedup v */
        // hide finish / replay buttons 
        narrationFinishGameButton.gameObject.SetActive(false);
        narrationPlayAgainButton.gameObject.SetActive(false);
        /* ^ TODO - 4 editor testing - remove before prod for quit speedup ^ */
        
        finishGame?.Invoke();
    }
#endregion

}
