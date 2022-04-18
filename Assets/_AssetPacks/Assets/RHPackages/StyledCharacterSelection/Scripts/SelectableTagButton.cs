using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prefabs.UI.CharacterSelection.StyledCharacterSelection.Scripts
{
    public interface ISelectionButton
    {
        public void Configure(SelectableTagButton.Config config);
        public void Hide();
        public void Display();
        public void SelectButton();
        public void ResetButton();
    }
    public class SelectableTagButton : MonoBehaviour, ISelectionButton
    {
        public static ISelectionButton Factory(SelectableTagButton prefab, Transform parent)
        {
            SelectableTagButton behaviour = Instantiate(prefab, parent);
            behaviour.Initialize();
            return behaviour;
        }
        public class Dependencies
        {
            public Image ButtonImg;
            public TextMeshProUGUI ButtonText;
            public Sprite UnselectedImage;
            public Sprite SelectedImage;
        }
        public class Config
        {
            public string Option { get; set; }
            public Action<string> ButtonAction { get; set; }
        }
        public void Initialize()
        {
            SetDependencies(new Dependencies()
            {
                ButtonImg = buttonImage,
                ButtonText = buttonText,
                UnselectedImage = unselectedImage,
                SelectedImage = selectedImage
            });            
        }

        [SerializeField] private Image buttonImage;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private Sprite unselectedImage;
        [SerializeField] private Sprite selectedImage;
        public Dependencies _dependencies { get; private set; }
        public void SetDependencies(Dependencies dependencies)
        {
            _dependencies = dependencies;
        }

        private Config _config;
        public void Configure(Config config)
        {
            _config = config;
            _dependencies.ButtonText.text = _config.Option;
            ResetButton();
        }
    
        public void Display()
        {
            this.gameObject.SetActive(true);
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }

        public void PerformAction()
        {
            _config.ButtonAction(_config.Option);
        }
        public void SelectButton()
        {
            _dependencies.ButtonImg.sprite = _dependencies.SelectedImage;
            this.transform.localScale = Vector3.one;
        }

        public void ResetButton()
        {
            _dependencies.ButtonImg.sprite =_dependencies.UnselectedImage;
            this.transform.localScale = Vector3.one* 0.8f;
        }
    }
}