using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using riddlehouse_libraries.products.models.DTOs;
using UnityEngine;
// using UnityEngine.XR.ARSubsystems; //TODO: AR ELEMENT

[TestFixture]
public class TestHuntSteps 
{
  //TODO: AR ELEMENT
  // public XRReferenceImageLibrary lib;

  [SetUp]
  public void Init()
  {
    //collects imagelibrary asset file from streaming assets; if null, then the file is missing.
    //TODO: AR ELEMENT
    // lib = Resources.Load<XRReferenceImageLibrary>("editor/testLibrary");
  }
  
  [Test]
  public void TestConvertLockedStepData()
  {
    string feedbackLink = "https://google.com";
    var stepData = new InternalDisplayStoryAndDoneHuntStep();
    stepData.StoryText = "story text";

    
    var unsafeAssetCollection = new List<IInternalHuntStep>() {stepData};
    var sut = new HuntSteps(feedbackLink, "id");
    sut.ConvertInternalStepdata(unsafeAssetCollection);
    Assert.IsNotNull(sut.GetElement(0));
    Assert.AreEqual(stepData.StoryText, ((DisplayStoryAndDoneHuntStep)sut.GetElement(0)).GetStoryText());
    Assert.AreEqual(feedbackLink, sut.GetFeedbackLink());
  }

  [Test]
  public void TestGetElement()
  {
    string feedbackLink = "https://google.com";

    var stepData = new InternalDisplayStoryAndDoneHuntStep();
    stepData.StoryText = "story text";

    var unsafeAssetCollection = new List<IInternalHuntStep>() {stepData};
    var sut = new HuntSteps(feedbackLink, "id");
    sut.ConvertInternalStepdata(unsafeAssetCollection);
    Assert.IsNotNull(sut.GetElement(0));
    Assert.AreEqual(stepData.GetStepType(), sut.GetElement(0).GetStepType());
  }
  
  [Test]
  public void TestGetLengthOfHunt()
  {
    string feedbackLink = "https://google.com";

    var stepData = new InternalDisplayStoryAndDoneHuntStep();
    stepData.StoryText = "story text";

    var unsafeAssetCollection = new List<IInternalHuntStep>() {stepData};
    var sut = new HuntSteps(feedbackLink, "id");
    sut.ConvertInternalStepdata(unsafeAssetCollection);
    Assert.AreEqual(sut.GetLengthOfHunt(), 1);
  }

  public DisplayRiddleAndSubmitAnswerStep CreateRiddleAndSubmitAnswer(string stepId, StepCondition condition)
  {
    string title = "title";
    string id = stepId;
    StepType type = StepType.DisplayRiddleAndSubmitAnswer;
    HuntAsset storyAsset = new HuntAsset() { Type = AssetType.StoryText, Url = "story.com" };
    HuntAsset riddleAsset = new HuntAsset() { Type = AssetType.RiddleText, Url = "riddle.com" };
    HuntAsset answerAsset = new HuntAsset() { Type = AssetType.TextAnswer, Url = "answer.com" };
    HuntAsset imageUrls = new HuntAsset() { Type = AssetType.ImageList, Url = "imageUrls.com" };

    var assets = new List<HuntAsset>() { storyAsset, riddleAsset, answerAsset, imageUrls };
    HuntStep step = new HuntStep() { Title = title, Id = id, Type = type, Assets = assets, Condition = condition };

    var textGetterMock = new Mock<ITextGetter>();
    textGetterMock.Setup(x => x.GetText(storyAsset.Url, false, It.IsAny<Action<string>>()))
      .Callback<string, bool, Action<string>>((theUrl, theCache, theAction) =>
      {
        theAction("StoryText");
      });
    
    textGetterMock.Setup(x => x.GetText(riddleAsset.Url, false, It.IsAny<Action<string>>()))
      .Callback<string, bool, Action<string>>((theUrl, theCache, theAction) =>
      {
        theAction("RiddleText");
      });
    
    textGetterMock.Setup(x => x.GetText(answerAsset.Url, false, It.IsAny<Action<string>>()))
      .Callback<string, bool, Action<string>>((theUrl, theCache, theAction) =>
      {
        theAction("AnswerText");
      });
    
    textGetterMock.Setup(x => x.GetText(imageUrls.Url, false, It.IsAny<Action<string>>()))
      .Callback<string, bool, Action<string>>((theUrl, theCache, theAction) =>
      {
        theAction("[]");
      });
    
    var imageGetterMock = new Mock<IImageGetter>();
    Action<bool> isReady = (success) => { };
    DisplayRiddleAndSubmitAnswerStep stepModel =
      new DisplayRiddleAndSubmitAnswerStep(step, textGetterMock.Object, imageGetterMock.Object, isReady);

    return stepModel;
  }
  
