using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

class ControlSetting : MonoBehaviour {
    public ControlsManager manager;
    public PlayerInput playerInput;
    public InputActionReference actionReference;
    public string compositePart;
    public Color color;
    public Color conflictColor;
    public Button rebind;
    public Button reset;
    public Image rebindImage;
    public TMP_Text bindingPreview;
    public TMP_Text waitingPrompt;
    public TMP_Text cancelPrompt;

    int bindingIndex;

    InputAction action => actionReference.action;
    InputAction playerAction => playerInput.actions.FindAction(action.id);

    public string path => action.bindings[bindingIndex].effectivePath;

    void Start() {
        InputBinding bindingMask = InputBinding.MaskByGroup(manager.controlScheme);
        if (!string.IsNullOrEmpty(compositePart)) bindingMask.name = compositePart;
        bindingIndex = action.GetBindingIndex(bindingMask);
        UpdatePreview();
    }

    void OnDisable() {
        if (GameManager.instance.quitting) return;
        Clean();
    }

    void Clean() {
        if (!action.actionMap.asset.enabled) action.actionMap.asset.Enable();
        rebind.interactable = true;
        bindingPreview.enabled = true;
        waitingPrompt.enabled = false;
        cancelPrompt.enabled = false;
        UpdatePreview();
    }

    void SyncOverride() {
        string overridePath = action.bindings[bindingIndex].overridePath;
        if (!string.IsNullOrEmpty(overridePath)) {
            playerAction.ApplyBindingOverride(bindingIndex, overridePath);
        } else {
            playerAction.RemoveBindingOverride(bindingIndex);
        }
    }

    public void UpdatePreview() {
        bindingPreview.text = InputBindings.DisplayString(action, bindingIndex);
        reset.interactable = !string.IsNullOrEmpty(action.bindings[bindingIndex].overridePath);
    }

    public void UpdateConflict(bool conflict) {
        rebindImage.color = conflict ? conflictColor : color;
    }

    public void Rebind() {
        action.actionMap.asset.Disable();
        rebind.interactable = false;
        bindingPreview.enabled = false;
        waitingPrompt.enabled = true;
        cancelPrompt.enabled = true;
        manager.Rebind(action, bindingIndex, () => {
            Clean();
            SyncOverride();
        });
    }

    public void ResetBind() {
        action.RemoveBindingOverride(bindingIndex);
        UpdatePreview();
        SyncOverride();
        manager.CheckConflict();
    }
}
