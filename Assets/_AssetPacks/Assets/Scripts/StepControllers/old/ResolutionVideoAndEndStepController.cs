using System.Collections;
using System.Collections.Generic;
using RHPackages.Core.Scripts;
using RHPackages.Core.Scripts.UI;
using UnityEngine;
public interface IResolutionVideoAndEndStepController
{
    public void StartStep(IResolutionVideoAndEndStep huntStep, IChristmasHuntController christmasHuntController, bool lastRiddle);
}

public class ResolutionVideoAndEndOldStepController : IResolutionVideoAndEndStepController, IOldStepController
{
    private bool _lastRiddle = false;
    
    public IResolutionVideoAndEndStep HuntStep { get; private set; }
    private IChristmasHuntController _christmasHuntController;

    private IEndHuntComponent _endController;
    private static List<ComponentType> TypesInOrder = new List<ComponentType>()
    {
        ComponentType.End
    };

    public ResolutionVideoAndEndOldStepController(IEndHuntComponent endController)
    {
        _endController = endController;
    }
    
    public void StartStep(IResolutionVideoAndEndStep huntStep, IChristmasHuntController christmasHuntController, bool lastRiddle)
    {
        _lastRiddle = lastRiddle;
        _christmasHuntController = christmasHuntController;
        HuntStep = huntStep;
        HuntStep.MarkAnswered();
        _endController.Configure(huntStep.GetEndText(), huntStep.GetResolutionVideoLink(), EndStep);
        
        _endController.GetComponentUIActions().Display();
        _christmasHuntController.MarkStepStarted(HuntStep.GetStepId());
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
        _christmasHuntController.EndHunt(true);
    }

    public List<ComponentType> GetTypesInOrder()
    {
        return TypesInOrder;
    }

    public ComponentType GetFirstStepTypeToShow()
    {
        return TypesInOrder[0];
    }
}
