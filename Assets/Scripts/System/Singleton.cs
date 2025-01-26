using UnityEngine;

namespace System
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        public static T Instance { get; private set; }

        public virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;

                print("Instantiate Singleton : " + Instance);
            }
            else
            {
                print("Destroy Singleton: " + gameObject.name);

                Destroy(gameObject);
            }
        }
    }
}

