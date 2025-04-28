using UnityEngine;
using UnityEngine.SceneManagement;
using Eflatun.SceneReference;

static class VectorUtils {
    public static Vector2 WithX(this Vector2 vec, float x) {
        vec.x = x;
        return vec;
    }

    public static Vector2 SlightUp(this Vector2 vec, float y) {
        return Vector2.ClampMagnitude(vec + Vector2.up * y, vec.magnitude);
    }
}

static class SceneUtils {
    public static SceneReference Ref(this Scene scene) => SceneReference.FromScenePath(scene.path);
}
