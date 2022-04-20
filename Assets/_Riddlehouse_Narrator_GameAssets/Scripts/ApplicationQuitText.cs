using TMPro;
using UnityEngine;

public class ApplicationQuitText : MonoBehaviour
{
    private TMP_Text text;
    private void Start()
    {
        if (text == null)
            text = GetComponent<TMP_Text>();

        AnimatedNarratorText.finishGame += FinishGame;
    }

    private void FinishGame()
    {
        text.enabled = true;
    }
}
