using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RHPackages.Core.Scripts.StepControllers;
using riddlehouse_libraries.products.Steps;
using UnityEngine;

namespace StepControllers
{
    public class DisplayRiddleAndSubmitAnswerStepController : IStepController
    {
        private IViewCollector _viewCollector;
        private IStepDataConverter _converter;
        
        public DisplayRiddleAndSubmitAnswerStepController(IViewCollector viewCollector, IStepDataConverter converter)
        {
            _converter = converter;
            _viewCollector = viewCollector;
        }

        private IGameViewCanvasController _mapCanvasController;
        private IDisplayRiddleAndSubmitAnswer _model;
        private Action _stepEnded;
        public async Task StartStep(IStep model, IGameViewCanvasController mapCanvasController, Action stepEnded)
        {
            _stepEnded = stepEnded;
            _model = _converter.ConvertToDisplayRiddleAndSubmitAnswer(model);
            _mapCanvasController = mapCanvasController;
            _storyView ??= await _viewCollector.StoryView(_model.Resource.StoryView.Address);
            _riddleView ??= await _viewCollector.RiddleView(_model.Resource.RiddleView.Address);
            _riddleView.Hide();
            ShowStory();
        }

        public string GetModelId()
        {
            if (_model != null)
                return _model.Id;
            return null;
        }

        private IStoryComponent _storyView;
        private void ShowStory()
        {
            var storyText = _model.StoryText.Text;
            if (!string.IsNullOrEmpty(storyText))
            {
                _mapCanvasController.AttachViewToCanvas(_storyView.GetComponentUIActions(), 1);
                _storyView.Configure(_model.StoryText.Text, "Videre", ShowRiddle);
                _storyView.GetComponentUIActions().Display();
            }
            else
            {
                ShowRiddle();
            }
        }
        private IRiddleTabComponent _riddleView;
        private void ShowRiddle()
        {
            _storyView.GetComponentUIActions().Hide();
            _mapCanvasController.AttachViewToCanvas(_riddleView, 2);
            
            _riddleView.Configure(_model.RiddleText.Text, _model.AnswerAsset, new List<Sprite>(), EndStep);

            _riddleView.Display();
        }
        
        private void EndStep()
        {
            _stepEnded.Invoke();
            _riddleView?.Hide();
        }
    }
}
