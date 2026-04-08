using System;
using System.Collections.Generic;
using UnityEngine;

public class MiniCutPartsLoader : PropertyLoader
{
    public override string Name => "MiniCut Parts";

    public override void Load(byte[] bytes, ref int index)
    {
        MiniCutParts parts = TTObjectManager.Create<MiniCutParts>(Name);

        // ### MiniCut Parts Count ###
        byte count = bytes[index];
        index++;
        IntegerProperty countProp = new("MiniCut Part Count", count, IntegerProperty.IntType.Byte) { generateOptions = TTProperty.FieldGenerateOptions.Hidden };
        parts.AddProperty(countProp);

        // ### MiniCut Parts ###
        var childrenProp = ChildrenProperty.Create<MiniCutPart>("MiniCut Parts", "", "MiniCut Part", new MiniCutPartLoader(), new byte[31], bytes, ref index, count, (e) =>
        {
            countProp.Value = (e.value as ChildProperty[]).Length;
        });
        parts.AddProperty(childrenProp);

        _value = parts;
    }

    public MiniCutParts LoadDefault()
    {
        int tempInd = 0;
        Load(new byte[1], ref tempInd);
        return GetValue<MiniCutParts>();
    }
}
