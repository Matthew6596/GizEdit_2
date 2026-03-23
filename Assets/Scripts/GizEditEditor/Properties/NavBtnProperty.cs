using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NavBtnProperty : TTProperty
{
    public Toggle input;
    private Action onClickAction;

    public NavBtnProperty(string name, Action onClick) : base(name, null, null, null, "")
    {
        onClickAction = onClick;
    }

    public override void GenerateField(Transform parent)
    {
        var field = EditorUIManager.Instance.CreateButton(parent, generateOptions, preferredWidth);
        field.transform.GetChild(0).GetComponent<TMP_Text>().text = name;
        field.onClick.AddListener(() =>
        {
            onClickAction?.Invoke();
        });
    }

    public override IEnumerable<byte> ToBytes() => new byte[0];
}
