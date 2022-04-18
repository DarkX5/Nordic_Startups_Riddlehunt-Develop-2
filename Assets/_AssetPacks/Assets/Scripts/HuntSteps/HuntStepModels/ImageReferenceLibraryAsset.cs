//TODO: AR Element

// using System;
// using System.Collections;
// using System.Collections.Generic;
// using riddlehouse_libraries.products.models;
// using UnityEngine;
// using UnityEngine.XR.ARSubsystems;
//
// public interface IImageReferenceLibraryAsset
// {
//     public XRReferenceImageLibrary GetLibrary();
// }
// public class ImageReferenceLibraryAsset :IImageReferenceLibraryAsset, IAsset
// {
//     private XRReferenceImageLibrary _library;
//     private Action<bool> _isReady;
//     public ImageReferenceLibraryAsset(string assetName, AssetBundle bundle, IAssetBundleHelper<XRReferenceImageLibrary> bundleHelper, Action<bool> isReady)
//     {
//         _isReady = isReady;
//         bundleHelper.RetrieveAsset(assetName, bundle, SetLibrary);
//     }
//
//     private void SetLibrary(XRReferenceImageLibrary library)
//     {
//         _library = library;
//         _isReady.Invoke(library != null);
//     }
//     public XRReferenceImageLibrary GetLibrary()
//     {
//         return _library;
//     }
//
//     public AssetType GetAssetType()
//     {
//         return AssetType.ImageLibraryReferenceBundle;
//     }
// }
