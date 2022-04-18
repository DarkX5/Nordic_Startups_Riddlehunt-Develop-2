using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMapCameraCenterPoint
{
    public Vector3 GetPosition();
    public void DestroySelf();
}

public class MapCameraCenterPoint : MonoBehaviour, IMapCameraCenterPoint
{
    public static IMapCameraCenterPoint Factory(MapCameraCenterPoint prefab, Transform parent)
    {
        MapCameraCenterPoint behaviour = Instantiate(prefab, parent);
        behaviour.SetToCenterAndScale();
        return behaviour;
    }

    public void SetToCenterAndScale()
    {
        var thisTransform = transform;
        thisTransform.localPosition = Vector3.zero;
        thisTransform.localScale = Vector3.one;
    }
    public Vector3 GetPosition()
    {
        return this.transform.position;
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
