using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using riddlehouse_libraries.products.Assets;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using riddlehouse_libraries.products.models.DTOs;
using UnityEngine;
public interface IDisplayRiddleAndSubmitAnswerStep
{
    public string GetStepId();
    public string GetStepTitle();
    public string GetRiddleText();
    public string GetStoryText();
    public IAnswerAsset GetAnswerData();
    public StepType GetStepType();
    public List<Sprite> GetRiddleImages();
}
public class DisplayRiddleAndSubmitAnswerStep : IDisplayRiddleAndSubmitAnswerStep, IHuntStep, IInternalHuntStep
{
    private IAnswerAsset _iAnswerData;
    private ITextAsset _storyText;
    private ITextAsset _riddleText;
    private ImageListAsset _imageList;
    
    private int _readyCount; //how many assets have we successfully downloaded
    private int targetCount; //how many assets to we expect to download.

    private readonly object _myLock;
    private readonly Action<bool> _isReady;
    private bool _hasFailed = false;
    private bool _downloadComplete = false;
    
    private readonly HuntStep _step;
    private readonly ITextGetter _textGetter;
    private readonly IImageGetter _imageGetter;

    public DisplayRiddleAndSubmitAnswerStep(HuntStep step, ITextGetter  textGetter, IImageGetter imageGetter, Action<bool> isReady)
    {
        if (step.Type != StepType.DisplayRiddleAndSubmitAnswer)
            throw new ArgumentException("Wrong step type in constructor");
        var helper = new AssetGetterHelper();
        _textGetter = textGetter;
        _imageGetter = imageGetter;
        
        _readyCount = 0;
        _myLock = new object();
        _isReady = isReady;
        
        _step = step;
        targetCount = 5; //story, riddle, answer, imageURls, Images
        //Add the count that's in the imageLink, after getting the imageLinks file..
        
        // try
        // {
            var imageAssetUrl = helper.GetAssetUrl(step, AssetType.ImageList);
            _textGetter.GetText(imageAssetUrl, false, (imageUrls) =>
            {
                _readyCount++; //mark that the urls have been downloaded.
                var imageLinks = JsonConvert.DeserializeObject<List<string>>(imageUrls);
                if (imageLinks == null)
                    imageLinks = new List<string>();
                _imageList = new ImageListAsset(_imageGetter, imageLinks, DownloadComplete);
            });

            _storyText = new StoryTextAsset(_textGetter, helper.GetAssetUrl(_step, AssetType.StoryText),
                DownloadComplete);
            
            _riddleText = new RiddleTextAsset(_textGetter, helper.GetAssetUrl(_step, AssetType.RiddleText),
                DownloadComplete);

            var answerHuntAsset = helper.GetAnswerHuntAsset(step);
            _iAnswerData = AnswerAssetHelper.Factory(step.Id, answerHuntAsset.Type, _textGetter, answerHuntAsset.Url, DownloadComplete);
        // }
        // catch
        // {
        //     DownloadComplete(false);
        // }
    }
    private void DownloadComplete(bool success)
    {
        //something failed so we inform the huntAssetGetter that it failed, and ditch the rest.
        //-- all downloads happen in parallel, so we need to abort if something fails on the way.
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
            
            lock (_myLock)
            {
                _readyCount++;
                if (_readyCount >= targetCount)
                {
                    _downloadComplete = true;
                    _isReady.Invoke(true);
                    _textGetter.DisposeSelf();
                    _imageGetter.DisposeSelf();
                }
            }
        }
    }
    
    public string GetStepTitle()
    {
        return _step.Title;
    }

    public string GetStepId()
    {
        return _step.Id;
    }

    public StepCondition GetCondition()
    {
        return _step.Condition;
    }

    public StepType GetStepType()
    {
        return _step.Type;
    }

    public bool HasAnswer()
    {
        return _iAnswerData.HasAnswer();
    }

    public void ClearSessionAndClearAnswer()
    {
        _iAnswerData.ClearSession();
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
        throw new NotImplementedException();
    }

    public string GetRiddleText()
    {
        return _riddleText.GetText();
    }

    public string GetStoryText()
    {
        return _storyText.GetText();
    }

    public IAnswerAsset GetAnswerData()
    {
        return _iAnswerData;
    }
    
    public List<Sprite> GetRiddleImages()
    {
        return _imageList.GetImages();
    }
    
}
