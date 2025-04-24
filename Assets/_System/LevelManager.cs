using UnityEngine;
using UnityEngine.UI;

class LevelManager : MonoBehaviour {
    public Button inanis;

    void OnEnable() {
        inanis.interactable = GameManager.instance.tutorialComplete;
    }
}
