using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSelection.Components;
using riddlehouse_libraries.products.resources;
using UI.UITools;
using UnityEngine;

public interface IRoleSelectionComponent
{
    public void Configure(RoleSelectionComponent.Config config);
}
[RequireComponent(typeof(ExecuteActionAtEndOfFixedFrame))]
public class RoleSelectionComponent : MonoBehaviour, IRoleSelectionComponent
{
    public class Config
    {
        public RoleSelectionComponentResource Resource { get; set; }
        public Action<string> AddRoleAction { get; set; }
    }
    public class Dependencies
    {
        public IHorizontalGridComponentResizer HorizontalGridComponentResizer { get; set; }
        public IRoleButtonInstantiater RoleButtonInstantiater { get; set; }
        public IExecuteActionAtEndOfFixedFrame ExecuteActionAtEndOfFixedFrame { get; set; }
    }

    public void Initialize()
    {
        _horizontalGridComponentResizer.Initialize();
        SetDependencies(new Dependencies()
        {
            HorizontalGridComponentResizer = _horizontalGridComponentResizer,
            RoleButtonInstantiater = new RoleButtonInstantiater(_roleButton, (RectTransform)this.transform),
            ExecuteActionAtEndOfFixedFrame = GetComponent<ExecuteActionAtEndOfFixedFrame>()
        });
    }

    [SerializeField] private RoleButton _roleButton;
    [SerializeField] private HorizontalGridComponentResizer _horizontalGridComponentResizer;
    public Dependencies _dependencies { get; set; }

    public void SetDependencies(Dependencies dependencies)
    {
        _dependencies = dependencies;
    }
    
    private List<IRoleButton> _roleButtons;
    private Config _config;
    public void Configure(Config config)
    {
        _config = config;
        _roleButtons ??= new List<IRoleButton>();
        for(int i = _roleButtons.Count; i < _config.Resource.RoleButtons.Count; i++)
            _roleButtons.Add(_dependencies.RoleButtonInstantiater.Create());
        
        for (int i = _config.Resource.RoleButtons.Count; i < _roleButtons.Count; i++)
        {
            _roleButtons[i].DestroySelf(); //destroys the gameobject
            _roleButtons.RemoveAt(i); //cleans up the array
            i--; //sets the counter one back to account for the now missing roleButton
        }

        for(int i = 0; i < _roleButtons.Count; i++)
        {
            _roleButtons[i].Configure(new RoleButton.Config()
            {
                Resource = _config.Resource.RoleButtons[i],
                buttonAction = AddRoleAction
            });
        }
        _dependencies.ExecuteActionAtEndOfFixedFrame.Configure(WaitForGameObjectToHaveWidth, UpdateGrid);
    }

    private void AddRoleAction(string id)
    {
        _config.AddRoleAction.Invoke(id);
    }
    private void WaitForGameObjectToHaveWidth()
    {
        if(_dependencies != null)
            if (_dependencies.HorizontalGridComponentResizer.CanResize())
                _dependencies.ExecuteActionAtEndOfFixedFrame.StopWaiting(false);
    }

    private void UpdateGrid()
    {
        var count = _roleButtons?.Count ?? 1;
        _dependencies?.HorizontalGridComponentResizer?.ResizeGrid(count, 450f);
    }
    public void OnEnable()
    {
        if(_dependencies != null)
            if(!_dependencies.ExecuteActionAtEndOfFixedFrame.IsWaiting())
                _dependencies.ExecuteActionAtEndOfFixedFrame.BeginWaiting();
    }

    public void OnDisable()
    {
        if(_dependencies != null)
            if(_dependencies.ExecuteActionAtEndOfFixedFrame.IsWaiting()) 
                _dependencies.ExecuteActionAtEndOfFixedFrame.StopWaiting(true);

    }
}