using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSelection;
using Riddlehouse.Core.Helpers.Helpers;
using Helpers;
using Prefabs.UI.CharacterSelection.StyledCharacterSelection.Scripts;
using riddlehouse_libraries.products.resources;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public interface IPlayerInformationController
{
    public void Configure(PlayerInformationView.Config config);
    public PlayerInformationInputStyles GetStyleType();
    public void DestroySelf();
    public void Display();
    public void Hide();
}
[RequireComponent(typeof(BasicComponentDisplayController))]
public class PlayerInformationView : MonoBehaviour, IPlayerInformationController
{    
    public static IPlayerInformationController Factory(PlayerInformationView prefab, RectTransform parent)
    {
        PlayerInformationView behaviour = Instantiate(prefab);
        behaviour.Initialize();
        behaviour._dependencies.DisplayController.FitToScreen(parent);
        behaviour.Hide();
        return behaviour;
    }
    public class Dependencies
    {
        public ISpriteHelper SpriteHelper { get; set; }
        public IFooter Footer { get; set; }
        public IComponentDisplayController DisplayController { get; set; }
        public TMP_InputField NameTextInput { get; set; }
        public TMP_InputField AgeTextInput { get; set; }
        public TextMeshProUGUI CharacterTitleField { get; set; }
        public TextMeshProUGUI CharacterDescriptionField { get; set; }
        public Image CharacterAvatar { get; set; }
    }

    public class Config
    {
        public CharacterOption characterOption { get; set; }
        public Output PreviousOutput { get; set; }
        public Action<Output> Approve { get; set; }
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

    public class Output
    {
        public string Age { get; set; }
        public string Name { get; set; }
    }

    public void Initialize()
    {
        var displayController = GetComponent<BasicComponentDisplayController>();
        displayController.Initialize();
        footer.Initialize();
        var dependencies = new Dependencies()
        {
            Footer = footer,
            DisplayController = displayController,
            AgeTextInput = ageTextInput,
            NameTextInput = nameTextInput,
            CharacterTitleField = characterTitleField,
            CharacterDescriptionField = characterDescriptionField,
            CharacterAvatar = characterAvatar,
            SpriteHelper = new SpriteHelper()
        };
        SetDependencies(dependencies);
    }

    [FormerlySerializedAs("characterFooter")] [SerializeField] private Footer footer;
    [SerializeField] private TMP_InputField ageTextInput;
    [SerializeField] private TMP_InputField nameTextInput;
    [SerializeField] private TextMeshProUGUI characterTitleField;
    [SerializeField] private TextMeshProUGUI characterDescriptionField;
    [SerializeField] private Image characterAvatar;
    public Dependencies _dependencies { get; set; }
    public void SetDependencies(Dependencies dependencies)
    {
        _dependencies = dependencies;
    }

    private Config _config;
    private Output _output;
    public async void Configure(Config config)
    {
        _config = config;
        _output = config.PreviousOutput ?? new Output();
        _dependencies.NameTextInput.text = _output.Name;
        _dependencies.AgeTextInput.text = _output.Age;

        var icon = _dependencies.SpriteHelper.GetSpriteFromByteArray(await _config.characterOption.CharacterIcon.GetIcon(null));
        var avatarImage = _dependencies.SpriteHelper.GetSpriteFromByteArray(await _config.characterOption.CharacterImage.GetIcon(null));

        _dependencies.Footer.Configure(new Footer.Config()
        {
            Icon = icon,
            IconFrame = _config.GetIconFrameColor(),
            Abort = Abort,
            Approve = Approve,
        });
        _dependencies.CharacterTitleField.text = _config.characterOption.CharacterName;
        _dependencies.CharacterDescriptionField.text = _config.characterOption.CharacterDescription;
        _dependencies.CharacterAvatar.sprite = avatarImage;
        NameTextUpdated();
        AgeTextUpdated();
    }

    public void Approve()
    {
        _config.Approve.Invoke(_output);
    }

    public void Abort()
    {
        _config.Abort.Invoke();
    }

    public void NameTextUpdated()
    {
        _output.Name = _dependencies.NameTextInput.text;
    }

    public void AgeTextUpdated()
    {
        _output.Age = _dependencies.AgeTextInput.text;
    }
    
    public void Display()
    {
        _dependencies.DisplayController.Display();
    }

    public void Hide()
    {
        _dependencies.DisplayController.Hide();
    }
    
    public PlayerInformationInputStyles GetStyleType()
    {
        return PlayerInformationInputStyles.Standard;
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
