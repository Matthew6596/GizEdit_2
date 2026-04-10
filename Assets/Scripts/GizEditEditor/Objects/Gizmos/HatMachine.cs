using System.Collections.Generic;
using UnityEngine;

public class HatMachine : TTObject
{
    private GameObject _activationTarget;
    public GameObject ActivationTarget
    {
        get
        {
            if (_activationTarget == null)
            {
                _activationTarget = new GameObject("hatmachine_activation_target");
                _activationTarget.transform.SetParent(transform);
                _activationTarget.transform.localPosition = Vector3.zero;
            }
            return _activationTarget;
        }
    }

    public static readonly Dictionary<string, byte> HatTypes = new()
    {
        { "Random", 0 },
        { "Leia", 1 },
        { "Fedora", 2 },
        { "Top Hat", 3 },
        { "Baseball Cap", 4 },
        { "Stormtrooper", 5 },
        { "Bounty Hunter", 6 },
        { "Droid Panel", 7 },
    };

    public void UpdateHandleColor(int colType)
    {

    }

    public void UpdateHatType(int hatType)
    {

    }
}
