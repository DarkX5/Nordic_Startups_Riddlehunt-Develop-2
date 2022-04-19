using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedNarratorController : MonoBehaviour
{
    [Header("Narration Data")]
    [SerializeField] private AnimatedNarratorImage narratorImage = null;
    [SerializeField] private AnimatedNarratorText narratorText = null;

    [Header("Buttons")]
    [SerializeField] private Button narrationNextParagraphButton = null;
    [SerializeField] private Button narrationPlayAgainButton = null;
    [SerializeField] private Button narrationFinishGameButton = null;
    // [SerializeField] private GameObject narrationButtonsContainer = null;

    [Header("DEBUG vars")]
    [SerializeField] private List<string> textContents = null;
    [SerializeField] private int paragraphIdx;

    private void Start()
    {
        if (narratorImage == null)
            narratorImage = FindObjectOfType<AnimatedNarratorImage>();
        if (narratorText == null)
            narratorText = FindObjectOfType<AnimatedNarratorText>();

        // get mission text data
        SetNarrationTextsAndSpeed(NarrationTextInitializer.Instance.GetMissionNarration(), NarrationTextInitializer.Instance.GetTextAnimationSpeed());

        // hide finish / replay buttons 
        narrationFinishGameButton.gameObject.SetActive(false);
        narrationPlayAgainButton.gameObject.SetActive(false);

        // check all needed visual elements are visible
        narratorImage.gameObject.SetActive(true);
        narratorText.gameObject.SetActive(true);
        narrationNextParagraphButton.gameObject.SetActive(true);

        paragraphIdx = 0;
        // write first paragraph
        NextParagraph();
    }

    private void FinishNarration()
    {
        // show finish / replay buttons 
        narrationFinishGameButton.gameObject.SetActive(true);
        narrationPlayAgainButton.gameObject.SetActive(true);

        // hide next paragraph button
        narrationNextParagraphButton.gameObject.SetActive(false);
    }

#region Get Data Methods
    public void SetNarrationTexts(List<string> newTextContentList)
    {
        textContents = newTextContentList;
    }
    public void SetNarrationTextsAndSpeed(List<string> newTextContentList, float newTextAnimationSpeed)
    {
        textContents = newTextContentList;
        narratorText.SetNarrationSpeed(newTextAnimationSpeed);
        // textAnimationSpeed = newTextAnimationSpeed;
    }
#endregion

    #region UI Called Methods
    public void NextParagraph()
    { // called from UI 
        if (textContents == null || textContents.Count < 1) { return; }

        narratorText.WriteText(textContents[paragraphIdx]);
        paragraphIdx += 1;

        // activate finish narratio UI when reaching the last paragraph 
        if (paragraphIdx >= textContents.Count)
        {
            FinishNarration();
        }
    }

    public void FinishGame()
    { // called from UI 
        /* v TODO - 4 editor testing - remove before prod for quit speedup v */
        narratorImage.gameObject.SetActive(false);
        narratorText.gameObject.SetActive(false);

        // hide finish / replay buttons 
        narrationFinishGameButton.gameObject.SetActive(false);
        narrationPlayAgainButton.gameObject.SetActive(false);
        /* ^ TODO - 4 editor testing - remove before prod for quit speedup ^ */

        Application.Quit();
    }


    public void PlayAgain()
    { // called from UI 
        // hide finish / replay buttons 
        narrationFinishGameButton.gameObject.SetActive(false);
        narrationPlayAgainButton.gameObject.SetActive(false);

        // show next paragraph button
        narrationNextParagraphButton.gameObject.SetActive(true);

        paragraphIdx = 0;
        NextParagraph();
    }
    #endregion

}
