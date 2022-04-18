using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Answers.MultipleChoice.Data.Icon;
using MultipleChoice.Icon.Transitions;
using UI.Answers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Answers.MultipleChoice.Buttons.Icon
{
    public interface IOldIconChoiceBtn
    {
        public void Configure(MultipleChoiceOldIconButton.Config config);
        public void Deselect(bool display);
        public void Select();
        public void ResetState(bool forceActive = false);
        public void TriggerAnimationCorrect(Action completed);
        public void TriggerAnimationWrong(Action completed);
        public void LockInteraction();
        public void UnlockInteraction();
        public void Hide();
        public void ButtonClickedAction();
    }
    
    [RequireComponent(typeof(MultipleChoiceIconStateTransitions))]
    public class MultipleChoiceOldIconButton : MonoBehaviour, IOldIconChoiceBtn
    {
        public static IOldIconChoiceBtn Factory(MultipleChoiceOldIconButton behaviour, Transform parent)
        {
            MultipleChoiceOldIconButton choiceOldBtn = Instantiate(behaviour, parent);
            return choiceOldBtn;
        }

        public class Dependencies
        {
            public IMultipleChoiceIconStateTransitions Transitions { get; set; }
            public Image IconImage { get; set; }
            public Image IconBackground { get; set; }
            public Button Button { get; set; }
        }

        public class Config
        {
            public IconWithValue iconWithValue { get; set; }
            public Action<IconWithValue> SelectionToggleAction;
            public MultipleChoiceState State { get; set; }
        }
        
        public Dependencies _dependencies { get; private set; }
        public void SetDependencies(Dependencies dependencies)
        {
            _dependencies = dependencies;
        }
        
        [FormerlySerializedAs("IconImage")] [SerializeField] private Image iconImage;
        [FormerlySerializedAs("IconBackground")] [SerializeField] private Image iconBackground;
        [SerializeField] private Button button;
        public void Awake()
        {
            var transitions = GetComponent<MultipleChoiceIconStateTransitions>();
            transitions.Configure(new MultipleChoiceIconStateTransitions.Config()
            {
                Ac = AnimationCurve.EaseInOut(0,0,1,1),
                AnimSpeedModifier = 2.0f,
                AnimThreshold = 0.1f,
                IconBackground = iconBackground,
                IdleColor = Color.white,
                SelectedColor = Color.green
            });
            SetDependencies(new Dependencies()
            {
                Transitions = transitions,
                IconImage = iconImage,
                IconBackground = iconBackground,
                Button = button,
            });
        }

        private Config _config;
        public void Configure(Config config)
        {
            _config = config;
            _dependencies.IconImage.sprite = _config.iconWithValue.Icon;
        }

        public void ButtonClickedAction()
        {
            if (!_dependencies.Transitions.IsAnimating())
            {
                if (_config.State == MultipleChoiceState.selected)
                {
                    Deselect(true);
                    _config.SelectionToggleAction.Invoke(_config.iconWithValue);
                }
                else if (_config.State == MultipleChoiceState.active)
                {
                    Select();
                    _config.SelectionToggleAction.Invoke(_config.iconWithValue);
                }
            }
        }

        public void ResetState(bool forceActive = false)
        {
            forceActive = gameObject.activeSelf == true || forceActive;
            Deselect(forceActive);
            UnlockInteraction();
        }
        
        public void Select()
        {
            if (gameObject.activeSelf)
                _dependencies.Transitions.StartAnimateSelected(SetSelected);
            else 
                Deselect(false);
        }

        private void SetSelected()
        {
            _config.State = MultipleChoiceState.selected;
        }
        public void Deselect(bool display)
        {
            this.gameObject.SetActive(display);
            if (gameObject.activeSelf)
                _dependencies.Transitions.StartAnimateDeselected(SetDeselected);
            else 
                SetDeselected();
        }

        private void SetDeselected()
        {
            if (gameObject.activeSelf)
            {
                _config.State = MultipleChoiceState.active;
            }
            else _config.State = MultipleChoiceState.hidden;
        }

        public void TriggerAnimationCorrect(Action completed)
        {
            if (gameObject.activeSelf)
                _dependencies.Transitions.StartAnimateFlash(() =>
                {
                    if(_config.State == MultipleChoiceState.selected)
                        Select();
                    completed.Invoke();
                }, Color.green);
        }

        public void TriggerAnimationWrong(Action completed)
        {
            if (gameObject.activeSelf)
                _dependencies.Transitions.StartAnimateFlash(() =>
                {
                    completed.Invoke();
                    ResetState();
                }, Color.red);
        }

        public void LockInteraction()
        {
            if(gameObject.activeSelf)
                StartCoroutine(LockInteractionEffect());
            else 
                _dependencies.Button.interactable = false;
        }

        public void UnlockInteraction()
        {
            _dependencies.Button.interactable = true;
        }

        private IEnumerator LockInteractionEffect()
        {
            yield return new WaitForSeconds(0.2f);
            _dependencies.Button.interactable = false;
        }

        public void Hide()
        {
            Deselect(false);
        }
    }
}
