using System;
using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using riddlehouse_libraries.products.Assets.Adressables;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.Stops;
using UnityEngine;

public interface IMapBoxPOIPlacement
{
    public void CreateAndConfigure(POIController.Config config, float spawnScale);
    public void UpdatePoiPositions();
    public void UpdatePOIState(string poiId, PoiStates state);
    public void DestroyAllPois();
}
public class POIPlacementData
{
    public IPOIController Controller;
    public string Location;
}
public class MapBoxPOIPlacement :IMapBoxPOIPlacement
{
    private AbstractMap _map;
    private Dictionary<string,POIPlacementData> _spawnedObjects;
    private float raisedHeight = 1f;
    private Transform _parent;
    private IPOIInstantiator _poiInstantiator;
    public MapBoxPOIPlacement(IPOIInstantiator poiInstantiator, AbstractMap map, Transform parent)
    {
        _poiInstantiator = poiInstantiator;
        _parent = parent;
        _map = map;
        _spawnedObjects = new Dictionary<string,POIPlacementData>();
    }

    public async void CreateAndConfigure(POIController.Config config, float spawnScale)
    {
        if (_spawnedObjects.ContainsKey(config.PoiAsset.Id))
            return;
        var poi = await _poiInstantiator.Create(config.Resource.PoiPrefab);
        var location = config.PoiAsset.RealWorldPosition.StringCoord();
        _spawnedObjects.Add(config.PoiAsset.Id, new POIPlacementData()
        {
            Controller = poi,
            Location = location
        });
        poi.Configure(config);
        PlaceMapBoxPoi(config.PoiAsset.Id, spawnScale, location);
    }
    
    private void PlaceMapBoxPoi(string id, float spawnScale, string location)
    {
        var poi = _spawnedObjects[id].Controller;
        var position = Conversions.StringToLatLon(location);
        var localPosition = _map.GeoToWorldPosition(position);
        localPosition += (Vector3.up*raisedHeight);
        poi.SetLocalPosition(localPosition, _parent);
        poi.SetLocalScale(new Vector3(spawnScale, spawnScale, spawnScale));
        poi.SetLocalRotation(new Vector3(90,0,0));
    }

    public void UpdatePoiPositions()
    {
        foreach (var id in _spawnedObjects.Keys)
        {
            var position = Conversions.StringToLatLon(_spawnedObjects[id].Location);
            var localPosition = _map.GeoToWorldPosition(position);
            localPosition += (Vector3.up*raisedHeight);
            _spawnedObjects[id].Controller.SetLocalPosition(localPosition);
        }
    }

    public void UpdatePOIState(string poiId, PoiStates state)
    {
        if(_spawnedObjects.ContainsKey(poiId))
            _spawnedObjects[poiId].Controller.UpdatePOIState(state);
    }

    public void DestroyAllPois()
    {
        foreach (var id in _spawnedObjects.Keys)
        {
            _spawnedObjects[id].Controller.DestroySelf();   
        }
    }
}
