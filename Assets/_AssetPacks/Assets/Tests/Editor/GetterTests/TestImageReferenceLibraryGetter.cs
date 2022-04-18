//TODO: AR Element

// using System;
// using System.Collections;
// using System.Collections.Generic;
// using Moq;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.XR.ARSubsystems;
//
// [TestFixture]
// public class TestImageReferenceLibraryGetter
// {
//     [Test]
//     public void TestFactory_Throws()
//     {
//         // Given no monobehavior
//         // When constructing the ImageReferenceLibraryGetter
//         // Then an exception is thrown
//         ImageReferenceLibraryGetter sut;
//         Assert.Throws<ArgumentException>(() => sut = ImageReferenceLibraryGetter.Factory(null));
//     }
//     [Test]
//     public void TestFactory_Succeeds()
//     {
//         // Given a monobehavior
//         // When constructing the ImageReferenceLibraryGetter
//         // Then a ImageReferenceLibraryGetter is returned
//         GameObject go = new GameObject();
//         var mono = go.AddComponent<HuntHomeComponentBehaviour>();
//         ImageReferenceLibraryGetter sut = ImageReferenceLibraryGetter.Factory(mono);
//         Assert.IsNotNull(sut);
//     }
//
//     [Test]
//     public void TestGetImageReferenceLibrary()
//     {
//         string assetBundleLink = "thisIsALink.com";
//         string assetBundleName = "assetName";
//         Action<XRReferenceImageLibrary> imgReferenceLibraryRetrievedAction = (imgReferenceLibrary) => { };
//         
//         var imageReferenceLibraryGetterBehaviorMock = new Mock<IImageReferenceLibraryGetterActions>();
//         imageReferenceLibraryGetterBehaviorMock
//             .Setup(x => x.GetImageReferenceLibrary(
//                 assetBundleLink, 
//                 1,
//                 assetBundleName, 
//                 It.IsAny<Action<XRReferenceImageLibrary>>()))
//             .Callback<string, uint, string, Action<XRReferenceImageLibrary>>((link, version, cache, imageLibraryReferenceRetrieved) =>
//             {
//                 //imageLibraryReferenceRetrieved.Invoke(null);
//             });
//         
//         var sut = new ImageReferenceLibraryGetter(imageReferenceLibraryGetterBehaviorMock.Object);
//         sut.GetImageReferenceLibrary(assetBundleLink, 1, assetBundleName, imgReferenceLibraryRetrievedAction);
//         imageReferenceLibraryGetterBehaviorMock
//             .Verify(x => x.GetImageReferenceLibrary(assetBundleLink, 1, assetBundleName, imgReferenceLibraryRetrievedAction));
//     }
// }
