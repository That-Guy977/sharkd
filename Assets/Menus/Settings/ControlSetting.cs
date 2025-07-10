using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;

class ControlSetting : MonoBehaviour {
    public PlayerInput playerInput;
    public InputActionReference actionReference;
    public string compositePart;
    public string controlScheme;
    public string cancelControl;
    public Button rebind;
    public Button reset;
    public TMP_Text bindingPreview;
    public TMP_Text cancelPrompt;
    public InputActionAsset UIInputActions;
    public string UIInputActionMap;

    int bindingIndex;
    InputActionMap uiActionMap;

    private RebindingOperation rebindOperation;

    InputAction action => playerInput.actions.FindAction(actionReference.action.id);

    void Start() {
        bindingIndex = action.bindings.IndexOf(
            (binding) =>
                Array.Exists(binding.groups.Split(InputBinding.Separator), (group) => group == controlScheme)
                && (string.IsNullOrEmpty(compositePart) || binding.isPartOfComposite && binding.name == compositePart)
        );
        uiActionMap = UIInputActions.FindActionMap(UIInputActionMap);
        UpdatePreview();
    }

    void OnEnable() {
        InputSystem.onActionChange += OnActionChange;
    }

    void OnDisable() {
        InputSystem.onActionChange -= OnActionChange;
        if (GameManager.instance.quitting) return;
        Clean();
    }

    void Clean() {
        rebindOperation?.Dispose();
        rebindOperation = null;
        if (!action.actionMap.asset.enabled) action.actionMap.asset.Enable();
        if (!uiActionMap.enabled) uiActionMap.Enable();
        rebind.interactable = true;
        cancelPrompt.enabled = false;
        UpdatePreview();
    }

    void UpdatePreview() {
        bindingPreview.text = action.GetBindingDisplayString(bindingIndex, InputBinding.DisplayStringOptions.DontIncludeInteractions);
    }

    public void Rebind() {
        action.actionMap.asset.Disable();
        uiActionMap.Disable();
        rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough(cancelControl)
            .OnComplete((_) => {
                UpdatePreview();
                Clean();
                reset.interactable = true;
            })
            .OnCancel((_) => Clean());
        rebind.interactable = false;
        bindingPreview.text = "Waiting...";
        cancelPrompt.enabled = true;
        rebindOperation.Start();
    }

    public void ResetBind() {
        action.RemoveBindingOverride(bindingIndex);
        reset.interactable = false;
        UpdatePreview();
    }

    void OnActionChange(object obj, InputActionChange change) {
        if (GameManager.instance.quitting || change != InputActionChange.BoundControlsChanged) return;
        UpdatePreview();
    }
}
