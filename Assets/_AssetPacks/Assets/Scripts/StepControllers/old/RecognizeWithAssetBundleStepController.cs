//TODO: AR Element
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using riddlehouse_libraries.products.models;
// using UnityEngine;
// public interface IRecognizeWithAssetBundleStepController
// {
//     public void StartStep(IRecognizeWithAssetsBundleStep huntStep, IHuntController huntController, bool lastRiddle);
// }
// public class RecognizeWithAssetBundleStepController : BaseStepController, IRecognizeWithAssetBundleStepController
// {
//     private bool _lastRiddle = false;
//     private StepControllerHelper helper;
//     
//     private static List<ComponentType> TypesInOrder = new List<ComponentType>()
//     {
//         ComponentType.Story, 
//         ComponentType.Scanning
//     };
//     
//     public IRecognizeWithAssetsBundleStep HuntStep { get; private set; }
//     private IHuntController _huntController;
//
//     private IStoryComponent _story;
//     private IHuntHomeComponent _huntHomeComponent;
//     private IEndHuntComponent _endHuntComponent;
//     private IScanningCorrectDisplayVideoComponent _scanningCorrect;
//
//     public RecognizeWithAssetBundleStepController(
//         ITabComponent tabComponent, 
//         IStoryComponent story, 
//         IHuntHomeComponent huntHomeComponent, 
//         IEndHuntComponent endHuntComponent, 
//         IScanningCorrectDisplayVideoComponent scanningCorrect
//         ) : base(StepType.RecognizeWithAssetBundle, tabComponent)
//     {
//         helper = new StepControllerHelper();
//
//         _story = story;
//         _huntHomeComponent = huntHomeComponent;
//         _endHuntComponent = endHuntComponent;
//         _scanningCorrect = scanningCorrect;
//
//         _views.Add(ComponentType.Story, story.GetComponentUIActions());
//         _views.Add(ComponentType.Scanning, scanningCorrect.GetComponentUIActions());
//         helper.AssureNecessaryStepViews(TypesInOrder, _views);
//         //todo: HuntHome and EndScreens are considered seperate to the step... this logic is wrong. - Philip
//         _views.Add(ComponentType.HuntHome, huntHomeComponent.GetComponentUIActions());
//         _views.Add(ComponentType.End, endHuntComponent.GetComponentUIActions());
//     }
//     public void StartStep(IRecognizeWithAssetsBundleStep huntStep, IHuntController huntController, bool lastRiddle)
//     {
//         if(huntStep == null) throw new ArgumentException("No huntStepAssets assigned");
//         if(huntStep.GetStepType() != StepType.RecognizeWithAssetBundle) throw new ArgumentException("HuntStep isn't of correct stepType");
//
//         _lastRiddle = lastRiddle;
//         _huntController = huntController;
//         _tabComponent.ConfigureForStepType(this);
//         HuntStep = huntStep;
//         var answerData = HuntStep.GetAnswerData();
//         if(answerData.HasAnswer())
//             ShowAssetInStep(ComponentType.Scanning);
//         else 
//             ShowAssetInStep(GetFirstStepTypeToShow());
//         
//         huntController.MarkStepStarted(HuntStep.GetStepId());
//
//     }
//     public override void ShowAssetInStep(ComponentType type)
//     {
//         if (HuntStep == null)
//             throw new ArgumentException("Please call start step before attempting to show an asset");
//         switch (type)
//         {
//             case ComponentType.Story:
//                 if (_story == null)
//                     throw new ArgumentException("No STORY assigned");
//                 _story.Configure(HuntStep.GetStoryText(), "Scan!", () => PrepScan());
//                 break;
//             case ComponentType.End:
//                 if (_endHuntComponent == null)
//                     throw new ArgumentException("No END assigned");
//                 //Supply an end text
//                 _endHuntComponent.Configure("", () => _huntController.EndHunt(true));
//                 break;
//             case ComponentType.HuntHome:
//                 break;
//             case ComponentType.Scanning:
//                 if (_scanningCorrect == null)
//                     throw new ArgumentException("No scanningCorrect assigned");
//                 IScanningCorrectDisplayVideoActions scanningCorrectDisplayVideoActions = _scanningCorrect.GetScanningCorrectDisplayComponentActions();
//                     scanningCorrectDisplayVideoActions.Configure(
//                         HuntStep.GetVideoToPlay(), 
//                         _lastRiddle? "Afslut" : "Næste gåde!",
//                         () => EndStep());
//                 break;
//             default:
//                 throw new ArgumentException("No such step in asset");
//         }
//         base.ShowAssetInStep(type);
//     }
//
//     private void PrepScan()
//     {
//         _story.GetComponentUIActions().Hide();
//         _huntHomeComponent.ConfigureARElements(MorphableARCamera.morph, HuntStep.GetImageLibraryReference(), () =>
//         {
//             ShowAssetInStep(ComponentType.Scanning);
//             var answerData = HuntStep.GetAnswerData();
//             ((BooleanAnswerData)answerData).SetAnswer(true);
//             _scanningCorrect.GetScanningCorrectDisplayComponentActions().ScanningSuccessAction();
//         });
//     }
//     public override void EndStep()
//     {
//         if (_lastRiddle)
//         {
//             ShowAssetInStep(ComponentType.End);
//         }
//         else
//         {
//             base.EndStep();
//             ShowAssetInStep(ComponentType.HuntHome);
//         }
//         _huntController.MarkStepEnded(HuntStep.GetStepId());
//     }
//     public override List<ComponentType> GetTypesInOrder()
//     {
//         return TypesInOrder;
//     }
//
//     public override ComponentType GetFirstStepTypeToShow()
//     {
//         return TypesInOrder[0];
//     }
// }
