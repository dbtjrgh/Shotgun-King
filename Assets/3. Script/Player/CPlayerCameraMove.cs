using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerCameraMove : MonoBehaviour
{
    #region 변수
    [SerializeField]
    private float mouseSpeed = 5f; // 회전 속도
    [SerializeField]
    private float minYAngle = -60f; // 상하 회전 제한 최소값
    [SerializeField]
    private float maxYAngle = 60f;  // 상하 회전 제한 최대값

    private float mouseX = 180f; // 좌우 회전값을 담을 변수 (초기값을 180도로 설정)
    private float mouseY = 0f;
    private CCameraTransView cameraTransView;
    private CStageResultUI stageResultUI;

    // 카메라만 회전시키기 위해 카메라 Transform을 따로 선언
    [SerializeField]
    private Transform playerCamera;
    [SerializeField]
    private Transform playerShotgun;
    #endregion

    private void Awake()
    {
        cameraTransView = FindObjectOfType<CCameraTransView>();
        if (stageResultUI == null)
        {
            stageResultUI = FindAnyObjectByType<CStageResultUI>();
        }

        // 초기 좌우 회전값을 180도로 설정하여 뒤를 보게 함
        this.transform.localEulerAngles = new Vector3(0, mouseX, 0);
        playerCamera.localEulerAngles = new Vector3(mouseY, 0, 0);
        playerShotgun.localEulerAngles = new Vector3(mouseY, 0, 0);
    }

    void Update()
    {
        if (stageResultUI == null)
        {
            stageResultUI = FindAnyObjectByType<CStageResultUI>();
        }
        if (cameraTransView.isInTopView)
        {
            return;
        }

        if (stageResultUI.resultUI.activeSelf)
        {
            return;
        }

        // 좌우 회전 (캐릭터 전체가 회전)
        mouseX += Input.GetAxis("Mouse X") * mouseSpeed;
        this.transform.localEulerAngles = new Vector3(0, mouseX, 0);

        // 상하 회전 (카메라만 회전)
        mouseY -= Input.GetAxis("Mouse Y") * mouseSpeed;
        mouseY = Mathf.Clamp(mouseY, minYAngle, maxYAngle); // 상하 회전값 제한
        playerCamera.localEulerAngles = new Vector3(mouseY, 0, 0);
        playerShotgun.localEulerAngles = new Vector3(mouseY, 0, 0);

    }
}
