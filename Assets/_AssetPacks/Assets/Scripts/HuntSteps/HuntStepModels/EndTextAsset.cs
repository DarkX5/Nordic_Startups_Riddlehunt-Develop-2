using System;
using System.Collections;
using System.Collections.Generic;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using UnityEngine;

public class EndTextAsset : ITextAsset
{
    private readonly AssetType _assetType;
    private readonly TextAsset _textAsset;
    public EndTextAsset(ITextGetter textGetter, string uri, Action<bool> isReady)
    {
        _assetType = AssetType.EndText;
        _textAsset = new TextAsset(textGetter, uri, isReady);
    }

    public string GetText()
    {
        return _textAsset.GetText();
    }

    public AssetType GetAssetType()
    {
        return _assetType;
    }
}
