using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;

public class GizmoSectionLoader : PropertyLoader
{
    public override string Name => "Gizmo Section";

    public static string CurrentLoadingSection;

    public override void Load(byte[] bytes, ref int index)
    {
        index += (int)_value;
    }

    public bool TryLoad(byte[] bytes, ref int index, string sectionName)
    {
        int startIndex = index;

        //Attempt to load section data
        string title = LoadBytes<string, String32Loader>(bytes, ref index);

        //Section locating / handling
        if (title != sectionName)
        {
            /*Warn($"{sectionName} couldn't be loaded at {startIndex}. Locating proper position now...");

            string searchStr = sectionName;
            List<byte> searchBytes = new();
            searchBytes.AddRange(BitConverter.GetBytes(searchStr.Length));
            searchBytes.AddRange(Encoding.UTF8.GetBytes(searchStr));

            //Search for section anywhere in bytes
            for (int i = 0; i < bytes.Length - searchBytes.Count; i++)
            {
                bool found = true;
                for (int j = 0; j < searchBytes.Count; j++)
                {
                    if (bytes[i + j] != searchBytes[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found && BitConverter.ToInt32(bytes,i-4) == searchStr.Length)
                {
                    index = i+searchStr.Length;
                    _value = BitConverter.ToInt32(bytes,index);
                    index += 4;
                    return true;
                }
            }*/

            Err($"Couldn't locate {sectionName}. i:{index}.");
            index = startIndex;
            return false;
        }

        CurrentLoadingSection = title;
        if (TTLoader.LogEnabled) Debug.Log($"Loading {CurrentLoadingSection} GizSection...");
        _value = BitConverter.ToInt32(bytes, index);
        index += 4;
        return true;
    }
}
