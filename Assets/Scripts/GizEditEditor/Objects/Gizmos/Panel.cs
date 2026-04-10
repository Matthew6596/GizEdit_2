using System.Collections.Generic;
using UnityEngine;

public class Panel : TTObject
{
    private GameObject _activationTarget;
    public GameObject ActivationTarget
    {
        get
        {
            if (_activationTarget == null)
            {
                _activationTarget = new GameObject("panel_activation_target");
                _activationTarget.transform.SetParent(transform);
                _activationTarget.transform.localPosition = Vector3.zero;
            }
            return _activationTarget;
        }
    }

    public static readonly Dictionary<string, byte> PanelTypes = new()
    {
        { "Astromech Droid", 0 },
        { "Protocol Droid", 1 },
        { "Bounty Hunter", 2 },
        { "Stormtrooper", 3 },
    };

    public void UpdatePanelType(int type)
    {

    }

    public void ToggleAlternativeFace(bool on)
    {

    }

    public void ToggleAlternativeBody(bool on)
    {

    }
}
