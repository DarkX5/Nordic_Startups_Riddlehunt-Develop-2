using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using riddlehouse_libraries.products.Assets.AnswerAssets;
using TestRiddlehouseLibraries.LibraryTests.Products.Assets;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Playables;

public class TestWispAnswerComponent
{
    private WispAnswerComponent.Dependencies CreateDependencies(
        List<Mock<IWispAnswerButton>> wispAnswerButtons = null,
        IRunesPack runesMock = null)

    {
        runesMock ??= CreateRunesPack();
        if (wispAnswerButtons == null)
        {
            wispAnswerButtons = new List<Mock<IWispAnswerButton>>();
            for (int i = 0; i < 6; i++)
            {
                wispAnswerButtons.Add(new Mock<IWispAnswerButton>());
            }
        }
        
        var answerButtonMocks = new List<IWispAnswerButton>();
        foreach (var buttonMock in wispAnswerButtons)
        {
            answerButtonMocks.Add(buttonMock.Object);
        }
        
        return new WispAnswerComponent.Dependencies()
        {
            AnswerButtons = answerButtonMocks,
            Runes = runesMock
        };
    }

    private MultipleChoiceTextAnswerAsset CreateMultipleChoiceTextAnswerAsset(
        List<string> answerOptions = null, 
        string correctanswer = null, 
        MultipleChoiceLogic evaluation = MultipleChoiceLogic.ContainsAll,
        string seperator = null)
    {

        answerOptions ??= new List<string>() { "O", "R", "H", "A", "M", "S" };
        correctanswer ??= "H;A;M;";
        seperator ??= ";";
        
        return new MultipleChoiceTextAnswerAsset(new MultipleChoiceTextAnswerOptions()
        {
            AnswerOptions = answerOptions,
            CorrectAnswer = correctanswer,
            Evaluation = evaluation,
            Seperator = seperator
        });
    }

    private Mock<IWispAnswerButton> CreateWispAnswerButton(string option, Color selected, Color unselected, Sprite sprite)
    {
        var buttonMock = new Mock<IWispAnswerButton>();
        buttonMock.Setup(x =>
            x.Configure(
                option,
                It.IsAny<Action<ButtonState>>(),
                selected,
                unselected,
                sprite
            )).Verifiable();
        return buttonMock;
    }

    private void ConfirmWispAnswerButtonConfigured(Mock<IWispAnswerButton> answerButtonMock, string option, Color selected, Color unselected, Sprite sprite)
    {
        answerButtonMock.Verify(x => x.Configure(option, It.IsAny<Action<ButtonState>>(), selected, unselected, sprite));
    }

