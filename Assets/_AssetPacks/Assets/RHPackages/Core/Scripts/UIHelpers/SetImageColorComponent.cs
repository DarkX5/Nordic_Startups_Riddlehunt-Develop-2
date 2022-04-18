using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Image))]
public class SetImageColorComponent : MonoBehaviour
{
    public Image _imageComponent { get; private set; }

    public void Configure(Color color)
    {
        _imageComponent ??= GetComponent<Image>();
        _imageComponent.color = color;
    }
}
