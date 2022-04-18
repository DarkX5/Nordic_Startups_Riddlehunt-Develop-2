using System;
using UnityEngine;

namespace Riddlehouse.Core.Helpers.Helpers
{
    public interface IUIFitters
    {
        public void FitToFullscreen(RectTransform childRectTransform, RectTransform parent);
        public void FitToGlobalView(RectTransform childRectTransform);
    }
    public class UIFitters : IUIFitters
    {
        public void FitToFullscreen(RectTransform childRectTransform, RectTransform parent)
        {
            if (parent == null)
                throw new ArgumentException("parent was null, not allowed");
            if (childRectTransform == null)
                throw new ArgumentException("childRectTransform was null, not allowed");
            
            childRectTransform.SetParent(parent);
            childRectTransform.localPosition = Vector3.zero;
            childRectTransform.localScale = Vector3.one;
            childRectTransform.anchorMin = Vector2.zero;
            childRectTransform.anchorMax = Vector2.one;
            childRectTransform.offsetMin = Vector2.zero;
            childRectTransform.offsetMax = Vector2.zero;
        }

        public void FitToGlobalView(RectTransform childRectTransform)
        {
            if (childRectTransform == null)
                throw new ArgumentException("childRectTransform was null, not allowed");
            
            childRectTransform.SetParent(null);
            childRectTransform.localPosition = Vector3.zero;
            childRectTransform.localScale = Vector3.one;
            childRectTransform.anchorMin = Vector2.zero;
            childRectTransform.anchorMax = Vector2.one;
            childRectTransform.offsetMin = Vector2.zero;
            childRectTransform.offsetMax = Vector2.zero;
        }
    }
}