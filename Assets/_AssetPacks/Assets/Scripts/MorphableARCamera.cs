//TODO: AR Element
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.XR.ARFoundation;
// using UnityEngine.XR.ARSubsystems;
//
// public interface IMorphableARCameraActions
// {
//     public void PrepARCamera(XRReferenceImageLibrary library, Action foundTarget);
// }
// public class MorphableARCamera : MonoBehaviour, IMorphableARCameraActions
// {
//     public static MorphableARCamera morph;
//     public Camera UICamera;
//     public Camera ARCamera;
//     public void Awake()
//     {
//         if (morph == null)
//         {
//             morph = this;
//         }
//         else
//         {
//             Destroy(this.gameObject);
//         }
//         _arTrackedImageManager.trackedImagesChanged += FoundTargetUnsubscribe;
//     }
//     
//     #if UNITY_EDITOR
//     public void FixedUpdate()
//     {
//         if (_arTrackedImageManager.enabled)
//         {
//             if (Input.GetButtonDown("Jump"))
//             {
//                 EditorConditionsMet();
//             }
//         }
//     }
//     #endif
//     [SerializeField]
//     private ARSessionOrigin arSessionOrigin;
//     [SerializeField]
//     private ARSession _arSession;
//
//     [SerializeField] private ARTrackedImageManager _arTrackedImageManager;
//
//     public void OnDestroy()
//     {
//         _arTrackedImageManager.trackedImagesChanged -= FoundTargetUnsubscribe;
//     }
//
//     // private Action<ARTrackedImagesChangedEventArgs> _foundTargetRef;
//     private Action _foundTargetRef = () => {};
//     public void PrepARCamera(XRReferenceImageLibrary library, Action foundTarget)
//     {
//         //library MUST be attached before enabling the arTrackedimageManager.
//         SwitchCameraDisplay(false);
//         _arTrackedImageManager.referenceLibrary = library;
//         _arTrackedImageManager.enabled = true;
//         _foundTargetRef += foundTarget;
//     }
//
//     public void FoundTargetUnsubscribe(ARTrackedImagesChangedEventArgs eventArgs)
//     {
//         foreach (var newImage in eventArgs.added)
//         {
//             SwitchCameraDisplay(true);
//             _foundTargetRef.Invoke();
//             _arTrackedImageManager.enabled = false;
//             _foundTargetRef = () => {};
//             break;
//         }
//     }
//
//     private void SwitchCameraDisplay(bool isUIDisplay)
//     {
//         if (isUIDisplay)
//         {
//             UICamera.gameObject.SetActive(true);
//             ARCamera.gameObject.SetActive(false);
//         }
//         else
//         {
//             UICamera.gameObject.SetActive(false);
//             ARCamera.gameObject.SetActive(true);
//         }
//     }
// #if UNITY_EDITOR
//     private void EditorConditionsMet()
//     {
//         SwitchCameraDisplay(true);
//         _foundTargetRef.Invoke();
//         _arTrackedImageManager.enabled = false;
//         _foundTargetRef = () => {};
//     }
// #endif
// }
