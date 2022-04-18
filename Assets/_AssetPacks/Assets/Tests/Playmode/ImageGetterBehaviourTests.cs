using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ImageGetterBehaviourTests
{
    [UnityTest]
    public IEnumerator ImageGetter_DownloadImageFile_Sprite_Is_NotNull()
    {
        //Given a link to an image file on the server
        //When the imageGetter GetImage is called with the link
        //The the content of that file is downloaded, packed in a sprite and returned.
        
        var textGetter = new MonoBehaviourTest<ImageGetterDownloadTest>();
        yield return textGetter;
        Assert.IsNotNull(textGetter.component.DownloadedImage);
    }
}
public class ImageGetterDownloadTest : MonoBehaviour, IMonoBehaviourTest
{
    public bool IsTestFinished { get; set; }
    public Sprite DownloadedImage;
    private ImageGetter _imageGetter;
    public void Awake() //this is the main where the test is run.
    {
        _imageGetter = ImageGetter.Factory(this);
        _imageGetter.GetImage(
            "https://cdn.escaperoom-riddlehouse.dk/riddlehunt/unity_getter_test_files/Image/image.jpg", 
            false, 
            SetImage);
    }

    private void SetImage(Sprite image) //end test and clean up
    {
        DownloadedImage = image;
        _imageGetter.DisposeSelf();
        IsTestFinished = true;
    }
}