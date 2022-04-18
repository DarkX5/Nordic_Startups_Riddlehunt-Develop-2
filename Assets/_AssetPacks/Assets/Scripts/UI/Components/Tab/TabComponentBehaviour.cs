using System;
using System.Collections;
using System.Collections.Generic;
using RHPackages.Core.Scripts;
using RHPackages.Core.Scripts.UI;
using Riddlehouse.Core.Helpers.Helpers;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public interface ITabComponent
{
    public void ConfigureTabs(List<IViewActions> components);
    public void Add(IViewActions viewActions);
    public void ConfigureForStepType(IOldStepController oldStepController);
    public bool HasTypeInViews(ComponentType type);
    public void HomeAction();
    public void Display(ComponentType type);

    public IViewActions GetComponentUIAction(ComponentType type);
}
public class TabComponent : ITabComponent
{
    public static ITabComponent Factory(GameObject go, RectTransform parent)
    {
        var tabComponentActions = new ComponentHelper<TabComponentBehaviour>().GetBehaviourIfExists(go);
        var uiFitters = new UIFitters();
        uiFitters.FitToFullscreen((RectTransform)tabComponentActions.transform, parent);
        var tabComponent =  new TabComponent(tabComponentActions,uiFitters);
        tabComponentActions.SetTabComponent(tabComponent);
        return tabComponent;
    }
    
    //Factory
    private Action _homeAction;
    private Dictionary<ComponentType, IViewActions> ActiveViewComponents { get; set; }
    private Dictionary<ComponentType, ITabBtn> ActiveTabBtns { get; set; }
    private readonly IUIFitters _uiFitters;
    private readonly ITabComponentActions _tabComponentActions;
    public TabComponent(ITabComponentActions tabComponentActions, IUIFitters uiFitters)
    {
        _tabComponentActions = tabComponentActions;
        _uiFitters = uiFitters;
        ActiveViewComponents = new Dictionary<ComponentType, IViewActions>();
        ActiveTabBtns = new Dictionary<ComponentType, ITabBtn>();
    }

    public void AddHome(IViewActions home)
    {
        if (home.GetComponentType() != ComponentType.HuntHome)
            throw new ArgumentException("Can only add components of type home to the home tab.");
        
        _homeAction = () => Display(ComponentType.HuntHome);
        if (ActiveViewComponents.ContainsKey(home.GetComponentType()))
            throw new ArgumentException("Tried to add the same view type component twice.");
        ActiveViewComponents.Add(home.GetComponentType(), home);
        _uiFitters.FitToFullscreen(home.GetRectTransform(), _tabComponentActions.GetContentParent());
        _homeAction.Invoke();
    }

    public void HomeAction()
    {
        _homeAction.Invoke();
    }

    public void AddEnd(IViewActions end)
    {
        if (end.GetComponentType() != ComponentType.End)
            throw new ArgumentException("Can only add components of type end to the end tab.");
        
        if (ActiveViewComponents.ContainsKey(end.GetComponentType()))
            throw new ArgumentException("Tried to add the same view type component twice.");
        ActiveViewComponents.Add(end.GetComponentType(), end);
        _uiFitters.FitToFullscreen(end.GetRectTransform(), _tabComponentActions.GetContentParent());
    }

    public void EndAction()
    {
        throw new ArgumentException("Not intended for direct use, please use an implemented stepController class.");
    }

    private bool hasBeenConfigured = false;
    public void ConfigureTabs(List<IViewActions> components)
    {
        if (hasBeenConfigured)
            throw new ArgumentException(
                "This function is only intended to be called once, if you need to add more later, use the ADD function.");
        hasBeenConfigured = true;
        foreach (var component in components)
        {
            if (component.GetComponentType() == ComponentType.HuntHome)
            {
                AddHome(component);
            }
            else if (component.GetComponentType() == ComponentType.End)
            {
                AddEnd(component);
            }
            else
            {
                Add(component);
            }
        }
    }

    public void Add(IViewActions viewActions)
    {
        var key = viewActions.GetComponentType();
        if (ActiveViewComponents.ContainsKey(key))
            throw new ArgumentException("Tried to add the same view type component twice.");
        ActiveViewComponents.Add(key, viewActions);
        _uiFitters.FitToFullscreen(viewActions.GetRectTransform(), _tabComponentActions.GetContentParent());
        var tabController = _tabComponentActions.GetTabBarController();
        tabController.AddAndConfigure(viewActions, Display);
        Display(ComponentType.HuntHome);
    }

    public void ConfigureForStepType(IOldStepController oldStepController)
    {
        var possibleTypes = oldStepController.GetTypesInOrder();
        var currentStep = oldStepController.GetFirstStepTypeToShow();
        _tabComponentActions.GetTabBarController().ConfigureNeededButtons(possibleTypes);
        Display(currentStep);
    }

    public bool HasTypeInViews(ComponentType type)
    {
        return ActiveViewComponents.ContainsKey(type);
    }

    public void Display(ComponentType type)
    {
        if (ActiveViewComponents.ContainsKey(type))
        {
            if (!ActiveViewComponents[type].IsShown())
            {
                _tabComponentActions.GetTabBarController().UpdateButtonStates(type);

                foreach (var key in ActiveViewComponents.Keys)
                {
                    ActiveViewComponents[key].Hide();
                }

                ActiveViewComponents[type].Display();
            }
        }
    }

    public IViewActions GetComponentUIAction(ComponentType type)
    { 
        return ActiveViewComponents[type];
    }
}

public interface ITabComponentActions
{
    public void SetTabComponent(ITabComponent tabComponent);
    public RectTransform GetContentParent();
    public ITabBarController GetTabBarController();

    public void HomeAction();
}
public class TabComponentBehaviour : MonoBehaviour, ITabComponentActions
{
 [SerializeField] private RectTransform contentParent;
 [SerializeField] private TabBarControllerBehaviour tabBarControllerBehaviour;
 private ITabBarController _tabBarController;
 public ITabComponent _tabComponent { get; private set; }
 public ITabBarController GetTabBarController()
 {
     return _tabBarController ??= TabBarController.Factory(tabBarControllerBehaviour);
 }

 public void HomeAction()
 {
     _tabComponent.HomeAction();
 }

 public void SetTabComponent(ITabComponent tabComponent)
 {
     _tabComponent = new ComponentHelper<ITabComponent>().SetLogicInstance(tabComponent, _tabComponent);
 }

 public RectTransform GetContentParent()
    {
        if (contentParent == null)
            throw new ArgumentException("Value cannot be null");
        return contentParent;
    }
}
