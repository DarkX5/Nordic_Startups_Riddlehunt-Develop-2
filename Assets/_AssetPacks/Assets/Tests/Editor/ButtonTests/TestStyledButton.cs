using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSelection.View;
using Components.Buttons;
using NUnit.Framework;
using riddlehouse_libraries.products;
using riddlehouse_libraries.products.resources;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[TestFixture]
public class TestStyledButton
{
    private StyledButton.Config _config;

    [SetUp]
    public void Init()
    {

        _config = new StyledButton.Config()
        {
            Resource = new StyledButtonComponentResource()
            {
                ButtonColor =  RHColor.Black,
                ButtonText = "text",
                ButtonTextColor = RHColor.White
            }
        };
    }

    [TearDown]
    public void TearDown()
    {
        _config = null;
    }
    private StyledButton.Dependencies CreateDependencies()
    {
        return new StyledButton.Dependencies()
        {
            Button = new GameObject().AddComponent<Button>(),
            ButtonBackground = new GameObject().AddComponent<Image>(),
            ButtonText = new GameObject().AddComponent<TextMeshProUGUI>()
        };
    }
    [Test]
    public void TestSetDependencies_SetsClassDependencies()
    {
        //Given a newly created StyledButton
        //When SetDependencies is called with an object
        //Then those dependencies are mapped internally in the SUT.
        
        //Arrange
        var sut = new GameObject().AddComponent<StyledButton>();
        var dependencies = CreateDependencies();
        //Act
        sut.SetDependencies(dependencies);
        //Assert
        Assert.AreEqual(dependencies, sut._dependencies);
    }

    [Test]
    public void TestConfigure_SetsValuesInDependencies()
    {
        //Given a new initialized StyledButton
        //When Configure is called
        //Then the buttontext and background is set
        
        var sut = new GameObject().AddComponent<StyledButton>();
        var dependencies = CreateDependencies();
        sut.SetDependencies(dependencies);

        //Act
        sut.Configure(_config);
        
        //Assert
        Assert.AreEqual(_config.Resource.ButtonColor.R, ((Color32)dependencies.ButtonBackground.color).r);
        Assert.AreEqual(_config.Resource.ButtonColor.G, ((Color32)dependencies.ButtonBackground.color).g);
        Assert.AreEqual(_config.Resource.ButtonColor.B, ((Color32)dependencies.ButtonBackground.color).b);
        Assert.AreEqual(_config.Resource.ButtonColor.A, ((Color32)dependencies.ButtonBackground.color).a);

        Assert.AreEqual(_config.Resource.ButtonText, dependencies.ButtonText.text);
        Assert.AreEqual(_config.Resource.ButtonTextColor.R, ((Color32)dependencies.ButtonText.color).r);
        Assert.AreEqual(_config.Resource.ButtonTextColor.G, ((Color32)dependencies.ButtonText.color).g);
        Assert.AreEqual(_config.Resource.ButtonTextColor.B, ((Color32)dependencies.ButtonText.color).b);
        Assert.AreEqual(_config.Resource.ButtonTextColor.A, ((Color32)dependencies.ButtonText.color).a);
    }
    [TestCase(false)]
    [TestCase(true)]
    [Test]
    public void TestSetInteractable_SetsTheButtonInteractableValue_ToInput(bool input)
    {
        //Given a new initialied StyledButton
        //When SetInteractable is called
        //Then the Button.Interactable is set to value of input.
        
        var sut = new GameObject().AddComponent<StyledButton>();
        var dependencies = CreateDependencies();
        sut.SetDependencies(dependencies);

        //Act
        sut.SetInteractable(input);
        
        //Assert
        Assert.AreEqual(input, dependencies.Button.interactable);
    }
    
    [Test]
    public void TestSetInteractable_SetsTheButtonInteractableValue_ToInput()
    {
        //Given a new initialied StyledButton
        //When SetInteractable is called
        //Then the Button.Interactable is set to value of input.
        
        var sut = new GameObject().AddComponent<StyledButton>();
        var dependencies = CreateDependencies();
        sut.SetDependencies(dependencies);

        bool hasBeenCalled = false;
        Action buttonAction = () => { hasBeenCalled = true; };
        _config.Action = buttonAction;
        sut.Configure(_config);
        
        //Act
        sut.PerformAction();
        
        //Assert
        Assert.IsTrue(hasBeenCalled);
    }
}
