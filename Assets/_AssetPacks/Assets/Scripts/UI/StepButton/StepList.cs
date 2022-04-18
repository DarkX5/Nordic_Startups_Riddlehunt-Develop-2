using System;
using System.Collections.Generic;
using riddlehouse_libraries.products.models;
using riddlehouse_libraries.products.models.DTOs;
using UnityEngine;

public interface IStepList
{
    public void ConfigureStepList(int stepReached, IHuntSteps huntSteps, List<IStepBtn> buttons, Action<int> buttonAction);
}
public class StepList : IStepList
{
    private List<IStepBtn> _buttons;
    public void ConfigureStepList(int stepReached, IHuntSteps huntSteps, List<IStepBtn> buttons, Action<int> buttonAction)
    {
        var huntLength = huntSteps.GetLengthOfHunt();
        if (huntLength > buttons.Count)
            throw new ArgumentException("There must be at least one button for every hunt step");
        
        _buttons = buttons;
        for(int i = 0; i < huntLength; i++)
        {
            _buttons[i].Configure(stepReached, i, huntSteps.GetElement(i).GetStepTitle(), buttonAction);
        }
    }
}
