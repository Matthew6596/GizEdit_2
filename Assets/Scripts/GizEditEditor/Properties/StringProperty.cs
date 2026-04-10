using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;
using System;
using System.Text;

public class StringProperty : TTProperty
{
    public enum MaxSize { Byte, Short, Int, Long }

    private MaxSize maxSize;
    public TMP_InputField input;

    public StringProperty(string name, string value, MaxSize maxSize, string info = "",  UnityAction<ChangeEventData> onValueChange = null, string defaultValue = "") : base(name, defaultValue, value, onValueChange, info)
    {
        this.maxSize = maxSize;
    }

    public override void GenerateField(Transform parent)
    {
        input = EditorUIManager.Instance.CreateLabeledInputField(parent, name, generateOptions, preferredWidth);
        input.characterLimit = (maxSize) switch { MaxSize.Byte => 255, MaxSize.Short => short.MaxValue, _ => int.MaxValue };
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
        int len = val.Length;
        switch (maxSize)
        {
            case MaxSize.Byte: bytes.Add((byte)len); break;
            case MaxSize.Short: bytes.AddRange(BitConverter.GetBytes((short)len)); break;
            default: bytes.AddRange(BitConverter.GetBytes(len)); break;
        }
        bytes.AddRange(Encoding.UTF8.GetBytes(val));
        return bytes;
    }
}
