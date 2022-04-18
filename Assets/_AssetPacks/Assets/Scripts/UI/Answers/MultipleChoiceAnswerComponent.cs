using System;
using System.Collections.Generic;
using Riddlehouse.Core.Helpers.Helpers;
using Helpers;
using RHPackages.Core.Scripts;
using RHPackages.Core.Scripts.UI;
using riddlehouse_libraries.products.Assets;
using riddlehouse_libraries.products.Assets.AnswerAssets;
using TMPro;
using UnityEngine;

namespace UI.Answers
{
    public interface IChoiceButtonInstantiator
    {
        public ITextChoiceBtn CreateTextButton(int sibblingIndex);
        public IIconChoiceBtn CreateIconButton(int sibblingIndex);
    }

    public class ChoiceButtonInstantiator : IChoiceButtonInstantiator
    {
        private TextChoiceBtn _stringChoiceBtnPrefab;
        private IconChoiceBtn _iconChoiceBtnPrefab;

        private Transform _parent;
        public ChoiceButtonInstantiator(TextChoiceBtn stringChoiceBtnPrefab, IconChoiceBtn iconChoiceBtnPrefab, Transform parent)
        {
            _stringChoiceBtnPrefab = stringChoiceBtnPrefab;
            _parent = parent;
            _iconChoiceBtnPrefab = iconChoiceBtnPrefab;
        }

        public ITextChoiceBtn CreateTextButton(int sibblingIndex)
        {
            return TextChoiceBtn.Factory(_stringChoiceBtnPrefab, _parent, sibblingIndex);
        }

        public IIconChoiceBtn CreateIconButton(int sibblingIndex)
        {
            return IconChoiceBtn.Factory(_iconChoiceBtnPrefab, _parent, sibblingIndex);
        }
    }
    
    public interface IMultipleChoiceAnswerComponent : IViewActions
    {
        public void Configure(IMultipleChoiceTextIconAnswerAsset answerAsset, Action buttonAction);
        public void Configure(IMultipleChoiceTextAnswerAsset answerAsset, Action buttonAction);
        public void SubmitAnswer();
        public void ChooseAnswer(string choice);
        public IViewActions GetComponentUIActions();
    }

    public class MultipleChoiceAnswerComponent : MonoBehaviour, IMultipleChoiceAnswerComponent
    {
        public static IMultipleChoiceAnswerComponent Factory(MultipleChoiceAnswerComponent prefab, Transform parent)
        {
            MultipleChoiceAnswerComponent behaviour = Instantiate(prefab, parent);
            behaviour.Initialize();
            return behaviour;
        }

        public class Dependencies
        {
            public ISpriteHelper SpriteHelper;
            public IChoiceButtonInstantiator ChoiceButtonInstantiator;
            public IStandardButton NextButton;
            public TextMeshProUGUI AnswerTextField;
        }

        private void Initialize()
        {
            SetDependencies(new Dependencies()
            {
                SpriteHelper = new SpriteHelper(),
                NextButton = nextButton,
                AnswerTextField = serializedAnswerText,
                ChoiceButtonInstantiator = new ChoiceButtonInstantiator(textChoiceButtonPrefab, iconChoiceBtnPrefab, this.transform)
            });
        }

        public AnswerState State { get; private set; } = AnswerState.None;
        private ComponentType ComponentType { get; } = ComponentType.MultipleChoiceText;

        [SerializeField] private IconChoiceBtn iconChoiceBtnPrefab;
        [SerializeField] private TextChoiceBtn textChoiceButtonPrefab;
        [SerializeField] private StandardButtonBehaviour nextButton;
        [SerializeField] private TextMeshProUGUI serializedAnswerText;

        private Action _buttonAction;
        private IMultipleChoiceTextAnswerAsset _answerAsset;

        public Dependencies _dependencies { get; private set; }

        public void SetDependencies(Dependencies dependencies)
        {
            ChoiceBtns = new List<IChoiceBtn>();
            _dependencies = dependencies;
        }

        private List<IChoiceBtn> ChoiceBtns;

        public void Configure(IMultipleChoiceTextIconAnswerAsset answerAsset, Action buttonAction) {
            _answerAsset = answerAsset;
            _buttonAction = buttonAction;

            var icons = _dependencies.SpriteHelper.ConvertByteArrayListToSpriteList(answerAsset.Icons);
            
            configureIconButtonsOnScreen(answerAsset, icons);
            
            _dependencies.NextButton.Configure("Svar", SubmitAnswer);
        
            var recordedAnswer = answerAsset.RecordedAnswer;
            if(recordedAnswer != null)
                SetPreviousAnswer(answerAsset.GetRecordedAnswers());      
        }

