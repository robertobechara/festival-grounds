using UnityEngine;

namespace FestivalGrounds.Data
{
    [CreateAssetMenu(fileName = "SimConfig", menuName = "FestivalGrounds/Sim Configuration")]
    public class SimConfigSO : ScriptableObject
    {
        [Header("Needs Settings")]
        [Tooltip("The maximum value for any need.")]
        public float MaxNeedValue = 100f;

        [Tooltip("When a need drops below this value, the Sim will seek a facility.")]
        [Range(1, 100)]
        public float NeedThreshold = 30f;
        
        [Header("Decay Rates (points per second)")]
        [Tooltip("How many points of hunger are lost per second.")]
        public float HungerDecayRate = 15f;

        [Tooltip("How many points of bladder are lost per second.")]
        public float BladderDecayRate = 20f;
        
        [Tooltip("How many points of energy are lost per second.")]
        public float EnergyDecayRate = 10f; 

        [Header("Replenish Rates")]
        [Tooltip("How many points of energy are gained per second while sleeping.")]
        public float EnergyReplenishRate = 25f; 
    }
}