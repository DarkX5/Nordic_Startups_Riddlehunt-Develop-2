using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;
using UnityEngine;

[TestFixture]
public class TestResolutionVideoAndEndStepController
{
    private Mock<IResolutionVideoAndEndStep> resolutionVideoEndStepMock;
    private Mock<IChristmasHuntController> huntControllerMock;
    private Mock<IViewActions> endUIActions;
    private Mock<IEndHuntComponent> endControllerMock;

    private string endText;
    private string endVideoUrl;
    private string id;
    
    [SetUp]
    public void Init()
    {
        resolutionVideoEndStepMock = new Mock<IResolutionVideoAndEndStep>();
        huntControllerMock = new Mock<IChristmasHuntController>();
        endControllerMock = new Mock<IEndHuntComponent>();
        endUIActions = new Mock<IViewActions>();

        endText = "endText";
        endVideoUrl = "https://endVideoUrl.com";
        id = "id";
        
        resolutionVideoEndStepMock.Setup(x=>x.GetEndText()).Returns(endText).Verifiable();
        resolutionVideoEndStepMock.Setup(x=>x.GetResolutionVideoLink()).Returns(endVideoUrl).Verifiable();
        resolutionVideoEndStepMock.Setup(x => x.GetStepId()).Returns(id).Verifiable();
        huntControllerMock.Setup(x => x.MarkStepStarted(id)).Verifiable();
        endControllerMock.Setup(x => x.Configure(endText, It.IsAny<Action>())).Verifiable();
        endControllerMock.Setup(x => x.GetComponentUIActions()).Returns(endUIActions.Object).Verifiable();
        endUIActions.Setup(x => x.Display()).Verifiable();
    }

    [TearDown]
    public void TearDown()
    {
        resolutionVideoEndStepMock = null;
        huntControllerMock = null;
        endControllerMock = null;
    }

    [Test]
    public void TestStartStep_CallsFunctions_In_Dependencies()
    {
        var sut = new ResolutionVideoAndEndOldStepController(endControllerMock.Object);
        resolutionVideoEndStepMock.Setup(x => x.MarkAnswered()).Verifiable();

        sut.StartStep(resolutionVideoEndStepMock.Object, huntControllerMock.Object, true);
        
        resolutionVideoEndStepMock.Verify(x => x.MarkAnswered());
        resolutionVideoEndStepMock.Verify(x=>x.GetEndText());
        resolutionVideoEndStepMock.Verify(x=>x.GetResolutionVideoLink());
        resolutionVideoEndStepMock.Verify(x => x.GetStepId());
        huntControllerMock.Verify(x => x.MarkStepStarted(id));

        endControllerMock.Verify(x => x.Configure(endText, endVideoUrl, It.IsAny<Action>()));
        endControllerMock.Verify(x => x.GetComponentUIActions());
        endUIActions.Verify(x => x.Display());
    }
    
    [Test]
    public void TestEndStep_CallsFunction_In_HuntController()
    {
        huntControllerMock.Setup(x => x.EndHunt(true)).Verifiable();
        
        var sut = new ResolutionVideoAndEndOldStepController(endControllerMock.Object);
        
        sut.StartStep(resolutionVideoEndStepMock.Object, huntControllerMock.Object, true);
        
        sut.EndStep();
        
        huntControllerMock.Verify(x => x.EndHunt(true));

    }
}
