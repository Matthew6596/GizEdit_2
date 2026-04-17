using SFB;
using System;
using System.Collections;
using UnityEngine;

public class GizFileLoader : FileLoader
{
    public override string Name => "Gizmos";

    public override void Load(byte[] bytes, ref int index)
    {
        base.Load(bytes, ref index);

        var obj = (TTFileObject)_value;
        obj.name = System.IO.Path.GetFileName(TTLoader.CurrentLoadingFilePath);

        int magic = BitConverter.ToInt32(bytes, 0);
        if (magic == 0) return;
        index += 4;

        obj.AddProperty(new IntegerProperty("magic", magic, IntegerProperty.IntType.Int, "") { generateOptions = TTProperty.FieldGenerateOptions.Hidden});

        //Load subroutine so each section can update progress bar
        //Index doesn't need ref since this reads entire file
        TTLoader.StartLoadSubroutine(LoadSubroutine(obj, bytes, index));

    }

    private IEnumerator LoadSubroutine(TTFileObject obj, byte[] bytes, int index)
    {
        const int progressMax = 17 + 1;

        yield return TTLoader.UpdateLoadProgress(0, progressMax, "Loading GizObstacle, GizBuildit, and GizForce Sections...");
        obj.AddProperty(new ChildProperty("GizObstacle Section", LoadBytes<GizObstacleSection, GizObstacleSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("GizBuildit Section", LoadBytes<GizBuilditSection, GizBuilditSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("GizForce Section", LoadBytes<GizForceSection, GizForceSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

        yield return TTLoader.UpdateLoadProgress(3, progressMax, "Loading blowup and GizmoPickup Sections...");
        obj.AddProperty(new ChildProperty("blowup Section", LoadBytes<blowupSection, blowupSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("GizmoPickup Section", LoadBytes<GizmoPickupSection, GizmoPickupSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

        yield return TTLoader.UpdateLoadProgress(5, progressMax, "Loading Lever, Spinner, Minicut, and Tube Sections...");
        obj.AddProperty(new ChildProperty("Lever Section", LoadBytes<LeverSection, LeverSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("Spinner Section", LoadBytes<SpinnerSection, SpinnerSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("MiniCut Section", LoadBytes<MiniCutSection, MiniCutSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("Tube Section", LoadBytes<TubeSection, TubeSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

        yield return TTLoader.UpdateLoadProgress(9, progressMax, "Loading ZipUp, GizTurret, BombGenerator, and Panel Sections...");
        obj.AddProperty(new ChildProperty("ZipUp Section", LoadBytes<ZipUpSection, ZipUpSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("GizTurret Section", LoadBytes<GizTurretSection, GizTurretSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("BombGenerator Section", LoadBytes<BombGeneratorSection, BombGeneratorSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("Panel Section", LoadBytes<PanelSection, PanelSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

        yield return TTLoader.UpdateLoadProgress(13, progressMax, "Loading HatMachine, PushBlocks, Torp Machine, and ShadowEditor Sections...");
        obj.AddProperty(new ChildProperty("HatMachine Section", LoadBytes<HatMachineSection, HatMachineSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("PushBlocks Section", LoadBytes<PushBlocksSection, PushBlocksSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("Torp Machine Section", LoadBytes<TorpMachineSection, TorpMachineSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("ShadowEditor Section", LoadBytes<ShadowEditorSection, ShadowEditorSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

        yield return TTLoader.UpdateLoadProgress(17, progressMax, "Done loading gizmos!");

        /*obj.AddProperty(new ChildProperty("GizObstacle Section", LoadBytes<GizObstacleSection, GizObstacleSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("GizObstacle Section", LoadBytes<GizObstacleSection, GizObstacleSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });
        obj.AddProperty(new ChildProperty("GizObstacle Section", LoadBytes<GizObstacleSection, GizObstacleSectionLoader>(bytes, ref index)) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });*/

        obj.AddProperty(new IntegerProperty("End Buffer", 0, IntegerProperty.IntType.Int, "") { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

        // # Editor Properties #
        obj.AddProperty(new NavBtnProperty("Export", () =>
        {
            string p = StandaloneFileBrowser.SaveFilePanel("Export GIZ File", Settings.Get("tcs_path"), "gizmos.giz", "giz");
            if (p == "") return;
            TTExporter.ExportFile(obj, p);
        }));

        obj.AddProperty(new NavBtnProperty("Unload", () =>
        {
            obj.Destroy();
            EditorUIManager.Instance.RemoveHierarchyRoot(obj);
            EditorUIManager.Instance.ClearPropertyPanel();
        }));

        EditorUIManager.Instance.AddHierarchyRoot(obj);

        yield return TTLoader.UpdateLoadProgress(18, progressMax, "Hierarchy refreshing...");

        TTLoader.EndLoadSubroutine();
    }
}
