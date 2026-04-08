using UnityEngine;

public class PanelSectionLoader : GizmoSectionLoader
{
    public override string Name => "Panel Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "Panel")) return;

        PanelSection section = TTObjectManager.Create<PanelSection>(Name);
        
        // ### Version ###
        int version = LoadBytes<int, IntLoader>(bytes, ref index);
        section.AddProperty(new IntegerProperty("Version", version, IntegerProperty.IntType.Int, "") { generateOptions = TTProperty.FieldGenerateOptions.ReadonlyWName });

        // ### Panel Count ###
        int count = LoadBytes<int, IntLoader>(bytes, ref index);
        IntegerProperty countProp = new("Panel Count", count, IntegerProperty.IntType.Int, "") { generateOptions = TTProperty.FieldGenerateOptions.Hidden };
        section.AddProperty(countProp);

        // ### Panels ###
        ChildrenProperty childrenProp = ChildrenProperty.Create<Panel>("Panels", "", "Panel", new PanelLoader(version), new byte[41], bytes, ref index, count, (e) =>
        {
            countProp.Value = (e.value as ChildProperty[]).Length;
        }, TTProperty.FieldGenerateOptions.HiddenWName);
        section.AddProperty(childrenProp);

        // # Panel Menu Options #
        EditorUIManager.Instance.AddMenuOption("Gizmos/Create/New Panel", () =>
        {
            (childrenProp.AddNewChild().Value as TTObject).GeneratePropertyPanel();
        });

        _value = section;
    }
}