  [Test]
  public void TestHasStepConditionsBeenMet_AllConditionsMet_Returns_True()
  {
    //Given two huntsteps of type DisplayRiddleAndSubmitAnswer; 2 requires 1 to be solved before starting.
    //When the huntstep is asked to evaluate if the conditions of the second one has been met. (aka; 1 has been solved)
    //Then the system checks the conditions and returns true.
    
    //Arrange
    string feedbackLink = "https://google.com";
    string id1 = "id1";
    string id2 = "id2";

    StepCondition condition1 = null;
    StepCondition condition2 = new StepCondition() 
      //requires Id1 to be solved before it can be played.
      { Type = StepConditionTypes.Prerequisite, Ids = new List<string>() {id1}, Style = StepBtnStyles.Disabled };

    var unsafeAssetCollection = new List<IInternalHuntStep>()
    {
      CreateRiddleAndSubmitAnswer(id1, condition1),
      CreateRiddleAndSubmitAnswer(id2, condition2)
    };
    
    var sut = new HuntSteps(feedbackLink, "id");
    sut.ConvertInternalStepdata(unsafeAssetCollection);

    //solve id 1.
    var answerData1 = ((DisplayRiddleAndSubmitAnswerStep)sut.GetElement(0)).GetAnswerData();
    ((StringAnswerData)(answerData1)).SetAnswer("corect answer");

    //act
    var conditionsMet = sut.HasStepConditionsBeenMet(id2);
    //assert
    Assert.IsTrue(conditionsMet);
  }

  [Test]
  public void TestHasStepConditionsBeenMet_SomeConditionsNotMet_Returns_False()
  {
    //Given three huntsteps of type DisplayRiddleAndSubmitAnswer; 3 requires 1 and 2 to be solved.
    //When the system is asked to evaluate if the conditions have been met.
    //Then the function returns false, because only one of the two dependencies have been solved.
    
    //Arrange
    string feedbackLink = "https://google.com";
    string id1 = "id1";
    string id2 = "id2";
    string id3 = "id3";

    StepCondition condition1 = null;
    StepCondition condition2 = new StepCondition() 
      //requires Id1 to be solved before it can be played.
      { Type = StepConditionTypes.Prerequisite, Ids = new List<string>() {id1}, Style = StepBtnStyles.Disabled };
    StepCondition condition3 = new StepCondition() 
      //requires Id2 to be solved before it can be played. (and by default also id1, since id2 requires id1)
      { Type = StepConditionTypes.Prerequisite, Ids = new List<string>() {id1, id2}, Style = StepBtnStyles.Hidden }; 
    
    var unsafeAssetCollection = new List<IInternalHuntStep>()
    {
      CreateRiddleAndSubmitAnswer(id1, condition1),
      CreateRiddleAndSubmitAnswer(id2, condition2),
      CreateRiddleAndSubmitAnswer(id3, condition3)
    };
    
    var sut = new HuntSteps(feedbackLink, "id");
    sut.ConvertInternalStepdata(unsafeAssetCollection);
    
    //solve id 1.
    var answerData1 = (StringAnswerData)((DisplayRiddleAndSubmitAnswerStep)sut.GetElement(0)).GetAnswerData();
    answerData1.SetAnswer("correct answer");
    
    //Act
    var conditionsMet = sut.HasStepConditionsBeenMet(id3);
    //Assert
    Assert.IsFalse(conditionsMet);
  }

  [Test]
  public void TestHasStepConditionsBeenMet_ConditionsAreNull_Returns_True()
  {
    //Given one huntStep with no conditions available
    //When the system is asked to evaluate if the step has it's conditions met
    //Then the system returns true.
    
    //Arrange
    string feedbackLink = "https://google.com";
    string id1 = "id1";

    StepCondition condition1 = null;

    var unsafeAssetCollection = new List<IInternalHuntStep>()
    {
      CreateRiddleAndSubmitAnswer(id1, condition1)
    };

    var sut = new HuntSteps(feedbackLink, "id");
    sut.ConvertInternalStepdata(unsafeAssetCollection);

    //Act
    var conditionsMet = sut.HasStepConditionsBeenMet(id1);
    //Assert
    Assert.IsTrue(conditionsMet);
  }
  
  [Test]
  public void TestIsLastStep()
  {
    string feedbackLink = "https://google.com";

    var stepData = new InternalDisplayStoryAndDoneHuntStep();
    stepData.StoryText = "story text";
    stepData.StepId = "0";
    
    var stepData2 = new InternalDisplayStoryAndDoneHuntStep();
    stepData2.StoryText = "story text";
    stepData.StepId = "1";

    var unsafeAssetCollection = new List<IInternalHuntStep>() {stepData, stepData2};
    var sut = new HuntSteps(feedbackLink, "id");
    sut.ConvertInternalStepdata(unsafeAssetCollection);
    Assert.IsFalse(sut.IsLastStep(sut.GetElement(0)));
    Assert.IsTrue(sut.IsLastStep(sut.GetElement(1)));
  }
}
