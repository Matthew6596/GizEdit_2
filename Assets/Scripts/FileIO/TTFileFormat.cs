using System;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public enum TTGame { TCS, LB1, LIJ1 }
public enum FileDataType { Text, Bytes, Lines }

[Serializable]
public struct TTFileFormat
{
    public string name;
    public string ext;
    [SerializeField]
    public TTGame game;
    [SerializeField]
    public FileDataType type;
    public string[] loaders;

    public static TTFileFormat FromFile(string path)
    {
        try
        {
            string txt = File.ReadAllText(path);
            var ttfileformat = JsonUtility.FromJson<TTFileFormat>(txt);
            return ttfileformat;
        }
        catch(IOException ioe)
        {
            Debug.LogError($"IO Error reading TTFileFormat: {ioe}");
        }

        return default;
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/TTFileFormat", false, 1)]
    private static void CreateNewAsset()
    {
        ProjectWindowUtil.CreateAssetWithContent("ttg_ext_format.json", JsonUtility.ToJson(new TTFileFormat(), true));
    }
#endif
}
