using System;
using System.Collections;
using System.Collections.Generic;
using riddlehouse_libraries.environments.Models;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Riddlehunt.Beta.Environment.Controls
{
    public interface IEnvironmentTargetControls
    {
        public void Configure(EnvironmentTargetControls.Config config);
        public void Display();
        public void Hide();
    }
    public class EnvironmentTargetControls : MonoBehaviour, IEnvironmentTargetControls
    {
        public static IEnvironmentTargetControls Factory(EnvironmentTargetControls prefab, Transform parent)
        {
            return Instantiate(prefab.gameObject, parent).GetComponent<EnvironmentTargetControls>();
        }

        public class Dependencies
        {
            public TextMeshProUGUI Label { get; set; }
        }

        public class Config
        {
            public Target Target { get; set; }
            public Action<Target> ButtonAction { get; set; }
        }
        
       public void Awake()
        {
            SetDependencies(new Dependencies()
            {
                Label = label
            });
        }

        public Dependencies _dependencies { get; private set; }
        [SerializeField] private TextMeshProUGUI label;
        public void SetDependencies(Dependencies dependencies)
        {
            _dependencies = dependencies;
        }

        private Config _config;
        public void Configure(Config config)
        {
            if(_dependencies == null)
                Awake();
            _config = config;
            _dependencies.Label.text = config.Target.Name;
        }
        
        public void ButtonAction()
        {
            _config.ButtonAction.Invoke(_config.Target);
        }

        public void Display()
        {
            this.gameObject.SetActive(true);
        }
        public void Hide()
        {
            this.gameObject.SetActive(false);
        }
    }
}
