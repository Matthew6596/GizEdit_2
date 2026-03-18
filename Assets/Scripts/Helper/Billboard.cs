using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        transform.LookAt(cam.position);
        transform.Rotate(Vector3.right, 90);
    }
}
