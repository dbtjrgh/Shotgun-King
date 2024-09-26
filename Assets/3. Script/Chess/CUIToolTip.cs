using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; 

public class CUIToolTip : MonoBehaviour
{
    public GameObject tip;
    public Canvas canvas;
    public Vector3 offset = new Vector3(10f, 10f, 0f); // ���콺 Ŀ���� tip ������ �Ÿ� ����

    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;

    private void Start()
    {
        if (tip != null)
        {
            tip.SetActive(false);
        }

        // ĵ������ GraphicRaycaster ����
        raycaster = canvas.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current; // ���� EventSystem ��������
    }

    private void Update()
    {
        // ���콺�� UI ��� ���� �ִ��� ����
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        bool isHovering = false;

        foreach (RaycastResult result in results)
        {
            if (result.gameObject == gameObject)
            {
                isHovering = true;
                break;
            }
        }

        // ���콺�� UI ���� ���� �� ���� Ȱ��ȭ
        if (isHovering)
        {
            if (tip != null && !tip.activeSelf)
            {
                tip.SetActive(true);
            }
            MoveTipToMouse();
        }
        else
        {
            if (tip != null && tip.activeSelf)
            {
                tip.SetActive(false);
            }
        }
    }

    // ���콺 ��ġ�� ���� ���� �̵�
    private void MoveTipToMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        tip.transform.position = mousePosition + offset; // ���콺 ��ġ�� �������� �߰��� tip ��ġ ����
    }
}
