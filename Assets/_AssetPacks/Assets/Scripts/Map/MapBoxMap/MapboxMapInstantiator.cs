using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMapboxMapInstantiator
{
    public IMapBoxMap CreateOrCollect(Transform parent);
}

public class MapboxMapInstantiator : MonoBehaviour, IMapboxMapInstantiator
{
    [SerializeField] private MapBoxMap prefab;

    private MapBoxMap _activeMap;
    public IMapBoxMap CreateOrCollect(Transform parent)
    {
        if (_activeMap == null)
            _activeMap = MapBoxMap.Factory(prefab, parent);
        return _activeMap;
    }
}
