// CameraController interface

using UnityEngine;

public interface ICameraController
{
    void Follow(Vector3 position);
    public Vector3 Position { get; set; }
}

public interface ITransformer
{
    // Set to look at targetposition and returns eulerangles
    Vector3 LookAt(Vector3 targetPosition);
    void MoveTo(Vector3 position);
}