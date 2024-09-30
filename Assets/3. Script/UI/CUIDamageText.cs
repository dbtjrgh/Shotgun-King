using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CUIDamageText : MonoBehaviour
{
    public Transform trEnemy;// ���� Ʈ������
    private CUIDamagePool damagePool;

    void Start()
    {
        damagePool = FindObjectOfType<CUIDamagePool>();
    }

    void Update()
    {
        if (trEnemy != null)
        {
            // ���� ��ġ�� �������� ���� ��ġ�� ����
            transform.position = trEnemy.position;
            // ī�޶� ���ϵ��� ȸ��
            transform.LookAt(Camera.main.transform);
            // LookAt�� �ؽ�Ʈ�� �ݴ� �������� ���� ������ 180�� ȸ��
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

            // �ð��� ������ �ö󰡰Բ�
            transform.position = Vector3.Lerp(startPosition, endPosition, t);

            // �ð��� ������ ������Բ�
            textMesh.color = Color.Lerp(originalColor, new Color(originalColor.r, originalColor.g, originalColor.b, 0), t);

            // �ð��� ������ ��Ʈ ũ�Ⱑ �۾����Բ�
            textMesh.fontSize = Mathf.Lerp(originalFontSize, 0, t);

            yield return null;
        }
        // ��Ȱ���� ���� �� �ʱ�ȭ
        transform.position = startPosition;
        textMesh.color = originalColor;
        textMesh.fontSize = originalFontSize;

        // ������Ʈ�� Ǯ�� ��ȯ
        damagePool.ReturnObject(gameObject);
    }
}
