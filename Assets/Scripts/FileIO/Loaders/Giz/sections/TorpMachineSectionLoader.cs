using UnityEngine;

public class TorpMachineSectionLoader : GizmoSectionLoader
{
    public override string Name => "Torp Machine Section";

    public override void Load(byte[] bytes, ref int index)
    {
        if (!TryLoad(bytes, ref index, "Torp Machine")) return;

        TorpMachineSection section = TTObjectManager.Create<TorpMachineSection>(Name);

        // ### Version ###
        int version = LoadBytes<int, IntLoader>(bytes, ref index);
        section.AddProperty(new IntegerProperty("Version", GetTargetVersion(version), IntegerProperty.IntType.Int, "") { generateOptions = TTProperty.FieldGenerateOptions.ReadonlyWName });

        // ### Torp Machine Count ###
        int count = LoadBytes<int, IntLoader>(bytes, ref index);
        IntegerProperty countProp = new("Pickup Count", count, IntegerProperty.IntType.Int, "") { generateOptions = TTProperty.FieldGenerateOptions.Hidden };
        section.AddProperty(countProp);

        // ### Scale ###
        float scale = 1;
        if (version >= 3) scale = LoadBytes<float, FloatLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 3))
        {
            // ## Scale ##
            section.AddProperty(new FloatProperty("Scale", scale, FloatProperty.FloatType.Float, "", (e) =>
            {
                foreach(var torp in section.FindProperty<ChildrenProperty>("Torp Machines").GetChildrenValues<TorpMachine>())
                {
                    torp.RefreshScale();
                }
            }, 1));
        }

        // ### Torp Machines ###
        var childrenProp = ChildrenProperty.Create<TorpMachine>("Torp Machines", "", "Torp Machine", new TorpMachineLoader(version), new byte[19], bytes, ref index, count, (e) =>
        {
            countProp.Value = (e.value as ChildProperty[]).Length;
        }, TTProperty.FieldGenerateOptions.HiddenWName);
        section.AddProperty(childrenProp);

        // # Torp Machine Menu Options #
        section.AddProperty(new EditOptionProperty("Gizmos/Create/New Torp Machine", () =>
        {
            (childrenProp.AddNewChild().Value as TTObject).GeneratePropertyPanel();
        }));

        _value = section;
    }
}
