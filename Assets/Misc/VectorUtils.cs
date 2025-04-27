using UnityEngine;

static class VectorUtils {
    public static Vector2 WithX(this Vector2 vec, float x) {
        vec.x = x;
        return vec;
    }
}
