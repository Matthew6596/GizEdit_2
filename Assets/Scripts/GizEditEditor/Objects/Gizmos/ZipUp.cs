using UnityEngine;

public class ZipUp : TTObject
{
    private Transform _startTransform, _axisTransform, _endTransform;
    public Transform StartTransform { get
        {
            if(_startTransform == null)
            {
                GameObject obj = new("zipup_start");
                _startTransform = obj.transform;
                _startTransform.SetParent(transform);
            }
            return _startTransform;
        } 
    }
    public Transform AxisTransform { get
        {
            if (_axisTransform == null)
            {
                GameObject obj = new("zipup_axis");
                _axisTransform = obj.transform;
                _axisTransform.SetParent(transform);
            }
            return _axisTransform;
        }
    }
    public Transform EndTransform { get
        {
            if (_endTransform == null)
            {
                GameObject obj = new("zipup_end");
                _endTransform = obj.transform;
                _endTransform.SetParent(transform);
            }
            return _endTransform;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
