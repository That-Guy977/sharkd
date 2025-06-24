using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
class ExternalLink : MonoBehaviour {
    public Site site;
    public string id;
    public string page;

    Image image;

    void Awake() {
        image = GetComponent<Image>();
    }

    void Update() {
        if (site) {
            image.sprite = site.icon;
        }
    }

    public void OpenLink() {
        Application.OpenURL($"https://{string.Format(site.baseLink, id, page)}");
    }
}
