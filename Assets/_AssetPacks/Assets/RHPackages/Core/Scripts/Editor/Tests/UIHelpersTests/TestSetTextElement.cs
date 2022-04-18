using NUnit.Framework;
using RHPackages.Core.Scripts.UIHelpers;
using UnityEngine;

public class TestSetTextElement
{
    [Test]
    public void TestConfigure()
    {
        //Given a SetTextElement
        //When Configure is called with a string variable
        //Then that text is inserted into the textfield.
        string text = "dfksmdlfsndkfjdnfkjn";
        var sut = new GameObject().AddComponent<SetTextElement>();
        sut.Configure(text);
        Assert.IsNotNull(sut.TextField);
        Assert.AreEqual(text, sut.TextField.text);
    }
}
