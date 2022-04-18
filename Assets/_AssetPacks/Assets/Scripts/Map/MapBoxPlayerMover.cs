using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MapBoxPlayerMover : MonoBehaviour, IMapPlayerMover
{
    public static IMapPlayerMover Factory(MapBoxPlayerMover prefab, Transform parent, float scale)
    {
        MapBoxPlayerMover behaviour = Instantiate(prefab, parent);
        behaviour.Initialize();
        var thisTransform = behaviour.transform;
        thisTransform.localPosition = Vector3.zero;
        thisTransform.localScale = Vector3.one * scale;
        return behaviour;
    }

    public class Dependencies
    {
        public IGameObjectDestroyer God;
    }

    private void Initialize()
    {
        SetDependencies(
            new Dependencies()
            {
                God = gameObject.AddComponent<GameObjectDestroyer>()
            });
    }

    public Dependencies _dependencies { get; private set; }
    public void SetDependencies(Dependencies dependencies)
    {
        _dependencies = dependencies;
    }

    public void Configure(MapPlayerMover.Config config)
    {

    }

    public void PositionPlayer(Vector2 pos)
    {
        throw new System.NotImplementedException();
    }

    public Vector3 GetPosition()
    {
        return this.transform.position;
    }

    public void DetachPanGesture()
    {
    }

    public void DestroySelf()
    {
        _dependencies.God.Destroy();
    }
}
