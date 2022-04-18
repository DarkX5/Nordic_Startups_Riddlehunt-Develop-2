using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public interface IImageSlider
{
    public void Open(Action animationComplete = null);
    public void Close(Action animationComplete = null);
    public bool IsAnimating();
    public bool IsOpen();
}

public class ImageSliderFromBottomToTop : MonoBehaviour, IImageSlider
{
    [SerializeField] private RectTransform target;
    [SerializeField] private float top;
    [SerializeField] private AnimationCurve acIn;
    [SerializeField] private AnimationCurve acOut;
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
            StartCoroutine(DropIn(animationComplete));
    }

    public void Close(Action animationComplete = null)
    {
        if(!isAnimating && isOpen)
            StartCoroutine(DropOut(animationComplete));
    }

    public bool IsAnimating()
    {
        return isAnimating;
    }

    public bool IsOpen()
    {
        return isOpen;
    }

    private IEnumerator DropIn(Action animationComplete = null)
    {
        isAnimating = true;
        float curveTime = 0f;
        float curveAmount = acIn.Evaluate(curveTime);
        float movement = 0f;

        while (curveAmount < 1.0f)
        {
            curveTime += Time.deltaTime * animSpeedModifier;
            curveAmount = acIn.Evaluate(curveTime);
            movement = -target.rect.height + (target.rect.height * curveAmount);
            target.offsetMin = new Vector2(target.offsetMin.x, movement);
            target.offsetMax = new Vector2(target.offsetMax.x, movement-top);
            yield return _frameWaiter ?? new WaitForEndOfFrame();
        }

        movement = -target.rect.height + (target.rect.height * 1.0f);
        target.offsetMin = new Vector2(target.offsetMin.x, movement);
        target.offsetMax = new Vector2(target.offsetMax.x, movement-top);
        
        isOpen = true;
        isAnimating = false;
        animationComplete?.Invoke();
    }
    private IEnumerator DropOut(Action animationComplete = null)
    {
        isAnimating = true;

        float curveTime = 0f;
        float curveAmount = acOut.Evaluate(curveTime);
        float movement = 0f;

        while (curveAmount > 0.0f)
        {
            curveTime += Time.deltaTime * animSpeedModifier;
            curveAmount = acOut.Evaluate(curveTime);
            movement = -target.rect.height + (target.rect.height * curveAmount);
            target.offsetMin = new Vector2(target.offsetMin.x, movement);
            target.offsetMax = new Vector2(target.offsetMax.x, movement-top);
            yield return _frameWaiter ?? new WaitForEndOfFrame();
        }

        isOpen = false;
        isAnimating = false;
        animationComplete?.Invoke();
    }
}
