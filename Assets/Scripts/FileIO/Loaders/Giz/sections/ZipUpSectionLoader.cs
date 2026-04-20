using System.Linq;
using UnityEngine;

public class ZipUpSectionLoader : GizmoSectionLoader
{
    public override string Name => "ZipUp Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "ZipUp")) return;

        ZipUpSection section = TTObjectManager.Create<ZipUpSection>(Name);

        // ### Version ###
        int version = LoadBytes<int,IntLoader>(bytes, ref index);
        section.AddProperty(new IntegerProperty("Version", GetTargetVersion(version), IntegerProperty.IntType.Int) { generateOptions = TTProperty.FieldGenerateOptions.ReadonlyWName });

        // ### ZipUp Count ###
        int count = LoadBytes<int, IntLoader>(bytes, ref index);
        IntegerProperty countProp = new("ZipUp Count", count, IntegerProperty.IntType.Int) { generateOptions = TTProperty.FieldGenerateOptions.Hidden };
        section.AddProperty(countProp);

        // ### ZipUps ###
        var childrenProp = ChildrenProperty.Create<ZipUp>("ZipUps", "", "ZipUp", new ZipUpLoader(version), new byte[62], bytes, ref index, count, (e) =>
        {
            countProp.Value = (e.value as ChildProperty[]).Length;
        }, TTProperty.FieldGenerateOptions.HiddenWName);
        section.AddProperty(childrenProp);

        // # ZipUp Menu Options #
        section.AddProperty(new EditOptionProperty("Gizmos/Create/New ZipUp", () =>
        {
            (childrenProp.AddNewChild().Value as TTObject).GeneratePropertyPanel();
        }));

        _value = section;
    }
}
