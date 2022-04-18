using System;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prefabs.UI.CharacterSelection.StyledCharacterSelection.Scripts.Editor.Tests
{
    public class TestSelectableTagButton 
    {
        private SelectableTagButton.Dependencies CreateDependencies(Sprite selectedImage, Sprite unselectedImage)
        {
            return new SelectableTagButton.Dependencies()
            {
                ButtonImg = new GameObject().AddComponent<Image>(),
                ButtonText = new GameObject().AddComponent<TextMeshProUGUI>(),
                SelectedImage = selectedImage,
                UnselectedImage = unselectedImage
            };
        }
    
        [Test]
        public void TestSetDependencies()
        {
            //Arrange
            var selected = Sprite.Create(Texture2D.redTexture, Rect.zero, Vector2.down);
            var unselected = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
            var sut = new GameObject().AddComponent<SelectableTagButton>();
            var dependencies = CreateDependencies(selected, unselected);
            //Act
            sut.SetDependencies(dependencies);
            //Assert
            Assert.AreEqual(dependencies, sut._dependencies);
        }
        
        [Test]
        public void TestDisplay()
        {
            //Arrange
            var sut = new GameObject().AddComponent<SelectableTagButton>();
            sut.gameObject.SetActive(false);
            //Act
            sut.Display();
            //Assert
            Assert.IsTrue(sut.gameObject.activeSelf);
        }
        [Test]
        public void TestHide()
        {
            //Arrange
            var sut = new GameObject().AddComponent<SelectableTagButton>();
            sut.gameObject.SetActive(true);
            //Act
            sut.Hide();
            //Assert
            Assert.IsFalse(sut.gameObject.activeSelf);
        }
        [Test]
        public void TestConfigure()
        {
            //Arrange
            var selected = Sprite.Create(Texture2D.redTexture, Rect.zero, Vector2.down);
            var unselected = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
            var sut = new GameObject().AddComponent<SelectableTagButton>();
            var dependencies = CreateDependencies(selected, unselected);
            sut.SetDependencies(dependencies);
            string option = "optionA";
            //Act
            sut.Configure(new SelectableTagButton.Config()
            {
                Option = option
            });
            //Assert
            Assert.AreEqual(option, sut._dependencies.ButtonText.text);
            Assert.AreEqual(Vector3.one*0.8f, sut.transform.localScale);
            Assert.AreEqual(unselected, sut._dependencies.ButtonImg.sprite);
        }
    
        [Test]
        public void TestPerformAction()
        {
            //Arrange
            var selected = Sprite.Create(Texture2D.redTexture, Rect.zero, Vector2.down);
            var unselected = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
            var sut = new GameObject().AddComponent<SelectableTagButton>();
            var dependencies = CreateDependencies(selected, unselected);
            sut.SetDependencies(dependencies);
            string option = "optionA";
            string choice = "not this choice";
            Action<string> buttonAction = (msg) =>
            {
                choice = msg;
            };
            sut.Configure(new SelectableTagButton.Config()
            {
                Option = option,
                ButtonAction = buttonAction
            });
            //Act
            sut.PerformAction();
            //Assert
            Assert.AreEqual(option, choice);
        }
        
        [Test]
        public void TestSelectButton()
        {
            //Arrange
            var selected = Sprite.Create(Texture2D.redTexture, Rect.zero, Vector2.down);
            var unselected = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
            var sut = new GameObject().AddComponent<SelectableTagButton>();
            var dependencies = CreateDependencies(selected, unselected);
            sut.SetDependencies(dependencies);
            string option = "optionA";
            sut.Configure(new SelectableTagButton.Config()
            {
                Option = option
            });
            //Act
            sut.SelectButton();
            //Assert
            Assert.AreEqual(Vector3.one, sut.transform.localScale);
            Assert.AreEqual(selected, sut._dependencies.ButtonImg.sprite);
        }
        
        [Test]
        public void ResetButton()
        {
            //Arrange
            var selected = Sprite.Create(Texture2D.redTexture, Rect.zero, Vector2.down);
            var unselected = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
            var sut = new GameObject().AddComponent<SelectableTagButton>();
            var dependencies = CreateDependencies(selected, unselected);
            sut.SetDependencies(dependencies);
            string option = "optionA";
            sut.Configure(new SelectableTagButton.Config()
            {
                Option = option
            });
            //Act
            sut.ResetButton();
            //Assert
            Assert.AreEqual(Vector3.one*0.8f, sut.transform.localScale);
            Assert.AreEqual(unselected, sut._dependencies.ButtonImg.sprite);
        }
    }
}
