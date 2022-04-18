using System;
using System.Collections;
using System.Collections.Generic;
using riddlehouse_libraries.products.huntProduct;
using riddlehouse_libraries.products.resources;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CharacterSelection.Components
{
    public interface ICharacterSelectionComponent
    {
        public void Configure(CharacterSelectionComponent.Config config);
        public void RegisterCharacter(HuntCharacterData huntPlayer);
        public void RemoveCharacter(HuntCharacterData huntPlayer);
    }

    public class CharacterSelectionComponent : MonoBehaviour, ICharacterSelectionComponent
    {
        public class Dependencies
        {
            public IRoleSelectionComponent RoleSelectionComponent { get; set; }
            public ISelectedCharactersListComponent SelectedCharactersListComponent { get; set; }
        }

        public class Config
        {
            public CharacterSelectionComponentResource Resource;
            public Action<string> SelectCharacterToAdd { get; set; }
            public Action<string> EditCharacterAction { get; set; }
            public Action<string> RemoveCharacterAction { get; set; }

        }
        
        public void Initialize()
        {
            roleSelectionComponent.Initialize();
            selectedCharactersListComponent.Initialize();
            SetDependencies(new Dependencies()
            {
                RoleSelectionComponent = roleSelectionComponent,
                SelectedCharactersListComponent = selectedCharactersListComponent
            });
        }

        [SerializeField] private VariableLayoutElementVerticalReziser le;
        
        [SerializeField] private RoleSelectionComponent roleSelectionComponent;
        [SerializeField] private SelectedCharactersListComponent selectedCharactersListComponent;
        public Dependencies _dependencies { get; private set; }

        public void SetDependencies(Dependencies dependencies)
        {
            _dependencies = dependencies;
        }

        private Config _config;
        public void Configure(Config config)
        {
            _config = config;
            _dependencies.RoleSelectionComponent.Configure(new RoleSelectionComponent.Config()
            {
                AddRoleAction = SelectCharacterToAdd,
                Resource = _config.Resource.RoleSelectionComponent
            });
            _dependencies.SelectedCharactersListComponent.Configure(new SelectedCharactersListComponent.Config()
            {
                Resource = _config.Resource.RoleSelectionComponent,
                EditCharacterAction = EditCharacter,
                RemoveCharacterAction = RemoveCharacter
            });
        }

        public void OnEnable()
        {
            le.StartUIUpdate();
        }

        public void RegisterCharacter(HuntCharacterData huntPlayer)
        {
            _dependencies.SelectedCharactersListComponent.AddCharacterButtonToList(huntPlayer);
        }

        public void RemoveCharacter(HuntCharacterData huntPlayer)
        {
            _dependencies.SelectedCharactersListComponent.RemoveCharacterButton(huntPlayer);
        }


        private void EditCharacter(string id)
        {
            _config.EditCharacterAction.Invoke(id);
        }

        private void RemoveCharacter(string id)
        {
            _config.RemoveCharacterAction.Invoke(id);

        }

        private void SelectCharacterToAdd(string roleTitle)
        {
            _config.SelectCharacterToAdd.Invoke(roleTitle);
        }
    }
}