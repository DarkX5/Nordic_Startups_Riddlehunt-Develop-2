//TODO: AR ELEMENT

// using System.Collections;
// using Newtonsoft.Json;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.TestTools;
// using UnityEngine.XR.ARSubsystems;
//
// public class AssetBundleGetterBehaviourTests
// {
//     [UnityTest]
//     public IEnumerator AssetBundleGetter_DownloadAssetBundle_AssetNotNull()
//     {
//         //Given a link to an assetbundlemanifest
//         //When that manifest is processed
//         //Then the system chooses the assetbundle based on the platform, and retrieves it.
//         //-- assetbundle is not null.
//         
//         var assetGetter = new MonoBehaviourTest<AssetBundleDownloadTest>();
//         yield return assetGetter;
//         Assert.IsNotNull(assetGetter.component.bundle);
//     }
//     
//     [UnityTest]
//     public IEnumerator AssetBundleGetterHelper_UnpackImageReferenceLibrary_AssetNotNull()
//     {
//         //Given a link to an assetbundlemanifest
//         //When that manifest is processed
//         //Then the system gets the assetbundle and unpacks the ImageReferenceLibrary asset from it.
//         var assetGetter = new MonoBehaviourTest<ImageReferenceLibraryDownloadTest>();
//         yield return assetGetter;
//         Assert.IsNotNull(assetGetter.component.library);
//     }
// }
// public class AssetBundleDownloadTest : MonoBehaviour, IMonoBehaviourTest
// {
//     public bool IsTestFinished { get; set; }
//     public AssetBundle bundle;
//     
//     private TextGetter _textGetter;
//     private AssetBundleGetter _assetBundleGetter;
//     
//     public void Awake() //this is the main, where the test is actually run.
//     {
//         _textGetter = TextGetter.Factory(this);
//         _textGetter.GetText(
//             "https://cdn.escaperoom-riddlehouse.dk/riddlehunt/unity_getter_test_files/AssetBundle/asset_bundle_manifest.json", 
//             false,
//             GetManifestFile);
//     }
//
//     private void GetManifestFile(string manifestJson) //get the manifest file from the server.
//     {
//         AssetBundleManifest manifest = JsonConvert.DeserializeObject<AssetBundleManifest>(manifestJson);
//         _assetBundleGetter = AssetBundleGetter.Factory(this);
//         _assetBundleGetter.GetAssetBundle(manifest, GetAssetBundle);
//     }
//
//     private void GetAssetBundle(AssetBundle assetBundle) //end test and clean up.
//     {
//         bundle = assetBundle;
//         _textGetter.DisposeSelf();
//         _assetBundleGetter.DisposeSelf();
//         IsTestFinished = true;
//     }
// }
//
// public class ImageReferenceLibraryDownloadTest : MonoBehaviour, IMonoBehaviourTest
// {
//     public bool IsTestFinished { get; set; }
//     public XRReferenceImageLibrary library;
//     
//     private AssetBundleHelper<XRReferenceImageLibrary> _helper;
//     private AssetBundle _bundle;
//     private AssetBundleManifest _manifest;
//     
//     
//     TextGetter _textGetter;
//     AssetBundleGetter _assetBundleGetter;
//     public void Awake() //this is the main, where the test is actually run.
//     {
//         _textGetter = TextGetter.Factory(this);
//         _textGetter.GetText(
//             "https://cdn.escaperoom-riddlehouse.dk/riddlehunt/unity_getter_test_files/AssetBundle/asset_bundle_manifest.json", 
//             false,
//             GetManifestFile);
//     }
//
//     private void GetManifestFile(string manifestJson) //get the manifest file from the server.
//     {
//         _manifest = JsonConvert.DeserializeObject<AssetBundleManifest>(manifestJson);
//         _assetBundleGetter = AssetBundleGetter.Factory(this);
//         _assetBundleGetter.GetAssetBundle(_manifest, GetAssetBundle);
//     }
//
//     private void GetAssetBundle(AssetBundle assetBundle) //unpack asset from the bundle.
//     {
//         _bundle = assetBundle;
//         _helper = new AssetBundleHelper<XRReferenceImageLibrary>(this);
//         _helper.RetrieveAsset(_manifest?.GetPlatformRelevantData().GetAssetName(AssetBundleAssetType.XRImageReferenceLibrary), _bundle, SetImageReferenceLibary);
//     }
//
//     private void SetImageReferenceLibary(XRReferenceImageLibrary imageLibrary) //end test and clean up.
//     {
//         library = imageLibrary;
//         _bundle.Unload(true);
//         _textGetter.DisposeSelf();
//         //_assetBundleGetter.DisposeSelf();
//         IsTestFinished = true;
//     }
// }