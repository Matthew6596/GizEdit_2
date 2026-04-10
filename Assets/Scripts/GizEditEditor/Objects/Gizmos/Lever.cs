using System.Collections.Generic;
using UnityEngine;

public class Lever : TTObject
{
    private GameObject _activationTarget;
    public GameObject ActivationTarget { get 
        {
            if(_activationTarget == null)
            {
                _activationTarget = new GameObject("lever_activation_target");
                _activationTarget.transform.SetParent(transform);
                _activationTarget.transform.localPosition = Vector3.zero;
            }
            return _activationTarget;
        }
    }

    private void Awake()
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static readonly Dictionary<string, byte> StudColors = new()
    {
        { "None", 0 },
        { "Red", (byte)'r' },
        { "Orange", (byte)'o' },
        { "Yellow", (byte)'y' },
        { "Lime", (byte)'l' },
        { "Green", (byte)'g' },
        { "Light Blue", (byte)'u' },
        { "Blue", (byte)'b' },
        { "Purple", (byte)'p' },
        { "Brown", (byte)'w' }
    };

    public void UpdateHandleColor(int colType)
    {
        string handleMat = (colType) switch
        {
            1 => "",
            _ => ""
        };

        //set material
        //TTResourceManager.GetMaterial(handleMat);
    }
}
