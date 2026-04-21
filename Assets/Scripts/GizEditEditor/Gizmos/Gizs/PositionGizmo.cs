using UnityEngine;

public class PositionGizmo : EditorGizmo
{
    public ArrowGizmoPart xArrow, yArrow, zArrow;
    public PlaneGizmoPart xzPlane, xyPlane, yzPlane;

    private bool started = false;

    private void Start()
    {
        if(!started) StartGizmo();
    }

    private void StartGizmo()
    {
        Vector3 initPos = GetValue<Vector3>();

        xzPlane = CreateGizmoPart<PlaneGizmoPart, Vector2>(new(initPos.x, initPos.z));
        xzPlane.Set(new Vector3(1, 0, 1), Color.green);
        xyPlane = CreateGizmoPart<PlaneGizmoPart, Vector2>(new(initPos.x, initPos.y));
        xyPlane.Set(new Vector3(1, 1, 0), Color.blue);
        yzPlane = CreateGizmoPart<PlaneGizmoPart, Vector2>(new(initPos.y, initPos.z));
        yzPlane.Set(new Vector3(0, 1, 1), Color.red);

        xArrow = CreateGizmoPart<ArrowGizmoPart, float>(initPos.x);
        xArrow.Set(Vector3.right, Color.red);
        yArrow = CreateGizmoPart<ArrowGizmoPart, float>(initPos.y);
        yArrow.Set(Vector3.up, Color.green);
        zArrow = CreateGizmoPart<ArrowGizmoPart, float>(initPos.z);
        zArrow.Set(Vector3.forward, Color.blue);

        xArrow.OnValueChange.AddListener((e) => { UpdatePos(new(e, 0, 0), new(0, 1, 1)); });
        yArrow.OnValueChange.AddListener((e) => { UpdatePos(new(0, e, 0), new(1, 0, 1)); });
        zArrow.OnValueChange.AddListener((e) => { UpdatePos(new(0, 0, e), new(1, 1, 0)); });
        xzPlane.OnValueChange.AddListener((e) => { UpdatePos(new(e.x, 0, e.y), new(0, 1, 0)); });
        xyPlane.OnValueChange.AddListener((e) => { UpdatePos(new(e.x, e.y, 0), new(0, 0, 1)); });
        yzPlane.OnValueChange.AddListener((e) => { UpdatePos(new(0, e.x, e.y), new(1, 0, 0)); });

        started = true;
    }

    private void Update()
    {
        
    }

    private void UpdatePos(Vector3 newPos, Vector3 inverseAxis)
    {
        Vector3 pos = GetValue<Vector3>();
        Value = Vector3.Scale(pos, inverseAxis) + newPos;
        OnValueChange.Invoke(Value);
    }

    private T1 CreateGizmoPart<T1,T2>(T2 value) where T1 : EditorGizmoPart<T2>
    {
        GameObject obj = new("editor_gizmo_part");
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero;
        T1 gizPart = obj.AddComponent<T1>();
        gizPart.value = value;
        return gizPart;
    }

    public void RefreshValues()
    {
        if (!started) StartGizmo();
        Vector3 pos = (Vector3)Value;
        xArrow.value = pos.x;
        yArrow.value = pos.y;
        zArrow.value = pos.z;
        xzPlane.value = new(pos.x, pos.z);
        xyPlane.value = new(pos.x, pos.y);
        yzPlane.value = new(pos.y, pos.z);
    }
}
