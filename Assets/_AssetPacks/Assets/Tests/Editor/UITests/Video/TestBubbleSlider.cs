using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;

[TestFixture]
public class TestBubbleSlider : MonoBehaviour
{
    private GameObject canvasGameObject;
    private Canvas rootCanvas;
    
    private GameObject go;
    private CanvasGroup cg;
    private RectTransform targetTransform;
    private RectTransform leftLockPoint;
    private RectTransform rightLockPoint;

    private Mock<IBubbleSliderTransitions> bubbleSliderTransitionsMock;
    
    [SetUp]
    public void Init()
    {
        go = new GameObject();
        targetTransform = go.AddComponent<RectTransform>();
        leftLockPoint = new GameObject().AddComponent<RectTransform>();
        rightLockPoint = new GameObject().AddComponent<RectTransform>();
        cg = go.AddComponent<CanvasGroup>();

        canvasGameObject = new GameObject();
        rootCanvas = canvasGameObject.AddComponent<Canvas>();

        bubbleSliderTransitionsMock = new Mock<IBubbleSliderTransitions>();
        
        var sut = go.AddComponent<BubbleSlider>();
        sut.SetDependencies(new BubbleSlider.Dependencies()
        {
            LeftLockPoint = leftLockPoint,
            RightLockPoint = rightLockPoint,
            TargetTransform = targetTransform,
            Ac = AnimationCurve.Linear(0,0,1,1),
            Cg = cg
        });
        sut.Configure(rootCanvas);
    }

    [TearDown]
    public void TearDown()
    {
        go = null;
        targetTransform = null;
    }

    [Test]
    public void TestSetDependencies()
    {
        var sut = go.AddComponent<BubbleSlider>();
        var dependencies = new BubbleSlider.Dependencies()
        {
            LeftLockPoint = leftLockPoint,
            RightLockPoint = rightLockPoint,
            TargetTransform = targetTransform,
            Ac = AnimationCurve.Linear(0, 0, 1, 1),
            Cg = cg
        };
        
        sut.SetDependencies(dependencies);
        
        Assert.AreSame(dependencies, sut._dependencies);
    }
    
    [Test]
    public void TestConfigure()
    {
        var sut = go.AddComponent<BubbleSlider>();
        sut.SetDependencies(new BubbleSlider.Dependencies()
        {
            LeftLockPoint = leftLockPoint,
            RightLockPoint = rightLockPoint,
            TargetTransform = targetTransform,
            Ac = AnimationCurve.Linear(0,0,1,1),
            Cg = cg
        });
        
        sut.Configure(rootCanvas);
        
        Assert.AreSame(rootCanvas, sut.ViewCanvas);
    }

    [Test]
    public void TestLockBubbleToLeft()
    {
        var sut = go.GetComponent<BubbleSlider>();
        var expectedWidth =  sut._dependencies.LeftLockPoint.rect.width - sut._dependencies.TargetTransform.rect.height;
        var expectedLockPoint = new Vector2(expectedWidth, sut._dependencies.TargetTransform.anchoredPosition.y);
        
        bubbleSliderTransitionsMock.Setup(x => 
            x.StartTransition(It.IsAny<RectTransform>(), It.IsAny<Vector2>(), It.IsAny<Action>()))
            .Callback<RectTransform, Vector2, Action>((theTarget, thePoint, theAction) =>
            {
                theAction.Invoke();
                Assert.AreEqual(expectedLockPoint, thePoint);
                Assert.AreSame(sut._dependencies.TargetTransform, theTarget);
            }).Verifiable();
        
        sut.SetDependencies(new BubbleSlider.Dependencies()
        {
            Transitions = bubbleSliderTransitionsMock.Object,
            LeftLockPoint = leftLockPoint,
            RightLockPoint = rightLockPoint,
            TargetTransform = targetTransform,
            Ac = AnimationCurve.Linear(0,0,1,1),
            Cg = cg
        });

        sut.LockBubbleToLeft();
        
        Assert.AreEqual(BubbleSlideLockType.left, sut.GetLockType());
        bubbleSliderTransitionsMock.Verify(x => x.StartTransition(It.IsAny<RectTransform>(), It.IsAny<Vector2>(), It.IsAny<Action>()));
    }

    [Test]
    public void TestLockBubbleToRight()
    {
        var sut = go.GetComponent<BubbleSlider>();
        var expectedWidth =  sut._dependencies.RightLockPoint.rect.width - sut._dependencies.TargetTransform.rect.height;
        var expectedLockPoint = new Vector2(expectedWidth, sut._dependencies.TargetTransform.anchoredPosition.y);
        
        bubbleSliderTransitionsMock.Setup(x => 
                x.StartTransition(It.IsAny<RectTransform>(), It.IsAny<Vector2>(), It.IsAny<Action>()))
            .Callback<RectTransform, Vector2, Action>((theTarget, thePoint, theAction) =>
            {
                theAction.Invoke();
                Assert.AreEqual(expectedLockPoint, thePoint);
                Assert.AreSame(sut._dependencies.TargetTransform, theTarget);
            }).Verifiable();
        
        sut.SetDependencies(new BubbleSlider.Dependencies()
        {
            Transitions = bubbleSliderTransitionsMock.Object,
            LeftLockPoint = leftLockPoint,
            RightLockPoint = rightLockPoint,
            TargetTransform = targetTransform,
            Ac = AnimationCurve.Linear(0,0,1,1),
            Cg = cg
        });

        sut.LockBubbleToRight();
        
        Assert.AreEqual(BubbleSlideLockType.right, sut.GetLockType());
        bubbleSliderTransitionsMock.Verify(x => x.StartTransition(It.IsAny<RectTransform>(), It.IsAny<Vector2>(), It.IsAny<Action>()));
    }
    
