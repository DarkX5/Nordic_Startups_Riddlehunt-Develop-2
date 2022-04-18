using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface ILayoutElementVerticalResizerWithAnimation
{
    public void StartUIUpdate(float oldHeight, float newHeight, bool open);
    public void Configure(LayoutElementVerticalResizerWithAnimation.Config config);
}
public class LayoutElementVerticalResizerWithAnimation : MonoBehaviour, ILayoutElementVerticalResizerWithAnimation
{
    public class Config
    {
        public AnimationCurve AcIn { get; set; }
        public float AcInThreshold { get; set; }
        public AnimationCurve AcOut { get; set; }
        public float AcOutThreshold { get; set; }

        public LayoutElement ResizingLayoutElement { get; set; } //BodyLayoutElement
        public LayoutElement LayoutOwner { get; set; } //CardLayoutelement
        public RectTransform LayoutContentOwner { get; set; } //top level in the ui hierarchy
        public float AnimSpeedModifier { get; set; }
        public List<RectTransform> StaticElements { get; set; } //All non resizing elements

    }

    public Config _config;
    public void Configure(Config config)
    {
        _config = config;
    }
    
    public void StartUIUpdate(float oldHeight, float newHeight, bool open)
    {
        if(open)
            StartCoroutine(AnimateCardOpen(oldHeight, newHeight));
        else 
            StartCoroutine(AnimateCardClose(oldHeight, newHeight));
    }
    
    private IEnumerator AnimateCardOpen(float oldHeight, float newHeight)
    {
        float curveTime = 0f;
        float curveAmount = _config.AcIn.Evaluate(curveTime);
        float step = 0f;
        float diffBetweenOldNew = newHeight - oldHeight;
        while (curveAmount < _config.AcInThreshold)
        {
            curveTime += Time.fixedDeltaTime * _config.AnimSpeedModifier;
            curveAmount = _config.AcIn.Evaluate(curveTime);
            float calcStepHeight =  oldHeight+(diffBetweenOldNew * curveAmount);
            step = Mathf.Clamp(calcStepHeight, oldHeight, newHeight);

            var heightToAdd = 0f;
            foreach (var element in _config.StaticElements)
            {
                heightToAdd += element.rect.height;
            }
            _config.ResizingLayoutElement.preferredHeight = calcStepHeight-heightToAdd; //static sizes

            _config.LayoutOwner.preferredHeight = step; //cardlayoutelement
            LayoutRebuilder.ForceRebuildLayoutImmediate(_config.LayoutContentOwner); //content element
            yield return new WaitForFixedUpdate();
        }
        _config.LayoutOwner.preferredHeight = newHeight;
    }
    private IEnumerator AnimateCardClose(float oldHeight, float newHeight)
    {
        float curveTime = 0f;
        float curveAmount = _config.AcOut.Evaluate(curveTime);
        float step = 0f;
        float diffBetweenOldNew = oldHeight - newHeight;

        while (curveAmount > _config.AcOutThreshold)
        {
            curveTime += Time.fixedDeltaTime * _config.AnimSpeedModifier;
            curveAmount = _config.AcOut.Evaluate(curveTime);
            float calcStepHeight =  newHeight+(diffBetweenOldNew * curveAmount);
            step = Mathf.Clamp(calcStepHeight, newHeight, oldHeight);
            var heightToAdd = 0f;
            foreach (var element in _config.StaticElements)
            {
                heightToAdd += element.rect.height;
            }
            _config.ResizingLayoutElement.preferredHeight = calcStepHeight-heightToAdd;
            
            _config.LayoutOwner.preferredHeight = step;
            LayoutRebuilder.ForceRebuildLayoutImmediate(_config.LayoutContentOwner);
            yield return new WaitForFixedUpdate();
        }
        _config.LayoutOwner.preferredHeight = newHeight;
    }
}
