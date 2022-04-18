using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using riddlehouse_libraries.environments.Models;
using Riddlehunt.Beta.Environment.Controls;
using TMPro;
using UnityEngine;

[TestFixture]
public class TestEnvironmentTargetControls
{
    private TextMeshProUGUI _label;
    [SetUp]
    public void Init()
    {
        _label = new GameObject().AddComponent<TextMeshProUGUI>();
    }

    [TearDown]
    public void TearDown()
    {
        _label = null;
    }

    [Test]
    public void TestSetDependencies()
    {
        //Given a new EnvironmentTargetControls
        //When starting up
        //Then the dependencies are configured.
        
        //Arrange
        var sut = new GameObject().AddComponent<EnvironmentTargetControls>();
        //Act
        sut.SetDependencies(new EnvironmentTargetControls.Dependencies()
        {
            Label = _label
        });
        //Assert
        Assert.AreSame(_label, sut._dependencies.Label);
    }

    [Test]
    public void TestConfigure_SetsLabel_With_Value()
    {
        //Given an existing EnvironmentTargetControls
        //When configure is called
        //Then the label is set.
        
        //Arrange
        var labelText = "environment";
        var url = "https://environmentUrl.com";
        
        var sut = new GameObject().AddComponent<EnvironmentTargetControls>();
        sut.SetDependencies(new EnvironmentTargetControls.Dependencies()
        {
            Label = _label
        });
        //Act
        sut.Configure(new EnvironmentTargetControls.Config()
        {
            Target = new Target()
            {
                Name = labelText,
                Url = url
            },
            ButtonAction = (value) => {}
        });
        //Assert
        Assert.AreEqual(labelText, sut._dependencies.Label.text);
    }

    [Test]
    public void TestButtonAction_Calls_ConfiguredAction()
    {
        //Given a configured EnvironmentTargetControl
        //When buttonAction is called
        //Then the configured action is called.
        
        //Arrange
        var labelText = "environment";
        var url = "https://environmentUrl.com";
        
        var sut = new GameObject().AddComponent<EnvironmentTargetControls>();
        sut.SetDependencies(new EnvironmentTargetControls.Dependencies()
        {
            Label = _label
        });
        string recordedValue = "";
        sut.Configure(new EnvironmentTargetControls.Config()
        {
            Target = new Target()
            {
                Name = labelText,
                Url = url
            },
            ButtonAction = (value) => { recordedValue = url;}
        });
        
        //Act
        sut.ButtonAction();
        //Assert
        Assert.AreEqual(url, recordedValue);
    }
}
