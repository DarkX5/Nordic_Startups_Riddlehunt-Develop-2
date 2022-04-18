//TODO: AR Element

// using System;
// using Newtonsoft.Json;
// using riddlehouse_libraries.products.models;
// using riddlehouse_libraries.products.models.DTOs;
// using UnityEngine.XR.ARSubsystems;
//
// public interface IRecognizeWithAssetsBundleStep
// {
//     public string GetStepId();
//     public string GetStepTitle();
//     public string GetStoryText();
//     public string GetVideoToPlay();
//     public IAnswerData GetAnswerData();
//     public StepType GetStepType();
//     public XRReferenceImageLibrary GetImageLibraryReference();
// }
// public class RecognizeWithAssetBundleStep: IRecognizeWithAssetsBundleStep, IHuntStep, IInternalHuntStep
// {
//     private ITextAsset _storyText;
//     private IAnswerData _iAnswerData;
//     private IImageReferenceLibraryAsset _libraryAsset;
//     private string _videoURLToPlay;
//     
//     private int _readyCount; //how many assets have we successfully downloaded
//     private int targetCount; //how many assets to we expect to download.
//     
//     private readonly object _myLock;
//     private readonly Action<bool> _isReady;
//     private bool _hasFailed = false;
//     private bool _downloadComplete = false;
//
//     private readonly HuntStep _step;
//     private readonly ITextGetter _textGetter;
//     private readonly IAssetBundleGetter _assetBundleGetter;
//     private readonly IAssetBundleHelper<XRReferenceImageLibrary> _assetBundleHelper;
//     
//     public RecognizeWithAssetBundleStep(HuntStep step, ITextGetter textGetter, IAssetBundleGetter assetBundleGetter, IAssetBundleHelper<XRReferenceImageLibrary> assetBundleHelper, Action<bool> isReady)
//     {
//         if (step.Type != StepType.RecognizeWithAssetBundle)
//             throw new ArgumentException("Wrong step type in constructor");
//         var helper = new AssetGetterHelper();
//         _textGetter = textGetter;
//         _assetBundleGetter = assetBundleGetter;
//         _assetBundleHelper = assetBundleHelper;
//         
//         _readyCount = 0;
//         _myLock = new object();
//         _isReady = isReady;
//         
//         _step = step;
//         targetCount = 2; //story, imageReference
//
//         _videoURLToPlay = helper.GetAssetUrl(step, AssetType.VideoToPlay);
//         
//         _storyText = new StoryTextAsset(_textGetter, helper.GetAssetUrl(_step, AssetType.StoryText),
//             DownloadComplete);
//         
//         _iAnswerData = new BooleanAnswerData();
//         _textGetter.GetText(helper.GetAssetUrl(_step, AssetType.AssetBundleManifest), false, (manifestJson) =>
//         {
//             var manifest = JsonConvert.DeserializeObject<AssetBundleManifest>(manifestJson);
//             _assetBundleGetter.GetAssetBundle(manifest, (assetBundle) =>
//             {
//                 _libraryAsset = new ImageReferenceLibraryAsset(
//                     manifest?.GetPlatformRelevantData().GetAssetName(AssetBundleAssetType.XRImageReferenceLibrary),
//                     assetBundle,
//                     _assetBundleHelper,
//                     DownloadComplete
//                 );
//             });
//         });
//
//     }
//
//     private void DownloadComplete(bool success)
//     { 
//         if (!_hasFailed && !_downloadComplete)
//         { 
//             if (!success)
//             {
//                 _isReady.Invoke(false);
//                 _textGetter.DisposeSelf();
//                 _assetBundleGetter.DisposeSelf();
//                 _hasFailed = true;
//                 return;
//             }
//             
//             lock (_myLock)
//             {
//                 _readyCount++;
//                 if (_readyCount >= targetCount)
//                 {
//                     _downloadComplete = true;
//                     _isReady.Invoke(true);
//                     _textGetter.DisposeSelf();
//                     _assetBundleGetter.DisposeSelf();
//                 }
//             }
//         }
//     }
//
//     public string GetStoryText()
//     {
//         return _storyText.GetText();
//     }
//
//     public string GetVideoToPlay()
//     {
//         return _videoURLToPlay;
//     }
//
//     public IAnswerData GetAnswerData()
//     {
//         return _iAnswerData;
//     }
//     
//     public string GetStepTitle()
//     {
//         return _step.Title;
//     }
//
//     public string GetStepId()
//     {
//         return _step.Id;
//     }
//
//     public StepCondition GetCondition()
//     {
//         return _step.Condition;
//     }
//
//     public StepType GetStepType()
//     {
//         return _step.Type;
//     }
//
//     public bool HasAnswer()
//     {
//         return _iAnswerData.HasAnswer();
//     }
//
//     public XRReferenceImageLibrary GetImageLibraryReference()
//     {
//         return _libraryAsset.GetLibrary();
//     }
//
//     public bool IsExpectedStepType(StepType steptype)
//     {
//         return steptype == GetStepType();
//     }
//
//     public bool ValidateAssetConfiguration()
//     {
//         return _downloadComplete;
//     }
//
//     public bool DidBypassValidation()
//     {
//         return false;
//     }
//
//     public void SetBypassvalidation(string url)
//     {
//         throw new System.NotImplementedException();
//     }
// }
