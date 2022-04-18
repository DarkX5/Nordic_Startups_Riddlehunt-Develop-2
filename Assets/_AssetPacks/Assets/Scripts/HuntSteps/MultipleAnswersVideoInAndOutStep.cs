using System;
using System.Collections;
using System.Collections.Generic;
using riddlehouse_libraries.products.Assets;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using riddlehouse_libraries.products.models.DTOs;
using UnityEngine;

public interface IMultipleAnswersVideoInAndOutStep
{
    public string GetStepId();
    public string GetStepTitle();
    public IAnswerAsset GetAnswerData();
    public StepType GetStepType();
    public string GetIntroVideoUrl();
    public string GetOutroVideoUrl();
}
public class MultipleAnswersVideoInAndOutStep : IMultipleAnswersVideoInAndOutStep, IHuntStep, IInternalHuntStep
{
    private string _introVideoUrl;
    private string _outroVideoUrl;
    private IAnswerAsset _iAnswerData;

    private readonly Action<bool> _isReady;
    private bool _hasFailed = false;
    private bool _downloadComplete = false;
    
    private readonly HuntStep _step;
    private readonly ITextGetter _textGetter;
    private readonly IImageGetter _imageGetter;
    
    public MultipleAnswersVideoInAndOutStep(HuntStep step, ITextGetter  textGetter, IImageGetter imageGetter, Action<bool> isReady)
    {
        if (step.Type != StepType.MultipleAnswersVideoInAndOut)
            throw new ArgumentException("Wrong step type in constructor");
        var helper = new AssetGetterHelper();
        _textGetter = textGetter;
        _imageGetter = imageGetter;
        
        _isReady = isReady;
        
        _step = step;

        _introVideoUrl = helper.GetAssetUrl(step, AssetType.VideoIn);
        _outroVideoUrl = helper.GetAssetUrl(step, AssetType.VideoOut);
        
        var answerHuntAsset = helper.GetAnswerHuntAsset(step);
        _iAnswerData = AnswerAssetHelper.Factory(step.Id, answerHuntAsset.Type, _textGetter, answerHuntAsset.Url, DownloadComplete, imageGetter);
        _iAnswerData.SetHuntSessionPersistor(new HuntSessionPersistor(), step.Id);
    }
    private void DownloadComplete(bool success)
    {
        if (!_hasFailed && !_downloadComplete)
        {
            if (!success)
            {
                _isReady.Invoke(false);
                _textGetter.DisposeSelf();
                _imageGetter.DisposeSelf();
                _hasFailed = true;
                return;
            }
            _downloadComplete = true;
            _isReady.Invoke(true);
            _textGetter.DisposeSelf();
            _imageGetter.DisposeSelf();
        }
    }

    public string GetStepId()
    {
        return _step.Id;
    }
    public string GetStepTitle()
    {
        return _step.Title;
    }

    public StepType GetStepType()
    {
        return _step.Type;
    }
    //todo: this is only temporary code, delete me when removing internal types!
    public bool IsExpectedStepType(StepType steptype)
    {
        return steptype == GetStepType();
    }
    //todo: this is only temporary code, delete me when removing internal types!
    public bool ValidateAssetConfiguration()
    {
        return _downloadComplete;
    }
    //todo: this is only temporary code, delete me when removing internal types!
    public bool DidBypassValidation()
    {
        return true;
    }
    //todo: this is only temporary code, delete me when removing internal types!
    public void SetBypassvalidation(string url)
    {
        throw new System.NotImplementedException();
    }

    public StepCondition GetCondition()
    {
        return _step.Condition;
    }

    public bool HasAnswer()
    {
        return _iAnswerData.HasAnswer();
    }

    public void ClearSessionAndClearAnswer()
    {
        _iAnswerData.ClearSession();
    }

    public IAnswerAsset GetAnswerData()
    {
        return _iAnswerData;
    }

    public string GetIntroVideoUrl()
    {
        return _introVideoUrl;
    }

    public string GetOutroVideoUrl()
    {
        return _outroVideoUrl;
    }
}
