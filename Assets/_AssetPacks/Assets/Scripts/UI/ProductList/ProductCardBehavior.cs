using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hunt;
using riddlehouse_libraries.products;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using riddlehouse_libraries.products.models;
using riddlehouse_libraries.products.models.DTOs;
using riddlehouse_libraries.analytics;
using UnityEditor;
using Zenject;

[RequireComponent(typeof(WaitToExecuteAction))]
[RequireComponent(typeof(ExecuteActionAtEndOfFixedFrame))]
[RequireComponent(typeof(LayoutElementVerticalResizer))]
public class ProductCardBehavior : MonoBehaviour, IProductCard
{
    public static IProductCard Factory(ProductCardBehavior prefab, RectTransform parent, IProductEvents productEvents)
    {
        var behaviour = Instantiate(prefab, parent);
        behaviour.SetParent(parent);
        behaviour.Initialize(productEvents);
        return behaviour;
    }
    
    [Inject] private IHuntInstantiater _huntInstantiater;
    
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private RectTransform titleTransform;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private RectTransform descriptionTransform;
    [SerializeField] private RectTransform playBtn;
    [SerializeField] private Image bannerImage;
    [SerializeField] private LayoutElement cardLayoutElement;
    [SerializeField] private LayoutElement bodyLayoutElement;
    [SerializeField] private RectTransform layoutContentOwner;
    [SerializeField] private RectTransform bannerRect;

    public class Dependencies
    {
        public ILoadingView LoaderView { get; set; }
        public IHuntInstantiater HuntInstantiater { get; set; }
        public IImageGetter ImageGetter { get; set; }
        public IProductEvents ProductEvents { get; set; }
        public IProductCardStartPanelDataGetter ProductCardStartPanelDataGetter { get; set; }
        public TextMeshProUGUI Title { get; set; }
        public RectTransform TitleTransform { get; set; }
        public TextMeshProUGUI Description { get; set; }
        public RectTransform DescriptionTransform { get; set; }
        public RectTransform PlayBtnTransform { get; set; }
        public Image BannerImage { get; set; }
        public LayoutElement CardLayoutElement { get; set; }
        public LayoutElement BodyLayoutElement { get; set; }
        public RectTransform LayoutContentOwner { get; set; }
        public RectTransform BannerRect { get; set; }
        public IWaitToExecuteAction WaitToExecuteAction { get; set; }
        public IExecuteActionAtEndOfFixedFrame ExecuteActionAtEndOfFixedFrame { get; set; }
    }

    public void Initialize(IProductEvents productEvents)
    {
        SetDependencies(new Dependencies()
        {
            LoaderView = _loaderView,
            HuntInstantiater = _huntInstantiater,
            ImageGetter = ImageGetter.Factory(this),
            ProductEvents = productEvents,
            ProductCardStartPanelDataGetter = new ProductCardStartPanelDataGetter(),
            Title = title,
            TitleTransform = titleTransform,
            Description = description,
            DescriptionTransform = descriptionTransform,
            PlayBtnTransform = playBtn,
            BannerImage = bannerImage,
            CardLayoutElement = cardLayoutElement,
            BodyLayoutElement = bodyLayoutElement,
            LayoutContentOwner = layoutContentOwner,
            BannerRect = bannerRect,
            WaitToExecuteAction = GetComponent<WaitToExecuteAction>(),
            ExecuteActionAtEndOfFixedFrame = GetComponent<ExecuteActionAtEndOfFixedFrame>(),
        });
    }
    public Dependencies _dependencies { get; private set; }
    public void SetDependencies(Dependencies dependencies)
    {
        _dependencies = dependencies;
    }
    
    public string GetProductType()
    {
        throw new NotImplementedException();
    }

    public void SetParent(Transform parent)
    {
        var t = transform;
        t.SetParent(parent);
        t.localScale = new Vector3(1,1,1);
    }

    private string _descriptionExcerpt;
    private string _fullDescription;
    [SerializeField] public float bottomPadding;
    [SerializeField] private bool displaysExerpt = false;
    [SerializeField] private  float animSpeedModifier;
    private ProductBasicsDTO _productData;
    private float _oldDescriptionSize = Single.NaN;
    [Inject] private ILoadingView _loaderView;

