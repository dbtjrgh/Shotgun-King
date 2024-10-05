using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class CPlayerShooting : MonoBehaviour
{
    #region ����
    public GameObject firePoint;
    public Transform bulletCasePos;
    public GameObject projectTile;
    public GameObject bulletCase;
    public GameObject shotgun;
    public GameObject shotgunReload;
    public Animation camAnim;
    private CCameraTransView cameraTransView;
    private CStageResultUI stageResultUI;
    private CStageDefeatUI stageDefeatUI;
    private CBoardManager boardManager;
    public LineRenderer lineRenderer;

    // �Ѿ� UI ����
    public GameObject LoadedBulletUI; // ������ �Ѿ��� ���� UI
    public GameObject PlayerBulletUI; // �÷��̾ ������ �ִ� �Ѿ��� ���� UI
    public GameObject BulletPrefab;
    public GameObject EmptyBulletPrefab;

    // �������� UI ǥ��
    public TextMeshProUGUI stageFloor;

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

    // �ɼ� �޴�
    public GameObject optionMenuUI; // �ɼ� �޴� UI
    public UnityEngine.UI.Button optionBackButton;
    public UnityEngine.UI.Button backMainMenuButton;
    private bool isPaused = false; // ���� �Ͻ� ���� ����
    #endregion

    private void Awake()
    {
        cameraTransView = FindObjectOfType<CCameraTransView>();
        boardManager = FindObjectOfType<CBoardManager>();
        optionBackButton.onClick.AddListener(ResumeGameButton);
        backMainMenuButton.onClick.AddListener(BackMainMenuButton);
    }

    private void Start()
    {
        UpdateBulletUI();
    }

    private void Update()
    {
        MinshotgunDistance = MaxshotgunDistance - 2;
        VisualizeShotgunSpread();
        if (boardManager == null)
        {
            boardManager = FindObjectOfType<CBoardManager>();
        }
        if (stageResultUI == null)
        {
            stageResultUI = FindAnyObjectByType<CStageResultUI>();
        }

        if (stageDefeatUI == null)
        {
            stageDefeatUI = FindAnyObjectByType<CStageDefeatUI>();
        }
        if (boardManager.isTutorial)
        {
            stageFloor.text = ($"���� �� : <color=red>Ʃ�丮�� ��");
        }
        else
        {
            stageFloor.text = ($"���� �� : <color=red>{boardManager.stageFloor}��");
        }

        if (cameraTransView.isInTopView)
        {
            return;
        }

        if (stageResultUI.resultUI.activeSelf)
        {
            return;
        }

        // ESC Ű�� ���� �ɼ� �޴� ���
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGameButton();
            }
            else
            {
                PauseGameButton();
            }
        }

        if (Input.GetMouseButtonDown(0) && loadedBullets > 0 && !boardManager.isWhiteTurn)
        {
            CSoundManager.Instance.PlaySfx(5);
            // ���� ���ٸ� �� �ѱ��
            boardManager.isWhiteTurn = true;
            camAnim.Play(camAnim.clip.name);
            ShootShotgun();
            loadedBullets--; // �Ѿ��� �߻��ϸ� ������ �Ѿ��� �ϳ� ����
            UpdateBulletUI(); // UI ������Ʈ
        }

        // RŰ�� ���� ���� (������)
        // ������ �Ѿ��� 2�� �̸��̰� �÷��̾� �ָӴϿ� �Ѿ��� �־�� ��.
        if (Input.GetKeyDown(KeyCode.R) && loadedBullets < maxLoadedBullets && currentBullets > 0 && !boardManager.isWhiteTurn)
        {
            // �������� �ߴٸ� �� �ѱ��
            boardManager.isWhiteTurn = true;
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
        // �Ѿ� �߻�
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
        // ź�� ����
        GameObject intantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = intantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-2, -1) + Vector3.up * Random.Range(2, 3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
        Destroy(intantCase, 3f);
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
        CSoundManager.Instance.PlaySfx(4);
        // ���� ������ �ִ� �Ѿ� �߿��� ���ǿ� ������ �Ѿ� �� ���
        int bulletsToLoad = Mathf.Min(maxLoadedBullets - loadedBullets, currentBullets);
        loadedBullets += bulletsToLoad;
        currentBullets -= bulletsToLoad;

        UpdateBulletUI();
        StartCoroutine(ReloadMotion());
    }

    private IEnumerator ReloadMotion()
    {
        shotgun.SetActive(false);
        shotgunReload.SetActive(true);
        yield return new WaitForSeconds(1f);
        shotgun.SetActive(true);
        shotgunReload.SetActive(false);
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
        if (loadedBullets == maxLoadedBullets && currentBullets < maxBullets)
        {
            currentBullets++;
        }
        else if (loadedBullets < maxLoadedBullets && currentBullets > 0)
        {
            Reload();
        }
        else if (loadedBullets < maxLoadedBullets && currentBullets == 0)
        {
            currentBullets++;
        }
        UpdateBulletUI();
    }

    // ���� �Ͻ� ����
    public void PauseGameButton()
    {
        optionMenuUI.SetActive(true); // �ɼ� �޴� Ȱ��ȭ
        Time.timeScale = 0f; // ���� �Ͻ� ����
        isPaused = true;
        Cursor.lockState = CursorLockMode.None; // ���콺 Ŀ�� Ȱ��ȭ
    }

    // ���� �簳
    public void ResumeGameButton()
    {
        CSoundManager.Instance.PlaySfx(3);
        optionMenuUI.SetActive(false); // �ɼ� �޴� ��Ȱ��ȭ
        Time.timeScale = 1f; // ���� �簳
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked; // ���콺 Ŀ�� ��Ȱ��ȭ
    }

    // ���� ����
    public void BackMainMenuButton()
    {
        CSoundManager.Instance.PlaySfx(2);
        optionMenuUI.SetActive(false); // �ɼ� �޴� ��Ȱ��ȭ
        Time.timeScale = 1f; // ���� �簳
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked; // ���콺 Ŀ�� ��Ȱ��ȭ
        SceneManager.LoadScene("TitleScene");
    }
}