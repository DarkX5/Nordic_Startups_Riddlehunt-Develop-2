using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public interface IMapCamControllerInstantiator
    {
        public IMapCamController CreateXY();
        public IMapCamController CreateXZ();
    }

    public class MapCamControllerInstantiator : IMapCamControllerInstantiator
    {
        private MapCamController Prefab;
        private Transform Parent;
        public MapCamControllerInstantiator(MapCamController prefab, Transform parent)
        {
            Prefab = prefab;
            Parent = parent;
        }

        public IMapCamController CreateXY()
        {
            return MapCamController.Factory(Prefab, Parent, MapCamNavOrientation.XY);
        }
        public IMapCamController CreateXZ()
        {
            return MapCamController.Factory(Prefab, Parent, MapCamNavOrientation.XZ);
        }
    }
}