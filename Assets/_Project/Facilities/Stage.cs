using FestivalGrounds.Core;
using FestivalGrounds.Data;
using UnityEngine;

namespace FestivalGrounds.Facilities
{
    public class Stage : MonoBehaviour, IFacility
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
    }
}