using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CUIDamageText : MonoBehaviour
{
    public Transform trEnemy;// 적의 트랜스폼
    private CUIDamagePool damagePool;

    void Start()
    {
        damagePool = FindObjectOfType<CUIDamagePool>();
    }

    void Update()
    {
        if (trEnemy != null)
        {
            // 적의 위치에 오프셋을 더한 위치로 설정
            transform.position = trEnemy.position;
            // 카메라를 향하도록 회전
            transform.LookAt(Camera.main.transform);
            // LookAt이 텍스트를 반대 방향으로 보기 때문에 180도 회전
            transform.Rotate(0, 180, 0);
        }
    }

    public void Initialize(Transform enemy, Vector3 offset, CUIDamagePool pool)
    {
        this.trEnemy = enemy;
        this.damagePool = pool;
        transform.position = enemy.position + offset;
        StartCoroutine(FadeAndMove());
    }

    private IEnumerator FadeAndMove()
    {
        TextMeshProUGUI textMesh = GetComponent<TextMeshProUGUI>();
        Color originalColor = textMesh.color;
        float originalFontSize = textMesh.fontSize;
        float duration = 1f;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + new Vector3(Random.Range(-1.0f, 1.0f), 1.0f, Random.Range(-1.0f, 1.0f));

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // 시간이 갈수록 올라가게끔
            transform.position = Vector3.Lerp(startPosition, endPosition, t);

            // 시간이 갈수록 사라지게끔
            textMesh.color = Color.Lerp(originalColor, new Color(originalColor.r, originalColor.g, originalColor.b, 0), t);

            // 시간이 갈수록 폰트 크기가 작아지게끔
            textMesh.fontSize = Mathf.Lerp(originalFontSize, 0, t);

            yield return null;
        }
        // 재활용을 위한 값 초기화
        transform.position = startPosition;
        textMesh.color = originalColor;
        textMesh.fontSize = originalFontSize;

        // 오브젝트를 풀에 반환
        damagePool.ReturnObject(gameObject);
    }
}
