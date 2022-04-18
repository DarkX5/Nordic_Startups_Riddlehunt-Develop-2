using System.Collections;
using System.Collections.Generic;
using riddlehouse_libraries.products.resources;
using UnityEngine;
using Color = UnityEngine.Color;

public interface ICharacterIconDisplay
{
    public void Configure(Color frameColor, Sprite icon);
}
public class CharacterIconDisplay : MonoBehaviour, ICharacterIconDisplay
{
    public class Dependencies
    {
        public ChangeImageSpriteComponent SpriteUpdater;
        public SetImageColorComponent ColorUpdater;
    }

    [SerializeField] private ChangeImageSpriteComponent iconImage;
    [SerializeField] private SetImageColorComponent frameImage;
    public void Initialize()
    {
        SetDependencies(new Dependencies()
        {
            SpriteUpdater = iconImage,
            ColorUpdater = frameImage
        });
    }
    
    public Dependencies _dependencies { get; private set; }

    public void SetDependencies(Dependencies dependencies)
    {
        _dependencies = dependencies;
    }
    
    public void Configure(Color frameColor, Sprite icon) {
        _dependencies.SpriteUpdater.Configure(icon);
        _dependencies.ColorUpdater.Configure(frameColor);
    }
}
