using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using riddlehouse_libraries.products.AssetTypes;
using UnityEngine;

public interface IMap2DPOIPlacement
{
    public Task<IPOIController> CreateAndConfigure(POIController.Config config, float spawnScale);
    public void UpdatePOIState(string poiId, PoiStates state);
    public void DestroyAllPois();
}

public class Map2DPOIPlacement : IMap2DPOIPlacement
{

    private Dictionary<string,POIPlacementData> _spawnedObjects;

    private IPOIInstantiator _poiInstantiator;
    public Map2DPOIPlacement(IPOIInstantiator poiInstantiator)
    {
        _poiInstantiator = poiInstantiator;
        _spawnedObjects = new Dictionary<string,POIPlacementData>();
    }

    public async Task<IPOIController> CreateAndConfigure(POIController.Config config, float spawnScale)
    {
        if (_spawnedObjects.ContainsKey(config.PoiAsset.Id))
            return _spawnedObjects[config.PoiAsset.Id].Controller;
        var poi = await _poiInstantiator.Create(config.Resource.PoiPrefab);
        var location = config.PoiAsset.RealWorldPosition.StringCoord();
        _spawnedObjects.Add(config.PoiAsset.Id, new POIPlacementData()
        {
            Controller = poi,
            Location = location
        });
        poi.Configure(config);
        return poi;
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
        _spawnedObjects.Clear();
    }
}
