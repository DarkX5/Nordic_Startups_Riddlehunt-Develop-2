using System;
using System.Collections.Generic;
using RHPackages.Core.Scripts;
using RHPackages.Core.Scripts.UI;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;

public class StepControllerHelper 
{
    public void AssureNecessaryStepViews(List<ComponentType> typesInOrder, Dictionary<ComponentType, IViewActions> views)
    {
        bool allPresent = false;
        if (typesInOrder.Count == views.Count)
        {
            allPresent = true;
            foreach (var type in typesInOrder)
            {
                if (views.ContainsKey(type))
                {
                    if(views[type] != null)
                        continue;
                }
                allPresent = false;
                break;
            }
        }
        if(!allPresent)
            throw new ArgumentException("Missing elements in stepController.");
    }

    public void AssureHuntStep(StepType currentStepType, StepType expectedStepType)
    {
        if(currentStepType != expectedStepType) throw new ArgumentException("HuntStep isn't of correct stepType");
    }
}