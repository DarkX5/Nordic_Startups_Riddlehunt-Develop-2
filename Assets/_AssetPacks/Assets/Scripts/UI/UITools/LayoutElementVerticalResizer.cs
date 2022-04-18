using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LayoutElementVerticalResizer : MonoBehaviour, ILayoutElementVerticalResizer
{
    [SerializeField] private List<LayoutElement> definedElements;
    [SerializeField] private RectTransform contentTarget; 
    [SerializeField] private LayoutElement resizingLayoutElement;

    public void StartUIUpdate()
    {
        StartCoroutine(AwaitUIChange());
    }
    
    private float CalculateDefinedHeight()
    {
        var definedHeight = 0f;
        foreach (var element in definedElements)
        {
            definedHeight += element.preferredHeight;
        }
        return definedHeight;
    }

    private IEnumerator AwaitUIChange()
    {
        var descriptionHeight = contentTarget.rect.height;
        while (Math.Abs(descriptionHeight - contentTarget.rect.height) < 0.15f)
        {
            yield return new WaitForFixedUpdate();
        }
        descriptionHeight = contentTarget.rect.height;

        var definedHeight = CalculateDefinedHeight();
        var clampedHeight = Mathf.Clamp(definedHeight + descriptionHeight, 100, 2100);
        resizingLayoutElement.preferredHeight = clampedHeight;
    }

    public void OnDisable()
    {
        StopCoroutine(AwaitUIChange());
    }
}
