using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;

public class ArrayProperty<P> : TTProperty where P : TTProperty
{
    public PropertyLoader itemLoader;
    public Func<ArrayPropItemInfo,P> createNewItem;
    public object defaultItemValue;
    public string genericItemName;
    public IntegerProperty countProp;

    private Transform contentArea;

    public ArrayProperty(string name, P[] value, string info = "", UnityAction<ChangeEventData> onValueChange = null, P[] defaultValue = null) : base(name, defaultValue, value, onValueChange, info)
    {

    }

    public override void GenerateField(Transform parent)
    {
        if (generateOptions.HasFlag(FieldGenerateOptions.Hidden)) return;
        if (generateOptions.HasFlag(FieldGenerateOptions.Partial) && !generateOptions.HasFlag(FieldGenerateOptions.ShowName)) return;
        bool showBtns = !generateOptions.HasFlag(FieldGenerateOptions.Readonly);
        bool childValsShown = !generateOptions.HasFlag(FieldGenerateOptions.Partial);
        bool hasLabel = generateOptions.HasFlag(FieldGenerateOptions.ShowName);

        if (contentArea != null) GameObject.Destroy(contentArea.gameObject);

        Transform p = parent;
        contentArea = EditorUIManager.Instance.CreateContentArea(parent, LayoutMode.Vertical);
        var children = (P[])Value;
        for(int i=0; i<children.Length; i++)
        {
            P child = children[i];
            var childField = EditorUIManager.Instance.CreateContentAreaBG(contentArea, LayoutMode.Vertical, EditorColorType.WindowTertiary);
            Transform titleField = null;

            if (hasLabel)
            {
                titleField = EditorUIManager.Instance.CreateLabeledField(contentArea, child.name, FieldGenerateOptions.Default, 240);
                titleField.GetComponent<HorizontalLayoutGroup>().padding = new(2, 4, 2, 2);
            }

            if (showBtns && hasLabel)
            {
                var delBtn = EditorUIManager.Instance.CreateIconButton(titleField, EditorUIManager.Instance.trashIcon, FieldGenerateOptions.Default, 14);
                int ind = i;
                delBtn.onClick.AddListener(() => { RemoveChild(ind); GenerateField(p); });
            }

            if(childValsShown) child.GenerateField(childField);

            if(showBtns && !hasLabel)
            {
                var delBtn = EditorUIManager.Instance.CreateIconButton(childField.GetChild(1), EditorUIManager.Instance.trashIcon, FieldGenerateOptions.Default, 14);
                int ind = i;
                delBtn.onClick.AddListener(() => { RemoveChild(ind); GenerateField(p); });
            }
        }
        if (showBtns)
        {
            var addField = EditorUIManager.Instance.CreateLabeledField(contentArea, $"Add {genericItemName}", FieldGenerateOptions.Default, 240);
            addField.GetComponent<HorizontalLayoutGroup>().padding = new(2, 4, 2, 2);
            var addBtn = EditorUIManager.Instance.CreateIconButton(addField, EditorUIManager.Instance.plusIcon, FieldGenerateOptions.Default, 14);
            addBtn.onClick.AddListener(() => { AddNewChild(); GenerateField(p); });
        }
    }

    public override void RefreshValueDisplays(object value)
    {
        //Debug.Log("Refreshing " + name+" (children)");
        foreach (var child in Value as P[]) child.RefreshValueDisplays(child.Value);
    }

    public P AddNewChild(int index=-1)
    {
        var list = (Value as P[]).ToList();
        var item = createNewItem(new($"{genericItemName} {(index == -1 ? list.Count : index)}", defaultItemValue));
        if (index == -1) list.Add(item);
        else list.Insert(index, item);
        Value = list.ToArray();
        return item;
    }

    public void RemoveChild(int index)
    {
        List<P> list = (Value as P[]).ToList();
        list.RemoveAt(index);
        Value = list.ToArray();
    }

    public static ArrayProperty<T> Create<T,V>(string name, string info, string itemName, PropertyLoader itemLoader, V defaultItemValue, byte[] bytes, ref int index, IntegerProperty countProp, Func<ArrayPropItemInfo,T> createNewItem, UnityAction<ChangeEventData> onValueChange=null, FieldGenerateOptions generateOptions = FieldGenerateOptions.Default) where T : TTProperty
    {
        int childCount = countProp.Value.Convert<int>();

        T[] props = new T[childCount];
        for(int i=0; i<childCount; i++)
        {
            itemLoader.Load(bytes, ref index);
            if (TTLoader.LogEnabled) Debug.Log($"ArrayProperty Create Log: item {i}, index: {index}");
            props[i] = createNewItem(new($"{itemName} {i}", itemLoader.GetValue<V>()));
        }
        ArrayProperty<T> arr = new(name, props, info, onValueChange, new T[0])
        {
            itemLoader = itemLoader,
            defaultItemValue = defaultItemValue,
            genericItemName = itemName,
            createNewItem = createNewItem,
            generateOptions = generateOptions,
            countProp = countProp,
        };
        arr.onValueChanged.AddListener((e) =>
        {
            countProp.Value = (e.value as P[]).Length;
        });
        return arr;
    }

    public override IEnumerable<byte> ToBytes()
    {
        List<byte> bytes = new();
        foreach (var item in Value as P[]) bytes.AddRange(item.ToBytes());
        return bytes;
    }

    public override void ResetToDefault()
    {
        
    }
}

public class ArrayPropItemInfo
{
    public string name;
    public object defaultValue;

    public ArrayPropItemInfo(string name, object defaultValue)
    {
        this.name = name;
        this.defaultValue = defaultValue;
    }
}