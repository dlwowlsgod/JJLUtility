using UnityEngine;

namespace JJLUtility
{
    /// <summary>
    /// A generic base class to implement a Singleton pattern for Unity MonoBehaviours.
    /// Ensures that only one instance of the given MonoBehaviour type exists in the scene,
    /// destroying additional instances if necessary. The instance persists across scene loads.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the MonoBehaviour subclass.
    /// </typeparam>
    public class SingletonBehavior<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    var sameComponents = FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                    if (sameComponents.Length > 0)
                    {
                        if (sameComponents.Length > 1)
                        {
                            for (int i = 1; i < sameComponents.Length; i++)
                            {
                                Destroy(sameComponents[i].gameObject);
                            }
                        }
                        _instance = sameComponents[0];
                    }
                    else
                    {
                        var singletonObject = new GameObject(typeof(T).Name, typeof(T));
                        _instance = singletonObject.GetComponent<T>();
                    }
                }
                
                return _instance;
            }
        }
        
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }

            if (_instance != this)
            {
                Destroy(gameObject);
            }
            
            if (transform.root != null)
            {
                DontDestroyOnLoad(transform.root.gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}