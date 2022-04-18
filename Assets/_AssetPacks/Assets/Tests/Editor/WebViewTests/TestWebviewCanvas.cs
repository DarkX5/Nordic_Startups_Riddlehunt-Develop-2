using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;

public class TestWebviewCanvas 
{
    [Test]
    public void TestConfigureAndDisplay_NoWebviewAvailable_CreatesInstance()
    {
        string url = "https://url.com";

        Mock<IGenericWebView> genericWebview = new Mock<IGenericWebView>();
        genericWebview.Setup(x => x.Load(url)).Verifiable();
        genericWebview.Setup(x => x.Open()).Verifiable();
        Mock<ICreateGenericWebView> webviewCreatorMock = new Mock<ICreateGenericWebView>();

        webviewCreatorMock.Setup(x => x.Create()).Returns(genericWebview.Object).Verifiable();
        var sut = new GameObject().AddComponent<WebviewCanvas>();
        sut.SetDependencies(new WebviewCanvas.Dependencies()
        {
            WebViewCreator = webviewCreatorMock.Object
        });
        
        sut.ConfigureAndDisplay(new WebviewCanvas.Config()
        {
           Url = "https://url.com"
        });
        
        webviewCreatorMock.Verify(x => x.Create(), Times.Once);
        genericWebview.Verify(x => x.Load(url), Times.Once);
        genericWebview.Verify(x => x.Open(), Times.Once);
    }
    
    [Test] //TODO: missing test - test for subscribing if the actions aren't null, and not subscribing if they are null.
    public void TestConfigureAndDisplay_WebviewAvailable_Reuses()
    {
        string url = "https://url.com";
        string url2 = "https://url2.com";

        Mock<IGenericWebView> genericWebview = new Mock<IGenericWebView>();
        genericWebview.Setup(x => x.Load(url)).Verifiable();
        genericWebview.Setup(x => x.Load(url2)).Verifiable();
        genericWebview.Setup(x => x.Open()).Verifiable();
        Mock<ICreateGenericWebView> webviewCreatorMock = new Mock<ICreateGenericWebView>();

        webviewCreatorMock.Setup(x => x.Create()).Returns(genericWebview.Object).Verifiable();
        var sut = new GameObject().AddComponent<WebviewCanvas>();
        sut.SetDependencies(new WebviewCanvas.Dependencies()
        {
            WebViewCreator = webviewCreatorMock.Object
        });
        
        sut.ConfigureAndDisplay(new WebviewCanvas.Config()
        {
            Url = url
        });
        sut.ConfigureAndDisplay(new WebviewCanvas.Config()
        {
            Url = url2
        });
        webviewCreatorMock.Verify(x => x.Create(), Times.Once);
        genericWebview.Verify(x => x.Load(url), Times.Once);
        genericWebview.Verify(x => x.Load(url2), Times.Once);

        genericWebview.Verify(x => x.Open(), Times.Exactly(2));
    }

    [Test]
    public void TestHide_ConfigAndWebviewExists_Unsubs_GameObject_Inactive()
    {
        string url = "https://url.com";

        Mock<IGenericWebView> genericWebview = new Mock<IGenericWebView>();
        genericWebview.Setup(x => x.UnsubscribeToCloseEvent(It.IsAny<Action<UniWebView>>())).Verifiable();
        genericWebview.Setup(x => x.UnsubscribeToMessages(It.IsAny<Action<UniWebViewMessage>>())).Verifiable();

        Mock<ICreateGenericWebView> webviewCreatorMock = new Mock<ICreateGenericWebView>();
        webviewCreatorMock.Setup(x => x.Create()).Returns(genericWebview.Object).Verifiable();

        var sut = new GameObject().AddComponent<WebviewCanvas>();

        sut.SetDependencies(new WebviewCanvas.Dependencies()
        {
            WebViewCreator = webviewCreatorMock.Object
        });
        
        sut.ConfigureAndDisplay(new WebviewCanvas.Config()
        {
            Url = url,
            closeAction = (view) => {},
            messageAction = (message) => {}
        });
        
        sut.Hide();
        
        Assert.IsFalse(sut.gameObject.activeSelf);
        genericWebview.Verify(x => x.UnsubscribeToCloseEvent(It.IsAny<Action<UniWebView>>()));
        genericWebview.Verify(x => x.UnsubscribeToMessages(It.IsAny<Action<UniWebViewMessage>>()));
    }
}
