using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductIntroViewDemo : MonoBehaviour
{
    [SerializeField] private ProductIntroView prefab;

    private IProductIntroView _introView;
    void Start()
    {
        _introView = ProductIntroView.Factory(prefab, this.transform);
        _introView.Configure(new ProductIntroView.Config()
        {
            Url = "https://www.google.com",
            BackAction = GoBack,
            ContinueAction = GoForwards
        });
        _introView.Display();
    }
    private void GoBack()
    {
        Debug.Log("going backwards");
    }
    private void GoForwards()
    {
        Debug.Log("going forwards");
    }
}
