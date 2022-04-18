using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using UnityEditor;
using UnityEngine;

[TestFixture]
public class TestBaseStepController
{
    public Dictionary<ComponentType, IViewActions> CreateViews(
        IViewActions huntStartMock,
        IViewActions storyUIMock,
        IViewActions endHuntUIMock
        )
    {
        var Views = new Dictionary<ComponentType, IViewActions>();
        Views = new Dictionary<ComponentType, IViewActions>() {};
        Views.Add(ComponentType.HuntHome, huntStartMock);
        Views.Add(ComponentType.Story, storyUIMock);
        Views.Add(ComponentType.End, endHuntUIMock);
        return Views;
    }

    public Mock<IViewActions> CreateHuntComponentUIActionsMock(bool shown, ComponentType type)
    {
        var mock = new Mock<IViewActions>();
        mock.Setup(x => x.Display()).Verifiable();
        mock.Setup(x => x.IsShown()).Returns(shown).Verifiable();
        mock.Setup(x => x.GetComponentType()).Returns(type).Verifiable();
        return mock;
    }
    [Test]
    public void TestShowAssetInStep_Succeeds()
    {
        //Given a BaseStepController with a HuntStart screen
        //when ShowAssetInStep is called
        //Then the screen is shown.

        //setup
        var huntStartMock = CreateHuntComponentUIActionsMock(true, ComponentType.HuntHome);
        var storyUIMock = CreateHuntComponentUIActionsMock(true, ComponentType.Story); //past screen, now going to show huntStart
        var endHuntUIMock = CreateHuntComponentUIActionsMock(false, ComponentType.End);

        var tabComponentMock = new Mock<ITabComponent>();
        tabComponentMock.Setup(x => x.Display(ComponentType.HuntHome)).Verifiable();
        BaseOldStepController sut = new BaseOldStepController(StepType.DisplayStoryAndDone, tabComponentMock.Object);
        sut._views.Add(ComponentType.HuntHome, huntStartMock.Object);
        sut._views.Add(ComponentType.Story, storyUIMock.Object);
        sut._views.Add(ComponentType.End, endHuntUIMock.Object);
        sut.StartStep();
        //Act
        sut.ShowAssetInStep(ComponentType.HuntHome);
        //Assert
        Assert.IsTrue(sut.IsShowingAsset(ComponentType.HuntHome));
        tabComponentMock.Verify(x => x.Display(ComponentType.HuntHome));
        huntStartMock.Verify(x => x.IsShown());
        huntStartMock.Verify(x=> x.GetComponentType());
    }
    [Test]
    public void TestShowAssetInStep_Fails()
    {
        //Given a BaseStepController with a HuntStart screen
        //when ShowAssetInStep is called, but for some reason the screen can't hide.
        //Then the screen isn't hidden.
        //setup
        var huntStartMock = CreateHuntComponentUIActionsMock(false, ComponentType.HuntHome);
        var storyUIMock = CreateHuntComponentUIActionsMock(false, ComponentType.Story);
        var endHuntUIMock = CreateHuntComponentUIActionsMock(false, ComponentType.End);
        var tabComponentMock = new Mock<ITabComponent>();
        tabComponentMock.Setup(x => x.Display(ComponentType.HuntHome)).Verifiable();
        BaseOldStepController sut = new BaseOldStepController(StepType.DisplayStoryAndDone, tabComponentMock.Object);
        sut._views.Add(ComponentType.HuntHome, huntStartMock.Object);
        sut._views.Add(ComponentType.Story, storyUIMock.Object);
        sut._views.Add(ComponentType.End, endHuntUIMock.Object);
        sut.StartStep();
        //Act
        sut.ShowAssetInStep(ComponentType.HuntHome);
        //Assert
        Assert.IsFalse(sut.IsShowingAsset(ComponentType.HuntHome));
        tabComponentMock.Verify(x => x.Display(ComponentType.HuntHome));
        huntStartMock.Verify(x => x.IsShown());
    }
    [Test]
    public void TestShowAssetInStep_Throws()
    {
        //Given a BaseStepController without a HuntStart screen
        //when ShowAssetInStep is called for the HuntStart
        //Then an exception is thrown.
        
        //setup
        // var huntStartMock = null;
        var huntStartMock = CreateHuntComponentUIActionsMock(false, ComponentType.HuntHome);
        var storyUIMock = CreateHuntComponentUIActionsMock(false, ComponentType.Story);
        var endHuntUIMock = CreateHuntComponentUIActionsMock(false, ComponentType.End);
        var tabComponentMock = new Mock<ITabComponent>();

        BaseOldStepController sut = new BaseOldStepController(StepType.DisplayStoryAndDone, tabComponentMock.Object);
        sut._views.Add(ComponentType.HuntHome, huntStartMock.Object);
        sut._views.Add(ComponentType.Story, storyUIMock.Object);
        sut._views.Add(ComponentType.End, endHuntUIMock.Object);
        sut._views.Add(ComponentType.Scanning, null);
        sut.StartStep();
        //Act & Assert
        Assert.Throws<ArgumentException>(() => sut.ShowAssetInStep(ComponentType.Scanning));
    }
    [Test]
    public void TestIsShowingAsset_True()
    {
        //Given a BaseStepController with the necessary screens.
        //when IsShowingAsset and screen is shown correctly.
        //Then the function returns true.   
        //Setup
        var huntStartMock = CreateHuntComponentUIActionsMock(true, ComponentType.HuntHome);
        var storyUIMock = CreateHuntComponentUIActionsMock(false, ComponentType.Story);
        var endHuntUIMock = CreateHuntComponentUIActionsMock(false, ComponentType.End);
        var tabComponentMock = new Mock<ITabComponent>();
        BaseOldStepController sut = new BaseOldStepController(StepType.DisplayStoryAndDone, tabComponentMock.Object);
        sut._views.Add(ComponentType.HuntHome, huntStartMock.Object);
        sut._views.Add(ComponentType.Story, storyUIMock.Object);
        sut._views.Add(ComponentType.End, endHuntUIMock.Object);
        sut.StartStep();
        //Act
        sut.ShowAssetInStep(ComponentType.HuntHome);
        //Assert
        Assert.IsTrue(sut.IsShowingAsset(ComponentType.HuntHome));
        huntStartMock.Verify(x => x.IsShown());
    }
    [Test]
    public void TestIsShowingAsset_False()
    {
        //Given a BaseStepController with the necessary screens.
        //when IsShowingAsset and screen can't be shown.
        //Then the function returns false.   
        
        //Setup
        var huntStartMock = CreateHuntComponentUIActionsMock(false, ComponentType.HuntHome);
        var storyUIMock = CreateHuntComponentUIActionsMock(false, ComponentType.Story);
        var scanningFoundUIMock = CreateHuntComponentUIActionsMock(false, ComponentType.Scanning);
        var endHuntUIMock = CreateHuntComponentUIActionsMock(false, ComponentType.End);
        var tabComponentMock = new Mock<ITabComponent>();

        BaseOldStepController sut = new BaseOldStepController(StepType.DisplayStoryAndDone, tabComponentMock.Object);
        sut._views.Add(ComponentType.HuntHome, huntStartMock.Object);
        sut._views.Add(ComponentType.Story, storyUIMock.Object);
        sut._views.Add(ComponentType.End, endHuntUIMock.Object);
        sut.StartStep();
        //Act
        sut.ShowAssetInStep(ComponentType.HuntHome);
        //Assert
        Assert.IsFalse(sut.IsShowingAsset(ComponentType.HuntHome));
        huntStartMock.Verify(x => x.IsShown());
    }
    [Test]
    public void TestIsShowingAsset_Throws()
    {
        //Given a BaseStepController without a HuntStart screen
        //when IsShowingAsset is called for the HuntStart
        //Then an exception is thrown.
        //Setup
        // var huntStartMock = null;
        var huntStartMock = CreateHuntComponentUIActionsMock(false, ComponentType.HuntHome);
        var storyUIMock = CreateHuntComponentUIActionsMock(false, ComponentType.Story);
        var endHuntUIMock = CreateHuntComponentUIActionsMock(false, ComponentType.End);
        //Act
        var tabComponentMock = new Mock<ITabComponent>();
        BaseOldStepController sut = new BaseOldStepController(StepType.DisplayStoryAndDone, tabComponentMock.Object);
        sut._views.Add(ComponentType.HuntHome, huntStartMock.Object);
        sut._views.Add(ComponentType.Story, storyUIMock.Object);
        sut._views.Add(ComponentType.End, endHuntUIMock.Object);
        sut._views.Add(ComponentType.Scanning, null);
        //Assert
        Assert.Throws<ArgumentException>(() => sut.IsShowingAsset(ComponentType.Scanning));
    }

