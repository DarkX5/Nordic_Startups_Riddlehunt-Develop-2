using System;
using UnityEngine;
using TMPro;

public class NarrationTextSettings : MonoBehaviour
{
    [SerializeField] private TMP_Text speedTextOutput = null; 

#region  Event Actions
    public static event Action increaseNarrationSpeed;
    public static event Action decreaseNarrationSpeed;
#endregion

    private void Start() {
        AnimatedNarratorText.finishGame += FinishGame;
        AnimatedNarratorController.writingSpeedValueUpdate += UpdateSpeedUIText;

        if(speedTextOutput == null) {
            var textList = transform.GetComponentsInChildren<TMP_Text>();
            speedTextOutput = textList[textList.Length - 1];
        }
    }

    private void FinishGame() {
        gameObject.SetActive(false);
    }
    private void UpdateSpeedUIText(int newValue)
    {
        // update the ui displaying speed - in case of 0 (zero) or below -> show "Instant" instead
        speedTextOutput.text = newValue <= 0 ? "Instant" : newValue.ToString();
    }

    public void IncreaseNarrationSpeed()
    { // called from UI
        increaseNarrationSpeed?.Invoke();
    }
    public void DecreaseNarrationSpeed()
    { // called from UI
        decreaseNarrationSpeed?.Invoke();
    }
}
