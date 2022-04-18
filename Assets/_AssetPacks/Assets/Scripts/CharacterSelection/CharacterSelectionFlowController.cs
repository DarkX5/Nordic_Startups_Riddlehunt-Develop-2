using System;
using System.Collections.Generic;
using CharacterSelection.View;
using Components.Buttons;
using Components.Selection.MultipleChoice;
using Riddlehouse.Core.Helpers.Helpers;
using Helpers;
using riddlehouse_libraries.products.huntProduct;
using riddlehouse_libraries.products.resources;
using UnityEditor;
using UnityEngine;
using CharacterSelectionView = CharacterSelection.View.CharacterSelectionView;

namespace CharacterSelection
{
    public interface ICharacterSelectionFlowController
    {
        public void Configure(CharacterSelectionFlowController.Config config);
    }

    public enum CharacterSelectionStyles
    {
        Standard
    }

    public enum TagSelectionStyles
    {
        Standard
    }

    public enum PlayerInformationInputStyles
    {
        Standard
    }

    public class CharacterSelectionFlowController : MonoBehaviour, ICharacterSelectionFlowController
    {
        public static ICharacterSelectionFlowController Factory(CharacterSelectionFlowController prefab,
            RectTransform parent)
        {
            CharacterSelectionFlowController behaviour = Instantiate(prefab);
            behaviour.Initialize();
            UIFitters uiFitters = new UIFitters();
            uiFitters.FitToFullscreen((RectTransform)behaviour.transform, parent);
            return behaviour;
        }

        public class Dependencies
        {
            public ISpriteHelper SpriteHelper { get; set; }
            public ICharacterSelectionStyler Styler { get; set; }
        }

        public class Config
        {
            public CharacterSelectionFlowResource Resource { get; set; }
            public Action<List<HuntCharacterData>> FlowComplete { get; set; }
            public Action FlowAbandoned { get; set; }
        }

        [SerializeField] private CharacterSelectionStyler styler;
        private ICharacterSelectionController _characterSelectionController;
        private ITagSelectionController _tagSelectionController;
        private IPlayerInformationController _playerInformationController;

        public void Initialize()
        {
            SetDependencies(new Dependencies()
            {
                Styler = styler
            });
        }

        public Dependencies _dependencies { get; set; }

        public void SetDependencies(Dependencies dependencies)
        {
            _dependencies = dependencies;
        }

        private Config _config;

        public void Configure(Config config)
        {
            _config = config;
            var characterSelectionStyle = CharacterSelectionStyles.Standard;
            var tagSelectionStyles = TagSelectionStyles.Standard;
            var playerInformationStyle = PlayerInformationInputStyles.Standard;

            CleanUpStyles(characterSelectionStyle, tagSelectionStyles, playerInformationStyle);

            var parent = (RectTransform)transform;
            _characterSelectionController ??= _dependencies.Styler.CreateSelectionHome(characterSelectionStyle, parent);
            _tagSelectionController ??= _dependencies.Styler.CreateTagView(tagSelectionStyles, parent);
            _playerInformationController ??=
                _dependencies.Styler.CreatePlayerInformationView(playerInformationStyle, parent);

            _characterSelectionController.Configure(
                new CharacterSelectionView.Config()
                {
                    Resource = config.Resource.CharacterSelectionView,
                    StartGameAction = EndFlow,
                    AddPlayerAndMapCharacter = StartAddCharacter,
                    EditCharacterAction = EditCharacter,
                    RemoveCharacterAction = RemoveExistingCharacter,
                    BackAction = AbortCharacterSelection
                });
            _characterSelectionController.Display();

            RegisteredCharacters = new List<HuntCharacterData>();
        }

        private void EndFlow()
        {
            _characterSelectionController?.DestroySelf();
            _tagSelectionController?.DestroySelf();
            _playerInformationController?.DestroySelf();
            _config.FlowComplete.Invoke(RegisteredCharacters);
        }

        private void CleanUpStyles(CharacterSelectionStyles selectionStyle, TagSelectionStyles tagStyle,
            PlayerInformationInputStyles playerInformationInputStyle)
        {
            if (_characterSelectionController?.GetStyleType() != selectionStyle)
                _characterSelectionController?.DestroySelf();
            if (_tagSelectionController?.GetStyleType() != tagStyle) _tagSelectionController?.DestroySelf();
            if (_playerInformationController?.GetStyleType() != playerInformationInputStyle)
                _playerInformationController?.DestroySelf();
        }

        public List<HuntCharacterData> RegisteredCharacters { get; set; } //Todo: this data needs to be in a service.

        //could be private but in order to more easily test, we leave them public, but don't include them in the interface.
        public void EditCharacter(string id)
        {
            var idx = RegisteredCharacters.FindIndex(x => x.Id == id);
            if (idx != -1)
            {
                _currentlyActiveHuntPlayer = RegisteredCharacters[idx];
                DisplayCharacterInformation();
            }
        }

