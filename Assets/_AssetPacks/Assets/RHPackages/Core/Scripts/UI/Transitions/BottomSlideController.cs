using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BottomSlideController : MonoBehaviour
{
    [FormerlySerializedAs("_transform")] [SerializeField] private RectTransform target;
    [SerializeField] private AnimationCurve acIn;
    [SerializeField] private AnimationCurve acOut;
    [SerializeField] private float animSpeedModifier;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private bool isAnimating = false;
    private WaitForEndOfFrame _frameWaiter;

    [SerializeField] private RectTransform footer;
    [SerializeField] private float Min;
    [SerializeField] private float Max;
    [SerializeField] private float padding;
    public void Start()
    {
        _frameWaiter = new WaitForEndOfFrame();
        Max = ((RectTransform)target.parent).rect.height - padding;
        Min = footer.sizeDelta.y + padding;

        target.anchorMin = new Vector2(0, 0);
        target.anchorMax = new Vector2(1, 0);
        target.pivot = new Vector2(0.5f, 0);
        
        target.sizeDelta = new Vector2(-(padding*2), Min);
    }

    public void Clicked()
    {
        if (!isAnimating)
        {
            if (isOpen)
                Close();
            else
                Open();
        }
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
        while (target.sizeDelta.y < Max)
        {
            curveTime += Time.deltaTime * animSpeedModifier;
            curveAmount = acIn.Evaluate(curveTime);
            
            var newHeight = Min+ (curveAmount * Max);
            newHeight = Mathf.Clamp(newHeight, Min, Max+5f);
            target.sizeDelta = new Vector2(target.sizeDelta.x, newHeight);
            
            yield return _frameWaiter ?? new WaitForEndOfFrame();
        }
        target.sizeDelta = new Vector2(target.sizeDelta.x, Max);

        isOpen = true;
        isAnimating = false;
        animationComplete?.Invoke();
    }
    private IEnumerator DropOut(Action animationComplete = null)
    {
        isAnimating = true;

        float curveTime = 0f;
        float curveAmount = acOut.Evaluate(curveTime);

        while (target.sizeDelta.y > Min)
        {
            curveTime += Time.deltaTime * animSpeedModifier;
            curveAmount = acOut.Evaluate(curveTime);
            var newHeight = curveAmount * Max;
            newHeight = Mathf.Clamp(newHeight, Min-5f, Max);
            target.sizeDelta = new Vector2(target.sizeDelta.x, newHeight);
            yield return _frameWaiter ?? new WaitForEndOfFrame();
        }
        target.sizeDelta = new Vector2(target.sizeDelta.x, Min);

        isOpen = false;
        isAnimating = false;
        animationComplete?.Invoke();
    }
}
