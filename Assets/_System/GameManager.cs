using System.Collections.Generic;
using UnityEngine;

class GameManager : MonoBehaviour {
    public static GameManager instance;

    [field: SerializeField] public PlayerController player { get; private set; }
    [field: SerializeField] public SceneLoader loader { get; private set; }
    [field: SerializeField] public new Camera camera { get; private set; }

    [Header("Overlay Menus")]
    public Canvas pause;
    public Canvas win;
    public Canvas defeat;
    public Canvas settings;

    [Header("Game State")]
    public GameState state = GameState.MainMenu;
    public bool levelEnd = false;
    public bool tutorialComplete = false;

    Stack<Canvas> overlays = new();

    public bool overlayOpen => overlays.Count > 0;

    void Awake() {
        if (!instance) {
            instance = this;
        } else {
            Destroy(this);
        }
    }

    void OnEnable() {
        Clean();
    }

    public void Clean() {
        player.gameObject.SetActive(false);
        float cameraHalfHeight = camera.orthographicSize;
        float cameraHalfWidth = cameraHalfHeight * camera.aspect;
        camera.transform.position = new Vector3(cameraHalfWidth, cameraHalfHeight, -10);
        while (overlayOpen) {
            CloseOverlay();
        }
    }

    protected void OnExit() {
        if (state == GameState.Transitioning || levelEnd) return;
        if (overlays.Count > 0) {
            CloseOverlay();
        } else if (state == GameState.InLevel || state == GameState.Tutorial) {
            Pause();
        }
    }

    public void OpenOverlay(Canvas overlay) {
        overlays.Push(overlay);
        overlay.gameObject.SetActive(true);
    }

    public void CloseOverlay() {
        overlays.Pop().gameObject.SetActive(false);
    }

    public void Pause() {
        OpenOverlay(pause);
    }

    public void Unpause() {
        while (pause.gameObject.activeSelf && overlays.Count > 0) {
            CloseOverlay();
        }
    }

    public void Win() {
        OpenOverlay(win);
    }

    public void Defeat() {
        OpenOverlay(defeat);
    }

    public void Settings() {
        OpenOverlay(settings);
    }
}

enum GameState {
    MainMenu,
    Transitioning,
    InLevel,
    Tutorial,
}
