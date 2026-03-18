using System;
using UnityEngine;

public abstract class PropertyLoader
{
    public abstract string Name { get; }

    protected object _value;

    public T GetValue<T>() => (T)Convert.ChangeType(_value, typeof(T));

    public virtual void Load(byte[] bytes, ref int index) 
        => throw new NotImplementedException($"{Name} PropertyLoader cannot be loaded with bytes");
    public virtual void Load(string text, ref int index)
        => throw new NotImplementedException($"{Name} PropertyLoader cannot be loaded with text");
    public virtual void Load(string[] lines, ref int index)
        => throw new NotImplementedException($"{Name} PropertyLoader cannot be loaded with lines");

    protected void Err(string msg) => TTLoader.Err(Name, msg);
    protected void Warn(string msg) => TTLoader.Warn(Name, msg);
    protected void Log(string msg) => TTLoader.Log(Name, msg);

    public static T LoadBytes<T, L>(byte[] bytes, ref int index) where L : PropertyLoader,new()
    {
        var loader = new L();
        loader.Load(bytes, ref index);
        return loader.GetValue<T>();
    }
}