    [TestCase(BubbleSlideLockType.left)]
    [TestCase(BubbleSlideLockType.right)]
    [TestCase(BubbleSlideLockType.none)]
    [Test]
    public void TestOpenBubble(BubbleSlideLockType expectedLockType)
    {
        var expectedTimesTransitionCalled = expectedLockType == BubbleSlideLockType.none ? 0 : 2;
        var sut = go.GetComponent<BubbleSlider>();
        
        var expectedLockPoint = new Vector2(0f, 0f);
        
        bubbleSliderTransitionsMock.Setup(x => 
                x.StartTransition(It.IsAny<RectTransform>(), It.IsAny<Vector2>(), It.IsAny<Action>()))
            .Callback<RectTransform, Vector2, Action>((theTarget, thePoint, theAction) =>
            {
                theAction.Invoke();
                Assert.AreEqual(expectedLockPoint, thePoint);
                Assert.AreSame(sut._dependencies.TargetTransform, theTarget);
            }).Verifiable();
        
        sut.SetDependencies(new BubbleSlider.Dependencies()
        {
            Transitions = bubbleSliderTransitionsMock.Object,
            LeftLockPoint = leftLockPoint,
            RightLockPoint = rightLockPoint,
            TargetTransform = targetTransform,
            Ac = AnimationCurve.Linear(0,0,1,1),
            Cg = cg
        });

        if(expectedLockType == BubbleSlideLockType.left)
            sut.LockBubbleToLeft();
        else if(expectedLockType == BubbleSlideLockType.right)
            sut.LockBubbleToRight();
        
        sut.OpenBubble();
        
        Assert.AreEqual(BubbleSlideLockType.none, sut.GetLockType());
        bubbleSliderTransitionsMock.Verify(x => x.StartTransition(It.IsAny<RectTransform>(), It.IsAny<Vector2>(), It.IsAny<Action>()), Times.Exactly(expectedTimesTransitionCalled));
    }

    [Test]
    public void TestUpdate_IsHighlighting_ScaleIsSmallest()
    {
        var sut = go.GetComponent<BubbleSlider>();
        
        sut.SetDependencies(new BubbleSlider.Dependencies()
        {
            LeftLockPoint = leftLockPoint,
            RightLockPoint = rightLockPoint,
            TargetTransform = targetTransform,
            Ac = AnimationCurve.Linear(0,0,1,1),
            Cg = cg
        });

        sut.StartDropHighlight();
        
        for (int i = 0; i < 200; i++)
        {
            sut.Update();
        }
        
        Assert.IsTrue(Math.Abs(sut._dependencies.TargetTransform.localScale.x - 0.8f) > 0.05f);
    }
    
    [Test]
    public void TestUpdate_IsHighlighting_ScaleIsBiggest()
    {
        var sut = go.GetComponent<BubbleSlider>();
        
        sut.SetDependencies(new BubbleSlider.Dependencies()
        {
            LeftLockPoint = leftLockPoint,
            RightLockPoint = rightLockPoint,
            TargetTransform = targetTransform,
            Ac = AnimationCurve.Linear(0,0,1,1),
            Cg = cg
        });
        
        sut.StartDropHighlight();
        for (int i = 0; i < 200; i++)
        {
            sut.Update();
        }

        sut.StopDropHighlight();
        for (int i = 0; i < 200; i++)
        {
            sut.Update();
        }
        
        Assert.IsTrue(Math.Abs(sut._dependencies.TargetTransform.localScale.x - 1f) == 0);
    }

    [Test]
    public void TestOnBeginDrag()
    {
        var sut = go.GetComponent<BubbleSlider>();
        
        sut.SetDependencies(new BubbleSlider.Dependencies()
        {
            LeftLockPoint = leftLockPoint,
            RightLockPoint = rightLockPoint,
            TargetTransform = targetTransform,
            Ac = AnimationCurve.Linear(0,0,1,1),
            Cg = cg
        });
        
        sut.OnBeginDrag(null);
        
        Assert.AreEqual(BubbleSlideLockType.none, sut.GetLockType());
        Assert.IsFalse(cg.blocksRaycasts);
    }

    
    [Test]
    public void TestOnEndDrag()
    {
        var sut = go.GetComponent<BubbleSlider>();
        
        sut.SetDependencies(new BubbleSlider.Dependencies()
        {
            LeftLockPoint = leftLockPoint,
            RightLockPoint = rightLockPoint,
            TargetTransform = targetTransform,
            Ac = AnimationCurve.Linear(0,0,1,1),
            Cg = cg
        });
        
        sut.OnBeginDrag(null);
        
        Assert.AreEqual(BubbleSlideLockType.none, sut.GetLockType());
        Assert.IsFalse(cg.blocksRaycasts);
    }

    [Test]
    public void TestOnDrag()
    {
        rootCanvas.scaleFactor = 1f;
        var eventData = new PointerEventData(EventSystem.current);
        eventData.delta = Vector2.one;
        
        var sut = go.GetComponent<BubbleSlider>();
        
        sut.SetDependencies(new BubbleSlider.Dependencies()
        {
            LeftLockPoint = leftLockPoint,
            RightLockPoint = rightLockPoint,
            TargetTransform = targetTransform,
            Ac = AnimationCurve.Linear(0,0,1,1),
            Cg = cg
        });
        
        sut.Configure(rootCanvas);
        var expectedValue = sut._dependencies.TargetTransform.anchoredPosition + eventData.delta / rootCanvas.scaleFactor;
        sut.OnDrag(eventData);
        Assert.AreEqual(expectedValue, sut._dependencies.TargetTransform.anchoredPosition);
    }
}
