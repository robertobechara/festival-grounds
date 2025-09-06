using FestivalGrounds.Core;
using FestivalGrounds.Data;
using UnityEngine;

namespace FestivalGrounds.Facilities
{
    public interface IFacility
    {
        Transform transform { get; }
        FacilityType Type { get; }
        int UseDurationMinutes { get; }
        int RevenuePerUse { get; }

        void Initialize(FacilityConfigSO config);
        // The return type is now our new, explicit struct
        FacilityInteractionPoint RequestUse(ISimControllable sim); 
        void OnFinishedUsing(ISimControllable sim);
    }
}