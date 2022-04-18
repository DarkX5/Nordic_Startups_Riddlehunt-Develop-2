using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using riddlehouse_libraries.products.Assets;
using riddlehouse_libraries.products.AssetTypes;
using TestRiddlehouseLibraries.LibraryTests.Products.Assets;
using UnityEngine;

public class BooleanAnswerData : IAnswerAsset
{
    private HuntSessionPersistor _sessionPersistor;
    private string _sessionId;
    public BooleanAnswerData(string identifier)
    {
        _sessionId = identifier;
        _sessionPersistor = new HuntSessionPersistor();
        if (_sessionPersistor.HasAnswerInSession(_sessionId))
        {
            DidSetAnswerOnce = true;
            RecordedAnswer = _sessionPersistor.GetBooleanAnswer(_sessionId);
        }
    }
    /// <summary>
    /// Answer given by the user.
    /// </summary>
    public bool RecordedAnswer { get; private set; } = false;
    
    /// <summary>
    /// Has this answer been set atleast once ?
    /// </summary>
    private bool DidSetAnswerOnce { get; set; }= false;

    public AnswerType GetAnswerType()
    {
        return AnswerType.ScanningBoolean;
    }

    public void SetHuntSessionPersistor(IHuntSessionPersistor huntSessionPersistor, string sessionId)
    {
        throw new System.NotImplementedException();
    }

    public bool HasAnswer()
    {
        return DidSetAnswerOnce;
    }

    public bool HasCorrectAnswer()
    {
        return RecordedAnswer;
    }

    public void ClearSession()
    {
        DidSetAnswerOnce = false;
        RecordedAnswer = false;
        _sessionPersistor.ClearAnswerInSession(_sessionId);
    }

    public string Url
    {
        get { throw new NotImplementedException(); }
    }

    public Task DownloadForOffline()
    {
        throw new System.NotImplementedException();
    }

    public AssetType Type { get; }

    public void SetAnswer(bool answer)
    {
        DidSetAnswerOnce = true;
        RecordedAnswer = answer;
        _sessionPersistor.TickBooleanAnswer(_sessionId);
    }
}