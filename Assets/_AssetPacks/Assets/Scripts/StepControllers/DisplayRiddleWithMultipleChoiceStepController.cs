using System;
using System.Threading.Tasks;
using Helpers;
using riddlehouse_libraries.products.Assets.AnswerAssets;
using riddlehouse_libraries.products.Steps;
using StepControllers;
using UnityEngine;

namespace RHPackages.Core.Scripts.StepControllers
{
    public class DisplayRiddleWithMultipleChoiceStepController : IStepController
    {
        private readonly IViewCollector _viewCollector;
        private readonly IStepDataConverter _converter;
        private readonly ISpriteHelper _spriteHelper;
        public DisplayRiddleWithMultipleChoiceStepController(IViewCollector viewCollector, IStepDataConverter converter, ISpriteHelper spriteHelper)
        {
            _converter = converter;
            _viewCollector = viewCollector;
            _spriteHelper = spriteHelper;
        }
        
        private IGameViewCanvasController _gameCanvasController;
        private IDisplayRiddleWithMultipleChoice _model;
        private Action _stepEnded;
        private IWispContainerAnswerView _answerView;
        public async Task StartStep(IStep model, IGameViewCanvasController mapCanvasController, Action stepEnded)
        {
            _stepEnded = stepEnded;
            _model = _converter.ConvertToDisplayRiddleWithMultipleChoice(model);
            _gameCanvasController = mapCanvasController;
            _answerView ??= await _viewCollector.AnswerView(_model.Resource.AnswerView.Address);
            await ShowAnswerView();
        }

        private async Task ShowAnswerView()
        {
            var characterColor = new Color32(_model.Resource.CharacterColor.R, _model.Resource.CharacterColor.G,
                _model.Resource.CharacterColor.B, _model.Resource.CharacterColor.A);
            var rawIcon = await _model.Resource.CharacterIcon.GetIcon();
            var characterIcon = _spriteHelper.GetSpriteFromByteArray(rawIcon);
            _gameCanvasController.AttachViewToCanvas(_answerView, 1);
            _answerView.Configure(
                _model.AnswerAsset, 
                _model.RiddleText.Text, 
                characterColor,
                characterIcon, 
                EndStep,
                EndStep
            );
            _answerView.Display();
        }
        
        public string GetModelId()
        {
            return _model.Id;
        }
        
        private void EndStep()
        {
            _answerView.Hide();
            _stepEnded.Invoke();
        }
    }
}
