using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TTObject : MouseInteractable 
{
    public override CursorType CursorType => CursorType.Click;

    public TTProperty[] properties;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GeneratePropertyPanel()
    {
        EditorUIManager.Instance.ClearPropertyPanel();
        Transform propPanel = EditorUIManager.Instance.propertyPanel.contentArea;
        foreach (var prop in properties)
        {
            if (!prop.generateOptions.HasFlag(TTProperty.FieldGenerateOptions.Hidden))
            {
                prop.GenerateField(propPanel);
                prop.RefreshValueDisplays(prop.Value);
            }
        }
    }

    public void AddProperty(TTProperty prop)
    {
        if(properties == null) properties = new TTProperty[] { prop };
        else properties = properties.Append(prop).ToArray();
    }

    public void AddProperties(TTProperty[] props)
    {
        if (properties == null)
        {
            properties = new TTProperty[props.Length];
            Array.Copy(props, properties, props.Length);
        }
        else
        {
            List<TTProperty> l = properties.ToList();
            l.AddRange(props);
            properties = l.ToArray();
        }
    }

    public T FindProperty<T>(string name) where T : TTProperty
    {
        if(properties == null) return null;
        return properties.Where((p)=>p.name == name).FirstOrDefault() as T;
    }

    public T FindPropertyValue<T>(string name)
    {
        TTProperty p = FindProperty<TTProperty>(name);
        if(p == null) return default;
        return (T)Convert.ChangeType(p.Value, typeof(T));
    }

    public override void OnLeftClick()
    {
        GeneratePropertyPanel();
    }

    public override void OnRightClick()
    {
        //generate context menu
    }
}
