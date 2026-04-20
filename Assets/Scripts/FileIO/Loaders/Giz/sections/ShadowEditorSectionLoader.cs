using UnityEngine;

public class ShadowEditorSectionLoader : GizmoSectionLoader
{
    public override string Name => "ShadowEditor Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "ShadowEditor")) return;

        ShadowEditorSection section = TTObjectManager.Create<ShadowEditorSection>(Name);

        // ### Version ###
        byte version = bytes[index];
        index++;
        section.AddProperty(new IntegerProperty("Version", GetTargetVersion(version), IntegerProperty.IntType.Byte) { generateOptions = TTProperty.FieldGenerateOptions.ReadonlyWName });

        // ### ShadowEdit Count ###
        byte count = bytes[index];
        index++;
        IntegerProperty countProp = new("ShadowEdit Count", count, IntegerProperty.IntType.Byte) { generateOptions = TTProperty.FieldGenerateOptions.Hidden };
        section.AddProperty(countProp);

        // ### Shadow Edits ###
        ChildrenProperty childrenProp = ChildrenProperty.Create<ShadowEdit>("Shadow Edits", "", "Shadow Edit", new ShadowEditLoader(version), new byte[64], bytes, ref index, count, (e) =>
        {
            countProp.Value = (e.value as ChildProperty[]).Length;
        }, TTProperty.FieldGenerateOptions.HiddenWName);
        section.AddProperty(childrenProp);

        // # ShadowEditor Menu Options #
        section.AddProperty(new EditOptionProperty("Gizmos/Create/New ShadowEditor", () =>
        {
            (childrenProp.AddNewChild().Value as TTObject).GeneratePropertyPanel();
        }));

        _value = section;
    }
}
