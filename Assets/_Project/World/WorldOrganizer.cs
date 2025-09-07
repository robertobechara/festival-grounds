using UnityEngine;

namespace FestivalGrounds.World
{
    // This component lives in the World scene and acts as a central, reliable
    // point of contact for objects within that scene.
    public class WorldOrganizer : MonoBehaviour
    {
        public static WorldOrganizer Instance { get; private set; }

        [Header("Scene References")]
        [Tooltip("The parent object for all spawned Sims.")]
        public Transform SimsParent;
        
        [Tooltip("The parent object for all placed facilities.")]
        public Transform PlacedFacilitiesParent;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}