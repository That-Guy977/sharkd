using System;
using UnityEngine.InputSystem;

class InputBindings : Singleton<InputBindings> {
    public event Action bindingsChanged;

    void Start() {
        InputSystem.onActionChange += OnActionChange;
    }

    void OnDestroy() {
        InputSystem.onActionChange -= OnActionChange;
    }

    void OnActionChange(object obj, InputActionChange change) {
        if (GameManager.instance.quitting || change != InputActionChange.BoundControlsChanged) return;
        bindingsChanged?.Invoke();
    }

    public static string DisplayString(InputAction action, int bindingIndex) {
        return action.GetBindingDisplayString(bindingIndex, InputBinding.DisplayStringOptions.DontIncludeInteractions);
    }
}
