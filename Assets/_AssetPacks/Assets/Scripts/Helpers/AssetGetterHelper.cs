using System;
using System.Collections;
using System.Collections.Generic;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using UnityEngine;

public class AssetGetterHelper
{
    public string GetAssetUrl(HuntStep step, AssetType type)
    {
        var idx = step.Assets.FindIndex(x => x.Type == type);
        if (idx != -1)
        {
            return step.Assets[idx].Url;
        }
        throw new ArgumentException("Asset of type: " + type + " not found in step data.");
    }

    public HuntAsset GetAnswerHuntAsset(HuntStep step)
    {
        var idx = step.Assets.FindIndex(x => 
            x.Type == AssetType.NumericAnswer ||
            x.Type == AssetType.TextAnswer ||
            x.Type == AssetType.MultipleChoiceTextAnswer ||
            x.Type == AssetType.MultipleChoiceAnswerIcons);

        if (idx != -1)
            return step.Assets[idx];
        
        throw new ArgumentException("no answer found in step data.");
    }
}
