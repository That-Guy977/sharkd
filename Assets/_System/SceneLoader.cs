using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Eflatun.SceneReference;

class SceneLoader : MonoBehaviour {
    public SceneReference scene;
    public float delay;
    public Animation transitionOut;
    public Animation transitionIn;

    public void LoadScene() {
        StartCoroutine(Load());
    }

    private IEnumerator Load() {
        Scene current = SceneManager.GetActiveScene();
        Animation animOut = Instantiate(transitionOut);
        yield return new WaitWhile(() => animOut.isPlaying);
        yield return SceneManager.LoadSceneAsync(scene.BuildIndex, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(scene.LoadedScene);
        yield return new WaitForSeconds(delay);
        Animation animIn = Instantiate(transitionIn);
        SceneManager.UnloadSceneAsync(current);
        yield return new WaitWhile(() => animIn.isPlaying);
        Destroy(animIn.gameObject);
    }
}
