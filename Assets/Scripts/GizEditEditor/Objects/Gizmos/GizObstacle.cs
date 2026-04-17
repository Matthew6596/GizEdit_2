using UnityEngine;

public class GizObstacle : TTObject
{
    public GameObject renderObj;

    private GameObject _boundsCornerBox;
    private GameObject _boundsCorner;
    public GameObject BoundsCorner
    {
        get
        {
            if (_boundsCorner == null)
            {
                _boundsCornerBox = new("obstalce_bounds_corner_box");
                _boundsCornerBox.transform.SetParent(transform);
                _boundsCornerBox.transform.position = Vector3.zero;
                _boundsCorner = new GameObject("obstacle_bounds_corner");
                _boundsCorner.transform.SetParent(_boundsCornerBox.transform);
            }
            return _boundsCorner;
        }
    }

    private void Update()
    {
        if (_boundsCornerBox != null) _boundsCornerBox.transform.position = Vector3.zero;
    }
}
