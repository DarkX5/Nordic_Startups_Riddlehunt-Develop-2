using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using riddlehouse_libraries.products.Assets;
using riddlehouse_libraries.products.Assets.AnswerAssets;
using riddlehouse_libraries.products.AssetTypes;
using TestRiddlehouseLibraries.LibraryTests.Products.Assets;
using UnityEngine;

public enum MultipleChoiceTypes
{
    Text,
    Icon
};

public class StringMultipleChoiceAnswerData : IAnswerAsset
{
    public string Url
    {
        get { throw new NotImplementedException(); }
    }
    public string CorrectAnswers { get; private set; }
    public List<string> PossibleAnswers { get; private set; }
    public string RecordedAnswer { get; private set; } = null;
    public string Seperator { get; private set; }
    public MultipleChoiceLogic _logic { get; private set; }
    private string _sessionId;
    private HuntSessionPersistor _sessionPersistor;

    public StringMultipleChoiceAnswerData(string identifier, List<string> options, string correctAnswer, string seperator, MultipleChoiceLogic logic)
    {
        _logic = logic;
        Seperator = seperator;
        CorrectAnswers = correctAnswer;
        PossibleAnswers = options;
        _sessionId = identifier;
        _sessionPersistor = new HuntSessionPersistor();
        if (_sessionPersistor.HasAnswerInSession(_sessionId))
        {
            RecordedAnswer = _sessionPersistor.GetStringAnswer(_sessionId);
        }
    }
    public virtual AnswerType GetAnswerType()
    {
        return AnswerType.MultipleChoiceText;
    }

    public void SetHuntSessionPersistor(IHuntSessionPersistor huntSessionPersistor, string sessionId)
    {
        throw new NotImplementedException();
    }

    public bool HasAnswer()
    {
        return RecordedAnswer != null;
    }

    public bool HasCorrectAnswer()
    {
        if (RecordedAnswer == null)
            return false;
        switch (_logic)
        {
            case MultipleChoiceLogic.Exact:
                return HasExactSameAnswer();
            case MultipleChoiceLogic.ContainsAll:
                return ContainsAllAnswersInRandomOrder();
            default:
                throw new ArgumentException("case not covered");
        }
    }

    private bool HasExactSameAnswer()
    {
        return CorrectAnswers == RecordedAnswer;
    }

    private bool ContainsAllAnswersInRandomOrder()
    {
        var c = 0; 
        var recordedAnswers = GetRecordedAnswers();
        foreach (var answer in recordedAnswers)
        {
            var correct = CorrectAnswers.Contains(answer);
            if (!correct)
                break;
            c++;
        }

        var AmountOfCorrectAnswers = SplitString(CorrectAnswers).Length;
        if (c == AmountOfCorrectAnswers)
            return true;
        return false;
    }

    public void ClearSession()
    {
        _sessionPersistor.ClearAnswerInSession(_sessionId);
        RecordedAnswer = null;
    }

    public Task DownloadForOffline()
    {
        throw new NotImplementedException();
    }

    public AssetType Type { get; }

    public void SetAnswer(string answer)
    {
        RecordedAnswer = answer;
        _sessionPersistor.SetStringAnswer(_sessionId, RecordedAnswer);
    }

    public void AddAnswer(string answer)
    {
        RecordedAnswer ??= "";
        answer += Seperator;
        if (!RecordedAnswer.Contains(answer))
        {
            RecordedAnswer += answer;
            _sessionPersistor.SetStringAnswer(_sessionId, RecordedAnswer);
        }
    }

    public void RemoveAnswer(string answer)
    {
        if (RecordedAnswer == null)
            return;
        answer += Seperator;
        if (RecordedAnswer.Contains(answer))
        {
            RecordedAnswer = RecordedAnswer.Replace(answer, "");
            _sessionPersistor.SetStringAnswer(_sessionId, RecordedAnswer);
        }
        if (string.IsNullOrEmpty(RecordedAnswer))
            RecordedAnswer = null;
    }

    public string[] GetRecordedAnswers()
    {
        return SplitString(RecordedAnswer);
    }

    private string[] SplitString(string source)
    {
        return source?.Split(Convert.ToChar(Seperator));
    }
}
