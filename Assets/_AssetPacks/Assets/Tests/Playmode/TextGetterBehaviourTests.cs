using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TextGetterBehaviourTests
{
    [UnityTest]
    public IEnumerator TextGetter_DownloadTextFile_Text_Is_TextFile()
    {
        //Given a link to a text file on the server
        //When the textgetter is initiated with the link
        //The the content of that file is returned.
        
        var textGetter = new MonoBehaviourTest<TextGetterDownloadTest>();
        yield return textGetter;
        Assert.AreEqual("TextFile", textGetter.component.DownloadedText);
    }
}

public class TextGetterDownloadTest : MonoBehaviour, IMonoBehaviourTest
{
    public bool IsTestFinished { get; set; }
    public string DownloadedText;
    private TextGetter _textGetter;
    public void Awake() //this is the main where the test is run.
    {
        _textGetter = TextGetter.Factory(this);
        _textGetter.GetText(
            "https://cdn.escaperoom-riddlehouse.dk/riddlehunt/unity_getter_test_files/Text/TextFile.txt", 
            false, 
            SetText);
    }

    private void SetText(string downloadedText) //end test and clean up
    {
        DownloadedText = downloadedText;
        _textGetter.DisposeSelf();
        IsTestFinished = true;
    }
}
