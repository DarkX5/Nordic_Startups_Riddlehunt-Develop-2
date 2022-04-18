using System.Collections;
using System.Collections.Generic;
using Riddlehouse.Core.Helpers.Helpers;
using UnityEngine;

namespace RHPackages.Core.Scripts.UI
{
    public enum ComponentType { HuntHome, Story, Riddle, TextAnswer, MultipleChoiceText, Scanning, End, RiddleTab, Tab, NumericAnswer, Rating, Resolution, MultipleChoiceIconsVideoInAndOut, FullscreenView }
    public interface IViewActions
    {
        public void FitInView(RectTransform parent, IUIFitters uiFitters);
        public void FitInView(RectTransform parent, IUIFitters uiFitters, int index);
        public void Display();
        public void Hide();
        public bool IsShown();
        public ComponentType GetComponentType();
        public RectTransform GetRectTransform();
    }
}