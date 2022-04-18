using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TMPro;
using UnityEngine;

[TestFixture]
public class TestAgreementAcceptanceController
{
    private GameObject _go;
    private TermsAcceptanceController.Dependencies _dependencies;
    
    private Mock<IImageSlider> _imageSliderMock;
    private Mock<ILayoutElementVerticalResizer> _verticalResizerMock;
    private TextMeshProUGUI _titleFieldMock;
    private TextMeshProUGUI _descriptionFieldMock;

    private string _title;
    private string _description;

    public TermsAcceptanceController.Config CreateConfig(
        Action readAgreementEvent = null, 
        Action acceptAgreementEvent = null, 
        Action declineAgreementEvent = null
        )
    {
        return new TermsAcceptanceController.Config()
        {
            ReadAgreementEvent = readAgreementEvent,
            AcceptAgreementEvent = acceptAgreementEvent,
            DeclineAgreementEvent = declineAgreementEvent,
            Title = _title,
            Reason = _description
        };
    }

    public TermsAcceptanceController.Dependencies CreateDependencies( Mock<IImageSlider> imageSlider, Mock<ILayoutElementVerticalResizer> verticalResizer)
    {
        _imageSliderMock = imageSlider;
        _verticalResizerMock = verticalResizer;
        _titleFieldMock = new GameObject().AddComponent<TextMeshProUGUI>();
        _descriptionFieldMock = new GameObject().AddComponent<TextMeshProUGUI>();

        return new TermsAcceptanceController.Dependencies()
        {
            SlideComponent = _imageSliderMock.Object,
            TitleField = _titleFieldMock,
            DescriptionField = _descriptionFieldMock,
            VerticalResizer = _verticalResizerMock.Object
        };
    }
    
    [SetUp]
    public void Init()
    {
        _go = new GameObject();
        _title = "title";
        _description = "description";
       _dependencies = CreateDependencies(new Mock<IImageSlider>(), new Mock<ILayoutElementVerticalResizer>());

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
        //Given a new AgreementAcceptanceController
        //When the configure is called
        //The UI is configured
        
        //Arrange
        var sut = _go.AddComponent<TermsAcceptanceController>();
        var imageSliderMock = new Mock<IImageSlider>();

        var verticalResizerMock = new Mock<ILayoutElementVerticalResizer>();
        verticalResizerMock.Setup(x => x.StartUIUpdate()).Verifiable();
        _dependencies = CreateDependencies(imageSliderMock, verticalResizerMock);
        sut.SetDependencies(_dependencies);
        var config = CreateConfig();
        //Act
        sut.Configure(config);
        
        //Assert
        Assert.AreEqual(_title, _titleFieldMock.text);
        Assert.AreEqual(_description, _descriptionFieldMock.text);
        verticalResizerMock.Verify(x => x.StartUIUpdate());
    }
    
    [Test]
    public void TestReadAgreement()
    {
        //Given a configured AgreementAcceptanceController
        //When the readAgreement is called
        //The readAgreementEvent is called
        
        //Arrange
        var hasBeenCalled = false;
        Action readAgreementEvent = () =>
        {
            hasBeenCalled = true;
        };
        var sut = _go.AddComponent<TermsAcceptanceController>();
        sut.SetDependencies(_dependencies);
        var config = CreateConfig(readAgreementEvent, null, null);
        sut.Configure(config);

        //Act
        sut.ReadAgreement();
        
        //Assert
        Assert.IsTrue(hasBeenCalled);
    }

    [Test]
    public void TestAcceptAgreement()
    {
        //Given a configured AgreementAcceptanceController
        //When the acceptAgreement is called
        //The acceptAgreementEvent is called
        
        //Arrange
        var hasBeenCalled = false;
        Action acceptAgreementEvent = () =>
        {
            hasBeenCalled = true;
        };
        var sut = _go.AddComponent<TermsAcceptanceController>();
        sut.SetDependencies(_dependencies);
        var config = CreateConfig(null, acceptAgreementEvent, null);
        sut.Configure(config);

        //Act
        sut.AcceptAgreement();
        
        //Assert
        Assert.IsTrue(hasBeenCalled);
    }

    [Test]
    public void TestDeclineAgreement()
    {
        //Given a configured AgreementAcceptanceController
        //When the declineAgreement is called
        //The declineAgreementEvent is called
        
        //Arrange
        var hasBeenCalled = false;
        Action declineAgreementEvent = () =>
        {
            hasBeenCalled = true;
        };
        var sut = _go.AddComponent<TermsAcceptanceController>();
        sut.SetDependencies(_dependencies);
        var config = CreateConfig(null, null, declineAgreementEvent);
        sut.Configure(config);

        //Act
        sut.DeclineAgreement();
        
        //Assert
        Assert.IsTrue(hasBeenCalled);
    }
    
    [Test]
    public void TestOpen()
    {
        //Given a new AgreementAcceptanceController
        //When Open is called
        //Then Open is called in the slideComponent
        
        //Arrange
        var sut = _go.AddComponent<TermsAcceptanceController>();

        var imageSliderMock = new Mock<IImageSlider>();
        imageSliderMock.Setup(x => x.Open(It.IsAny<Action>())).Verifiable();

        var verticalResizerMock = new Mock<ILayoutElementVerticalResizer>();
        
        _dependencies = CreateDependencies(imageSliderMock, verticalResizerMock);
        sut.SetDependencies(_dependencies);
        
        //Act
        sut.Open();
        
        //Assert
        imageSliderMock.Verify(x => x.Open(It.IsAny<Action>()));
    }
    
    [Test]
    public void TestClose()
    {
        //Given a new AgreementAcceptanceController
        //When Close is called
        //Then Close is called in the slideComponent
        
        //Arrange
        var sut = _go.AddComponent<TermsAcceptanceController>();

        var imageSliderMock = new Mock<IImageSlider>();
        imageSliderMock.Setup(x => x.Close(It.IsAny<Action>())).Verifiable();
        
        var verticalResizerMock = new Mock<ILayoutElementVerticalResizer>();
        
        _dependencies = CreateDependencies(imageSliderMock, verticalResizerMock);
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
        //Given a new AgreementAcceptanceController
        //When IsAnimating is called
        //Then the function returns whether ImageSlider isAnimating
        
        //Arrange
        var sut = _go.AddComponent<TermsAcceptanceController>();

        var imageSliderMock = new Mock<IImageSlider>();
        imageSliderMock.Setup(x => x.IsAnimating()).Returns(isAnimating).Verifiable();
        
        var verticalResizerMock = new Mock<ILayoutElementVerticalResizer>();
        
        _dependencies = CreateDependencies(imageSliderMock, verticalResizerMock);
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
        //Given a new AgreementAcceptanceController
        //When IsOpen is called
        //Then the function returns whether ImageSlider is open
        
        //Arrange
        var sut = _go.AddComponent<TermsAcceptanceController>();

        var imageSliderMock = new Mock<IImageSlider>();
        imageSliderMock.Setup(x => x.IsOpen()).Returns(isOpen).Verifiable();
        
        var verticalResizerMock = new Mock<ILayoutElementVerticalResizer>();
        
        _dependencies = CreateDependencies(imageSliderMock, verticalResizerMock);
        sut.SetDependencies(_dependencies);
        
        //Act
        var value = sut.IsOpen();
        
        //Assert
        Assert.AreEqual(isOpen, value);
        imageSliderMock.Verify(x => x.IsOpen());
    }
}
