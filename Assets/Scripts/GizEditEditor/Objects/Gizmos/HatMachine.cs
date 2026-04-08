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

    public static readonly string[] HatTypes = new string[] //Stormtrooper is default
    {
        "Random", "Leia", "Fedora", "Top Hat", "Baseball Cap", "Stormtrooper", "Bounty Hunter",
        "Droid Panel",
    };

    public void UpdateHandleColor(int colType)
    {

    }

    public void UpdateHatType(int hatType)
    {

    }
}
