using System;
using UnityEngine;
using Zenject;

public class FollowPlayer : MonoBehaviour
{
    // The target we are following
    [SerializeField]
    private Transform target;
    // The distance in the x-z plane to the target
    [SerializeField]
    private float height = 0f;

    private ICameraController CameraController;
    
    // Use this for initialization
    void Start()
    {
        CameraController = new MyCameraController(transform, height);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        CameraController.Follow(target.position);
    }
}