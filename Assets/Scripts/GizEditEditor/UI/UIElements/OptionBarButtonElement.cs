using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using System;
using Unity.VisualScripting;

public class OptionBarButtonElement : ButtonElement
{
    public Transform subMenu;
    [NonSerialized]
    public bool hoveredOver = false, subMenuWasOpened = false;

    private Coroutine subMenuCloseRoutine;

    private void Awake()
    {
        void Hover()
        {
            hoveredOver = true;
            if (subMenu == null) CursorSet.SetCursor(CursorType.Click);
            else
            {
                if(subMenuCloseRoutine != null) StopCoroutine(subMenuCloseRoutine);
                subMenu.SetAsLastSibling();
                subMenu.gameObject.SetActive(true);

                var parentRect = GetComponent<RectTransform>();
                var subRect = subMenu.GetComponent<RectTransform>();

                Transform parentPanel = transform.parent;
                if (parentPanel != null) LayoutRebuilder.ForceRebuildLayoutImmediate(parentPanel.GetComponent<RectTransform>());
                LayoutRebuilder.ForceRebuildLayoutImmediate(subRect);
                Rect pRect = parentRect.rect;

                Vector3 offset = parentPanel.GetComponent<HorizontalLayoutGroup>() != null ? new Vector3(0, -pRect.height / 2) : new Vector3(pRect.width, pRect.height / 2);
                subRect.position = parentRect.position + (offset * EditorUIManager.CanvasScale);

                subMenuWasOpened = true;
            }
        }

        void UnHover()
        {
            hoveredOver = false;
            CursorSet.SetCursor(CursorType.Normal);
            if (subMenu != null)
            {
                if(subMenuCloseRoutine != null) StopCoroutine(subMenuCloseRoutine);
                subMenuCloseRoutine = StartCoroutine(subMenuCloseDelay());
            }
        }

        void Click()
        {

        }

        //On Hover
        AddEventTrigger((e) => { Hover(); }, EventTriggerType.PointerEnter);

        //On Exit
        AddEventTrigger((e) => { UnHover(); }, EventTriggerType.PointerExit);

        AddEventTrigger((e) => { Click(); }, EventTriggerType.PointerClick);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator subMenuCloseDelay()
    {
        yield return new WaitForSeconds(0.1f);
        while (HoveringSubMenu()) yield return null;
        if(!HoveringSubMenu()) subMenu.gameObject.SetActive(false);
        subMenuCloseRoutine = null;
    }

    public bool HoveringSubMenu()
    {
        foreach(var childBtn in subMenu.GetComponentsInChildren<OptionBarButtonElement>())
        {
            if (childBtn.hoveredOver || childBtn.subMenu.gameObject.activeSelf) return true;
        }
        return false;
    }
}
