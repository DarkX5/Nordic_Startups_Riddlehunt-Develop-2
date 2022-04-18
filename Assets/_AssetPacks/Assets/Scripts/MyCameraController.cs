using UnityEngine;
using Zenject;

public class MyCameraController : ICameraController
{
    private Transform Transform;
    public Vector3 Rotation { get; set; }
    public Vector3 Position { get; set; }
    private float Height;

    public MyCameraController(Transform transform, float height)
    {
        Transform = transform;
        Height = height;
    }
    public void Follow(Vector3 position)
    {
        Position = new Vector3(position.x, Height, position.z);
        Transform.position = Position;
        Transform.LookAt(position);
    }
}