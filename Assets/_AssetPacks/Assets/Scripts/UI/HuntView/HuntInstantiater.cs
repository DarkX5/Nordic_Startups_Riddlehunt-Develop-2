using System;
using Hunt;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using UnityEngine;
using UnityEngine.Serialization;


public interface IHuntInstantiater
{
    public IHuntView Create(ProductTypes productType, IProductEvents productEvents);
    public IProductController Create(ProductTypes productType, string productId);
}

public class HuntInstantiater : MonoBehaviour, IHuntInstantiater
{
    public IHuntView Create(ProductTypes productType, IProductEvents productEvents)
    {
        switch (productType)
        {
            case ProductTypes.ChristmasHunt: //timerobber was renamed as christmashunt.
                return CreateTimeRobberProduct(productEvents);
            default:
                throw new ArgumentException("no such product card type found.");
                break;
        }
    }

    public IProductController Create(ProductTypes productType, string productId)
    {
        return CreateHuntProduct(productId);
    }

    [SerializeField] private TimerobberProductBehaviour _timerobberProduct;
    private IHuntView CreateTimeRobberProduct(IProductEvents productEvents)
    {
        var product = TimerobberProductBehaviour.Factory(Instantiate(_timerobberProduct), productEvents, Camera.main);
        return (TimerobberProductBehaviour)product;
    }

    [FormerlySerializedAs("huntProductController")] [FormerlySerializedAs("huntProductStart")] [SerializeField] private ProductController productController;
    private IProductController CreateHuntProduct(string productId)
    {
        return ProductController.Factory(productController, productId);
    }
}
