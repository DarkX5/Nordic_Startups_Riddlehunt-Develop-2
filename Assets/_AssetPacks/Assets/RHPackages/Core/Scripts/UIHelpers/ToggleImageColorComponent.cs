using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ToggleImageColorComponent : MonoBehaviour
{
    public Image ImageComponent { get; private set; }
    public bool IsSelected { get; private set; } = false;
    private Color _selected;
    private Color _unselected;
    public void Configure(Color selected, Color unselected)
    {
        EnsureImageComponentExists();
        _selected = selected;
        _unselected = unselected;
        SetStateUnselected();
    }
    
    public void ToggleColor()
    {
        if (!IsSelected)
            SetStateSelected();
        else
            SetStateUnselected();
    }

    public void SetStateSelected()
    {
        EnsureImageComponentExists();
        IsSelected = true;
        ImageComponent.color = _selected;
    }
    
    public void SetStateUnselected()
    {
        EnsureImageComponentExists();
        IsSelected = false;
        ImageComponent.color = _unselected;
    }

    private void EnsureImageComponentExists()
    {
        ImageComponent ??= GetComponent<Image>();
    }
}
