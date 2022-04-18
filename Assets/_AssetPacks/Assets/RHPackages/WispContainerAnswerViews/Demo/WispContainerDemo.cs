using System.Collections;
using System.Collections.Generic;
using riddlehouse_libraries.products.Assets.AnswerAssets;
using UnityEngine;

public class WispContainerDemo : MonoBehaviour
{
    [SerializeField] private Sprite characterIcon;
    [SerializeField] private Color characterColor;
    
    [SerializeField] private WispContainerAnswerView prefab;
    private MultipleChoiceTextAnswerAsset answerAsset;
    public void Start()
    {
        var view = WispContainerAnswerView.Factory(prefab.gameObject, (RectTransform)this.transform);

        answerAsset = new MultipleChoiceTextAnswerAsset(new MultipleChoiceTextAnswerOptions()
        {
            AnswerOptions = new List<string>() { "O", "R", "H", "A", "M", "S" },
            CorrectAnswer = "H;A;M;",
            Evaluation = MultipleChoiceLogic.ContainsAll,
            Seperator = ";"
        });

        string riddleText =
@"Lorem ipsum dolor sit amet, consectetur adipiscing elit. 
Integer ac felis sit amet augue dignissim mollis nec eget urna. Praesent mollis, ipsum vel dignissim aliquet, massa massa vestibulum ipsum, ac commodo tellus ex id turpis. 
Vestibulum vitae ante eget enim interdum lacinia. Sed non odio dictum, luctus enim a, facilisis sapien.";
        
        view.Configure(answerAsset, riddleText, characterColor, characterIcon, AcceptAction, AbortAction);
    }

    public void AcceptAction()
    {
        if (answerAsset.HasCorrectAnswer())
            Debug.Log("CORRECT: " +answerAsset.RecordedAnswer);
        else 
            Debug.Log("INCORRECT: " +answerAsset.RecordedAnswer + " expected: " +answerAsset.CorrectAnswer);
    }

    public void AbortAction()
    {
        Debug.Log("ABORTING VIEW!");
    }
}
