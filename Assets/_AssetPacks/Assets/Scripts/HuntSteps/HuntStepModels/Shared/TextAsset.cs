using System;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;

public interface IAsset
{
    public AssetType GetAssetType();
}
public interface ITextAsset
{
    public string GetText();
}
public class TextAsset : ITextAsset
{
    private readonly ITextGetter _textGetter;
    private readonly Action<bool> _ready;
    
    public string Text { get; set; }
    public TextAsset(ITextGetter textGetter, string uri, Action<bool> isReady)
    {
        _ready = isReady;
        _textGetter = textGetter;
        ConfigureTextAsset(uri);
    }
    
    private void ConfigureTextAsset(string textUri)
    {
        try
        {
            _textGetter.GetText(textUri, false,
                SetTextAfterDownload);
        }
        catch
        {
            _ready(false);
        }
    }
    
    private void SetTextAfterDownload(string text)
    {
        Text = text;
        _ready(true);
    }

    public string GetText()
    {
        return Text;
    }
}
