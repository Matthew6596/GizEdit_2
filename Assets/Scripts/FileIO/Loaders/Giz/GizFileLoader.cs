using System;
using UnityEngine;

public class GizFileLoader : FileLoader
{
    public override string Name => "Gizmos";

    public override void Load(byte[] bytes, ref int index)
    {
        base.Load(bytes, ref index);

        var obj = (TTFileObject)_value;
        obj.name = Name;

        int magic = BitConverter.ToInt32(bytes, 0);
        if (magic == 0) return;
        index += 4;

        obj.AddProperty(new IntegerProperty("magic", magic, IntegerProperty.IntType.Int, "") { generateOptions = TTProperty.FieldGenerateOptions.Hidden});

        obj.AddProperty(new ChildProperty("GizObstacle Section", LoadBytes<GizObstacleSection, GizObstacleSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("GizBuildit Section", LoadBytes<GizBuilditSection, GizBuilditSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("GizForce Section", LoadBytes<GizForceSection, GizForceSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("blowup Section", LoadBytes<blowupSection, blowupSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("GizmoPickup Section", LoadBytes<GizmoPickupSection, GizmoPickupSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("Lever Section", LoadBytes<LeverSection, LeverSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("Spinner Section", LoadBytes<SpinnerSection, SpinnerSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("MiniCut Section", LoadBytes<MiniCutSection, MiniCutSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("Tube Section", LoadBytes<TubeSection, TubeSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("ZipUp Section", LoadBytes<ZipUpSection, ZipUpSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("GizTurret Section", LoadBytes<GizTurretSection, GizTurretSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("BombGenerator Section", LoadBytes<BombGeneratorSection, BombGeneratorSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("Panel Section", LoadBytes<PanelSection, PanelSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("HatMachine Section", LoadBytes<HatMachineSection, HatMachineSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("PushBlocks Section", LoadBytes<PushBlocksSection, PushBlocksSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("Torp Machine Section", LoadBytes<TorpMachineSection, TorpMachineSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("ShadowEditor Section", LoadBytes<ShadowEditorSection, ShadowEditorSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

        /*obj.AddProperty(new ChildProperty("GizObstacle Section", LoadBytes<GizObstacleSection, GizObstacleSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("GizObstacle Section", LoadBytes<GizObstacleSection, GizObstacleSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("GizObstacle Section", LoadBytes<GizObstacleSection, GizObstacleSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });*/

        obj.AddProperty(new IntegerProperty("End Buffer", 0, IntegerProperty.IntType.Int, "") { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

        // How to generate GIZ in hierarchy
        EditorUIManager.Instance.GenerateHierarchyFromRoot(obj, new string[]
        {
            $"{obj.properties[1].name}/Obstacles/Special Objects/Special Objects",
            $"{obj.properties[2].name}/Buildits/Special Objects/Special Objects",
            $"{obj.properties[3].name}/Forces/Special Objects/Special Objects",
            $"{obj.properties[4].name}/Blowup Objects",
            $"../Blowups",
            $"{obj.properties[5].name}/Pickups",
            $"{obj.properties[6].name}/Levers",
            $"{obj.properties[7].name}/Spinners/Special Objects/Special Objects",
            $"{obj.properties[8].name}/MiniCuts/MiniCut Parts/MiniCut Parts",
            $"{obj.properties[9].name}/Tubes",
            $"{obj.properties[10].name}/ZipUps",
            $"{obj.properties[11].name}/Turrets/Special Objects/Special Objects",
            $"{obj.properties[12].name}/Bomb Generators/Special Objects/Special Objects",
            $"{obj.properties[13].name}/Panels",
            $"{obj.properties[14].name}/Hat Machines",
            $"{obj.properties[15].name}/Push Blocks",
            $"{obj.properties[16].name}/Torp Machines",
            $"{obj.properties[17].name}/Shadow Edits",
        });
    }
}
