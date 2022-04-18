using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prefabs.UI.CharacterSelection.StyledCharacterSelection
{
   public interface ISelectedPlayerButton
   {
      public void Configure(SelectedPlayerButton.Config config);
      public void Remove();
      public void Edit();
      public void DestroySelf();
   }
   public class SelectedPlayerButton : MonoBehaviour, ISelectedPlayerButton
   {
      public static SelectedPlayerButton Factory(SelectedPlayerButton prefab, Transform parent)
      {
         var behaviour = Instantiate(prefab, parent);
         behaviour.Initialize();
         return behaviour;
      }
      public class Dependencies
      {
         public TextMeshProUGUI TextField;
         public Image IconField;
      }

      public void Initialize()
      {
         SetDependencies(
            new Dependencies()
            {
               TextField = textField,
               IconField = iconField
            });   
      }
   
      [SerializeField] private TextMeshProUGUI textField;
      [SerializeField] private Image iconField;

      public Dependencies _dependencies { get; private set; }

      public void SetDependencies(Dependencies dependencies)
      {
         _dependencies = dependencies;
      }

      public class Config
      {
         public Action<string> Edit;
         public Action<string> Remove;
         public string Id;
         public List<string> Tags;
         public string Label;
         public Sprite Icon;
      }

      private Config _config;
      public void Configure(Config config)
      {
         _config = config;
         
         var tagLine = _config.Label + " | ";
         if (_config?.Tags != null)
            foreach (var characterTag in _config.Tags)
            {
               tagLine += $"{characterTag}";
               if (characterTag != _config.Tags[_config.Tags.Count - 1])
                  tagLine += " | ";
            }
         
         _dependencies.TextField.text = tagLine;
         _dependencies.IconField.sprite = _config.Icon;
      }
      public void Remove()
      {
         _config.Remove(_config.Id);
      }

      public void Edit()
      {
         _config.Edit(_config.Id);
      }
      
      public void DestroySelf()
      {
         DestroyImmediate(this.gameObject);
      }
   }
}