using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FestivalGrounds.Data
{
    [CreateAssetMenu(fileName = "FacilityDatabase", menuName = "FestivalGrounds/Facility Database")]
    public class FacilityDatabaseSO : ScriptableObject
    {
        // This list will hold all of our facility configuration assets.
        public List<FacilityConfigSO> AllFacilityConfigs;

        // A helper method to find the correct config based on its type.
        public FacilityConfigSO GetConfigForType(FacilityType type)
        {
            return AllFacilityConfigs.FirstOrDefault(config => config.Type == type);
        }
    }
}