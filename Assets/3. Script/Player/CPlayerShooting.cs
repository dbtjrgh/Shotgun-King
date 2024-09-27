using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerShooting : MonoBehaviour
{
    #region ����
    public GameObject firePoint;
    public GameObject projectTile;
    public Animation camAnim;
    private CCameraTransView cameraTransView;
    private CBoardManager boardManager;
    public LineRenderer lineRenderer;

    // �Ѿ� UI ����
    public GameObject LoadedBulletUI; // ������ �Ѿ��� ���� UI
    public GameObject PlayerBulletUI; // �÷��̾ ������ �ִ� �Ѿ��� ���� UI
    public GameObject BulletPrefab;
    public GameObject EmptyBulletPrefab;

    public int shotgunDamage;
    public int MinshotgunDistance;
    [Range(1f, 8f)] public int MaxshotgunDistance;
    public float shotAngle;
    public int numberOfPoints;

    // �Ѿ� ���� ����
    public int maxBullets = 6; // �ִ� ���� ������ �Ѿ� ��
    public int currentBullets = 6; // ���� �÷��̾ ������ �ִ� �Ѿ� ��
    public int maxLoadedBullets = 2; // ���ǿ� ���� ������ �ִ� �Ѿ� ��
    public int loadedBullets = 2; // ���� ������ �Ѿ� ��
    #endregion

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
            // ���� ���ٸ� �� �ѱ��
            boardManager.isWhiteTurn = !boardManager.isWhiteTurn;
            camAnim.Play(camAnim.clip.name);
            ShootShotgun();
            loadedBullets--; // �Ѿ��� �߻��ϸ� ������ �Ѿ��� �ϳ� ����
            UpdateBulletUI(); // UI ������Ʈ
        }

        // RŰ�� ���� ���� (������)
        // ������ �Ѿ��� 2�� �̸��̰� �÷��̾� �ָӴϿ� �Ѿ��� �־�� ��.
        if (Input.GetKeyDown(KeyCode.R) && loadedBullets < maxLoadedBullets && currentBullets > 0)
        {
            // �������� �ߴٸ� �� �ѱ��
            boardManager.isWhiteTurn = !boardManager.isWhiteTurn;
            Reload();
        }
    }

    // �߻� ���� �ð�ȭ �Լ�
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

    // �Ѿ� �߻� �Լ�
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

    // ������ �Ѿ� UI ������Ʈ �Լ�
    private void UpdateLoadedBulletUI()
    {
        // ���� UI ����
        foreach (Transform child in LoadedBulletUI.transform)
        {
            Destroy(child.gameObject);
        }

        // ������ �Ѿ� ǥ��
        for (int i = 0; i < loadedBullets; i++)
        {
            Instantiate(BulletPrefab, LoadedBulletUI.transform);
        }

        // �� ���� ���� ǥ��
        for (int i = loadedBullets; i < maxLoadedBullets; i++)
        {
            Instantiate(EmptyBulletPrefab, LoadedBulletUI.transform);
        }
    }

    // �÷��̾ ������ �ִ� �Ѿ� UI ������Ʈ �Լ�
    private void UpdatePlayerBulletUI()
    {
        // ���� UI ����
        foreach (Transform child in PlayerBulletUI.transform)
        {
            Destroy(child.gameObject);
        }

        // ���� �Ѿ� ǥ��
        for (int i = 0; i < currentBullets; i++)
        {
            Instantiate(BulletPrefab, PlayerBulletUI.transform);
        }

        // �� ���� ǥ��
        for (int i = currentBullets; i < maxBullets; i++)
        {
            Instantiate(EmptyBulletPrefab, PlayerBulletUI.transform);
        }
    }

    // �� ���� UI ������Ʈ �Լ� ȣ��
    private void UpdateBulletUI()
    {
        UpdateLoadedBulletUI();  // ������ �Ѿ� UI ������Ʈ
        UpdatePlayerBulletUI();  // �÷��̾ ������ �Ѿ� UI ������Ʈ
    }

    // ���� �Լ�
    private void Reload()
    {
        // ���� ������ �ִ� �Ѿ� �߿��� ���ǿ� ������ �Ѿ� �� ���
        int bulletsToLoad = Mathf.Min(maxLoadedBullets - loadedBullets, currentBullets);
        loadedBullets += bulletsToLoad;
        currentBullets -= bulletsToLoad;

        UpdateBulletUI();
    }

    /// <summary>
    /// �Ѿ� ���� ����
    /// �� if (������ �Ѿ��� �� �� ������ && �������� �� && �÷��̾� �ָӴϿ� �Ѿ��� �� ������ ���� ��)
    /// { �÷��̾� �ָӴϿ� �Ѿ� 1�� ���� }
    /// �� else if (������ �Ѿ��� �ѹ��̶� ������� �� && �������� �� && �÷��̾ �����ϰ� �ִ� �Ѿ��� ���� ��)
    /// { �÷��̾� �ָӴϿ��� �Ѿ��� �����ͼ� ����,  }
    /// �� else if (������ �Ѿ��� �ѹ��̶� ������� �� &&  �������� �� && �÷��̾ �����ϰ� �ִ� �Ѿ��� ���� ��)
    /// { �÷��̾� �ָӴϿ� �Ѿ� 1�� ����  }
    /// </summary>
    public void MoveAndReload()
    {
        if(loadedBullets == maxLoadedBullets && currentBullets < maxBullets)
        {
            currentBullets++;
        }
        else if( loadedBullets < maxLoadedBullets && currentBullets > 0)
        {
            Reload();
        }
        else if (loadedBullets < maxLoadedBullets && currentBullets == 0)
        {
            currentBullets++;
        }
        UpdateBulletUI();
    }
}