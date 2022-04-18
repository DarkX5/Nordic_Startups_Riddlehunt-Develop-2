using System.Collections;
using System.Collections.Generic;
using Answers.MultipleChoice.Data.Icon;
using UnityEngine;

namespace MultipleChoice.Icon.Display
{
    public interface IIconDisplayInstantiater
    {
        public IIconDisplay CreateButton();
    }

    public class IconDisplayInstantiater : IIconDisplayInstantiater
    {
        private IconDisplay _prefab;
        private RectTransform _content;
        public IconDisplayInstantiater(IconDisplay prefab, RectTransform content)
        {
            _prefab = prefab;
            _content = content;
        }

        public IIconDisplay CreateButton()
        {
            return IconDisplay.Factory(_prefab, _content);
        }
    }
    
    public interface IMultipleChoiceIconDisplay
    {
        public void AddToDisplay(Sprite icon, Color background);
        public void Clear();
    }
    public class MultipleChoiceIconDisplay : MonoBehaviour, IMultipleChoiceIconDisplay
    {
        [SerializeField] private IconDisplay IconDisplayPrefab;
        private List<IIconDisplay> _icons;

        public class Dependencies
        {
            public IIconDisplayInstantiater IconDisplayInstantiater { get; set; }
        } 
        
        public void Awake()
        {
            SetDependencies(new Dependencies()
            {
                IconDisplayInstantiater = new IconDisplayInstantiater(IconDisplayPrefab, (RectTransform)this.transform)
            });
        }
        public Dependencies _dependencies { get; private set; }
        public void SetDependencies(Dependencies dependencies)
        {
            _icons = new List<IIconDisplay>();
            _dependencies = dependencies;
        }

        public void AddToDisplay(Sprite icon, Color background)
        {
            var idx = _icons.FindIndex(x => x.IsActive() == false);
            if (idx == -1)
            {
                idx = _icons.Count;
                _icons.Add(_dependencies.IconDisplayInstantiater.CreateButton());
            }
            _icons[idx].Configure(icon, background);
        }

        public void Clear()
        {
            foreach (var icon in _icons)
            {
                icon.Hide();
            }
        }
    }
}