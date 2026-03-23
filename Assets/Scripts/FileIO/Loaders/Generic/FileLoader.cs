using UnityEngine;
using System.IO;

public class FileLoader : PropertyLoader
{
    public override string Name => "File Object";

    public override void Load(byte[] bytes, ref int index) => Load();

    public override void Load(string text, ref int index) => Load();

    public override void Load(string[] lines, ref int index) => Load();

    private void Load()
    {
        var obj = TTObjectManager.Create<TTFileObject>("FileObject");
        obj.fileName = Path.GetFileNameWithoutExtension(TTLoader.CurrentLoadingFilePath);
        obj.extension = Path.GetExtension(TTLoader.CurrentLoadingFilePath);
        obj.dataType = TTLoader.CurrentLoadingFileType;
        obj.ogpath = TTLoader.CurrentLoadingFilePath;
        _value = obj;
        EditorUIManager.Instance.AddObjectToHierarchy(Name, 0, () => { obj.GeneratePropertyPanel(); });
    }
}
