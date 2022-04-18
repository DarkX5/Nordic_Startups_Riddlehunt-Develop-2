//TODO: AR Element

// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using UnityEngine;
// using UnityEngine.Networking;
// using UnityEngine.XR.ARSubsystems;
//
// public interface IImageReferenceLibraryGetter
// {
//     public void GetImageReferenceLibrary(string fileLink, uint version, string assetName, Action<XRReferenceImageLibrary> imageReferenceLibraryRetrieved);
// }
// public interface IImageReferenceLibraryGetterActions
// {
//     public void GetImageReferenceLibrary(string fileLink, uint version, string assetName, Action<XRReferenceImageLibrary> imageReferenceLibraryRetrieved);
// }
//
// public class ImageReferenceLibraryGetter: IImageReferenceLibraryGetter
// {
//     public static ImageReferenceLibraryGetter Factory(MonoBehaviour monoBehaviour)
//     {
//         if (monoBehaviour == null)
//         {
//             throw new ArgumentException("monobehavior missing in textGetter");
//         }
//
//         var imageLibraryGetterBehavior = new ImageReferenceLibraryGetterBehavior(monoBehaviour);
//         var imageReferenceLibraryGetter = new ImageReferenceLibraryGetter(imageLibraryGetterBehavior);
//
//         return imageReferenceLibraryGetter;
//     }
//     
//     private readonly IImageReferenceLibraryGetterActions _ImageReferenceLibraryGetterActions;
//     public ImageReferenceLibraryGetter(IImageReferenceLibraryGetterActions imageReferenceLibraryGetterActions)
//     {
//         _ImageReferenceLibraryGetterActions = imageReferenceLibraryGetterActions;
//     }
//     public void GetImageReferenceLibrary(string fileLink, uint version, string assetName, Action<XRReferenceImageLibrary> imageReferenceLibraryRetrieved)
//     {
//         _ImageReferenceLibraryGetterActions.GetImageReferenceLibrary(fileLink, version, assetName, imageReferenceLibraryRetrieved);
//     }
// }
// public class ImageReferenceLibraryGetterBehavior : FileGetter, IImageReferenceLibraryGetterActions
// {
//     public readonly MonoBehaviour _monoBehaviour;
//     private UnityWebRequest _uwr;
//
//     public ImageReferenceLibraryGetterBehavior(MonoBehaviour monoBehaviour)
//     {
//         _monoBehaviour = monoBehaviour;
//     }
//     public void GetImageReferenceLibrary(string fileLink, uint version, string assetName, Action<XRReferenceImageLibrary> imageReferenceLibraryRetrieved)
//     {
//         _monoBehaviour.StartCoroutine(RetrieveAsset(fileLink, assetName, imageReferenceLibraryRetrieved, version));
//     }
//     
//     IEnumerator RetrieveAsset(string fileLink, string assetName, Action<XRReferenceImageLibrary> fileRetrieved, uint version)
//     {
//         using (_uwr = UnityWebRequestAssetBundle.GetAssetBundle(fileLink, version, 0))
//         {
//             yield return _uwr.SendWebRequest();
//  
//             if (_uwr.result == UnityWebRequest.Result.ConnectionError || _uwr.result == UnityWebRequest.Result.ProtocolError)
//             {
//                 throw new ArgumentException(_uwr.error);
//             }
//             // Get downloaded asset bundle
//             AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(_uwr);
//
//             var assetLoadRequest = bundle.LoadAssetAsync<XRReferenceImageLibrary>(assetName);
//             yield return assetLoadRequest;
//
//             XRReferenceImageLibrary lib = assetLoadRequest.asset as XRReferenceImageLibrary;
//             if (lib != null)
//             {
//                 Debug.Log("unpacked library: "+lib.name);
//                 fileRetrieved.Invoke(lib);
//             }
//             else
//             {
//                 throw new NullReferenceException();
//             }
//             _uwr.Dispose();
//             bundle.Unload(false);
//         }
//     }
// }
