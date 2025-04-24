using System.Collections.Generic;
using UnityEngine;

class GameManager : MonoBehaviour {
    public static GameManager instance;

    [field: SerializeField] public GameObject player { get; private set; }
    [field: SerializeField] public new Camera camera { get; private set; }

    [Header("Overlay Menus")]
    public Canvas settings;
    public Canvas pause;

    [Header("Game State")]
    public GameState state = GameState.MainMenu;
    public bool tutorialComplete = false;

    Stack<Canvas> overlays = new();

    public enum GameState {
        MainMenu,
        InLevel
    }

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    protected void OnExit() {
        if (overlays.Count > 0) {
            Canvas overlay = overlays.Pop();
            overlay.gameObject.SetActive(false);
        } else if (state == GameState.InLevel) {
            Pause();
        }
    }

    public void OpenOverlay(Canvas overlay) {
        overlays.Push(overlay);
        overlay.gameObject.SetActive(true);
    }

    public void Pause() {
        OpenOverlay(pause);
    }

    public void Settings() {
        OpenOverlay(settings);
    }
}
