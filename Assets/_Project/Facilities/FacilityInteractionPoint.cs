using UnityEngine;

namespace FestivalGrounds.Facilities
{
    public struct FacilityInteractionPoint
    {
        public readonly Transform Spot;
        public readonly InteractionType Type;

        public FacilityInteractionPoint(Transform spot, InteractionType type)
        {
            Spot = spot;
            Type = type;
        }
    }
}