using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TTExporter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Export()
    {
        ExportOptions options = new();
        //var options = EditorUIManager.Instance.
        StartCoroutine(Export(options));
    }

    public IEnumerator Export(ExportOptions options)
    {
        yield return null;
    }

    public class ExportOptions
    {

    }
}
