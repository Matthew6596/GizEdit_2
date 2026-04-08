using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class GizTurretLoader : PropertyLoader
{
    public override string Name => "GizTurret";

    private int version;

    public GizTurretLoader(int version)
    {
        this.version = version;
    }

    public override void Load(byte[] bytes, ref int index)
    {
        GizTurret turret = TTObjectManager.Create<GizTurret>(Name);

        // ### Name ###
        string name = Encoding.UTF8.GetString(bytes, index, 16);
        turret.AddProperty(new StringFixLenProperty("Name", name, 16, ""));
        index += 16;

        // ### Special Objects ###
        GizSpecialObjectsLoader objsLoader = new((o, ind) =>
        {
            // ### Unknown 1 ###
            short unk1 = 0;
            if (version >= 3)
            {
                unk1 = BitConverter.ToInt16(bytes, ind);
                ind += 2;
            }
            if (ShouldAddProperty(version, v => v >= 3))
            {
                // ## Unknown 1 ##
                o.AddProperty(new IntegerProperty("Unknown (1)", unk1, IntegerProperty.IntType.Short));
            }
            return ind;
        });
        objsLoader.Load(bytes, ref index);
        var specialObjects = objsLoader.GetValue<GizSpecialObjects>();
        turret.AddProperty(new ChildProperty("Special Objects", specialObjects, "", (e) => { }, objsLoader.LoadDefault()) { generateOptions = TTProperty.FieldGenerateOptions.Hidden });

        // # Special Objects Editor Nav Buttons #
        specialObjects.PrependProperty(new NavBtnProperty($"<-- Back to GizTurret", () => { turret.GeneratePropertyPanel(); }));
        turret.AddProperty(new NavBtnProperty("Special Objects -->", () => { specialObjects.GeneratePropertyPanel(); }));

        // ### Unknown 2 ###
        PositionProperty posProp = new("Position?", LoadBytes<Vector3, Vector3Loader>(bytes, ref index), turret.transform);
        turret.AddProperty(posProp);

        // ### Unknown 3 ###
        turret.AddProperty(new Vector3Property("Unknown 3", LoadBytes<Vector3, Vector3Loader>(bytes, ref index), "..."));

        // ### Unknown 4 ###
        turret.AddProperty(new Vector3Property("Unknown 4", LoadBytes<Vector3, Vector3Loader>(bytes, ref index), "..."));

        // ### Unknown 5 ###
        turret.AddProperty(new Vector3Property("Unknown 5", LoadBytes<Vector3, Vector3Loader>(bytes, ref index), "..."));

        // ### Unknown 6 ###
        turret.AddProperty(new IntegerProperty("Unknown 6", LoadBytes<int, IntLoader>(bytes, ref index), IntegerProperty.IntType.Int, "..."));

        // ### Unknown 7 ###
        turret.AddProperty(new IntegerProperty("Unknown 7", LoadBytes<int, IntLoader>(bytes, ref index), IntegerProperty.IntType.Int, "..."));

        // ### Unknown 8 ###
        turret.AddProperty(new IntegerProperty("Unknown 8", LoadBytes<int, IntLoader>(bytes, ref index), IntegerProperty.IntType.Int, "..."));

        // ### Unknown 9 ###
        turret.AddProperty(new IntegerProperty("Unknown 9", LoadBytes<int, IntLoader>(bytes, ref index), IntegerProperty.IntType.Int, "..."));

        // ### Unknown 10 ###
        turret.AddProperty(new IntegerProperty("Unknown 10", LoadBytes<int, IntLoader>(bytes, ref index), IntegerProperty.IntType.Int, "..."));

        // ### Unknown 11 ###
        turret.AddProperty(new IntegerProperty("Unknown 11", LoadBytes<int, IntLoader>(bytes, ref index), IntegerProperty.IntType.Int, "..."));

        // ### Unknown 12 ###
        int unk12 = 0;
        if (version >= 2) unk12 = LoadBytes<int, IntLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 2)) turret.AddProperty(new IntegerProperty("Unknown 6", unk12, IntegerProperty.IntType.Int, "..."));

        // ### Unknown Count ###
        IntegerProperty unkCountProp = new("Unknown Count", bytes[index], IntegerProperty.IntType.Byte) { generateOptions = TTProperty.FieldGenerateOptions.Hidden };
        index++;
        turret.AddProperty(unkCountProp);

        // ### Unknown 13 ###
        var linkObjectsProp = ArrayProperty<Vector3Property>.Create("Unknown 13", "", "Unk13", new Vector3Loader(), Vector3.zero, bytes, ref index, unkCountProp, (info) =>
        {
            return new Vector3Property($"{info.name}", (Vector3)info.defaultValue, "...");
        }, (e) => { }, TTProperty.FieldGenerateOptions.None); ;
        turret.AddProperty(linkObjectsProp);

        // ### Unknown 14 ###
        turret.AddProperty(new FloatProperty("Unknown 14", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

        // ### Unknown 15 ###
        turret.AddProperty(new FloatProperty("Unknown 15", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

        // ### Unknown 16 ###
        turret.AddProperty(new FloatProperty("Unknown 16", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "This is always 0 in Vanilla TCS."));

        // ### Unknown 17 ###
        turret.AddProperty(new FloatProperty("Unknown 17", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

        // ### Unknown 18 ###
        turret.AddProperty(new FloatProperty("Unknown 18", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

        // ### Unknown 19 ###
        turret.AddProperty(new FloatProperty("Unknown 19", LoadBytes<float, FloatLoader>(bytes, ref index), FloatProperty.FloatType.Float, "..."));

        // ### Minimum Studs Value? ###
        turret.AddProperty(new IntegerProperty("Minimum Studs Value?", (ushort)LoadBytes<short, ShortLoader>(bytes, ref index), IntegerProperty.IntType.UShort, "..."));

        // ### Maximum Studs Value? ###
        turret.AddProperty(new IntegerProperty("Maximum Studs Value?", (ushort)LoadBytes<short, ShortLoader>(bytes, ref index), IntegerProperty.IntType.UShort, "..."));

        // ### Studs Angle? ###
        GameObject studsSpawnObjTEMP = new("studs_spawn_obj_TEMP");
        studsSpawnObjTEMP.transform.SetParent(turret.transform);
        studsSpawnObjTEMP.transform.localPosition = Vector3.zero;
        turret.AddProperty(new AngleProperty("Studs Angle?", (ushort)LoadBytes<short, ShortLoader>(bytes, ref index), studsSpawnObjTEMP.transform, "The angle at which studs emit."));

        // ### Studs Position? ###
        turret.AddProperty(new PositionProperty("Studs Position?", LoadBytes<Vector3, Vector3Loader>(bytes, ref index), studsSpawnObjTEMP.transform, "The relative position at which studs emit.") { isSecondaryPosGiz = true, primaryPosProperty = posProp });

        // ### Studs Speed? ###
        float studSpd = 1.5f;
        if (version >= 6) studSpd = LoadBytes<float, FloatLoader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 6))
        {
            // ## Studs Speed? ##
            turret.AddProperty(new FloatProperty("Studs Speed?", studSpd, FloatProperty.FloatType.Float, "The speed of the studs as they emit.", (e) => { }, 1.5f));
        }

        // ### Unknown 20 ###
        turret.AddProperty(new IntegerProperty("Unknown 20", bytes[index], IntegerProperty.IntType.Byte, "..."));
        index++;

        // ### Unknown 21 ###
        // ### Unknown 22 ###
        byte unk21 = 0;
        short unk22 = 0;
        if (version >= 4)
        {
            unk21 = bytes[index];
            index++;
            unk22 = LoadBytes<short, ShortLoader>(bytes, ref index);
        }
        if (ShouldAddProperty(version, v => v >= 4))
        {
            // ## Unknown 21 ##
            turret.AddProperty(new IntegerProperty("Unknown 21", unk21, IntegerProperty.IntType.Byte, "..."));

            // ## Unknown 22 ##
            turret.AddProperty(new IntegerProperty("Unknown 22", unk22, IntegerProperty.IntType.Short, "..."));
        }

        // ### Blaster Material? ###
        turret.AddProperty(new StringProperty("Blaster Material?", LoadBytes<string, String8Loader>(bytes, ref index), StringProperty.MaxSize.Byte, "..."));

        // ### Part? ###
        turret.AddProperty(new StringProperty("Part?", LoadBytes<string, String8Loader>(bytes, ref index), StringProperty.MaxSize.Byte, "..."));

        // ### Part? ###
        turret.AddProperty(new StringProperty("Part?", LoadBytes<string, String8Loader>(bytes, ref index), StringProperty.MaxSize.Byte, "..."));

        // ### Part? ###
        string part3 = "";
        if (version >= 7) part3 = LoadBytes<string, String8Loader>(bytes, ref index);
        if (ShouldAddProperty(version, v => v >= 7)) turret.AddProperty(new StringProperty("Part?", part3, StringProperty.MaxSize.Byte, "..."));

        // ### Blowup? ###
        turret.AddProperty(new StringProperty("Blowup?", LoadBytes<string, String8Loader>(bytes, ref index), StringProperty.MaxSize.Byte, "..."));

        // ### Unknown 23 ###
        turret.AddProperty(new IntegerProperty("Unknown 23", LoadBytes<short, ShortLoader>(bytes, ref index), IntegerProperty.IntType.Short, "..."));

        _value = turret;
    }
}
