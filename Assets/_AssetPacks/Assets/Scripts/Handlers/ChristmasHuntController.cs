using System;
using System.Collections;
using System.Collections.Generic;
using riddlehouse_libraries.analytics;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using UnityEngine;
using Zenject;

public interface IChristmasHuntController
{
    public void ConfigureHunt(IHuntSteps huntSteps, Dictionary<StepType, IOldStepController> stepControllers, Action huntCompleted);
    public void ConfigureRiddle(string stepId);
    public IHuntStep GetStepData(int stepInHunt = -1);
    public void EndHunt(bool completed);

    public IHuntSteps GetCurrentHuntSteps();
    public void MarkStepStarted(string stepId);
    public void MarkStepEnded(string stepId);
    public void MarkStepAttempted(string stepId);
}
public class ChristmasChristmasHuntController : IChristmasHuntController
{
    public IHuntSteps CurrentHuntSteps { get; private set; }
    public Dictionary<StepType, IOldStepController> StepControllers { get; private set; }
    private bool IsHuntActive { get; set; } = false;
    //The huntflow handler, handles the steps as 0-index, but displays it as 1 indexed outside.
    private int HuntStepReached  { get; set; } = -1;

    private Action _huntCompleted;
    
    /// <summary>
    /// Configures and starts the hunt with the valid huntData and huntStartAction implementation.
    /// </summary>
    /// <param name="huntAssets">the stepData needed for the hunt to run.</param>
    /// <param name="huntStart">the actions implementation of huntStart</param>
    /// <exception cref="ArgumentException">returns an argument exception if the hunt couldn't be started.</exception>
    public void ConfigureHunt(IHuntSteps huntSteps, Dictionary<StepType, IOldStepController> stepControllers, Action huntCompleted)
    {
        StepControllers = stepControllers;
        SetHunt(huntSteps);
        _huntCompleted = huntCompleted;
    }
    private void SetHunt(IHuntSteps huntSteps)
    {
        if (IsHuntActive)
            throw new ArgumentException("Hunt is already active, end the current one before you can start a new one");

        IsHuntActive = true;
        HuntStepReached = -1;
        CurrentHuntSteps = huntSteps;
        
        if (CurrentHuntSteps == null)
            throw new ArgumentException("huntData was null, can't start hunt");
        if (StepControllers == null)
            throw new ArgumentException("stepControllers was null, can't start hunt");
        if (StepControllers.Count < 1)
            throw new ArgumentException("stepControllers collection is empty, can't start hunt");
    }

    public void ConfigureRiddle(string stepId)
    {
        var step = CurrentHuntSteps.GetElement(stepId);
        if (step != null)
        {
            StartStep(step);
            return;
        }
        throw new ArgumentException("Step of ID: " + stepId+" not found");
    }

    /// <summary>
    /// Retrieves the stepData for the hunt.
    /// </summary>
    /// <param name="stepInHunt">StepIndex wanted (0 indexed) leave blank for current step data.</param>
    /// <returns>The huntData used in the indexed step. If hunt isn't active, returns null.</returns>
    /// <exception cref="ArgumentException">returns an argument exception if the hunt was started, but no stepdata could be found.</exception>
    public IHuntStep GetStepData(int stepInHunt = -1)
    {
        if (stepInHunt == -1)
            stepInHunt = HuntStepReached;
        if (IsHuntActive)
        {
            if (CurrentHuntSteps == null)
                throw new ArgumentException("No data in hunt");
            if(stepInHunt < CurrentHuntSteps.GetLengthOfHunt())
                return CurrentHuntSteps.GetElement(stepInHunt < 0? 0: stepInHunt);
        }
        return null;
    }

    private void StartStep(IHuntStep step)
    {
        bool isLastStep = CurrentHuntSteps.IsLastStep(step);
        switch (step.GetStepType())
        {
            case StepType.DisplayStoryAndDone:
                DisplayStoryAndDoneOldStepController ctrl = (DisplayStoryAndDoneOldStepController)StepControllers[step.GetStepType()];
                ctrl.StartStep((DisplayStoryAndDoneHuntStep)step, this, isLastStep);
                break;
            case StepType.DisplayRiddleAndSubmitAnswer:
                DisplayStoryAndSubmitAnswerOldStepController submitAnswerctrl = (DisplayStoryAndSubmitAnswerOldStepController)StepControllers[step.GetStepType()];
                submitAnswerctrl.StartStep((DisplayRiddleAndSubmitAnswerStep)step, this, isLastStep);
                break;
            case StepType.HuntResolutionAndEnd:
                HuntResolutionAndEndOldStepController huntResolutionAndEndOldStepController = (HuntResolutionAndEndOldStepController)StepControllers[step.GetStepType()];
                huntResolutionAndEndOldStepController.StartStep((HuntResolutionAndEndStep)step, this, isLastStep);
                break;
            //TODO: AR Element
            // case StepType.RecognizeWithAssetBundle:
            //     RecognizeWithAssetBundleStepController recognizeWithAssetStepController = (RecognizeWithAssetBundleStepController)StepControllers[step.GetStepType()];
            //     recognizeWithAssetStepController.StartStep((RecognizeWithAssetBundleStep)step, this, isLastStep);
            //     break;
            case StepType.MultipleAnswersVideoInAndOut:
                MultipleAnswersVideoInAndOutOldStepController multipleAnswersVideoInAndOutOldStepController =
                    (MultipleAnswersVideoInAndOutOldStepController)StepControllers[step.GetStepType()];
                multipleAnswersVideoInAndOutOldStepController.StartStep((MultipleAnswersVideoInAndOutStep)step, this, isLastStep);
                break;
            case StepType.ResolutionVideoAndEnd:
                ResolutionVideoAndEndOldStepController resolutionVideoAndEndOldStep =
                    (ResolutionVideoAndEndOldStepController)StepControllers[step.GetStepType()];
                resolutionVideoAndEndOldStep.StartStep((ResolutionVideoAndEndStep)step, this, isLastStep);
                break;
            default:
                throw new ArgumentException("Step type configuration not declared");
        }
    }

    /// <summary>
    /// Ends the hunt, and sets the variables ready for the next one.
    /// </summary>
    public void EndHunt(bool completed)
    {
        if (CurrentHuntSteps != null)
        {
            if (completed)
            {
                CurrentHuntSteps.ClearSession();
                _huntCompleted?.Invoke();
                _huntCompleted = null;
            }
            IsHuntActive = false;
            CurrentHuntSteps = null;
            StepControllers = null;
            HuntStepReached = -1;
        }
    }
    public IHuntSteps GetCurrentHuntSteps()
    {
        return CurrentHuntSteps;
    }

    public void MarkStepStarted(string stepId)
    {
        new Analytics().StepStart(CurrentHuntSteps.GetProductID(),stepId);
    }

    public void MarkStepEnded(string stepId)
    {
        new Analytics().StepEnd(CurrentHuntSteps.GetProductID(), stepId);
    }

    public void MarkStepAttempted(string stepId)
    {
        new Analytics().RegisterAttempt(CurrentHuntSteps.GetProductID(), stepId);
    }
}
