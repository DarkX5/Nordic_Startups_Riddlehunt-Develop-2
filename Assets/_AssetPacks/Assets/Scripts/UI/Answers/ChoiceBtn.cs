using System;
using System.Collections;
using System.Collections.Generic;
using Riddlehouse.Core.Helpers.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Answers
{
    public enum MultipleChoiceState
    {
        selected,
        active,
        hidden
    };

    public enum ChoiceButtonType
    {
        Text,
        Icon
    }
    public interface IChoiceBtn
    {
        public ChoiceButtonType GetType();
        public void Configure(string choice, Action<string> buttonAction);
        public void SetState(MultipleChoiceState state);
        public MultipleChoiceState GetState();
        public string GetChoiceValue();
        public void Select();
    }

    public interface IChoiceBtnAnimator
    {
        public void SetBool(string variable, bool value);
    }

    public class ChoiceBtnAnimator : IChoiceBtnAnimator
    {
        private Animator _animator;

        public ChoiceBtnAnimator(Animator animator)
        {
            _animator = animator;
        }

        public void SetBool(string variable, bool value)
        {
            _animator.SetBool(variable, value);
        }
    }

    public class ChoiceBtn : MonoBehaviour, IChoiceBtn
    {
        private Action<string> _buttonAction;
        private MultipleChoiceState _state;

        public class Dependencies
        {
            public IChoiceBtnAnimator Animator { get; set; }
        }

        public Dependencies _dependencies { get; private set; }
        
        public void SetDependencies(Dependencies dependencies)
        {
            _dependencies = dependencies;
        }

        private string choiceValue = null;

        public virtual ChoiceButtonType GetType()
        {
            throw new NotImplementedException();
        }

        public void Configure(string choice, Action<string> buttonAction)
        {
            _buttonAction = buttonAction;
            choiceValue = choice;
            SetState(MultipleChoiceState.active);
        }

        public string GetChoiceValue()
        {
            return choiceValue;
        }

        public void SetState(MultipleChoiceState state)
        {
            switch (state)
            {
                case MultipleChoiceState.selected:
                    Display();

                    _dependencies.Animator.SetBool("IsHighlighted", true);
                    _dependencies.Animator.SetBool("IsIdle", false);
                    break;
                case MultipleChoiceState.active:
                    Display();
                    _dependencies.Animator.SetBool("IsHighlighted", false);
                    _dependencies.Animator.SetBool("IsIdle", true);
                    break;
                case MultipleChoiceState.hidden:
                    Hide();
                    break;
            }

            _state = state;
        }

        public MultipleChoiceState GetState()
        {
            return _state;
        }

        public void OnEnable()
        {
            SetState(GetState());
        }

        public void Select()
        {
            _buttonAction.Invoke(choiceValue);
        }

        public void Display()
        {
            if (!this.gameObject.activeSelf)
                this.gameObject.SetActive(true);
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }
    }
}