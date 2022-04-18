using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using riddlehouse_libraries.products;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using riddlehouse_libraries.products.models;
using riddlehouse_libraries.products.models.DTOs;
using riddlehouse_libraries.analytics;
using UnityEngine.Serialization;
public class TimeRobberProductCard : IProductCard
{
    private IProductCardStartPanelDataGetter ProductCardStartPanelDataGetter = null;
    public static TimeRobberProductCard Factory(TimeRobberProductCardBehaviour behaviour, RectTransform parent, IProductEvents productEvents)
    {
        if (productEvents == null)
        {
            throw new ArgumentException("ProductEvents missing in factory");
        }
        
        var imageGetter = ImageGetter.Factory(behaviour);
        var card = new TimeRobberProductCard(behaviour.gameObject, behaviour, imageGetter, productEvents);
        behaviour.SetProductCard(card);
        if(card.ProductCardStartPanelDataGetter == null) //unity adaptation
            card.SetDataGetter (new ProductCardStartPanelDataGetter());
        card.SetParent(parent);
         return card;
    }
    /// <summary>
    /// Designed for use with Mock objects in tests.
    /// </summary>
    /// <param name="_productCardStartPanelDataGetter">MockedObject</param>
    public void SetDataGetter(IProductCardStartPanelDataGetter _productCardStartPanelDataGetter)
    {
        if(ProductCardStartPanelDataGetter == null)
            ProductCardStartPanelDataGetter = _productCardStartPanelDataGetter;
    }

    private readonly GameObject _cardGameObject;
    
    private readonly ICardActions _cardActions;
    private readonly IImageGetter _imageGetter;
    private readonly IProductEvents _productEvents;
    
    private string productID = "[null]";
    public void SetProductId(string id)
    {
        if(productID == "[null]")
            productID = id;
    }
    
    public TimeRobberProductCard(
        GameObject card, 
        ICardActions actions, 
        IImageGetter imageGetter, 
        IProductEvents productEvents
        )
    {
        _cardGameObject = card;
        _cardActions = actions;
        _imageGetter = imageGetter;
        _productEvents = productEvents;
    }

    public string GetProductType()
    {
        throw new NotImplementedException();
    }

    public void SetParent(Transform parent)
    {
        _cardGameObject.transform.SetParent(parent);
        _cardGameObject.transform.localScale = new Vector3(1,1,1);
    }

    public void Configure(ProductBasicsDTO cardData, Action readyForDisplay)
    {
        _imageGetter.GetImage(cardData.BackgroundPictureUrl, false,(collectedSprite) =>
        {
            _cardActions.Configure(cardData, collectedSprite, readyForDisplay);
        });
    }

    public void Configure(ProductBasicsDTO productData, Action cardReady, Action startLoading, Action stopLoading)
    {
        throw new NotImplementedException();
    }

    public void StartGame(IProductEvents productEvents, Action<bool> isReady, StartPanelData startPanelData = null)
    {
        throw new NotImplementedException();
    }

    public async void Play()
    {
        var startPanelDto = await ProductCardStartPanelDataGetter.GetPanelData(productID);
        new Analytics().ProductStart(productID);
        _cardActions.StartGame(_productEvents, (ready) =>
        {
            if (ready)
            {
                _productEvents.ProductStarted();
            }
        }, startPanelDto);
    }
}

public class TimeRobberProductCardBehaviour : MonoBehaviour, ICardActions
{
    [SerializeField]
    private TMP_Text title;
    [SerializeField]
    private RectTransform titleTransform;
    
    [SerializeField]
    private TMP_Text description;
    [SerializeField]
    private RectTransform descriptionTransform;
    
    private string descriptionExcerpt;
    private string fullDescription;
    
    [SerializeField] 
    private RectTransform playBtn;
    
    [SerializeField]
    private string imageURl;
    [SerializeField]
    private Image bannerImage;
    
    [SerializeField]
    private bool displaysExerpt = false;
    
    [SerializeField]
    private LayoutElement cardLayoutElement;
    [SerializeField]
    private LayoutElement bodyLayoutElement;
    [SerializeField]
    private RectTransform layoutContentOwner;

    [SerializeField] private RectTransform bannerRect;

    [SerializeField] public float bottomPadding;


    private TimeRobberProductCard ProductCard;
    
    [SerializeField] private TimerobberProductBehaviour timerobberProductBehaviourPrefab;

    [SerializeField] private CanvasGroup _canvasGroup;
    
    /// <summary>
    /// For testing purposes, since we can't bind this layer from inside MoQ without it.
    /// </summary>
    /// <param name="_productCard">the associated business logic necessary to run the program.</param>
    /// <exception cref="ArgumentException">will be sent if you try to set the product card after it's already been set.</exception>
    public void SetProductCard(TimeRobberProductCard _productCard)
    {
        if(ProductCard == null)
            ProductCard = _productCard;
        else
        {
            throw new ArgumentException("Can only be set once.");
        }
    }
    
    public void Configure(ProductBasicsDTO cardData, Sprite collectedSprite, Action readyForDisplay)
    {
        title.text = cardData.Title;
        descriptionExcerpt = 150 > cardData.Description.Length
            ? cardData.Description
            : cardData.Description.Substring(0, 150) + " (...)";
        fullDescription = cardData.Description;
        description.text = descriptionExcerpt;
        displaysExerpt = false;
        if (collectedSprite != null && bannerImage != null)
        {
            bannerImage.sprite = collectedSprite;
            _canvasGroup.alpha = 1;
        }
        else
        {
            throw new ArgumentException(
                "The sprite event was called, but no sprite was assigned at the time of invocation.");
        }
        ProductCard.SetProductId(cardData.Id);
        ResizeUI();
        readyForDisplay.Invoke();
    }

