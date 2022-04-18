using System;
using RHPackages.Core.Scripts;
using RHPackages.Core.Scripts.UI;
using Riddlehouse.Core.Helpers.Helpers;
using UnityEngine;
using UnityEngine.UI;

public interface IRatingComponent
{
    public void PerformAction();
    public IViewActions GetComponentUIActions();
    public void Configure(Action<int> btnAction);
}

public class RatingComponent : IRatingComponent
{
    private readonly IViewActions _viewActions;
    private readonly IRatingComponentComponentActions _ratingComponentActions;
    private Action<int> _btnAction;

    public static IRatingComponent Factory(GameObject go)
    {
        var behaviour = new ComponentHelper<RatingComponentBehaviour>().GetBehaviourIfExists(go);
        var component = new RatingComponent(behaviour, behaviour);
        behaviour.SetLogicInstanceOfComponent(component);
        return component;
    }

    public RatingComponent(IRatingComponentComponentActions ratingComponentComponentActions,
        IViewActions viewActions)
    {
        _ratingComponentActions = ratingComponentComponentActions;
        _viewActions = viewActions;
    }

    public void PerformAction()
    {
        if (_ratingComponentActions.SliderHasBeenDragged())
        {
            // User submitted a rating
            _btnAction.Invoke((int)_ratingComponentActions.GetRating());
        }
        else
        {
            // No ratings
            _btnAction.Invoke(-1);
        }
    }

    public IViewActions GetComponentUIActions()
    {
        return _viewActions;
    }

    public void Configure(Action<int> btnAction)
    {
        _btnAction = btnAction;
        _ratingComponentActions.Configure(2);
    }
}

public interface IRatingComponentComponentActions
{
    void PerformAction();
    void Configure(float value);
    float GetRating();
    bool SliderHasBeenDragged();
}

public class RatingComponentBehaviour : MonoBehaviour, IRatingComponentComponentActions, IViewActions
{
    [SerializeField] private Image image;
    [SerializeField] private Slider serializedSlider;
    [SerializeField] private Sprite smileyHappy;
    [SerializeField] private Sprite smileySmile;
    [SerializeField] private Sprite smileyMeh;
    [SerializeField] private Sprite smileySad;
    [SerializeField] private Sprite smileyAngry;

    private Sprite[] _smileys;
    private ITestable<Slider> _testableSlider;
    private IRatingComponent _ratingComponent;
    private readonly ComponentType _componentType;
    
    //used in Unity Event System
    public bool sliderHasBeenDragged { get; set; }

    public RatingComponentBehaviour()
    {
        _componentType = ComponentType.Rating;
    }

    public void Awake()
    {
        SetDependencies(serializedSlider);
    }

    public void Start()
    {
        sliderHasBeenDragged = false;
        _smileys = new Sprite[5];
        _smileys[0] = smileyAngry;
        _smileys[1] = smileySad;
        _smileys[2] = smileyMeh;
        _smileys[3] = smileySmile;
        _smileys[4] = smileyHappy;
        
        _testableSlider.Get().onValueChanged.AddListener(delegate { SetSmileyFromSliderValue(); });
        SetSmileyFromSliderValue();
    }
    
    public void SetDependencies(Slider slider)
    {
        _testableSlider = new TestableComponent<Slider>(slider);
    }

    public void Configure(float rating)
    {
        _testableSlider.Get().value = rating;
    }

    public void FitInView(RectTransform parent, IUIFitters uiFitters)
    {
        uiFitters.FitToFullscreen(GetRectTransform(), parent);
    }

    public void FitInView(RectTransform parent, IUIFitters uiFitters, int index)
    {
        throw new NotImplementedException();
    }

    public void Display()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }

    public bool IsShown()
    {
        return this.gameObject.activeSelf;
    }

    public ComponentType GetComponentType()
    {
        return _componentType;
    }

    public RectTransform GetRectTransform()
    {
        return (RectTransform) this.transform;
    }

    public void PerformAction()
    {
        _ratingComponent.PerformAction();
    }

    public void SetLogicInstanceOfComponent(IRatingComponent ratingComponent)
    {
        _ratingComponent = new ComponentHelper<IRatingComponent>().SetLogicInstance(ratingComponent, _ratingComponent);
    }

    public float GetRating()
    {
        return _testableSlider.Get().value;
    }

    public bool SliderHasBeenDragged()
    {
        return sliderHasBeenDragged;
    }

    public IRatingComponent GetRatingComponent()
    {
        return _ratingComponent;
    }

    private void SetSmileyFromSliderValue()
    {
        var sliderValue = Convert.ToInt16(Math.Floor(_testableSlider.Get().value));       //Convert throws exceptions where a cast does not
        sliderValue = sliderValue < 4 ? sliderValue : (short) 4;
        image.sprite = _smileys[sliderValue];
    }
    
}