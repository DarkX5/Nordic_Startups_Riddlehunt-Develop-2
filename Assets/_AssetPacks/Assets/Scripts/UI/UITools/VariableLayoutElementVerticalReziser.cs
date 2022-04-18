using System;
using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity;
using UnityEngine;
using UnityEngine.UI;

public class VariableLayoutElementVerticalReziser : MonoBehaviour
{
    [SerializeField] private List<RectTransform> definedElements;
    [SerializeField] private RectTransform observableElement;
    [SerializeField] private LayoutElement resizingLayoutElement;
    [SerializeField] private float offset;
    public void StartUIUpdate()
    {
        StartCoroutine(AwaitUIChange());
    }
    
    private float CalculateDefinedHeight()
    {
        var definedHeight = 0f;
        foreach (var element in definedElements)
        {
            definedHeight += element.rect.height;
        }
        definedHeight = definedHeight + offset;
        return definedHeight;
    }

    private IEnumerator AwaitUIChange()
    {
        var observedHeight = observableElement.rect.height;
        while (Math.Abs(observedHeight - observableElement.rect.height) < 0.15f)
        {
            yield return new WaitForFixedUpdate();
        }
        var definedHeight = CalculateDefinedHeight();
        resizingLayoutElement.preferredHeight = definedHeight;
    }

    public void OnDisable()
    {
        StopCoroutine(AwaitUIChange());
    }
}
