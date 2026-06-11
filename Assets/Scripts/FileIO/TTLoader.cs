using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TTLoader : MonoBehaviour
{
    public static TTLoader Instance {get; private set;}

    /// <summary>
    /// If true, updates objects to their newest verison upon loading.
    /// </summary>
    public static bool AutoVersionUpdate { get => Settings.Get("auto_version_update") == "true"; }
    public static Dictionary<string, int> TargetVersions { get; } = new();
    public static bool HasVersionTarget(string name, out int targetVersion) => TargetVersions.TryGetValue(name, out targetVersion);
    public static bool ShouldAddProperty(string name, int version, Func<int,bool> versionComparison)
    {
        bool hasTargVers = HasVersionTarget(name, out int targVers);
        return ((!hasTargVers && versionComparison(version)) || (hasTargVers && versionComparison(targVers)));
    }
    public static bool LogEnabled { get; set; }
    public static bool LoadingPaused { get; set; }
    public static string CurrentLoadingFilePath { get; private set; }
    public static FileDataType CurrentLoadingFileType { get; private set; }

    public string TTFileFormatsResourcePath;
    public TTGame game;

    private TTFileFormat[] fileFormats;

    private void Awake()
    {
        Instance = this;

        LoadTTFileFormats();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Settings.GetOrSetDefault("auto_version_update", "false");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LoadTTFileFormats()
    {
        //Get all formats from resources
        TextAsset[] formats = Resources.LoadAll<TextAsset>(TTFileFormatsResourcePath);
        fileFormats = new TTFileFormat[formats.Length];

        //Get TTFileFormats from each file's json
        for (int i = 0; i < formats.Length; i++)
        {
            fileFormats[i] = JsonUtility.FromJson<TTFileFormat>(formats[i].text);
            Debug.Log("loaded file format: " + fileFormats[i].name + ", " + fileFormats[i].ext);
        }
    }

    private TTFileFormat GetTTFileFormat(string ext, TTGame game) => fileFormats.Where((f) => f.ext == ext && f.game == game).FirstOrDefault();

    private readonly static string[] fileLoadOrder = new string[]
    {
        "_PC.GSC","",".GIZ"
    };

    private void WaitForEnd(IEnumerator ienumerator, Action endAction)
    {
        StartCoroutine(waitForEnd(ienumerator, endAction));
    }

    IEnumerator waitForEnd(IEnumerator ienumerator, Action endAction)
    {
        yield return ienumerator;
        endAction();
    }

    public void LoadALevel()
    {
        string[] res = SFB.StandaloneFileBrowser.OpenFolderPanel("Select a level", Settings.Get("tcs_path"), false);
        if (res.Length == 0) return;

        if (!TTResourceManager.WorkingGameLoaded)
        {
            WaitForEnd(TTResourceManager.Instance.LoadDataPaths(), () =>
            {
                /*Debug.Log("Action being actioned LEVEL");
                if (TTResourceManager.WorkingGameLoaded) StartCoroutine(LoadLevel(res[0]));
                else EditorUIManager.Instance.Inform("Things.gsc must be loaded for gizmo elements to load properly. Please try loading the level again.");*/
            });
        }
        else StartCoroutine(LoadLevel(res[0]));
    }

    public void LoadAFile()
    {
        string[] res = SFB.StandaloneFileBrowser.OpenFilePanel("Select a level", Settings.Get("tcs_path"), new SFB.ExtensionFilter[] { new("Any", "*") },false);
        if (res.Length == 0) return;

        if (Path.GetExtension(res[0]) == ".giz" && !TTResourceManager.WorkingGameLoaded)
        {
            WaitForEnd(TTResourceManager.Instance.LoadDataPaths(), () =>
            {
                /*Debug.Log("Action being actioned FILE");
                if (TTResourceManager.WorkingGameLoaded) StartCoroutine(LoadFile(res[0], () => { TTObjectManager.InitializeAllProperties(); }));
                else EditorUIManager.Instance.Inform("Things.gsc must be loaded for gizmo elements to load properly. Please try loading the .giz file again.");*/
            });
        }
        else StartCoroutine(LoadFile(res[0], () => { TTObjectManager.InitializeAllProperties(); }));
    }

    public static IEnumerator LoadLevel(string directory)
    {
        TTObjectManager.UnloadAll();

        //TTFileObject[] existingFiles = FindObjectsByType<TTFileObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        /*if (existingFiles.Length > 0) {
            bool cancel = false;
            EditorUIManager.Instance.Warn("Do you want to unload the files currently open?",null,"Unload Current Files?",
                ("Cancel", () => { cancel = true; }), ("No, Keep Loaded", () => { }), ("Yes, Unload", () => {
                    foreach(var existingFile in existingFiles)
                    {
                        existingFile.Destroy();
                        EditorUIManager.Instance.RemoveHierarchyRoot(existingFile);
                    }
                    EditorUIManager.Instance.ClearPropertyPanel();
                }
            ));
            while (EditorUIManager.IsPopupOpen) yield return null;
            if (cancel) yield break;
        }*/

        /*
        if (existingFiles.Length > 0)
        {
            foreach (var existingFile in existingFiles)
            {
                existingFile.Destroy();
                EditorUIManager.Instance.RemoveHierarchyRoot(existingFile);
            }
        }*/

        int loadIndex = 0;
        string levelName = Path.GetFileName(directory);
        //Load all files in dependent order
        while(loadIndex < fileLoadOrder.Length)
        {
            if (loadIndex < fileLoadOrder.Length && string.IsNullOrEmpty(fileLoadOrder[loadIndex]))
            {
                loadIndex++;
                continue;
            }
            //Load some files at same time if possible
            List<Coroutine> loadRoutines = new();
            int loadingCount = 0;
            while (loadIndex < fileLoadOrder.Length && !string.IsNullOrEmpty(fileLoadOrder[loadIndex])) //using "" to separate groups of files
            {
                string fpath = Path.Combine(directory, levelName+fileLoadOrder[loadIndex]);
                //If file with given extension exists, load it
                Debug.Log("attempting load: " + fpath);
                if (File.Exists(fpath))
                {
                    loadingCount++;
                    loadRoutines.Add(Instance.StartCoroutine(LoadFile(fpath, () => { loadingCount--; })));
                    Debug.Log("file exists, load routine added");
                }
                loadIndex++;
            }
            //Wait until files in batch done loading before continuing to next batch
            yield return new WaitUntil(() => loadingCount <= 0);

            //Load properties for objects created so far
            TTObjectManager.InitializeAllProperties();
        }
        Debug.LogWarning("does this routine ever end?"); //it does lowkey
    }

    public static IEnumerator LoadFile(string path, Action finished)
    {
        string ext = Path.GetExtension(path).ToLower()[1..]; //get ext lowercase without period
        var fileFormat = Instance.GetTTFileFormat(ext, Instance.game);

        if (ext == "giz" && !TTResourceManager.WorkingGameLoaded)
        {
            Instance.StartCoroutine(TTResourceManager.Instance.LoadDataPaths());
            yield break;
        }

        CurrentLoadingFilePath = path;
        CurrentLoadingFileType = fileFormat.type;

        yield return Instance.StartCoroutine(loadRoutine(path, fileFormat));
        finished?.Invoke();
    }

    static IEnumerator loadRoutine(string path, TTFileFormat fileFormat)
    {
        int progress = 0;
        EditorUIManager.Instance.ShowProgressBar($"Loading {fileFormat.ext} file {Path.GetFileName(path)}", "Loading file contents...");
        yield return null;

        Debug.Log("reached load routine");
        object fileContents = null;

        //Attempt to read file contents
        try
        {
            switch (fileFormat.type)
            {
                case FileDataType.Text: fileContents = File.ReadAllText(path); break;
                case FileDataType.Lines: fileContents = File.ReadAllLines(path); break;
                case FileDataType.Bytes: fileContents = File.ReadAllBytes(path); break;
            }
        }
        catch (IOException ioe)
        {
            Debug.LogError($"Error loading '{path}': {ioe}");
            EditorUIManager.Instance.UpdateProgressBar(progress / (float)fileFormat.loaders.Length, $"Error loading '{path}': {ioe}", EditorUIManager.Instance.errorColor);
            yield break;
        }

        LoadingPaused = false;
        //Load with each loader
        int index = 0;
        foreach (var loader in fileFormat.loaders)
        {
            EditorUIManager.Instance.UpdateProgressBar(progress / (float)fileFormat.loaders.Length, $"Loading {loader}... (this may take a minute)");
            yield return null;

            PropertyLoaderFactory.Load(loader, fileFormat.type, fileContents, ref index);
            while (LoadingPaused) yield return null;
            progress++;
        }
        EditorUIManager.Instance.CloseProgressBar();
    }

    public static void StartLoadSubroutine(IEnumerator routine)
    {
        LoadingPaused = true;
        Instance.StartCoroutine(routine);
    }

    public static void EndLoadSubroutine() => LoadingPaused = false;
    public static Coroutine UpdateLoadProgress(int progress, int total, string msg)
    {
        IEnumerator updateLoad()
        {
            EditorUIManager.Instance.UpdateProgressBar(progress / (float)total, msg);
            yield return null;
        }

        return Instance.StartCoroutine(updateLoad());
    }

    public static void Err(string loader,string msg)
    {
        Debug.LogError($"Loader '{loader}' gave error: '{msg}'");
    }

    public static void Warn(string loader, string msg)
    {
        Debug.LogWarning($"Loader '{loader}' gave warning: '{msg}'");
    }

    public static void Log(string loader, string msg)
    {
        Debug.Log($"Loader '{loader}' gave log: '{msg}'");
    }
}

public static class ConvertExt
{
    public static T Convert<T>(this object val) => (T)System.Convert.ChangeType(val, typeof(T));
}