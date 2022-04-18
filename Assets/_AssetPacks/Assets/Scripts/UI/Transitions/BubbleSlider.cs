using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public interface IBubbleSlider
{
    public void SetDependencies(BubbleSlider.Dependencies dependencies);
    public void Configure(Canvas cv);
    public void LockBubbleToLeft();
    public void LockBubbleToRight();
    public void OpenBubble();
    public void StartDropHighlight();
    public void StopDropHighlight();
    public void OnBeginDrag(PointerEventData eventData);
    public void OnEndDrag(PointerEventData eventData);
    public void OnDrag(PointerEventData eventData);
}

public enum BubbleSlideLockType { none, left, right }
[RequireComponent(typeof(BubbleSliderTransitions))]
[RequireComponent(typeof(CanvasGroup))]
public class BubbleSlider : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IBubbleSlider
{
    public class Dependencies
    {
        public IBubbleSliderTransitions Transitions { get; set; }
        public RectTransform LeftLockPoint { get; set; }
        public RectTransform RightLockPoint { get; set; }
        public RectTransform TargetTransform { get; set; }
        public AnimationCurve Ac { get; set; }
        public CanvasGroup Cg { get; set; }
    }

    public void SetDependencies(Dependencies dependencies)
    {
        _dependencies = dependencies;
        var transitions = dependencies.Transitions ?? GetComponent<BubbleSliderTransitions>();
        transitions.Configure(new BubbleSliderTransitions.Dependencies()
        {
            AnimSpeedModifier = AnimSpeedModifier,
            DistanceThreshold = DistanceThreshold
        });
        _transitions = transitions;
    }

    private IBubbleSliderTransitions _transitions;
    public Dependencies _dependencies { get; private set; }
    public Canvas ViewCanvas { get; private set; }
    public float AnimSpeedModifier { get; private set; } = 1f;
    public float DistanceThreshold { get; private set; } = 10f;
    public bool IsAnimating  { get; private set; } = false;
    private BubbleSlideLockType _currentLockType = BubbleSlideLockType.none;
    
    public BubbleSlideLockType GetLockType()
    {
        return _currentLockType;
    }

    public void Configure(Canvas cv)
    {
        ViewCanvas = cv;
    }

    public void LockBubbleToLeft()
    {
        if (!IsAnimating)
        {
            _currentLockType = BubbleSlideLockType.left;
            var width = _dependencies.LeftLockPoint.rect.width - _dependencies.TargetTransform.rect.width;
            StartTransition(new Vector2(width, _dependencies.TargetTransform.anchoredPosition.y));
        }
    }

    public void LockBubbleToRight()
    {
        if (!IsAnimating)
        {
            _currentLockType = BubbleSlideLockType.right;
            var width = _dependencies.TargetTransform.rect.width - _dependencies.RightLockPoint.rect.width;
            StartTransition(new Vector2(width, _dependencies.TargetTransform.anchoredPosition.y));
        }
    }

    public void OpenBubble()
    {
        if (!IsAnimating)
        {
            if (_currentLockType == BubbleSlideLockType.none)
            {
                return;
            }
            else if (_currentLockType == BubbleSlideLockType.left || _currentLockType == BubbleSlideLockType.right)
            {
                _currentLockType = BubbleSlideLockType.none;
                StartTransition(new Vector2(0, 0));
            }
        }
    }

    private void StartTransition(Vector2 lockPoint)
    {
        IsAnimating = true;
        highlighting = false;
        _transitions.StartTransition(_dependencies.TargetTransform, lockPoint, () =>
        {
            IsAnimating = false;
        });            
    }

    [SerializeField] private bool highlighting = false;
    [SerializeField] private bool dragging = false;

    public void StartDropHighlight()
    {
        if (dragging)
        {
            highlighting = true;
        }
    }

    public void StopDropHighlight()
    {
        if (dragging)
        {
            highlighting = false;
        }
    }

    public void Update()
    {
        if(_dependencies != null) {
            if (highlighting)
            {
                if (_dependencies.TargetTransform.localScale.x > 0.8f)
                    _dependencies.TargetTransform.localScale = Vector3.Lerp(_dependencies.TargetTransform.localScale,
                        new Vector3(0.7f, 0.7f, 0.7f),
                        5f * Time.deltaTime);
                else if (Math.Abs(_dependencies.TargetTransform.localScale.x - 0.8f) > 0.05f)
                    _dependencies.TargetTransform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            }
            else
            {
                if (_dependencies.TargetTransform.localScale.x < 1f)
                    _dependencies.TargetTransform.localScale = Vector3.Lerp(_dependencies.TargetTransform.localScale,
                        new Vector3(1.1f, 1.1f, 1.1f),
                        5f * Time.deltaTime);
                else if (Math.Abs(_dependencies.TargetTransform.localScale.x - 1f) > 0.05f)
                    _dependencies.TargetTransform.localScale = Vector3.one;
            }
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
        _currentLockType = BubbleSlideLockType.none;
        _dependencies.Cg.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
        _dependencies.Cg.blocksRaycasts = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _dependencies.TargetTransform.anchoredPosition += eventData.delta / ViewCanvas.scaleFactor;
    }
    
}