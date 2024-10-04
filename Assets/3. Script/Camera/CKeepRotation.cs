using UnityEngine;

public class CKeepRotation : MonoBehaviour
{
    private Quaternion initialWorldRotation;

    void Awake()
    {
        // 자식 오브젝트의 초기 월드 좌표계 회전 저장
        initialWorldRotation = transform.rotation;
    }

    void LateUpdate()
    {
        // 부모 오브젝트가 회전해도 자식의 월드 회전 값을 고정
        transform.rotation = initialWorldRotation;
    }
}
