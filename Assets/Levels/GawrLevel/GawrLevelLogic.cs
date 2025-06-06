using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

class GawrLevelLogic : MonoBehaviour {
    public GawrController gawr;
    public float winSlowdownDuration;
    public float winInitialSlowdown;
    public float winDelay;
    public float mergeOffset;
    public float mergeDuration;
    public float mergeDelay;
    public float mergeHighlightInDuration;
    public float mergeHighlightOutDuration;
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
        if (!GameManager.instance.tutorialShown) {
            StartCoroutine(Tutorial());
        } else {
            SpawnGawr();
        }
    }

    public void TutorialDone() {
        GameManager.instance.tutorialShown = true;
        SpawnGawr();
    }

    public void TurnTutorialDone() {
        GameManager.instance.tutorialComplete = true;
        GameManager.instance.Win();
    }

    public void Win() {
        StartCoroutine(WinSequence());
    }

    void SpawnGawr() {
        EntitySpawnPoint spawnPoint = player.transform.position.Farthest(spawnPoints);
        spawnPoint.gameObject.SetActive(true);
    }

    private IEnumerator Tutorial() {
        yield return player.entrance;
        director.Play(tutorial);
    }

    private IEnumerator WinSequence() {
        GameManager.instance.levelEnd = true;
        Time.timeScale = winInitialSlowdown;
        float elapsedTime = 0;
        do {
            elapsedTime += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(winInitialSlowdown, 0, elapsedTime / winSlowdownDuration);
            yield return null;
        } while (Time.timeScale > 0);
        yield return new WaitForSecondsRealtime(winDelay);
        if (!GameManager.instance.tutorialComplete) {
            yield return MergeSequence();
        } else {
            GameManager.instance.Win();
        }
    }

    private IEnumerator MergeSequence() {
        Time.timeScale = 1;
        Entity playerEntity = player.GetComponent<Entity>();
        Entity gawrEntity = gawr.GetComponent<Entity>();
        player.SetFrozen(true);
        player.Clean();
        playerEntity.hud.enabled = false;
        gawr.SetFrozen(true);
        gawrEntity.hud.enabled = false;
        Vector2 playerPos = player.transform.position;
        Vector2 gawrPos = gawr.transform.position;
        Vector2 target = new Vector2(
            (playerPos.x + gawrPos.x) / 2,
            Mathf.Max(playerPos.y, gawrPos.y) + mergeOffset
        );
        float mergeAcceleration = Mathf.Abs(target.x - playerPos.x) * 2 / Mathf.Pow(mergeDuration, 2);
        float playerDir = Mathf.Sign(target.x - playerPos.x);
        float gawrDir = Mathf.Sign(target.x - gawrPos.x);
        Vector2 playerVel = new Vector2(0, (target.y - playerPos.y) / mergeDuration);
        Vector2 gawrVel = new Vector2(0, (target.y - gawrPos.y) / mergeDuration);
        playerEntity.Highlight(mergeHighlightInDuration);
        gawrEntity.Highlight(mergeHighlightInDuration);
        float elapsedTime = 0;
        do {
            elapsedTime += Time.deltaTime;
            playerVel.x += mergeAcceleration * playerDir * Time.deltaTime;
            playerPos += playerVel * Time.deltaTime;
            playerPos.x = playerDir > 0 ? Mathf.Min(playerPos.x, target.x) : Mathf.Max(playerPos.x, target.x);
            playerPos.y = Mathf.Min(playerPos.y, target.y);
            player.transform.position = playerPos;
            gawrVel.x += mergeAcceleration * gawrDir * Time.deltaTime;
            gawrPos += gawrVel * Time.deltaTime;
            gawrPos.x = gawrDir > 0 ? Mathf.Min(gawrPos.x, target.x) : Mathf.Max(gawrPos.x, target.x);
            gawrPos.y = Mathf.Min(gawrPos.y, target.y);
            gawr.transform.position = gawrPos;
            yield return null;
        } while (elapsedTime < mergeDuration);
        yield return new WaitForSeconds(mergeDelay);
        gawr.gameObject.SetActive(false);
        yield return playerEntity.Dehighlight(mergeHighlightOutDuration);
        player.SetFrozen(false);
        playerEntity.ResetHighlight();
        director.Play(turnTutorial);
    }
}
