using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MultipleChoice.Icon.Display
{
    public interface IIconDisplay
    {
        public void Configure(Sprite icon, Color background);
        public bool IsActive();
        public void Hide();
    }
    public class IconDisplay : MonoBehaviour, IIconDisplay
    {
        public static IIconDisplay Factory(IconDisplay prefab, RectTransform parent)
        {
            return Instantiate(prefab, parent);
        }
        
        public class Dependencies
        {
            public Image IconBackground  { get; set; }
            public Image IconImage { get; set; }
        }
        [SerializeField] private Image iconBackground;
        [SerializeField] private Image iconImage;

        public void Awake()
        {
            SetDependencies(new Dependencies()
            {
                IconBackground = iconBackground,
                IconImage = iconImage
            });
        }
        public Dependencies _dependencies { get; private set; }
        private void SetDependencies(Dependencies dependencies)
        {
            _dependencies = dependencies;
        }

        public void Configure(Sprite icon, Color background)
        {
            iconBackground.color = background;
            iconImage.sprite = icon;
            this.gameObject.SetActive(true);
        }

        public bool IsActive()
        {
            return this.gameObject.activeSelf;
        }
        
        public void Hide()
        {
            this.gameObject.SetActive(false);
        }
    }
}