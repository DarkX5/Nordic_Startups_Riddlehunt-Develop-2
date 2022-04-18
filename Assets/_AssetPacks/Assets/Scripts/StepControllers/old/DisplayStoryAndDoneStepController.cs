using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RHPackages.Core.Scripts;
using RHPackages.Core.Scripts.UI;
using riddlehouse_libraries.analytics;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using UnityEngine;

public interface IDisplayHintAndDoneStepController
{
    public void StartStep(IDisplayStoryAndDoneHuntStep huntStep, IChristmasHuntController christmasHuntController, bool lastRiddle);
}
public class DisplayStoryAndDoneOldStepController : BaseOldStepController, IDisplayHintAndDoneStepController
{
    private IStoryComponent _story;
    private IHuntHomeComponent _huntHomeComponent;
    private IEndHuntComponent _endHuntComponent;
    private bool _lastRiddle = false;
    public IDisplayStoryAndDoneHuntStep HuntStep { get; private set; }
    private IChristmasHuntController _christmasHuntController;
    public DisplayStoryAndDoneOldStepController(ITabComponent tabComponent, IStoryComponent story,IHuntHomeComponent huntHomeComponent, IEndHuntComponent endHuntComponent) : base(StepType.DisplayStoryAndDone, tabComponent)
    {
        _story = story ?? throw new ArgumentException("No story assigned");
        _huntHomeComponent = huntHomeComponent ?? throw new ArgumentException("No huntStart assigned");
        _endHuntComponent = endHuntComponent ?? throw new ArgumentException("No endHunt assigned");
        
        _views.Add(ComponentType.Story, story.GetComponentUIActions());
        _views.Add(ComponentType.HuntHome, huntHomeComponent.GetComponentUIActions());
        _views.Add(ComponentType.End, endHuntComponent.GetComponentUIActions());
    } 

    public void StartStep(IDisplayStoryAndDoneHuntStep huntStep, IChristmasHuntController christmasHuntController, bool lastRiddle)
    {
        if(huntStep == null) throw new ArgumentException("No huntStepAssets assigned");
        if(huntStep.GetStepType() != StepType.DisplayStoryAndDone) throw new ArgumentException("HuntStep isn't of correct stepType");
        
        _christmasHuntController = christmasHuntController;
        _lastRiddle = lastRiddle;
        HuntStep = huntStep;
        ShowAssetInStep(ComponentType.Story);
        _tabComponent.ConfigureForStepType(this);
        
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
                _story.Configure(HuntStep.GetStoryText(), "OK!", EndStep);
                break;
            case ComponentType.HuntHome:
                break;
            case ComponentType.End:
                if (_endHuntComponent == null)
                    throw new ArgumentException("No END assigned");
                //Todo: supply an endText
                _endHuntComponent.Configure("", () => _christmasHuntController.EndHunt(true));
                break;
            default:
                throw new ArgumentException("No such step in asset");
        }
        
        base.ShowAssetInStep(type);
    }

    public override void EndStep()
    {
        HuntStep.MarkComplete();
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
            ComponentType.Story
        };
    }

    public override ComponentType GetFirstStepTypeToShow()
    {
        return ComponentType.Story;
    }
}