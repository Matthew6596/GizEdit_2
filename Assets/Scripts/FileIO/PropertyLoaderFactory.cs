using System.IO;
using UnityEngine;

public class PropertyLoaderFactory
{
    public static PropertyLoader Get(string name) => (name) switch
    {
        "GizObstacle Section" => new GizObstacleSectionLoader(),
        "GizBuildit Section" => new GizBuilditSectionLoader(),
        "GizForce Section" => new GizForceSectionLoader(),
        "blowup Section" => new blowupSectionLoader(),
        "GizmoPickup Section" => new GizmoPickupSectionLoader(),
        "Lever Section" => new LeverSectionLoader(),
        "Spinner Section" => new SpinnerSectionLoader(),
        "MiniCut Section" => new MiniCutSectionLoader(),
        "Tube Section" => new TubeSectionLoader(),
        "ZipUp Section" => new ZipUpSectionLoader(),
        "GizTurret Section" => new GizTurretSectionLoader(),
        "BombGenerator Section" => new BombGeneratorSectionLoader(),
        "Panel Section" => new PanelSectionLoader(),
        "HatMachine Section" => new HatMachineSectionLoader(),
        "PushBlocks Section" => new PushBlocksSectionLoader(),
        "Torp Machine Section" => new TorpMachineSectionLoader(),
        "ShadowEditor Section" => new ShadowEditorSectionLoader(),

        "File Object" => new FileLoader(),
        "Gizmos" => new GizFileLoader(),

        _ => new NullPropertyLoader()
    };

    public static void Load(string loaderName, FileDataType type, object fileContents, ref int index)
    {
        //Get the PropertyLoader by name
        var loader = Get(loaderName);

        //Load the property
        switch (type)
        {
            case FileDataType.Text: loader.Load(fileContents as string, ref index); break;
            case FileDataType.Lines: loader.Load(fileContents as string[], ref index); break;
            case FileDataType.Bytes: loader.Load(fileContents as byte[], ref index); break;
        }
    }
}
