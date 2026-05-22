using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SFB;

public class TTExporter : MonoBehaviour
{
    public static TTExporter Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void Export()
    {
        ExportOptions options = new();

        //string[] ret = StandaloneFileBrowser.OpenFolderPanel("Select Level Export Location", Settings.Get("tcs_path"), false);
        //if (ret.Length == 0) return;
        //options.path = ret[0];

        //TEMP no other files exporting
        string ret = StandaloneFileBrowser.SaveFilePanel("Export GIZ File", Settings.Get("tcs_path"), "gizmos.giz", "giz");
        if (ret == "" || !Directory.Exists(Path.GetDirectoryName(ret))) return;
        options.path = ret;

        //var options = EditorUIManager.Instance.
        StartCoroutine(Export(options));
    }

    public IEnumerator Export(ExportOptions options)
    {
        EditorUIManager.Instance.ShowProgressBar("Exporting Files", "Finding all files to export...");

        var files = FindObjectsByType<TTFileObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        int count = 0;
        for(int i=0; i<files.Length; i++)
        {
            var file = files[i];

            if (file.extension.ToLower().Contains("gsc")) continue; //skip gsc

            //string p = Path.Combine(options.path, $"{file.fileName}{file.extension}");
            string p = options.path;
            if (count > 0) continue;
            EditorUIManager.Instance.UpdateProgressBar(i / (float)files.Length, $"Exporting {Path.GetFileName(p)}");

            ExportFile(file, p);
            count++;

            yield return null;
        }

        EditorUIManager.Instance.CloseProgressBar();
        //EditorUIManager.Instance.Inform($"File(s) exported successfully! to path: {options.path}");
        EditorUIManager.Instance.Inform($"File exported successfully! to path: {options.path}");
    }

    public static void ExportFile(TTFileObject file, string p)
    {
        EditorUIManager.Instance.ShowProgressBar("Exporting File", $"Exporting file to {p}");
        try
        {
            switch (file.dataType)
            {
                case FileDataType.Text:
                    StringBuilder sb = new();
                    foreach (var prop in file.properties) sb.Append(prop.ToText());
                    File.WriteAllText(p, sb.ToString());
                    break;
                case FileDataType.Lines:
                    List<string> lines = new();
                    foreach (var prop in file.properties) lines.AddRange(prop.ToLines());
                    File.WriteAllLines(p, lines.ToArray());
                    break;
                default:
                    List<byte> bytes = new();
                    foreach (var prop in file.properties) bytes.AddRange(prop.ToBytes());
                    File.WriteAllBytes(p, bytes.ToArray());
                    break;
            }
        }
        catch (IOException ioe)
        {
            EditorUIManager.Instance.CloseProgressBar();
            EditorUIManager.Instance.Err($"IO Error exporting {file.fileName}{file.extension}: " + ioe);
        }
        EditorUIManager.Instance.CloseProgressBar();
    }

    public class ExportOptions
    {
        public string path;
    }
}
