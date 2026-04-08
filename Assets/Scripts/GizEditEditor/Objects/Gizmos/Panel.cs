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

    public static readonly string[] PanelTypes = new string[]
    {
        "Astromech Droid", "Protocol Droid", "Bounty Hunter", "Stormtrooper"
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
