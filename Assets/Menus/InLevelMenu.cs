using UnityEngine;
using Eflatun.SceneReference;

class InLevelMenu : MonoBehaviour {
    public SceneReference mainMenu;
    public SceneLoader.Transition restartTransition;
    public SceneLoader.Transition exitTransition;

    void OnEnable() {
        Time.timeScale = 0;
        GameManager.instance.player.SetFrozen(true);
        AudioListener.pause = true;
        MusicPlayer.instance.Pause();
    }

    void OnDisable() {
        if (GameManager.instance.quitting) return;
        Time.timeScale = 1;
        GameManager.instance.player.SetFrozen(false);
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
