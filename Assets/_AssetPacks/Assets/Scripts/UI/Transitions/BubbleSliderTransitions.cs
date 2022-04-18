using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBubbleSliderTransitions
{
    public void Configure(BubbleSliderTransitions.Dependencies dependencies);
    public void StartTransition(RectTransform targetTransform, Vector2 lockPoint, Action transitionComplete);
}
public class BubbleSliderTransitions : MonoBehaviour, IBubbleSliderTransitions
{
    public class Dependencies
    {
        public AnimationCurve Ac = AnimationCurve.EaseInOut(0,0,1,1);
        public float AnimSpeedModifier = 1f;
        public float DistanceThreshold = 10f;
    }

    private Dependencies _dependencies;
    public void Configure(BubbleSliderTransitions.Dependencies dependencies)
    {
        _dependencies = dependencies;
    }

    public void StartTransition(RectTransform targetTransform, Vector2 lockPoint, Action transitionComplete)
    {
        StartCoroutine(FlowtoPoint(targetTransform, lockPoint, transitionComplete));
    }

    private IEnumerator FlowtoPoint(RectTransform targetTransform, Vector2 lockPoint, Action transitionComplete)
    {
        float curveTime = 0f;
        var distance = _dependencies.DistanceThreshold + 1f;

        while (distance > _dependencies.DistanceThreshold)
        {
            curveTime += Time.deltaTime * _dependencies.AnimSpeedModifier;
            float curveAmount = _dependencies.Ac.Evaluate(curveTime);

            var targetPosition = targetTransform.anchoredPosition;

            distance = Vector2.Distance(new Vector2(lockPoint.x, 0f),
                new Vector2(targetPosition.x, 0f));

            var movedDistance = distance * curveAmount;

            bool shouldMoveLeft = targetPosition.x > lockPoint.x;

            if (shouldMoveLeft)
            {
                targetTransform.anchoredPosition = CalcLeft(targetPosition.x, movedDistance, targetPosition.y);
            }
            else
            {
                targetTransform.anchoredPosition = CalcRight(targetPosition.x, movedDistance, targetPosition.y);
            }

            yield return new WaitForEndOfFrame();
        }

        targetTransform.anchoredPosition = new Vector2(lockPoint.x, targetTransform.anchoredPosition.y);
        targetTransform.localScale = Vector3.one;
        transitionComplete.Invoke();
    }
    public Vector2 CalcRight(float videoX, float moveDistance, float videoY)
    {
        return new Vector2(videoX + moveDistance, videoY);
    }

    public Vector2 CalcLeft(float videoX, float moveDistance, float videoY)
    {
        return new Vector2(videoX - moveDistance, videoY);
    }
    
}
