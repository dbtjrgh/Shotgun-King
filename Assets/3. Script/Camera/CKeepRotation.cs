using UnityEngine;

public class CKeepRotation : MonoBehaviour
{
    private Quaternion initialWorldRotation;

    void Awake()
    {
        // �ڽ� ������Ʈ�� �ʱ� ���� ��ǥ�� ȸ�� ����
        initialWorldRotation = transform.rotation;
    }

    void LateUpdate()
    {
        // �θ� ������Ʈ�� ȸ���ص� �ڽ��� ���� ȸ�� ���� ����
        transform.rotation = initialWorldRotation;
    }
}
