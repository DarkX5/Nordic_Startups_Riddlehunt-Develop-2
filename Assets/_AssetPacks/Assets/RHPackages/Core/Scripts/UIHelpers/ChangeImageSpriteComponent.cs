using System.Collections;
using System.Collections.Generic;
using riddlehouse_libraries.products.resources;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ChangeImageSpriteComponent : MonoBehaviour
{
    public Image _imageComponent { get; private set; }

    public void Configure(Sprite icon)
    {
        _imageComponent ??= GetComponent<Image>();
        _imageComponent.sprite = icon;
    }
}
