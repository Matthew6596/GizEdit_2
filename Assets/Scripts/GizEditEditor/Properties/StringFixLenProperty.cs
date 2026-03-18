using System.Collections.Generic;
using System.Text;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class StringFixLenProperty : TTProperty
{
    private int length;
    public TMP_InputField input;

    public StringFixLenProperty(string name, string value, int length, string info = "", UnityAction<ChangeEventData> onValueChange = null, string defaultValue = "") : base(name, defaultValue, value, onValueChange, info)
    {
        this.length = length;
    }

    public override void GenerateField(Transform parent)
    {
        input = EditorUIManager.Instance.CreateLabeledInputField(parent, name, generateOptions, preferredWidth);
        input.characterLimit = length;
        input.onValueChanged.AddListener((e) => { Value = e.ToString(); });
    }

    public override void RefreshValueDisplays(object value)
    {
        if (input != null) input.SetTextWithoutNotify(value.ToString());
    }

    public override IEnumerable<byte> ToBytes()
    {
        List<byte> bytes = new();
        string val = Value.ToString();
        bytes.AddRange(Encoding.UTF8.GetBytes(val));
        if (bytes.Count > length) return bytes.Take(length);
        else if (bytes.Count < length) bytes.AddRange(new byte[length - bytes.Count]);
        return bytes;
    }
}
