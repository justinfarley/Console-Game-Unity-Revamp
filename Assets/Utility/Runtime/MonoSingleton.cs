using System;
using UnityEngine;

namespace RedLobsterStudios.Util {
    /// <summary>
    /// Useful for Managers that persist between scenes
    /// </summary>
    /// <typeparam name="T">The type of manager. e.g. GameManager, AudioManager, etc.</typeparam>
    public class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        protected Action OnInitialized = null;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        protected virtual void Awake()
        {
            if( _instance != null )
                Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
            OnInitialized?.Invoke();
        }
    }
}
