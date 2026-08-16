using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NavLblProperty : TTProperty
{
    public NavLblProperty(string name) : base(name, null, null, null, "")
    {
    }

    public override void GenerateField(Transform parent)
    {
        var field = EditorUIManager.Instance.CreateLabeledField(parent, name, generateOptions, preferredWidth);
    }

    public override IEnumerable<byte> ToBytes() => new byte[0];
}
