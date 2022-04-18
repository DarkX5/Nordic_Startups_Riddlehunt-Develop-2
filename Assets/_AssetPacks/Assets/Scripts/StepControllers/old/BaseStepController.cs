using System;
using System.Collections;
using System.Collections.Generic;
using RHPackages.Core.Scripts;
using RHPackages.Core.Scripts.UI;
using Riddlehouse.Core.Helpers.Helpers;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using UnityEngine;


public interface IOldStepController
{
    public void ShowAssetInStep(ComponentType type);
    public bool IsShowingAsset(ComponentType type);
    public void EndStep();
    public List<ComponentType> GetTypesInOrder();
    public ComponentType GetFirstStepTypeToShow();
}

public class BaseOldStepController : IOldStepController
{
    public readonly Dictionary<ComponentType, IViewActions> _views;
    public StepType StepType{ get; private set; }
    public IHuntStep _huntStep;
    public ITabComponent _tabComponent { get; private set; }
    public BaseOldStepController(StepType stepType, ITabComponent tabComponent)
    {
        StepType = stepType;
        _views = new Dictionary<ComponentType, IViewActions>();
        _tabComponent = tabComponent;
    }

    public void StartStep()
    {
        _huntStep = null;
    }

    public virtual void ShowAssetInStep(ComponentType type)
    {
        if (!_views.ContainsKey(type) || _views[type] == null)
            throw new ArgumentException("trying to access a non-existant view");

        _tabComponent.Display(type);
        //_views[type].Display();
        foreach (var key in _views.Keys)
        {
            if (_views[key].IsShown() && _views[key].GetComponentType() != type)
                _views[key].Hide();
        }
    }

    public virtual bool IsShowingAsset(ComponentType type)
    {
        if(!_views.ContainsKey(type) || _views[type] == null)
            throw new ArgumentException("trying to access a non-existant view");

        return _views[type].IsShown();
    }

    public virtual void EndStep()
    {
        foreach (var key in _views.Keys)
        {
            if(_views[key] == null)
                throw new ArgumentException("trying to access a non-existant view");
            
            if(_views[key].IsShown())
                _views[key].Hide();
        }
    }

    public virtual List<ComponentType> GetTypesInOrder()
    {
        throw new ArgumentException("Not intended for direct use, please use an implemented stepController class.");
    }

    public virtual ComponentType GetFirstStepTypeToShow()
    {
        throw new ArgumentException("Not intended for direct use, please use an implemented stepController class.");
    }
}