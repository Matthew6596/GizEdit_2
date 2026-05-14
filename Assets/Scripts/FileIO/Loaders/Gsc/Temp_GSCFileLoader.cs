using System;
using UnityEngine;

public class Temp_GSCFileLoader : FileLoader
{
    public override string Name => "GSC File";

    public static GameObject parentObj;
    public static Transform parentTransform;

    public override void Load(byte[] bytes, ref int index)
    {
        //load gsc
        parentObj = new("GSC File Object");
        parentTransform = parentObj.transform;
        var gscFile = parentObj.AddComponent<Temp_GSCFile>();
        GSCScene scene = new();
        SceneLoader.LoadGSC(bytes, scene);
    }
}
