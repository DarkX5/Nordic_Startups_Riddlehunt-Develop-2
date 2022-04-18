using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSelection.Components;
using Components.Buttons;
using Riddlehouse.Core.Helpers.Helpers;
using riddlehouse_libraries.products.huntProduct;
using riddlehouse_libraries.products.resources;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using CharacterSelectionComponent = CharacterSelection.Components.CharacterSelectionComponent;
using RoleButton = CharacterSelection.Components.RoleButton;

namespace CharacterSelection.View
{
    public interface ICharacterSelectionController
    {
        public void Configure(CharacterSelectionView.Config config);
        public CharacterSelectionStyles GetStyleType();
        public void DestroySelf();
        public void Display();
        public void Hide();
        public void RegisterCharacter(HuntCharacterData huntPlayer);
        public void RemoveCharacter(HuntCharacterData huntPlayer);
    }
    
    [RequireComponent(typeof(BasicComponentDisplayController))]
    public class CharacterSelectionView : MonoBehaviour, ICharacterSelectionController
    {
        public static ICharacterSelectionController Factory(CharacterSelectionView prefab, RectTransform parent)
        {
            CharacterSelectionView behaviour = Instantiate(prefab);
            UIFitters uiFitters = new UIFitters();
            uiFitters.FitToFullscreen((RectTransform)behaviour.transform, parent);
            behaviour.Initialize();
            behaviour.Hide();
            return behaviour;
        }
        
        public class Dependencies
        {
            public TextMeshProUGUI Title { get; set; }
            public IComponentDisplayController DisplayController { get; set; }
            public ICharacterSelectionComponent CharacterSelectionComponent { get; set; }
        }
        
        public class Config
        {
            public CharacterSelectionViewResource Resource { get; set; }
            public Action StartGameAction { get; set; }
            public Action<string> AddPlayerAndMapCharacter { get; set; }
            public Action<string> EditCharacterAction { get; set; }
            public Action<string> RemoveCharacterAction { get; set; }
            public Action BackAction { get; set; }
        }

        public void Initialize()
        {
            characterSelectionComponent.Initialize();
            SetDependencies(new Dependencies()
            {
                Title = title,
                DisplayController = GetComponent<BasicComponentDisplayController>(),
                CharacterSelectionComponent = characterSelectionComponent
            });
        }
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private CharacterSelectionComponent characterSelectionComponent;
        public Dependencies _dependencies { get; set; }
        public void SetDependencies(Dependencies dependencies)
        {
            _dependencies = dependencies;
        }

        private Config _config;
        public void Configure(Config config)
        {
            _config = config;
            _dependencies.Title.text = _config.Resource.TitleText;

            _dependencies.CharacterSelectionComponent.Configure(
              new CharacterSelectionComponent.Config()
              {
                  Resource = _config.Resource.CharacterSelectionComponent,
                  SelectCharacterToAdd = AddPlayerAndMapCharacter,
                  EditCharacterAction = EditCharacterAction,
                  RemoveCharacterAction = RemoveCharacterAction
              });
        }

        public void StartGameAction()
        {
            _config.StartGameAction();
        }
        
        private CharacterSelectionViewResource _libraryConfig;
        public void Configure(CharacterSelectionViewResource config)
        {
            _libraryConfig = config;
        }
        private void AddPlayerAndMapCharacter(string roleTitle)
        {
            _config.AddPlayerAndMapCharacter.Invoke(roleTitle);
        }

        public void RegisterCharacter(HuntCharacterData huntPlayer)
        {
            _dependencies.CharacterSelectionComponent.RegisterCharacter(huntPlayer);
        }

        public void RemoveCharacter(HuntCharacterData huntPlayer) //needs better naming - this is a function called by the flow - should ONLY call the component.
        {
            _dependencies.CharacterSelectionComponent.RemoveCharacter(huntPlayer);
        }

        private void EditCharacterAction(string id)
        {
            _config.EditCharacterAction.Invoke(id);
        }

        private void RemoveCharacterAction(string id) //needs better naming - this is the callback to the flow informing that a button has been deleted.
        {
            _config.RemoveCharacterAction.Invoke(id);
        }

        public void Display()
        {
            _dependencies.DisplayController.Display();
        }

        public void Hide()
        {
            _dependencies.DisplayController.Hide();
        }
        public CharacterSelectionStyles GetStyleType()
        {
            return CharacterSelectionStyles.Standard;
        }

        public void DestroySelf()
        {
            Destroy(this.gameObject);
        }

        public void BackBtnAction()
        {
            _config.BackAction();
        }
    }
}