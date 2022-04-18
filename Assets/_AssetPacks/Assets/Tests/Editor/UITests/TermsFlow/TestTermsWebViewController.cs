using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class TestAgreementWebViewController
{
    private GameObject _go;
    private TermsWebViewController.Dependencies _dependencies;
    
    private Mock<IImageSlider> _imageSliderMock;
    private RectTransform _referenceTransformMock;

    private string url;

    public TermsWebViewController.Config CreateConfig(Action returnEvent = null)
    {
        return new TermsWebViewController.Config()
        {
            ReturnEvent = returnEvent,
            Url = url
        };
    }
    
    public TermsWebViewController.Dependencies CreateDependencies( Mock<IImageSlider> imageSlider)
    {
        _imageSliderMock = imageSlider;
        _referenceTransformMock = new GameObject().AddComponent<RectTransform>();

        return new TermsWebViewController.Dependencies()
        {
            slideComponent = _imageSliderMock.Object,
            referenceTransform = _referenceTransformMock,
        };
    }
    [SetUp]
    public void Init()
    {
        _go = new GameObject();
        url = "https://google.com";
        _dependencies = CreateDependencies(new Mock<IImageSlider>());

    }

    [TearDown]
    public void TearDown()
    {
        _go = null;
        _imageSliderMock = null;
        _dependencies = null;
    }

    [Test]
    public void TestConfigure()
    {
        //Given an agreementWebViewController
        //When configure is called
        //Then the UI is configured
        
        //Arrange
        var sut = _go.AddComponent<TermsWebViewController>();
        sut.SetDependencies(_dependencies);
        var hasBeenCalled = false;
        Action returnAction = () =>
        {
            hasBeenCalled = true;
        };
        
        var config = CreateConfig(returnAction);

        //Act
        sut.Configure(config);
        sut.ReturnButtonAction();
        
        //Assert
        Assert.IsTrue(hasBeenCalled);
    }
    
    [Test]
    public void TestOpen()
    {
        //Given a new AgreementWebViewController
        //When Open is called
        //Then Open is called in the slideComponent
        
        //Arrange
        var sut = _go.AddComponent<TermsWebViewController>();

        var imageSliderMock = new Mock<IImageSlider>();
        imageSliderMock.Setup(x => x.Open(It.IsAny<Action>())).Verifiable();
        
        _dependencies = CreateDependencies(imageSliderMock);
        sut.SetDependencies(_dependencies);
        
        //Act
        sut.Open();
        
        //Assert
        imageSliderMock.Verify(x => x.Open(It.IsAny<Action>()));
    }
    
    [Test]
    public void TestClose()
    {
        //Given a new AgreementWebViewController
        //When Close is called
        //Then Close is called in the slideComponent
        
        //Arrange
        var sut = _go.AddComponent<TermsWebViewController>();

        var imageSliderMock = new Mock<IImageSlider>();
        imageSliderMock.Setup(x => x.Close(It.IsAny<Action>())).Verifiable();
        
        _dependencies = CreateDependencies(imageSliderMock);
        sut.SetDependencies(_dependencies);
        
        //Act
        sut.Close();
        
        //Assert
        imageSliderMock.Verify(x => x.Close(It.IsAny<Action>()));
    }

    
    [TestCase(true)]
    [TestCase(false)]
    [Test]
    public void TestIsAnimating(bool isAnimating)
    {
        //Given a new agreementWebViewController
        //When IsAnimating is called
        //Then the function returns whether ImageSlider isAnimating
        
        
        //Arrange
        var sut = _go.AddComponent<TermsWebViewController>();
        
        var imageSliderMock = new Mock<IImageSlider>();
        imageSliderMock.Setup(x => x.IsAnimating()).Returns(isAnimating).Verifiable();
        _dependencies = CreateDependencies(imageSliderMock);

        sut.SetDependencies(_dependencies);
        //Act
        var value = sut.IsAnimating();
        
        //Assert
        Assert.AreEqual(isAnimating, value);
        imageSliderMock.Verify(x => x.IsAnimating());
    }
    [TestCase(true)]
    [TestCase(false)]
    [Test]
    public void TestIsOpen(bool isOpen)
    {
        //Given a new agreementWebViewController
        //When IsOpen is called
        //Then the function returns whether ImageSlider IsOpen
        
        //Arrange
        var sut = _go.AddComponent<TermsWebViewController>();
        
        var imageSliderMock = new Mock<IImageSlider>();
        imageSliderMock.Setup(x => x.IsOpen()).Returns(isOpen).Verifiable();
        _dependencies = CreateDependencies(imageSliderMock);

        sut.SetDependencies(_dependencies);
        //Act
        var value = sut.IsOpen();
        
        //Assert
        Assert.AreEqual(isOpen, value);
        imageSliderMock.Verify(x => x.IsOpen());
    }
}
