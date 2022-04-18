using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public enum ConditionalStepButtonState {Complete, Incomplete, Hidden, Disabled, Highlighted}

public interface IConditionalStepBtn
{
    public void Configure(ConditionalStepButtonState newState, string stepId, string labelTxt, Action<string> buttonAction);
    public void Hide();
    public void Display();
    public void PerformAction();
    public ConditionalStepButtonState GetState();
    public GameObject GetGameObject();
}

public interface IConditionalStepButtonActions
{
    public void Configure(ConditionalStepButtonState newState, string stepId, string labelTxt, Action<string> buttonAction);
    public void Hide();
    public void Display();
    public void PerformAction();
    public GameObject GetGameObject();
}

public class ConditionalStepBtn: IConditionalStepBtn
{
    public static IConditionalStepBtn Factory(GameObject go)
    {
        var stepBtnInterfaces = go.GetComponent<ConditionalStepBtnBehavior>();

        if (stepBtnInterfaces == null)
        {
            throw new ArgumentException("StepBtnBehavior missing on prefab");
        }

        var stepBtn = new ConditionalStepBtn(stepBtnInterfaces);
        stepBtnInterfaces.SetStepButton(stepBtn);
        return stepBtn;
    }
    
    private ConditionalStepButtonState _state;
    public readonly IConditionalStepButtonActions _stepButtonActions;
    public ConditionalStepBtn(IConditionalStepButtonActions stepButtonActions)
    {
        _stepButtonActions = stepButtonActions;
        _state = ConditionalStepButtonState.Incomplete;
    }

    public void Configure(ConditionalStepButtonState newState, string stepId, string labelTxt, Action<string> buttonAction)
    {
        _stepButtonActions.Configure(newState, stepId, labelTxt, buttonAction);
        _state = newState;
    }

    public void Hide()
    {
        _stepButtonActions.Hide();
    }

    public void Display()
    {
        _stepButtonActions.Display();
    }

    public void PerformAction()
    {
        _stepButtonActions.PerformAction();
    }

    public ConditionalStepButtonState GetState()
    {
        return _state;
    }

    public GameObject GetGameObject()
    {
        return _stepButtonActions.GetGameObject();
    }
}
public class ConditionalStepBtnBehavior : MonoBehaviour, IConditionalStepButtonActions
{
    public static IConditionalStepBtn Factory(ConditionalStepBtnBehavior prefab, RectTransform parent)
    {
        ConditionalStepBtnBehavior behaviour = Instantiate(prefab,parent);
        var stepBtn = new ConditionalStepBtn(behaviour);
        behaviour.SetStepButton(stepBtn);
        return stepBtn;
    }
    
    private Action<string> _buttonAction = (id) => { };
    private string _stepId = "";
    [SerializeField] private Button btn;
    [FormerlySerializedAs("_animator")] [SerializeField] private Animator animator;
    [FormerlySerializedAs("_label")] [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private ConditionalStepButtonState __state;
    private IConditionalStepBtn _stepBtn;

    public void SetStepButton(IConditionalStepBtn stepBtn)
    {
        if(_stepBtn != null)
            throw new ArgumentException("Can only be set once.");
        _stepBtn = stepBtn;
    }

    public void Start()
    {
        SetState(__state);
    }
    
    public void Configure(ConditionalStepButtonState newState, string stepId, string labelTxt, Action<string> buttonAction)
    {
        _stepId = stepId;
        label.text = labelTxt;
        _buttonAction = buttonAction;

        SetState(newState);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public void Display()
    {
        this.gameObject.SetActive(true);
    }

    private void SetState(ConditionalStepButtonState state)
    {
        __state = state;
        if (state == ConditionalStepButtonState.Highlighted)
        {
            this.gameObject.SetActive(true);
            btn.interactable = true;
            animator.SetBool("Highlight", true);
            animator.SetBool("Disable", false);
            animator.SetBool("Idle", false);

        }
        else if (state == ConditionalStepButtonState.Hidden)
        {
            btn.interactable = false;
            this.gameObject.SetActive(false);
        }
        else if (state == ConditionalStepButtonState.Disabled)
        {
            this.gameObject.SetActive(true);
            btn.interactable = false;
            animator.SetBool("Disable", true);
            animator.SetBool("Highlight", false);
            animator.SetBool("Idle", false);
        }
        else if (state == ConditionalStepButtonState.Incomplete)
        {
            this.gameObject.SetActive(true);
            btn.interactable = true;
            animator.SetBool("Idle", true);
            animator.SetBool("Completed", false);
            animator.SetBool("Disable", false);
            animator.SetBool("Highlight", false);
        }
        else if(state == ConditionalStepButtonState.Complete)
        {
            this.gameObject.SetActive(true);
            btn.interactable = true;
            animator.SetBool("Idle", true);
            animator.SetBool("Completed", true);
            animator.SetBool("Disable", false);
            animator.SetBool("Highlight", false);
        }
        else
        {
            throw new ArgumentException("Don't know how to handle that state: "+state);
        }
    }
    public void OnEnable()
    {
        SetState(__state);
    }
    public void PerformAction()
    {
        _buttonAction.Invoke(_stepId);
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
}
