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

    private float mouseX = 180f; // �¿� ȸ������ ���� ���� (�ʱⰪ�� 180���� ����)
    private float mouseY = 0f;
    private CCameraTransView cameraTransView;

    // ī�޶� ȸ����Ű�� ���� ī�޶� Transform�� ���� ����
    [SerializeField]
    private Transform playerCamera;
    [SerializeField]
    private Transform playerShotgun;

    private void Awake()
    {
        cameraTransView = FindObjectOfType<CCameraTransView>();

        // �ʱ� �¿� ȸ������ 180���� �����Ͽ� �ڸ� ���� ��
        this.transform.localEulerAngles = new Vector3(0, mouseX, 0);
        playerCamera.localEulerAngles = new Vector3(mouseY, 0, 0);
        playerShotgun.localEulerAngles = new Vector3(mouseY, 0, 0);
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
            playerShotgun.localEulerAngles = new Vector3(mouseY, 0, 0);
        }
    }
}
