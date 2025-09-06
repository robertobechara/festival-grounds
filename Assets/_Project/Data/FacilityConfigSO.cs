using UnityEngine;

namespace FestivalGrounds.Data
{
    [CreateAssetMenu(fileName = "FacilityConfig", menuName = "FestivalGrounds/Facility Configuration")]
    public class FacilityConfigSO : ScriptableObject
    {
        [Header("Display Info")]
        public string Name;
        public FacilityType Type;

        [Header("Gameplay Stats")]
        public int Cost;
        [Tooltip("How many Sims can use this facility at the same time.")]
        public int Capacity = 1;
        [Tooltip("The maximum number of Sims that can wait in the queue.")]
        public int QueueSize = 5;
        [Tooltip("How long, in game minutes, a Sim will occupy this facility.")]
        public int UseDurationMinutes = 5; 
        [Tooltip("The amount of money erned each time a Sim uses this facility.")]
        public int revenuePerUse = 0;

        [Header("Prefabs")]
        public GameObject Prefab;
        public GameObject GhostPrefab;
    }
}