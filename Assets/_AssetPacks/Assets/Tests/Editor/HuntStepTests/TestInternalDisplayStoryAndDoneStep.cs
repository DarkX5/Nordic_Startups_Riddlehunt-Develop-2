using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using UnityEngine;

[TestFixture]
public class TestInternalDisplayStoryAndDoneStep
{
    [Test]
    public void TestGetStepType()
    {
        //Given a new InternalDisplayStoryAndDoneHuntStep, of steptype DisplayStoryAndDone
        //When the GetStepType is called
        //Then is returns the same stepType as it's initialized with.
        
        var sut = new InternalDisplayStoryAndDoneHuntStep();
        Assert.AreEqual(StepType.DisplayStoryAndDone, sut.GetStepType());
    }

    [Test]
    public void TestIsExpectedStepType_Succeeds()
    {
        //Given a new InternalDisplayStoryAndDoneHuntStep, of steptype DisplayStoryAndDone
        //When the IsExpectedStepType is called with the steptype DisplayStoryAndDone
        //Then it returns true
        
        var sut = new InternalDisplayStoryAndDoneHuntStep();
        Assert.IsTrue(sut.IsExpectedStepType(StepType.DisplayStoryAndDone));
    }
    [Test]
    public void TestIsExpectedStepType_Fails()
    {
        //Given a new InternalDisplayStoryAndDoneHuntStep, of steptype DisplayStoryAndDone
        //When the IsExpectedStepType is called with the steptype DisplayStoryAndDone
        //Then it returns false
        
        var sut = new InternalDisplayStoryAndDoneHuntStep();
        Assert.IsFalse(sut.IsExpectedStepType(StepType.RecognizeImageAndPlayVideo));
    }

    [Test]
    public void TestValidateAssetConfiguration_Succeeds()
    {
        //Given a new InternalDisplayStoryAndDoneHuntStep, of steptype DisplayStoryAndDone
        //when ValidateAssetConfiguration is called
        //Then the function returns true
        
        var sut = new InternalDisplayStoryAndDoneHuntStep();
        sut.StoryText = "StoryText";

        Assert.IsTrue(sut.ValidateAssetConfiguration());
    }
    
    [Test]
    public void TestValidateAssetConfiguration_Fails()
    {
        //Given a new InternalDisplayStoryAndDoneHuntStep, of steptype DisplayStoryAndDone
        //when ValidateAssetConfiguration is called
        //Then the function returns false
        
        var sut = new InternalDisplayStoryAndDoneHuntStep();
        sut.StoryText = null;

        Assert.IsFalse(sut.ValidateAssetConfiguration());
    }

    [Test]
    public void TestBypassEvaluation_Suceeds()
    {
        //Given a new InternalDisplayStoryAndDoneHuntStep, of steptype DisplayStoryAndDone
        //when Bypassvalidation is called
        //Then the variable of the same name is set.
        
        var sut = new InternalDisplayStoryAndDoneHuntStep();
        sut.SetBypassvalidation("link");
        Assert.IsTrue(sut.DidBypassValidation());
    }
    
    [Test]
    public void TestBypassEvaluation_Fails()
    {
        //Given a new InternalDisplayStoryAndDoneHuntStep, of steptype DisplayStoryAndDone
        //when DidBypassEvaluation is called
        //Then the function returns false, since no one ever told it to bypassvalidation.
        
        //this function is intended to be extended by the child-class.
        var sut = new InternalDisplayStoryAndDoneHuntStep();
        Assert.IsFalse(sut.DidBypassValidation());
    }
    
    [Test]
    public void TestBypassAndValidateAssetConfiguration_Succeeds()
    {
        //Given a new InternalDisplayStoryAndDoneHuntStep, of steptype DisplayStoryAndDone
        //when ValidateAssetConfiguration is called
        //Then the function returns true
        
        var sut = new InternalDisplayStoryAndDoneHuntStep();
        sut.StoryText = "StoryText";

        sut.SetBypassvalidation("link");
        Assert.IsTrue(sut.ValidateAssetConfiguration());
    }
}
