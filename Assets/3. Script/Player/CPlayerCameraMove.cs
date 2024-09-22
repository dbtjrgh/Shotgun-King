using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerCameraMove : MonoBehaviour
{
    [SerializeField]
    private float mouseSpeed = 5f; // ȸ�� �ӵ�
    [SerializeField]
    private float minYAngle = -60f; // ���� ȸ�� ���� �ּҰ�
    [SerializeField]
    private float maxYAngle = 60f;  // ���� ȸ�� ���� �ִ밪

    private float mouseX = 0f; // �¿� ȸ������ ���� ����
    private float mouseY = 0f;
    private CCameraTransView cameraTransView;

    // ī�޶� ȸ����Ű�� ���� ī�޶� Transform�� ���� ����
    [SerializeField]
    private Transform playerCamera;

    private void Awake()
    {
        cameraTransView = FindObjectOfType<CCameraTransView>();
        this.transform.localEulerAngles = new Vector3(0, mouseX, 0); // �ʱ� �¿� ȸ�� ����
        playerCamera.localEulerAngles = new Vector3(mouseY, 0, 0); // ī�޶� ���� ȸ�� ����
    }

    void Update()
    {
        if (!cameraTransView.isInTopView)
        {
            Cursor.lockState = CursorLockMode.Locked;

            // �¿� ȸ�� (ĳ���� ��ü�� ȸ��)
            mouseX += Input.GetAxis("Mouse X") * mouseSpeed;
            this.transform.localEulerAngles = new Vector3(0, mouseX, 0);

            // ���� ȸ�� (ī�޶� ȸ��)
            mouseY -= Input.GetAxis("Mouse Y") * mouseSpeed;
            mouseY = Mathf.Clamp(mouseY, minYAngle, maxYAngle); // ���� ȸ���� ����
            playerCamera.localEulerAngles = new Vector3(mouseY, 0, 0);
        }
    }
}
