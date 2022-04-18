using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface ICanvasGroupFader
{
    public void Open(Action animationComplete = null);
    public void Close(Action animationComplete = null);
    public bool IsAnimating();
    public bool IsOpen();
    public void ForceClosed();
    public void SetInteractable(bool interactable);

}
public class CanvasGroupFader: MonoBehaviour, ICanvasGroupFader
{
    [SerializeField] private AnimationCurve acIn;
    [SerializeField] private AnimationCurve acOut;
    [SerializeField] private float alpha;
    [SerializeField] private CanvasGroup valueToFade;
    [SerializeField] private float animSpeedModifier;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private bool isAnimating = false;
    private WaitForEndOfFrame _frameWaiter;

    private bool _interactable;

    public void Awake()
    {
        _frameWaiter = new WaitForEndOfFrame();
    }

    public void SetInteractable(bool interactable)
    {
        _interactable = interactable;
        valueToFade.interactable = _interactable;
    }
    
    public void Open(Action animationComplete = null)
    {
        if(!isAnimating && !isOpen)
            StartCoroutine(FadeIn(animationComplete));
    }

    public void Close(Action animationComplete = null)
    {
        if(!isAnimating && isOpen)
            StartCoroutine(FadeOut(animationComplete));
    }
    public void ForceClosed()
    {
        RepeatingEventValue.Invoke(0f);
        valueToFade.alpha = 0;
        valueToFade.interactable = false;
        valueToFade.blocksRaycasts = false;
        isOpen = false;
        isAnimating = false;
    }
    public bool IsAnimating()
    {
        return isAnimating;
    }

    public bool IsOpen()
    {
        return isOpen;
    }

    public Action<float> RepeatingEventValue = (value) => {};

    private IEnumerator FadeIn(Action animationComplete = null)
    {
        isAnimating = true;

        float curveTime = 0f;
        float curveAmount = acIn.Evaluate(curveTime);
        while (curveAmount < 1.0f)
        {
            curveTime += Time.deltaTime * animSpeedModifier;
            curveAmount = acIn.Evaluate(curveTime);
            RepeatingEventValue.Invoke(curveAmount);
            valueToFade.alpha = Mathf.Clamp(curveAmount, 0, alpha);
            yield return _frameWaiter ?? new WaitForEndOfFrame();
        }

        RepeatingEventValue.Invoke(1f);
        valueToFade.alpha = alpha;
        valueToFade.interactable = _interactable;
        valueToFade.blocksRaycasts = true;
        isOpen = true;
        isAnimating = false;
        animationComplete?.Invoke();
    }

    private IEnumerator FadeOut(Action animationComplete = null)
    {
        isAnimating = true;

        float curveTime = 0f;
        float curveAmount = acOut.Evaluate(curveTime);

        while (curveAmount > 0.0f)
        {
            curveTime += Time.deltaTime * animSpeedModifier;
            curveAmount = acOut.Evaluate(curveTime);
            RepeatingEventValue.Invoke(curveAmount);
            valueToFade.alpha = Mathf.Clamp(curveAmount, 0, alpha);
            yield return _frameWaiter ?? new WaitForEndOfFrame();
        }

        ForceClosed();
        animationComplete?.Invoke();
    }
}
