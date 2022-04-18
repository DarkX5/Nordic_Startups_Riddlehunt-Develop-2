using System;
using System.Collections;
using System.Collections.Generic;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using riddlehouse_libraries.products.models.DTOs;
using UnityEngine;
using UnityEngine.Serialization;

public interface IProductCardInstantiater
{
    public IProductCard Create(ProductBasicsDTO productData, RectTransform parent, IProductEvents productEvents);
}

public class ProductCardInstantiater: MonoBehaviour, IProductCardInstantiater
{
    public IProductCard Create(ProductBasicsDTO productData, RectTransform parent, IProductEvents productEvents)
    {
        switch (productData.ProductCardStyle)
        {
            case ProductCardStyles.Hunt:
                return CreateHuntProductCard(productData, parent, productEvents);
            case ProductCardStyles.ChristmasHunt:
                return CreateTimeRobberProductCard(productData, parent, productEvents);
        }
        return CreateDefaultStyle(productData, parent, productEvents);
    }
    
    [SerializeField] TimeRobberProductCardBehaviour defaultStyle;
    private IProductCard CreateDefaultStyle(ProductBasicsDTO productData,  RectTransform parent, IProductEvents productEvents)
    {
        return TimeRobberProductCard.Factory(Instantiate(defaultStyle), parent, productEvents);
    }
    
    [FormerlySerializedAs("_timeRobberProductCardBehaviour")] [SerializeField] private TimeRobberProductCardBehaviour timeRobberProductCardBehaviour;
    private IProductCard CreateTimeRobberProductCard(ProductBasicsDTO productData,  RectTransform parent, IProductEvents productEvents)
    {
        return TimeRobberProductCard.Factory(Instantiate(timeRobberProductCardBehaviour), parent, productEvents);
    }
    
    [FormerlySerializedAs("HuntCardPrefab")] [SerializeField] private ProductCardBehavior huntCardPrefab;
    private IProductCard CreateHuntProductCard(ProductBasicsDTO productData,  RectTransform parent, IProductEvents productEvents) //second parameter is temporary
    {
        return ProductCardBehavior.Factory(huntCardPrefab, parent, productEvents);
    }
}
