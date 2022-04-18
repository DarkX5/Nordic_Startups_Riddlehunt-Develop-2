using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using riddlehouse_libraries.products.AssetTypes;
using riddlehouse_libraries.products.models;
using UnityEngine;
// using UnityEngine.XR.ARSubsystems; //TODO: AR Element

public interface IHuntAssetGetter
{
    public void GetHuntAssets(HuntFlow huntFlow, string productId, string feedbackLink, Action<IHuntSteps> huntDataIsReady);
}

public class HuntAssetGetter : IHuntAssetGetter
{
    public static IHuntAssetGetter Factory(MonoBehaviour monoBehaviour)
    {
        if (monoBehaviour == null)
        {
            throw new ArgumentException("monobehavior missing in huntAssetGetter");
        }

        var huntAssetGetterBehaviour = new HuntAssetGetterBehaviour(monoBehaviour);
        var huntAssetGetter = new HuntAssetGetter(huntAssetGetterBehaviour);

        return huntAssetGetter;
    }

    private readonly IHuntAssetGetterActions _iHuntAssetGetterActions;
    public HuntAssetGetter(IHuntAssetGetterActions huntAssetGetterActions)
    {
        _iHuntAssetGetterActions = huntAssetGetterActions;
    }
    public void GetHuntAssets(HuntFlow huntFlow, string productId, string feedbackLink, Action<IHuntSteps> huntDataIsReady)
    {
        _iHuntAssetGetterActions.GetHuntAssets(huntFlow, productId, feedbackLink, huntDataIsReady);
    }
}

public interface IHuntAssetGetterActions
{
    public void GetHuntAssets(HuntFlow huntFlow, string productId, string feedbackLink, Action<IHuntSteps> huntDataIsReady);
}

public class HuntAssetGetterBehaviour : IHuntAssetGetterActions
{
    private HuntSteps _huntSteps;
    private List<IInternalHuntStep> _unsafeAssetcollection;
    private MonoBehaviour _monoBehaviour;
    public HuntAssetGetterBehaviour(MonoBehaviour monoBehaviour)
    {
        _monoBehaviour = monoBehaviour;
    }
    
    public void GetHuntAssets(HuntFlow huntFlow, string productId, string feedbackLink, Action<IHuntSteps> huntDataIsReady)
    {
        _huntSteps = new HuntSteps(feedbackLink, productId);
        _unsafeAssetcollection = new List<IInternalHuntStep>();
        foreach (var step in huntFlow.Steps)
        {
            switch (step.Type)
            {
                case StepType.DisplayStoryAndDone:
                    //NOTE: next time anything in this stepModel is changed, we should update it away from internals, and into the new self-contained model! - Philip Haugaard.
                    ConfigureDisplayStoryAndDoneHuntStep(step);
                break;
                //TODO: AR Element
                // case StepType.RecognizeWithAssetBundle:
                //     _unsafeAssetcollection.Add(new RecognizeWithAssetBundleStep(step, 
                //         TextGetter.Factory(_monoBehaviour), 
                //         AssetBundleGetter.Factory(_monoBehaviour), 
                //         new AssetBundleHelper<XRReferenceImageLibrary>(_monoBehaviour),
                //         (success) =>
                //         {
                //             
                //         }));
                //     break;
                case StepType.DisplayRiddleAndSubmitAnswer:
                    _unsafeAssetcollection.Add(new DisplayRiddleAndSubmitAnswerStep(step,
                        TextGetter.Factory(_monoBehaviour), 
                        ImageGetter.Factory(_monoBehaviour),
                        (success) =>
                        {

                        }));
                    break;
                case StepType.HuntResolutionAndEnd:
                    _unsafeAssetcollection.Add( new HuntResolutionAndEndStep(step, 
                        TextGetter.Factory(_monoBehaviour), 
                        ImageGetter.Factory(_monoBehaviour), 
                        (success) =>
                        {
                            
                        }));
                    break;
                case StepType.MultipleAnswersVideoInAndOut:
                    _unsafeAssetcollection.Add(new MultipleAnswersVideoInAndOutStep(step, 
                        TextGetter.Factory(_monoBehaviour), 
                        ImageGetter.Factory(_monoBehaviour), 
                        (success) =>
                        {
                            
                        }));
                    break;
                case StepType.ResolutionVideoAndEnd:
                    _unsafeAssetcollection.Add(new ResolutionVideoAndEndStep(step, 
                        TextGetter.Factory(_monoBehaviour), 
                        (success) =>
                        {
                            
                        }));
                    break;
            }
        }
        _monoBehaviour.StartCoroutine(WaitForData(huntDataIsReady));
    }
    //NOTE: next time anything in this stepModel is changed, we should update it away from internals, and into the new self-contained model! - Philip Haugaard.
    private void ConfigureDisplayStoryAndDoneHuntStep(HuntStep step)
    { 
        int i = _unsafeAssetcollection.Count;
        _unsafeAssetcollection.Add(new InternalDisplayStoryAndDoneHuntStep());
        ((InternalDisplayStoryAndDoneHuntStep)_unsafeAssetcollection[i]).StepTitle = step.Title;
        ((InternalDisplayStoryAndDoneHuntStep)_unsafeAssetcollection[i]).StepId = step.Id;
        ((InternalDisplayStoryAndDoneHuntStep)_unsafeAssetcollection[i]).Condition = step.Condition;

        foreach (var asset in step.Assets)
        {
            var textGetter = TextGetter.Factory(_monoBehaviour);
            switch (asset.Type)
            {
                case AssetType.StoryText: //string
                    textGetter.GetText(asset.Url, false,
                        (txt) => { ((InternalDisplayStoryAndDoneHuntStep)_unsafeAssetcollection[i]).StoryText = txt; });
                    break;
            }
        }
    }

    IEnumerator WaitForData(Action<HuntSteps> stepDataIsReady)
    {   
        bool allStepsValidated = false;
        while (!allStepsValidated)
        {
            foreach (var stepData in _unsafeAssetcollection)
            {
                bool currentStepValidation = stepData.ValidateAssetConfiguration();
                if (currentStepValidation)
                {
                    allStepsValidated = true;
                }
                else
                {
                    allStepsValidated = false;
                    break;
                }
            }
            Debug.Log("Waiting for data");
            yield return new WaitForSeconds(0.5f);
        }
        _huntSteps.ConvertInternalStepdata(_unsafeAssetcollection);
        stepDataIsReady.Invoke(_huntSteps);
    }
}