        public void Configure(IMultipleChoiceTextAnswerAsset answerAsset, Action buttonAction) 
        {
            _answerAsset = answerAsset;
            _buttonAction = buttonAction;
            
            ConfigureTextButtonsOnScreen(answerAsset);
            
            _dependencies.NextButton.Configure("Svar", SubmitAnswer);
        
            var recordedAnswer = answerAsset.RecordedAnswer;
            if(recordedAnswer != null)
                SetPreviousAnswer(answerAsset.GetRecordedAnswers());      
        }

        private void ConfigureTextButtonsOnScreen(IMultipleChoiceTextAnswerAsset answerAsset)
        {
            int i = 0;
            foreach(var choice in ChoiceBtns)
                choice.SetState(MultipleChoiceState.hidden);

            var textChoiceBtnList = ChoiceBtns.FindAll(x => x.GetType() == ChoiceButtonType.Text);

            foreach (var answer in answerAsset.AnswerOptions)
            {
                ITextChoiceBtn button;
                if (i >= textChoiceBtnList.Count)
                {
                    button = _dependencies.ChoiceButtonInstantiator.CreateTextButton(i);
                    ChoiceBtns.Add(button);
                }
                else
                    button = (ITextChoiceBtn)textChoiceBtnList[i];
                button.Configure(answer, answer, ChooseAnswer);
                i++;
            }
        }
        private void configureIconButtonsOnScreen(IMultipleChoiceTextIconAnswerAsset answerAsset, List<Sprite> icons)
        {
            int i = 0;
            foreach(var choice in ChoiceBtns)
                choice.SetState(MultipleChoiceState.hidden);

            if (answerAsset.Icons.Count != answerAsset.AnswerOptions.Count)
                throw new ArgumentException("Data error, not enough icons.");

            var iconChoiceBtnList = ChoiceBtns.FindAll(x => x.GetType() == ChoiceButtonType.Icon);

            foreach (var answer in answerAsset.AnswerOptions)
            {
                IIconChoiceBtn button;
                if (i >= iconChoiceBtnList.Count)
                {
                    button = _dependencies.ChoiceButtonInstantiator.CreateIconButton(i);
                    ChoiceBtns.Add(button);
                }
                else
                    button = (IIconChoiceBtn)iconChoiceBtnList[i];
                button.Configure(icons[i], answer, ChooseAnswer);
                i++;
            }
        }

        public void SubmitAnswer()
        {
            if (_answerAsset.HasAnswer())
                _buttonAction.Invoke();
        }
    
        public void ChooseAnswer(string choice)
        {
            var button = ChoiceBtns.Find(x => x.GetChoiceValue() == choice);
            if (button != null)
            {
                if (button.GetState() == MultipleChoiceState.selected)
                {
                    button.SetState(MultipleChoiceState.active);
                    _answerAsset.RemoveAnswer(choice);
                }
                else
                {
                    button.SetState(MultipleChoiceState.selected);
                    _answerAsset.AddAnswer(choice);
                }
                SetAnswerButtonState(_answerAsset);
                Debug.Log((_answerAsset).RecordedAnswer);
            }
        }
        
        private void SetPreviousAnswer(string[] recordedChoices)
        {
            foreach (var choice in recordedChoices)
            {
                if(!string.IsNullOrEmpty(choice))
                    ChooseAnswer(choice);
            }
        }

        private void SetAnswerButtonState(IAnswerAsset answerData)
        {
            if (answerData.HasAnswer())
            {
                if (answerData.HasCorrectAnswer())
                {
                    State = AnswerState.Correct;
                    _dependencies.AnswerTextField.color = Color.green;
                }
                else
                {
                    State = AnswerState.Incorrect;
                    _dependencies.AnswerTextField.color = Color.red;
                }
            }
            else
            {
                State = AnswerState.None;
                _dependencies.AnswerTextField.color = new Color() { a = 1, b = 0.196f, g = 0.196f, r = 0.196f };
            }
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
            return ComponentType;
        }

        public RectTransform GetRectTransform()
        {
            return (RectTransform) this.transform;
        }

        public IViewActions GetComponentUIActions()
        {
            return this;
        }
    }
}