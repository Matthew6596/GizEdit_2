using System.Collections;
using System.IO;
using UnityEngine;

public class PropertyLoaderFactory
{
    public static PropertyLoader Get(string name) => (name) switch
    {
        "File Object" => new FileLoader(),
        "Gizmos" => new GizFileLoader(),

        _ => new NullPropertyLoader()
    };

    public static void Load(string loaderName, FileDataType type, object fileContents, ref int index)
    {
        //Get the PropertyLoader by name
        var loader = Get(loaderName);

        //Load the property
        switch (type)
        {
            case FileDataType.Text: loader.Load(fileContents as string, ref index); break;
            case FileDataType.Lines: loader.Load(fileContents as string[], ref index); break;
            case FileDataType.Bytes: loader.Load(fileContents as byte[], ref index); break;
        }
    }
}
