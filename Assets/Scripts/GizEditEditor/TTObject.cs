using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TTObject : MouseInteractable 
{
    public static TTObject LastSelectedObject { get; private set; }

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

    public virtual void InitStaticProperties(){}

    public void GeneratePropertyPanel()
    {
        LastSelectedObject = this;
        EditorGizmoManager.DestroyAllGizmos();
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

    public void InsertProperties(TTProperty[] props, int index)
    {
        if(properties == null)
        {
            properties = new TTProperty[props.Length];
            Array.Copy(props, properties, props.Length);
        }
        else
        {
            List<TTProperty> l = properties.ToList();
            l.InsertRange(index, props);
            properties = l.ToArray();
        }
    }

    public void RemoveProperties(int index, int count)
    {
        if (properties == null) return;
        
        List<TTProperty> l = properties.ToList();
        l.RemoveRange(index, count);
        properties = l.ToArray();
    }

    public void PrependProperty(TTProperty prop)
    {
        if (properties == null) properties = new TTProperty[] { prop };
        else properties = properties.Prepend(prop).ToArray();
    }

    public void InsertProperty(TTProperty prop, int index)
    {
        if (properties == null) properties = new TTProperty[] { prop };
        else
        {
            List<TTProperty> props = properties.ToList();
            props.Insert(index, prop);
            properties = props.ToArray();
        }
    }

    public void InsertPropertyAfter(TTProperty prop, string otherPropName)
    {
        if (properties == null) properties = new TTProperty[] { prop };
        else
        {
            int propInd = FindPropertyIndex(otherPropName);
            if (propInd == -1 || propInd == properties.Length-1) AddProperty(prop);
            else InsertProperty(prop, propInd+1);
        }
    }

    public void InsertPropertyBefore(TTProperty prop, string otherPropName)
    {
        if (properties == null) properties = new TTProperty[] { prop };
        else
        {
            int propInd = FindPropertyIndex(otherPropName);
            if (propInd == -1) PrependProperty(prop);
            else InsertProperty(prop, propInd);
        }
    }

    public TTProperty FindProperty(string name)
    {
        if (properties == null) return null;
        return properties.Where((p) => p.name == name).FirstOrDefault();
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

    public int FindPropertyIndex(string name) => properties==null ? -1 : properties.Select((p, i) => (p, i)).First((e) => e.p.name == name).i;

    public void ResetToDefault()
    {
        foreach (var prop in properties) prop.ResetToDefault();
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