    private List<string> CreateIds()
    {
       return new List<string>() { "black", "red", "linearGray", "normal", "gray", "white" };
    }
    private List<Sprite> CreateSprites()
    {
        var spriteBlack = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.down);
        var spriteRed = Sprite.Create(Texture2D.redTexture, Rect.zero, Vector2.down);
        var spriteLinearGray = Sprite.Create(Texture2D.linearGrayTexture, Rect.zero, Vector2.down);
        var spriteNormal = Sprite.Create(Texture2D.normalTexture, Rect.zero, Vector2.down);
        var spriteGray = Sprite.Create(Texture2D.grayTexture, Rect.zero, Vector2.down);
        var spriteWhite = Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.down);
        return new List<Sprite>()
        {
            spriteBlack,
            spriteRed,
            spriteLinearGray,
            spriteNormal,
            spriteGray,
            spriteWhite
        };
    }
    private RunesPack CreateRunesPack(List<string> ids = null, List<Sprite> sprites = null)
    {
        ids ??= CreateIds();

        if (sprites == null)
        {
            sprites = CreateSprites();
        }
        return new RunesPack(ids, sprites);
    }

    [Test]
    public void TestConfigure_ConfiguresAllButtons()
    {
        //Given a new WispAnswerComponent with 6 buttons, and answerAsset with 6 buttons and a runesPack
        //When configure is called with that answerasset
        //Then the buttons are all configured with the appropriate assets.
        
        //Arrange
        var answerAsset = CreateMultipleChoiceTextAnswerAsset();
        var selected = Color.black;
        var unselected = Color.blue;

        var sprites = CreateSprites();
        var runes = CreateRunesPack(answerAsset.AnswerOptions, sprites);

        var buttons = new List<Mock<IWispAnswerButton>>();
        for (int i = 0; i < answerAsset.AnswerOptions.Count; i++)
        {
            buttons.Add(CreateWispAnswerButton(
                answerAsset.AnswerOptions[i],
                selected,
                unselected,
                sprites[i]));
        }

        var dependencies = CreateDependencies(buttons, runes);
        
        var sut = new GameObject().AddComponent<WispAnswerComponent>();
        sut.SetDependencies(dependencies);

        //Act
        sut.Configure(answerAsset, selected, unselected);
        
        //Assert
        for (int i = 0; i < buttons.Count; i++)
        {
            ConfirmWispAnswerButtonConfigured(buttons[i], answerAsset.AnswerOptions[i], selected, unselected, sprites[i]);
        }
    }
    
    [Test]
    public void TestConfigure_WithPreviousSession()
    {
        //Given a new WispAnswerComponent with 6 buttons, an answerAsset with 6 buttons, a runesPack and a previousAnswer in the session.
        //When configure is called with that answerasset
        //Then the buttons are all configured with the appropriate states.
        
        //Arrange
        var identifier = "wisp_test_config";
        var answerAsset = CreateMultipleChoiceTextAnswerAsset();
        var sessionPersistor = new Mock<IHuntSessionPersistor>();
        sessionPersistor.Setup(x => x.HasAnswerInSession(identifier)).Returns(true).Verifiable();
        sessionPersistor.Setup(x => x.GetStringAnswer(identifier)).Returns("H;A;M;").Verifiable();
        answerAsset.SetHuntSessionPersistor(sessionPersistor.Object, identifier);
        var selected = Color.black;
        var unselected = Color.blue;

        var sprites = CreateSprites();
        var runes = CreateRunesPack(answerAsset.AnswerOptions, sprites);

        var buttons = new List<Mock<IWispAnswerButton>>();
        for (int i = 0; i < answerAsset.AnswerOptions.Count; i++)
        {
            buttons.Add(CreateWispAnswerButton(
                answerAsset.AnswerOptions[i],
                selected,
                unselected,
                sprites[i]));
        }

        var dependencies = CreateDependencies(buttons, runes);
        
        var sut = new GameObject().AddComponent<WispAnswerComponent>();
        sut.SetDependencies(dependencies);

        //Act
        sut.Configure(answerAsset, selected, unselected);
        
        //Assert
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].Verify(x => x.SetState(false));
            ConfirmWispAnswerButtonConfigured(buttons[i], answerAsset.AnswerOptions[i], selected, unselected, sprites[i]);
            if(answerAsset.RecordedAnswer.Contains(answerAsset.AnswerOptions[i]))
                buttons[i].Verify(x => x.SetState(true));
        }
        sessionPersistor.Verify(x => x.HasAnswerInSession(identifier));
        sessionPersistor.Verify(x => x.GetStringAnswer(identifier));
    }

    [Test]
    public void TestConfigure_6_Options_5_Buttons_Throws()
    {
        //Given a WispAnswerComponent, an AnswerAsset with 6 options, and a runesPack with only 5 buttons.
        //When Configure is called
        //Then the function throws and error.
        
        var answerAsset = CreateMultipleChoiceTextAnswerAsset();
        var sut = new GameObject().AddComponent<WispAnswerComponent>();
        var buttons = new List<Mock<IWispAnswerButton>>();
        
        for (int i = 0; i < 5; i++)
        {
            buttons.Add(new Mock<IWispAnswerButton>());
        }
        var dependencies = CreateDependencies(buttons);

        sut.SetDependencies(dependencies);
        
        //Act & Assert
        Assert.Throws<ArgumentException>(() => sut.Configure(answerAsset, Color.black, Color.blue));
    }

    [Test]
    public void TestSubmitAnswerChosen_Callback_Updates_AnswerAsset()
    {
        var ids = CreateIds();
        var answerAssetMock = new Mock<IMultipleChoiceTextAnswerAsset>();
        answerAssetMock.Setup(x => x.AnswerOptions).Returns(ids);
        answerAssetMock.Setup(x => x.AddAnswer(ids[0])).Verifiable();
        answerAssetMock.Setup(x => x.RemoveAnswer(ids[1])).Verifiable();

        var runes = CreateRunesPack(ids);
        var selected = Color.black;
        var unselected = Color.blue;

        var buttons = new List<Mock<IWispAnswerButton>>();
        for (int i = 0; i < ids.Count; i++)
        {
            buttons.Add(new Mock<IWispAnswerButton>());
        }

        buttons[0].Setup(x => x.Configure(
                ids[0],
                It.IsAny<Action<ButtonState>>(),
                selected, unselected, It.IsAny<Sprite>()))
            .Callback<string, Action<ButtonState>, Color, Color, Sprite>
            ((value, theAction, selected, unselected, sprite) =>
            {
                theAction.Invoke(new ButtonState()
                {
                    Selected = true,
                    Value =  ids[0],
                });
                theAction.Invoke(new ButtonState()
                {
                    Selected = false,
                    Value =  ids[1],
                });
            }).Verifiable();
        
        var sut = new GameObject().AddComponent<WispAnswerComponent>();
        var dependencies = CreateDependencies(buttons, runes);
        sut.SetDependencies(dependencies);
        
        //Act -> action happens in button callback
        sut.Configure(answerAssetMock.Object, selected,unselected);
        
        
        //Assert
        buttons[0].Verify(x => x.Configure(
            ids[0],
            It.IsAny<Action<ButtonState>>(),
            selected, unselected, It.IsAny<Sprite>()));
            
        answerAssetMock.Verify(x => x.AnswerOptions);
        answerAssetMock.Verify(x => x.AddAnswer(ids[0]));
        answerAssetMock.Verify(x => x.RemoveAnswer(ids[1]));
       
    }
}