        private void CommitChangesToExistingCharacter(string id)
        {
            var idx = RegisteredCharacters.FindIndex(x => x.Id == id);
            _characterSelectionController.RemoveCharacter(RegisteredCharacters[idx]);
            AddCharacter(_currentlyActiveHuntPlayer);
        }

        private void RemoveExistingCharacter(string id)
        {
            var idx = RegisteredCharacters.FindIndex(x => x.Id == id);
            if (idx != -1)
                RegisteredCharacters.RemoveAt(idx);
        }

        private HuntCharacterData _currentlyActiveHuntPlayer;

        private TagOptionResource _lastTag = null;

        //could be private but in order to more easily test, we leave them public, but don't include them in the interface.
        public void StartAddCharacter(string role)
        {
            _currentlyActiveHuntPlayer = new HuntCharacterData();
            _currentlyActiveHuntPlayer.Id = System.Guid.NewGuid().ToString();
            _currentlyActiveHuntPlayer.AddOrReplaceTag(_config.Resource.TagOptions[0].Id,
                role);
            _lastTag = _config.Resource.TagOptions[0];
            DisplayCharacterInformation();
        }

        private void DisplayCharacterInformation()
        {
            var roleString = _currentlyActiveHuntPlayer.GetTagFromCategory(_config.Resource.TagOptions[0].Id);
            var role = _config.Resource.CharacterOptions.Find(x => x.Id == roleString);
            
            _playerInformationController.Configure(new PlayerInformationView.Config()
            {
                characterOption = role,
                PreviousOutput = new PlayerInformationView.Output()
                {
                    Name  = _currentlyActiveHuntPlayer.PlayerName,
                    Age = _currentlyActiveHuntPlayer.PlayerAge
                },
                Abort = AbortAddCharacter,
                Approve = (output) =>
                {
                    _currentlyActiveHuntPlayer.PlayerAge = output.Age;
                    _currentlyActiveHuntPlayer.PlayerName = output.Name;
                    if (_config.Resource.TagOptions.Count > 1)
                    {
                        DisplayTagSelection(_config.Resource.TagOptions[1]);
                    }
                }
            });
            _playerInformationController.Display();
            _characterSelectionController.Hide();
        }

        private void DisplayTagSelection(TagOptionResource tagOption)
        {
            var roleString = _currentlyActiveHuntPlayer.GetTagFromCategory(_config.Resource.TagOptions[0].Id);
            var role = _config.Resource.CharacterOptions.Find(x => x.Id == roleString);
            _lastTag = tagOption;
            _tagSelectionController.Configure(new TagSelectionView.Config()
            {
                Resource = _config.Resource.TagSelectionViewResource,
                characterOption = role,
                Option = tagOption,
                PreviousSelection = _currentlyActiveHuntPlayer.GetTagFromCategory(tagOption.Id),
                Abort = AbortTagSelectionGoBackToCharacterInformation,
                CommitTag = CommitTag
            });
            _tagSelectionController.Display();
            _playerInformationController.Hide();
        }

        public void CommitTag(TagSelectionView.TagChoice tagChosen)
        {
            _tagSelectionController.Hide();
            _currentlyActiveHuntPlayer.AddOrReplaceTag(tagChosen.TagId, tagChosen.TagValue);
            var nextTagIndex = _config.Resource.TagOptions.FindIndex(x => x.Id == _lastTag.Id) + 1;
            if (nextTagIndex >= _config.Resource.TagOptions.Count - 1)
            {
                CompleteAddCharacter();
            }
            else
            {
                DisplayTagSelection(_config.Resource.TagOptions[nextTagIndex]);
                _tagSelectionController.Display();
            }
        }

        public void CompleteAddCharacter()
        {
            var idx = RegisteredCharacters.FindIndex(x => x.Id == _currentlyActiveHuntPlayer.Id);
            if (idx == -1)
            {
                AddCharacter(_currentlyActiveHuntPlayer);
            }
            else
            {
                CommitChangesToExistingCharacter(_currentlyActiveHuntPlayer.Id);
            }

            AbortAddCharacter();
        }

        public void AbortAddCharacter()
        {
            _lastTag = null;
            _currentlyActiveHuntPlayer = null;
            _tagSelectionController.Hide();
            _playerInformationController.Hide();
            _characterSelectionController.Display();
        }

        public void AbortTagSelectionGoBackToCharacterInformation()
        {
            _tagSelectionController.Hide();
            _playerInformationController.Display();
        }

        private void AddCharacter(HuntCharacterData data)
        {
            _characterSelectionController.RegisterCharacter(data);
            RegisteredCharacters.Add(data);
        }

        public void AbortCharacterSelection()
        {
            if (_config.FlowAbandoned != null)
            {
                _characterSelectionController?.DestroySelf();
                _tagSelectionController?.DestroySelf();
                _playerInformationController?.DestroySelf();
                _config.FlowAbandoned();
            }
        }
    }
}