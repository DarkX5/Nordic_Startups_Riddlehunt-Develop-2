using System;
using System.Collections.Generic;
using System.Linq;
using RHPackages.Core.Scripts;
using RHPackages.Core.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

public interface ITabBarController
{
    public void AddAndConfigure(IViewActions viewActions, Action<ComponentType> btnAction);
    public void UpdateButtonStates(ComponentType stepToHighlight);
    public void ConfigureNeededButtons(List<ComponentType> stepsInOrder);
    public bool HasKey(ComponentType key);
    public ComponentType GetHighlighted();


}
public class TabBarController : ITabBarController
{
    public static ITabBarController Factory(ITabBarControllerActions  tabBarControllerActions)
    {
        return new TabBarController(tabBarControllerActions);
    }
    public float ActiveWidth { get; private set; }
    public float HighlightedWidth { get; private set; }

    public readonly ITabBarControllerActions TabBarControllerActions; 
    private Dictionary<ComponentType,ITabBtn> ActiveTabBtns { get; set; }
    private Dictionary<ComponentType, ITabBtn> InactiveTabBtns { get; set; }
    public ComponentType highlightedTab;

    public TabBarController(ITabBarControllerActions tabBarControllerActions)
    {
        TabBarControllerActions = tabBarControllerActions;
        ActiveTabBtns = new Dictionary<ComponentType, ITabBtn>();
        InactiveTabBtns = new Dictionary<ComponentType, ITabBtn>();
    }
    
    public void AddAndConfigure(IViewActions viewActions, Action<ComponentType> btnAction)
    {
        var key = viewActions.GetComponentType();
        if (!HasKey(key))
        {
            var tabBtn = TabBarControllerActions.InstantiateButton(viewActions);
            tabBtn.Configure(TabBarControllerActions.MapButtonTitle(key), key, btnAction);
            ActiveTabBtns.Add(key, tabBtn);
            CalculateWidth();
            tabBtn.SetTabButtonState(TabButtonState.Hidden, ActiveWidth); //default state: active
        }
    }
    int highestIdx = 0;
    public void UpdateButtonStates(ComponentType stepToHighlight)
    {
        if(!ActiveTabBtns.ContainsKey(stepToHighlight))
            MoveTabBtnToActive(stepToHighlight);
        
        //if stepcontroller is set -> update based on step.
        //Get highlightedTab's index position, evaluate the rest based on that.
        // -- story, 1 - active
        // -- riddle, 2 - highlighted
        // -- scanning, hide all TAB UI.
        // -- validation, 3 - hidden
        if (ActiveTabBtns.ContainsKey(stepToHighlight))
        {
            var currentIdx = ActiveTabBtns[stepToHighlight].GetSiblingIndex();
            if (currentIdx > highestIdx)
                highestIdx = currentIdx;
        }

        if (stepToHighlight == ComponentType.HuntHome)
            highestIdx = 0;
        
        foreach (var key in ActiveTabBtns.Keys)
        {
            if (ActiveTabBtns[key] == null)
                throw new ArgumentException("Requested ComponentType doesn't exist.");

            if (key == stepToHighlight)
                ActiveTabBtns[key].SetTabButtonState(TabButtonState.Highlighted, HighlightedWidth);
            else if(ActiveTabBtns[key].GetSiblingIndex() <= highestIdx)
                ActiveTabBtns[key].SetTabButtonState(TabButtonState.Active, ActiveWidth);
            else 
                ActiveTabBtns[key].SetTabButtonState(TabButtonState.Hidden, ActiveWidth);
        }
        highlightedTab = stepToHighlight;
        TabBarControllerActions.RebuildLayout();
    }

