using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using riddlehouse_libraries.environments.Models;
using UnityEngine;
using Zenject;

namespace Riddlehunt.Beta.Environment.Controls
{
    public interface ICreateEnvironmentTargetEntry
    {
        public IEnvironmentTargetControls Create();
    }
    public class CreateEnvironmentTargetEntry : ICreateEnvironmentTargetEntry
    {
        private EnvironmentTargetControls _prefab;
        private Transform _parent;
        public CreateEnvironmentTargetEntry(EnvironmentTargetControls prefab, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
        }
        public IEnvironmentTargetControls Create()
        {
            return EnvironmentTargetControls.Factory(_prefab, _parent);
        }
    }

    public interface IEnvironmentListControls
    {
        public void Configure(EnvironmentListControls.Config config);

    }
    public class EnvironmentListControls : MonoBehaviour, IEnvironmentListControls
    {
        public class Dependencies
        {
            public ICreateEnvironmentTargetEntry EnvironmentTargetCreator { get; set; }
        }

        public class Config
        {
            public List<Target> EnvironmentTargets { get; set; }
            public Action<Target> ButtonAction { get; set; }
        }
        private List<IEnvironmentTargetControls> targetController;
        [SerializeField] private EnvironmentTargetControls prefab;
        void Awake()
        {
            SetDependencies(new Dependencies()
            {
                EnvironmentTargetCreator = new CreateEnvironmentTargetEntry(prefab, this.transform)
            });
        }

        public Dependencies _dependencies { get; private set; }
        public void SetDependencies(Dependencies dependencies)
        {
            targetController = new List<IEnvironmentTargetControls>();
            _dependencies = dependencies;
        }

        public void Configure(Config config)
        {
            UpdateList(config.EnvironmentTargets, config.ButtonAction);
        }

        private void UpdateList(List<Target> possibleEnvironments, Action<Target> action)
        {
            foreach(var target in targetController)
                target.Hide();
            
            if (targetController.Count < possibleEnvironments.Count)
            {
                var listSize = targetController.Count;
                for (int i = listSize; i < possibleEnvironments.Count; i++)
                {
                    targetController.Add(_dependencies.EnvironmentTargetCreator.Create());
                }
            }
            
            var idx = 0;
            foreach (var target in possibleEnvironments)
            {
                targetController[idx].Configure(new EnvironmentTargetControls.Config()
                {
                  Target = target,
                  ButtonAction = action
                });
                targetController[idx].Display();
                idx++;
            }
        }
    }
}
