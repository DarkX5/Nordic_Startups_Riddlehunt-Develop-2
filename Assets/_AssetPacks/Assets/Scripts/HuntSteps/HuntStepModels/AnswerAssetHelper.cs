using System;
using System.Collections;
using System.Collections.Generic;
using Answers.MultipleChoice.Data.Icon;
using riddlehouse_libraries.products.Assets;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using UnityEngine;
public class AnswerAssetHelper
{
    public static IAnswerAsset Factory(string identifier, AssetType assetType, ITextGetter textGetter, string uri, Action<bool> isReady, IImageGetter imageGetter = null)
    {
        switch (assetType)
        {
            case AssetType.TextAnswer:
                return new StringAnswerData(identifier, textGetter, uri, isReady);
            case AssetType.NumericAnswer:
                return new NumericAnswerData(identifier, textGetter, uri, isReady);
            // case AssetType.MultipleChoiceTextAnswer: //deprected in its current form.
            //     return new StringMultipleChoiceAnswerData(identifier, textGetter, uri, isReady, ";");
            case AssetType.MultipleChoiceAnswerIcons:
                return new IconMultipleChoiceAnswerData(identifier, textGetter, imageGetter, uri, isReady);
                break;
            default:
                throw new ArgumentException("Answer type not defined in loader");
        }
    }
}
