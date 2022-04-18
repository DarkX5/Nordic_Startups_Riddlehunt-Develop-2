using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RHPackages.Core.Scripts;
using RHPackages.Core.Scripts.UI;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using UnityEngine;

public interface IDisplayStoryAndSubmitAnswerStepController
{
    public void StartStep(IDisplayRiddleAndSubmitAnswerStep huntStep, IChristmasHuntController christmasHuntController, bool lastRiddle);
}
public class DisplayStoryAndSubmitAnswerOldStepController: BaseOldStepController, IDisplayStoryAndSubmitAnswerStepController
{
    private IStoryComponent _story;
    private IRiddleTabComponent _riddle;
    private IHuntHomeComponent _huntHomeComponent;
    private IEndHuntComponent _endHuntComponent;
    private bool _lastRiddle = false;
    public IDisplayRiddleAndSubmitAnswerStep HuntStep { get; private set; }
    private IChristmasHuntController _christmasHuntController;
    public DisplayStoryAndSubmitAnswerOldStepController(ITabComponent tabComponent, IStoryComponent story, IRiddleTabComponent riddle, IHuntHomeComponent huntHomeComponent, IEndHuntComponent endHuntComponent) : base(StepType.DisplayStoryAndDone, tabComponent)
    {
        _story = story ?? throw new ArgumentException("No story assigned");
        _riddle = riddle ?? throw new ArgumentException("No riddle assigned");
        _huntHomeComponent = huntHomeComponent ?? throw new ArgumentException("No huntStart assigned");
        _endHuntComponent = endHuntComponent ?? throw new ArgumentException("No endHunt assigned");
        
        _views.Add(ComponentType.Story, story.GetComponentUIActions());
        _views.Add(ComponentType.RiddleTab, riddle);
        _views.Add(ComponentType.HuntHome, huntHomeComponent.GetComponentUIActions());
        _views.Add(ComponentType.End, endHuntComponent.GetComponentUIActions());
    }

    public void StartStep(IDisplayRiddleAndSubmitAnswerStep huntStep, IChristmasHuntController christmasHuntController, bool lastRiddle)
    {
        if(huntStep == null) throw new ArgumentException("No huntStepAssets assigned");
        if(huntStep.GetStepType() != StepType.DisplayRiddleAndSubmitAnswer) throw new ArgumentException("No huntStep isn't of correct stepType");

        _christmasHuntController = christmasHuntController;
        _lastRiddle = lastRiddle;
        HuntStep = huntStep;
        _tabComponent.ConfigureForStepType(this);
        if (HuntStep.GetAnswerData().HasAnswer())
            ShowAssetInStep(ComponentType.RiddleTab);
        else
            ShowAssetInStep(ComponentType.Story);

        christmasHuntController.MarkStepStarted(HuntStep.GetStepId());
    }
    public override void ShowAssetInStep(ComponentType type)
    {
        if (HuntStep == null)
            throw new ArgumentException("Please call start step before attempting to show an asset");
        
        switch (type)
        {
            case ComponentType.Story:
                if (_story == null)
                    throw new ArgumentException("No STORY assigned");
                _story.Configure(HuntStep.GetStoryText(), "OK!", () => ShowAssetInStep(ComponentType.RiddleTab));
                break;
            case ComponentType.RiddleTab:
                if(_riddle == null) 
                    throw new ArgumentException("No Riddle assigned");
                _riddle.Configure(
                    HuntStep.GetRiddleText(), 
                    HuntStep.GetAnswerData(),
                    HuntStep.GetRiddleImages(),
                    () =>
                    {
                        _christmasHuntController.MarkStepAttempted(HuntStep.GetStepId());
                        EndStep();
                    });
                break;
            case ComponentType.HuntHome:
                break;
            case ComponentType.End:
                if (_endHuntComponent == null)
                    throw new ArgumentException("No END assigned");
                //Todo: supply an end text
                _endHuntComponent.Configure("",() => _christmasHuntController.EndHunt(true));
                break;
            default:
                throw new ArgumentException("No such step in asset");
        }
        
        base.ShowAssetInStep(type);
    }

    public override void EndStep()
    {
        if (_lastRiddle)
        {
            ShowAssetInStep(ComponentType.End);
        }
        else
        {
            base.EndStep();
            ShowAssetInStep(ComponentType.HuntHome);
        }
        _christmasHuntController.MarkStepEnded(HuntStep.GetStepId());
    }
    public override List<ComponentType> GetTypesInOrder()
    {
        return new List<ComponentType>()
        {
            ComponentType.Story,
            ComponentType.RiddleTab
        };
    }

    public override ComponentType GetFirstStepTypeToShow()
    {
        return ComponentType.Story;
    }
}