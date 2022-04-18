using System;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;

public class RiddleTextAsset : ITextAsset, IAsset
{
    private readonly AssetType _assetType;
    private readonly TextAsset _textAsset;
    public RiddleTextAsset(ITextGetter textGetter, string uri, Action<bool> isReady)
    {
        _assetType = AssetType.RiddleText;
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
