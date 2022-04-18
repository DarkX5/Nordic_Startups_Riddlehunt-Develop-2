using System;
using System.Collections;
using System.Collections.Generic;
using riddlehouse_libraries.Terms;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface ITermsAcceptanceController
{
    public void Configure(TermsAcceptanceController.Config config);
    public void Open(Action completed = null);
    public void Close(Action completed = null);
    public bool IsAnimating();
    public bool IsOpen();
}

[RequireComponent(typeof(ImageSliderFromBottomToTop))]
public class TermsAcceptanceController : MonoBehaviour, ITermsAcceptanceController
{
    public class Config
    {
        public Action ReadAgreementEvent { get; set; }
        public Action AcceptAgreementEvent { get; set; }
        public Action DeclineAgreementEvent  { get; set; }
        public string Title  { get; set; }
        public string Reason  { get; set; }
    }

    public class Dependencies
    { 
        public IImageSlider SlideComponent { get; set; }
        public TextMeshProUGUI TitleField { get; set; }
        public TextMeshProUGUI DescriptionField { get; set; }
        public ILayoutElementVerticalResizer VerticalResizer { get; set; }
    }

    public void Awake()
    {
        SetDependencies(new Dependencies()
        {
            SlideComponent = slideComponent,
            TitleField = titleField,
            DescriptionField = descriptionField,
            VerticalResizer = layoutElementVerticalResizer
        });
    }
    public Dependencies _dependencies { get; private set; }
    public void SetDependencies(Dependencies dependencies)
    {
        _dependencies = dependencies;
    }

    [SerializeField] private ImageSliderFromBottomToTop slideComponent;
    [SerializeField] private TextMeshProUGUI titleField;
    [SerializeField] private TextMeshProUGUI descriptionField;
    [SerializeField] private LayoutElementVerticalResizer layoutElementVerticalResizer;
    private Config _config;
    
    public void Configure(Config config)
    {
        if(_dependencies.SlideComponent == null)
            _dependencies.SlideComponent = GetComponent<ImageSliderFromBottomToTop>();
        _config = config;

        _dependencies.TitleField.text = config.Title;
        _dependencies.DescriptionField.text = config.Reason;
        _dependencies.VerticalResizer.StartUIUpdate();
    }

    public void DeclineAgreement()
    {
        _config.DeclineAgreementEvent.Invoke();
    }

    public void AcceptAgreement()
    {
        _config.AcceptAgreementEvent.Invoke();
    }

    public void ReadAgreement()
    {
        _config.ReadAgreementEvent.Invoke();
    }
    
    public void Open(Action completed = null)
    {
        _dependencies.SlideComponent.Open(completed);
    }

    public void Close(Action completed = null)
    {
        _dependencies.SlideComponent.Close(completed);
    }
    
    public bool IsAnimating()
    {
        return _dependencies.SlideComponent.IsAnimating();
    }

    public bool IsOpen()
    {
        return _dependencies.SlideComponent.IsOpen();
    }
}
