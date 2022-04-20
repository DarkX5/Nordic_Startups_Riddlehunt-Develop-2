using System;
using UnityEngine;

public class NarrationTextSettings : MonoBehaviour
{
    public static event Action increaseNarrationSpeed;
    public static event Action decreaseNarrationSpeed;

    private void Start() {
        AnimatedNarratorText.finishGame += FinishGame;
    }

    private void FinishGame() {
        gameObject.SetActive(false);
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
