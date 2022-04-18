using System;
using System.Collections;
using System.Collections.Generic;
using Riddlehouse.Core.Helpers.Helpers;
using riddlehouse_libraries.products.resources;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Answers
{
    public interface IIconChoiceBtn : IChoiceBtn
    {
        public void Configure(Sprite choiceBtnIcon, string choice, Action<string> buttonAction);
    }
    public class IconChoiceBtn : ChoiceBtn, IIconChoiceBtn
    {
        public static IIconChoiceBtn Factory(IconChoiceBtn prefab, Transform parent, int sibblingIndex)
        {
            IconChoiceBtn behaviour = Instantiate(prefab, parent);
            behaviour.transform.SetSiblingIndex(sibblingIndex);
            behaviour.Initialize();
            return behaviour;
        }

        public class Dependencies : ChoiceBtn.Dependencies
        {
            public Image IconField { get; set; }
        }

        private void Initialize()
        {
            SetDependencies(new Dependencies()
            {
                Animator = new ChoiceBtnAnimator(serializedAnimator),
                IconField = iconField
            });
        }
        [SerializeField] private Animator serializedAnimator;
        [SerializeField] private Image iconField;
        
        public new Dependencies _dependencies { get; private set; }

        public void SetDependencies(Dependencies dependencies)
        {
            SetDependencies((ChoiceBtn.Dependencies)dependencies);
            _dependencies = dependencies;
        }

        public void Configure(Sprite choiceBtnIcon, string choice, Action<string> buttonAction)
        {
            Configure(choice, buttonAction);
            _dependencies.IconField.sprite = choiceBtnIcon;
        }
        public virtual ChoiceButtonType GetType()
        {
            return ChoiceButtonType.Icon;
        }
    }
}