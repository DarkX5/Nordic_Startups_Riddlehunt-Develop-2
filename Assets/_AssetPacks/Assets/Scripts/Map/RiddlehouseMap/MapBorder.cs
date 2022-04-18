using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum MapBorderDirection
{
    North, South, East, West
}

public interface IMapBorder
{
    public MapBorderDirection GetBorderDirection();
}
[RequireComponent(typeof(BoxCollider2D))]
public class MapBorder : MonoBehaviour, IMapBorder
{
    public class Dependencies
    {
        public SpriteRenderer Map { get; set; }
        public BoxCollider2D BoxCollider2D { get; set; }
    }
    public static IMapBorder Factory(MapBorder prefab, Transform parent, MapBorderDirection direction, Dependencies dependencies)
    {
        var behaviour = Instantiate(prefab, parent);
        behaviour.name = direction.ToString();
        behaviour.Initialize(dependencies);
        behaviour.Configure(direction);
        return behaviour;
    }

    public void Initialize(Dependencies dependencies)
    {
        dependencies.BoxCollider2D = this.GetComponent<BoxCollider2D>();
        SetDependencies(dependencies);
    }

    public Dependencies _dependencies { get; private set; }
    public void SetDependencies(Dependencies dependencies)
    {
        _dependencies = dependencies;
    }
    
    private void OnEnable()
    {
        if(_dependencies != null)
            AdjustAndAutoPosition();
    }
    
    public MapBorderDirection BorderDirection { get; private set; }
    public void Configure(MapBorderDirection direction)
    {
        BorderDirection = direction;
        AdjustAndAutoPosition();
    }

    private void AdjustAndAutoPosition()
    {
        switch (BorderDirection)
        {
            case MapBorderDirection.North:
                AdjustAndPositionYHorizontalBorder(false);
                break;
            case MapBorderDirection.South:
                AdjustAndPositionYHorizontalBorder(true);
                break;
            case MapBorderDirection.East:
                AdjustAndPositionXVerticalBorder(false);
                break;
            case MapBorderDirection.West:
                AdjustAndPositionXVerticalBorder(true);
                break;
        }
    }
    
    private void AdjustAndPositionYHorizontalBorder(bool inverse)
    {
        var topPosition = (_dependencies.Map.sprite.bounds.size.y / 2) + (this.transform.localScale.y / 2);
        if (inverse)
            topPosition *= -1;
        this.transform.localPosition = new Vector3(0, topPosition, 0f);
        _dependencies.BoxCollider2D.size = new Vector2(_dependencies.Map.sprite.bounds.size.x, 1);
    }
    
    private void AdjustAndPositionXVerticalBorder(bool inverse)
    {
        var rightPosition = (_dependencies.Map.sprite.bounds.size.x / 2) + (this.transform.localScale.x / 2);
        if (inverse)
            rightPosition *= -1;
        this.transform.localPosition = new Vector3(rightPosition, 0, 0f);
        _dependencies.BoxCollider2D.size = new Vector2(1, _dependencies.Map.sprite.bounds.size.y);
    }

    public MapBorderDirection GetBorderDirection()
    {
        return BorderDirection;
    }
}
