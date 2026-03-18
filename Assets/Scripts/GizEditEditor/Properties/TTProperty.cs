using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class TTProperty
{
    public string name;

    public UnityEvent<ChangeEventData> onValueChanged = new();

    protected object _value;

    /// <summary>
    /// The value of this property. Setting this will invoke OnValueChanged events, but SetValue can avoid this
    /// </summary>
    public object Value { 
        get
        {
            return _value;
        }
        set
        {
            _value = value;
            RefreshValueDisplays(_value);
            onValueChanged.Invoke(new(this,_value));
        }
    }

    private object defaultValue;
    private string info;

    public FieldGenerateOptions generateOptions = FieldGenerateOptions.Default;
    public int preferredWidth=80;

    public TTProperty(string name, object defaultValue, object value, UnityAction<ChangeEventData> onChangeAction, string info)
    {
        this.name = name;
        this.defaultValue = defaultValue;
        this.info = info;

        if(onChangeAction != null) onValueChanged.AddListener(onChangeAction);

        object val = value;
        TTObjectManager.AddPropertyInitializationListener(()=> { Value = val; });
    }

    public virtual void RefreshValueDisplays(object value) { }

    public void ResetToDefault()
    {
        Value = defaultValue;
    }

    /// <summary>
    /// Set the Value without invoking OnValueChanged events
    /// </summary>
    protected void SetValueWithoutNotify(object val)
    {
        _value = val;
    }

    public abstract void GenerateField(Transform parent);

    public class ChangeEventData
    {
        public TTProperty sender;
        public object value;

        public ChangeEventData(TTProperty sender, object value)
        {
            this.sender = sender;
            this.value = value;
        }
    }

    [Flags]
    public enum FieldGenerateOptions
    {
        None = 0, NewLine=0b0001, ShowName=0b0010, Hidden=0b0100, Readonly=0b1000,
        Default = NewLine | ShowName
    }

    public virtual IEnumerable<byte> ToBytes() { return new byte[0]; }
    public virtual IEnumerable<string> ToLines() { return new string[0]; }
    public virtual string ToText() { return ""; }
}
