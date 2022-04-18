using System;
using System.Collections;
using System.Collections.Generic;
using riddlehouse_libraries.products.models;
using riddlehouse_libraries.products.models.DTOs;
using UnityEngine;
public interface IProductCard
{
    public string GetProductType();
    public void SetParent(Transform parent);
    public void Configure(ProductBasicsDTO productData, Action cardReady);
    public void StartGame(IProductEvents productEvents, System.Action<bool> isReady, StartPanelData startPanelData);
}