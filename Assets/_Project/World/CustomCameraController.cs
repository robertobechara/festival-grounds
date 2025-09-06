using UnityEngine;
using UnityEngine.InputSystem;
using FestivalGrounds.Core;

namespace FestivalGrounds.World
{
    /// <summary>
    /// A custom, dependency-free "god camera" controller.
    /// Manages panning, rotating, elevating, and zooming.
    /// To be placed on the top-level Camera Rig object.
    /// </summary>
    public class CustomCameraController : MonoBehaviour
    {
        [Header("Camera Reference")]
        [Tooltip("The main camera, which will be controlled for zooming.")]
        [SerializeField] private Camera _mainCamera;

        [Header("Movement Settings")]
        [SerializeField] private float _panSpeed = 30f;
        [SerializeField] private float _elevateSpeed = 30f;
        [SerializeField] private float _movementSmoothing = 5f;

        [Header("Rotation Settings")]
        [SerializeField] private float _rotationSpeed = 10f;
        [SerializeField] private float _pitchSmoothing = 8f;
        [SerializeField] private float _yawSmoothing = 8f;
        [SerializeField] private Vector2 _pitchLimits = new Vector2(10f, 85f);

        [Header("Zoom Settings")]
        [SerializeField] private float _zoomSpeed = 5f;
        [SerializeField] private float _zoomSmoothing = 8f;
        [SerializeField] private Vector2 _zoomLimits = new Vector2(20f, 80f); // Field of View limits

        // Input
        private PlayerControls _playerControls;
        private Vector2 _panInput;
        private float _elevateInput;
        private Vector2 _rotateInput;
        private float _zoomInput;
        private bool _isRotationEnabled;

        // State
        private Vector3 _targetPosition;
        private float _targetYaw;
        private float _targetPitch;
        private float _targetFov;

        private void Awake()
        {
            _playerControls = new PlayerControls();
            
            // Initialize targets to current transform state
            _targetPosition = transform.position;
            _targetYaw = transform.rotation.eulerAngles.y;
            // Assumes a child pivot object exists for pitch control
            _targetPitch = transform.GetChild(0).localRotation.eulerAngles.x; 

            if(_mainCamera != null)
            {
                _targetFov = _mainCamera.fieldOfView;
            }
        }

        private void OnEnable()
        {
            _playerControls.Camera.Enable();
        }

        private void OnDisable()
        {
            _playerControls.Camera.Disable();
        }

        private void Update()
        {
            HandleInput();
            Move();
            Rotate();
            Zoom();
        }
        
        private void HandleInput()
        {
            _panInput = _playerControls.Camera.Pan.ReadValue<Vector2>();
            _elevateInput = _playerControls.Camera.Elevate.ReadValue<float>();
            _isRotationEnabled = _playerControls.Camera.RotateToggle.IsPressed();
            _rotateInput = _isRotationEnabled ? _playerControls.Camera.Rotate.ReadValue<Vector2>() : Vector2.zero;
            _zoomInput = _playerControls.Camera.Zoom.ReadValue<Vector2>().y;
        }

        private void Move()
        {
            Vector3 forward = transform.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 right = transform.right;
            right.y = 0;
            right.Normalize();

            // Calculate movement vectors scaled by their respective speeds
            Vector3 panMovement = (forward * _panInput.y + right * _panInput.x) * _panSpeed;
            Vector3 elevateMovement = Vector3.up * _elevateInput * _elevateSpeed;

            // Apply the combined, scaled movement to the target position
            _targetPosition += (panMovement + elevateMovement) * Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _movementSmoothing);
        }

        private void Rotate()
        {
            _targetYaw += _rotateInput.x * _rotationSpeed * Time.deltaTime;
            _targetPitch -= _rotateInput.y * _rotationSpeed * Time.deltaTime;
            _targetPitch = Mathf.Clamp(_targetPitch, _pitchLimits.x, _pitchLimits.y);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, _targetYaw, 0), Time.deltaTime * _yawSmoothing);
            
            Transform pivot = transform.GetChild(0); // Pivot handles Pitch (X-axis)
            pivot.localRotation = Quaternion.Slerp(pivot.localRotation, Quaternion.Euler(_targetPitch, 0, 0), Time.deltaTime * _pitchSmoothing);
        }

        private void Zoom()
        {
            if (_mainCamera == null) return;
            
            float scrollDelta = -_zoomInput * 0.1f;
            _targetFov += scrollDelta * _zoomSpeed;
            _targetFov = Mathf.Clamp(_targetFov, _zoomLimits.x, _zoomLimits.y);
            
            _mainCamera.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView, _targetFov, Time.deltaTime * _zoomSmoothing);
        }
    }
}