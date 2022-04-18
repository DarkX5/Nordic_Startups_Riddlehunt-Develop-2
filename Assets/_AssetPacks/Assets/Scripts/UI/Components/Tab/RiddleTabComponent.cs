using System;
using System.Collections.Generic;
using RHPackages.Core.Scripts;
using RHPackages.Core.Scripts.UI;
using Riddlehouse.Core.Helpers.Helpers;
using riddlehouse_libraries.products.Assets;
using riddlehouse_libraries.products.Assets.AnswerAssets;
using TestRiddlehouseLibraries.LibraryTests.Products.Assets;
using UI.Answers;
using UnityEngine;
using UnityEngine.Serialization;

public interface IRiddleTabComponent : IViewActions
{
    public void Configure(string riddleText, IAnswerAsset answerAsset, List<Sprite> riddleImages, Action btnAction);

    // public void Configure(string riddleText, IAnswerAsset answerAsset, List<Sprite> riddleImages, Action btnAction);
}

public class RiddleTabComponent : MonoBehaviour, IRiddleTabComponent
{
    public static IRiddleTabComponent Factory(RiddleTabComponent prefab, Transform parent)
    {
        RiddleTabComponent behaviour = Instantiate(prefab, parent);
        behaviour.Initialize();
        return behaviour;
    }

    private void Initialize()
    {
        _UIFitters = new UIFitters();

        var riddleGameObject = Instantiate(riddleComponentBehaviourPrefab.gameObject); //instantiater ?
        var iRiddleComponent = RiddleComponent.Factory(riddleGameObject);
        iRiddleComponent.GetComponentUIActions().FitInView(contentParent, _UIFitters);
        iRiddleComponent.GetComponentUIActions().GetRectTransform().SetSiblingIndex(1);
        
        var iImageGridComponent = ImageGridComponentBehaviour.Factory(serializedImageGridComponentPrefab.gameObject, contentParent);
        var imageGridTransform = iImageGridComponent.GetComponentUIActions().GetRectTransform();
        imageGridTransform.SetSiblingIndex(2);
        
        var answerGameObject = Instantiate(textAnswerComponentBehaviourPrefab.gameObject);
        var iTextAnswerComponent = TextTextAnswerComponent.Factory(answerGameObject);
        iTextAnswerComponent.GetComponentUIActions().FitInView(contentParent, _UIFitters);
        iTextAnswerComponent.GetComponentUIActions().GetRectTransform().SetSiblingIndex(3);

        var iMultipleChoiceAnswerComponent = MultipleChoiceAnswerComponent.Factory(multipleChoiceAnswerComponentBehaviourPrefab, null);

        iMultipleChoiceAnswerComponent.GetComponentUIActions().FitInView(contentParent, _UIFitters);
        iMultipleChoiceAnswerComponent.GetComponentUIActions().GetRectTransform().SetSiblingIndex(4);

        var numericAnswerGameObject = Instantiate(numericAnswerComponentBehaviourPrefab.gameObject);
        var iNumericAnswerGameObject = NumericAnswerComponent.Factory(numericAnswerGameObject);
        iNumericAnswerGameObject.GetComponentUIActions().FitInView(contentParent, _UIFitters);
        iNumericAnswerGameObject.GetComponentUIActions().GetRectTransform().SetSiblingIndex(5);
        
        SetDependencies(serializedSelfRectTransform, iRiddleComponent, iTextAnswerComponent, iMultipleChoiceAnswerComponent, iNumericAnswerGameObject, iImageGridComponent);
    }
    
    [SerializeField] private RiddleHuntComponentBehaviour riddleComponentBehaviourPrefab;
    [SerializeField] private MultipleChoiceAnswerComponent multipleChoiceAnswerComponentBehaviourPrefab;
    [SerializeField] private TextAnswerComponentBehaviour textAnswerComponentBehaviourPrefab;
    [SerializeField] private NumericAnswerComponentBehaviour numericAnswerComponentBehaviourPrefab;
    [SerializeField] private ImageGridComponentBehaviour serializedImageGridComponentPrefab;
    
