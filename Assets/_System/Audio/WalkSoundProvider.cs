using System;
using UnityEngine;
using AYellowpaper.SerializedCollections;

class WalkSoundProvider : MonoBehaviour {
    public static WalkSoundProvider instance;

    public float stepRate;
    [SerializedDictionary("Terrain", "Bank")]
    public SerializedDictionary<TerrainType, WalkSoundBankWrapper> terrainSoundBanks;

    [Serializable]
    public struct WalkSoundBankWrapper {
        [SerializedDictionary("Type", "SoundFX")]
        public SerializedDictionary<WalkSoundType, AudioBankProvider> soundBank;
    }

    void Awake() {
        if (!instance) {
            instance = this;
        } else {
            Destroy(this);
        }
    }

    public void Emit(TerrainType terrain, WalkSoundType type, float volume = 1) {
        if (terrainSoundBanks.TryGetValue(terrain, out var soundBankWrapper)) {
            if (soundBankWrapper.soundBank.TryGetValue(type, out var playables)) {
                SoundFXPlayer.instance.Play(playables, volume);
            }
        }
    }
}

enum TerrainType {
    Stone,
    Water,
}

enum WalkSoundType {
    Step,
    Jump,
    Land,
}
