using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Eflatun.SceneReference;

class InitialLoader : MonoBehaviour {
    public SceneReference mainMenu;

    void Start() {
        if (SceneManager.loadedSceneCount == 1) {
            StartCoroutine(Load());
        }
    }

    private IEnumerator Load() {
        yield return SceneManager.LoadSceneAsync(mainMenu.Name, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(mainMenu.LoadedScene);
    }
}
