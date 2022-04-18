using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using TMPro;
using UnityEngine;

[TestFixture]
public class TestBubbleDropController
{
    private GameObject go;
    private RectTransform _rt;
    private Mock<IBubbleSlider> bubbleSliderMock;

    [SetUp]
    public void Init()
    {
        go = new GameObject();
        bubbleSliderMock = new Mock<IBubbleSlider>();
        _rt = go.AddComponent<RectTransform>();
        
    }

    [TearDown]
    public void TearDown()
    {
        go = null;
        bubbleSliderMock = null;
        _rt = null;
    }
    
    
    [Test]
    public void TestSetDependencies()
    {
        var sut = go.AddComponent<BubbleDropController>();
        var dependencies = new BubbleDropController.Dependencies()
        {
            AssociatedBubbleSlider = bubbleSliderMock.Object,
            lockType = BubbleSlideLockType.left
        };
        
        sut.Configure(dependencies);
        
        Assert.AreSame(dependencies, sut._dependencies);
    }


    [Test]
    public void TestPositionSelf_locktype_is_Left()
    {
        var sut = go.AddComponent<BubbleDropController>();
        var dependencies = new BubbleDropController.Dependencies()
        {
            AssociatedBubbleSlider = bubbleSliderMock.Object,
            lockType = BubbleSlideLockType.left
        };
        
        sut.Configure(dependencies);

        sut.PositionSelf();
        
        var rt = sut.GetRectTransform();
        
        Assert.AreEqual(new Vector2(0f,0f),rt.anchorMin);
        Assert.AreEqual(new Vector2(0f,1f),rt.anchorMax);
        Assert.AreEqual(new Vector2(0f,0.5f),rt.pivot);
        Assert.AreEqual(new Vector2(0,0),rt.offsetMin);
        var sizeDelta = rt.sizeDelta;
        Assert.AreEqual(new Vector2(164, sizeDelta.y), sizeDelta);
        Assert.AreEqual(0, rt.GetSiblingIndex());
    }
    
    [Test]
    public void TestPositionSelf_locktype_is_Right()
    {
        var sut = go.AddComponent<BubbleDropController>();
        var dependencies = new BubbleDropController.Dependencies()
        {
            AssociatedBubbleSlider = bubbleSliderMock.Object,
            lockType = BubbleSlideLockType.right
        };
        
        sut.Configure(dependencies);

        sut.PositionSelf();
        
        var rt = sut.GetRectTransform();
        
        Assert.AreEqual(new Vector2(1f,0f),rt.anchorMin);
        Assert.AreEqual(new Vector2(1f,1f),rt.anchorMax);
        Assert.AreEqual(new Vector2(1f,0.5f),rt.pivot);
        Assert.AreEqual(new Vector2(-164,0),rt.offsetMin);
        var sizeDelta = rt.sizeDelta;
        Assert.AreEqual(new Vector2(164, 0), sizeDelta);
        Assert.AreEqual(0, rt.GetSiblingIndex());
    }
    
    [Test]
    public void TestPositionSelf_lockType_is_None_Throws()
    {
        var sut = go.AddComponent<BubbleDropController>();
        var dependencies = new BubbleDropController.Dependencies()
        {
            AssociatedBubbleSlider = bubbleSliderMock.Object,
            lockType = BubbleSlideLockType.none
        };
        
        sut.Configure(dependencies);

        Assert.Throws<ArgumentException>(() => sut.PositionSelf());
    }

    [TestCase(BubbleSlideLockType.left)]
    [TestCase(BubbleSlideLockType.right)]
    [Test]
    public void TestOnDrop(BubbleSlideLockType lockType)
    {
        var sut = go.AddComponent<BubbleDropController>();
        if(lockType == BubbleSlideLockType.left)
            bubbleSliderMock.Setup(x => x.LockBubbleToLeft()).Verifiable();
        else if(lockType == BubbleSlideLockType.right)
            bubbleSliderMock.Setup(x => x.LockBubbleToRight()).Verifiable();
        var dependencies = new BubbleDropController.Dependencies()
        {
            AssociatedBubbleSlider = bubbleSliderMock.Object,
            lockType = lockType
        };
        sut.Configure(dependencies);
        
        sut.OnDrop(null);
        
        if(lockType == BubbleSlideLockType.left)
            bubbleSliderMock.Verify(x => x.LockBubbleToLeft());
        else if(lockType == BubbleSlideLockType.right)
            bubbleSliderMock.Verify(x => x.LockBubbleToRight());
    }
    
    [Test]
    public void TestDropHovering()
    {
        var sut = go.AddComponent<BubbleDropController>();
        bubbleSliderMock.Setup(x => x.StartDropHighlight()).Verifiable();
        var dependencies = new BubbleDropController.Dependencies()
        {
            AssociatedBubbleSlider = bubbleSliderMock.Object,
            lockType = BubbleSlideLockType.none
        };
        sut.Configure(dependencies);
        
        sut.DropHovering();

        bubbleSliderMock.Verify(x => x.StartDropHighlight());
    }

    [Test]
    public void TestDropStopHovering()
    {
        var sut = go.AddComponent<BubbleDropController>();
        bubbleSliderMock.Setup(x => x.StopDropHighlight()).Verifiable();
        var dependencies = new BubbleDropController.Dependencies()
        {
            AssociatedBubbleSlider = bubbleSliderMock.Object,
            lockType = BubbleSlideLockType.none
        };
        
        sut.Configure(dependencies);
        
        sut.DropStopHovering();
        bubbleSliderMock.Verify(x => x.StopDropHighlight());
    }

    [Test]
    public void TestGetRectTransform()
    {
        var sut = go.AddComponent<BubbleDropController>();
        var dependencies = new BubbleDropController.Dependencies()
        {
            AssociatedBubbleSlider = bubbleSliderMock.Object,
            lockType = BubbleSlideLockType.none
        };
        sut.Configure(dependencies);
        Assert.AreSame(_rt, sut.GetRectTransform());
    }
}
