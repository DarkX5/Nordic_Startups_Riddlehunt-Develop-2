using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using riddlehouse_libraries.products.Assets.Map2DAssets;
using riddlehouse_libraries.products.huntProduct;
using riddlehouse_libraries.products.resources;
using riddlehouse_libraries.products.resources.Map;
using UnityEngine;

public class MapBoxMapDemo : MonoBehaviour
{
   [SerializeField] private MapBoxMap mapBoxMapPrefab;
    // Start is called before the first frame update
    void Start()
    {
        var mapBox = MapBoxMap.Factory(mapBoxMapPrefab, this.transform);
        var resource = new MapCameraResource()
        {
            ZoomSpeed = 15f,
            StartZoom = 30f,
            MaxZoom = 60f,
            MinZoom = 12f,
            CameraMoveSpeed = 12f,
            PlayerMoveSpeed = 3f 
        };

        var mapCoord = new Map2DPosition(55.664582974714854, 12.388976612470556);
        mapBox.Configure(new MapBoxMap.Config()
        {
            Resource = resource,
            MapCoordinate = mapCoord
        });

        var client = new HttpClient();
        var persistentDataPath = DataPathHelper.PersistentDataPath;
        var cacheConfig = new HuntProductCacheConfig(client, persistentDataPath);
        //spawn scale 5
        var poiData1 = new POI2DListAsset.Poi2DAsset()
        {
            Id = "id1",
            RealWorldPosition = new Map2DPosition(55.664582974714854, 12.388976612470556),
            ResourceId = "sword"
        };
        
        var poiData2 = new POI2DListAsset.Poi2DAsset()
        {
            Id = "id2",
            RealWorldPosition = new Map2DPosition(55.66443773950198, 12.379427948262077),
            ResourceId = "map"
        };
        
        var poiData3 = new POI2DListAsset.Poi2DAsset()
        {
            Id = "id3",
            RealWorldPosition = new Map2DPosition(55.66332425163186, 12.398782768714112),
            ResourceId = "book"
        };
        
        var poiData4 = new POI2DListAsset.Poi2DAsset()
        {
            Id = "id4",
            RealWorldPosition = new Map2DPosition(55.66528493731333, 12.388858595239004),
            ResourceId = "stop"
        };

        var poiSword = new POI2DResource(new Icon(cacheConfig, new IconDto(){
                Url = "https://cdn.escaperoom-riddlehouse.dk/riddlehunt/ProductResourceService/sword.png"
            })
        )
        {
            BackgroundColor = new RHColor(255, 255, 255, 255),
            FrameColor = new RHColor(0, 0, 0, 255),
            IconColor = new RHColor(33, 33, 33, 255),
            PixelsPerUnit = 5,
            ResourceId = "sword"
        };
        var poiBook = new POI2DResource(new Icon(cacheConfig, new IconDto(){
                Url = "https://cdn.escaperoom-riddlehouse.dk/riddlehunt/ProductResourceService/book.png"
            })
        )
        {
            BackgroundColor = new RHColor(255, 255, 255, 255),
            FrameColor = new RHColor(0, 189, 0, 255),
            IconColor = new RHColor(33, 33, 33, 255),
            PixelsPerUnit = 5,
            ResourceId = "book"
        };
        var poiMap = new POI2DResource(new Icon(cacheConfig,new IconDto(){
                Url = "https://cdn.escaperoom-riddlehouse.dk/riddlehunt/ProductResourceService/map.png"
            })
        )
        {
            BackgroundColor = new RHColor(255, 255, 255, 255),
            FrameColor = new RHColor(0, 0, 189, 255),
            IconColor = new RHColor(33, 33, 33, 255),
            PixelsPerUnit = 5,
            ResourceId = "map"
        };

        var poiStop = new POI2DResource(new Icon(cacheConfig,new IconDto(){
                Url = "https://cdn.escaperoom-riddlehouse.dk/riddlehunt/ProductResourceService/stopIcon.png"
            })
        )
        {
            BackgroundColor = new RHColor(209, 189, 61, 255),
            FrameColor = new RHColor(0, 0, 0, 255),
            IconColor = new RHColor(33, 33, 33, 255),
            PixelsPerUnit = 5,
            ResourceId = "stop"
        };
        mapBox.CreatePositionAndConfigurePoi(poiData1, poiSword, ButtonClick);
        mapBox.CreatePositionAndConfigurePoi(poiData2, poiBook, ButtonClick);
        mapBox.CreatePositionAndConfigurePoi(poiData3, poiMap, ButtonClick);
        mapBox.CreatePositionAndConfigurePoi(poiData4, poiStop, ButtonClick);
    }

    private void ButtonClick(string id)
    {
        Debug.Log(id);
    }
}
