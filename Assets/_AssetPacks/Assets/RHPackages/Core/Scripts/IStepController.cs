using System;
using System.Threading.Tasks;
using riddlehouse_libraries.products.Steps;

namespace RHPackages.Core.Scripts.StepControllers
{
    public interface IStepController
    {
        public Task StartStep(IStep model, IGameViewCanvasController mapCanvasController, Action stepEnded);
        public string GetModelId();
    }
}
