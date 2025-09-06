using FestivalGrounds.Core;
using FestivalGrounds.Data;
using FestivalGrounds.Facilities;
using FestivalGrounds.Economy;
using UnityEngine;
using UnityEngine.AI;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FestivalGrounds.Sims
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class SimController : MonoBehaviour, ISimControllable
    {
        [Header("AI Tuning")]
        [SerializeField] private float _arrivalThreshold = 0.5f;
        [SerializeField] private float _baseMovementSpeed = 3.5f;

        [Header("Wander Settings")]
        [SerializeField] private float _wanderRadius = 20f;
        [SerializeField] private float _wanderPauseDuration = 3f;

        // --- SERVICES & CONFIG ---
        private NavMeshAgent _agent;
        private EventBus _eventBus;
        private IFacilityTrackerService _facilityTracker;
        private ITimeService _timeService;
        private SimConfigSO _config;
        private SimAIState _currentState;
        private float _wanderStateTimer;
        private DateTime _interactionEndTime;
        private float _hunger, _bladder, _energy;
        private Tent _assignedTent;
        private IFacility _targetFacility;
        private IEconomyService _economyService;

        public void Initialize(SimConfigSO config)
        {
            _config = config;
            _hunger = UnityEngine.Random.Range(config.MaxNeedValue * 0.8f, config.MaxNeedValue);
            _bladder = UnityEngine.Random.Range(config.MaxNeedValue * 0.6f, config.MaxNeedValue * 0.9f);
            _energy = config.MaxNeedValue;
        }

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _eventBus = ServiceLocator.GetService<EventBus>();
            _facilityTracker = ServiceLocator.GetService<IFacilityTrackerService>();
            _timeService = ServiceLocator.GetService<ITimeService>();
            _economyService = ServiceLocator.GetService<IEconomyService>();

            _agent.speed = _baseMovementSpeed;
            _agent.stoppingDistance = _arrivalThreshold;
        }

        private void OnEnable()
        {
            _eventBus.Subscribe<TimeScaleChangedEvent>(OnTimeScaleChanged);
        }
        private void OnDisable()
        {
            _eventBus.Unsubscribe<TimeScaleChangedEvent>(OnTimeScaleChanged);
        }
        private void Start()
        {
            AssignAndGoToTent();
        }
        private void Update()
        {
            if (_timeService.IsPaused) return;

            UpdateNeeds();
            UpdateState();
        }
        private void OnTimeScaleChanged(TimeScaleChangedEvent e)
        {
            _agent.speed = _baseMovementSpeed * e.NewTimeScale;
        }

        public void PromoteFromQueue(Transform targetSpot)
        {
            Debug.Log($"[{gameObject.name}] Promoted from queue. New target: {targetSpot.name}", targetSpot);
            SetNavDestination(targetSpot.position, SimAIState.SeekingFacility);
        }

        #region State Machine

        private void SetState(SimAIState newState)
        {
            if (_currentState == newState) return;
            Debug.Log($"[{gameObject.name}] State Change: {_currentState} -> {newState}");
            _currentState = newState;
            _wanderStateTimer = 0f;
        }

        private void UpdateState()
        {
            if (_agent.pathPending) return;
            bool hasArrived = _agent.remainingDistance <= _agent.stoppingDistance;

            switch (_currentState)
            {
                case SimAIState.Wandering:
                    if (hasArrived)
                    {
                        if (CheckForCriticalNeeds()) return;
                        _wanderStateTimer += _timeService.DeltaTime;
                        if (_wanderStateTimer >= _wanderPauseDuration) FindNewWanderTarget();
                    }
                    break;
                case SimAIState.NavigatingToTent:
                case SimAIState.ReturningToTent:
                    if (hasArrived)
                    {
                        if (_currentState == SimAIState.NavigatingToTent) SetState(SimAIState.Wandering);
                        else SetState(SimAIState.Sleeping);
                    }
                    break;
                case SimAIState.SeekingFacility:
                    if (hasArrived)
                    {
                        Debug.Log($"[{gameObject.name}] Arrived at assigned spot. Entering UsingFacility state.", _targetFacility.transform);
                        _interactionEndTime = _timeService.CurrentTime.AddMinutes(_targetFacility.UseDurationMinutes);
                        SetState(SimAIState.UsingFacility);
                    }
                    break;
                case SimAIState.Queueing:
                    // Do nothing. Wait for the facility to call PromoteFromQueue.
                    break;
                case SimAIState.UsingFacility:
                    if (_timeService.CurrentTime >= _interactionEndTime) 
                    {
                        _targetFacility.OnFinishedUsing(this);
                        if (_targetFacility.Type == FacilityType.FoodStall) _hunger = _config.MaxNeedValue;
                        if (_targetFacility.Type == FacilityType.Toilet) _bladder = _config.MaxNeedValue;
                        _economyService.AddFunds(_targetFacility.RevenuePerUse);
                        Debug.Log($"[{gameObject.name}] Finished using {_targetFacility.Type}. Payed: {_targetFacility.RevenuePerUse}.");
                        SetState(SimAIState.Wandering);
                    }
                    break;
                case SimAIState.Sleeping:
                    _energy += _config.EnergyReplenishRate * _timeService.DeltaTime;
                    if (_energy >= _config.MaxNeedValue)
                    {
                        _energy = _config.MaxNeedValue;
                        SetState(SimAIState.Wandering);
                    }
                    break;
            }
        }

        private bool CheckForCriticalNeeds()
        {
            if (_currentState != SimAIState.Wandering) return false;

            FacilityType mostCriticalNeed = FacilityType.None;
            float mostCriticalValue = _config.NeedThreshold;

            if (_energy <= _config.NeedThreshold) { mostCriticalValue = _energy; mostCriticalNeed = FacilityType.Tent; }
            if (_bladder <= _config.NeedThreshold) { mostCriticalValue = _bladder; mostCriticalNeed = FacilityType.Toilet; }
            if (_hunger <= _config.NeedThreshold) { mostCriticalValue = _hunger; mostCriticalNeed = FacilityType.FoodStall; }

            if (mostCriticalNeed == FacilityType.None) return false;

            Debug.Log($"[{gameObject.name}] Critical need detected: {mostCriticalNeed}.");

            if (mostCriticalNeed == FacilityType.Tent)
            {
                if (_assignedTent != null && FindRandomPointNear(_assignedTent.transform.position, 1.5f, out Vector3 sleepDest))
                {
                    SetNavDestination(sleepDest, SimAIState.ReturningToTent);
                    return true;
                }
            }
            else
            {
                Transform facilityTransform = _facilityTracker.FindNearestFacility(mostCriticalNeed, transform.position);
                if (facilityTransform != null)
                {
                    _targetFacility = facilityTransform.GetComponent<IFacility>();
                    Debug.Log($"[{gameObject.name}] Found nearest {mostCriticalNeed}: {_targetFacility.transform.name}. Requesting use.", facilityTransform);

                    FacilityInteractionPoint interactionPoint = _targetFacility.RequestUse(this);
                    if (interactionPoint.Spot != null)
                    {
                        Debug.Log($"[{gameObject.name}] Facility returned spot: {interactionPoint.Spot.name} with type: {interactionPoint.Type}");

                        if (interactionPoint.Type == InteractionType.Use)
                            SetNavDestination(interactionPoint.Spot.position, SimAIState.SeekingFacility);
                        else
                            SetNavDestination(interactionPoint.Spot.position, SimAIState.Queueing);
                        return true;
                    }
                    else
                    {
                        Debug.LogWarning($"[{gameObject.name}] Facility {_targetFacility.transform.name} was found but returned a null spot (it is full).");
                    }
                }
                else
                {
                     Debug.LogWarning($"[{gameObject.name}] Critical need for {mostCriticalNeed} but no facility could be found.");
                }
            }
            return false;
        }

        private void SetNavDestination(Vector3 destination, SimAIState newState)
        {
            _agent.SetDestination(destination);
            SetState(newState);
        }
        #endregion

        #region Needs Management

        private void UpdateNeeds()
        {
            if (_config == null) return;
            _hunger -= _config.HungerDecayRate * _timeService.DeltaTime;
            _bladder -= _config.BladderDecayRate * _timeService.DeltaTime;
            if (_currentState != SimAIState.Sleeping)
                _energy -= _config.EnergyDecayRate * _timeService.DeltaTime;
        }

        private void AssignAndGoToTent()
        {
            _assignedTent = _facilityTracker.FindAvailableTent(transform.position);
            if (_assignedTent != null)
            {
                _assignedTent.AssignSim(this.gameObject);
                if (FindRandomPointNear(_assignedTent.transform.position, 3f, out Vector3 destination))
                    SetNavDestination(destination, SimAIState.NavigatingToTent);
                else
                    SetNavDestination(_assignedTent.transform.position, SimAIState.NavigatingToTent);
            }
            else
            {
                SetState(SimAIState.Wandering);
            }
        }

        private void FindNewWanderTarget()
        {
            _wanderStateTimer = 0f;
            if (FindRandomPointNear(transform.position, _wanderRadius, out Vector3 destination))
            {
                _agent.SetDestination(destination);
            }
        }

        private bool FindRandomPointNear(Vector3 origin, float radius, out Vector3 result)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius;
                randomDirection += origin;
                if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, radius, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }
            result = origin;
            return false;
        }

        #endregion

        #region Debugging

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
            string debugText = $"State: {_currentState}\n" +
                               $"H: {_hunger:F0} | B: {_bladder:F0} | E: {_energy:F0}";
            Handles.Label(transform.position + Vector3.up * 2.5f, debugText);
#endif
        }
        
        #endregion
    }
}