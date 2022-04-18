using System;
using System.Collections;
using System.Collections.Generic;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using riddlehouse_libraries.products.models.DTOs;
using UnityEngine;

public interface IDisplayStoryAndDoneHuntStep
{
    public string GetStepId();
    public string GetStoryText();
    public void MarkComplete();
    public StepType GetStepType();
}

public class InternalDisplayStoryAndDoneHuntStep : InternalHuntStep
{
    //NOTE: next time anything in this stepModel is changed, we should update it away from internals, and into the new self-contained model! - Philip Haugaard.
    public string StoryText { get; set; }= null;
    
    public InternalDisplayStoryAndDoneHuntStep(): base(StepType.DisplayStoryAndDone)
    {
    }
    
    public override bool ValidateAssetConfiguration()
    {
        if (StoryText == null)
            return false;
        return true;
    }
    
    public new void SetBypassvalidation (string url)
    {
        base.SetBypassvalidation(url);
    }
}
public class DisplayStoryAndDoneHuntStep : IDisplayStoryAndDoneHuntStep, IHuntStep
{
    private readonly string _stepId;
    private readonly string _stepTitle;
    private readonly StepCondition _condition;
    private readonly StepType _stepType;
    private readonly string _storyText;
    private readonly BooleanAnswerData _answerData;

    public DisplayStoryAndDoneHuntStep(InternalDisplayStoryAndDoneHuntStep internalDisplayStoryAndDoneHuntStep)
    {
        if (!internalDisplayStoryAndDoneHuntStep.ValidateAssetConfiguration())
        {
            throw new ArgumentException("Unsafe internal data passed into an external class, not allowed");
        }

        _stepId = internalDisplayStoryAndDoneHuntStep.StepId;
        _condition = internalDisplayStoryAndDoneHuntStep.Condition;
        _stepTitle = internalDisplayStoryAndDoneHuntStep.StepTitle;
        _stepType = internalDisplayStoryAndDoneHuntStep.StepType;
        _storyText = internalDisplayStoryAndDoneHuntStep.StoryText;
        _answerData = new BooleanAnswerData(_stepId);
    }

    public string GetStoryText()
    {
        return _storyText;
    }

    public void MarkComplete()
    {
        _answerData.SetAnswer(true);
    }

    public string GetStepId()
    {
        return _stepId;
    }

    public StepCondition GetCondition()
    {
        return _condition;
    }

    public StepType GetStepType()
    {
        return _stepType;
    }

    public bool HasAnswer()
    {
        return _answerData.HasAnswer();
    }

    public void ClearSessionAndClearAnswer()
    {
        throw new NotImplementedException();
    }

    public string GetStepTitle()
    {
        return _stepTitle;
    }
}