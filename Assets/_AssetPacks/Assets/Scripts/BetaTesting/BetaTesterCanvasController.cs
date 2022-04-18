using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using riddlehouse_libraries.environments;
using riddlehouse_libraries.environments.Models;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Riddlehunt.Beta.Environment.Controls
{
    public interface IBetaTesterController
    {
        public Task Configure();
        public void Display();
        public void Hide();
        public bool IsCurrentlyActive();
    }
    public class BetaTesterCanvasController : MonoBehaviour, IBetaTesterController
    {
        public static IBetaTesterController Factory(BetaTesterCanvasController prefab)
        {
            var behavioour = Instantiate(prefab.gameObject).GetComponent<BetaTesterCanvasController>();
            behavioour.InitializeFromFactory();
            return behavioour;
        }
        public class Dependencies
        {
            public ILoginHandler LoginHandler { get; set; }
            public IStandardButton ResetToDefaultButton { get; set; }
            public TextMeshProUGUI EnvironmentLabel { get; set; }
            public IEnvironmentListControls ListControls { get; set; }
            public IGetEnvironmentTargets EnvironmentTargetGetter { get; set; }
            public ISetEnvironmentTarget EnvironmentTargetSetter { get; set; }
        }

        [Inject] private LoginHandler loginHandler;
        [SerializeField] private StandardButtonBehaviour resetToDefaultButton;
        [SerializeField] private TextMeshProUGUI environmentLabel;
        [SerializeField] private EnvironmentListControls listControls;
        void InitializeFromFactory()
        {
            var environmentService = new EnvironmentService();
            SetDependencies(new Dependencies()
            {
                LoginHandler = loginHandler,
                ResetToDefaultButton = resetToDefaultButton,
                EnvironmentLabel = environmentLabel,
                ListControls = listControls,
                EnvironmentTargetGetter = environmentService,
                EnvironmentTargetSetter = environmentService
            });
            
            Hide();
        }
        
        public Dependencies _dependencies { get; private set; }
        public void SetDependencies(Dependencies dependencies)
        {
            dependencies.ResetToDefaultButton.Configure("Reset", ResetToDefault);
            _dependencies = dependencies;
        }

        public async Task Configure()
        {
            var isDefaultTarget = _dependencies.EnvironmentTargetGetter.CanSwitchEnvironment();
            var targets = new List<Target>();
            if (isDefaultTarget)
            {
                targets = await _dependencies.EnvironmentTargetGetter.GetPossibleTargets();
                if (targets.Count < 1)
                {
                    return;
                }
            }
            _dependencies.ListControls.Configure(new EnvironmentListControls.Config()
            {
                EnvironmentTargets = targets,
                ButtonAction = SetTarget
            });
            Display();
        }

        public void SetTarget(Target target)
        {
            _dependencies.EnvironmentTargetSetter.SetTarget(target.Url);
            _dependencies.EnvironmentLabel.text = "Current Environment: "+ target.Name;
            _dependencies.LoginHandler.Logout();
            Hide();
        }

        public void ResetToDefault()
        {
            _dependencies.EnvironmentTargetSetter.ResetToDefaultTarget();
            _dependencies.EnvironmentLabel.text = "Current Environment: Default";
            _dependencies.LoginHandler.Logout();
            Hide();
        }

        public void Display()
        {
            this.gameObject.SetActive(true);
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }

        public bool IsCurrentlyActive()
        {
            return this.gameObject.activeSelf;
        }
    }
}
