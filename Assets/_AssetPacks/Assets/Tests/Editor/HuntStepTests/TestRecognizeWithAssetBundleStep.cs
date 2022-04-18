//TODO: AR ELEMENT
// using System;
// using System.Collections.Generic;
// using System.IO;
// using Moq;
// using Newtonsoft.Json;
// using NUnit.Framework;
// using riddlehouse_libraries.products.models;
// using UnityEngine;
// using UnityEngine.XR.ARSubsystems;
//
// [TestFixture]
// public class TestRecognizeWithAssetBundleStep
// {
//     public XRReferenceImageLibrary lib;
//     public AssetBundle bundle;
//     private Action<bool> readyAction;
//
//     private string _title;
//     private string _videoUrl;
//     private string _storyUrl;
//     private string _manifestUrl;
//     private string _bundleUrl;
//
//     private string _storyText;
//     private HuntAsset _storyAsset;
//     private HuntAsset _videoAsset;
//     private HuntAsset _assetBundleManifest;
//
//     private Mock<IAssetBundleManifest> _manifestMock;
//     private string _assetName = "image_reference_library";
//     private string _assetType;
//     private HuntStep _step;
//
//     private Mock<ITextGetter> _textGetterMock;
//     private Mock<IAssetBundleGetter> _assetBundleGetterMock;
//     private Mock<IAssetBundleHelper<XRReferenceImageLibrary>> _assetBundleHelperMock;
//     
//     [SetUp]
//     public void Init()
//     {
//         //collects imagelibrary asset file from streaming assets; if null, then the file is missing.
//         lib = Resources.Load<XRReferenceImageLibrary>("editor/testLibrary");
//         bundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "editor/test_image_reference_library"));
//         readyAction = (success) => { };
//         _title = "title";
//         _storyUrl = "http://www.storyUrl.com";
//         _storyText = "story text";
//         _videoUrl = "http://videoUrl.com";
//         _manifestUrl = "http://ManifestUrl.com";
//         _bundleUrl = "http://BundleUrl.com";
//         
//         _storyAsset = new HuntAsset() {Type = AssetType.StoryText, Url = _storyUrl};
//         _videoAsset = new HuntAsset() { Type = AssetType.VideoToPlay, Url = _videoUrl };
//         _assetBundleManifest = new HuntAsset() { Type = AssetType.AssetBundleManifest, Url = _manifestUrl };
//         
//         _assetName = "image_reference_library";
//         _assetType = AssetBundleAssetType.XRImageReferenceLibrary.ToString();
//
//         AssetBundleManifest manifest = new AssetBundleManifest()
//         {
//             Android = new AssetBundlePlatformData()
//             {
//                 Assets = new List<BundleAsset>() { new BundleAsset() { Name = _assetName, Type = _assetType } },
//                 Uri = _bundleUrl,
//                 Version = 1
//             },
//             IOS = new AssetBundlePlatformData()
//             {
//                 Assets = new List<BundleAsset>() { new BundleAsset() { Name = _assetName, Type = _assetType } },
//                 Uri = _bundleUrl,
//                 Version = 1
//             }
//         };
//         
//         _step = new HuntStep() { Assets = new List<HuntAsset>() { _storyAsset, _videoAsset, _assetBundleManifest }, Title = _title, Type = StepType.RecognizeWithAssetBundle};
//         
//         _textGetterMock = new Mock<ITextGetter>();
//
//         _textGetterMock.Setup(x => x.GetText(_storyUrl, false, It.IsAny<Action<string>>()))
//             .Callback<string, bool, Action<string>>((theUrl, theCache, theAction) =>
//             {
//                 theAction.Invoke(_storyText);
//             }).Verifiable();
//         
//         _textGetterMock.Setup(x=> x.GetText(_manifestUrl, false, It.IsAny<Action<string>>()))
//             .Callback<string, bool, Action<string>>((theUrl, theCache, theAction) =>
//             {
//                 theAction(JsonConvert.SerializeObject(manifest));
//             }).Verifiable();
//         
//         _assetBundleGetterMock = new Mock<IAssetBundleGetter>();
//         _assetBundleGetterMock.Setup(
//             x => x.GetAssetBundle(
//                 It.IsAny<IAssetBundleManifest>(),
//                 It.IsAny<Action<AssetBundle>>())
//             ).Callback<IAssetBundleManifest, Action<AssetBundle>>((theManifest, theAssetBundleAction) =>
//         {
//             theAssetBundleAction(bundle); //we are using this, but since we can't create it at test-time, we use null as the matcher.
//         });
//         
//         _assetBundleHelperMock = new Mock<IAssetBundleHelper<XRReferenceImageLibrary>>();
//         _assetBundleHelperMock
//             .Setup(x => x.RetrieveAsset(_assetName, It.IsAny<AssetBundle>(), It.IsAny<Action<XRReferenceImageLibrary>>()))
//             .Callback<string, AssetBundle, Action<XRReferenceImageLibrary>>(
//                 (theAssetName, theAssetBundle, theImageLibraryRetrievedAction) =>
//                 {
//                     theImageLibraryRetrievedAction.Invoke(lib);
//                 });
//     }
//
//     [TearDown]
//     public void TearDown()
//     {
//         lib = null;
//         if(bundle != null)
//             bundle.Unload(true);
//         bundle = null;
//         _title = null;
//         _storyUrl = null;
//         _storyText = null;
//         _videoUrl = null;
//         _bundleUrl = null;
//
//         _storyAsset = null;
//         _videoAsset = null;
//         _assetBundleManifest = null;
//         _step = null;
//         
//         _textGetterMock = null;
//         _assetBundleGetterMock = null;
//         _assetBundleHelperMock = null;
//     }
//     
//     [Test]
//     public void TestNewInstance_wrong_type_throws()
//     {
//         //Given an unexpected type of huntStep
//         //When constructor is called.
//         //Then the constructor throws an error.
//         
//         //Arrange
//         _step = new HuntStep() { Type = StepType.HuntResolutionAndEnd, Assets = new List<HuntAsset>() { _storyAsset } };
//
//         //Act and Assert
//         Assert.Throws<ArgumentException>(() => new RecognizeWithAssetBundleStep(_step, _textGetterMock.Object, _assetBundleGetterMock.Object, _assetBundleHelperMock.Object, readyAction));
//     }
//     
//     [Test]
//     public void TestGetStepType()
//     {
//         //Given a new RecognizeWithAssetBundleStep with corresponding type
//         //When GetStepType is called
//         //returns correctStepType
//         
//         //Arrange
//         var sut = new RecognizeWithAssetBundleStep(_step, _textGetterMock.Object, _assetBundleGetterMock.Object, _assetBundleHelperMock.Object, readyAction);
//         //Act
//         var stepType = sut.GetStepType();
//         //Assert
//         Assert.AreEqual(StepType.RecognizeWithAssetBundle, stepType);
//     }
//     
//     [Test]
//     public void TestGetStoryText()
//     {
//         //Given a new RecognizeWithAssetBundleStep
//         //When GetStoryText is called
//         //Then the storyText element is returned.
//         
//         //Arrange
//         var sut = new RecognizeWithAssetBundleStep(_step, _textGetterMock.Object, _assetBundleGetterMock.Object, _assetBundleHelperMock.Object, readyAction);
//         //Act
//         var storyText = sut.GetStoryText();
//         //Assert
//         Assert.AreEqual(_storyText, storyText);
//     }
//     
//     [Test]
//     public void TestGetImageLibraryReference()
//     {
//         //Given a new RecognizeWithAssetBundleStep
//         //When GetImageLibraryReference is called
//         //Then the image reference libary is returned
//         
//         //Arrange
//         var sut = new RecognizeWithAssetBundleStep(_step, _textGetterMock.Object, _assetBundleGetterMock.Object, _assetBundleHelperMock.Object, readyAction);
//         //Act
//         var library = sut.GetImageLibraryReference();
//         //Assert
//         Assert.AreEqual(lib, library);
//     }
//     
//     [Test]
//     public void TestGetVideoToPlay()
//     {
//         //Given a new RecognizeWithAssetBundleStep
//         //When getVideoToPlay is called
//         //Then the video link is returned
//         
//         //Arrange
//         var sut = new RecognizeWithAssetBundleStep(_step, _textGetterMock.Object, _assetBundleGetterMock.Object, _assetBundleHelperMock.Object, readyAction);
//         //Act
//         var videoLink = sut.GetVideoToPlay();
//         //Assert
//         Assert.AreEqual(_videoUrl, videoLink);
//     }
//     
//     [Test]
//     public void TestGetAnswerData()
//     {
//         //Given a new RecognizeWithAssetBundleStep
//         //When getAnswerdata is called
//         //Then a scanning boolean answertype is returned.
//         
//         //arrange
//         var sut = new RecognizeWithAssetBundleStep(_step, _textGetterMock.Object, _assetBundleGetterMock.Object, _assetBundleHelperMock.Object, readyAction);
//         //act
//         var answer = sut.GetAnswerData();
//         //Assert
//         Assert.AreEqual(AnswerType.ScanningBoolean, answer.GetAnswerType());
//     }
// }
