using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;
using riddlehouse.video;
using riddlehouse_libraries.products.models;
using TMPro;
using UnityEngine;
// using UnityEngine.XR.ARSubsystems;

[TestFixture]
public class TestHuntHomeComponentBehaviour
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

    public HuntHomeComponentBehaviour CreateHuntHomeBehaviourWithDependencies(ISimpleVideoView video = null,
        TextMeshProUGUI textMeshProUGUI = null, GameObject productGraphics = null,
        RectTransform scrollViewTransform = null)
    {
        var huntHomeGameObject = new GameObject();
        var huntHomeBehaviour = huntHomeGameObject.AddComponent<HuntHomeComponentBehaviour>();
        var textdisplayMock = textMeshProUGUI == null
            ? huntHomeGameObject.AddComponent<TextMeshProUGUI>()
            : textMeshProUGUI;
        scrollViewTransform = scrollViewTransform == null
            ? new GameObject().AddComponent<RectTransform>()
            : scrollViewTransform;
        var stepBtnMock = new GameObject().AddComponent<ConditionalStepBtnBehavior>();
        video ??= new Mock<ISimpleVideoView>().Object;
        productGraphics = productGraphics == null ? new GameObject() : productGraphics;

        huntHomeBehaviour.SetDependencies(stepBtnMock.gameObject, productGraphics, textdisplayMock, scrollViewTransform,
            video, new GameObject());
        return huntHomeBehaviour;
    }

    [Test]
    public void TestConstructor_Sets_Type_Hunt_Home()
    {
        //Given a user starts a new hunt
        //When a new HuntHome is created
        //Then that HuntHome has the type HuntHome.

        //Arrange
        GameObject go = new GameObject();
        //Act
        var sut = go.AddComponent<HuntHomeComponentBehaviour>();
        //Assert
        Assert.AreEqual(ComponentType.HuntHome, sut._viewType);
    }

    [Ignore("Requires updated - will be deprecated upon UI remake.")]
    [Test]
    public void TestConfigure_Configures_Component_For_Viewing()
    {
        //Arrange
        StartPanelData startPanelData = new StartPanelData()
            { HasAccess = true, Id = "id", Title = "title", VideoUrl = "url" };

        var productTitle = new GameObject().AddComponent<TextMeshProUGUI>();
        var productGraphics = new GameObject();

        var videoMock = new Mock<ISimpleVideoView>();
        videoMock.Setup(x => x.Initialize(It.IsAny<VideoCanvasController>())).Verifiable();
        videoMock.Setup(x => x.Configure(It.IsAny<SimpleVideoView.Config>())).Verifiable();

        //Act
        var sut = CreateHuntHomeBehaviourWithDependencies(videoMock.Object, productTitle, productGraphics);
        sut.Configure(startPanelData, introVideoPrepared =>
        {
            //action not necessary for this test.
        });
        
        //Assert
        videoMock.Verify(x => x.Initialize(It.IsAny<VideoCanvasController>()));
        videoMock.Verify(x => x.Configure(It.IsAny<SimpleVideoView.Config>()));
        Assert.AreEqual(startPanelData.Title, productTitle.text);
        Assert.IsTrue(productGraphics.activeSelf);
    }

    // [Test] //TODO: AR ELEMENT
    // public void TestConfigureARElements_ARSession_IsStarted_Behaviour()
    // {
    //     //Given the user has started a huntStep
    //     //When the step needs ARScanning
    //     //Then the ARElements are configured.
    //
    //     //Arrange 
    //     var huntStepMock = new Mock<IRecognizeImageAndPlayVideoHuntStep>();
    //     huntStepMock.Setup(x => x.GetImageLibraryReference()).Returns(lib).Verifiable();
    //     Action foundTarget = () => { };
    //
    //     var morphableARCameraMock = new Mock<IMorphableARCameraActions>();
    //     morphableARCameraMock.Setup(x => x.PrepARCamera(lib, foundTarget)).Verifiable();
    //
    //     //Act
    //     var sut = CreateHuntHomeBehaviourWithDependencies(new Mock<IVideo>().Object);
    //     sut.ConfigureARElements(morphableARCameraMock.Object, lib, foundTarget);
    //
    //     //Assert
    //     morphableARCameraMock.Verify(x => x.PrepARCamera(lib, foundTarget));
    // }

    [Test]
    public void TestHuntReady_Configures_StepView_UI()
    {
        //Given a user is watching the intro-video on the HuntHome
        //When the required hunt assets are finished downloading
        //Then we create & Configure the HuntHome StepView

        //Arrange
        var lengthOfHunt = 3;
        var stepListMock = new Mock<IConditionalStepList>();
        var huntControllerMock = new Mock<IChristmasHuntController>();
        var huntStepsMock = new Mock<IHuntSteps>();
        huntStepsMock.Setup(x => x.GetLengthOfHunt()).Returns(lengthOfHunt);
        huntControllerMock.Setup(x => x.GetCurrentHuntSteps()).Returns(huntStepsMock.Object).Verifiable();
        var scrollViewContentTransform = new GameObject().AddComponent<RectTransform>();
        var buttons = new List<IConditionalStepBtn>();
        stepListMock.Setup(x =>
                x.ConfigureStepList(huntStepsMock.Object, It.IsAny<List<IConditionalStepBtn>>(),
                    It.IsAny<Action<string>>()))
            .Verifiable();
        //Act
        var sut = CreateHuntHomeBehaviourWithDependencies(null, null, null, scrollViewContentTransform);
        sut.HuntReady(huntControllerMock.Object, stepListMock.Object, buttons);
        //Assert
        stepListMock.Verify(x =>
            x.ConfigureStepList(huntStepsMock.Object, It.IsAny<List<IConditionalStepBtn>>(),
                It.IsAny<Action<string>>()));
        foreach (var istepBtn in buttons)
        {
            var transform = istepBtn.GetGameObject().transform;
            Assert.AreEqual(Vector3.one, transform.localScale);
            Assert.AreSame(scrollViewContentTransform, transform.parent);
        }
    }

    [Test]
    public void TestReturnToAppHome_CallsFunction_In_Component()
    {
        //Given a new HuntHomeComponent
        //When ReturnToApphome is Called
        //Then ReturnToAppHome is called on the component.
        
        //Arrange
        GameObject go = new GameObject();
        Mock<IHuntHomeComponent> componentMock = new Mock<IHuntHomeComponent>();
        componentMock.Setup(x => x.ReturnToAppHome()).Verifiable();
        var sut = go.AddComponent<HuntHomeComponentBehaviour>();
        sut.SetLogic(componentMock.Object);
        //Act
        sut.ReturnToAppHome();
        //Assert
        componentMock.Verify(x => x.ReturnToAppHome());
    }
}
