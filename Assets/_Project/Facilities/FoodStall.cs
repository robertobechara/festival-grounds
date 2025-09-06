using FestivalGrounds.Core;
using FestivalGrounds.Data;
using UnityEngine;

namespace FestivalGrounds.Facilities
{
    [RequireComponent(typeof(UserSpotHandler), typeof(QueueHandler), typeof(FacilitySpotGenerator))]
    public class FoodStall : MonoBehaviour, IFacility
    {
        private UserSpotHandler _userSpotHandler;
        private QueueHandler _queueHandler;
        private FacilitySpotGenerator _spotGenerator;
        
           // --- IFacility Implementation ---
        public FacilityType Type => _config.Type;
        public int UseDurationMinutes => _config.UseDurationMinutes;
        public int RevenuePerUse => _config.revenuePerUse;
        private FacilityConfigSO _config;

        private void Awake()
        {
            _userSpotHandler = GetComponent<UserSpotHandler>();
            _queueHandler = GetComponent<QueueHandler>();
            _spotGenerator = GetComponent<FacilitySpotGenerator>();
        }

        public void Initialize(FacilityConfigSO config)
        {
            _config = config;
            // Step 1: Tell the generator to create the spots based on the config data.
            _spotGenerator.GenerateSpots(config);

            // Step 2: Give the generated spots to the handlers for them to manage.
            _userSpotHandler.Initialize(_spotGenerator.GeneratedUserSpots);
            _queueHandler.Initialize(_spotGenerator.GeneratedQueueSpots);

            // Step 3: Subscribe to events.
            _userSpotHandler.OnSpotFreed += OnUserSpotFreed;
        }

        public FacilityInteractionPoint RequestUse(ISimControllable sim)
        {
            if (_userSpotHandler.HasSpace)
            {
                Transform spot = _userSpotHandler.ClaimSpot(sim);
                if (spot != null)
                {
                    // We have a user spot. Tell the Sim to USE it.
                    return new FacilityInteractionPoint(spot, InteractionType.Use);
                }
            }
            
            if (_queueHandler.HasSpace)
            {
                Transform spot = _queueHandler.Enqueue(sim);
                if (spot != null)
                {
                    // We have a queue spot. Tell the Sim to QUEUE.
                    return new FacilityInteractionPoint(spot, InteractionType.Queue);
                }
            }

            // Facility is full, return a null spot.
            return new FacilityInteractionPoint(null, InteractionType.Queue); 
        }

        public void OnFinishedUsing(ISimControllable sim)
        {
            _userSpotHandler.ReleaseSpot(sim);
        }

        private void OnDestroy()
        {
            if (_userSpotHandler != null)
                _userSpotHandler.OnSpotFreed -= OnUserSpotFreed;
        }

        private void OnUserSpotFreed(ISimControllable simWhoLeft)
        {
            ISimControllable nextSim = _queueHandler.Dequeue();
            if (nextSim != null)
            {
                Transform spot = _userSpotHandler.ClaimSpot(nextSim);
                if (spot != null)
                {
                    nextSim.PromoteFromQueue(spot);
                }
            }
        }
    }
}