using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;

class ControlSetting : MonoBehaviour {
    public ControlsManager manager;
    public PlayerInput playerInput;
    public InputActionReference actionReference;
    public string compositePart;
    public Button rebind;
    public Button reset;
    public TMP_Text bindingPreview;
    public TMP_Text cancelPrompt;

    int bindingIndex;

    InputAction action => playerInput.actions.FindAction(actionReference.action.id);

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
        reset.interactable = !string.IsNullOrEmpty(action.bindings[bindingIndex].overridePath);
        cancelPrompt.enabled = false;
        UpdatePreview();
    }

    public void UpdatePreview() {
        bindingPreview.text = action.GetBindingDisplayString(bindingIndex, InputBinding.DisplayStringOptions.DontIncludeInteractions);
    }

    public void Rebind() {
        action.actionMap.asset.Disable();
        rebind.interactable = false;
        bindingPreview.text = "Waiting...";
        cancelPrompt.enabled = true;
        manager.Rebind(action, bindingIndex, Clean);
    }

    public void ResetBind() {
        action.RemoveBindingOverride(bindingIndex);
        reset.interactable = false;
        UpdatePreview();
    }
}
