using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public interface IBubbleDropController
{
    public void OnDrop(PointerEventData eventData);
    public void DropHovering();
    public void DropStopHovering();
    public RectTransform GetRectTransform();
    public void DeactivateUI();
    public void ActivateUI();

}
[RequireComponent(typeof(RectTransform))]
public class BubbleDropController : MonoBehaviour, IDropHandler, IBubbleDropController
{
    public static IBubbleDropController Factory(BubbleDropController prefab, RectTransform parent, BubbleSlider slider, BubbleSlideLockType type)
    {
        if (BubbleSlideLockType.none == type) throw new ArgumentException("Not allowed");
        
        var behaviour = Instantiate(prefab, parent).GetComponent<BubbleDropController>();
        behaviour.Configure(new Dependencies()
        {
            AssociatedBubbleSlider = slider,
            lockType = type
        });
        behaviour.PositionSelf();
        return behaviour;
    }
    public class Dependencies
    {
        public IBubbleSlider AssociatedBubbleSlider { get; set; }
        public BubbleSlideLockType lockType { get; set; }
    }

    public Dependencies _dependencies { get; set; }
    private RectTransform _rt;
    public void Configure(Dependencies dependencies)
    {
        _dependencies = dependencies;
        _rt = GetComponent<RectTransform>();
    }

    public void PositionSelf()
    {
        if (_dependencies.lockType == BubbleSlideLockType.none)
            throw new ArgumentException("not allowed");
        
        if (_dependencies.lockType == BubbleSlideLockType.left)
        {
            _rt.anchorMin = new Vector2(0f,0f);
            _rt.anchorMax = new Vector2(0f, 1f);
            _rt.pivot = new Vector2(0f, 0.5f);
            
            _rt.offsetMin =
                new Vector2(0, 0);
            _rt.sizeDelta = new Vector2(164, _rt.sizeDelta.y);
        }
        else if (_dependencies.lockType == BubbleSlideLockType.right)
        {
            _rt.anchorMin = new Vector2(1f,0f);
            _rt.anchorMax = new Vector2(1f, 1f);
            _rt.pivot = new Vector2(1f, 0.5f);
            
            _rt.offsetMin =
                new Vector2(-164, 0);
            _rt.offsetMax =
                new Vector2(0, 0);

            _rt.sizeDelta = new Vector2(164, _rt.sizeDelta.y);
        }
        _rt.SetSiblingIndex(0);
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        if(_dependencies.lockType == BubbleSlideLockType.left)
            _dependencies.AssociatedBubbleSlider.LockBubbleToLeft();
        if(_dependencies.lockType == BubbleSlideLockType.right)
            _dependencies.AssociatedBubbleSlider.LockBubbleToRight();
    }

    public void DropHovering()
    {
        _dependencies.AssociatedBubbleSlider.StartDropHighlight();
    }

    public void DropStopHovering()
    {
        _dependencies.AssociatedBubbleSlider.StopDropHighlight();
    }

    public RectTransform GetRectTransform()
    {
        return _rt;
    }

    public void DeactivateUI()
    {
        this.gameObject.SetActive(false);
    }

    public void ActivateUI()
    {
        this.gameObject.SetActive(true);
    }
}
