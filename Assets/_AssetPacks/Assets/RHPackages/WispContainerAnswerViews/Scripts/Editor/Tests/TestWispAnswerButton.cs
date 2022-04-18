using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestWispAnswerButton
{
    [Test]
    public void TestConfigure()
    {
        var value = "adsjdkn";
        Action<ButtonState> buttonAction = null;
        Color selected = Color.black;
        Color unselected = Color.blue;
        Sprite icon = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);

        var sut = new GameObject().AddComponent<WispAnswerButton>();
        sut.Configure(value, buttonAction, selected, unselected, icon);
        
        Assert.AreEqual(icon, sut._changeImageSpriteComponent._imageComponent.sprite);
        Assert.AreEqual(unselected, sut._toggleImageColorComponent.ImageComponent.color);
    }

    [Test]
    public void TestButtonPressed()
    {
        //Arrange
        var value = "adsjdkn";
        ButtonState state = null;
        Action<ButtonState> buttonAction = (newState) =>
        {
            state = newState;
        };
        Color selected = Color.black;
        Color unselected = Color.blue;
        Sprite icon = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);

        var sut = new GameObject().AddComponent<WispAnswerButton>();
        sut.Configure(value, buttonAction, selected, unselected, icon);
        
        //Act
        sut.ButtonPressed();
        
        //Assert
        Assert.AreEqual(selected, sut._toggleImageColorComponent.ImageComponent.color);
        Assert.IsNotNull(state);
        Assert.AreEqual(value, state.Value);
        Assert.AreEqual(true, state.Selected);
        Assert.IsTrue(sut._toggleImageColorComponent.IsSelected);

        //Act 2
        sut.ButtonPressed();
        
        //Assert 2
        Assert.AreEqual(false, state.Selected);
        Assert.IsFalse(sut._toggleImageColorComponent.IsSelected);

    }
    
    [Test]
    public void TestSetState_SetsToSelected()
    {
        var value = "adsjdkn";
        Action<ButtonState> buttonAction = null;
        Color selected = Color.black;
        Color unselected = Color.blue;
        Sprite icon = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);

        var sut = new GameObject().AddComponent<WispAnswerButton>();
        sut.Configure(value, buttonAction, selected, unselected, icon);
        
        sut.SetState(true);
        
        Assert.AreEqual(selected, sut._toggleImageColorComponent.ImageComponent.color);
        Assert.IsTrue(sut._toggleImageColorComponent.IsSelected);
    }
    [Test]
    public void TestSetState_SetsToUnSelected()
    {
        var value = "adsjdkn";
        Action<ButtonState> buttonAction = null;
        Color selected = Color.black;
        Color unselected = Color.blue;
        Sprite icon = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);

        var sut = new GameObject().AddComponent<WispAnswerButton>();
        sut.Configure(value, buttonAction, selected, unselected, icon);
        
        sut.SetState(false);
        
        Assert.AreEqual(unselected, sut._toggleImageColorComponent.ImageComponent.color);
        Assert.IsFalse(sut._toggleImageColorComponent.IsSelected);
    }
}
