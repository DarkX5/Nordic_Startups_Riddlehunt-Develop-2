using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultipleChoice.Icon.Transitions
{
    public interface IMultipleChoiceIconStateTransitions
    {
        public void Configure(MultipleChoiceIconStateTransitions.Config config);
        public void StartAnimateSelected(Action setSelected);
        public void StartAnimateDeselected(Action setDeselected);
        public void StartAnimateFlash(Action completed, Color highlightColor);
        public bool IsAnimating();
    }
    public class MultipleChoiceIconStateTransitions : MonoBehaviour, IMultipleChoiceIconStateTransitions
    {
        public class Config
        {
            public AnimationCurve Ac { get; set; }
            public float AnimSpeedModifier { get; set; }
            public float AnimThreshold { get; set; }
            public Image IconBackground { get; set; }
            public Color IdleColor { get; set; }
            public Color SelectedColor { get; set; }
        }

        private bool _isAnimating = false;
        private Config _config;
        public void Configure(Config config)
        {
            _config = config;
        }

        public void StartAnimateFlash(Action completed, Color highlightColor)
        {
            _config.IconBackground.color = _config.IdleColor;

            StartCoroutine(AnimateToColor(() => //begin first flash going to highlightColor //red or green
            {
                //Handheld.Vibrate();
                StartCoroutine(AnimateToColor(() =>//end first flash going to idleColor //white
                {
                    StartCoroutine(AnimateToColor(() => //begin second flash going to highlightColor //red or green
                    {
                        //Handheld.Vibrate();
                        StartCoroutine(AnimateToColor(completed, _config.IdleColor, _config.AnimSpeedModifier/2f)); //end second flash going to endColor //white
                    },  highlightColor, _config.AnimSpeedModifier/2f));
                }, _config.IdleColor, _config.AnimSpeedModifier/2f));
            }, highlightColor, _config.AnimSpeedModifier/2f));
        }
        
        public void StartAnimateSelected(Action setSelected)
        {
            StartCoroutine(AnimateToColor(setSelected, _config.SelectedColor, _config.AnimSpeedModifier));
        }

        public void StartAnimateDeselected(Action setDeselected)
        {
            StartCoroutine(AnimateToColor(setDeselected, _config.IdleColor, _config.AnimSpeedModifier));
        }

        public bool IsAnimating()
        {
            return _isAnimating;
        }

        private IEnumerator AnimateToColor(Action completed, Color toColor, float animationSpeed)
        {
            _isAnimating = true;
            float curveTime = 0f;
            var colorVector = new Vector3(_config.IconBackground.color.r, _config.IconBackground.color.g, _config.IconBackground.color.b);
            var selectedVector = new Vector3(toColor.r, toColor.g, toColor.b);
            var distance = Vector3.Distance(colorVector,selectedVector);
            while (distance > _config.AnimThreshold)
            {
                curveTime += Time.deltaTime * animationSpeed;
                float curveAmount = _config.Ac.Evaluate(curveTime);
                colorVector = Vector3.Lerp(colorVector, selectedVector, 1f*curveAmount);
                _config.IconBackground.color = new Color(colorVector.x, colorVector.y, colorVector.z,
                    _config.IconBackground.color.a);
                distance = Vector3.Distance(colorVector,selectedVector);
                yield return new WaitForEndOfFrame();
            }
            
            _config.IconBackground.color = toColor;
            _isAnimating = false;
            completed();
        }
    }
}