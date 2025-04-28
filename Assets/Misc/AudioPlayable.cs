using System;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

interface AudioPlayable {
    AudioClip clip { get; }
    float volume { get; }
}

[Serializable]
struct AudioSinglePlayable : AudioPlayable {
    [field: SerializeField] public AudioClip clip { get; set; }
    [field: SerializeField] public float volume { get; set; }
}

[Serializable]
struct AudioBankPlayable : AudioPlayable {
    public List<AudioClip> clips;
    [field: SerializeField] public float volume { get; set; }

    public AudioClip clip => clips[Random.Range(0, clips.Count)];
}
