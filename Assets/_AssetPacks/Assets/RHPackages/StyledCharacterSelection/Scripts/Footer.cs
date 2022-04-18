using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Prefabs.UI.CharacterSelection.StyledCharacterSelection.Scripts
{
    public interface IFooter
    {
        public void Configure(Footer.Config config);
    }
    public class Footer : MonoBehaviour, IFooter
    {
        public void Initialize()
        {
            characterIconDisplay.Initialize();
            SetDependencies(new Dependencies()
            {
                CharacterIconDisplay = characterIconDisplay
            });
        }
        public class Dependencies
        {
            public ICharacterIconDisplay CharacterIconDisplay;
        }

        [SerializeField] private CharacterIconDisplay characterIconDisplay;

        public Dependencies _dependencies { get; private set; }
        public void SetDependencies(Dependencies dependencies)
        {
            _dependencies = dependencies;
        }
    
        public class Config
        {
            public Sprite Icon;
            public Color IconFrame;
            public Action Approve;
            public Action Abort;
        }

        private Config _config;
        public void Configure(Config config)
        {
            _config = config;
            _dependencies.CharacterIconDisplay.Configure(_config.IconFrame, _config.Icon);
        }
        public void Approve()
        {
            _config.Approve();
        }

        public void Abort()
        {
            _config.Abort();
        }
    }
}