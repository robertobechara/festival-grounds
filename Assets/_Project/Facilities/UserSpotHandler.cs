using FestivalGrounds.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FestivalGrounds.Facilities
{
    public class UserSpotHandler : MonoBehaviour
    {
        public event Action<ISimControllable> OnSpotFreed;
        private readonly Dictionary<ISimControllable, Transform> _activeUsers = new Dictionary<ISimControllable, Transform>();
        private Transform[] _userSpots;

        public bool HasSpace => _activeUsers.Count < _userSpots.Length;

        public void Initialize(Transform[] spots)
        {
            _userSpots = spots;
        }

        public Transform ClaimSpot(ISimControllable sim)
        {
            if (!HasSpace) return null;
            foreach (Transform spot in _userSpots)
            {
                if (!_activeUsers.ContainsValue(spot))
                {
                    _activeUsers.Add(sim, spot);
                    return spot;
                }
            }
            return null;
        }

        public void ReleaseSpot(ISimControllable sim)
        {
            if (_activeUsers.ContainsKey(sim))
            {
                _activeUsers.Remove(sim);
                OnSpotFreed?.Invoke(sim);
            }
        }
    }
}