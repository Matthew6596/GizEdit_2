using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class FloatProperty : TTProperty
{
    public enum FloatType { Float, Double };

    private FloatType type;
    public TMP_InputField input;

    public FloatProperty(string name, double value, FloatType type, string info = "", UnityAction<ChangeEventData> onValueChange = null, double defaultValue = 0) : base(name, defaultValue, value, onValueChange, info)
    {
        this.type = type;
    }

    public override void GenerateField(Transform parent)
    {
        input = EditorUIManager.Instance.CreateLabeledInputField(parent, name, generateOptions, preferredWidth);
        input.characterValidation = TMP_InputField.CharacterValidation.Decimal;
        input.onValueChanged.AddListener((e) => 
        {
            if (double.TryParse(e.ToString(), out double v) && (type==FloatType.Double?v:v.Convert<float>()) != (type==FloatType.Double?Value.Convert<double>() : Value.Convert<float>())) Value = v;
        });
    }

    public override void RefreshValueDisplays(object value)
    {
        if (input != null) input.SetTextWithoutNotify(value.ToString());
    }

    public override IEnumerable<byte> ToBytes() => (type) switch
    {
        FloatType.Float => BitConverter.GetBytes(Value.Convert<float>()),
        FloatType.Double => BitConverter.GetBytes((double)Value),
        _ => BitConverter.GetBytes(Value.Convert<float>())
    };
}
