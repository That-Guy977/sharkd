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

    [Serializable]
    public struct Transition {
        public float delay;
        public Animator animOut, animIn;
    }

    void Awake() {
        if (instance == null) {
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

    public void LoadScene(SceneReference scene, Transition transition) {
        StartCoroutine(Load(scene, transition));
    }

    public void LoadLevel(SceneReference level) {
        StartCoroutine(Load(level, levelTransition));
        GameManager.instance.state = GameManager.GameState.InLevel;
    }

    private IEnumerator Load(SceneReference scene, Transition transition) {
        InstantiateParameters instParams = new() { scene = central.LoadedScene };
        Animator animOut = Instantiate(transition.animOut, instParams);
        yield return new WaitWhile(() => animOut.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);
        GameManager.instance.Clean();
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        yield return SceneManager.LoadSceneAsync(scene.BuildIndex, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(scene.LoadedScene);
        yield return new WaitForSecondsRealtime(transition.delay);
        Animator animIn = Instantiate(transition.animIn, instParams);
        Destroy(animOut.gameObject);
        yield return new WaitWhile(() => animIn.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);
        Destroy(animIn.gameObject);
    }

    private IEnumerator LoadInitial() {
        yield return SceneManager.LoadSceneAsync(initial.Name, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(initial.LoadedScene);
    }
}
