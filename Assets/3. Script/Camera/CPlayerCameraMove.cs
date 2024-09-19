using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerCameraMove : MonoBehaviour
{
    [SerializeField]
    private float mouseSpeed = 5f; // ȸ���ӵ�
    private float mouseX = 0f; // �¿� ȸ������ ���� ����
    private CCameraTransView cameraTransView;

    private void Awake()
    {
        cameraTransView = FindObjectOfType<CCameraTransView>();
    }
    void Update()
    {
        if (!cameraTransView.isInTopView)
        {
            Cursor.lockState = CursorLockMode.Locked;
            mouseX += Input.GetAxis("Mouse X") * mouseSpeed;

            this.transform.localEulerAngles = new Vector3(0, mouseX, 0);
        }
    }
}
