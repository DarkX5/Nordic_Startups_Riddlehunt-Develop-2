using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class QuestionnaireWebview : MonoBehaviour
{
    [Inject] private CanvasLayerManager _clm;
    private IWebviewCanvas _webviewCanvas;
    public void DisplayQuestionnaire(string url)
    {
        _webviewCanvas = _clm.GetWebViewCanvas();
        _webviewCanvas.ConfigureAndDisplay(new WebviewCanvas.Config()
        {
            Url = url,
            closeAction = CloseAction,
            messageAction = MessageAction
        });
    }

    void CloseAction(UniWebView view)
    {
        Debug.Log("Should Close");
        _webviewCanvas.Hide();
    }

    void MessageAction(UniWebViewMessage message)
    {
        Debug.Log("Message recieved");
        if (message.Path.Equals("feedback")) {
            _webviewCanvas.Hide();
        }
    }
}
