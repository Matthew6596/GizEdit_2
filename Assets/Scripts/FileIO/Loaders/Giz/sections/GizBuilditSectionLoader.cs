using UnityEngine;

public class GizBuilditSectionLoader : GizmoSectionLoader
{
    public override string Name => "GizBuildit Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "GizBuildit")) return;

        GizBuilditSection section = TTObjectManager.Create<GizBuilditSection>(Name);

        // ### Version ###
        byte version = bytes[index];
        index++;

        section.AddProperty(new IntegerProperty("Version", GetTargetVersion(version), IntegerProperty.IntType.Byte) { generateOptions = TTProperty.FieldGenerateOptions.ReadonlyWName });

        // ### Buildit Count ###
        short count = LoadBytes<short, ShortLoader>(bytes, ref index);
        IntegerProperty countProp = new("Buildit Count", count, IntegerProperty.IntType.Short) { generateOptions = TTProperty.FieldGenerateOptions.Hidden };
        section.AddProperty(countProp);

        // ### Buildits ###
        byte[] defaultBytes = new byte[76];
        defaultBytes[28] = 3;
        var childrenProp = ChildrenProperty.Create<GizBuildit>("Buildits", "", "Buildit", new GizBuilditLoader(version), defaultBytes, bytes, ref index, count, (e) =>
        {
            countProp.Value = (e.value as ChildProperty[]).Length;
        }, TTProperty.FieldGenerateOptions.HiddenWName);
        section.AddProperty(childrenProp);

        // # Buildit Menu Options #
        EditorUIManager.Instance.AddMenuOption("Gizmos/Create/New Buildit", () =>
        {
            (childrenProp.AddNewChild().Value as TTObject).GeneratePropertyPanel();
        });

        _value = section;
    }
}
