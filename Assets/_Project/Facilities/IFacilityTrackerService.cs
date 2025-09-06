using FestivalGrounds.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FestivalGrounds.Facilities
{
    // The public interface for our new service
    public interface IFacilityTrackerService
    {
        void Register(Component facility, FacilityType type);
        void Unregister(Component facility, FacilityType type);
        Transform FindNearestFacility(FacilityType type, Vector3 position);
        Tent FindAvailableTent(Vector3 position);
    }

    public class FacilityTrackerService : IFacilityTrackerService
    {
        // A dictionary to hold lists of facilities, keyed by their type
        private readonly Dictionary<FacilityType, List<Component>> _facilities = new Dictionary<FacilityType, List<Component>>();

        public void Register(Component facility, FacilityType type)
        {
            // If we've never seen this type before, create a new list for it
            if (!_facilities.ContainsKey(type))
            {
                _facilities[type] = new List<Component>();
            }

            if (!_facilities[type].Contains(facility))
            {
                _facilities[type].Add(facility);
                Debug.Log($"[FacilityTracker] Registered a {type}. Total now: {_facilities[type].Count}");
            }
        }

        public void Unregister(Component facility, FacilityType type)
        {
            if (_facilities.ContainsKey(type) && _facilities[type].Contains(facility))
            {
                _facilities[type].Remove(facility);
            }
        }

        public Transform FindNearestFacility(FacilityType type, Vector3 position)
        {
            if (!_facilities.ContainsKey(type) || _facilities[type].Count == 0)
            {
                return null; // No facilities of this type exist
            }

            // Find the facility of the given type that is closest to the Sim's position
            return _facilities[type]
                .OrderBy(f => Vector3.Distance(position, f.transform.position))
                .FirstOrDefault()?.transform;
        }

        public Tent FindAvailableTent(Vector3 position)
        {
            FacilityType tentType = FacilityType.Tent; // We know we're looking for Tents

            if (!_facilities.ContainsKey(tentType) || _facilities[tentType].Count == 0)
            {
                return null;
            }

            // This is a more complex query:
            // 1. Get all components of type Tent.
            // 2. Filter that list to only include tents that have capacity.
            // 3. Order the remaining tents by distance.
            // 4. Return the first one, or null if none are found.
            return _facilities[tentType]
                .OfType<Tent>()
                .Where(t => t.HasCapacity)
                .OrderBy(t => Vector3.Distance(position, t.transform.position))
                .FirstOrDefault();
        }
    }
}