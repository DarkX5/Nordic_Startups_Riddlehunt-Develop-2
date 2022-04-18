using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class TestProductIntroView
{
    private Mock<IStandardButton> _continueButton;
    private Mock<IStandardButton> _backButton;
    private Mock<IGenericWebView> _genericWebview;
    private Mock<ILoginHandler> _loginHandler;

    private Mock<ICanvasLayerManager> _clm;
    private Mock<ILoaderView> _loaderViewMock;
    private Mock<IVideoCanvasController> _videoCanvasController;
    private Mock<IVideoController> _videoController;
    [SetUp]
    public void Init()
    {
        _continueButton = new Mock<IStandardButton>();
        _backButton = new Mock<IStandardButton>();
        _genericWebview = new Mock<IGenericWebView>();
        _loginHandler = new Mock<ILoginHandler>();

        _clm = new Mock<ICanvasLayerManager>();
        _videoCanvasController = new Mock<IVideoCanvasController>();
        _videoController = new Mock<IVideoController>();
        _loaderViewMock = new Mock<ILoaderView>();
        _loaderViewMock.Setup(x => x.Display()).Verifiable();
        _loaderViewMock.Setup(x => x.Hide()).Verifiable();
        _videoCanvasController.Setup(x => x.GetVideoController()).Returns(_videoController.Object).Verifiable();
        
        _clm.Setup(x => x.GetVideoCanvas()).Returns(_videoCanvasController.Object).Verifiable();
        _clm.Setup(x => x.GetLoaderView()).Returns(_loaderViewMock.Object).Verifiable();
    }

    [TearDown]
    public void TearDown()
    {
        _continueButton = null;
        _backButton = null;
        _genericWebview = null;
        _loginHandler = null;
    }

    public void TestSetDependencies()
    {
        //Given a new uninitialized ProductIntroView
        //When SetDependencies is called
        //Then Dependencies are set.
        
        //Arrange
        var sut = new GameObject().AddComponent<ProductIntroView>();
        var dependencies = new ProductIntroView.Dependencies()
        {
            ContinueButton = _continueButton.Object,
            BackButton = _backButton.Object,
            GenericWebView = _genericWebview.Object,
            LoginHandler = _loginHandler.Object,
            Clm = _clm.Object
        };
        
        //Act
        sut.SetDependencies(dependencies);
        
        //Assert
        Assert.AreEqual(dependencies, sut._dependencies);
        _clm.Verify(x => x.GetVideoCanvas());
        _clm.Verify(x => x.GetLoaderView());
    }

    [Test]
    [TestCase(true, "Start")]
    [TestCase(false, "Log Ind")]
    public void TestConfigure_ConfiguresButtons(bool loggedIn, string continueText)
    {
        //Given a new initialized ProductIntroView
        //When configure is called
        //Then the individual parts are configured.
        
        //Arrange
        var url = "https://www.google.com";
        
        _genericWebview.Setup(x => x.Prepare()).Verifiable();

        Action backAction = () => { };
        Action continueAction = () => { };

        _backButton.Setup(x=> x.Configure("Tilbage", backAction)).Verifiable();
        
        if (loggedIn)
            _continueButton.Setup(x=> x. Configure(continueText, It.IsAny<Action>())).Verifiable();
        else
            _continueButton.Setup(x=> x. Configure(continueText, _loginHandler.Object.LoginAction)).Verifiable();

        _loginHandler.Setup(x => x.IsLoggedInAsUser()).Returns(loggedIn).Verifiable();
        
        var sut = new GameObject().AddComponent<ProductIntroView>();
        var dependencies = new ProductIntroView.Dependencies()
        {
            ContinueButton = _continueButton.Object,
            BackButton = _backButton.Object,
            GenericWebView = _genericWebview.Object,
            LoginHandler = _loginHandler.Object,
            Clm = _clm.Object
        };
        sut.SetDependencies(dependencies);

        var config = new ProductIntroView.Config()
        {
            Url = url,
            BackAction = backAction,
            ContinueAction = continueAction
        };
        //Act
        sut.Configure(config);
        
        //Assert
        _genericWebview.Verify(x => x.Prepare());

        _backButton.Verify(x => x.Configure("Tilbage", backAction));
        if (loggedIn)
            _continueButton.Verify(x => x.Configure(continueText, It.IsAny<Action>()));
        else
            _continueButton.Verify(x => x.Configure(continueText, _loginHandler.Object.LoginAction));
    }
}
