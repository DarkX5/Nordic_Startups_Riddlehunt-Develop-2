using Hunt;
using riddlehouse_libraries;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace SceneInstallers
{
    public class StartupSceneInstaller : MonoInstaller<StartupSceneInstaller>
    {
        [SerializeField] private CanvasLayerTypes canvasLayerTypes;
        [SerializeField] private ProductCardInstantiater productCardInstantiater;
        [SerializeField] private HuntInstantiater huntInstantiater;
        [FormerlySerializedAs("MainPrefab")] [SerializeField] private MainController mainPrefab;
        [SerializeField] private StopControllerInstantiator stopControllerInstantiator;
        [SerializeField] private MapboxMapInstantiator _mapboxMapInstantiator;
        [SerializeField] private LogoLoadingController _initalLoadingScreen;

        public override void InstallBindings()
        {
            var addressableAssetLoader = this.gameObject.AddComponent<AddressableAssetLoader>();
            var clm = new CanvasLayerManager(canvasLayerTypes);
            var loginHandler = new LoginHandler(new LoginWebView(clm));
            Container.Bind<CanvasLayerManager>().FromInstance(clm);
            Container.Bind<IStartup>().To<Startup>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LoginHandler>().FromInstance(loginHandler).AsSingle().NonLazy();
            Container.BindInterfacesTo<ProcessDeepLinkMngr>().AsSingle().NonLazy();
            Container.Bind<IProductCardInstantiater>().FromInstance(productCardInstantiater);
            Container.Bind<IHuntInstantiater>().FromInstance(huntInstantiater);
            Container.Bind<IStopControllerInstantiator>().FromInstance(stopControllerInstantiator);
            Container.Bind<IMapboxMapInstantiator>().FromInstance(_mapboxMapInstantiator);
            Container.Bind<ILoadingView>().FromInstance(_initalLoadingScreen);
            Container.Bind<IAddressableAssetLoader>().FromInstance(addressableAssetLoader);
        }

        public override void Start()
        {
            base.Start();
            InitializeScene();
        }
        public void InitializeScene()
        {
            MainController.Factory(mainPrefab, Camera.main);
        }
    }
}