using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;

public class TestAssetBundleGetter
{
    [Test]
    public void TestFactory_No_MonoBehaviour_Throws()
    {
        //Given no monobehaviour available.
        //When the factory is called.
        //Then the system throws.
        Assert.Throws<ArgumentException>( () => AssetBundleGetter.Factory(null));
    }

    [Test]
    public void GetAssetBundle()
    {
        //Given an assetbundlegetter and an assetbundlemanifest.
        //When the GetAssetBundle is called.
        //Then the GetAssetBundle action is called.
        
        var manifestMock = new Mock<IAssetBundleManifest>();
        Action<AssetBundle> bundleRetrieved = (bundle) => {};
        var assetBundleGetterActionsMock = new Mock<IAssetBundleGetterActions>();
        assetBundleGetterActionsMock.Setup(x => x.GetAssetBundle(manifestMock.Object, bundleRetrieved)).Verifiable();
        var assetBundleGetter = new AssetBundleGetter(assetBundleGetterActionsMock.Object);
        assetBundleGetter.GetAssetBundle(manifestMock.Object, bundleRetrieved);
        assetBundleGetterActionsMock.Verify(x => x.GetAssetBundle(manifestMock.Object, bundleRetrieved));
    }

    [Test]
    public void DisposeSelf()
    {
        //Given an assetbundlegetter.
        //When the DisposeSelf is called.
        //Then the DisposeSelf action is called,
        var assetBundleGetterActionsMock = new Mock<IAssetBundleGetterActions>();
        assetBundleGetterActionsMock.Setup(x => x.DisposeSelf()).Verifiable();
        var assetBundleGetter = new AssetBundleGetter(assetBundleGetterActionsMock.Object);
        assetBundleGetter.DisposeSelf();
        assetBundleGetterActionsMock.Verify(x =>x.DisposeSelf());
    }
}
