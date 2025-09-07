using UnityEngine;
using UnityEngine.InputSystem;

namespace FestivalGrounds.Core {
        
    public class InputPicker : MonoBehaviour
    {
        private PlayerControls actions;

        [SerializeField] private LayerMask selectableMask;
        [SerializeField] private Camera worldCamera;

        private void Awake()
        {
            actions = new PlayerControls();
        }

        private void OnEnable()
        {
            actions.Gameplay.Enable();
            actions.Gameplay.Select.performed += OnSelect;
            actions.Gameplay.Cancel.performed += OnCancel;
        }

        private void OnDisable()
        {
            actions.Gameplay.Select.performed -= OnSelect;
            actions.Gameplay.Cancel.performed -= OnCancel;
            actions.Gameplay.Disable();
        }

        private void OnSelect(InputAction.CallbackContext ctx)
        {
            var cam = worldCamera != null ? worldCamera : Camera.main;
            var ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

            Debug.Log($"OnSelect and camera not null");

            if (Physics.Raycast(ray, out var hit, 1000f, selectableMask))
            {
                if (hit.collider.TryGetComponent<ISelectable>(out var selectable))
                {
                    Debug.Log($"Raycast hit: {hit.collider.name} -> selecting {selectable.DisplayName}");
                    SelectionService.Instance.Select(selectable);
                    return;
                }
                Debug.Log("[InputPicker] Hit selectable layer, but no ISelectable found on parents.");
            }

            SelectionService.Instance.ClearSelection();
        }

        private void OnCancel(InputAction.CallbackContext ctx)
        {
            SelectionService.Instance.ClearSelection();
        }
    }
}