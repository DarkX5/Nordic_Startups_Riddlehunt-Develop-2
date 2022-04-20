// using System;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedNarratorController : MonoBehaviour
{
    
#region  Serialized Fields
    [Header("Narration Data")]
    [SerializeField] private Canvas narrationCanvas = null;
    [SerializeField] private GameObject narratorPrefab = null;
    [SerializeField] private GameObject narratorTextboxPrefab = null;
    [Tooltip("How much should the character writing speed increase / decrease for each button click")]
    [SerializeField] private float narrationTextSpeedStep = 0.025f;
#endregion

#region  Internal Vars
    private AnimatedNarratorImage narratorImage = null;
    private AnimatedNarratorText narratorText = null;
    private List<string> textContents = null;
    private int paragraphIdx;
    private Vector2 narrationTextSpeedMinMax = new Vector2(0f, 1f);
    private float textAnimationSpeed;
#endregion

    public static event Action finishNarration;

    private void Start()
    {
        // generate narrator & textbox
        narratorImage = (Instantiate(narratorPrefab, narrationCanvas.transform) as GameObject).transform.GetComponent<AnimatedNarratorImage>(); // FindObjectOfType<AnimatedNarratorImage>();
        narratorText = (Instantiate(narratorTextboxPrefab, narrationCanvas.transform) as GameObject).transform.GetComponent<AnimatedNarratorText>(); // FindObjectOfType<AnimatedNarratorText>();

        // get & set narration data
        SetNarrationTextsAndSpeed(NarrationTextInitializer.Instance.GetMissionNarration(), NarrationTextInitializer.Instance.GetTextAnimationSpeed());

        // check all needed visual elements are visible
        narratorImage.gameObject.SetActive(true);
        narratorText.gameObject.SetActive(true);
        
        // subscribe to narrator text button events
        AnimatedNarratorText.nextParagraph += NextParagraph;
        AnimatedNarratorText.playAgain += PlayAgain;
        AnimatedNarratorText.finishGame += FinishGame;

        // subscribe to narration text speed button events
        NarrationTextSettings.increaseNarrationSpeed += IncreaseNarrationSpeed;
        NarrationTextSettings.decreaseNarrationSpeed += DecreaseNarrationSpeed;

        paragraphIdx = 0;
        // write first paragraph
        NextParagraph();
    }

    private void FinishNarration()
    {
        finishNarration?.Invoke();
    }
    private void IncreaseNarrationSpeed()
    {
        if (textAnimationSpeed < narrationTextSpeedMinMax.y)
        {
            textAnimationSpeed += narrationTextSpeedStep;
        }
        else
        {
            // set textAnimationSpeed to max value
            textAnimationSpeed = narrationTextSpeedMinMax.y;
        }
    }
    private void DecreaseNarrationSpeed()
    {
        if (textAnimationSpeed > narrationTextSpeedMinMax.x)
        {
            textAnimationSpeed -= narrationTextSpeedStep;
        }
        else
        {
            // set textAnimationSpeed to min value
            textAnimationSpeed = narrationTextSpeedMinMax.x;
        }
    }

#region Get Data Methods
    public void SetNarrationTexts(List<string> newTextContentList)
    {
        textContents = newTextContentList;
    }
    public void SetNarrationTextsAndSpeed(List<string> newTextContentList, float newTextAnimationSpeed)
    {
        textContents = newTextContentList;
        textAnimationSpeed = newTextAnimationSpeed;
    }
#endregion

#region UI Called Methods
    public void NextParagraph()
    {
        if (textContents == null || paragraphIdx >= textContents.Count) { return; }

        narratorText.WriteText(textContents[paragraphIdx], textAnimationSpeed);
        paragraphIdx += 1;

        // activate finish narratio UI when reaching the last paragraph 
        if (paragraphIdx >= textContents.Count)
        {
            FinishNarration();
        }
    }

    public void FinishGame()
    {
        /* v TODO - 4 editor testing - remove before prod for quit speedup v */
        narratorImage.gameObject.SetActive(false);
        narratorText.gameObject.SetActive(false);
        /* ^ TODO - 4 editor testing - remove before prod for quit speedup ^ */

        Application.Quit();
    }


    public void PlayAgain()
    {
        paragraphIdx = 0;
        NextParagraph();
    }
#endregion

}
