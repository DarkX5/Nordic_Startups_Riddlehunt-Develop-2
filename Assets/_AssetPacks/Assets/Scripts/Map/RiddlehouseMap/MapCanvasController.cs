using System;
using System.Collections;
using System.Collections.Generic;
using RHPackages.Core.Scripts;
using RHPackages.Core.Scripts.UI;
using Riddlehouse.Core.Helpers.Helpers;
using UnityEngine;

namespace Hunt
{
    public interface IMapCanvasController : IGameViewCanvasController
    {
        public void ConfigureAndDisplay(MapCanvasController.Config config);
    }
    
    [RequireComponent(typeof(CanvasController))]
    public class MapCanvasController : MonoBehaviour, IMapCanvasController
    {
        public static IMapCanvasController Factory(MapCanvasController prefab, Transform parent)
        {
            var behaviour = Instantiate(prefab, parent);
            behaviour.Initialize();
            return behaviour;
        }

        public class Dependencies
        {
            public IMapMenuController MapMenuController { get; set; }
            public ICanvasController CanvasController { get; set; }
        }

        public class Config
        {
            public CanvasController.Config CanvasConfig { get; set; }
            public Action BackButtonAction { get; set; }
        }
        
        public void Initialize()
        {
            var canvasController = GetComponent<CanvasController>();
            canvasController.Initialize();

            var mapMenuController = MapMenuController.Factory(_mapMenuController, (RectTransform)this.transform);

            SetDependencies(new Dependencies()
            {
                MapMenuController = mapMenuController,
                CanvasController = canvasController
            });
        }

        [SerializeField] private MapMenuController _mapMenuController;
        public Dependencies _dependencies { get; private set; }
    
        public void SetDependencies(Dependencies dependencies)
        {
            _dependencies = dependencies;
        }

        private Config _config;
        public void ConfigureAndDisplay(Config config)
        {
            _config = config;
            _dependencies.CanvasController.Configure();
            _dependencies.MapMenuController.Configure(_config.BackButtonAction);
            _dependencies.MapMenuController.Display();
            Display();
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }

        public void Display()
        {
            this.gameObject.SetActive(true);
        }

        public void AttachViewToCanvas(IViewActions view, int index)
        {
            view.FitInView((RectTransform)transform, new UIFitters(), index);
        }
    }
}