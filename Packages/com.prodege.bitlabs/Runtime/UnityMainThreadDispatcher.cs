using System;
using System.Collections.Generic;
using UnityEngine;

namespace BitLabsCallbacks
{
    /// <summary>
    /// Helper class to dispatch actions to Unity's main thread.
    /// Android callbacks may arrive on background threads, but Unity API calls
    /// must happen on the main thread.
    /// </summary>
    public class UnityMainThreadDispatcher : MonoBehaviour
    {
        private static UnityMainThreadDispatcher _instance;
        private static readonly Queue<Action> _executionQueue = new Queue<Action>();

        /// <summary>
        /// Gets or creates the singleton instance of UnityMainThreadDispatcher.
        /// </summary>
        public static UnityMainThreadDispatcher Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("UnityMainThreadDispatcher");
                    _instance = go.AddComponent<UnityMainThreadDispatcher>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        /// <summary>
        /// Enqueues an action to be executed on the main thread.
        /// </summary>
        /// <param name="action">The action to execute on the main thread.</param>
        public void Enqueue(Action action)
        {
            if (action == null) return;

            lock (_executionQueue)
            {
                _executionQueue.Enqueue(action);
            }
        }

        /// <summary>
        /// Executes all queued actions on the main thread.
        /// </summary>
        private void Update()
        {
            lock (_executionQueue)
            {
                while (_executionQueue.Count > 0)
                {
                    try
                    {
                        _executionQueue.Dequeue()?.Invoke();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[BitLabs] Error executing queued action: {e}");
                    }
                }
            }
        }
    }
}
