using System;
using System.Collections;
using System.Collections.Generic;
using riddlehouse_libraries.products.Assets.AnswerAssets;
using UnityEngine;
using UnityEngine.Serialization;

public interface IWispAnswerComponent
{
    public void Configure(IMultipleChoiceTextAnswerAsset answerAsset, Color selected, Color unselected);
}
public class ButtonState
{
    public bool Selected;
    public string Value;
}
public class WispAnswerComponent : MonoBehaviour, IWispAnswerComponent
{
    public class Dependencies
    {
        public List<IWispAnswerButton> AnswerButtons;

        public IRunesPack Runes;
    }

    public void Initialize()
    {
        var buttonList = new List<IWispAnswerButton>();
        foreach (var button in answerButtons)
        {
            buttonList.Add(button);
        }
        SetDependencies(new Dependencies()
        {
            AnswerButtons = buttonList,
            Runes = runes
        });
    }

    [FormerlySerializedAs("AnswerButtons")] [SerializeField] private List<WispAnswerButton> answerButtons;

    [SerializeField] private RunesPack runes;

    private Dependencies _dependencies;
    public void SetDependencies(Dependencies dependencies)
    {
        _dependencies = dependencies;
    }

    private IMultipleChoiceTextAnswerAsset _answerAsset;
    public void Configure(IMultipleChoiceTextAnswerAsset answerAsset, Color selected, Color unselected)
    {
        _answerAsset = answerAsset;
        if (_answerAsset.AnswerOptions.Count != _dependencies.AnswerButtons.Count)
            throw new ArgumentException("this answer type needs as many options as there are buttons");

        int i = 0;
        foreach (var answerButton in _dependencies.AnswerButtons)
        {
            var option = answerAsset.AnswerOptions[i];
            answerButton.Configure(option, SubmitAnswerChosen, selected, unselected,
                _dependencies.Runes.GetRuneIcon(option));
            answerButton.SetState(false);
            if (answerAsset.HasAnswer())
            {
                if (answerAsset.RecordedAnswer.Contains(option))
                    answerButton.SetState(true);
            }
            i++;
        }
    }

    private void SubmitAnswerChosen(ButtonState state)
    {
        if(state.Selected)
            _answerAsset.AddAnswer(state.Value);
        else _answerAsset.RemoveAnswer(state.Value);
    }
}
