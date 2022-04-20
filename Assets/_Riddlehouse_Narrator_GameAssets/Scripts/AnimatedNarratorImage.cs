using UnityEngine;

public class AnimatedNarratorImage : MonoBehaviour
{
    [SerializeField] private string animatorBoolParameterName = "narratorIsBreathing";
    Animator narratorAnimator = null;
    bool narratorIsBreathing = false;

    // Start is called before the first frame update
    void Start()
    {
        narratorAnimator = GetComponentInChildren<Animator>();

        // subscribe to text events
        AnimatedNarratorText.writingStarted += StartBreathing;
        AnimatedNarratorText.writingFinished += StopBreathing;
    }

    private void StartBreathing() {
        narratorAnimator?.SetBool(animatorBoolParameterName, true);
    }
    private void StopBreathing()
    {
        narratorAnimator?.SetBool(animatorBoolParameterName, false);
    }

}