    [Test]
    public void TestEndStep_Success()
    {
        //Given a BaseStepController with the necessary screens.
        //when EndStep is called.
        //Then all active screens are hidden. 
        //Setup
        var huntStartMock = CreateHuntComponentUIActionsMock(true, ComponentType.HuntHome);
        var storyUIMock = CreateHuntComponentUIActionsMock(true, ComponentType.Story); //past screen, now going to show huntStart
        var endHuntUIMock = CreateHuntComponentUIActionsMock(false, ComponentType.End);
        huntStartMock.Setup(x => x.Hide()).Verifiable();
        storyUIMock.Setup(x => x.Hide()).Verifiable();
        var tabComponentMock = new Mock<ITabComponent>();

        BaseOldStepController sut = new BaseOldStepController(StepType.DisplayStoryAndDone, tabComponentMock.Object);
        sut._views.Add(ComponentType.HuntHome, huntStartMock.Object);
        sut._views.Add(ComponentType.Story, storyUIMock.Object);
        sut._views.Add(ComponentType.End, endHuntUIMock.Object);
        //Act
        sut.EndStep();
        //Assert
        huntStartMock.Verify(x => x.IsShown());
        huntStartMock.Verify(x=> x.Hide());
        storyUIMock.Verify(x => x.IsShown());
        storyUIMock.Verify(x=> x.Hide());
    }

