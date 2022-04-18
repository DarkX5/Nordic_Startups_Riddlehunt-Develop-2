using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using riddlehouse_libraries.products;
using riddlehouse_libraries.products.Assets.Adressables;
using riddlehouse_libraries.products.Assets.Map2DAssets;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.huntProduct;
using riddlehouse_libraries.products.models;
using riddlehouse_libraries.products.resources;
using riddlehouse_libraries.products.resources.Map;
using riddlehouse_libraries.products.Stops;
using UnityEngine;
using UnityEngine.Serialization;

public class Map2DDemo : MonoBehaviour
{
    [FormerlySerializedAs("mapPrefab")] [SerializeField] private Map2D prefab;

    [SerializeField] private Sprite mapImg;
    
    [SerializeField] private POIController poiPrefab;

    private IMap2D _map;
    // Start is called before the first frame update
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        var parentObj = new GameObject();
        _map = Map2D.Factory(prefab, parentObj.transform);

        var Map2DRectResource = new Map2DRectAsset(new Map2DPosition(55.56288825131802, 12.197588969073888),
            new Map2DPosition(55.66288825131802, 12.297588969073888));
        var client = new HttpClient();
        var persistentDataPath = DataPathHelper.PersistentDataPath;
        var cacheConfig = new HuntProductCacheConfig(client, persistentDataPath);
        
        var poiResource = new List<POI2DResource>()
        {
            new POI2DResource(new Icon(cacheConfig, new IconDto()
            {
                Url = "https://cdn.escaperoom-riddlehouse.dk/riddlehunt/ProductResourceService/sword.png"
            }))
            {
                ResourceId = "sword",
                BackgroundColor = new RHColor(255, 255, 255, 255),
                FrameColor = new RHColor(0, 0, 0, 255),
                IconColor = new RHColor(33, 33, 33, 255),
                PixelsPerUnit = 5,
            },
            new POI2DResource(new Icon(cacheConfig, new IconDto()
            {
                Url = "https://cdn.escaperoom-riddlehouse.dk/riddlehunt/ProductResourceService/map.png"
            }))
            {
                ResourceId = "map",
                BackgroundColor = new RHColor(255, 255, 255, 255),
                FrameColor = new RHColor(0, 0, 0, 255),
                IconColor = new RHColor(33, 33, 33, 255),
                PixelsPerUnit = 5,
            },
            new POI2DResource(new Icon(cacheConfig, new IconDto()
            {
                Url = "https://cdn.escaperoom-riddlehouse.dk/riddlehunt/ProductResourceService/book.png"
            }))
            {
                ResourceId = "book",
                BackgroundColor = new RHColor(255, 255, 255, 255),
                FrameColor = new RHColor(0, 0, 0, 255),
                IconColor = new RHColor(33, 33, 33, 255),
                PixelsPerUnit = 5,
            }
        };

        
        var poiList = new List<POI2DListAsset.Poi2DAsset>();
        var poi_sword = new POI2DListAsset.Poi2DAsset()
        {
            ResourceId = "sword",
            RealWorldPosition = new Map2DPosition(55.56288825131802, 12.197588969073888),
            Id = "",
            ActionId = ""
        };
        poiList.Add(poi_sword);
        var poi_map = new POI2DListAsset.Poi2DAsset()
        {
            ResourceId = "map",
            RealWorldPosition = new Map2DPosition(55.66288825131802, 12.197588969073888),
            Id = "",
            ActionId = ""
        };
        poiList.Add(poi_map);
        var poi_book = new POI2DListAsset.Poi2DAsset()
        {
            ResourceId = "book",
            RealWorldPosition = new Map2DPosition(55.61288925131802, 12.2475889607388),
            Id = "",
            ActionId = ""
        };

        poiList.Add(poi_book);
        var poiData = new POI2DListAsset(poiList);
            
        var map2DStopData = new Map2DStop(
            new Riddlehouse2DMapResourceService(
            new MapResource()
                { 
                    MapCameraResource = new MapCameraResource() {
                        ZoomSpeed = 7f,
                        StartZoom = 8f,
                        MaxZoom = 30f,
                        MinZoom = 5f,
                        CameraMoveSpeed = 7f,
                        PlayerMoveSpeed = 3f
                    },
                   
                    MapCanvasResource = new MapCanvasResource()
                    {
                        MapCanvasPrefab = new AddressableWithTag("GameCanvas","Assets/Prefabs/Hunt/MapCanvas.prefab")
                    }
                }
            ),
            Map2DRectResource,
            poiData,
            new Map2DImageAsset(mapImg.texture.GetRawTextureData()),
            "title",
            "id",
            StopType.MapStop2D,
            null,
            null,
            null);
        
        _map.Configure(new Map2D.Config()
        {
            Resource = map2DStopData,
        });
    }
}