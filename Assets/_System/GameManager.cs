using System.Collections.Generic;
using UnityEngine;

class GameManager : Singleton<GameManager> {
    [field: SerializeField] public PlayerController player { get; private set; }
    [field: SerializeField] public SceneLoader loader { get; private set; }
    [field: SerializeField] public new CameraController camera { get; private set; }

    [Header("Overlay Menus")]
    public Canvas pause;
    public Canvas win;
    public Canvas defeat;
    public Canvas settings;

    [Header("Game State")]
    public GameState state = GameState.MainMenu;
    public bool levelEnd = false;
    public bool tutorialShown = false;
    public bool tutorialComplete = false;

    readonly Stack<Canvas> overlays = new();

    private bool toPause = false;
    private bool toExit = false;

    public bool overlayOpen => overlays.Count > 0;
    public bool quitting { get; private set; } = false;

    void Update() {
        if (overlayOpen) {
            if (toExit) {
                CloseOverlay();
            } else if (toPause && state == GameState.InLevel) {
                Unpause();
            }
        } else if ((toPause || toExit) && state == GameState.InLevel) {
            Pause();
        }
        toPause = toExit = false;
    }

    public void Clean() {
        player.gameObject.SetActive(false);
        camera.Clean();
        CloseAllOverlays();
    }

    protected void OnPause() {
        if (levelEnd) return;
        toPause = true;
    }

    protected void OnExit() {
        if (state == GameState.Transitioning || levelEnd) return;
        toExit = true;
    }

    public void OpenOverlay(Canvas overlay) {
        overlays.Push(overlay);
        overlay.sortingOrder = overlays.Count;
        overlay.gameObject.SetActive(true);
    }

    void CloseOverlay() {
        overlays.Pop().gameObject.SetActive(false);
    }

    void CloseAllOverlays() {
        while (overlayOpen) {
            CloseOverlay();
        }
    }

    public void Pause() {
        OpenOverlay(pause);
    }

    public void Unpause() {
        CloseAllOverlays();
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

    void OnApplicationQuit() {
        quitting = true;
    }
}

enum GameState {
    MainMenu,
    Transitioning,
    InLevel,
}
