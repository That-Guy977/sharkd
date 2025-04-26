using UnityEngine;
using Eflatun.SceneReference;

class InLevelMenu : MonoBehaviour {
    public SceneReference mainMenu;
    public SceneLoader.Transition restartTransition;
    public SceneLoader.Transition exitTransition;

    void OnEnable() {
        Time.timeScale = 0;
    }

    void OnDisable() {
        Time.timeScale = 1;
    }

    public void ShowSettings() {
        GameManager.instance.Settings();
    }

    public void Exit() {
        SceneLoader.instance.LoadScene(mainMenu, exitTransition, GameManager.GameState.MainMenu);
    }

    public void Restart() {
        SceneLoader.instance.ReloadScene(restartTransition);
    }
}
