using System;
using Moq;
using NUnit.Framework;
using RHPackages.Core.Scripts.UI;
using riddlehouse_libraries.products.Assets;
using UnityEngine;

namespace Tests.Editor.UITests.ComponentsTests.AnswersTests
{
    public class TestNumericAnswerComponent
    {
        private Mock<INumericAnswerComponentActions> ActionComponentMock;
        private Mock<IViewActions> HuntComponentUIActionsMock;
        
        private string answerAssetUrl;
        private float _answer;
        private string identifier;

        [SetUp]
        public void Init()
        {
            ActionComponentMock = new Mock<INumericAnswerComponentActions>();
            HuntComponentUIActionsMock = new Mock<IViewActions>();
            
            _answer = 42;
  
            identifier = "numericAnswerComponent-answer-identifier";
            PlayerPrefs.DeleteKey(identifier);
        }

        [TearDown]
        public void TearDown()
        {
            ActionComponentMock = null;
            HuntComponentUIActionsMock = null;
        }

        [TestCase(42)]
        [TestCase(float.MaxValue)]
        [Test]
        public void TestConfigure_AnswerData_And_BtnAction_Is_Assigned(float answerField)
        {
            // Given a freshly spawned NumericAnswerComponent
            // When config is called
            // Then AnswerData and BtnAction is assigned correctly

            // Arrange
            var answerAsset = new NumericAnswerAsset(_answer);

            answerAsset.SetAnswer(answerField);
            ActionComponentMock.Setup(x => x.Configure(answerAsset)).Verifiable();

            // Act
            var sut = new NumericAnswerComponent(ActionComponentMock.Object, HuntComponentUIActionsMock.Object);
            sut.Configure(answerAsset, null);

            // Assert
            ActionComponentMock.Verify(x => x.Configure(answerAsset));
        }


        [Test]
        public void PerformAction_Performs_Specified_Action()
        {
            // Given a user submits their answer
            // When answering a riddle with a numeric answer component
            // Then the perform action method of said component is called

            // Arrange
            const int testAnswer = 42;
            var answerAsset = new NumericAnswerAsset(_answer);
            answerAsset.SetAnswer(testAnswer);
            var hasBeenCalled = false;

            void BtnAction()
            {
                hasBeenCalled = true;
            }

            var sut = new NumericAnswerComponent(ActionComponentMock.Object, HuntComponentUIActionsMock.Object);

            // Act
            sut.Configure(answerAsset, BtnAction);
            sut.PerformAction();

            // Assert
            Assert.IsTrue(hasBeenCalled);
        }

        [TestCase("42", ExpectedResult = 42)]
        [TestCase("-42", ExpectedResult = -42)]
        [TestCase("this should give you 42", ExpectedResult = 42)]
        [TestCase("0", ExpectedResult = 0)]
        [TestCase("forty two", ExpectedResult = 0)]
        [TestCase("", ExpectedResult = float.MaxValue)]
        [TestCase(null, ExpectedResult = float.MaxValue)]
        [Test]
        public float TestSetCurrentValue_Takes_String_Returns_float(string answer)
        {
            // Given a user needs to submit an numeric answer
            // When submitting the answer
            // Then validate that it is a numeric answer

            // Arrange
            var answerAsset = new NumericAnswerAsset(_answer);
            var sut = new NumericAnswerComponent(ActionComponentMock.Object, HuntComponentUIActionsMock.Object);
            sut.Configure(answerAsset, null);

            // Act
            sut.SetCurrentValue(answer);

            // Assert
            return answerAsset.RecordedAnswer;
        }
    }
}