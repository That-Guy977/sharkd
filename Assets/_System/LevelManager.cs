using UnityEngine;
using UnityEngine.UI;

class LevelManager : MonoBehaviour {
    public Button inanis;

    void Start() {
        inanis.interactable = GameManager.instance.tutorialComplete;
    }
}
