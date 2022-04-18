using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using RHPackages.Core.Scripts.StepControllers;
using riddlehouse_libraries.products.Steps;
using StepControllers;
using UnityEngine;

public interface IStepControllerInstantiator
{
    public IStepController CreateDisplayRiddleAndSubmitAnswerStepController();
    public IStepController CreateDisplayRiddleWithMultipleChoiceStepController();
}
public class StepControllerInstantiator : IStepControllerInstantiator
{
    private IAddressableAssetLoader _addressableAssetLoader;
    public StepControllerInstantiator(IAddressableAssetLoader addressableAssetLoader)
    {
        _addressableAssetLoader = addressableAssetLoader;
    }
    public IStepController CreateDisplayRiddleAndSubmitAnswerStepController()
    {
        return new DisplayRiddleAndSubmitAnswerStepController(
            new ViewCollector(_addressableAssetLoader), 
            new StepDataConverter(new TypeChecker()));
    }
    public IStepController CreateDisplayRiddleWithMultipleChoiceStepController()
    {
        return new DisplayRiddleWithMultipleChoiceStepController(
            new ViewCollector(_addressableAssetLoader), 
            new StepDataConverter(new TypeChecker()), 
            new SpriteHelper());
    }
}

