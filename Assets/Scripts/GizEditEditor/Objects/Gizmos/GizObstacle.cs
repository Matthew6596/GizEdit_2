using UnityEngine;

public class GizObstacle : TTObject
{
    private GameObject _boundsCorner;
    public GameObject BoundsCorner
    {
        get
        {
            if (_boundsCorner == null) _boundsCorner = new GameObject("obstacle_bounds_corner");
            return _boundsCorner;
        }
    }
}
