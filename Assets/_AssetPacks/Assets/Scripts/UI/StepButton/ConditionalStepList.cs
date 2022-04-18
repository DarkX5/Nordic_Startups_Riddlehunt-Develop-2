using System;
using System.Collections;
using System.Collections.Generic;
using riddlehouse_libraries.products.models.DTOs;
using UnityEngine;
public interface IConditionalStepList
{
    public void ConfigureStepList(IHuntSteps huntSteps, List<IConditionalStepBtn> buttons, Action<string> buttonAction);
}
public class ConditionalStepList : IConditionalStepList
{
    private List<IConditionalStepBtn> _buttons;
    public void ConfigureStepList(IHuntSteps huntSteps, List<IConditionalStepBtn> buttons, Action<string> buttonAction)
    {
        var huntLength = huntSteps.GetLengthOfHunt();
        if (huntLength > buttons.Count)
            throw new ArgumentException("There must be at least one button for every hunt step");
        
        _buttons = buttons;
        for(int i = 0; i < huntLength; i++)
        {
            var step = huntSteps.GetElement(i);

            ConditionalStepButtonState state;
            if (huntSteps.HasStepConditionsBeenMet(step.GetStepId()))
            {
                if (step.HasAnswer())
                    state = ConditionalStepButtonState.Complete;
                else
                    state = ConditionalStepButtonState.Incomplete;
            }
            else
            {
                if (step.GetCondition().Style == StepBtnStyles.Disabled)
                    state = ConditionalStepButtonState.Disabled;
                else if (step.GetCondition().Style == StepBtnStyles.Hidden)
                    state = ConditionalStepButtonState.Hidden;
                else
                    throw new ArgumentException("Style not configured.");
            }

            _buttons[i].Configure(state, step.GetStepId(), step.GetStepTitle(), buttonAction);
        }
    }
}
