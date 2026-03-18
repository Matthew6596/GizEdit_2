using UnityEngine;

public static class Tween
{
    public static float tweenRateMultiplier = 80;
    public static Vector3 LazyTween(Vector3 currentPos, Vector3 targetPos, float rate)
    {
        return Vector3.MoveTowards(currentPos, targetPos, Vector3.Distance(currentPos, targetPos) * rate);
    }
    public static Vector3 LazyTweenAvgUnscaled(Vector3 currentPos, Vector3 targetPos, float rate)
    {
        return Vector3.MoveTowards(currentPos, targetPos, Vector3.Distance(currentPos, targetPos) * rate * Time.unscaledDeltaTime *tweenRateMultiplier);
    }
    public static Vector3 LazyTweenUnscaled(Vector3 currentPos, Vector3 targetPos, float rate)
    {
        return Vector3.MoveTowards(currentPos, targetPos, Vector3.Distance(currentPos, targetPos) * rate * Time.unscaledDeltaTime * tweenRateMultiplier);
    }
    public static float LazyTween(float currentNum, float targetNum, float rate)
    {
        if (currentNum < targetNum) return currentNum + (targetNum - currentNum) * rate;
        else return currentNum + (currentNum - targetNum) * -rate;
    }
    public static float LazyTweenAvgUnscaled(float currentNum, float targetNum, float rate)
    {
        if (currentNum < targetNum) return currentNum + (targetNum - currentNum) * rate * Time.unscaledDeltaTime * tweenRateMultiplier;
        else return currentNum + (currentNum - targetNum) * -rate * Time.unscaledDeltaTime * tweenRateMultiplier;
    }
    public static float LazyTweenUnscaled(float currentNum, float targetNum, float rate)
    {
        if (currentNum < targetNum) return currentNum + (targetNum - currentNum) * rate * Time.unscaledDeltaTime * tweenRateMultiplier;
        else return currentNum + (currentNum - targetNum) * -rate * Time.unscaledDeltaTime * tweenRateMultiplier;
    }
    public static void LazyTweenUnscaled(Transform currentTransform, Transform targetTransform, float rate)
    {
        currentTransform.SetLocalPositionAndRotation(LazyTweenAvgUnscaled(currentTransform.localPosition, targetTransform.localPosition, rate * Time.unscaledDeltaTime * tweenRateMultiplier), Quaternion.Euler(LazyTweenAvgUnscaled(currentTransform.localRotation.eulerAngles,targetTransform.localRotation.eulerAngles, rate * Time.unscaledDeltaTime * tweenRateMultiplier)));
        currentTransform.localScale = LazyTweenAvgUnscaled(currentTransform.localScale, targetTransform.localScale, rate * Time.unscaledDeltaTime * tweenRateMultiplier);
    }
    public static Vector3 LazyTweenScaled(Vector3 currentPos, Vector3 targetPos, float rate)
    {
        return Vector3.MoveTowards(currentPos, targetPos, Vector3.Distance(currentPos, targetPos) * rate * Time.deltaTime *tweenRateMultiplier);
    }
    public static float LazyTweenScaled(float currentNum, float targetNum, float rate)
    {
        if (currentNum < targetNum) return currentNum + (targetNum - currentNum) * rate * Time.deltaTime * tweenRateMultiplier;
        else return currentNum + (currentNum - targetNum) * -rate * Time.deltaTime * tweenRateMultiplier;
    }
    public static void LazyTweenScaled(Transform currentTransform, Transform targetTransform, float rate)
    {
        currentTransform.SetLocalPositionAndRotation(LazyTweenScaled(currentTransform.localPosition, targetTransform.localPosition, rate *Time.deltaTime * tweenRateMultiplier), Quaternion.Euler(LazyTweenScaled(currentTransform.localRotation.eulerAngles,targetTransform.localRotation.eulerAngles, rate * Time.deltaTime * tweenRateMultiplier)));
        currentTransform.localScale = LazyTweenScaled(currentTransform.localScale, targetTransform.localScale, rate * Time.deltaTime *tweenRateMultiplier);
    }
    public static Quaternion LazyTweenUnscaled(Quaternion currentPos, Quaternion targetPos, float rate)
    {
        return Quaternion.RotateTowards(currentPos, targetPos, Vector3.Distance(currentPos.eulerAngles, targetPos.eulerAngles) * rate *Time.unscaledDeltaTime * tweenRateMultiplier);
    }
    public static Quaternion LazyTweenScaled(Quaternion currentPos, Quaternion targetPos, float rate)
    {
        return Quaternion.RotateTowards(currentPos, targetPos, Vector3.Distance(currentPos.eulerAngles, targetPos.eulerAngles) * rate *Time.deltaTime * tweenRateMultiplier);
    }
    public static Quaternion Wobble(float speed, float amount, float time) { return Quaternion.Euler(0, 0, Mathf.Sin(speed * time) *amount); }
    public static Quaternion WobbleUnscaled(float speed, float amount) { return Quaternion.Euler(0, 0, Mathf.Sin(speed * Time.unscaledTime) *amount); }
    public static Quaternion WobbleScaled(float speed, float amount) { return Quaternion.Euler(0, 0, Mathf.Sin(speed * Time.time) * amount); }

    public static float TweenAngleScaled(float currAng, float targetAng, float rate)
    {
        return currAng + Mathf.DeltaAngle(currAng, targetAng)*rate*tweenRateMultiplier*Time.deltaTime;
    }
    public static float TweenAngleUnscaled(float currAng, float targetAng, float rate)
    {
        return currAng + Mathf.DeltaAngle(currAng, targetAng) * rate * tweenRateMultiplier * Time.unscaledDeltaTime;
    }
}
