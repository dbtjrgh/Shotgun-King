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

    // 샷건의 공격범위를 시각화할 LineRenderer
    public LineRenderer lineRenderer;

    // 샷건의 공격범위를 알려주는 프로젝터
    // public Projector shotgunProjector;

    // 샷건 데미지에 따라 총알 갯수로 구현 (총알 하나당 데미지 1)
    public int shotgunDamage;
    // 샷건 사거리에 따라 총알 사거리 구현
    public int MinshotgunDistance;
    [Range(1f, 8f)]
    public int MaxshotgunDistance;
    // 샷건 발사각 (수평)
    public float shotAngle;

    public int numberOfPoints;

    private void Awake()
    {
        cameraTransView = FindObjectOfType<CCameraTransView>();
        boardManager = FindObjectOfType<CBoardManager>();
    }

    private void Update()
    {
        // 사거리 설정
        // shotgunProjector.farClipPlane = MaxshotgunDistance;

        // 수평 발사각을 수직 시야각으로 변환
        // shotgunProjector.fieldOfView = CalculateVerticalFOV(shotAngle, shotgunProjector.aspectRatio);

        MinshotgunDistance = MaxshotgunDistance - 2;

        // 발사 범위를 시각화
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
    // 발사 범위 시각화 함수
    private void VisualizeShotgunSpread()
    {
        lineRenderer.positionCount = numberOfPoints;
        Vector3[] linePoints = new Vector3[numberOfPoints];

        for (int i = 0; i < numberOfPoints; i++)
        {
            // 각도 계산 (수평으로만 범위를 퍼트림)
            float angle = Mathf.Lerp(-shotAngle / 2, shotAngle / 2, (float)i / (numberOfPoints - 1));

            // 해당 각도로 위치 계산 (사거리 설정)
            Vector3 direction = Quaternion.Euler(0, angle, 0) * firePoint.transform.forward;

            // Y축은 firePoint의 Y축을 유지하고 나머지는 계산된 위치로 설정
            linePoints[i] = new Vector3(
                firePoint.transform.position.x + direction.x * (MaxshotgunDistance - 0.5f),
                firePoint.transform.position.y,  // Y축을 0으로 고정
                firePoint.transform.position.z + direction.z * (MaxshotgunDistance - 0.5f)
            );
        }

        lineRenderer.SetPositions(linePoints);
    }

    private void ShootShotgun()
    {
        for (int i = 0; i < shotgunDamage; i++)
        {
            // Y축으로만 랜덤하게 퍼지도록 설정
            float randomY = Random.Range(-shotAngle / 3, shotAngle / 3);
            float randomX = Random.Range(-5, 5);

            // Y축 회전만 반영한 회전값
            Quaternion randomRotation = Quaternion.Euler(randomX, randomY, 0);

            // 원래 firePoint의 회전에 랜덤 회전을 추가
            Quaternion finalRotation = firePoint.transform.rotation * randomRotation;

            // 발사체 생성
            GameObject projectileInstance = Instantiate(projectTile, firePoint.transform.position, finalRotation);

            // 생성된 발사체의 CProjectile 스크립트에서 maxDistance를 MaxshotgunDistance로 설정
            CProjectile projectileScript = projectileInstance.GetComponent<CProjectile>();
            if (projectileScript != null)
            {
                projectileScript.maxDistance = MaxshotgunDistance;
            }
        }
    }
}
