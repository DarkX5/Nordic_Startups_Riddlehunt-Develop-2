using System;
using System.Collections;
using System.Collections.Generic;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using riddlehouse_libraries.products.models.DTOs;
using UnityEngine;

public interface IResolutionVideoAndEndStep
{
    public string GetStepId();
    public string GetResolutionVideoLink();
    public string GetEndText();
    public void MarkAnswered();
}

public class ResolutionVideoAndEndStep : IResolutionVideoAndEndStep, IHuntStep, IInternalHuntStep
{
    private string ResolutionVideoLink { get; set; }
    private string EndText { get; set; }
    private HuntStep HuntStep { get; set; }
    private readonly ITextGetter _textGetter;
    
    private BooleanAnswerData _answerData;
    
    private bool _hasFailed = false;
    private bool _downloadComplete = false;
    private Action<bool> _isReady;
    
    public ResolutionVideoAndEndStep(HuntStep step, ITextGetter textGetter, Action<bool> isReady)
    {
        if (step.Type != StepType.ResolutionVideoAndEnd)
            throw new ArgumentException("Wrong step type in constructor");
        var helper = new AssetGetterHelper();
        
        HuntStep = step;
        _textGetter = textGetter;
        _isReady = isReady;
        _textGetter = textGetter;

        _answerData = new BooleanAnswerData(step.Id);
        try
        {
            ResolutionVideoLink = helper.GetAssetUrl(step, AssetType.VideoToPlay);
            var endTextLink = helper.GetAssetUrl(step, AssetType.EndText);
            _textGetter.GetText(endTextLink, false, (value) =>
            {
                EndText = value;
                DownloadComplete(true);
            });
        }
        catch
        {
            DownloadComplete(false);
        }

    }
    
    private void DownloadComplete(bool success)
    {
        if (!_hasFailed && !_downloadComplete)
        {
            if (!success)
            {
                _isReady.Invoke(false);
                _textGetter.DisposeSelf();
                _hasFailed = true;
                return;
            }
            _downloadComplete = true;
            _isReady.Invoke(true);
            _textGetter.DisposeSelf();
        }
    }
    
    public string GetResolutionVideoLink()
    {
        return ResolutionVideoLink;
    }

    public string GetEndText()
    {
        return EndText;
    }

    public void MarkAnswered()
    {
        _answerData.SetAnswer(true);
    }

    public string GetStepTitle()
    {
        return HuntStep.Title;
    }

    public StepType GetStepType()
    {
        return HuntStep.Type;
    }
    public string GetStepId()
    {
        return HuntStep.Id;
    }
    public StepCondition GetCondition()
    {
        return HuntStep.Condition;
    }

    public bool IsExpectedStepType(StepType steptype)
    {
        return HuntStep.Type == steptype;
    }

    public bool ValidateAssetConfiguration()
    {
        return _downloadComplete;
    }

    public bool DidBypassValidation()
    {
        return true;
    }

    public void SetBypassvalidation(string url)
    {
        throw new System.NotImplementedException();
    }

    public bool HasAnswer()
    {
        return _answerData.HasCorrectAnswer();
    }

    public void ClearSessionAndClearAnswer()
    {
        _answerData.ClearSession();
    }
}
