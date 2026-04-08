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

    public static readonly string[] StudColors = new string[]
    {
        "None", "Red", "Orange", "Yellow", "Lime", "Green", "Light Blue",
        "Blue", "Purple", "Brown"
    };

    public static int GetPickupType(char c) => (c) switch
    {
        'r' => 1, 'o' => 2, 'y' => 3, 'l' => 4,
        'g' => 5, 'u' => 6, 'b' => 7, 'p' => 8, 'w' => 9,
        _ => 0,
    };

    public static char GetPickupChar(int n) => (n) switch
    {
        1 => 'r', 2 => 'o', 3 => 'y', 4 => 'l', 5 => 'g',
        6 => 'u', 7 => 'b', 8 => 'p', 9 => 'w',
        _ => '\0'
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
