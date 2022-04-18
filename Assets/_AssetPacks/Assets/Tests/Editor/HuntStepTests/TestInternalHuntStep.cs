using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using UnityEngine;

[TestFixture]
public class TestInternalHuntStep
{
    [Test]
    public void TestGetStepType()
    {
        //Given a new internal hunt step, of steptype DisplayStoryAndDone
        //When the GetStepType is called
        //Then is returns the same stepType as it's initialized with.
        
        var sut = new InternalHuntStep(StepType.DisplayStoryAndDone);
        Assert.AreEqual(StepType.DisplayStoryAndDone, sut.GetStepType());
    }

    [Test]
    public void TestIsExpectedStepType_Succeeds()
    {
        //Given a new internal hunt step, of steptype DisplayStoryAndDone
        //When the IsExpectedStepType is called with the steptype DisplayStoryAndDone
        //Then it returns true
        
        var sut = new InternalHuntStep(StepType.DisplayStoryAndDone);
        Assert.IsTrue(sut.IsExpectedStepType(StepType.DisplayStoryAndDone));
    }
    [Test]
    public void TestIsExpectedStepType_Fails()
    {
        //Given a new internal hunt step, of steptype DisplayStoryAndDone
        //When the IsExpectedStepType is called with the steptype RecognizeImageAndPlayVideo
        //Then it returns false
        
        var sut = new InternalHuntStep(StepType.DisplayStoryAndDone);
        Assert.IsFalse(sut.IsExpectedStepType(StepType.RecognizeImageAndPlayVideo));
    }

    [Test]
    public void TestValidateAssetConfiguration()
    {
        //given a new internal hunt step, of steptype DisplayStoryAndDone
        //when ValidateAssetConfiguration is called
        //Then a not implemented exception is thrown.
        
        //All step types should have their own class,
        //this is the base class not intended to be used as standalone.
        var sut = new InternalHuntStep(StepType.DisplayStoryAndDone);
        Assert.Throws <ArgumentException>(() => sut.ValidateAssetConfiguration());
    }

    [Test]
    public void TestDidBypassEvaluation()
    {
        //given a new internal hunt step, of steptype DisplayStoryAndDone
        //when Bypassvalidation is called
        //Then the variable of the same name is set.
        
        //this function is intended to be extended by the child-class.
        var sut = new InternalHuntStep(StepType.DisplayStoryAndDone);
        sut.SetBypassvalidation("link");
        Assert.IsTrue(sut.DidBypassValidation());
    }
}
