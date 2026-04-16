using UnityEngine;
using System.IO;
using System.Collections.Generic;
using static UnityEditor.PlayerSettings;

public class Settings : MonoBehaviour
{
    public string[] defaultSettings;
    public EditorPanel settingsPanel;

    public static string FilePath { get; private set; }
    private readonly static Dictionary<string,string> settings = new();

    private void Awake()
    {
        FilePath = Path.Combine(Application.persistentDataPath,"settings.txt");
        if(!File.Exists(FilePath)) File.WriteAllLines(FilePath, defaultSettings);
        Load();
    }

    public static string Get(string key) => settings[key];
    public static bool TryGet(string key, out string value)
    {
        value = string.Empty;

        if (settings.ContainsKey(key))
        {
            value = settings[key];
            return true;
        }
        
        return false;
    }

    public static void Set(string key, string value)
    {
        if (settings.ContainsKey(key)) settings[key] = value;
        else settings.Add(key, value);
    }

    public static string GetOrSetDefault(string key, string defaultVal)
    {
        if(!TryGet(key,out string v))
        {
            v = defaultVal;
            Set(key, v);
            Save();
        }
        return v;
    }

    public static void Save()
    {
        try
        {
            List<string> lines = new();
            foreach (var pair in settings) lines.Add($"{pair.Key} = {pair.Value}");
            File.WriteAllLines(FilePath, lines);
            EditorUIManager.Instance.Inform($"Settings saved successfully at {FilePath}.", "Settings Saved");
        }
        catch (IOException ioe)
        {
            EditorUIManager.Instance.Err("Error saving settings",ioe);
        }
    }

    public static void Load()
    {
        try
        {
            settings.Clear();
            foreach(var line in File.ReadAllLines(FilePath))
            {
                int eqInd = line.IndexOf('=');
                settings.Add(line[..eqInd].Trim(), line[(eqInd + 1)..].Trim());
            }
        }
        catch(IOException ioe)
        {
            EditorUIManager.Instance.Err("Error loading settings",ioe);
        }
    }

    public void LoadMenu()
    {
        settingsPanel.Open();
        settingsPanel.Clear();
        Transform content = settingsPanel.contentArea;

        foreach(var pair in settings)
        {
            string key = pair.Key;
            var inp = EditorUIManager.Instance.CreateLabeledInputField(content, key, TTProperty.FieldGenerateOptions.Default);
            inp.SetTextWithoutNotify(pair.Value);
            inp.onValueChanged.AddListener((e) => { Set(key, e); });
        }
    }
}
