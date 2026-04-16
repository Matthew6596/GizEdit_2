using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

public class IntBitFlagsProperty : IntegerProperty
{
    private readonly BoolProperty[] bitProps;

    public IntBitFlagsProperty(string name, int value, string[] bitOptions, string info = "", UnityAction<ChangeEventData> onValueChange = null, int defaultValue = 0) : base(name, value, IntType.Int, info, onValueChange, defaultValue)
    {
        if (bitOptions == null || bitOptions.Length != 32) Debug.LogError("Bit Options in IntBitFlags Property needs to have length of 32.");

        //Create bit props
        bitProps = new BoolProperty[32];
        int mask = 0b00000000000000000000000000000001;
        for (int i = 0; i < 32; i++)
        {
            int lineInd = bitOptions[i].IndexOf('|');

            string optionName = lineInd == -1 ? bitOptions[i] : bitOptions[i][..lineInd];
            string optionInfo = lineInd == -1 ? "" : bitOptions[i][lineInd..];

            int boolMask = mask;
            bitProps[i] = new(optionName, (value & mask) != 0, optionInfo, (e) =>
            {
                int val = Value.Convert<int>();
                if ((bool)e.value) Value = val | boolMask;
                else Value = val & (0b11111111111111111111111111111111 - boolMask);
            }) { generateOptions = generateOptions };

            mask <<= 1;
        }
    }

    public override void GenerateField(Transform parent)
    {
        for (int i = 0; i < 32; i++)
        {
            if (bitProps[i].name == "") continue;
            bitProps[i].GenerateField(parent);
        }
    }

    public override void RefreshValueDisplays(object value)
    {
        int val = value.Convert<int>();
        int mask = 0b00000000000000000000000000000001;
        for (int i = 0; i < 32; i++)
        {
            if (bitProps[i].name == "")
            {
                mask <<= 1;
                continue;
            }
            bitProps[i].RefreshValueDisplays((val & mask) != 0);

            mask <<= 1;
        }
    }

    public void SetFlag(int index, bool value) => bitProps[index].Value = value;

    public void SetAllFlags(params int[] indicies)
    {
        for (int i = 0; i < 32; i++) bitProps[i].Value = indicies.Contains(i);
    }
}
