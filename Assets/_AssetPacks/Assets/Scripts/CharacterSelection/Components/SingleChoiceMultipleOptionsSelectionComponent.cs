
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prefabs.UI.CharacterSelection.StyledCharacterSelection.Scripts;
using riddlehouse.video;
using riddlehouse_libraries.products.resources;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Color = UnityEngine.Color;

namespace Components.Selection.MultipleChoice
{
    public interface ISingleChoiceMultipleOptionsSelectionComponent
    {
        public void Configure(SingleChoiceMultipleOptionsSelectionComponent.Config config);
        public void DefineOptionChosen(string option);

        public string GetOptionChosen();
    }

    public interface ISelectionButtonInstantiater
    {
        public ISelectionButton Create();
    }

    public class SelectableTagButtonInstantiater : ISelectionButtonInstantiater
    {
        public SelectableTagButton _prefab;
        public RectTransform _parent;

        public SelectableTagButtonInstantiater(SelectableTagButton prefab, RectTransform parent)
        {
            _prefab = prefab;
            _parent = parent;
        }

        public ISelectionButton Create()
        {
            return SelectableTagButton.Factory(_prefab, _parent);
        }
    }

    
    public class SingleChoiceMultipleOptionsSelectionComponent : MonoBehaviour,ISingleChoiceMultipleOptionsSelectionComponent
    {
        public class Dependencies
        {
            public ISelectionButtonInstantiater SelectionButtonInstantiater { get; set; }
        }
        
        public class Config
        {
            public List<string> Options { get; set; }
        }
        public void Initialize()
        {
            SetDependencies(new Dependencies()
            {
                SelectionButtonInstantiater = new SelectableTagButtonInstantiater(selectionButton, contentParent)
            });
        }

        [SerializeField] private SelectableTagButton selectionButton;
        [SerializeField] private RectTransform contentParent;
        public Dependencies _dependencies { get; private set; }
        public void SetDependencies(Dependencies dependencies)
        {
            _dependencies = dependencies;
            buttons = new Dictionary<string, ISelectionButton>();
        }

        private Dictionary<string, ISelectionButton> buttons;
        private string _currentOptionChosen;
        private Config _config = new Config();
        public void Configure(Config config)
        {
            _config = config;
            _currentOptionChosen = null;
            CreateOptionsList(_config.Options);
            HideAllOptions();
            ConfigureOptionsList(_config.Options);
        }

        private void CreateOptionsList(List<string> options)
        {
            if(options.Count > buttons.Count)
                for (int i = buttons.Count; i < options.Count; i++)
                   buttons.Add(options[i], _dependencies.SelectionButtonInstantiater.Create());
        }

        private void HideAllOptions()
        {
            foreach (var key in buttons.Keys)
            {
                buttons[key].Hide();
            }
        }

        private void ConfigureOptionsList(List<string> options)
        {
            for (int i = 0; i < options.Count; i++)
            {
                buttons[options[i]].Configure(new SelectableTagButton.Config
                {
                    Option = options[i],
                    ButtonAction = DefineOptionChosen
                });
                buttons[options[i]].Display();
            }
        }
        
        public void DefineOptionChosen(string option)
        {
            if (buttons.ContainsKey(option))
            {
                _currentOptionChosen = option;
                buttons[option].SelectButton();
                foreach (var key in buttons.Keys)
                {
                    if(key != option)
                        buttons[key].ResetButton();
                }
            }
        }

        public string GetOptionChosen()
        {
            return _currentOptionChosen;
        }
    }
}
