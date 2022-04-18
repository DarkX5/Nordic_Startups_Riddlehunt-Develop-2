using System;
using System.Collections;
using System.Collections.Generic;
using RHPackages.Core.Scripts.UI;
using RHPackages.Core.Scripts.UIHelpers;
using Riddlehouse.Core.Helpers.Helpers;
using riddlehouse_libraries.products.Assets.AnswerAssets;
using UnityEngine;
using UnityEngine.Serialization;

public interface IWispContainerAnswerView : IViewActions
{
    public void Configure(IMultipleChoiceTextAnswerAsset answerAsset, string riddleText, Color characterFrameColor,
        Sprite characterIcon, Action acceptAction, Action abortAction);
}
public class WispContainerAnswerView : BaseFullscreenView, IWispContainerAnswerView
{
    public static IWispContainerAnswerView Factory(GameObject prefab, RectTransform parent)
    {
        if (prefab.GetComponent<WispContainerAnswerView>() == null)
            throw new ArgumentException("Prefab doesn't match expectations.");
        
        WispContainerAnswerView behaviour = Instantiate(prefab, parent).GetComponent<WispContainerAnswerView>();
        behaviour.Initialize();
        return behaviour;
    }
    
    public class Dependencies
    {
        public Color SelectedColor;
        public Color IdleColor;
        public IWispAnswerComponent WispAnswerComponent;
        public IWispFooter WispFooter;
        public SetTextElement RiddleTextField;
    }

    public void Initialize()
    {
        wispAnswerComponent.Initialize();
        wispFooter.Initialize();
        SetDependencies(new Dependencies()
        {
            SelectedColor = selectedColor,
            IdleColor = idleColor,
            WispAnswerComponent = wispAnswerComponent,
            WispFooter = wispFooter,
            RiddleTextField = riddleTextField,
        });
    }

    [SerializeField] private Color selectedColor;
    [SerializeField] private Color idleColor;
    [SerializeField] private WispFooter wispFooter;
    [SerializeField] private WispAnswerComponent wispAnswerComponent;
    [SerializeField] private SetTextElement riddleTextField;
    public Dependencies _dependencies { get; private set; }
    public void SetDependencies(Dependencies dependencies)
    {
        _dependencies = dependencies;
    }

    private IMultipleChoiceTextAnswerAsset _answerAsset;
    private Action _acceptAction;
    private Action _abortAction;
    public void Configure(
        IMultipleChoiceTextAnswerAsset answerAsset, 
        string riddleText, 
        Color characterFrameColor, 
        Sprite characterIcon, 
        Action acceptAction, 
        Action abortAction)
    {
        _answerAsset = answerAsset;
        _acceptAction = acceptAction;
        _abortAction = abortAction;
        _dependencies.WispAnswerComponent.Configure(answerAsset, _dependencies.SelectedColor, _dependencies.IdleColor);
        _dependencies.WispFooter.Configure(characterFrameColor, characterIcon, AcceptAction, AbortAction);
        _dependencies.RiddleTextField.Configure(riddleText);
    }

    private void AcceptAction()
    {
        _acceptAction.Invoke();
    }

    private void AbortAction()
    {
        _answerAsset?.ClearSession();
        _abortAction.Invoke();
    }
}
