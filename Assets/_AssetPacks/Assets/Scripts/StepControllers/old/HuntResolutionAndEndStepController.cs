using System;
using System.Collections;
using System.Collections.Generic;
using RHPackages.Core.Scripts;
using RHPackages.Core.Scripts.UI;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using UnityEngine;

public interface IHuntResolutionAndEndStepController
{
    public void StartStep(IHuntResolutionAndEndStep huntStep, IChristmasHuntController christmasHuntController, bool lastRiddle);
}

public class HuntResolutionAndEndOldStepController : BaseOldStepController, IHuntResolutionAndEndStepController
{
    private bool _lastRiddle = false;
    private StepControllerHelper helper;
    
    private static List<ComponentType> TypesInOrder = new List<ComponentType>()
    {
        ComponentType.Story,
        ComponentType.RiddleTab,
        ComponentType.Resolution
    };
    
    public IHuntResolutionAndEndStep HuntStep { get; private set; }
    private IChristmasHuntController _christmasHuntController;
    
    private IStoryComponent _story;
    private IRiddleTabComponent _riddleTab;
    private IHuntHomeComponent _huntHomeComponent;
    private IEndHuntComponent _endHuntComponent;
    private IResolutionComponent _resolutionComponent;
    
    public HuntResolutionAndEndOldStepController(
        ITabComponent tabComponent, 
        IStoryComponent story, 
        IRiddleTabComponent riddleTab, 
        IHuntHomeComponent huntHomeComponent, 
        IEndHuntComponent endHuntComponent,
        IResolutionComponent resolutionComponent) : base(StepType.HuntResolutionAndEnd, tabComponent)
    {
        helper = new StepControllerHelper();

        _story = story;
        _riddleTab = riddleTab;
        _huntHomeComponent = huntHomeComponent;
        _endHuntComponent = endHuntComponent;
        _resolutionComponent = resolutionComponent;
        
        _views.Add(ComponentType.Story, story.GetComponentUIActions());
        _views.Add(ComponentType.RiddleTab, riddleTab);
        _views.Add(ComponentType.Resolution, resolutionComponent.GetComponentUIActions());
        helper.AssureNecessaryStepViews(TypesInOrder, _views);
        
        //todo: HuntHome and EndScreens are considered seperate to the step... this logic is wrong. - Philip
        _views.Add(ComponentType.HuntHome, huntHomeComponent.GetComponentUIActions());
        _views.Add(ComponentType.End, endHuntComponent.GetComponentUIActions());
    }

    public void StartStep(IHuntResolutionAndEndStep huntStep, IChristmasHuntController christmasHuntController, bool lastRiddle)
    {
        helper.AssureHuntStep(huntStep.GetStepType(), StepType.HuntResolutionAndEnd);
        _christmasHuntController = christmasHuntController;
        _lastRiddle = lastRiddle;
        HuntStep = huntStep;
        _tabComponent.ConfigureForStepType(this);
        ShowAssetInStep(GetFirstStepTypeToShow());
        
        christmasHuntController.MarkStepStarted(HuntStep.GetStepId());
    }
    
    public override void ShowAssetInStep(ComponentType type)
    {
        switch (type)
        {
            case ComponentType.Story:
                _story.Configure(HuntStep.GetStoryText(), "OK!", () => ShowAssetInStep(ComponentType.RiddleTab));
                break;
            case ComponentType.RiddleTab:
                _riddleTab.Configure(
                    HuntStep.GetRiddleText(), 
                    HuntStep.GetAnswerData(), 
                    HuntStep.GetRiddleImages(),
                    () =>
                    {
                        _christmasHuntController.MarkStepAttempted(HuntStep.GetStepId());
                        ShowAssetInStep(ComponentType.Resolution);
                    });
                break;
            case ComponentType.HuntHome:
                break;
            case ComponentType.Resolution:
                _resolutionComponent.Configure(() => EndStep(), HuntStep.GetVideoUrl());
                break;
            case ComponentType.End:
                _endHuntComponent.Configure(HuntStep.GetEndText(), () => _christmasHuntController.EndHunt(true));
                break;
            default:
                throw new ArgumentException("No such asset in step");
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
        return TypesInOrder;
    }

    public override ComponentType GetFirstStepTypeToShow()
    {
        return TypesInOrder[0];
    }
}
