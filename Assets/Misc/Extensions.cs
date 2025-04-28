using UnityEngine;
using UnityEngine.SceneManagement;
using Eflatun.SceneReference;

static class VectorUtils {
    public static Vector2 WithX(this Vector2 vec, float x) {
        vec.x = x;
        return vec;
    }
}

static class SceneUtils {
    public static SceneReference Ref(this Scene scene) => SceneReference.FromScenePath(scene.path);
}
