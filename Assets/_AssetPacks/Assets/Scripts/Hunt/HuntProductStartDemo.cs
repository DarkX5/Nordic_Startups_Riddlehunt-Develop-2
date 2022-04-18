using System.Collections;
using System.Collections.Generic;
using Hunt;
using riddlehouse_libraries.products;
using UnityEngine;

public class HuntProductStartDemo : MonoBehaviour
{
    [SerializeField] private ProductController prefab;
    private IProductController _productStartController;
    
    // Start is called before the first frame update
    void Start()
    {
        var productId = "productID";
        _productStartController = ProductController.Factory(prefab, productId);
        _productStartController.Configure(new ProductController.Config()
        {
        });
    }
}
