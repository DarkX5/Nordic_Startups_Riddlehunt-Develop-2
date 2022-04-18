using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CharacterSelection;
using Riddlehouse.Core.Helpers.Helpers;
using riddlehouse_libraries.products;
using riddlehouse_libraries.products.huntProduct;
using riddlehouse_libraries.products.models;
using UnityEngine;
using Zenject;

namespace Hunt
{
    public interface IHuntControllerInstantiator
    {
        public IHuntController Create();
    }

    public class HuntControllerInstantiator: IHuntControllerInstantiator
    {
        private readonly HuntController _prefab;
        private readonly Transform _parent;

        public HuntControllerInstantiator(HuntController prefab, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
        }

        public IHuntController Create()
        {
            return HuntController.Factory(_prefab, _parent);
        }
    }

    public interface IAdressableViewInstantiator
    {
        public Task<ILoadingView> GetLoaderView(string address);
    }
    public class AdressableViewInstantiator: IAdressableViewInstantiator
    {
        private IAddressableAssetLoader _loader;
        private RectTransform _parent;
        public AdressableViewInstantiator(IAddressableAssetLoader loader, RectTransform parent)
        {
            _loader = loader;
            _parent = parent;
        }

        public async Task<ILoadingView> GetLoaderView(string address)
        {
            var prefab = await _loader.DownloadGameobject(address);
            ILoadingView view = LogoLoadingController.Factory(prefab, _parent);
            return view;
        }
    }

    public interface IProductController
    {
        public Task Configure(ProductController.Config config);
        public void DestroySelf();
    }

    [RequireComponent(typeof(GameObjectDestroyer))]
    [RequireComponent(typeof(CanvasController))]
    public class ProductController : MonoBehaviour, IProductController, IHuntView
    {
        public static IProductController Factory(ProductController prefab, string productId)
        {
            ProductController behaviour = Instantiate(prefab);
            behaviour.Initialize(productId);
            return behaviour;
        }
        
        public class Dependencies
        {
            public ICharacterSelectionFlowController CharacterSelectionFlowController { get; set; }
            public IProductResourceService ProductResourceService { get; set; }
            public ICanvasController CanvasController { get; set; }
            public IHuntControllerInstantiator HuntControllerInstantiator { get; set; }
            public IGetProductFlowData GetProductFlowData { get; set; }
            public ICanvasLayerManager Clm { get; set; }
            public IGameObjectDestroyer GOD { get; set; }
            public IAdressableViewInstantiator AdressableViewInstantiator { get; set; }
        }

        public class Config
        {
            public string ProductId { get; set; }
            public Action<bool> EndProduct { get; set; }
        }

        [Inject] private IAddressableAssetLoader _addressableAssetLoader;
        [Inject] private CanvasLayerManager _clm;
        [SerializeField] private CharacterSelectionFlowController characterSelectionFlowPrefab;
        [SerializeField] private HuntController huntControllerPrefab;

        public void Initialize(string productId)
        {
            var characterSelectionFlowController =
                CharacterSelectionFlowController.Factory(characterSelectionFlowPrefab, (RectTransform)this.transform);

            var canvasController = GetComponent<CanvasController>();
            canvasController.Initialize();
            Debug.Log(DataPathHelper.PersistentDataPath);

            SetDependencies(new Dependencies()
            {
                CharacterSelectionFlowController = characterSelectionFlowController,
                ProductResourceService =
                    new ProductResourceService(DataPathHelper.PersistentDataPath, productId),
                CanvasController = canvasController,
                HuntControllerInstantiator = new HuntControllerInstantiator(huntControllerPrefab, this.transform),
                GetProductFlowData = new ProductService(DataPathHelper.PersistentDataPath),
                Clm = _clm,
                GOD = GetComponent<GameObjectDestroyer>(),
                AdressableViewInstantiator = new AdressableViewInstantiator(_addressableAssetLoader, (RectTransform)this.transform)
            });
        }

        public Dependencies _dependencies { get; private set; }

        public void SetDependencies(Dependencies dependencies)
        {
            _dependencies = dependencies;
            _dependencies.CanvasController.Configure(new CanvasController.Config()
            {
                ViewCamera = Camera.main
            });
        }

        private IHuntController _huntController;
        private ILoadingView _loadingView;
        private Config _config;

        public async Task Configure(Config config)
        {
            _config = config;
            await _dependencies.ProductResourceService.IsReady();

            _loadingView = await _dependencies.AdressableViewInstantiator.GetLoaderView(_dependencies
                .ProductResourceService.CharacterSelectionResources.LoadingView.Address);
            _loadingView.FitInView((RectTransform)this.transform, new UIFitters(), this.transform.childCount+1); //put this view on top of the stack.

            _dependencies.CharacterSelectionFlowController.Configure(new CharacterSelectionFlowController.Config()
            {
                Resource = _dependencies.ProductResourceService.CharacterSelectionResources
                    .CharacterSelectionFlowResource,
                FlowComplete = CharacterSelectionFlowComplete,
                FlowAbandoned = DestroySelf
            });
        }

        private async void CharacterSelectionFlowComplete(List<HuntCharacterData> players)
        {
            _loadingView.Display();
            var flow = await _dependencies.GetProductFlowData.GetHuntProductFlow(_config.ProductId, players, new HuntSessionPersistor());
            _huntController = _dependencies.HuntControllerInstantiator.Create();
            _loadingView.FitInView((RectTransform)this.transform, new UIFitters(), this.transform.childCount+1); //put this view on top of the stack.
            _huntController.Configure(new HuntController.Config()
            {
                Ready = _loadingView.Hide,
                Flow = flow,
                EndHunt = EndProduct
            });
        }

        public void EndProduct(bool completed)
        {
            _config?.EndProduct.Invoke(completed);
        }
        
        public void DestroySelf()
        {
            _huntController?.DestroySelf();
            _dependencies.CanvasController.DestroySelf();
            _dependencies.GOD.Destroy();
        }
        
        // clean up below later.
        
        public Task Configure(StartPanelData startPanelData, IHuntAssetGetter assetGetter,
            IGetProductFlowData flowGetter) //old config function, deprecated.
        {
            throw new System.NotImplementedException();
        }
    }
}