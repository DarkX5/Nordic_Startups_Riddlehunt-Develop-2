using System;
using System.Collections.Generic;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using riddlehouse_libraries.products.models.DTOs;

#region internal
public interface IInternalHuntStep
{
    public string GetStepTitle();
    public StepType GetStepType();
    public bool IsExpectedStepType(StepType steptype);
    public bool ValidateAssetConfiguration();
    public bool DidBypassValidation();
    public void SetBypassvalidation(string url);
}

public class InternalHuntStep : IInternalHuntStep
{
    public string StepId { get; set; }
    public StepCondition Condition { get; set; }
    public string StepTitle { get; set; } = null;
    public StepType StepType { get; private set; }
    private bool _bypassValidation { get; set; } = false;
    public InternalHuntStep(StepType stepType)
    {
        StepType = stepType;
    }

    public string GetStepTitle()
    {
        return StepTitle;
    }

    public StepType GetStepType()
    {
        return StepType;
    }

    public bool IsExpectedStepType(StepType type)
    {
        return StepType == type;
    }

    public virtual bool ValidateAssetConfiguration()
    {
        throw new ArgumentException("Not intended for direct use, please use an implemented HuntStep class.");
    }

    public virtual bool DidBypassValidation()
    {
        return _bypassValidation;
    }

    public virtual void SetBypassvalidation(string url)
    {
        _bypassValidation = true;
    }
}
#endregion
public interface IHuntStep
{
    public string GetStepTitle();
    public string GetStepId();
    public StepCondition GetCondition();
    public StepType GetStepType();
    public bool HasAnswer();
    public void ClearSessionAndClearAnswer();
}
public interface IHuntSteps
{
    public IHuntStep GetElement(int step);
    public IHuntStep GetElement(string stepId);
    public int GetLengthOfHunt();
    public string GetFeedbackLink();
    public bool HasStepConditionsBeenMet(string stepId);
    public bool IsLastStep(IHuntStep step);
    public string GetProductID();
    public void ClearSession();
}
public class HuntSteps : IHuntSteps
{
    private List<IHuntStep> huntSteps;
    private string _feedbackLink;
    private string _productId;
    public HuntSteps(string feedbackLink, string productId)
    {
        _feedbackLink = feedbackLink;
        huntSteps = new List<IHuntStep>();
        _productId = productId;
    }

    public IHuntStep GetElement(int step)
    {
        if (huntSteps.Count-1 >= step && step > -1)
        {
            return huntSteps[step];
        }
        throw new ArgumentException("No steps available at: " + step + ", the list ended at: " + huntSteps.Count);
    }

    public IHuntStep GetElement(string stepId)
    {
       return huntSteps.Find(x => x.GetStepId() == stepId);
    }

    public int GetLengthOfHunt()
    {
        return huntSteps.Count;
    }

    public string GetFeedbackLink()
    {
        return _feedbackLink;
    }

    /// <summary>
    /// Checks if a specific step has it's conditions met.
    /// </summary>
    /// <param name="stepId">Id of the step to check.</param>
    /// <returns>True if the conditions are met; false if they aren't.</returns>
    public bool HasStepConditionsBeenMet(string stepId)
    {
        var stepToCheck = huntSteps.Find(x => x.GetStepId() == stepId);
        bool conditionMet = true;
        
        if (stepToCheck != null)
        {
            var condition = stepToCheck.GetCondition();
            if (condition == null)
                return true;

            if (condition.Ids.Count == 0)
            {
                foreach (var step in huntSteps)
                {
                    if (stepId != step.GetStepId())
                    {
                        if (conditionMet)
                        {
                            conditionMet = CheckStep(step);
                        }
                        else return false;
                    }
                }
            }
            else
            {
                foreach (var conditionId in condition.Ids)
                {
                    if (conditionMet)
                    {
                        var singleStepToCheck = huntSteps.Find(x => x.GetStepId() == conditionId);
                        conditionMet = CheckStep(singleStepToCheck);
                    }
                    else return false;
                }
            }
        }
        return conditionMet;
    }

