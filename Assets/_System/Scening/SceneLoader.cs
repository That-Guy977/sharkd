using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using AYellowpaper.SerializedCollections;
using Eflatun.SceneReference;

class SceneLoader : MonoBehaviour {
    [SerializedDictionary("Scene", "Transition")]
    public SceneDictionary<Transition> transitions;

    [Serializable]
    public struct Transition {
        public float delay;
        public Animation animOut, animIn;
    }

    public void LoadScene(SceneReference scene) {
        StartCoroutine(Load(scene));
    }

    private IEnumerator Load(SceneReference scene) {
        Transition transition = transitions[scene];
        Scene current = SceneManager.GetActiveScene();
        Animation animOut = Instantiate(transition.animOut);
        yield return new WaitWhile(() => animOut.isPlaying);
        yield return SceneManager.LoadSceneAsync(scene.BuildIndex, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(scene.LoadedScene);
        yield return new WaitForSeconds(transition.delay);
        Animation animIn = Instantiate(transition.animIn);
        SceneManager.UnloadSceneAsync(current);
        yield return new WaitWhile(() => animIn.isPlaying);
        Destroy(animIn.gameObject);
    }
}