    public void Configure(ProductBasicsDTO productData, Action cardReady)
    {
        _productData = productData;
        
        _dependencies.ExecuteActionAtEndOfFixedFrame.Configure(WaitUntilDescriptionHasChangedSize, SetLayoutUpdate);
        
        _dependencies.Title.text = productData.Title;
        _fullDescription = productData.Description;
        _dependencies.Description.text = _descriptionExcerpt = LimitString(productData.Description, 150);
        displaysExerpt = true;
        
        _dependencies.ImageGetter.GetImage(productData.BackgroundPictureUrl, false, (collectedSprite) =>
        {
            if (collectedSprite != null && _dependencies.BannerImage != null)
            {
                _dependencies.BannerImage.sprite = collectedSprite;
            }
            else
            {
                throw new ArgumentException(
                    "The sprite event was called, but no sprite was assigned at the time of invocation.");
            }
            
            ResizeUI();
            cardReady.Invoke();
        });
    }

    private string LimitString(string productDescription, int limit)
    {
        if (150 > productDescription.Length)
            return productDescription;
        return productDescription.Substring(0, limit) + " (...)";
    }
    public void Configure(ProductBasicsDTO cardData, Sprite collectedSprite, Action readyForDisplay)
    {
        throw new NotImplementedException();
    }

    public async void Play()
    {
        _dependencies.LoaderView.Display();
        var startPanelData = await _dependencies.ProductCardStartPanelDataGetter.GetPanelData(_productData.Id);
        StartGame(_dependencies.ProductEvents, null, startPanelData);
    }

    public void StartGame(IProductEvents productEvents,  Action<bool> isReady, StartPanelData startPanelData)
    {
        new Analytics().ProductStart(_productData.Id);
        var huntView = _dependencies.HuntInstantiater.Create(startPanelData.ProductType, startPanelData.Id);
        _dependencies.WaitToExecuteAction.StopWaiting();
        _dependencies.WaitToExecuteAction.Configure(async () =>
        {
            await huntView.Configure(new ProductController.Config() { 
                ProductId = startPanelData.Id, 
                EndProduct = (complete) =>
            {
                huntView.DestroySelf();
            }});
            _dependencies.LoaderView.Hide();
        }, 0.1f);
        _dependencies.WaitToExecuteAction.BeginWaiting();

    }

    //used by a touch event on the product card image background - but for some reason not registered.
    // ---Do not clean this code unless you change the UI.
    public void ToggleExcerpt()
    {
        displaysExerpt = !displaysExerpt;
        if (displaysExerpt)
        {
            _dependencies.Description.text = _descriptionExcerpt;
        }
        else
        {
            _dependencies.Description.text = _fullDescription;
        }
        ResizeUI();
    }

    private void ResizeUI()
    {
        if (!gameObject.activeSelf) //can't activate coroutine from a disabled gameobject, so in order to hide initialization reset. We set the data here.
        {
            _dependencies.CardLayoutElement.preferredHeight = _dependencies.LayoutContentOwner.rect.height;
            LayoutRebuilder.ForceRebuildLayoutImmediate(_dependencies.LayoutContentOwner);
            gameObject.SetActive(true);
        }
        else
        {
            _oldDescriptionSize = _dependencies.DescriptionTransform.rect.height;
            _dependencies.ExecuteActionAtEndOfFixedFrame.BeginWaiting();
        }
    }

    private void WaitUntilDescriptionHasChangedSize()
    {
        _oldDescriptionSize = _dependencies.DescriptionTransform.rect.height;
        if (Math.Abs(_oldDescriptionSize - _dependencies.DescriptionTransform.rect.height) < 0.5f)
        {
            _dependencies.ExecuteActionAtEndOfFixedFrame.StopWaiting(false);
        }
    }

    private void SetLayoutUpdate()
    {
        _oldDescriptionSize = _dependencies.DescriptionTransform.rect.height;
        _dependencies.BodyLayoutElement.preferredHeight = _oldDescriptionSize + _dependencies.TitleTransform.rect.height+_dependencies.PlayBtnTransform.rect.height+bottomPadding;
        _dependencies.CardLayoutElement.preferredHeight = _dependencies.LayoutContentOwner.rect.height+_oldDescriptionSize;
    }

    public void OnDisable()
    {
        _dependencies.ExecuteActionAtEndOfFixedFrame.StopWaiting(true);
    }
}
