using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using UnityEngine;
// using UnityEngine.XR.ARSubsystems;

[TestFixture]
public class TestHuntAssetGetter
{
    //TODO: AR Element
    // public XRReferenceImageLibrary lib;

    [SetUp]
    public void Init()
    {
        //collects imagelibrary asset file from streaming assets; if null, then the file is missing.
        //TODO: AR Element
        // lib = Resources.Load<XRReferenceImageLibrary>("editor/testLibrary");
    }
    
    [Test]
    public void TestFactory_Throws()
    {
        // Given no monobehavior
        // When constructing the HuntDataGetter
        // Then an exception is thrown
        IHuntAssetGetter sut;
        Assert.Throws<ArgumentException>(() => sut = HuntAssetGetter.Factory(null));
    }
    [Test]
    public void TestFactory_Succeeds()
    {
        // Given a monobehavior
        // When constructing the HuntDataGetter
        // Then a HuntDataGetter is returned
        GameObject go = new GameObject();
        var mono = go.AddComponent<HuntHomeComponentBehaviour>();
        IHuntAssetGetter sut = HuntAssetGetter.Factory(mono);
        Assert.IsNotNull(sut);
    }

    [Test]
    public void TestGetHuntAssets_Succeeds()
    {
        //Given a correct huntflow
        //When the user is watching the hunt intro video.
        //Then the hunt files are downloaded and processed into live-assets (IHuntDataCollection)

        var feedbackLink = "https://google.com";
        
        var stepType = StepType.RecognizeImageAndPlayVideo;
        
        var storyTextUrl = "https://some.url";
        var storyTextType = AssetType.StoryText;

        var ImageLibraryUrl = "https://some.url";
        var ImageLibrarytype = AssetType.ImageLibraryReferenceBundle;
        
        var ImageLibraryManifestURL = "https://some.url";
        var ImageLibraryManifestType = AssetType.ImageLibraryReferenceBundleManifest;
        
        var videoUrl = "https://some.url";
        var videoType = AssetType.VideoToPlay;
        
        HuntAsset storyTextAssetDto = new HuntAsset() { Url = storyTextUrl, Type = storyTextType};
        HuntAsset imageLibraryAssetDto = new HuntAsset() { Url = ImageLibraryUrl, Type = ImageLibrarytype};
        HuntAsset imageLibraryManifestAssetDto = new HuntAsset() { Url = ImageLibraryManifestURL, Type = ImageLibraryManifestType};
        HuntAsset videoAssetDto = new HuntAsset() { Url = videoUrl, Type = videoType};
        
        HuntStep stepDto = new HuntStep() { Type = stepType, Assets = new List<HuntAsset>(new[] {storyTextAssetDto, imageLibraryAssetDto, imageLibraryManifestAssetDto, videoAssetDto})};
        HuntFlow huntFlow = new HuntFlow() { Steps = new List<HuntStep>(new[]{stepDto})};
        
        var stepdata = new InternalDisplayStoryAndDoneHuntStep();
        stepdata.StoryText = "story text";

        List<IInternalHuntStep> unsafeAssetCollection = new List<IInternalHuntStep>() {stepdata};
        var huntdata = new HuntSteps(feedbackLink, "id");
        huntdata.ConvertInternalStepdata(unsafeAssetCollection);
        
        var huntAssetGetterActions = new Mock<IHuntAssetGetterActions>();
        huntAssetGetterActions.Setup(x => x.GetHuntAssets(huntFlow, "id", feedbackLink, It.IsAny<Action<IHuntSteps>>()))
            .Callback<HuntFlow, string, string, Action<IHuntSteps>>((huntFlowInternalParameter, theProductId, myFeedbackLink, huntDataRetrieved) =>
            {
                huntDataRetrieved.Invoke(huntdata);
            });
        IHuntAssetGetter sut = new HuntAssetGetter(huntAssetGetterActions.Object);
        IHuntSteps huntStepsIsReturned = null;
        sut.GetHuntAssets(huntFlow, "id", feedbackLink, (huntData) =>
        {
            huntStepsIsReturned = huntData;
        });
        Assert.IsNotNull(huntStepsIsReturned);
        Assert.AreEqual(feedbackLink, huntdata.GetFeedbackLink());
    }
}