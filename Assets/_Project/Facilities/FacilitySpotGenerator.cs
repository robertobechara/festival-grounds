using FestivalGrounds.Data;
using System.Collections.Generic;
using UnityEngine;

namespace FestivalGrounds.Facilities
{
    public class FacilitySpotGenerator : MonoBehaviour
    {
        [Header("Scene Reference")]
        [Tooltip("A single point defining the front-center of the interaction area.")]
        [SerializeField] private Transform _interactionPoint;

        [Header("Layout Tuning")]
        [Tooltip("The direction the queue will form, relative to the Interaction Point's orientation.")]
        [SerializeField] private Vector3 _queueDirection = Vector3.back;
        [Tooltip("The distance between each spot in the queue.")]
        [SerializeField] private float _queueSpacing = 1.5f;
        [Tooltip("The distance between each user spot if capacity is greater than 1.")]
        [SerializeField] private float _userSpotSpacing = 1.5f;

        public Transform[] GeneratedUserSpots { get; private set; }
        public Transform[] GeneratedQueueSpots { get; private set; }

        public void GenerateSpots(FacilityConfigSO config)
        {
            if (_interactionPoint == null)
            {
                Debug.LogError($"Facility '{gameObject.name}' is missing its Interaction Point reference!", this);
                return;
            }

            // --- Generate User Spots ---
            List<Transform> userSpots = new List<Transform>();
            Vector3 userStartOffset = -_interactionPoint.right * (_userSpotSpacing * (config.Capacity - 1)) / 2f;

            for (int i = 0; i < config.Capacity; i++)
            {
                GameObject spotGO = new GameObject($"UserSpot_{i + 1}");
                spotGO.transform.SetParent(_interactionPoint);
                spotGO.transform.localPosition = userStartOffset + (_interactionPoint.right * (i * _userSpotSpacing));
                spotGO.transform.localRotation = Quaternion.identity;
                userSpots.Add(spotGO.transform);
            }
            GeneratedUserSpots = userSpots.ToArray();

            // --- Generate Queue Spots ---
            List<Transform> queueSpots = new List<Transform>();
            for (int i = 0; i < config.QueueSize; i++)
            {
                GameObject spotGO = new GameObject($"QueueSpot_{i + 1}");
                spotGO.transform.SetParent(_interactionPoint);
                // The queue forms based on the direction and spacing
                Vector3 queuePosition = _queueDirection.normalized * ((i + 1) * _queueSpacing);
                spotGO.transform.localPosition = queuePosition;
                spotGO.transform.localRotation = Quaternion.identity;
                queueSpots.Add(spotGO.transform);
            }
            GeneratedQueueSpots = queueSpots.ToArray();
        }
    }
}