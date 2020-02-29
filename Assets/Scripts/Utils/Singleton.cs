using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton class, for components that should only have one instance.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour where T : Singleton<T> {
    private static T instance;
    public static T Instance {
        get {
            return instance;
        }
    }

    /// <summary>
    /// Returns wherever the singleton's instance is initialized or not.
    /// </summary>
    public static bool IsInitialized {
        get {
            return instance != null;
        }
    }

    /// <summary>
    /// Sets the unique singlton instance.
    /// </summary>
    protected virtual void Awake() {
        if (instance != null) {
            Debug.LogErrorFormat("No multiple instances of a singleton allowed! {0}", GetType().Name);
        } else {
            instance = (T)this;
        }
    }

    protected virtual void OnDestroy() {
        if (instance == this) {
            instance = null;
        }
    }
}
