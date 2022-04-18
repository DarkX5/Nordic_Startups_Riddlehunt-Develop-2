using riddlehouse_libraries.products.models;
using riddlehouse_libraries.products.models.DTOs;
using UnityEngine;

public interface ICardActions
{
    public void Configure(ProductBasicsDTO cardData, Sprite collectedSprite, System.Action readyForDisplay);
    public void StartGame(IProductEvents productEvents, System.Action<bool> isReady, StartPanelData startPanelData = null);
}