    public bool IsLastStep(IHuntStep step)
    {
        if (huntSteps[huntSteps.Count - 1].GetStepId() == step.GetStepId())
        {
            return true;
        }
        return false;
    }

    public string GetProductID()
    {
        return _productId;
    }

    public void ClearSession()
    {
        foreach (var step in huntSteps)
        {
            step.ClearSessionAndClearAnswer();
        }
    }

    private bool CheckStep(IHuntStep singleStepToCheck)
    {
        bool conditionMet = true;
        switch (singleStepToCheck.GetStepType())
        {
            case StepType.DisplayStoryAndDone:
                break;
            case StepType.DisplayRiddleAndSubmitAnswer:
                DisplayRiddleAndSubmitAnswerStep riddleAndSubmitAnswerModel = (DisplayRiddleAndSubmitAnswerStep)singleStepToCheck;
                conditionMet = riddleAndSubmitAnswerModel.GetAnswerData().HasAnswer();
                break;
            //TODO: AR Element
            // case StepType.RecognizeWithAssetBundle:
            //     RecognizeWithAssetBundleStep recognizeWithAssetBundleModel = (RecognizeWithAssetBundleStep)singleStepToCheck;
            //     conditionMet = recognizeWithAssetBundleModel.GetAnswerData().HasAnswer();
            //     break;
            case StepType.HuntResolutionAndEnd:
                HuntResolutionAndEndStep huntResolutionAndEndStepModel = (HuntResolutionAndEndStep)singleStepToCheck;
                conditionMet = huntResolutionAndEndStepModel.GetAnswerData().HasAnswer();
                break;
            case StepType.MultipleAnswersVideoInAndOut:
                MultipleAnswersVideoInAndOutStep multipleAnswerVideoInAndOut = (MultipleAnswersVideoInAndOutStep)singleStepToCheck;
                conditionMet = multipleAnswerVideoInAndOut.GetAnswerData().HasAnswer();
                break;
            case StepType.ResolutionVideoAndEnd:
                ResolutionVideoAndEndStep resolutionVideoAndEndStep = (ResolutionVideoAndEndStep)singleStepToCheck;
                conditionMet = resolutionVideoAndEndStep.HasAnswer();
                break;
            default:
                throw new ArgumentException("case not matched");
        }

        return conditionMet;
    }

    public void ConvertInternalStepdata(List<IInternalHuntStep> unsafeStepData) //todo: remove this during cleanup.
    {
        foreach (IInternalHuntStep internalHuntStep in unsafeStepData)
        {
            switch (internalHuntStep.GetStepType())
            {
                case StepType.DisplayStoryAndDone:
                    huntSteps.Add(new DisplayStoryAndDoneHuntStep((InternalDisplayStoryAndDoneHuntStep)internalHuntStep));
                    break;
                case StepType.DisplayRiddleAndSubmitAnswer:
                    huntSteps.Add((DisplayRiddleAndSubmitAnswerStep)internalHuntStep);
                    break;
                case StepType.HuntResolutionAndEnd:
                    huntSteps.Add((HuntResolutionAndEndStep)internalHuntStep);
                    break;
                //TODO: AR Element
                // case StepType.RecognizeWithAssetBundle:
                //     huntSteps.Add((RecognizeWithAssetBundleStep)internalHuntStep);
                //     break;
                case StepType.MultipleAnswersVideoInAndOut:
                    huntSteps.Add((MultipleAnswersVideoInAndOutStep)internalHuntStep);
                    break;
                case StepType.ResolutionVideoAndEnd:
                    huntSteps.Add((ResolutionVideoAndEndStep)internalHuntStep);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("unsafeStepData", "Hunt Step Type does not exist!");
            }
        }
    }
}