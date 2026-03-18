using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;
using System;

public class IntegerProperty : TTProperty
{
    public enum IntType { Byte, Short, Int, Long, SByte, UShort, UInt, ULong }

    private IntType type;
    public TMP_InputField input;

    public IntegerProperty(string name, long value, IntType type, string info = "", UnityAction<ChangeEventData> onValueChange = null, long defaultValue = 0) : base(name, defaultValue, value, onValueChange,info)
    {
        this.type = type;
    }

    public override void GenerateField(Transform parent)
    {
        input = EditorUIManager.Instance.CreateLabeledInputField(parent, name, generateOptions, preferredWidth);
        input.characterValidation = TMP_InputField.CharacterValidation.Integer;
        input.onValueChanged.AddListener((e) =>
        {
            if (long.TryParse(e.ToString(), out long v)) Value = v;
        });
    }

    public override void RefreshValueDisplays(object value)
    {
        if (input != null) input.SetTextWithoutNotify(value.ToString());
    }

    public override IEnumerable<byte> ToBytes() => (type) switch
    {
        IntType.Byte => new byte[] { Value.Convert<byte>() },
        IntType.Short => BitConverter.GetBytes(Value.Convert<short>()),
        IntType.Int => BitConverter.GetBytes(Value.Convert<int>()),
        IntType.Long => BitConverter.GetBytes((long)Value),
        IntType.SByte => new byte[] { (byte)Value.Convert<sbyte>() },
        IntType.UShort => BitConverter.GetBytes(Value.Convert<ushort>()),
        IntType.UInt => BitConverter.GetBytes(Value.Convert<uint>()),
        IntType.ULong => BitConverter.GetBytes(Value.Convert<ulong>()),
        _ => BitConverter.GetBytes(Value.Convert<int>())
    };
}