    [SerializeField] private RectTransform serializedSelfRectTransform;
    [SerializeField] private RectTransform contentParent;
    private TestableComponent<RectTransform> _selfRectTransform;
    
    private IRiddleComponent _riddleComponent;
    private ITextAnswerComponent _textAnswerComponent;
    private IMultipleChoiceAnswerComponent _multipleChoiceAnswerComponent;
    private INumericAnswerComponent _numericAnswerComponent;
    private IImageGridComponentActions _imageGridComponent;
    
    private UIFitters _UIFitters;
    
    public void SetDependencies(
        RectTransform selfRectTransform, 
        IRiddleComponent riddleComponent, 
        ITextAnswerComponent textAnswerComponent, 
        IMultipleChoiceAnswerComponent multipleChoiceAnswerComponent,
        INumericAnswerComponent numericAnswerComponent,
        IImageGridComponentActions imageGridComponentActions)
    {
        _selfRectTransform = new TestableComponent<RectTransform>(selfRectTransform);
        _riddleComponent = riddleComponent;
        _textAnswerComponent = textAnswerComponent;
        _multipleChoiceAnswerComponent = multipleChoiceAnswerComponent;
        _numericAnswerComponent = numericAnswerComponent;
        _imageGridComponent = imageGridComponentActions;
    }

    public void Configure(string riddleText, IAnswerAsset answerAsset, List<Sprite> riddleImages, Action btnAction)
    {
        ResetUI();
        _riddleComponent.Configure(riddleText);
        //_imageGridComponent.Configure(riddleImages);
        
        var answerType = answerAsset.GetAnswerType();
        
        switch (answerType)
        {
            case AnswerType.InputTextfield:
                _textAnswerComponent.Configure((TextAnswerAsset)answerAsset, btnAction);
                _textAnswerComponent.GetComponentUIActions().Display();
                break;
            case AnswerType.NumericTextField:
                _numericAnswerComponent.Configure((NumericAnswerAsset)answerAsset, btnAction);
                _numericAnswerComponent.GetComponentUIActions().Display();
                break;
            case AnswerType.MultipleChoiceText:
                _multipleChoiceAnswerComponent.Configure((MultipleChoiceTextAnswerAsset)answerAsset, btnAction);
                _multipleChoiceAnswerComponent.GetComponentUIActions().Display();
                break;
            case AnswerType.MultipleChoiceTextIcon:
                _multipleChoiceAnswerComponent.Configure((MultipleChoiceTextIconAnswerAsset)answerAsset, btnAction);
                _multipleChoiceAnswerComponent.GetComponentUIActions().Display();
                break;
            default:
                throw new ArgumentException("Answertype not defined, "+answerType+" not allowed");
        }
    }

    private void ResetUI()
    {
        _textAnswerComponent.GetComponentUIActions().Hide();
        _multipleChoiceAnswerComponent.GetComponentUIActions().Hide();
        _numericAnswerComponent.GetComponentUIActions().Hide();
        _imageGridComponent.Hide();
    }

    public void FitInView(RectTransform parent, IUIFitters uiFitters)
    {
        uiFitters.FitToFullscreen(_selfRectTransform.Get(), parent);
    }

    public void FitInView(RectTransform parent, IUIFitters uiFitters, int index)
    {
        uiFitters.FitToFullscreen(_selfRectTransform.Get(), parent);
        this.transform.SetSiblingIndex(index);
    }

    public void Display()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public bool IsShown()
    {
        return this.gameObject.activeSelf;
    }

    public ComponentType GetComponentType()
    {
        return ComponentType.RiddleTab;
    }

    public RectTransform GetRectTransform()
    {
        return _selfRectTransform.Get();
    }
}