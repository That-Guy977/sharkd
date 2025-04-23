using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Eflatun.SceneReference;

[Serializable]
class SceneDictionary<T> : SerializedDictionary<SceneReference, T> {
    class SceneEqualityComparator : IEqualityComparer<SceneReference> {
        public bool Equals(SceneReference s1, SceneReference s2) {
            return s1.Guid == s2.Guid;
        }

        public int GetHashCode(SceneReference scene) {
            return scene.Guid.GetHashCode();
        }
    }

    SceneDictionary() : base(new SceneEqualityComparator()) {}
}
