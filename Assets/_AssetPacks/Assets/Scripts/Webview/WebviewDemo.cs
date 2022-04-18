using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebviewDemo : MonoBehaviour
{
    [SerializeField] private WebviewCanvas prefab;
    // Start is called before the first frame update
    void Start()
    {
        var webviewCanvas = WebviewCanvas.Factory(prefab);
        webviewCanvas.ConfigureAndDisplay(new WebviewCanvas.Config()
        {
            Url = "https://google.com"
        });
    }
}
