using System;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products.Assets;
using TMPro;
using UnityEngine;

namespace Tests.Editor.BehaviorTests.Answers
{
    public class TestNumericAnswerComponentBehaviour
    {
        private GameObject _testGo;
        private NumericAnswerComponentBehaviour _behaviour;
        private TMP_InputField inputField;
        private TextMeshProUGUI answerField;

        private float _answer;

        private string identifier;
        [SetUp]
        public void SetUp()
        {
            _testGo = new GameObject();
            _behaviour = _testGo.AddComponent<NumericAnswerComponentBehaviour>();
            inputField = _testGo.AddComponent<TMP_InputField>();
            answerField = _testGo.AddComponent<TextMeshProUGUI>();

            _answer = 42;

            identifier = "numericAnswerComponentBehaviour-answer-identifier";
            PlayerPrefs.DeleteKey(identifier);
        }
        
        [TearDown]
        public void TearDown()
        {
            _testGo = null;
            _behaviour = null;
            inputField = null;
        }
        
          [Test]
    public void TestConfigure_NoAnswerAvailable_SetsState_None()
    {
        //Given a NumericAnswerComponentBehaviour, with a NumericAnswerData without an answer given.
        //When Configure is called
        //Answer state is set to none.
        
        // Arrange
        var go = new GameObject();
        var sut = _behaviour;
        sut.SetDependencies(inputField, answerField);
        
        var answerAsset = new NumericAnswerAsset(_answer);

        // Act
        sut.Configure(answerAsset);
        
        // Assert
        Assert.AreEqual(AnswerState.None, sut.State);
    }
    
    [Test]
    public void TestConfigure_NoAnswerAvailable_SetsState_Incorrect()
    {
        //Given a NumericAnswerComponentBehaviour, with a NumericAnswerData witg a wrong answer given.
        //When Configure is called
        //Answer state is set to incorrect.
        
        // Arrange
        var go = new GameObject();
        var sut = _behaviour;
        sut.SetDependencies(inputField, answerField);
        
        var answerAsset = new NumericAnswerAsset(_answer);
        answerAsset.SetAnswer(23);
        // Act
        sut.Configure(answerAsset);
        
        // Assert
        Assert.AreEqual(AnswerState.Incorrect, sut.State);
    }
    
    [Test]
    public void TestConfigure_NoAnswerAvailable_SetsState_Correct()
    {
        //Given a NumericAnswerComponentBehaviour, with a NumericAnswerData with a correct answer given.
        //When Configure is called
        //Answer state is set to correct.
        
        // Arrange
        // Arrange
        var go = new GameObject();
        var sut = _behaviour;
        sut.SetDependencies(inputField, answerField);
        
        var answerAsset = new NumericAnswerAsset(_answer);
        answerAsset.SetAnswer(42);
        // Act
        sut.Configure(answerAsset);
        
        // Assert
        Assert.AreEqual(AnswerState.Correct, sut.State);
    }

        [TestCase(1)]
        [TestCase(0)]
        [TestCase(-1)]
        [Test]
        public void TestIncreaseValueButtonAction_Increments_currentValue(int incrementValue)
        {
            // Given a user has already input an answer in the numeric number component
            // When incrementing the answer
            // Then the numeric answer component's answer is incremented by the specified value

            // Arrange
            const int currentValue = 42;
            var sut = _behaviour;

            var numericAnswerComponent = NumericAnswerComponent.Factory(sut.gameObject);
            var answerAsset = new NumericAnswerAsset(_answer);
            answerAsset.SetAnswer(currentValue);

            sut.SetDependencies(inputField,answerField);
            numericAnswerComponent.Configure(answerAsset, null);

            // Act
            sut.incrementValue = incrementValue;
            sut.IncreaseValueButtonAction();

            // Assert
            Assert.AreEqual(currentValue + incrementValue, answerAsset.RecordedAnswer);
        }
        
        [TestCase(1)]
        [TestCase(0)]
        [TestCase(-1)]
        [Test]
        public void TestDecreaseValueButtonAction_Decrements_currentValue(int decrementValue)
        {
            // Given a user has already input an answer in the numeric number component
            // When decrementing the answer
            // Then the numeric answer component's answer is decremented by the specified value

            // Arrange
            const int currentValue = 42;
            var sut = _behaviour;

            var numericAnswerComponent = NumericAnswerComponent.Factory(sut.gameObject);
            var answerAsset = new NumericAnswerAsset(_answer);
            answerAsset.SetAnswer(currentValue);

            sut.SetDependencies(inputField,answerField);
            numericAnswerComponent.Configure(answerAsset, null);

            // Act
            sut.incrementValue = decrementValue;
            sut.DecreaseValueButtonAction();

            // Assert
            Assert.AreEqual(currentValue - decrementValue, answerAsset.RecordedAnswer);
        }

        [Test]
        public void TestPerformAction_Invokes_Action()
        {
            // Given a user presses the answer button
            // When they want to submit their answer
            // Then the function mapped to the button is called.
            
            // Arrange
            var sut = _behaviour;
            var numericAnswerComponentMock = new Mock<INumericAnswerComponent>();
            numericAnswerComponentMock.Setup(x => x.PerformAction()).Verifiable();
            sut.SetAnswerComponent(numericAnswerComponentMock.Object);
            
            // Act
            sut.PerformAction();
            
            // Assert
            numericAnswerComponentMock.Verify(x => x.PerformAction());
        }
    }
}