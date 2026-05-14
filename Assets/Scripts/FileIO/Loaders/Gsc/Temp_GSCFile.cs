using System;
using System.Collections;
using UnityEngine;

public class Temp_GSCFile : MonoBehaviour
{
    public static Temp_GSCFile Instance {get;private set;}

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void DelayAction(Action action)
    {
        Instance.StartCoroutine(Instance.delay(action));
    }

    IEnumerator delay(Action action)
    {
        yield return null;
        action();
    }
}
