using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using UnityEngine;

[TestFixture]
public class TestDisplayStoryAndDoneStep
{
   [Test]
    public void TestNewInstance_throws()
    {
        //Given an unfulfilled internal step instance.
        //When constructor is called.
        //Then throws an error.
        var internalStep = new InternalDisplayStoryAndDoneHuntStep();
        internalStep.StoryText = null;
        Assert.Throws<ArgumentException>(() => new DisplayStoryAndDoneHuntStep(internalStep));
    }

    [Test]
    public void TestGetStepType()
    {
        //Given a new DisplayStoryAndDoneHuntStep with corresponding type
        //When GetStepType is called
        //returns correctStepType
        string storyText = "story text";
        var internalStep = new InternalDisplayStoryAndDoneHuntStep();
        internalStep.StoryText = storyText;
        var sut = new DisplayStoryAndDoneHuntStep(internalStep);
        Assert.AreEqual(StepType.DisplayStoryAndDone, sut.GetStepType());
    }

    [Test]
    public void TestGetStoryText()
    {
        //Given a new DisplayStoryAndDoneHuntStep
        //When GetStoryText is called
        //Then the storyText element is returned.
        
        string storyText = "story text";
        
        var internalStep = new InternalDisplayStoryAndDoneHuntStep();
        internalStep.StoryText = storyText;

        var sut = new DisplayStoryAndDoneHuntStep(internalStep);
        
        Assert.AreEqual(storyText, sut.GetStoryText());
    }
}
