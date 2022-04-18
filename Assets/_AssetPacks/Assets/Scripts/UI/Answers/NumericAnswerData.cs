using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using riddlehouse_libraries.products.Assets;
using riddlehouse_libraries.products.AssetTypes;
using TestRiddlehouseLibraries.LibraryTests.Products.Assets;
using UnityEngine;

public class NumericAnswerData: IAnswerAsset
{
    public string Url
    {
        get { throw new NotImplementedException(); }
    }
    /// <summary>
    /// Answer supplied from the CDN to compare against.
    /// </summary>
    public int CorrectAnswer { get; private set; }

    /// <summary>
    /// Answer given by the user and recorded by the system.
    /// </summary>
    public float RecordedAnswer { get; private set; }

    private string _sessionId;
    private HuntSessionPersistor _sessionPersistor;
    /// <summary>
    /// Instantiates with a link to the answer file.
    /// This constructor will retrieve the asset and unpack the data appropriately.
    /// </summary>
    /// <param name="identifier">The identifier for the session persistance handler.</param>
    /// <param name="textGetter">The getter to use for downloading the asset.</param>
    /// <param name="answerAssetUrl">The url to the answer file.</param>
    /// <param name="assetReady">The delegate action to invoke after completing succesfully/unsuccesfully.</param>
    public NumericAnswerData(string identifier, ITextGetter textGetter, string answerAssetUrl, Action<bool> assetReady)
    {
        try
        {
            textGetter.GetText(answerAssetUrl, false, (answer) => { 
                CorrectAnswer = int.Parse(answer);
                assetReady(true);
            });

            _sessionId = identifier;
            _sessionPersistor = new HuntSessionPersistor();
            if (_sessionPersistor.HasAnswerInSession(_sessionId))
            {
                RecordedAnswer = _sessionPersistor.GetNumericAnswer(_sessionId);
            }
        }
        catch
        {
            assetReady(false);
        }
    }

    public NumericAnswerData(string identifier, float correctAnswer)
    {
        CorrectAnswer = (int)correctAnswer;
        _sessionId = identifier;
        _sessionPersistor = new HuntSessionPersistor();
        if (_sessionPersistor.HasAnswerInSession(_sessionId))
        {
            RecordedAnswer = _sessionPersistor.GetNumericAnswer(_sessionId);
        }
    }
    
    public AnswerType GetAnswerType()
    {
        return AnswerType.NumericTextField;
    }

    public void SetHuntSessionPersistor(IHuntSessionPersistor huntSessionPersistor, string sessionId)
    {
        throw new NotImplementedException();
    }

    public bool HasAnswer()
    {
        //null object pattern, if it's not set then no answer has been given.
        return RecordedAnswer != null;
    }
    
    public bool HasCorrectAnswer()
    {
        return CorrectAnswer == RecordedAnswer;
    }

    public void ClearSession()
    {
        RecordedAnswer = float.MaxValue;
        _sessionPersistor.ClearAnswerInSession(_sessionId);
    }

    public Task DownloadForOffline()
    {
        throw new NotImplementedException();
    }

    public AssetType Type { get; }

    public void SetAnswer(int? answer)
    {
        RecordedAnswer = float.MaxValue;
        if(answer != null)
            _sessionPersistor.SetNumericAnswer(_sessionId, (int)answer);
    }
}
