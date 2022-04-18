using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSelection;
using CharacterSelection.View;
using Components.Selection.MultipleChoice;
using Riddlehouse.Core.Helpers.Helpers;
using Helpers;
using Prefabs.UI.CharacterSelection.StyledCharacterSelection.Scripts;
using riddlehouse_libraries.products.resources;
using TMPro;
using UnityEngine;
using Color = UnityEngine.Color;

namespace CharacterSelection.View
{
    public interface ITagSelectionController
    {
        public void Configure(TagSelectionView.Config config);
        public void Display();
        public void Hide();
        public TagSelectionStyles GetStyleType();
        public void DestroySelf();
    }

    [RequireComponent(typeof(BasicComponentDisplayController))]
    public class TagSelectionView : MonoBehaviour, ITagSelectionController
    {
        public static ITagSelectionController Factory(TagSelectionView prefab, RectTransform parent)
        {
            TagSelectionView behaviour = Instantiate(prefab);
            behaviour.Initialize();
            behaviour._dependencies.DisplayController.FitToScreen(parent);
            behaviour.Hide();
            return behaviour;
        }
        
        public class Config
        {
            public TagSelectionViewResource Resource { get; set; }
            public CharacterOption characterOption { get; set; }
            public TagOptionResource Option { get; set; }
            public string PreviousSelection { get; set; }
            public Action<TagChoice> CommitTag { get; set; }
            public Action Abort { get; set; }
            public Color GetIconFrameColor()
            {
                return new Color32(
                    characterOption.RoleColor.R, 
                    characterOption.RoleColor.G, 
                    characterOption.RoleColor.B,
                    characterOption.RoleColor.A
                );
            }
        }
        public class Dependencies
        {
            public TextMeshProUGUI TitleField { get; set; }
            public TextMeshProUGUI DescriptionField { get; set; }
            public ISingleChoiceMultipleOptionsSelectionComponent TagChoiceListComponent { get; set; }
            public IComponentDisplayController DisplayController { get; set; }
            public ISpriteHelper SpriteHelper { get; set; }
            public IFooter Footer { get; set; }
        }

        public class TagChoice
        {
            public string TagId { get; set; }
            public string TagValue { get; set; }
        }

        public void Initialize()
        {
            var displayController = GetComponent<BasicComponentDisplayController>();
            displayController.Initialize();
            tagChoiceListComponent.Initialize();
            footer.Initialize();
            SetDependencies(new Dependencies()
            {
                TitleField = titleField,
                DescriptionField = descriptionField,
                DisplayController = displayController,
                TagChoiceListComponent = tagChoiceListComponent,
                SpriteHelper = new SpriteHelper(),
                Footer = footer
            });
        }

        [SerializeField] private TextMeshProUGUI titleField;
        [SerializeField] private TextMeshProUGUI descriptionField;
        [SerializeField] private Footer footer;
        [SerializeField] private SingleChoiceMultipleOptionsSelectionComponent tagChoiceListComponent;
        public Dependencies _dependencies { get; private set; }
        public void SetDependencies(Dependencies dependencies)
        {
            _dependencies = dependencies;
        }

        private Config _config;
        public async void Configure(Config config)
        {
            _config = config;
            _dependencies.TitleField.text = config.Option.Title;
            _dependencies.DescriptionField.text = config.Option.Description;
            var icon = _dependencies.SpriteHelper.GetSpriteFromByteArray(await _config.characterOption.CharacterIcon.GetIcon(null));

            _dependencies.Footer.Configure(new Footer.Config()
            {
                Icon = icon,
                IconFrame = _config.GetIconFrameColor(),
                Approve = CommitTag,
                Abort = Abort
            });
            
            _dependencies.TagChoiceListComponent.Configure(new SingleChoiceMultipleOptionsSelectionComponent.Config()
            {
                Options = _config.Option.Options
            });
            if(_config.PreviousSelection != null)
                _dependencies.TagChoiceListComponent.DefineOptionChosen(_config.PreviousSelection);
        }

        public void CommitTag()
        {
            _config.CommitTag.Invoke(new TagChoice()
            {
                TagId = _config.Option.Id,
                TagValue = _dependencies.TagChoiceListComponent.GetOptionChosen()
            });
        }

        public void Abort()
        {
            _config.Abort();
        }
        
        public void Display()
        {
            _dependencies.DisplayController.Display();
        }

        public void Hide()
        {
            _dependencies.DisplayController.Hide();
        }
        
        public TagSelectionStyles GetStyleType()
        {
            return TagSelectionStyles.Standard;
        }

        public void DestroySelf()
        {
            DestroyImmediate(this.gameObject);
        }
    }
}