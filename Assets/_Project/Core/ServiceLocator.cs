using System;
using System.Collections.Generic;
using UnityEngine;

namespace FestivalGrounds.Core
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        /// <summary>
        /// Registers a service instance, making it available to the rest of the application.
        /// </summary>
        public static void Register<T>(T service)
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                Debug.LogWarning($"Service of type {type.Name} is already registered. Overwriting.");
                _services[type] = service;
            }
            else
            {
                _services.Add(type, service);
            }
        }

        /// <summary>
        /// Unregisters a service, removing it from the locator.
        /// </summary>
        public static void Unregister<T>()
        {
            var type = typeof(T);
            if (!_services.ContainsKey(type))
            {
                Debug.LogWarning($"Trying to unregister a service of type {type.Name} which is not registered.");
                return;
            }

            _services.Remove(type);
        }

        /// <summary>
        /// Retrieves a registered service.
        /// </summary>
        public static T GetService<T>()
        {
            var type = typeof(T);
            if (!_services.TryGetValue(type, out var service))
            {
                throw new Exception($"Service of type {type.Name} not found.");
            }
            return (T)service;
        }

        /// <summary>
        /// Clears all registered services. Useful for scene transitions or resets.
        /// </summary>
        public static void Clear()
        {
            _services.Clear();
            Debug.Log("All services cleared from ServiceLocator.");
        }
    }
}

