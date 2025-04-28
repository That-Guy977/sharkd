using UnityEngine;
using UnityEngine.InputSystem;
using Eflatun.SceneReference;

class InLevelMenu : MonoBehaviour {
    public PlayerInput playerInput;
    public SceneReference mainMenu;
    public SceneLoader.Transition restartTransition;
    public SceneLoader.Transition exitTransition;

    void OnEnable() {
        Time.timeScale = 0;
        playerInput.enabled = false;
        AudioListener.pause = true;
        MusicPlayer.instance.Pause();
    }

    void OnDisable() {
        Time.timeScale = 1;
        playerInput.enabled = true;
        AudioListener.pause = false;
        MusicPlayer.instance.Unpause();
    }

    public void ShowSettings() {
        GameManager.instance.Settings();
    }

    public void Exit() {
        SceneLoader.instance.LoadScene(mainMenu, exitTransition, GameState.MainMenu);
    }

    public void Restart() {
        SceneLoader.instance.ReloadScene(restartTransition);
    }
}
