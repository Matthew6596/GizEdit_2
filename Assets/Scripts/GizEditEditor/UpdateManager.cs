using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class UpdateManager : MonoBehaviour
{
    const string repoUrl = "https://github.com/Matthew6596/GizEdit_2";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckLatestVers()
    {
        AppVers appVersion = new(Application.version);
        string latestReleaseURL = $"{repoUrl}/releases/latest";
        var fetchRoutine = StartFetch(latestReleaseURL, (s) =>
        {
            //Parse latest release version
            int tagNameInd = s.IndexOf("tag_name") + "tag_name".Length + 1;
            string[] versStrs = s[tagNameInd..s.IndexOf('\"', tagNameInd)].Split("&amp;");
            Debug.Log("vers1: " + versStrs[0]);
            Debug.Log("vers2: " + versStrs[1]);
            AppVers latestVers;
            latestVers = new(versStrs[0]);
            string note = "";
            if (versStrs.Length > 1 && versStrs[1].Contains("experimental=1")) note = " Note: this version is experimental";

            //Compare latest release version to current
            if (!appVersion.Equals(latestVers))
            {
                EditorUIManager.Instance.Inform($"A new version of GizEdit is available: {versStrs[0]} " + note, "Update Available",
                    ("Close", () => { }
                ), ("Go to Update", () => { Application.OpenURL(latestReleaseURL); }
                ));
            }
            else if (note != "") EditorUIManager.Instance.Warn("This version of GizEdit is experimental.", null, "Experimental Version");
            else EditorUIManager.Instance.Inform("No new version of GizEdit was found.", "No Updates Available");

        }, (e) =>
        {
            EditorUIManager.Instance.Err("Couldn't fetch latest GizEdit release: "+e, null, "Update Fetch Error");
        });
    }

    public void ReportBug() => Application.OpenURL($"{repoUrl}/issues");

    public Coroutine StartFetch(string url, Action<string> onSuccess, Action<string> onError)
    {
        return StartCoroutine(Fetch(url, onSuccess, onError));
    }

    IEnumerator Fetch(string url, Action<string> onSuccess, Action<string> onError)
    {

        UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success) onError.Invoke(req.error);
        else onSuccess.Invoke(req.downloadHandler.text);
    }
}

public struct AppVers
{
    public int major, minor, hotfix;
    public AppVers(int major, int minor, int hotfix)
    {
        this.major = major;
        this.minor = minor;
        this.hotfix = hotfix;
    }

    public AppVers(string vers)
    {
        string[] split = vers.Split('.');
        major = int.Parse(split[0]);
        minor = int.Parse(split[1]);
        hotfix = int.Parse(split[2]);
    }

    public override readonly bool Equals(object obj) => obj is AppVers vers && vers.major == major && vers.minor == minor && vers.hotfix == hotfix;

    public override readonly int GetHashCode() => base.GetHashCode();

    public override readonly string ToString() => $"{major}.{minor}.{hotfix}";
}
