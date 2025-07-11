using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;

class ControlsManager : MonoBehaviour {
    public GameObject controlsRoot;
    public string controlScheme;
    [InputControl(layout = "Button")]
    public string cancelPath;
    public InputActionAsset UIInputActions;
    public string UIInputActionMap;

    readonly List<ControlSetting> controls = new();
    InputActionMap uiActionMap;

    private RebindingOperation rebindOperation;

    void Start() {
        controlsRoot.GetComponentsInChildren(includeInactive: true, controls);
        uiActionMap = UIInputActions.FindActionMap(UIInputActionMap);
    }

    void OnEnable() {
        InputSystem.onActionChange += OnActionChange;
    }

    void OnDisable() {
        InputSystem.onActionChange -= OnActionChange;
        Clean();
    }

    void Clean() {
        rebindOperation?.Dispose();
        rebindOperation = null;
        if (!uiActionMap.enabled) uiActionMap.Enable();
    }

    public void Rebind(InputAction action, int bindingIndex, Action clean) {
        uiActionMap.Disable();
        rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough(cancelPath)
            .OnComplete((_) => (clean + Clean)())
            .OnCancel((_) => (clean + Clean)())
            .Start();
    }

    void OnActionChange(object obj, InputActionChange change) {
        if (GameManager.instance.quitting || change != InputActionChange.BoundControlsChanged) return;
        controls.ForEach((control) => control.UpdatePreview());
    }
}
