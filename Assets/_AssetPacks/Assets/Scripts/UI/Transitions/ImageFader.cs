using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IImageFaderActions
{
    public void Open(Action animationComplete = null);
    public void Close(Action animationComplete = null);
    public bool IsAnimating();
    public bool IsOpen();

}
public class ImageFader : MonoBehaviour, IImageFaderActions
{
    [SerializeField] private AnimationCurve acIn;
    [SerializeField] private AnimationCurve acOut;
    [SerializeField] private float alpha;
    [SerializeField] private Image imageTofade;
    [SerializeField] private float animSpeedModifier;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private bool isAnimating = false;
    private WaitForEndOfFrame _frameWaiter;

    public void Awake()
    {
        _frameWaiter = new WaitForEndOfFrame();
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
    
    public bool IsAnimating()
    {
        return isAnimating;
    }

    public bool IsOpen()
    {
        return isOpen;
    }
    
    private IEnumerator FadeIn(Action animationComplete = null)
    {
        isAnimating = true;

        float curveTime = 0f;
        float curveAmount = acIn.Evaluate(curveTime);
        while (curveAmount < 1.0f)
        {
            curveTime += Time.deltaTime * animSpeedModifier;
            curveAmount = acIn.Evaluate(curveTime);
            imageTofade.color = new Color(imageTofade.color.r, imageTofade.color.g, imageTofade.color.b, Mathf.Clamp(curveAmount, 0, alpha));
            yield return _frameWaiter ?? new WaitForEndOfFrame();
        }
        imageTofade.color = new Color(imageTofade.color.r, imageTofade.color.g, imageTofade.color.b, alpha);
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
            imageTofade.color = new Color(imageTofade.color.r, imageTofade.color.g, imageTofade.color.b, Mathf.Clamp(curveAmount, 0, alpha));
            yield return _frameWaiter ?? new WaitForEndOfFrame();
        }
        imageTofade.color = new Color(imageTofade.color.r, imageTofade.color.g, imageTofade.color.b, 0.0f);
       
        isOpen = false;
        isAnimating = false;
        animationComplete?.Invoke();
    }
}
