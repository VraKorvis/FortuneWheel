using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class UpdateManager : MonoBehaviour
    {
        private readonly List<Action> _updates = new();

        public void Register(Action updateAction)
        {
            if (!_updates.Contains(updateAction))
            {
                _updates.Add(updateAction);
            }
        }

        public void Unregister(Action updateAction)
        {
            _updates.Remove(updateAction);
        }

        private void Update()
        {
            foreach (var t in _updates)
            {
                t?.Invoke();
            }
        }
    }
}