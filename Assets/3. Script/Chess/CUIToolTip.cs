using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; 

public class CUIToolTip : MonoBehaviour
{
    public GameObject tip;
    public Canvas canvas;
    public Vector3 offset = new Vector3(10f, 10f, 0f); // 마우스 커서와 tip 사이의 거리 조정

    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;

    private void Start()
    {
        if (tip != null)
        {
            tip.SetActive(false);
        }

        // 캔버스의 GraphicRaycaster 설정
        raycaster = canvas.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current; // 현재 EventSystem 가져오기
    }

    private void Update()
    {
        // 마우스가 UI 요소 위에 있는지 감지
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

        // 마우스가 UI 위에 있을 때 툴팁 활성화
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

    // 마우스 위치에 따라 툴팁 이동
    private void MoveTipToMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        tip.transform.position = mousePosition + offset; // 마우스 위치에 오프셋을 추가해 tip 위치 설정
    }
}
