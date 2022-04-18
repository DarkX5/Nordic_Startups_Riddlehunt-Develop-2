using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using CharacterSelection;
using Helpers;
using Prefabs.UI.CharacterSelection.StyledCharacterSelection;
using riddlehouse_libraries.products.huntProduct;
using riddlehouse_libraries.products.resources;
using UnityEngine;

public interface ISelectedCharactersListComponent
{
    public void Configure(SelectedCharactersListComponent.Config config);
    public void AddCharacterButtonToList(HuntCharacterData huntPlayer);
    public void RemoveCharacterButton(HuntCharacterData huntPlayer);
}

public interface ISelectedPlayerButtonInstantiater
{
    public ISelectedPlayerButton Create();
}

public class SelectedPlayerButtonInstantiater: ISelectedPlayerButtonInstantiater
{
    private readonly SelectedPlayerButton _prefab;
    private readonly RectTransform _parent;

    public SelectedPlayerButtonInstantiater(SelectedPlayerButton prefab, RectTransform parent)
    {
        _prefab = prefab;
        _parent = parent;
    }
    public ISelectedPlayerButton Create()
    {
       return SelectedPlayerButton.Factory(_prefab, _parent);
    }
}


public class SelectedCharactersListComponent : MonoBehaviour, ISelectedCharactersListComponent
{
    public class Dependencies
    {
        public ISelectedPlayerButtonInstantiater SelectedPlayerButtonInstantiater { get; set; }
        public ISpriteHelper SpriteHelper { get; set; }
    }

    public class Config
    {
        public RoleSelectionComponentResource Resource { get; set; }
        public Action<string> EditCharacterAction { get; set; }
        public Action<string> RemoveCharacterAction { get; set; }
    }
    [SerializeField] private SelectedPlayerButton buttonPrefab;
    public void Initialize()
    {
        SetDependencies(new Dependencies()
        {            
            SelectedPlayerButtonInstantiater = new SelectedPlayerButtonInstantiater(buttonPrefab, (RectTransform)transform),
            SpriteHelper = new SpriteHelper()
        });
    }

    public Dependencies _dependencies { get; private set; }

    public void SetDependencies(Dependencies dependencies)
    {
        _dependencies = dependencies;
    }
    private Dictionary<string,ISelectedPlayerButton> _selectedCharacterButtons;
    Config _config;
    public void Configure(Config config)
    {
        _config = config;
        _selectedCharacterButtons ??= new Dictionary<string, ISelectedPlayerButton>();
        foreach (var key in _selectedCharacterButtons.Keys)
        {
            _selectedCharacterButtons[key].DestroySelf();
        }
        _selectedCharacterButtons.Clear();
    }

    public async void AddCharacterButtonToList(HuntCharacterData huntPlayer)
    {
        var role = huntPlayer.Tags[_config.Resource.InitialTag];
        var rolebutton = _config.Resource.RoleButtons.Find(x => x.Title == role);
        var icon = _dependencies.SpriteHelper.GetSpriteFromByteArray(await rolebutton.Icon.GetIcon());
        
        var characterButton = _dependencies.SelectedPlayerButtonInstantiater.Create();
        characterButton.Configure(new SelectedPlayerButton.Config()
        {
            Id = huntPlayer.Id,
            Icon = icon,
            Label = huntPlayer.PlayerName,
            Tags = huntPlayer.Tags.Values.ToList(),
            Edit = EditCharacter,
            Remove = RemoveCharacter
        });
        _selectedCharacterButtons.Add(huntPlayer.Id, characterButton);
    }

    public void EditCharacter(string id)
    {
        _config.EditCharacterAction.Invoke(id);
    }

    public void RemoveCharacterButton(HuntCharacterData huntPlayer)
    {
        RemoveCharacter(huntPlayer.Id);
    }
    public void RemoveCharacter(string id)
    {
        _selectedCharacterButtons[id].DestroySelf();
        _selectedCharacterButtons.Remove(id);
        _config.RemoveCharacterAction.Invoke(id);
    }
}