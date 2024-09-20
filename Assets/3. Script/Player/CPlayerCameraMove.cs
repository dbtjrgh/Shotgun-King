using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerCameraMove : MonoBehaviour
{
    [SerializeField]
    private float mouseSpeed = 5f; // ȸ���ӵ�
    private float mouseX = 0f; // �¿� ȸ������ ���� ����
    private float mouseY = 0f;
    private CCameraTransView cameraTransView;

    private void Awake()
    {
        cameraTransView = FindObjectOfType<CCameraTransView>();
        this.transform.localEulerAngles = new Vector3(mouseY, mouseX, 0);
    }
    void Update()
    {
        if (!cameraTransView.isInTopView)
        {
            Cursor.lockState = CursorLockMode.Locked;
            mouseX += Input.GetAxis("Mouse X") * mouseSpeed;
            mouseY -= Input.GetAxis("Mouse Y") * mouseSpeed;

            this.transform.localEulerAngles = new Vector3(mouseY, mouseX, 0);
        }
    }
}
