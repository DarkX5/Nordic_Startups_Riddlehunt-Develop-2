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
    public interface ITextChoiceBtn : IChoiceBtn
    {
        
        public void Configure(string choiceBtnText, string choice, Action<string> buttonAction);
    }
    public class TextChoiceBtn : ChoiceBtn, ITextChoiceBtn
    {
        public static ITextChoiceBtn Factory(TextChoiceBtn prefab, Transform parent, int sibblingIndex)
        {
            TextChoiceBtn behaviour = Instantiate(prefab, parent);
            behaviour.transform.SetSiblingIndex(sibblingIndex);
            behaviour.Initialize();
            return behaviour;
        }

        public class Dependencies : ChoiceBtn.Dependencies
        {
            public TextMeshProUGUI TextField { get; set; }
        }

        private void Initialize()
        {
            SetDependencies(new Dependencies()
            {
                Animator = new ChoiceBtnAnimator(serializedAnimator),
                TextField = TextField
            });
        }
        [SerializeField] private Animator serializedAnimator;
        [SerializeField] private TextMeshProUGUI TextField;
        
        public new Dependencies _dependencies { get; private set; }

        public void SetDependencies(Dependencies dependencies)
        {
            SetDependencies((ChoiceBtn.Dependencies)dependencies);
            _dependencies = dependencies;
        }

        public void Configure(string choiceBtnText, string choice, Action<string> buttonAction)
        {
            Configure(choice, buttonAction);
            _dependencies.TextField.text = choiceBtnText;
        }
        
        public virtual ChoiceButtonType GetType()
        {
            return ChoiceButtonType.Text;
        }
    }
}