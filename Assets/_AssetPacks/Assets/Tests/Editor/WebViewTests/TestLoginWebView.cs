using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class TestLoginWebView : MonoBehaviour
{

    private Mock<IWebviewCanvas> webviewCanvasMock;
    private Mock<ICanvasLayerManager> canvasLayerManager;

    [SetUp]
    public void Init()
    {
        webviewCanvasMock = new Mock<IWebviewCanvas>();
        canvasLayerManager = new Mock<ICanvasLayerManager>();
    }

    [TearDown]
    public void TearDown()
    {
        webviewCanvasMock = null;
        canvasLayerManager = null;
    }

    [Test]
    public void TestDisplayLogin_Calls_ConfigureAndDisplay_In_WebviewCanvas_RecievesAndUnpacks_UserCode()
    {
        //Given an active LoginWebView
        //When the user has logged in, and recieves a uniwebview message from auth0
        //Then the message is sent back into the loginhandler.
        
        //arrange
        string theMessage = "uniwebview://auth0callback?code=rL4Hf8iTa43d2Ym8#";
        string url = "url.com";
        string _message = "";
        Action<string> unpackCallback = (message) =>
        {
            _message = message;
        };
        webviewCanvasMock.Setup(x => x.ConfigureAndDisplay(It.IsAny<WebviewCanvas.Config>()))
            .Callback<WebviewCanvas.Config>((theConfig) =>
            {
                theConfig.messageAction.Invoke(new UniWebViewMessage(theMessage));
            })
            .Verifiable();
        webviewCanvasMock.Setup(x => x.RemoveSelf()).Verifiable();
        canvasLayerManager.Setup(x => x.GetWebViewCanvas()).Returns(webviewCanvasMock.Object).Verifiable();

        var sut = new LoginWebView(canvasLayerManager.Object);
        
        //Act
        //-note, the action happens in the callback which is wrapped in the "MessageAction" portion of the LoginWebView.
        sut.DisplayLogin(url, unpackCallback);
        
        //Assert
        webviewCanvasMock.Verify(x => x.ConfigureAndDisplay(It.IsAny<WebviewCanvas.Config>()));
        canvasLayerManager.Verify(x => x.GetWebViewCanvas());
        
        webviewCanvasMock.Verify(x => x.RemoveSelf());
        
        Assert.AreEqual(theMessage, _message);
    }
}
