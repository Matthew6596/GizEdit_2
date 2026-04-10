using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BoolProperty : TTProperty
{
    public Toggle input;
    public byte trueValue=1, falseValue=0;

    public BoolProperty(string name, bool value, string info = "", UnityAction<ChangeEventData> onValueChange = null, bool defaultValue = false) : base(name, defaultValue, value, onValueChange, info)
    {

    }

    public override void GenerateField(Transform parent)
    {
        var field = EditorUIManager.Instance.CreateLabeledField(parent, name, generateOptions);
        input = EditorUIManager.Instance.CreateToggle(field, generateOptions);
        input.onValueChanged.AddListener((e) => { Value = e; });
    }

    public override void RefreshValueDisplays(object value)
    {
        if(input != null) input.SetIsOnWithoutNotify(value.Convert<bool>());
    }

    public override IEnumerable<byte> ToBytes() => new byte[] { Value.Convert<bool>() ? trueValue : falseValue };
}
