using System.Collections;
using System.Collections.Generic;
using riddlehouse_libraries.products.Assets;
using riddlehouse_libraries.products.Assets.AnswerAssets;
using TestRiddlehouseLibraries.LibraryTests.Products.Assets;
using UnityEngine;

public class IconStringMultipleChoiceAnswerData : StringMultipleChoiceAnswerData
{
    public List<Sprite> Icons { get; private set; }
    public IconStringMultipleChoiceAnswerData(List<Sprite> icons, string identifier, List<string> options, string correctAnswer, string seperator, MultipleChoiceLogic logic) : base(identifier, options, correctAnswer, seperator, logic)
    {
        Icons = icons;
    }
    public override AnswerType GetAnswerType()
    {
        return AnswerType.MultipleChoiceTextIcon;
    }   
}
