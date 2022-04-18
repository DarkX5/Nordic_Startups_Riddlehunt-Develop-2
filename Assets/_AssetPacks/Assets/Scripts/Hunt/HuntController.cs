using System;
using riddlehouse_libraries.products;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.huntProduct;
using riddlehouse_libraries.products.Stops;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Hunt
{
    public interface IHuntController
    {
        public void Configure(HuntController.Config config);
        public void EndHunt(bool completed);
        public void DestroySelf();
    }
    [RequireComponent(typeof(GameObjectDestroyer))]
    public class HuntController : MonoBehaviour, IHuntController
    {
        public static IHuntController Factory(HuntController prefab, Transform parent)
        {
            HuntController behaviour = Instantiate(prefab, parent);
            behaviour.Initialize();
            return behaviour;
        }
        
        public class Dependencies
        {
            public IGameObjectDestroyer GOD { get; set; }
            public IStopControllerInstantiator StopControllerInstantiator { get; set; }
            public IStopModelConverter StopModelConverter { get; set; }
        }

        public class Config
        {
            public Action Ready { get; set; }
            public HuntProductFlow Flow { get; set; }
            public Action<bool> EndHunt { get; set; }
        }
        
        public void Initialize()
        {
            SetDependencies(new Dependencies()
            {
                GOD = GetComponent<GameObjectDestroyer>(),
                StopControllerInstantiator = _stopControllerInstantiator,
                StopModelConverter = new StopModelConverter(),
            });
        }
        
        [Inject] private IStopControllerInstantiator _stopControllerInstantiator;
        
        public Dependencies _dependencies { get; private set; }
        public void SetDependencies(Dependencies dependencies)
        {
            _dependencies = dependencies;
        }

        private IStopController _stopController;
        private Config _config;
        public void Configure(Config config)
        {
            _config = config;
            switch (_config.Flow.Stop.Type)
            {
                case StopType.MapStop2D:
                    CreateMap2D(config.Ready);
                    break;
                case StopType.MapBox:
                    CreateMapBox(config.Ready);
                    break;
                default: 
                    throw new ArgumentException(
                        $"Stoptype not defined for huntcontroller; attempted to configure hunt with type: {_config.Flow.Stop.Type}"
                        );
            }  
        }

        private void CreateMap2D(Action ready)
        {
            var map2DStopController = _dependencies.StopControllerInstantiator.CreateMap2D(null);
            _stopController = map2DStopController;
            var stop = _dependencies.StopModelConverter.ConvertMap2DModel(_config.Flow.Stop);
            map2DStopController.ConfigureAndOpenStop(new Map2DStopController.Config()
            {
                Stop = stop,
                EndStop = EndHunt,
                StopControllerInstantiator = _dependencies.StopControllerInstantiator,
                MapCanvasControllerConfig = new MapCanvasController.Config(),
            });
            ready.Invoke();
        }

        private void CreateMapBox(Action ready)
        {
            var mapBoxStopController = _dependencies.StopControllerInstantiator.CreateMapBox(null);
            _stopController = mapBoxStopController;
            var stop = _dependencies.StopModelConverter.ConvertMapBoxModel(_config.Flow.Stop);
            mapBoxStopController.Configure(new MapBoxStopController.Config()
            {
                Stop = stop,
                MapCanvasControllerConfig = new MapCanvasController.Config(),
                StopControllerInstantiator = _dependencies.StopControllerInstantiator,
                EndStop = EndHunt
            });
            ready.Invoke();
            _stopController.OpenStop();
        }
        
        public void EndHunt(bool completed)
        {
            _config?.EndHunt.Invoke(completed);
        }

        public void DestroySelf()
        {
            _stopController?.DestroySelf();
            _dependencies.GOD.Destroy();
        }
    }
}
