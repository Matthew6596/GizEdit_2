using UnityEngine;

public class MiniCutSectionLoader : GizmoSectionLoader
{
    public override string Name => "MiniCut Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "MiniCut")) return;

        MiniCutSection section = TTObjectManager.Create<MiniCutSection>(Name);

        // ### Version ###
        int version = LoadBytes<int, IntLoader>(bytes, ref index);
        section.AddProperty(new IntegerProperty("Version", GetTargetVersion(version), IntegerProperty.IntType.Int, "In Vanilla TCS, this is always 1 and changing it does nothing.") { generateOptions = TTProperty.FieldGenerateOptions.ReadonlyWName });

        // ### MiniCut Count ###
        int count = LoadBytes<int, IntLoader>(bytes, ref index);
        IntegerProperty countProp = new("MiniCut Count", count, IntegerProperty.IntType.Int, "") { generateOptions = TTProperty.FieldGenerateOptions.Hidden };
        section.AddProperty(countProp);

        // ### MiniCuts ###
        ChildrenProperty childrenProp = ChildrenProperty.Create<MiniCut>("MiniCuts", "", "MiniCut", new MiniCutLoader(), new byte[22], bytes, ref index, count, (e) =>
        {
            countProp.Value = (e.value as ChildProperty[]).Length;
        }, TTProperty.FieldGenerateOptions.HiddenWName);
        section.AddProperty(childrenProp);

        // # Spinner Menu Options #
        EditorUIManager.Instance.AddMenuOption("Gizmos/Create/New MiniCut", () =>
        {
            (childrenProp.AddNewChild().Value as TTObject).GeneratePropertyPanel();
        });

        _value = section;
    }
}
