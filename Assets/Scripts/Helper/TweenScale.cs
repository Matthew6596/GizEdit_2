using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TweenScale : MonoBehaviour
{
    public float rate=0.1f;
    public bool scaledTime = true;
    private Vector3 targetScale, ogScale;

    // Start is called before the first frame update
    void Start()
    {
        if(ogScale==Vector3.zero)ogScale = transform.localScale;
        targetScale = ogScale;
    }
    public void InitOGScale() { ogScale = transform.localScale; }

    // Update is called once per frame
    void Update()
    {
        transform.localScale=(scaledTime)?Tween.LazyTweenScaled(transform.localScale, targetScale, rate): Tween.LazyTweenAvgUnscaled(transform.localScale, targetScale, rate);
    }

    public void SetOGScale(Vector3 scale) { ogScale = scale; }
    public void SetScaleX(float x){ targetScale.x = x; }
    public void SetScaleY(float y){ targetScale.y = y; }
    public void SetScaleZ(float z){ targetScale.z = z; }
    public void SetScaleXY(float xy) { targetScale.x = xy; targetScale.y = xy; }
    public void SetImmediateScaleX(float x) { transform.localScale = new(x,transform.localScale.y,transform.localScale.z); }
    public void SetImmediateScaleY(float y) { transform.localScale = new(transform.localScale.x, y,transform.localScale.z); }
    public void SetImmediateScaleZ(float z) { transform.localScale = new(transform.localScale.x, transform.localScale.y,z); }
    public void SetImmediateScaleXY(float xy) { transform.localScale = new(xy, xy,transform.localScale.z); }
    public void SetScaleMultOGX(float x) {  targetScale.x = ogScale.x*x; }
    public void SetScaleMultOGY(float y) {  targetScale.y = ogScale.y*y; }
    public void SetScaleMultOGZ(float z) {  targetScale.z = ogScale.z*z; }
    public void SetScaleMultOGXY(float xy) {  targetScale.x = ogScale.x*xy; targetScale.y = ogScale.y * xy; }
    public void SetScaleMultX(float x) { targetScale.x = targetScale.x * x; }
    public void SetScaleMultY(float y) { targetScale.y = targetScale.y * y; }
    public void SetScaleMultZ(float z) { targetScale.z = targetScale.z * z; }
    public void SetImmediateScaleMultOGX(float x) 
    { transform.localScale = new(x*ogScale.x, transform.localScale.y, transform.localScale.z); }
    public void SetImmediateScaleMultOGY(float y)
    { transform.localScale = new(transform.localScale.x, ogScale.y*y, transform.localScale.z); }
    public void SetImmediateScaleMultOGZ(float z)
    { transform.localScale = new(transform.localScale.x, transform.localScale.y, ogScale.z*z); }
    public void SetImmediateScaleMultOGXY(float xy)
    { transform.localScale = new(xy * ogScale.x, ogScale.y * xy, transform.localScale.z); }
    public void SetImmediateScaleMultX(float x)
    { transform.localScale = new(x * targetScale.x, transform.localScale.y, transform.localScale.z); }
    public void SetImmediateScaleMultY(float y)
    { transform.localScale = new(transform.localScale.x, targetScale.y * y, transform.localScale.z); }
    public void SetImmediateScaleMultZ(float z)
    { transform.localScale = new(transform.localScale.x, transform.localScale.y, targetScale.z * z); }
    public void ResetScale(){targetScale = ogScale;}
    public void ResetScaleImmediate(){transform.localScale = ogScale;}
}
