using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerShooting : MonoBehaviour
{
    public GameObject firePoint;
    public GameObject projectTile;

    public Animation camAnim;
    private CCameraTransView cameraTransView;
    private CBoardManager boardManager;

    // ������ ���ݹ����� �ð�ȭ�� LineRenderer
    public LineRenderer lineRenderer;

    // ������ ���ݹ����� �˷��ִ� ��������
    // public Projector shotgunProjector;

    // ���� �������� ���� �Ѿ� ������ ���� (�Ѿ� �ϳ��� ������ 1)
    public int shotgunDamage;
    // ���� ��Ÿ��� ���� �Ѿ� ��Ÿ� ����
    public int MinshotgunDistance;
    [Range(1f, 8f)]
    public int MaxshotgunDistance;
    // ���� �߻簢 (����)
    public float shotAngle;

    public int numberOfPoints;

    private void Awake()
    {
        cameraTransView = FindObjectOfType<CCameraTransView>();
        boardManager = FindObjectOfType<CBoardManager>();
    }

    private void Update()
    {
        // ��Ÿ� ����
        // shotgunProjector.farClipPlane = MaxshotgunDistance;

        // ���� �߻簢�� ���� �þ߰����� ��ȯ
        // shotgunProjector.fieldOfView = CalculateVerticalFOV(shotAngle, shotgunProjector.aspectRatio);

        MinshotgunDistance = MaxshotgunDistance - 2;

        // �߻� ������ �ð�ȭ
        VisualizeShotgunSpread();

        if (cameraTransView.isInTopView)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) && !boardManager.isWhiteTurn)
        {
            camAnim.Play(camAnim.clip.name);
            ShootShotgun();
        }
    }
    // �߻� ���� �ð�ȭ �Լ�
    private void VisualizeShotgunSpread()
    {
        lineRenderer.positionCount = numberOfPoints;
        Vector3[] linePoints = new Vector3[numberOfPoints];

        for (int i = 0; i < numberOfPoints; i++)
        {
            // ���� ��� (�������θ� ������ ��Ʈ��)
            float angle = Mathf.Lerp(-shotAngle / 2, shotAngle / 2, (float)i / (numberOfPoints - 1));

            // �ش� ������ ��ġ ��� (��Ÿ� ����)
            Vector3 direction = Quaternion.Euler(0, angle, 0) * firePoint.transform.forward;

            // Y���� firePoint�� Y���� �����ϰ� �������� ���� ��ġ�� ����
            linePoints[i] = new Vector3(
                firePoint.transform.position.x + direction.x * (MaxshotgunDistance - 0.5f),
                firePoint.transform.position.y,  // Y���� 0���� ����
                firePoint.transform.position.z + direction.z * (MaxshotgunDistance - 0.5f)
            );
        }

        lineRenderer.SetPositions(linePoints);
    }

    private void ShootShotgun()
    {
        for (int i = 0; i < shotgunDamage; i++)
        {
            // Y�����θ� �����ϰ� �������� ����
            float randomY = Random.Range(-shotAngle / 3, shotAngle / 3);
            float randomX = Random.Range(-5, 5);

            // Y�� ȸ���� �ݿ��� ȸ����
            Quaternion randomRotation = Quaternion.Euler(randomX, randomY, 0);

            // ���� firePoint�� ȸ���� ���� ȸ���� �߰�
            Quaternion finalRotation = firePoint.transform.rotation * randomRotation;

            // �߻�ü ����
            GameObject projectileInstance = Instantiate(projectTile, firePoint.transform.position, finalRotation);

            // ������ �߻�ü�� CProjectile ��ũ��Ʈ���� maxDistance�� MaxshotgunDistance�� ����
            CProjectile projectileScript = projectileInstance.GetComponent<CProjectile>();
            if (projectileScript != null)
            {
                projectileScript.maxDistance = MaxshotgunDistance;
            }
        }
    }
}
