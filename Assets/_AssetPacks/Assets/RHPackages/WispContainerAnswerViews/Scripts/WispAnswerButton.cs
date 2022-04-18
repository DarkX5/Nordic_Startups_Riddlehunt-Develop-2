using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWispAnswerButton
{
    public void Configure(string value, Action<ButtonState> answerGivenAction, Color selected, Color unselected, Sprite icon);
    public void ButtonPressed();
    public void SetState(bool selected);
}

[RequireComponent(typeof(ToggleImageColorComponent))]
[RequireComponent(typeof(ChangeImageSpriteComponent))]
public class WispAnswerButton : MonoBehaviour, IWispAnswerButton
{
    public ToggleImageColorComponent _toggleImageColorComponent { get; private set; }
    public ChangeImageSpriteComponent _changeImageSpriteComponent { get; private set; }
    private ButtonState state;
    private Action<ButtonState> _answerGivenAction;
    public void Configure(string value, Action<ButtonState> answerGivenAction, Color selected, Color unselected, Sprite icon)
    {
        EnsureDependenciesExist();
        state = new ButtonState()
        {
            Value = value,
            Selected = false
        };
        _answerGivenAction = answerGivenAction;
        _changeImageSpriteComponent.Configure(icon);
        _toggleImageColorComponent.Configure(selected,unselected);
    }

    public void ButtonPressed()
    {
        EnsureDependenciesExist();
        _toggleImageColorComponent.ToggleColor();
        state.Selected = _toggleImageColorComponent.IsSelected;
        _answerGivenAction.Invoke(state);
    }

    public void SetState(bool selected)
    {
        EnsureDependenciesExist();
        if (selected)
            _toggleImageColorComponent.SetStateSelected();
        else 
            _toggleImageColorComponent.SetStateUnselected();
    }
    
    private void EnsureDependenciesExist()
    {
        _toggleImageColorComponent ??= GetComponent<ToggleImageColorComponent>();
        _changeImageSpriteComponent ??= GetComponent<ChangeImageSpriteComponent>();
    }
}
