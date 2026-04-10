using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class EnumProperty : TTProperty
{
    private Dictionary<string, byte> options;
    public TMP_Dropdown input;

    public EnumProperty(string name, byte value, Dictionary<string,byte> options, string info = "", UnityAction<ChangeEventData> onValueChange = null, byte defaultValue = 0) : base(name, defaultValue, options.ContainsValue(value) ? value : defaultValue, onValueChange, info)
    {
        this.options = options;
    }

    public override void GenerateField(Transform parent)
    {
        var field = EditorUIManager.Instance.CreateLabeledField(parent, name, generateOptions);
        input = EditorUIManager.Instance.CreateDropdown(field, generateOptions, preferredWidth);
        input.options.Clear();
        input.AddOptions(options.Keys.ToList());
        input.onValueChanged.AddListener((e) => { Value = options[input.options[e.Convert<int>()].text]; });
    }

    public override void RefreshValueDisplays(object value)
    {
        if (input == null) return;

        byte val = (byte)value;
        string str = options.Where((o) => o.Value == val).FirstOrDefault().Key;
        int ind = -1;
        for(int i=0; i<input.options.Count; i++) if (input.options[i].text == str) { ind = i; break; }
        input.SetValueWithoutNotify(ind);
    }

    public override IEnumerable<byte> ToBytes() => new byte[] { (byte)Value };
}
