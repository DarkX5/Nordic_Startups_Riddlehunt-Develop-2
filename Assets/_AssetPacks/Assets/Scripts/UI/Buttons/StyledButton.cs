using System;
using System.Collections;
using System.Collections.Generic;
using riddlehouse_libraries.products.resources;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

namespace Components.Buttons { public interface IStyledButton
{
    public void Configure(StyledButton.Config config);
    public void SetInteractable(bool interactable);
    public void PerformAction();
}
public class StyledButton : MonoBehaviour,IStyledButton
{
    [SerializeField] private Button button;
    [SerializeField] private Image buttonBackground;
    [SerializeField] private TextMeshProUGUI buttonText;
    
    public class Dependencies
    {
        public Button Button { get; set; }
        public Image ButtonBackground { get; set; }
        public TextMeshProUGUI ButtonText { get; set; }
    }

    public class Config
    {
        public StyledButtonComponentResource Resource { get; set; }
        public Action Action  { get; set; }
    }

    public void Initialize()
    {
        SetDependencies(new Dependencies()
        {
            Button = button,
            ButtonBackground = buttonBackground,
            ButtonText = buttonText
        });
    }

    public Dependencies _dependencies { get; private set; }
    public void SetDependencies(Dependencies dependencies)
    {
        _dependencies = dependencies;
    }

    private Config _config;
    public void Configure(Config config)
    {
        _config = config;
        _dependencies.ButtonBackground.color = new Color32(_config.Resource.ButtonColor.R, _config.Resource.ButtonColor.G, _config.Resource.ButtonColor.B, _config.Resource.ButtonColor.A);
        _dependencies.ButtonText.text = config.Resource.ButtonText;
        _dependencies.ButtonText.color = new Color32(_config.Resource.ButtonTextColor.R, _config.Resource.ButtonTextColor.G, _config.Resource.ButtonTextColor.B, _config.Resource.ButtonTextColor.A);;
    }

    public void SetInteractable(bool interactable)
    {
        _dependencies.Button.interactable = interactable;
    }

    public void PerformAction()
    {
        _config.Action.Invoke();
    }
}
}