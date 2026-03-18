using UnityEngine;

public class Lever : TTObject
{
    public GameObject ActivationTarget { get; private set; }

    private void Awake()
    {
        ActivationTarget = new GameObject("lever_activation_target");
        ActivationTarget.transform.SetParent(transform);
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
        ""
    };
}
