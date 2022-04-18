using System;
using RHPackages.Core.Scripts;
using RHPackages.Core.Scripts.UI;
using Riddlehouse.Core.Helpers.Helpers;
using riddlehouse_libraries.products.Assets;
using TestRiddlehouseLibraries.LibraryTests.Products.Assets;
using TMPro;
using UnityEngine;

public interface ITextAnswerComponent
{
    public void Configure(TextAnswerAsset answerAsset, Action btnAction);
    public void PerformAction();
    public string GetAnswer();
    public void UpdateAnswer(string answer);
    public IViewActions GetComponentUIActions();
    
}

public class TextTextAnswerComponent : ITextAnswerComponent
{
    public static ITextAnswerComponent Factory(GameObject go)
    {
        var behaviour = new ComponentHelper<TextAnswerComponentBehaviour>().GetBehaviourIfExists(go);
        var component = new TextTextAnswerComponent(behaviour, behaviour);
        behaviour.SetAnswerComponent(component);
        return component;
    }

    private readonly ITextAnswerComponentActions _textAnswerComponentActions;
    private readonly IViewActions _viewActions;
    private Action _btnAction;
    private TextAnswerAsset _answerAsset;
    public TextTextAnswerComponent(ITextAnswerComponentActions textAnswerComponentActions, IViewActions viewActions)
    {
        _textAnswerComponentActions = textAnswerComponentActions;
        _viewActions = viewActions;
    }

    public void Configure(TextAnswerAsset answerAsset, Action btnAction)
    {
        _answerAsset = answerAsset;
        _textAnswerComponentActions.SetAnswerType(_answerAsset.GetAnswerType());
        _textAnswerComponentActions.Configure(_answerAsset);
        _btnAction = btnAction;
    }

    public Action GetBtnAction()
    {
        return _btnAction;
    }

    public void PerformAction()
    {
        if (_answerAsset.HasAnswer())
        {
            _btnAction.Invoke();    
        }
    }

    public string GetAnswer()
    {
        return _textAnswerComponentActions.GetAnswer();
    }

    public void UpdateAnswer(string answer)
    {
        _answerAsset.SetAnswer(answer);
        _textAnswerComponentActions.SetState(_answerAsset);
    }

    public IViewActions GetComponentUIActions()
    {
        return _viewActions;
    }
}

public interface ITextAnswerComponentActions
{
    public void Configure(TextAnswerAsset answerAsset);
    public void SetAnswerType(AnswerType type);
    public string GetAnswer();
    public void SetState(TextAnswerAsset answerAsset);
}
public enum AnswerState {Correct, Incorrect, None}
public class TextAnswerComponentBehaviour : MonoBehaviour, ITextAnswerComponentActions, IViewActions
{
    private AnswerType _type;
    [SerializeField] private TMP_InputField answerInputField;
    [SerializeField] private TextMeshProUGUI answerText;
    private ITextAnswerComponent _textAnswerComponent;
    private readonly ComponentType _componentType;
    public AnswerState _state { get; private set; }
    private TestableComponent<TMP_InputField> _answerInputField;
    private TestableComponent<TextMeshProUGUI> _answerText;
    public TextAnswerComponentBehaviour()
    {
        _state = AnswerState.None;
        _componentType = ComponentType.TextAnswer;
    }

    public void Awake()
    {
        SetDependencies(answerInputField, answerText);
    }
    public void SetDependencies(TMP_InputField inputField, TextMeshProUGUI text)
    {
        _answerInputField = new TestableComponent<TMP_InputField>(inputField);
        _answerText = new TestableComponent<TextMeshProUGUI>(text);
    }
    public void SetAnswerComponent(ITextAnswerComponent textAnswerComponent)
    {
        _textAnswerComponent = new ComponentHelper<ITextAnswerComponent>().SetLogicInstance(textAnswerComponent, _textAnswerComponent);
    }

    public AnswerType GetAnswerType()
    {
        return _type;
    }

    public void Configure(TextAnswerAsset answerAsset)
    {
        _answerInputField.Get().text = answerAsset.RecordedAnswer;
        SetState(answerAsset);
    }

    public void SetState(TextAnswerAsset answerAsset)
    {
        if (answerAsset.HasAnswer())
        {
            if (answerAsset.HasCorrectAnswer())
            {
                _state = AnswerState.Correct;
                _answerText.Get().color = Color.green;
            }
            else
            {
                _state = AnswerState.Incorrect;
                _answerText.Get().color = Color.red;
            }
        }
        else
        {
            _state = AnswerState.None;
            _answerText.Get().color = new Color() { a = 1, b = 0.196f, g = 0.196f, r = 0.196f };
        }
    }
    public void SetAnswerType(AnswerType type)
    {
        _type = type;
        switch (_type)
        {
            case AnswerType.InputTextfield:
                _answerInputField.Get().gameObject.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException($"Invalid AnswerType: {type}.");
        }
    }
    
    

    public void PerformAction()
    {
        _textAnswerComponent.PerformAction();
    }

    public void FitInView(RectTransform parent, IUIFitters uiFitters)
    {
        uiFitters.FitToFullscreen(GetRectTransform(), parent);
    }

    public void FitInView(RectTransform parent, IUIFitters uiFitters, int index)
    {
        throw new NotImplementedException();
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
        return _componentType;
    }

    public RectTransform GetRectTransform()
    {
        return (RectTransform) this.transform;
    }

    public string GetAnswer()
    {
        return _answerInputField.Get().text;
    }

    public void UpdateAnswer()
    {
        _textAnswerComponent.UpdateAnswer(_answerInputField.Get().text);
    }
}