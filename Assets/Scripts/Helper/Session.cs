using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A static class for managing data that is persistent through Unity scenes or meant to be global.
/// </summary>
public static class Session
{
    private readonly static Dictionary<string, object> data = new();
    private readonly static List<string> flags = new();

    public static event Action<string> OnFlagSet = (s) => { };
    public static event Action<string> OnFlagRemove = (s) => { };

    /// <summary>
    /// Clears all session variables.
    /// </summary>
    public static void Clear() {  data.Clear(); }

    /// <summary>
    /// Gets the session variable of the inputed key.
    /// </summary>
    /// <typeparam name="T">The data type of the session variable.</typeparam>
    /// <param name="key">The key of the session variable.</param>
    /// <returns>The value of the session variable.</returns>
    public static T Get<T>(string key, T defaultValue = default)
    {
        if (data.ContainsKey(key))
        {
            return (T)Convert.ChangeType(data[key], typeof(T));
        }
        else
        {
            Debug.LogWarning($"Attempted to get key \"{key}\" from Session. Key was not found so a default value was returned.");
            return defaultValue;
        }
    }

    public static bool GetFlag(string key) => flags.Contains(key);

    public static void SetFlag(string key)
    {
        flags.Add(key);
        OnFlagSet.Invoke(key);
    }
    public static void RemoveFlag(string key)
    {
        flags.Remove(key);
        OnFlagRemove.Invoke(key);
    }
    public static void ClearFlags() => flags.Clear();

    public static string[] GetAllFlags() => flags.ToArray();

    /// <summary>
    /// Tries to get the session variable of the inputted key if it exists.
    /// </summary>
    /// <typeparam name="T">The data type of the session variable.</typeparam>
    /// <param name="key">The key of the desired session variable.</param>
    /// <param name="value">The value of the session variable if it exists, default value otherwise.</param>
    /// <returns>True if the key exists, False otherwise.</returns>
    public static bool TryGet<T>(string key, out T value)
    {
        if (data.ContainsKey(key))
        {
            value = (T)System.Convert.ChangeType(data[key], typeof(T));
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }

    /// <summary>
    /// Sets or creates the session variable of the inputted key with the inputted value.
    /// </summary>
    /// <typeparam name="T">The data type of the session variable.</typeparam>
    /// <param name="key">The key for the session variable.</param>
    /// <param name="value">The value for the session variable.</param>
    public static void Set<T>(string key, T value)
    {
        if (data.ContainsKey(key)) data[key] = value;
        else data.Add(key, value);
    }

    /// <summary>
    /// Removes a specific session variable from the session dictionary.
    /// </summary>
    /// <param name="key">The key of the session variable to be removed.</param>
    public static void Remove(string key)
    {
        if (data.ContainsKey(key)) data.Remove(key);
        else Debug.LogWarning($"{key} was not found in Session and couldn't be removed.");
    }
}
