using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

public class EditorGizmoManager : MonoBehaviour
{
    public static EditorGizmoManager Instance { get; private set; }

    private static ButtonElement prevBtn;

    private static string _editState;
    public static string EditState { get=>_editState; set { 
            _editState = value.ToUpper();
            RefreshGizmosVisibility();

            //Button colors
            if(prevBtn != null)
            {
                prevBtn.colorType = EditorColorType.WindowPrimary;
                prevBtn.ApplyCurrentTheme();
            }
            var btn = EditorUIManager.Instance.FindEditorTool(_editState);
            if (btn != null)
            {
                btn.colorType = EditorColorType.GoodGreen;
                btn.ApplyCurrentTheme();
                prevBtn = btn;
            }

            onEditStateChange.Invoke(_editState);
            return; 
        }
    }
    public static UnityEvent<string> onEditStateChange = new();
    public static bool IsEditState(string state) => EditState == state.ToUpper();
    public static bool IsEditState(params string[] states)
    {
        foreach(string state in states) if(IsEditState(state)) return true;
        return false;
    }

    private readonly static List<EditorGizmo> gizmos = new();

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EditorUIManager.Instance.AddEditorTool("Move", () => { EditState = "Move"; }, -1);
        EditorUIManager.Instance.AddEditorTool("Rotate", () => { EditState = "Rotate"; }, -1);
        EditorUIManager.Instance.AddEditorTool("Scale", () => { EditState = "Scale"; }, -1);
        EditState = "MOVE";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static T Create<T>(object initialValue, UnityAction<object> callback) where T : EditorGizmo
    {
        GameObject gizmoObj = new("editor_gizmo");
        T giz = gizmoObj.AddComponent<T>();
        giz.Value = initialValue;
        giz.OnValueChange.AddListener(callback);
        gizmos.Add(giz);
        return giz;
    }

    public static void DestroyAllGizmos()
    {
        for(int i=gizmos.Count-1; i>=0; i--)
        {
            Destroy(gizmos[i].gameObject);
            gizmos.RemoveAt(i);
        }
    }

    public static void RefreshGizmosVisibility()
    {
        for (int i = gizmos.Count - 1; i >= 0; i--)
        {
            if (IsEditState(gizmos[i].validStates)) gizmos[i].Show();
            else gizmos[i].Hide();
        }
    }
}