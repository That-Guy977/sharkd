using UnityEngine;

class Singleton<T> : MonoBehaviour where T : Singleton<T> {
    public static T instance { get; private set; }

    protected void Awake() {
        if (!instance) {
            instance = (T)this;
        } else {
            Destroy(this);
        }
    }
}
