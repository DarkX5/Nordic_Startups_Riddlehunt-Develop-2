using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using riddlehouse_libraries.products.Assets.Map2DAssets;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.resources;
using riddlehouse_libraries.products.resources.Map;
using riddlehouse_libraries.products.Stops;
using UnityEngine;

public interface IPOIController
{
    public void Configure(POIController.Config config);
    public float GetXBounds();
    public float GetYBounds();
    public void SetLocalPosition(Vector3 pos, Transform parent);
    public void SetLocalPosition(Vector3 pos);
    public void SetLocalScale(Vector3 scale);
    public void SetLocalRotation(Vector3 rotation);
    public void DestroySelf();
    public Map2DPosition GetRealWorldPosition();
    public void UpdatePOIState(PoiStates state);
}
[RequireComponent(typeof(POIButtonAnimationController))]
public class POIController : MonoBehaviour, IPOIController
{
    public static IPOIController Factory(POIController prefab, Transform parent)
    {
        var behavior = Instantiate(prefab, parent);
        behavior.Initialize();
        return behavior;
    }

    public class Dependencies
    {
        public IPOIButtonAnimationController AnimationController { get; set; }
        public ISpriteHelper SpriteHelper { get; set; }
        
        public SpriteRenderer IconSR { get; set; }
        public SpriteRenderer BackgroundSR { get; set; }
        public SpriteRenderer FrameSR { get; set; }
    }

    public class POIStyle
    {
        private Action<bool> _ready;
        private ISpriteHelper _spriteHelper;
        private POI2DResource _resource;
        public POIStyle(POI2DResource resource, ISpriteHelper spriteHelper, Action<bool> ready)
        {
            _resource = resource;
            _spriteHelper = spriteHelper;
            _ready = ready;

            IconColor = new Color32(resource.IconColor.R, resource.IconColor.G, resource.IconColor.B,
                resource.IconColor.A);
            BackgroundColor = new Color32(resource.BackgroundColor.R, resource.BackgroundColor.G,
                resource.BackgroundColor.B, resource.BackgroundColor.A);
            FrameColor = new Color32(resource.FrameColor.R, resource.FrameColor.G, resource.FrameColor.B,
                resource.FrameColor.A);

        }

        private bool _initialized = false;
        public async void DownloadAssets()
        {
            if (!_initialized)
            {
                _initialized = true;
                try
                {
                    ButtonIcon = _spriteHelper.GetSpriteFromByteArray(await _resource.ButtonIcon.GetIcon(), _resource.PixelsPerUnit);
                    _ready.Invoke(true);
                }
                catch
                {
                    _ready.Invoke(false);
                }
            }
        }

        public Sprite ButtonIcon
        {
            get;
            private set;
        }
        
        public Color IconColor { get; private set; }
        public Color BackgroundColor { get; private set; }
        public Color FrameColor { get; private set; }

    }
    public class Config
    {
        public Action<string> ClickedAction { get; set; }
        public POI2DResource Resource { get; set; }
        public POI2DListAsset.Poi2DAsset PoiAsset { get; set; }
    }

    public void Awake()
    {
        Initialize();
    }
    
    private void Initialize()
    {
        var animationController = GetComponent<POIButtonAnimationController>();
        animationController.Initialize();
        SetDependencies(new Dependencies()
        {
            AnimationController = animationController,
            SpriteHelper = new SpriteHelper(),
            IconSR = IconSR,
            BackgroundSR = Background,
            FrameSR = Frame
        });
    }

    [SerializeField] private SpriteRenderer IconSR;
    [SerializeField] private SpriteRenderer Background;
    [SerializeField] private SpriteRenderer Frame;
    public Dependencies _dependencies { get; private set; }
    public void SetDependencies(Dependencies dependencies)
    {
        _dependencies = dependencies;
    }

    private Config _config;
    private POIStyle _style;
    public void Configure(Config config)
    {
        _config = config;
        _style = new POIStyle(config.Resource, _dependencies.SpriteHelper, POIStyleReady);
        _style.DownloadAssets();
    }

    public float GetXBounds()
    {
        return _dependencies.FrameSR.sprite.bounds.size.x;
    }

    public float GetYBounds()
    {
        return _dependencies.FrameSR.sprite.bounds.size.y;
    }
    
    public void SetLocalPosition(Vector3 pos, Transform parent)
    {
        transform.SetParent(parent);
        SetLocalPosition(pos);
    }

    public void SetLocalPosition(Vector3 pos)
    {
        transform.localPosition = pos;
    }

    public void SetLocalScale(Vector3 scale)
    {
        this.transform.localScale = scale;
    }
    
    public void SetLocalRotation(Vector3 rot)
    {
        this.transform.localRotation = Quaternion.Euler(rot);
    }

    public Map2DPosition GetRealWorldPosition()
    {
        return _config.PoiAsset.RealWorldPosition;
    }
    
    public void ClickedAction()
    {
        if (!_dependencies.AnimationController.IsDisabled())
        {
            _config?.ClickedAction(_config.PoiAsset.Id);
            _dependencies.AnimationController.ClickAnimation();
        }
    }

    private void POIStyleReady(bool completed)
    {
        if (completed)
        {
            _dependencies.IconSR.sprite = _style.ButtonIcon;
            _dependencies.IconSR.color = _style.IconColor;

            _dependencies.BackgroundSR.color = _style.BackgroundColor;
            _dependencies.FrameSR.color = _style.FrameColor;
        }
    }

    public void UpdatePOIState(PoiStates state)
    {
        if(state != PoiStates.Hidden)
            this.gameObject.SetActive(true);
        switch (state)
        {
            case PoiStates.Idle:
                _dependencies.AnimationController.SetIdle();
                break;
            case PoiStates.Highlighted:
                _dependencies.AnimationController.SetHighlighted();
                break;
            case PoiStates.Hidden:
                this.gameObject.SetActive(false);
                break;
            case PoiStates.Disabled:
                _dependencies.AnimationController.SetDisabled();
                break;
            case PoiStates.Completed:
                _dependencies.AnimationController.SetCompleted();
                break;
            default:
                throw new ArgumentException("No such case defined");
        }
    }
    
    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
