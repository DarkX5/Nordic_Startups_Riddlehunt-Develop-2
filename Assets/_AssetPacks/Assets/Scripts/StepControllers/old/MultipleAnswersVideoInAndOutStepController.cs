using System;
using System.Collections;
using System.Collections.Generic;
using Answers.MultipleChoice.Data.Icon;
using RHPackages.Core.Scripts;
using RHPackages.Core.Scripts.UI;
using riddlehouse_libraries.products.models;
using UnityEngine;

public interface IMultipleAnswersVideoInAndOutStepController
{
    public void StartStep(IMultipleAnswersVideoInAndOutStep huntStep, IChristmasHuntController christmasHuntController, bool lastRiddle);
}
public class MultipleAnswersVideoInAndOutOldStepController : IMultipleAnswersVideoInAndOutStepController, IOldStepController
{
    private bool _lastRiddle = false;
    private StepControllerHelper helper;
    
    public IMultipleAnswersVideoInAndOutStep HuntStep { get; private set; }
    private IChristmasHuntController _christmasHuntController;

    private VideoBasedRiddleViewController _viewController;
    private Action _goBack;
    private static List<ComponentType> TypesInOrder = new List<ComponentType>()
    {
        ComponentType.MultipleChoiceIconsVideoInAndOut
    };
    
    public MultipleAnswersVideoInAndOutOldStepController(VideoBasedRiddleViewController viewController, Action goback)
    {
        _viewController = viewController;
        _goBack = goback;
    }

    public void StartStep(IMultipleAnswersVideoInAndOutStep huntStep, IChristmasHuntController christmasHuntController, bool lastRiddle)
    {
        _lastRiddle = lastRiddle;
        _christmasHuntController = christmasHuntController;
        HuntStep = huntStep;
        var answerData = (IconMultipleChoiceAnswerData)HuntStep.GetAnswerData();
        _viewController.Display();
        _christmasHuntController.MarkStepStarted(HuntStep.GetStepId());
        _viewController.Configure(new VideoBasedRiddleViewController.Config()
        {
            AnswerData = answerData,
            IntroVideo = HuntStep.GetIntroVideoUrl(),
            OutroVideo = HuntStep.GetOutroVideoUrl(),
            StepCompleted = EndStep,
            GoBack = _goBack
        });
    }

    public void ShowAssetInStep(ComponentType type)
    {
        throw new System.NotImplementedException();
    }

    public bool IsShowingAsset(ComponentType type)
    {
        throw new System.NotImplementedException();
    }

    public void EndStep()
    {
        _christmasHuntController.MarkStepEnded(HuntStep.GetStepId());

        if (_lastRiddle)
            _christmasHuntController.EndHunt(true);
        else 
           _goBack.Invoke();
    }

    public List<ComponentType> GetTypesInOrder()
    {
        throw new System.NotImplementedException();
    }

    public ComponentType GetFirstStepTypeToShow()
    {
        throw new System.NotImplementedException();
    }
}
