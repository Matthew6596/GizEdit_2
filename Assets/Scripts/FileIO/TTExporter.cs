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

        //var options = EditorUIManager.Instance.
        StartCoroutine(Export(options));
    }

    public IEnumerator Export(ExportOptions options)
    {
        EditorUIManager.Instance.ShowProgressBar("Exporting Files", "Finding all files to export...");

        var files = FindObjectsByType<TTFileObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        for(int i=0; i<files.Length; i++)
        {
            var file = files[i];
            string p = Path.Combine(options.path, $"{file.fileName}{file.extension}");
            EditorUIManager.Instance.UpdateProgressBar(i / (float)files.Length, $"Exporting {file.fileName}{file.extension}");

            ExportFile(file, p);

            yield return null;
        }

        EditorUIManager.Instance.CloseProgressBar();
        EditorUIManager.Instance.Inform($"Files exported successfully! to path: {options.path}");
    }

    public static void ExportFile(TTFileObject file, string p)
    {
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
            EditorUIManager.Instance.Err($"IO Error exporting {file.fileName}{file.extension}: " + ioe);
        }
    }

    public class ExportOptions
    {
        public string path;
    }
}