    public void ConfigureNeededButtons(List<ComponentType> stepsInOrder)
    {
        foreach (var step in stepsInOrder)
        {
            if (InactiveTabBtns.ContainsKey(step))
            {
                MoveTabBtnToActive(step);
            }
        }

        List<ComponentType> typesToHide = new List<ComponentType>();
        foreach (var activeTabBtn in ActiveTabBtns)
        {
            if(!stepsInOrder.Contains(activeTabBtn.Key))
                typesToHide.Add(activeTabBtn.Key);
        }

        foreach (var type in typesToHide)
        {
            MoveTabBtnToInactive(type);
        }

        int i = 1; //home button should always be sibbling 0.
        foreach (var type in stepsInOrder)
        {
            ActiveTabBtns[type].SetSiblingIndex(i);
            i++;
        }
        highestIdx = 0;
    }

    private void MoveTabBtnToInactive(ComponentType key)
    {
        if (ActiveTabBtns.ContainsKey(key))
        {
            ActiveTabBtns[key].SetTabButtonState(TabButtonState.Hidden, ActiveWidth);
            InactiveTabBtns.Add(key, ActiveTabBtns[key]);
            ActiveTabBtns.Remove(key);
        }
    }
    private void MoveTabBtnToActive(ComponentType key)
    {
        if (InactiveTabBtns.ContainsKey(key))
        {
            InactiveTabBtns[key].SetTabButtonState(TabButtonState.Active, ActiveWidth);
            ActiveTabBtns.Add(key, InactiveTabBtns[key]);
            InactiveTabBtns.Remove(key);
        }
    }

    private void CalculateWidth()
    {
        var tabBarWidth = TabBarControllerActions.TabBarCurrentSize().x;
        var count = ActiveTabBtns.Count;
        var spacingRules = TabBarControllerActions.GetLayoutSpacingRules();
        var spacing = spacingRules.z*(count); //-1 after home button is integrated;
        var margins = spacingRules.x + spacingRules.y;
        var availableButtonSpace = tabBarWidth - spacing - margins;
        var spacePrBtn = availableButtonSpace / (count + 1);
        HighlightedWidth = spacePrBtn * 1.15f;
        ActiveWidth = spacePrBtn * (1f-(0.15f/count));
    }

    public bool HasKey(ComponentType key)
    {
        return ActiveTabBtns.ContainsKey(key);
    }

    public ComponentType GetHighlighted()
    {
        return highlightedTab;
    }
}

public interface ITabBarControllerActions
{
    ITabBtn InstantiateButton(IViewActions viewActions);
    Vector2 TabBarCurrentSize();
    public Vector3 GetLayoutSpacingRules();
    public void RebuildLayout();
    public string MapButtonTitle(ComponentType type);
}

public class TabBarControllerBehaviour : MonoBehaviour, ITabBarControllerActions
{
    [SerializeField] private TabBtnBehaviour btnBehaviourPrefab;
    [SerializeField] private RectTransform tabBtnContentParent;
    [SerializeField] private HorizontalLayoutGroup layoutGroup;
    [SerializeField] private TabButtonTitlesBehaviour _tabButtonTitlesBehaviour;
    public ITabBtn InstantiateButton(IViewActions viewActions)
    {
        var btn = Instantiate(btnBehaviourPrefab.gameObject);
        ITabBtn tabBtn = TabBtn.Factory(btn, tabBtnContentParent);
        return tabBtn;
    }

    public Vector2 TabBarCurrentSize()
    {
        return new Vector2(tabBtnContentParent.rect.width, tabBtnContentParent.rect.height);
    }

    /// <summary>
    /// Returns a triple wrapped in a vector 3.
    /// </summary>
    /// <returns>left, right, spacing.</returns>
    public Vector3 GetLayoutSpacingRules()
    {
        return new Vector3(layoutGroup.padding.left, layoutGroup.padding.right, layoutGroup.spacing);
    }

    public void RebuildLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(tabBtnContentParent);
    }

    public string MapButtonTitle(ComponentType type)
    {
        if (!_tabButtonTitlesBehaviour.titleMap.ContainsKey(type))
            throw new ArgumentException("Please remember to designate a title for the button in tabButtonTitlesBehaviour.");
        return _tabButtonTitlesBehaviour.titleMap[type];
    }
}
