using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Helpers;
using Moq;
using riddlehouse_libraries.products.resources;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CharacterSelection.Components
{
    public interface IRoleButtonInstantiater
    {
        public IRoleButton Create();
    }
    public class RoleButtonInstantiater: IRoleButtonInstantiater
    {
        private readonly RoleButton _prefab;
        private readonly RectTransform _parent;
        public RoleButtonInstantiater(RoleButton prefab, RectTransform parent)
        {
            _prefab = prefab;
            _parent = parent;
        }
        public IRoleButton Create()
        {
            return RoleButton.Factory(_prefab, _parent);
        }
    }
    
    public interface IRoleButton
    {
        public void Configure(RoleButton.Config config);
        public void DestroySelf();
    }
    public class RoleButton : MonoBehaviour, IRoleButton
    {
        public static IRoleButton Factory(RoleButton prefab, RectTransform parent)
        {
            RoleButton behaviour = Instantiate(prefab, parent);
            behaviour.Initialize();
            return behaviour;
        }
        public class Dependencies
        {
            public ISpriteHelper SpriteHelper { get; set; }
            public Image ButtonIcon { get; set; }
        }

        public class Config
        {
            public RoleButtonResource Resource { get; set; }
            public Action<string> buttonAction { get; set; }
        }
        
        public void Initialize()
        {
            SetDependencies(new Dependencies()
            {
                SpriteHelper = new SpriteHelper(),
                ButtonIcon = buttonIcon
            });
        }
        
        [SerializeField] private Image buttonIcon;
        public Dependencies _dependencies { get; private set; }
        public void SetDependencies(Dependencies dependencies)
        {
            _dependencies = dependencies;
        }

        private Config _config;
        private Sprite loadedIcon = null;
        public async void Configure(Config config)
        {
            _config = config;
            loadedIcon = _dependencies.SpriteHelper.GetSpriteFromByteArray(await _config.Resource.Icon.GetIcon());
            _dependencies.ButtonIcon.sprite = loadedIcon;
        }

        public void PerformAction()
        {
            _config.buttonAction.Invoke(_config.Resource.Title);
        }

        public void DestroySelf()
        {
            DestroyImmediate(this.gameObject);
        }
    }
}