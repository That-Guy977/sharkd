using System;
using UnityEngine.InputSystem;

class InputBindingsUpdater : Singleton<InputBindingsUpdater> {
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
}
