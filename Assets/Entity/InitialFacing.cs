using UnityEngine;

[ExecuteAlways]
class InitialFacing : MonoBehaviour {
    public Direction facing;

    Entity entity;

    void Awake() {
        entity = GetComponent<Entity>();
    }

    void Start() {
        entity.facing = facing;
    }

#if UNITY_EDITOR
    void Update() {
        entity.facing = facing;
    }
#endif
}