    public void StartGame(IProductEvents productEvents,  Action<bool> isReady, StartPanelData startPanelData = null)
    {
        StartCoroutine(SpawnDelay(CreateHuntView(productEvents), startPanelData, isReady));
    }

    public IEnumerator SpawnDelay(ITimerobberProductActions huntView, StartPanelData startPanelData, Action<bool> isReady)
    {
        yield return new WaitForSeconds(0.1f);
        huntView.Configure(startPanelData, null, new ProductService(DataPathHelper.PersistentDataPath));
        isReady(true);
    }
    private ITimerobberProductActions CreateHuntView(IProductEvents productEvents)
    {
        var foundTopParent = false;
        var p = this.transform;
        while (!foundTopParent)
        {
            if (p.CompareTag("UITop"))
                foundTopParent = true;
            else
                p = p.transform.parent;
        }

        RectTransform rt_parent = (RectTransform) p;
        
        var _huntView = TimerobberProductBehaviour.Factory(timerobberProductBehaviourPrefab, productEvents, Camera.main);
        return _huntView;
    }

    public void Play()
    {
        ProductCard.Play();
    }
    
    //used by a touch event on the product card image background - but for some reason not registered.
    // ---Do not clean this code unless you change the UI.
    public void ToggleExcerpt()
    {
        displaysExerpt = !displaysExerpt;
        if (displaysExerpt)
        {
            description.text = descriptionExcerpt;
        }
        else
        {
            description.text = fullDescription;
        }
        ResizeUI();
    }

    private void ResizeUI()
    {
        if (!gameObject.activeSelf) //can't activate coroutine from a disabled gameobject, so in order to hide initialization reset. We set the data here.
        {
            var necessaryHeight = calculateNecessaryHeight();
            bodyLayoutElement.preferredHeight = necessaryHeight;
            cardLayoutElement.preferredHeight = necessaryHeight + bannerRect.rect.height;
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutContentOwner);
            gameObject.SetActive(true);
        }
        else
        {
            StartCoroutine(setLayoutUpdate(descriptionTransform.rect.height));
        }
    }

    private IEnumerator setLayoutUpdate(float descriptionSize)
    {
        while (descriptionSize == descriptionTransform.rect.height)
        {
            yield return new WaitForFixedUpdate();
        }
        if (displaysExerpt)
        {
            StartCoroutine(animateCardClose(closed, 0.01f, animSpeedModifier, cardLayoutElement,
                cardLayoutElement.preferredHeight,
                calculateNecessaryHeight() + bannerRect.rect.height));
        }
        else
        {
            StartCoroutine(animateCardOpen(open, 0.99f, animSpeedModifier, cardLayoutElement,
                cardLayoutElement.preferredHeight,
                calculateNecessaryHeight() + bannerRect.rect.height));
        }
    }

    private float calculateNecessaryHeight()
    {
        var necessaryHeight = 0f;
        necessaryHeight += titleTransform.rect.height;

        necessaryHeight += descriptionTransform.rect.height;

        necessaryHeight += playBtn.rect.height;
        return (necessaryHeight += bottomPadding);
    }

    public AnimationCurve open;
    public AnimationCurve closed;
    public float animSpeedModifier;
    //open 1.0, close 0.0
    public IEnumerator animateCardOpen(AnimationCurve ac, float acThreshold, float animSpeedModifier, LayoutElement layout, float oldHeight, float newHeight)
    {
        float curveTime = 0f;
        float curveAmount = ac.Evaluate(curveTime);
        float step = 0f;
        float diffBetweenOldNew = newHeight - oldHeight;
        while (curveAmount < acThreshold)
        {
            curveTime += Time.fixedDeltaTime * animSpeedModifier;
            curveAmount = ac.Evaluate(curveTime);
            float calcStepHeight =  oldHeight+(diffBetweenOldNew * curveAmount);
            step = Mathf.Clamp(calcStepHeight, oldHeight, newHeight);
            bodyLayoutElement.preferredHeight = calcStepHeight-bannerRect.rect.height;

            layout.preferredHeight = step;
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutContentOwner);
            yield return new WaitForFixedUpdate();
        }
        layout.preferredHeight = newHeight;
    }
    public IEnumerator animateCardClose(AnimationCurve ac, float acThreshold, float animSpeedModifier, LayoutElement layout, float oldHeight, float newHeight)
    {
        float curveTime = 0f;
        float curveAmount = ac.Evaluate(curveTime);
        float step = 0f;
        float diffBetweenOldNew = oldHeight - newHeight;

        while (curveAmount > acThreshold)
        {
            curveTime += Time.fixedDeltaTime * animSpeedModifier;
            curveAmount = ac.Evaluate(curveTime);
            float calcStepHeight =  newHeight+(diffBetweenOldNew * curveAmount);
            step = Mathf.Clamp(calcStepHeight, newHeight, oldHeight);
            bodyLayoutElement.preferredHeight = calcStepHeight-bannerRect.rect.height;
            
            layout.preferredHeight = step;
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutContentOwner);
            yield return new WaitForFixedUpdate();
        }
        layout.preferredHeight = newHeight;
    }
}
