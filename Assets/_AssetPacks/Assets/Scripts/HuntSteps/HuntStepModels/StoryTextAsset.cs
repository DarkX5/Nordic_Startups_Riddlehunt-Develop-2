using System;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;

public class StoryTextAsset :ITextAsset, IAsset
{
    private readonly AssetType _assetType;
    private readonly TextAsset _textAsset;
    public StoryTextAsset(ITextGetter textGetter, string uri, Action<bool> isReady)
    {
        _assetType = AssetType.StoryText;
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
