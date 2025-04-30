using UnityEngine;
using UnityEngine.UI;

class LevelSelectManager : MonoBehaviour {
    public Button inanis;

    void Start() {
        inanis.interactable = GameManager.instance.tutorialComplete;
    }
}
