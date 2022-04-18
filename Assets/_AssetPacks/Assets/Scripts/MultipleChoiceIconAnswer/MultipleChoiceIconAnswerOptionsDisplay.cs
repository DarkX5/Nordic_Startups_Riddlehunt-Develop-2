using System;
using System.Collections;
using System.Collections.Generic;
using Answers.MultipleChoice.Buttons.Icon;
using Answers.MultipleChoice.Data.Icon;
using UI.Answers;
using UnityEngine;
using UnityEngine.UI;

namespace Answers.MultipleChoice.Components
{
    public interface IMultipleChoiceIconAnswerOptionsDisplay
    {
        public void Configure(MultipleChoiceIconAnswerOptionsDisplay.Config config);
    }

    public interface IIconButtonInstantiater
    {
        public IOldIconChoiceBtn CreateButton();
    }
    public class IconButtonInstantiater: IIconButtonInstantiater
    {
        private MultipleChoiceOldIconButton _prefab;
        private RectTransform _content;
        public IconButtonInstantiater(MultipleChoiceOldIconButton prefab, RectTransform content)
        {
            _prefab = prefab;
            _content = content;
        }

        public IOldIconChoiceBtn CreateButton()
        {
            return MultipleChoiceOldIconButton.Factory(_prefab, _content);
        }
    }

    public class MultipleChoiceIconAnswerOptionsDisplay : MonoBehaviour, IMultipleChoiceIconAnswerOptionsDisplay
    {

        public class Dependencies
        {
            public IIconButtonInstantiater ButtonInstantiater { get; set; }
            public RectTransform ContentParent { get; set; }
            public Button CommitAnswerButton { get; set; }
            public AudioSource ObjectAudioSource { get; set; }
        }
        
        public class Config
        {
            public Action RiddleComplete { get; set; }
            public IconMultipleChoiceAnswerData AnswerData { get; set; }
        }
        [SerializeField] private MultipleChoiceOldIconButton buttonPrefab;
        [SerializeField] private RectTransform content;
        [SerializeField] private Button SendAnswer;
        [SerializeField] private AudioSource audioSource;

        private List<IOldIconChoiceBtn> choices;
        public Dependencies _dependencies { get; set; }


        public void Awake()
        {
            SetDependencies(new Dependencies()
            {
                ButtonInstantiater = new IconButtonInstantiater(buttonPrefab, content),
                ContentParent = content,
                CommitAnswerButton = SendAnswer,
                ObjectAudioSource = audioSource
            });
        }

        public void SetDependencies(Dependencies dependencies)
        {
            collectiveAnswer = "";
            choices = new List<IOldIconChoiceBtn>();
            _dependencies = dependencies;
        }

        private Config _config;
        public void Configure(Config config)
        {
            _config = config;
            collectiveAnswer = config.AnswerData.RecordedAnswer;
            UpdateButtonList(config.AnswerData);
        }

        
        private void UpdateButtonList(IconMultipleChoiceAnswerData answerData)
        {
            _hasCompleted = false;
            foreach(var choice in choices)
                choice.Hide();
            ClearAnswer();
            _dependencies.CommitAnswerButton.interactable = true; //todo: test

            if (choices.Count < answerData.PossibleAnswers.Count)
            {
                var listSize = choices.Count;
                for (int i = listSize; i < answerData.PossibleAnswers.Count; i++)
                {
                    choices.Add(_dependencies.ButtonInstantiater.CreateButton());
                }
            }

            var idx = 0;
            foreach (var answer in answerData.PossibleAnswers)
            {
                choices[idx].Configure(new MultipleChoiceOldIconButton.Config()
                {
                    SelectionToggleAction = OptionChosen, 
                    iconWithValue = answer,
                    State = MultipleChoiceState.active
                });
                choices[idx].ResetState(true);
                idx++;
            }
        }

        public string collectiveAnswer { get; private set; }
        private List<IconWithValue> CurrentlySelected;
        private void OptionChosen(IconWithValue symbolChosen)
        {
            _dependencies.ObjectAudioSource.Play();
            if (CurrentlySelected.Contains(symbolChosen))
            {
                CurrentlySelected.Remove(symbolChosen);
                OptionUnchosen(symbolChosen.Value);
            }
            else
            {
                CurrentlySelected.Add(symbolChosen);
                OptionChosen(symbolChosen.Value);
            }
        }

        private void OptionUnchosen(string option)
        {
            collectiveAnswer = collectiveAnswer.Replace(option+";", "");
            // _config.AnswerData.SetAnswer(collectiveAnswer);
        }
        private void OptionChosen(string option)
        {
            collectiveAnswer += option+";";
            // _config.AnswerData.SetAnswer(collectiveAnswer);
        }

        public void ClearAnswer()
        {
            collectiveAnswer = "";
            _config.AnswerData.SetAnswer(collectiveAnswer);
            CurrentlySelected = new List<IconWithValue>();
        }

        private bool _hasCompleted = false;
        private readonly object _myLock = new object();
        public void CheckAnswer()
        {
            _dependencies.ObjectAudioSource.Play();
            _config.AnswerData.SetAnswer(collectiveAnswer);
            if (_config.AnswerData.HasCorrectAnswer())
            {
                _dependencies.CommitAnswerButton.interactable = false; //todo: test
                foreach(var choice in choices)
                    choice.TriggerAnimationCorrect(() =>
                    {
                        lock (_myLock)
                        {
                            if (!_hasCompleted)
                            {
                                _hasCompleted = true;
                                _config.RiddleComplete.Invoke();
                            }
                            choice.LockInteraction();
                        }
                    });
            }
            else
            {
                ClearAnswer();
                _dependencies.CommitAnswerButton.interactable = false; //todo: test
                foreach(var choice in choices)
                    choice.TriggerAnimationWrong(() =>
                    {
                        lock (_myLock)
                        {
                            _dependencies.CommitAnswerButton.interactable = true; //todo: test
                        }
                    });
            }
        }
    }
}