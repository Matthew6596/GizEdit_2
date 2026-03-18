using UnityEngine;

public class NullPropertyLoader : PropertyLoader
{
    public override string Name => "null";
    public override void Load(string bytes, ref int index)
    {
        Warn("Null property attempted to load: "+index);
    }
    public override void Load(byte[] bytes, ref int index)
    {
        Warn("Null property attempted to load: " + index);
    }
    public override void Load(string[] lines, ref int index)
    {
        Warn("Null property attempted to load: " + index);
    }
}
