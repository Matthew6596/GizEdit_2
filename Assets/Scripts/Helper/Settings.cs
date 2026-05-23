using UnityEngine;
using System.IO;
using System.Collections.Generic;

[DefaultExecutionOrder(-2)]
public class Settings : MonoBehaviour
{
    public static Settings Instance { get; private set; }

    public string[] defaultSettings;
    public EditorPanel settingsPanel;

    public static string FilePath { get; private set; }
    private readonly static Dictionary<string,string> settings = new();

    private void Awake()
    {
        Instance = this;
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
            return value != string.Empty;
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
            Save(silent:true);
        }
        return v;
    }

    public static void Save(bool silent=false)
    {
        try
        {
            List<string> lines = new();
            foreach (var pair in settings) lines.Add($"{pair.Key} = {pair.Value}");

            if (FilePath == "" || !Directory.Exists(Path.GetDirectoryName(FilePath))) 
                FilePath = Path.Combine(Application.persistentDataPath, "settings.txt");

            File.WriteAllLines(FilePath, lines);
            if(!silent) EditorUIManager.Instance.Inform($"Settings saved successfully at {FilePath}.", "Settings Saved");
        }
        catch (IOException ioe)
        {
            if(!silent) EditorUIManager.Instance.Err("Error saving settings",ioe);
        }
    }

    public static void Load()
    {
        try
        {
            settings.Clear();
            string[] lines = File.ReadAllLines(FilePath);
            foreach (var line in lines)
            {
                int eqInd = line.IndexOf('=');
                string key = line[..eqInd].Trim();
                string val = line[(eqInd + 1)..].Trim();
                settings.Add(key, val);
            }

            bool defaultAdded = false;
            foreach (var defaultSetting in Instance.defaultSettings)
            {
                int eqInd = defaultSetting.IndexOf('=');
                string key = defaultSetting[..eqInd].Trim();
                if (!settings.ContainsKey(key))
                {
                    settings.Add(key, defaultSetting[(eqInd + 1)..].Trim());
                    defaultAdded = true;
                }
            }
            if (defaultAdded) Save(silent: true);
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
