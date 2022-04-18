using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using riddlehouse_libraries.products.Assets.Map2DAssets;
using riddlehouse_libraries.products.huntProduct;
using riddlehouse_libraries.products.resources;
using riddlehouse_libraries.products.resources.Map;
using UnityEngine;

public class POIDemo : MonoBehaviour
{
    [SerializeField] private POIController prefab;
    public void Start()
    {   
        var client = new HttpClient();
        var persistentDataPath = DataPathHelper.PersistentDataPath;
        var cacheConfig = new HuntProductCacheConfig(client, persistentDataPath);
        
        var a = new POIController.Config()
        {
            ClickedAction = Clicked,
            Resource = new POI2DResource(new Icon(cacheConfig, new IconDto()
            {
                Url = "https://cdn.escaperoom-riddlehouse.dk/riddlehunt/ProductResourceService/book.png"
            }))
            {
                ResourceId = "demoA",
                BackgroundColor = new RHColor(255, 255, 255, 255),
                FrameColor = new RHColor(0, 0, 0, 255),
                IconColor = new RHColor(33, 33, 33, 255),
                PixelsPerUnit = 5
            },
            PoiAsset = new POI2DListAsset.Poi2DAsset()
            {
                Id = "A",
                ActionId = "start",
                RealWorldPosition = new Map2DPosition(55.55,12.12),
                ResourceId = "demoA"
            }
        };
        
        // var b = new POIController.Config()
        // {
        //     ClickedAction = Clicked,
        //     Resource = new POI2DResource(new Icon(cacheConfig, new IconDto()
        //     {
        //         Url = "https://cdn.escaperoom-riddlehouse.dk/riddlehunt/ProductResourceService/sword.png"
        //     }))
        //     {
        //         BackgroundColor = new Color(255, 255, 255, 255),
        //         FrameColor = new Color(0, 0, 0, 255),
        //         IconColor = new Color(33, 33, 33, 255),
        //         PixelsPerUnit = 5
        //     }
        // };
        var poi_a = POIController.Factory(prefab, null);

        poi_a.Configure(a);
        
        // var poi_b = POIController.Factory(prefab, null);
        // poi_b.Configure(b);
    }

    public void Clicked(string id)
    {
        Debug.Log("Clicked: "+id);
    }
}
