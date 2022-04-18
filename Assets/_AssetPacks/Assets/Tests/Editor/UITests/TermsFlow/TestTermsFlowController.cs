using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class TestTermsAndConditionsFlowController
{
    private GameObject go;
    private Mock<IImageFaderActions> imageFaderMock;
    private Mock<ITermsAcceptanceController> acceptanceControllerMock;
    private Mock<ITermsWebviewController> webviewControllerMock;
    private TermsFlowController.Dependencies dependencies;
    public TermsFlowController.Dependencies CreateDepencencies(Mock<IImageFaderActions> faderMock, Mock<ITermsAcceptanceController> acceptanceMock, Mock<ITermsWebviewController> webviewController)
    {
        return new TermsFlowController.Dependencies()
        {
            _fadeController = faderMock.Object,
            _acceptanceController = acceptanceMock.Object,
            _webviewController = webviewControllerMock.Object
        };
    }

    public TermsFlowController.Config CreateConfig(List<TermsInfo> pendingAgreements, Action<bool> flowComplete)
    {
        return new TermsFlowController.Config()
        {
            PendingAgreements = pendingAgreements,
            FlowComplete = flowComplete
        };
    }
    
    [SetUp]
    public void Init()
    {
        go = new GameObject();
        imageFaderMock = new Mock<IImageFaderActions>();
        acceptanceControllerMock = new Mock<ITermsAcceptanceController>();
        webviewControllerMock = new Mock<ITermsWebviewController>();

        dependencies = CreateDepencencies(imageFaderMock, acceptanceControllerMock, webviewControllerMock);
    }
    
    [TearDown]
    public void TearDown()
    {
        go = null;
        imageFaderMock = null;
        acceptanceControllerMock = null;
        webviewControllerMock = null;
    }

    [Test]
    public void TestConfigure()
    {
        //Arrange
        var pendingAgreements = new List<TermsInfo>()
        {
            new TermsInfo()
            {
                Id = "id",
                Reason = "description",
                Title = "title",
                Url = "https://google.com"
            }
        };
        var config = CreateConfig(pendingAgreements, null);

        imageFaderMock.Setup(x => x.IsAnimating()).Returns(false);
        acceptanceControllerMock.Setup(x => x.Configure(It.IsAny<TermsAcceptanceController.Config>())).Verifiable();
        webviewControllerMock.Setup(x => x.Configure(It.IsAny<TermsWebViewController.Config>())).Verifiable();

        dependencies = CreateDepencencies(imageFaderMock, acceptanceControllerMock, webviewControllerMock);

        var sut = go.AddComponent<TermsFlowController>();
        sut.SetDependencies(dependencies);
        
        //Act
        sut.Configure(config);
        
        //Assert
        acceptanceControllerMock.Verify(x => x.Configure(It.IsAny<TermsAcceptanceController.Config>()));
        webviewControllerMock.Verify(x => x.Configure(It.IsAny<TermsWebViewController.Config>()));
    }

    [Test]
    public void TestOpen()
    {
        imageFaderMock.Setup(x => x.IsAnimating()).Returns(false).Verifiable();
        
        imageFaderMock.Setup(x => x.Open(It.IsAny<Action>())).Verifiable();
        acceptanceControllerMock.Setup(x => x.Open(It.IsAny<Action>())).Verifiable();
        
        dependencies = CreateDepencencies(imageFaderMock, acceptanceControllerMock, webviewControllerMock);

        var sut = go.AddComponent<TermsFlowController>();
        sut.SetDependencies(dependencies);
        
        //Act
        sut.Open();
        //Assert
        imageFaderMock.Verify(x => x.IsAnimating());
        imageFaderMock.Verify(x => x.Open(It.IsAny<Action>()));
        acceptanceControllerMock.Verify(x => x.Open(It.IsAny<Action>()));
    }

    [Test]
    public void TestClose()
    {
        imageFaderMock.Setup(x => x.IsAnimating()).Returns(false);
        imageFaderMock.Setup(x => x.IsOpen()).Returns(true);
        imageFaderMock.Setup(x => x.Close(It.IsAny<Action>())).Verifiable();
        
        acceptanceControllerMock.Setup(x => x.IsAnimating()).Returns(false);
        acceptanceControllerMock.Setup(x => x.IsOpen()).Returns(true);
        acceptanceControllerMock.Setup(x => x.Close(It.IsAny<Action>())).Verifiable();

        webviewControllerMock.Setup(x => x.IsAnimating()).Returns(false);
        webviewControllerMock.Setup(x => x.IsOpen()).Returns(true);
        webviewControllerMock.Setup(x => x.Close(It.IsAny<Action>())).Verifiable();

        dependencies = CreateDepencencies(imageFaderMock, acceptanceControllerMock, webviewControllerMock);

        var sut = go.AddComponent<TermsFlowController>();
        sut.SetDependencies(dependencies);

        //Act
        sut.Close();
        //Assert
        imageFaderMock.Verify(x => x.Close(It.IsAny<Action>()));
        acceptanceControllerMock.Verify(x => x.Close(It.IsAny<Action>()));
        webviewControllerMock.Verify(x => x.Close(It.IsAny<Action>()));
    }
}
