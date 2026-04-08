using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BombGeneratorLoader : PropertyLoader
{
    public override string Name => "BombGenerator";

    private int version;

    public BombGeneratorLoader(int version)
    {
        this.version = version;
    }

    public override void Load(byte[] bytes, ref int index)
    {
        BombGenerator bombGen = TTObjectManager.Create<BombGenerator>(Name);

        // ### Name ###
        string name = Encoding.UTF8.GetString(bytes, index, 16);
        bombGen.AddProperty(new StringFixLenProperty("Name", name, 16, ""));
        index += 16;

        // ### Position ###
        bombGen.AddProperty(new PositionProperty("Position", LoadBytes<Vector3, Vector3Loader>(bytes, ref index), bombGen.transform));

        // ### Unknown 1 ###
        bombGen.AddProperty(new IntegerProperty("Unknown 1", LoadBytes<int, IntLoader>(bytes, ref index), IntegerProperty.IntType.Int, ""));

        // ### Special Objects ###
        GizSpecialObjectsLoader objsLoader = new();
        objsLoader.Load(bytes, ref index);
        var specialObjs = objsLoader.GetValue<GizSpecialObjects>();
        bombGen.AddProperty(new ChildProperty("Special Objects", specialObjs, "", (e) => { }, objsLoader.LoadDefault()) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

        // # Special Objects Editor Nav Buttons #
        specialObjs.PrependProperty(new NavBtnProperty($"<-- Back to BombGenerator", () => { bombGen.GeneratePropertyPanel(); }));
        bombGen.AddProperty(new NavBtnProperty("Special Objects -->", () => { specialObjs.GeneratePropertyPanel(); }));

        _value = bombGen;
    }
}
