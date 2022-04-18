using System;
using System.Runtime.InteropServices;
using RHPackages.Core.Scripts.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Hunt
{
    public interface IMapMenuController : IViewActions
    {
        public void Configure(Action backBtnAction);
        public void Display();
        public void Hide();
    }
    
    [RequireComponent(typeof(BasicComponentDisplayController))]
    public class MapMenuController : BaseFullscreenView, IMapMenuController
    {
        public static IMapMenuController Factory(MapMenuController prefab, RectTransform parent)
        {
            var behaviour = Instantiate(prefab);
            behaviour.Initialize(parent);
            behaviour.Hide();
            return behaviour;
        }
        
        public class Dependencies
        {
            public IComponentDisplayController DisplayController { get; set; }
        }
        
        public void Initialize(RectTransform parent)
        {
            var displayController = GetComponent<BasicComponentDisplayController>();
            displayController.Initialize();
            SetDependencies(new Dependencies()
            {
                DisplayController = displayController,
            });
            displayController.FitToScreen(parent);
        }
        public Dependencies _dependencies { get; private set; }

        public void SetDependencies(Dependencies dependencies)
        {
            _dependencies = dependencies;
        }

        private Action _BackBtnAction;
        public void Configure(Action backBtnAction)
        {
            _BackBtnAction = backBtnAction;
        }

        public void BackButtonAction()
        {
            _BackBtnAction.Invoke();
        }

        public override void Display()
        {
            _dependencies.DisplayController.Display();
        }

        public override void Hide()
        {
            _dependencies.DisplayController.Hide();
        }
    }
}
