using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
class ExternalLink : MonoBehaviour {
    public Site site;
    public string id;

    Image image;

    void Start() {
        image = GetComponent<Image>();
    }

    void Update() {
        if (site) {
            image.sprite = site.icon;
        }
    }

    public void OpenLink() {
        Application.OpenURL($"https://{string.Format(site.baseLink, id)}");
    }
}
