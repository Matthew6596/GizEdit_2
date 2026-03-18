using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TweenAngle : MonoBehaviour
{
    public float rate=0.1f;
    public bool scaledTime = true;
    private float targetAngle, ogAngle;

    // Start is called before the first frame update
    void Start()
    {
        if(ogAngle==0)ogAngle = transform.localEulerAngles.z;
        targetAngle = ogAngle;
    }
    public void InitOGAngle() { ogAngle = transform.localEulerAngles.z; }

    // Update is called once per frame
    void Update()
    {
        transform.localEulerAngles=new Vector3(0,0,(scaledTime)?Tween.TweenAngleScaled(transform.localEulerAngles.z, targetAngle, rate): Tween.TweenAngleUnscaled(transform.localEulerAngles.z, targetAngle, rate));
    }

    public void SetOGAngle(float angle) { ogAngle = angle; }

    public void SetAngle(float angle) { targetAngle = angle; }
    public void SetAngleImmediate(float angle) { transform.localEulerAngles = new(0, 0, angle); }

    public void ResetAngle(){targetAngle = ogAngle;}
    public void ResetAngleImmediate(){transform.localEulerAngles = new(0,0,ogAngle);}
}
