using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

[DisplayName("Input Action")]
class InputActionVariable : IVariableValueChanged, IVariableGroup {
    public InputActionReference actionReference;

    bool initialized;
    List<int> bindingIndices;
    Dictionary<string, int> compositeParts;

    InputAction action => actionReference.action;

    public event Action<IVariable> ValueChanged;

    void Clean() {
        initialized = false;
        InputBindingsUpdater.instance.bindingsChanged -= UpdateValue;
        Application.quitting -= Clean;
    }

    public object GetSourceValue(ISelectorInfo _) {
        if (!initialized) {
            InputBindingsUpdater.instance.bindingsChanged += UpdateValue;
            bindingIndices = new();
            compositeParts = new();
            InputBinding bindingMask = InputBinding.MaskByGroup("Keyboard");
            var bindings = action.bindings;
            for (int i = 0; i < bindings.Count; i++) {
                InputBinding binding = bindings[i];
                if (bindingMask.Matches(binding)) {
                    bindingIndices.Add(i);
                    if (!string.IsNullOrEmpty(binding.name)) {
                        compositeParts[binding.name] = i;
                    }
                }
            }
            initialized = true;
#if UNITY_EDITOR
            Application.quitting += Clean;
#endif
        }
        return this;
    }

    public override string ToString() {
        string result = "";
        foreach (int bindingIndex in bindingIndices) {
            result += DisplayString(bindingIndex);
        }
        return result;
    }

    public bool TryGetValue(string key, out IVariable value) {
        value = null;
        if (compositeParts.TryGetValue(key, out var bindingIndex)) {
            value = new Variable { value = DisplayString(bindingIndex) };
            return true;
        }
        return false;
    }

    string DisplayString(int bindingIndex) {
        string displayString = action.GetBindingDisplayString(bindingIndex, InputBinding.DisplayStringOptions.DontIncludeInteractions);
        if (displayString.Length == 1) {
            return displayString;
        } else {
            return $"[{displayString}]";
        }
    }

    void UpdateValue() {
        ValueChanged?.Invoke(this);
    }
}
