using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

struct Variable : IVariable {
    public object value;
    public object GetSourceValue(ISelectorInfo _) => value;
}
