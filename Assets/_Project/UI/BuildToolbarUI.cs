using FestivalGrounds.Build;
using FestivalGrounds.Core;
using FestivalGrounds.Data;
using UnityEngine;

namespace FestivalGrounds.UI
{
    // This script is now refactored for maximum clarity in the Unity Inspector.
    // It avoids using array indices to prevent wiring errors.
    public class BuildToolbarUI : MonoBehaviour
    {
        // We now have explicit fields for each config. This is much clearer in the Inspector.
        [Header("Facility Configs")]
        [SerializeField] private FacilityConfigSO _toiletConfig;
        [SerializeField] private FacilityConfigSO _foodStallConfig;
        [SerializeField] private FacilityConfigSO _tentConfig;
        [SerializeField] private FacilityConfigSO _stageConfig;

        private EventBus _eventBus;

        private void Start()
        {
            _eventBus = ServiceLocator.GetService<EventBus>();
        }

        // Each button now gets its own explicit method.
        // This is much safer to wire up in the OnClick() event.

        public void OnBuildToiletClicked()
        {
            if (_toiletConfig == null) return;
            _eventBus.Publish(new StartBuildModeEvent(_toiletConfig));
        }

        public void OnBuildFoodStallClicked()
        {
            if (_foodStallConfig == null) return;
            _eventBus.Publish(new StartBuildModeEvent(_foodStallConfig));
        }

        public void OnBuildTentClicked()
        {
            if (_tentConfig == null) return;
            _eventBus.Publish(new StartBuildModeEvent(_tentConfig));
        }

        public void OnBuildStageClicked()
        {
            if (_stageConfig == null) return;
            _eventBus.Publish(new StartBuildModeEvent(_stageConfig));
        }

        public void OnCancelBuildClicked()
        {
            // We need to check if an event bus exists, in case the button is clicked before Start() is called.
            if (_eventBus == null) return;
            _eventBus.Publish(new CancelBuildModeEvent());
        }
    }
}

