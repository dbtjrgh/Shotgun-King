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

    public LineRenderer lineRenderer;

    // 총알 UI 관리
    public GameObject LoadedBulletUI; // 장전된 총알을 위한 UI
    public GameObject PlayerBulletUI; // 플레이어가 가지고 있는 총알을 위한 UI
    public GameObject BulletPrefab;
    public GameObject EmptyBulletPrefab;

    public int shotgunDamage;
    public int MinshotgunDistance;
    [Range(1f, 8f)] public int MaxshotgunDistance;
    public float shotAngle;
    public int numberOfPoints;

    // 총알 관련 변수
    public int maxBullets = 6; // 최대 소유 가능한 총알 수
    public int currentBullets = 6; // 현재 플레이어가 가지고 있는 총알 수
    public int loadedBullets = 2; // 현재 장전된 총알 수
    public int maxLoadedBullets = 2; // 샷건에 장전 가능한 최대 총알 수

    private void Awake()
    {
        cameraTransView = FindObjectOfType<CCameraTransView>();
        boardManager = FindObjectOfType<CBoardManager>();
    }

    private void Start()
    {
        UpdateBulletUI();
    }

    private void Update()
    {
        MinshotgunDistance = MaxshotgunDistance - 2;
        VisualizeShotgunSpread();

        if (cameraTransView.isInTopView)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) && loadedBullets > 0 && !boardManager.isWhiteTurn)
        {
            camAnim.Play(camAnim.clip.name);
            ShootShotgun();
            loadedBullets--; // 총알을 발사하면 장전된 총알을 하나 감소
            UpdateBulletUI(); // UI 업데이트
        }

        // R키를 눌러 장전 (재장전)
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    // 발사 범위 시각화 함수
    private void VisualizeShotgunSpread()
    {
        lineRenderer.positionCount = numberOfPoints + 2;
        Vector3[] linePoints = new Vector3[numberOfPoints + 2];

        linePoints[0] = firePoint.transform.position;

        for (int i = 1; i <= numberOfPoints; i++)
        {
            float angle = Mathf.Lerp(-shotAngle / 2, shotAngle / 2, (float)(i - 1) / (numberOfPoints - 1));
            Vector3 direction = Quaternion.Euler(0, angle, 0) * firePoint.transform.forward;
            linePoints[i] = firePoint.transform.position + direction.normalized * MaxshotgunDistance;
        }

        linePoints[numberOfPoints + 1] = firePoint.transform.position;
        lineRenderer.SetPositions(linePoints);
    }

    // 총알 발사 함수
    private void ShootShotgun()
    {
        for (int i = 0; i < shotgunDamage; i++)
        {
            float randomY = Random.Range(-shotAngle / 3, shotAngle / 3);
            float randomX = Random.Range(-5, 5);

            Quaternion randomRotation = Quaternion.Euler(randomX, randomY, 0);
            Quaternion finalRotation = firePoint.transform.rotation * randomRotation;

            GameObject projectileInstance = Instantiate(projectTile, firePoint.transform.position, finalRotation);

            CProjectile projectileScript = projectileInstance.GetComponent<CProjectile>();
            if (projectileScript != null)
            {
                projectileScript.maxDistance = MaxshotgunDistance;
            }
        }
    }

    // 장전된 총알 UI 업데이트 함수
    private void UpdateLoadedBulletUI()
    {
        // 기존 UI 제거
        foreach (Transform child in LoadedBulletUI.transform)
        {
            Destroy(child.gameObject);
        }

        // 장전된 총알 표시
        for (int i = 0; i < loadedBullets; i++)
        {
            Instantiate(BulletPrefab, LoadedBulletUI.transform);
        }

        // 빈 장전 슬롯 표시
        for (int i = loadedBullets; i < maxLoadedBullets; i++)
        {
            Instantiate(EmptyBulletPrefab, LoadedBulletUI.transform);
        }
    }

    // 플레이어가 가지고 있는 총알 UI 업데이트 함수
    private void UpdatePlayerBulletUI()
    {
        // 기존 UI 제거
        foreach (Transform child in PlayerBulletUI.transform)
        {
            Destroy(child.gameObject);
        }

        // 남은 총알 표시
        for (int i = 0; i < currentBullets; i++)
        {
            Instantiate(BulletPrefab, PlayerBulletUI.transform);
        }

        // 빈 슬롯 표시
        for (int i = currentBullets; i < maxBullets; i++)
        {
            Instantiate(EmptyBulletPrefab, PlayerBulletUI.transform);
        }
    }

    // 두 가지 UI 업데이트 함수 호출
    private void UpdateBulletUI()
    {
        UpdateLoadedBulletUI();  // 장전된 총알 UI 업데이트
        UpdatePlayerBulletUI();  // 플레이어가 소유한 총알 UI 업데이트
    }

    // 장전 함수
    private void Reload()
    {
        // 현재 가지고 있는 총알 중에서 샷건에 장전할 총알 수 계산
        int bulletsToLoad = Mathf.Min(maxLoadedBullets - loadedBullets, currentBullets);
        loadedBullets += bulletsToLoad;
        currentBullets -= bulletsToLoad;

        UpdateBulletUI();
    }
}