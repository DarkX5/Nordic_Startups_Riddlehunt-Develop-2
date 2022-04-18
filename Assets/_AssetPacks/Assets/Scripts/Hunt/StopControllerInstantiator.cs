using System;
using System.Collections;
using System.Collections.Generic;
using riddlehouse_libraries.products.AssetTypes;
using UnityEngine;

namespace Hunt
{
    public interface IStopControllerInstantiator
    {
        public IMap2DStopController CreateMap2D(Transform parent);
        public IMapBoxStopController CreateMapBox(Transform parent);
    }

    public class StopControllerInstantiator : MonoBehaviour, IStopControllerInstantiator
    {
        [SerializeField] private Map2DStopController map2DStopControllerPrefab;
        [SerializeField] private MapBoxStopController mapBoxStopControllerPrefab;

        public IMap2DStopController CreateMap2D(Transform parent)
        {
           return Map2DStopController.Factory(map2DStopControllerPrefab, parent);
        }

        public IMapBoxStopController CreateMapBox(Transform parent)
        {
            return MapBoxStopController.Factory(mapBoxStopControllerPrefab, parent);
        }
    }
}