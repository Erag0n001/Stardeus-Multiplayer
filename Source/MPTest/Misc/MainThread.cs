using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Misc
{
    public class MainThread : MonoBehaviour
    {
        public static MainThread Instance;
        private static readonly ConcurrentQueue<Action> _executionQueue = new ConcurrentQueue<Action>();

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        public void Update()
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.TryDequeue(out Action act);
                act.Invoke();
            }
        }

        public void Enqueue(Action action) 
        {
            _executionQueue.Enqueue(action);
        }
    }
}
