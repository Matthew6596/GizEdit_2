using System;
using UnityEngine;

public class GizFileLoader : FileLoader
{
    public override string Name => "Gizmos";

    public override void Load(byte[] bytes, ref int index)
    {
        base.Load(bytes, ref index);

        var obj = (TTFileObject)_value;

        int magic = BitConverter.ToInt32(bytes, 0);
        if (magic == 0) return;
        index += 4;

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
    }
}
