using FestivalGrounds.Core;
using FestivalGrounds.Data;
using System.Collections.Generic;
using UnityEngine;

namespace FestivalGrounds.Facilities
{
    public class Tent : MonoBehaviour, IFacility
    {
        // --- IFacility Implementation ---
        public FacilityType Type => _config.Type;
        public int UseDurationMinutes => _config.UseDurationMinutes;
        public int RevenuePerUse => _config.revenuePerUse;
        private FacilityConfigSO _config;

        public void Initialize(FacilityConfigSO config)
        {
            _config = config;
        }

        public FacilityInteractionPoint RequestUse(ISimControllable sim) 
        { 
            return new FacilityInteractionPoint(null, InteractionType.Use);
        }
        public void OnFinishedUsing(ISimControllable sim) { }
        // --- End of IFacility Implementation ---

        // --- Tent-Specific Logic ---
        public bool HasCapacity => _config != null && _assignedSims.Count < _config.Capacity;
        private readonly List<GameObject> _assignedSims = new List<GameObject>();
        
        public void AssignSim(GameObject simObject)
        {
            if (HasCapacity && !_assignedSims.Contains(simObject))
            {
                _assignedSims.Add(simObject);
            }
        }
    }
}