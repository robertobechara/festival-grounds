using FestivalGrounds.Core;
using FestivalGrounds.Data;
using FestivalGrounds.Economy;
using FestivalGrounds.Facilities;
using FestivalGrounds.Sims;
using System.IO;
using System.Linq; 
using UnityEngine;

namespace FestivalGrounds.Game
{
    public class SaveLoadService : ISaveLoadService
    {
        // --- DEPENDENCIES ---
        private readonly IEconomyService _economyService;
        private readonly ITimeService _timeService;
        private readonly IFacilityTrackerService _facilityTracker;
        private readonly FacilityDatabaseSO _facilityDatabase;
        private readonly SimConfigSO _simConfig;
        private readonly GameObject _simPrefab;
        private readonly Transform _placedFacilitiesParent;
        private readonly Transform _simsParent;
        
        private readonly string _savePath;

        public SaveLoadService(FacilityDatabaseSO facilityDb, SimConfigSO simConfig, GameObject simPrefab, Transform facilitiesParent, Transform simsParent)
        {
            _economyService = ServiceLocator.GetService<IEconomyService>();
            _timeService = ServiceLocator.GetService<ITimeService>();
            _facilityTracker = ServiceLocator.GetService<IFacilityTrackerService>();

            _facilityDatabase = facilityDb;
            _simPrefab = simPrefab;
            _simConfig = simConfig; 
            _placedFacilitiesParent = facilitiesParent;
            _simsParent = simsParent;

            _savePath = Path.Combine(Application.persistentDataPath, "savegame.json");
        }

        public bool HasSaveFile()
        {
            return File.Exists(_savePath);
        }

        public void SaveGame()
        {
            Debug.Log("Saving game...");
            var state = new GameStateData();

            state.CurrentBudget = _economyService.CurrentBudget;
            state.GameTimeTicks = _timeService.CurrentTime.Ticks;
            state.CurrentSims = _simsParent.childCount;


            var allFacilities = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IFacility>();
            foreach (var facility in allFacilities)
            {
                var facilityData = new FacilityData
                {
                    Type = facility.Type,
                    Position = facility.transform.position,
                    Rotation = facility.transform.rotation
                };
                state.AllFacilitiesData.Add(facilityData);
            }

            var allSims = Object.FindObjectsByType<SimController>(FindObjectsSortMode.None);
            foreach (var sim in allSims)
            {
                state.AllSimsData.Add(sim.GetSimData());
            }

            string json = JsonUtility.ToJson(state, true);
            File.WriteAllText(_savePath, json);
            
            Debug.Log($"Game saved successfully to {_savePath}");
        }

        public void LoadGame()
        {
            if (!HasSaveFile()) return;
            
            Debug.Log("Loading game...");

            string json = File.ReadAllText(_savePath);
            var state = JsonUtility.FromJson<GameStateData>(json);

            // Wipe current world state
            foreach (Transform child in _simsParent) { Object.Destroy(child.gameObject); }
            foreach (Transform child in _placedFacilitiesParent) { Object.Destroy(child.gameObject); }

            // Load Core Systems
            _economyService.SetBudget(state.CurrentBudget);
            _timeService.SetTime(new System.DateTime(state.GameTimeTicks));
            
            // Load Facilities
            foreach (var facilityData in state.AllFacilitiesData)
            {
                FacilityConfigSO config = _facilityDatabase.GetConfigForType(facilityData.Type);
                if (config != null)
                {
                    GameObject facilityInstance = Object.Instantiate(config.Prefab, facilityData.Position, facilityData.Rotation, _placedFacilitiesParent);
                    if (facilityInstance.TryGetComponent(out IFacility facilityComponent))
                    {
                        facilityComponent.Initialize(config);
                        _facilityTracker.Register(facilityComponent as Component, facilityData.Type);
                    }
                }
            }
            
            // Load Sims
            foreach (var simData in state.AllSimsData)
            {
                GameObject simInstance = Object.Instantiate(_simPrefab, simData.Position, simData.Rotation, _simsParent);
                if (simInstance.TryGetComponent(out SimController simController))
                {
                    simController.Initialize(_simConfig);
                    simController.LoadState(simData);
                }
            }
            
            Debug.Log("Game loaded successfully.");
        }
    }
}