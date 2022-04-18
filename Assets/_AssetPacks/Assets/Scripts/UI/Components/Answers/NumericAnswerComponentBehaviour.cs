using System;
using System.Linq;
using Riddlehouse.Core.Helpers.Helpers;
using ModestTree;
using RHPackages.Core.Scripts;
using RHPackages.Core.Scripts.UI;
using riddlehouse_libraries.products.Assets;
using TMPro;
using UnityEngine;

public interface INumericAnswerComponent
{
    public void Configure(NumericAnswerAsset answerAsset, Action btnAction);
    public void PerformAction();
    public IViewActions GetComponentUIActions();
    public float GetCurrentValue();
    public void SetCurrentValue(string currentValue);
}

public class NumericAnswerComponent : INumericAnswerComponent
{
    private readonly INumericAnswerComponentActions _numericAnswerComponentActions;
    private readonly IViewActions _viewActions;
    private Action _btnAction;
    private NumericAnswerAsset _answerAsset;

    public static INumericAnswerComponent Factory(GameObject go)
    {
        var behaviour = new ComponentHelper<NumericAnswerComponentBehaviour>().GetBehaviourIfExists(go);
        var component = new NumericAnswerComponent(behaviour, behaviour);
        behaviour.SetAnswerComponent(component);
        return component;
    }

    public NumericAnswerComponent(INumericAnswerComponentActions numericAnswerComponentActions,
        IViewActions viewActions)
    {
        _numericAnswerComponentActions = numericAnswerComponentActions;
        _viewActions = viewActions;
    }

    public void Configure(NumericAnswerAsset answerAsset, Action btnAction)
    {
        _answerAsset = answerAsset;
        _numericAnswerComponentActions.Configure(answerAsset);
        _btnAction = btnAction;
    }

    public void PerformAction()
    {
        if (_answerAsset.HasAnswer())
            _btnAction.Invoke();
    }

    private float StringToFloat(string answer)
    {
        if (string.IsNullOrEmpty(answer)) return float.MaxValue;
        var preppedString = PrepAnswerString(answer);
        return preppedString.IsEmpty() ? 0 : float.Parse(preppedString);
    }

    public IViewActions GetComponentUIActions()
    {
        return _viewActions;
    }

    public float GetCurrentValue()
    {
        return Math.Abs(_answerAsset.RecordedAnswer - float.MaxValue) < 0.1f? 0 : _answerAsset.RecordedAnswer;
    }

    private string PrepAnswerString(string answer)
    {
        return answer.Where(c => char.IsDigit(c) || c == '-').Aggregate(string.Empty, (current, c) => current + c);
    }

    public void SetCurrentValue(string currentValue)
    {
        _answerAsset.SetAnswer(StringToFloat(currentValue));
        _numericAnswerComponentActions.SetState(_answerAsset);
    }
}

public interface INumericAnswerComponentActions
{
    public void Configure(NumericAnswerAsset answerAsset);
    public void SetState(NumericAnswerAsset answerAsset);
}

public class NumericAnswerComponentBehaviour : MonoBehaviour, INumericAnswerComponentActions, IViewActions
{
    public int incrementValue = 1;
    [SerializeField] private TMP_InputField serializedAnswerInputField;
    [SerializeField] private TextMeshProUGUI serializedAnswerText;

    private INumericAnswerComponent _numericAnswerComponent;
    private readonly ComponentType _componentType;
    public AnswerState State { get; private set; }
    private TestableComponent<TMP_InputField> numericAnswerInputField;
    private TestableComponent<TextMeshProUGUI> _answerText;

    public NumericAnswerComponentBehaviour()
    {
        State = AnswerState.None;
        _componentType = ComponentType.NumericAnswer;
    }
    public void Awake()
    {
        SetDependencies(serializedAnswerInputField, serializedAnswerText);
    }

    public void Start()
    {
        serializedAnswerInputField.onValueChanged.AddListener(delegate
            {
                _numericAnswerComponent.SetCurrentValue(serializedAnswerInputField.text);
            }
        );
    }

    public void SetDependencies(TMP_InputField tmpInputField, TextMeshProUGUI text)
    {
        numericAnswerInputField = new TestableComponent<TMP_InputField>(tmpInputField);
        _answerText = new TestableComponent<TextMeshProUGUI>(text);
    }

    public void SetAnswerComponent(INumericAnswerComponent numericAnswerComponent)
    {
        _numericAnswerComponent =
            new ComponentHelper<INumericAnswerComponent>().SetLogicInstance(numericAnswerComponent,
                _numericAnswerComponent);
    }

    public void Configure(NumericAnswerAsset answerAsset)
    {
        numericAnswerInputField.Get().text = answerAsset.HasAnswer() ? answerAsset.RecordedAnswer.ToString() : "";
        SetState(answerAsset);
    }
    
    public void SetState(NumericAnswerAsset answerAsset)
    {
        if (answerAsset.HasAnswer())
        {
            if (answerAsset.HasCorrectAnswer())
            {
                State = AnswerState.Correct;
                _answerText.Get().color = Color.green;
            }
            else
            {
                State = AnswerState.Incorrect;
                _answerText.Get().color = Color.red;
            }
        }
        else
        {
            State = AnswerState.None;
            _answerText.Get().color = new Color() { a = 1, b = 0.196f, g = 0.196f, r = 0.196f };
        }
    }

    public void PerformAction()
    {
        _numericAnswerComponent.PerformAction();
    }

    public void IncreaseValueButtonAction()
    {
        ChangeCurrentValue(incrementValue);
        UpdateAnswerText();
    }

    public void DecreaseValueButtonAction()
    {
        ChangeCurrentValue(incrementValue * -1);
        UpdateAnswerText();
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

    private void UpdateAnswerText()
    {
        numericAnswerInputField.Get().text = _numericAnswerComponent.GetCurrentValue().ToString();
    }

    private void ChangeCurrentValue(int value)
    {
        var currentValue = _numericAnswerComponent.GetCurrentValue();
        ((NumericAnswerComponent) _numericAnswerComponent).SetCurrentValue((currentValue + value).ToString());
    }
}