using System;
using System.Collections;
using System.Collections.Generic;
using riddlehouse_libraries.products.models.DTOs;
using UnityEngine;

public interface IConditionalStepListView
{
    public void Configure(ConditionalStepListView.Config config);
}

public interface IConditionalStepBtnInstantiater
{
    public IConditionalStepBtn Create();
}
public class ConditionalStepBtnInstantiater: IConditionalStepBtnInstantiater
{
    private ConditionalStepBtnBehavior _prefab;
    private RectTransform _parent;

    public ConditionalStepBtnInstantiater(ConditionalStepBtnBehavior prefab, RectTransform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }

    public IConditionalStepBtn Create()
    {
        return ConditionalStepBtnBehavior.Factory(_prefab, _parent);
    }
}


[RequireComponent(typeof(BasicComponentDisplayController))]
public class ConditionalStepListView : MonoBehaviour, IConditionalStepListView
{
    public static IConditionalStepListView Factory(ConditionalStepListView prefab, RectTransform parent)
    {
        ConditionalStepListView behaviour = Instantiate(prefab);
        behaviour.Initialize();
        behaviour._dependencies.DisplayController.FitToScreen(parent);
        return behaviour;
    }

    public class Dependencies
    {
        public IComponentDisplayController DisplayController { get; set; }
        public IConditionalStepBtnInstantiater ConditionalStepBtnInstantiater { get; set; }
        public IConditionalStepList ConditionalStepList { get; set; }
    }

    public class Config
    {
        public Action<string> ListButtonAction { get; set; }
        public IHuntSteps HuntSteps { get; set; }
    }

    public void Initialize()
    {
        var displayController = GetComponent<BasicComponentDisplayController>();
        displayController.Initialize();
        SetDependencies(new Dependencies()
        {
            DisplayController = displayController,
            ConditionalStepBtnInstantiater = new ConditionalStepBtnInstantiater(conditionalStepButtonPrefab,contentParent),
            ConditionalStepList = new ConditionalStepList()
        });
    }

    [SerializeField] private ConditionalStepBtnBehavior conditionalStepButtonPrefab;
    [SerializeField] private RectTransform contentParent;
    private List<IConditionalStepBtn> _buttons;

    public Dependencies _dependencies { get; private set; }

    public void SetDependencies(Dependencies dependencies)
    {
        _dependencies = dependencies;
        _buttons = new List<IConditionalStepBtn>();
    }

    private Config _config;
    public void Configure(Config config)
    {
        _config = config;
        HideAllButtons();
        PopulateButtonList();
        ConfigureButtonList();
    }

    private void PopulateButtonList()
    {
        var lengthOfHunt = _config.HuntSteps.GetLengthOfHunt();
        for (int i = _buttons.Count; i < lengthOfHunt; i++)
        {
            _buttons.Add(_dependencies.ConditionalStepBtnInstantiater.Create());
        }
    }

    private void ConfigureButtonList()
    {
        _dependencies.ConditionalStepList.ConfigureStepList(_config.HuntSteps, _buttons, ListButtonAction);
    }

    private void HideAllButtons()
    {
        for (int i = 0; i < _buttons.Count; i++)
        {
            _buttons[i].Hide();
        }
    }

    private void ListButtonAction(string id)
    {
        _config.ListButtonAction(id);
    }
}
