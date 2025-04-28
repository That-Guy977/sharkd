using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Eflatun.SceneReference;

class SceneLoader : MonoBehaviour {
    public static SceneLoader instance;

    public SceneReference central;
    public SceneReference initial;
    public Transition levelTransition;

    public SceneReference current => SceneReference.FromScenePath(SceneManager.GetActiveScene().path);

    [Serializable]
    public struct Transition {
        public float delay;
        public Animator animOut, animIn;
    }

    void Awake() {
        if (!instance) {
            instance = this;
        } else {
            Destroy(this);
        }
    }

    void Start() {
        Scene centralScene = SceneManager.GetSceneByBuildIndex(central.BuildIndex);
        if (SceneManager.loadedSceneCount == 1) {
            StartCoroutine(LoadInitial());
        } else if (SceneManager.GetActiveScene() == centralScene) {
            for (int i = 0; i < SceneManager.loadedSceneCount; i++) {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene != centralScene) {
                    SceneManager.SetActiveScene(scene);
                    break;
                }
            }
        }
    }

    public void LoadScene(SceneReference scene, Transition transition, GameState outState) {
        StartCoroutine(Load(scene, transition, outState));
    }

    public void ReloadScene(Transition transition) {
        StartCoroutine(Load(current, transition, GameManager.instance.state));
    }

    public void LoadLevel(SceneReference level) {
        StartCoroutine(Load(level, levelTransition, GameState.InLevel));
    }

    private IEnumerator Load(SceneReference scene, Transition transition, GameState outState) {
        if (GameManager.instance.state == GameState.Transitioning) yield break;
        GameManager.instance.state = GameState.Transitioning;
        InstantiateParameters instParams = new() { scene = central.LoadedScene };
        Animator animOut = Instantiate(transition.animOut, instParams);
        yield return new AnimatorPlaying(animOut);
        GameManager.instance.Clean();
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        yield return SceneManager.LoadSceneAsync(scene.BuildIndex, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(scene.LoadedScene);
        yield return new WaitForSecondsRealtime(transition.delay);
        Animator animIn = Instantiate(transition.animIn, instParams);
        Destroy(animOut.gameObject);
        yield return new AnimatorPlaying(animIn);
        Destroy(animIn.gameObject);
        GameManager.instance.state = outState;
    }

    private IEnumerator LoadInitial() {
        yield return SceneManager.LoadSceneAsync(initial.Name, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(initial.LoadedScene);
    }
}
