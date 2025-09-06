using FestivalGrounds.Core;
using FestivalGrounds.Data;
using FestivalGrounds.Economy;
using FestivalGrounds.Facilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace FestivalGrounds.Build
{
    public class BuildService : MonoBehaviour, IBuildService
    {
        [Header("Scene References")]
        [Tooltip("The parent transform where all runtime-placed facilities will be nested.")]
        [SerializeField] private Transform _placedFacilitiesParent;

        [Header("Settings")]
        [SerializeField] private LayerMask _terrainLayer;
        public bool IsBuilding { get; private set; }

        private EventBus _eventBus;
        private IEconomyService _economyService;
        private IFacilityTrackerService _facilityTracker;
        private Camera _mainCamera;
        private PlayerControls _playerControls;

        private FacilityConfigSO _currentFacilityConfig;
        private GameObject _ghostInstance;

        private void Awake()
        {
            _playerControls = new PlayerControls();
            _eventBus = ServiceLocator.GetService<EventBus>();
            _economyService = ServiceLocator.GetService<IEconomyService>();
            _facilityTracker = ServiceLocator.GetService<IFacilityTrackerService>();

            ServiceLocator.Register<IBuildService>(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<IBuildService>();
        }

        private void OnEnable()
        {
            _playerControls.Build.Enable();
            _eventBus.Subscribe<StartBuildModeEvent>(OnStartBuilding);
            _eventBus.Subscribe<CancelBuildModeEvent>(OnCancelBuild);
        }

        private void OnDisable()
        {
            _playerControls.Build.Disable();
            _eventBus.Unsubscribe<StartBuildModeEvent>(OnStartBuilding);
            _eventBus.Unsubscribe<CancelBuildModeEvent>(OnCancelBuild);
        }

        private void Update()
        {
            if (!IsBuilding || _ghostInstance == null) return;

            if (_mainCamera == null) _mainCamera = Camera.main;
            if (_mainCamera == null) return;

            bool isOverUI = EventSystem.current.IsPointerOverGameObject();

            // Update ghost visibility based on UI hover
            if (isOverUI != !_ghostInstance.activeSelf)
            {
                _ghostInstance.SetActive(!isOverUI);
            }

            // If over UI, do nothing else
            if (isOverUI) return;

            // If we are not over the UI, ensure the ghost is visible.
            if (!_ghostInstance.activeSelf)
                _ghostInstance.SetActive(true);

            // The rest of the raycast logic is now inside an else block.
            Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, _terrainLayer))
            {
                _ghostInstance.transform.position = hit.point;
            } 
            
            // Check for placement click directly here
            if (_playerControls.Build.Place.WasPerformedThisFrame())
            {
                OnConfirmBuild();
            }
        }

        private void OnStartBuilding(StartBuildModeEvent e)
        {
            if (IsBuilding) ExitBuildMode();
            if (_economyService.CurrentBudget < e.FacilityConfig.Cost) return;

            IsBuilding = true;
            _currentFacilityConfig = e.FacilityConfig;
            _ghostInstance = Instantiate(_currentFacilityConfig.GhostPrefab);
        }

        private void OnConfirmBuild()
        {
            if (!_economyService.TrySpend(_currentFacilityConfig.Cost)) return;

            GameObject newFacilityGO = Instantiate(_currentFacilityConfig.Prefab, _ghostInstance.transform.position, Quaternion.identity, _placedFacilitiesParent);

            if (newFacilityGO.TryGetComponent(out IFacility facilityComponent))
            {
                facilityComponent.Initialize(_currentFacilityConfig);
                _facilityTracker.Register(facilityComponent as Component, _currentFacilityConfig.Type);
            }
            else
            {
                Debug.LogWarning($"The placed prefab for '{_currentFacilityConfig.Name}' is missing a component that implements IFacility.");
            }

            ExitBuildMode();
        }

        private void OnCancelBuild(CancelBuildModeEvent e)
        {
            if (!IsBuilding) return;
            ExitBuildMode();
        }
        
        private void ExitBuildMode()
        {
            IsBuilding = false;
            if (_ghostInstance != null)
            {
                Destroy(_ghostInstance);
            }
            _ghostInstance = null;
            _currentFacilityConfig = null;
        }
    }
}

