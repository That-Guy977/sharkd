using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

interface AudioProvider {
    AudioClip clip { get; }
    float volume { get; }
}

[Serializable]
struct AudioSingleProvider : AudioProvider {
    [field: SerializeField] public AudioClip clip { get; set; }
    [field: SerializeField] public float volume { get; set; }
}

[Serializable]
struct AudioBankProvider : AudioProvider {
    public List<AudioClip> clips;
    [field: SerializeField] public float volume { get; set; }

    public AudioClip clip => clips[Random.Range(0, clips.Count)];
}
