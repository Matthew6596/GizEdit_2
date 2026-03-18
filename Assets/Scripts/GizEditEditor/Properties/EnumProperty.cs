using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class EnumProperty : TTProperty
{
    private string[] options;
    public TMP_Dropdown input;

    public EnumProperty(string name, int value, string[] options, string info = "", UnityAction<ChangeEventData> onValueChange = null, int defaultValue = 0) : base(name, defaultValue, value, onValueChange, info)
    {
        this.options = options;
    }

    public override void GenerateField(Transform parent)
    {
        var field = EditorUIManager.Instance.CreateLabeledField(parent, name, generateOptions);
        input = EditorUIManager.Instance.CreateDropdown(field, generateOptions, preferredWidth);
        input.options.Clear();
        input.AddOptions(options.ToList());
        input.onValueChanged.AddListener((e) => { Value = e.Convert<int>(); });
    }

    public override void RefreshValueDisplays(object value)
    {
        if (input != null) input.SetValueWithoutNotify(value.Convert<int>());
    }

    public override IEnumerable<byte> ToBytes() => new byte[] { Value.Convert<byte>() };
}
