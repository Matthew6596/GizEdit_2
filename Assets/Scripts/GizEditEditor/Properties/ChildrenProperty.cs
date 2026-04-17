using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;

public class ChildrenProperty : TTProperty, IObjectProperty
{
    public PropertyLoader childLoader;
    public byte[] defaultChildBytes;
    public string genericChildName;

    private Transform contentArea;
    private Transform parent;

    public ChildrenProperty(string name, ChildProperty[] value, string info = "", UnityAction<ChangeEventData> onValueChange = null, ChildProperty[] defaultValue = null) : base(name, defaultValue, value, onValueChange, info)
    {

    }

    public override void GenerateField(Transform parent)
    {
        if (generateOptions.HasFlag(FieldGenerateOptions.Hidden)) return;
        if (generateOptions.HasFlag(FieldGenerateOptions.Partial) && !generateOptions.HasFlag(FieldGenerateOptions.ShowName)) return;
        bool showBtns = !generateOptions.HasFlag(FieldGenerateOptions.Readonly);
        bool childValsShown = !generateOptions.HasFlag(FieldGenerateOptions.Partial);

        if (contentArea != null) GameObject.Destroy(contentArea.gameObject);

        Transform p = parent;
        contentArea = EditorUIManager.Instance.CreateContentArea(parent, LayoutMode.Vertical);
        var children = (ChildProperty[])Value;
        for(int i=0; i<children.Length; i++)
        {
            ChildProperty child = children[i];
            var childField = EditorUIManager.Instance.CreateContentAreaBG(contentArea, LayoutMode.Vertical, EditorColorType.WindowTertiary);
            var titleField = EditorUIManager.Instance.CreateLabeledField(contentArea, child.name, FieldGenerateOptions.Default, 240);
            titleField.GetComponent<HorizontalLayoutGroup>().padding = new(2, 4, 2, 2);
            if (showBtns)
            {
                var delBtn = EditorUIManager.Instance.CreateIconButton(titleField, EditorUIManager.Instance.trashIcon, FieldGenerateOptions.Default, 14);
                int ind = i;
                delBtn.onClick.AddListener(() => { RemoveChild(ind); GenerateField(p); });
            }
            if(childValsShown) child.GenerateField(contentArea);
        }
        if (showBtns)
        {
            var addField = EditorUIManager.Instance.CreateLabeledField(contentArea, $"Add {genericChildName}", FieldGenerateOptions.Default, 240);
            addField.GetComponent<HorizontalLayoutGroup>().padding = new(2, 4, 2, 2);
            var addBtn = EditorUIManager.Instance.CreateIconButton(addField, EditorUIManager.Instance.plusIcon, FieldGenerateOptions.Default, 14);
            addBtn.onClick.AddListener(() => { AddNewChild(); GenerateField(p); });
        }
    }

    public override void RefreshValueDisplays(object value)
    {
        //Debug.Log("Refreshing " + name+" (children)");
        foreach (var child in Value as ChildProperty[]) child.RefreshValueDisplays(child.Value);
    }

    public T[] GetChildrenValues<T>() where T : TTObject
    {
        List<T> values = new();
        foreach (var child in Value as ChildProperty[]) values.Add(child.Value as T);
        return values.ToArray();
    }

    public T GetChildValue<T>(int index) where T : TTObject
    {
        var arr = Value as ChildProperty[];
        return arr[index].Value as T;
    }

    public ChildProperty AddNewChild(int index=-1)
    {
        var list = (Value as ChildProperty[]).ToList();
        var childObj = childLoader.LoadNewTTObject(defaultChildBytes);
        childObj.transform.SetParent(parent);
        var child = CreateChildProp(childObj, index == -1 ? list.Count : index, genericChildName);
        if (index == -1) list.Add(child);
        else list.Insert(index, child);
        Value = list.ToArray();
        EditorUIManager.Instance.RefreshHierarchy();
        return child;
    }

    public void RemoveChild(int index)
    {
        List<ChildProperty> list = (Value as ChildProperty[]).ToList();
        list[index].Destroy();
        list.RemoveAt(index);
        Value = list.ToArray();
        EditorUIManager.Instance.RefreshHierarchy();
    }

    public static ChildProperty CreateChildProp(TTObject childObj, int indexId, string childName) => new($"{childName} {indexId}", childObj);

    //public static ChildProperty CreateChildProp(int indexId, PropertyLoader loader, byte[] defaultBytes, string childName) => new($"{childName} {indexId}", loader.LoadNewTTObject(defaultBytes));

    /*public ChildProperty[] LoadChildArray<T>(PropertyLoader loader, byte[] bytes, ref int index, int count, string name) where T : TTObject
    {
        List<ChildProperty> childProperties = new();
        for (int i = 0; i < count; i++)
        {
            loader.Load(bytes, ref index);
            childProperties.Add(CreateChildProp(name, loader.GetValue<T>(), i));
        }
        return childProperties.ToArray();
    }*/

    public static ChildrenProperty Create<T>(string name, string info, string childName, PropertyLoader childLoader, byte[] defaultChildBytes, byte[] bytes, ref int index, int childCount, UnityAction<ChangeEventData> onValueChange, FieldGenerateOptions generateOptions = FieldGenerateOptions.Default) where T : TTObject
    {
        List<ChildProperty> childProps = new();
        for(int i=0; i<childCount; i++)
        {
            childLoader.Load(bytes, ref index);
            if (TTLoader.LogEnabled) Debug.Log($"ChildrenProperty Create Log: child {i}, index: {index}");
            childProps.Add(CreateChildProp(childLoader.GetValue<T>(), i, childName));
        }
        ChildrenProperty children = new(name, childProps.ToArray(), info, onValueChange, new ChildProperty[0])
        {
            childLoader = childLoader,
            defaultChildBytes = defaultChildBytes,
            genericChildName = childName,
            generateOptions = generateOptions
        };
        return children;
    }

    public override IEnumerable<byte> ToBytes()
    {
        List<byte> bytes = new();
        foreach (var child in Value as ChildProperty[]) bytes.AddRange(child.ToBytes());
        return bytes;
    }

    public override void ResetToDefault()
    {
        
    }

    public override void Destroy()
    {
        foreach (var child in Value as ChildProperty[]) child.Destroy();
    }

    public void ParentObjects(Transform parent)
    {
        this.parent = parent;
        foreach (var child in Value as ChildProperty[]) ((TTObject)child.Value).transform.SetParent(parent);
    }
}
