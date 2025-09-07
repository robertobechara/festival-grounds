using FestivalGrounds.Core;
using FestivalGrounds.Data;
using FestivalGrounds.Economy;
using System.Collections;
using UnityEngine;

namespace FestivalGrounds.Sims
{
    public class SimSpawningService : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("The prefab to spawn for each Sim.")]
        [SerializeField] private GameObject _simPrefab;

        [Tooltip("How often, in game-time seconds, a new Sim should be spawned.")]
        [SerializeField] private float _spawnIntervalSeconds = 5f;

        [Tooltip("The maximum number of Sims allowed in the festival at one time.")]
        [SerializeField] private int _maxSims = 50;

        [Header("Sim Configuration")]
        [SerializeField] private SimConfigSO _simConfig;

        [Header("Economy Config")]
        [SerializeField] private EconomyConfigSO _economyConfig;

        [Header("Scene References")]
        [Tooltip("The transform where Sims will be spawned.")]
        [SerializeField] private Transform _spawnPoint;

        [Tooltip("The parent object for all spawned Sims to keep the hierarchy clean.")]
        [SerializeField] private Transform _simsParent;

        // --- SERVICES & STATE ---
        private EventBus _eventBus;
        private ITimeService _timeService; // Reference to our custom clock
        private IEconomyService _economyService; 
        private Coroutine _spawnLoop;
        

        private void Awake()
        {
            // Get all required services in Awake for reliability
            _eventBus = ServiceLocator.GetService<EventBus>();
            _timeService = ServiceLocator.GetService<ITimeService>();
            _economyService = ServiceLocator.GetService<IEconomyService>();

            _eventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnDestroy()
        {
            _eventBus?.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnGameStateChanged(GameStateChangedEvent e)
        {
            if (e.NewState == GameStateType.PlayMode)
            {
                if (_spawnLoop == null)
                {
                    _spawnLoop = StartCoroutine(SpawnLoop());
                }
            }
            else
            {
                if (_spawnLoop != null)
                {
                    StopCoroutine(_spawnLoop);
                    _spawnLoop = null;
                }
            }
        }

        private IEnumerator SpawnLoop()
        {
            while (_simsParent.childCount < _maxSims)
            {
                // We now wait for a manual timer driven by our custom clock,
                // instead of using WaitForSeconds().
                float timer = _spawnIntervalSeconds;
                while (timer > 0)
                {
                    timer -= _timeService.DeltaTime;
                    yield return null; // Wait for the next frame
                }
                SpawnSim();
            }
        }

        private void SpawnSim()
        {
            if (_simPrefab == null || _spawnPoint == null || _simsParent == null || _simConfig == null) return;

            _economyService.AddFunds(_economyConfig.TicketPrice);
            
            int _currentSimCount = _simsParent.childCount;
            GameObject simInstance = Instantiate(_simPrefab, _spawnPoint.position, Quaternion.identity, _simsParent);
            simInstance.name = $"Sim_{_currentSimCount}";

            if (simInstance.TryGetComponent(out SimController simController))
            {
                simController.Initialize(_simConfig);
            }
            Debug.Log($"Sim {_currentSimCount}/{_maxSims} has arrived. Ticket revenue: {_economyConfig.TicketPrice}.");
        }
    }
}