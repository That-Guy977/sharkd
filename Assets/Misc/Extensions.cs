using UnityEngine;
using UnityEngine.SceneManagement;
using Eflatun.SceneReference;
using System.Collections.Generic;

static class VectorUtils {
    public static Vector2 WithX(this Vector2 vec, float x) {
        vec.x = x;
        return vec;
    }

    public static Vector2 SlightUp(this Vector2 vec, float y) => Vector2.ClampMagnitude(vec + Vector2.up * y, vec.magnitude);
    
    public static T FarthestTarget<T>(this Vector2 pos, IEnumerable<T> targets) where T : Component {
        T farthest = null;
        float maxDist = 0;
        foreach (var target in targets) {
            float dist = Vector2.Distance(pos, target.transform.position);
            if (dist > maxDist) {
                farthest = target;
                maxDist = dist;
            }
        }
        return farthest;
    }

    public static T Farthest<T>(this Vector3 pos, List<T> targets) where T : Component {
        return ((Vector2)pos).FarthestTarget(targets);
    }
}

static class SceneUtils {
    public static SceneReference Ref(this Scene scene) => SceneReference.FromScenePath(scene.path);
}
