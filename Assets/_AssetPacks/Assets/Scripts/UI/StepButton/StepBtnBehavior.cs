using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public enum StepButtonState {previous, next, future}

public interface IStepBtn
{
    public void Configure(int stepReached, int stepIdx, string labelTxt, Action<int> buttonAction);
    public void PerformAction();
    public StepButtonState GetState();
    public GameObject GetGameObject();
}

public interface IStepButtonActions
{
    public void Configure(StepButtonState state, int stepIdx, string labelTxt, Action<int> buttonAction);
    public void PerformAction();
    public GameObject GetGameObject();
}

public class StepBtn: IStepBtn
{
    public static StepBtn Factory(GameObject go)
    {
        var StepBtninterfaces = go.GetComponent<StepBtnBehavior>();

        if (StepBtninterfaces == null)
        {
            throw new ArgumentException("StepBtnBehavior missing on prefab");
        }

        var stepBtn = new StepBtn(StepBtninterfaces);
        StepBtninterfaces.SetStepButton(stepBtn);
        return stepBtn;
    }
    
    private StepButtonState _state;
    public readonly IStepButtonActions _stepButtonActions;
    public StepBtn(IStepButtonActions stepButtonActions)
    {
        _stepButtonActions = stepButtonActions;
        _state = StepButtonState.previous;
    }

    public void Configure(int stepReached, int stepIdx, string labelTxt, Action<int> buttonAction)
    {
        if (stepReached == stepIdx)
            _state = StepButtonState.next;
        else if(stepReached > stepIdx)
            _state = StepButtonState.previous;
        else if (stepReached < stepIdx)
            _state = StepButtonState.future;
        else
            throw new ArgumentException("Invalid State");
        _stepButtonActions.Configure(_state, stepIdx, labelTxt, buttonAction);
    }

    public void PerformAction()
    {
        _stepButtonActions.PerformAction();
    }

    public StepButtonState GetState()
    {
        return _state;
    }

    public GameObject GetGameObject()
    {
        return _stepButtonActions.GetGameObject();
    }
}
public class StepBtnBehavior : MonoBehaviour, IStepButtonActions
{
    private Action<int> _buttonAction = (idx) => { };
    private int _stepIdx = -1;
    [SerializeField] private Button btn;
    [FormerlySerializedAs("_animator")] [SerializeField] private Animator animator;
    [FormerlySerializedAs("_label")] [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private StepButtonState __state;
    private IStepBtn _stepBtn;

    public void SetStepButton(IStepBtn stepBtn)
    {
        if(_stepBtn != null)
            throw new ArgumentException("Can only be set once.");
        _stepBtn = stepBtn;
    }

    public void Start()
    {
        SetState(__state);
    }
    
    public void Configure(StepButtonState state, int stepIdx, string labelTxt, Action<int> buttonAction)
    {
        _stepIdx = stepIdx;
        label.text = labelTxt;
        _buttonAction = buttonAction;

        SetState(state);
    }

    private void SetState(StepButtonState state)
    {
        __state = state;
        if (state == StepButtonState.next)
        {
            btn.interactable = true;
            animator.SetBool("Highlight", true);
            animator.SetBool("Disable", false);
            animator.SetBool("Idle", false);

        }
        else if (state == StepButtonState.future)
        {
            btn.interactable = false;
            animator.SetBool("Disable", true);
            animator.SetBool("Highlight", false);
            animator.SetBool("Idle", false);
        }
        else if(state == StepButtonState.previous)
        {
            btn.interactable = true;
            animator.SetBool("Idle", true);
            animator.SetBool("Disable", false);
            animator.SetBool("Highlight", false);
        }
        else
        {
            throw new ArgumentException("Don't know how to handle that state: "+state);
        }
    }

    public void PerformAction()
    {
        _buttonAction.Invoke(_stepIdx);
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
}