    [Test]
    public void TestEndStep_Throws()
    {
        //Given a BaseStepController with the missing screens.
        //When Endstep is called
        //Then the BaseStepController throws an error.

        //Setup
        var huntStartMock = CreateHuntComponentUIActionsMock(true, ComponentType.HuntHome);
        var storyUIMock =
            CreateHuntComponentUIActionsMock(false, ComponentType.Story); //past screen, now going to show huntStart
        //Act
        var tabComponentMock = new Mock<ITabComponent>();

        BaseOldStepController sut = new BaseOldStepController(StepType.DisplayStoryAndDone, tabComponentMock.Object);
        sut._views.Add(ComponentType.HuntHome, huntStartMock.Object);
        sut._views.Add(ComponentType.Story, storyUIMock.Object);
        sut._views.Add(ComponentType.End, null);
        //Assert
        Assert.Throws<ArgumentException>(() => sut.EndStep());
    }

    [Test]
    public void TestGetTypesInOrder()
    {
        var tabComponentMock = new Mock<ITabComponent>();

        BaseOldStepController sut = new BaseOldStepController(StepType.DisplayStoryAndDone, tabComponentMock.Object);

        Assert.Throws<ArgumentException>(() => sut.GetTypesInOrder());
    }
    [Test]
    public void TestGetFirstStepTypeToShow()
    {
        var tabComponentMock = new Mock<ITabComponent>();

        BaseOldStepController sut = new BaseOldStepController(StepType.DisplayStoryAndDone, tabComponentMock.Object);

        Assert.Throws<ArgumentException>(() => sut.GetFirstStepTypeToShow());
    }
}
