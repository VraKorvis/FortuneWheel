using System;
using System.Collections.Generic;

namespace Core
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> Services = new();

        public static void Register<T>(T service)
        {
            var type = typeof(T);
            if (Services.ContainsKey(type))
            {
                Services[type] = service;
            }
            else
            {
                Services.Add(type, service);
            }
        }

        public static T Get<T>()
        {
            var type = typeof(T);
            if (Services.TryGetValue(type, out var service))
            {
                return (T) service;
            }

            throw new Exception($"Service of type {type} is not registered in ServiceLocator!");
        }

        public static void Unregister<T>()
        {
            var type = typeof(T);
            if (IsRegistered<T>())
            {
                Services.Remove(type);
            }
        }
        
        public static bool IsRegistered<T>() => Services.ContainsKey(typeof(T));

        public static void Clear()
        {
            Services.Clear();
        }
    }
}