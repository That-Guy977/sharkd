using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

class GawrLevelLogic : MonoBehaviour {
    public GameObject gawr;
    public List<EntitySpawnPoint> spawnPoints;
    public TimelineAsset tutorial;
    public TimelineAsset turnTutorial;

    PlayerController player;
    PlayableDirector director;

    void Awake() {
        director = GetComponent<PlayableDirector>();
    }

    void Start() {
        player = GameManager.instance.player;
        if (!GameManager.instance.tutorialComplete) {
            StartCoroutine(Tutorial());
        } else {
            SpawnGawr();
        }
    }

    public void Win() {
        StartCoroutine(WinSequence(GameManager.instance.tutorialComplete));
        GameManager.instance.tutorialComplete = true;
    }

    public void SpawnGawr() {
        EntitySpawnPoint spawnPoint = player.transform.position.Farthest(spawnPoints);
        spawnPoint.gameObject.SetActive(true);
    }

    public void WinExitLevel() {
        GameManager.instance.Win();
    }

    private IEnumerator WinSequence(bool tutorialDone) {
        GameManager.instance.levelEnd = true;
        yield return MergeSequence();
        if (!tutorialDone) {
            director.Play(turnTutorial);
        } else {
            WinExitLevel();
        }
    }

    private IEnumerator MergeSequence() {
        yield break;
    }

    private IEnumerator Tutorial() {
        if (player.entrance != null) {
            yield return player.entrance;
        }
        director.Play(tutorial);
    }
}
