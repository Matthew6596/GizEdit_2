using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditOptionProperty : TTProperty
{
    private Button btn;

    public EditOptionProperty(string path, Action onClick, string info="") : base(path, null, null, null, info)
    {
        btn = EditorUIManager.Instance.AddMenuOption(path, onClick);
    }

    public override void GenerateField(Transform parent)
    {
        
    }

    public override void Destroy()
    {
        GameObject.Destroy(btn.gameObject);
    }
}
