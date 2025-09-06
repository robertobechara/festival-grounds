using FestivalGrounds.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FestivalGrounds.Build
{
    public class BuildInputHandler : MonoBehaviour
    {
        private PlayerControls _playerControls;
        private EventBus _eventBus;
        
        private void Awake()
        {
            _playerControls = new PlayerControls();
            _eventBus = ServiceLocator.GetService<EventBus>();
        }

        private void OnEnable()
        {
            _playerControls.Build.Enable();
            _playerControls.Build.Cancel.performed += OnCancelPerformed;
        }

        private void OnDisable()
        {
            _playerControls.Build.Disable();
            _playerControls.Build.Cancel.performed -= OnCancelPerformed;
        }

        private void OnCancelPerformed(InputAction.CallbackContext context)
        {
            _eventBus.Publish(new CancelBuildModeEvent());
        }
    }
}