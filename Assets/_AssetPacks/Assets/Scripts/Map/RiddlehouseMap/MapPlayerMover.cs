using System;
using System.Collections;
using System.Collections.Generic;
using DigitalRubyShared;
using UnityEngine;

public interface IMapPlayerMover
{
    public void Configure(MapPlayerMover.Config config);
    public void PositionPlayer(Vector2 pos);
    public Vector3 GetPosition();
    public void DetachPanGesture();
    public void DestroySelf();
}
[RequireComponent(typeof(Rigidbody))]
public class MapPlayerMover : MonoBehaviour, IMapPlayerMover
{
    public static IMapPlayerMover Factory(MapPlayerMover prefab, Transform parent, float scale, MapCamNavOrientation orientation)
    {
        MapPlayerMover behaviour = Instantiate(prefab, parent);
        behaviour.Initialize(orientation);
        var thisTransform = behaviour.transform;
        thisTransform.localPosition = Vector3.zero;
        thisTransform.localScale = Vector3.one * scale;
        return behaviour;
    }
    public class Dependencies
    {
        public IMapPlayerMovementHelper MapPlayerMovementHelper { get; set; }
    }

    public class Config
    {
        public IPanGestureRecognizer PanGestureRecognizer { get; set; }
        public float MoveSpeed { get; set; }
    }

    public void Initialize(MapCamNavOrientation orientation)
    {
        var physics = GetComponent<Rigidbody>();
        var dependencies = new Dependencies()
        {
            MapPlayerMovementHelper = new MapPlayerMovementHelper(orientation, physics)
        };
        SetDependencies(dependencies);
    }

    public Dependencies _dependencies { get; private set; }
    public void SetDependencies(Dependencies dependencies)
    {
        _dependencies = dependencies;
    }
    private bool _subscribed = false;
    private Config _config;
    public void Configure(Config config)
    {
        _config = config;
        AttachPanGesture();
    }
    
    private void AttachPanGesture()
    {
        if (_config != null && !_subscribed)
        {
            _config.PanGestureRecognizer.SubscribeToStateUpdated(PanGestureCallback);
            _subscribed = true;
        }
    }

    public void DetachPanGesture()
    {
        if (_config != null && _subscribed)
        {
            _config.PanGestureRecognizer.UnsubscribeToStateUpdated(PanGestureCallback);
            _subscribed = false;
        }
    }

    public void OnDisable()
    {
        DetachPanGesture();
    }
    public void OnEnable()
    {
        AttachPanGesture();
    }

    private void PanGestureCallback(GestureRecognizer gesture)
    {
        _dependencies.MapPlayerMovementHelper.MovePlayer(_config.MoveSpeed, gesture);
    }

    public void PositionPlayer(Vector2 pos)
    {
        this.transform.position = pos;
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
