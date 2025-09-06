using System;
using System.Collections.Generic;

namespace FestivalGrounds.Core
{
    public class EventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _subscriptions = new Dictionary<Type, List<Delegate>>();

        public void Subscribe<T>(Action<T> listener)
        {
            Type eventType = typeof(T);
            if (!_subscriptions.ContainsKey(eventType))
            {
                _subscriptions[eventType] = new List<Delegate>();
            }
            _subscriptions[eventType].Add(listener);
        }

        public void Unsubscribe<T>(Action<T> listener)
        {
            Type eventType = typeof(T);
            if (_subscriptions.ContainsKey(eventType))
            {
                _subscriptions[eventType].Remove(listener);
            }
        }

        public void Publish<T>(T eventToPublish)
        {
            Type eventType = typeof(T);
            if (_subscriptions.ContainsKey(eventType))
            {
                // Create a copy of the list to prevent issues if a subscriber unsubscribes during the loop
                var listeners = new List<Delegate>(_subscriptions[eventType]);
                foreach (var listener in listeners)
                {
                    ((Action<T>)listener)(eventToPublish);
                }
            }
        }
    }
}

