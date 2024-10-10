using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CKing : CChessman
{
    #region ����
    public int health = 8; // ŷ�� ü��
    public int currentHealth; // ŷ�� ���� ü��
    private Rigidbody rb; // ŷ�� Rigidbody ����
    private bool isDead = false;
    public GameObject kingStatus;
    public GameObject chessHp;
    public GameObject heartPrefab;
    public GameObject emptyHeartPrefab;
    private CBoardManager boardManager;
    #endregion


    private void Awake()
    {
        cameraTransView = FindObjectOfType<CCameraTransView>();
        damagePool = FindObjectOfType<CUIDamagePool>(); // ������ Ǯ ã��
        boardManager = FindObjectOfType<CBoardManager>();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody ����
        currentHealth = health;
        if (kingStatus != null)
        {
            kingStatus.SetActive(false);
        }
        if (isWhite)
        {
            UpdateHealthUI();
        }
    }

    private void Update()
    {
        if (cameraTransView == null)
        {
            cameraTransView = FindObjectOfType<CCameraTransView>();
        }
        else if (!cameraTransView.isInTopView && cameraTransView.playerCinemachine != null && isWhite)
        {
            Vector3 targetPosition = cameraTransView.playerCinemachine.transform.position;
            targetPosition.y = transform.position.y;  // y���� ������ ���·� LookAt ����

            transform.LookAt(targetPosition);
        }
    }
    private void UpdateHealthUI()
    {
        foreach (Transform child in chessHp.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < currentHealth; i++)
        {
            Instantiate(heartPrefab, chessHp.transform);
        }

        for (int i = 0; i < (health - currentHealth); i++)
        {
            Instantiate(emptyHeartPrefab, chessHp.transform);
        }
    }
    private void OnMouseEnter()
    {
        if (isWhite)
        {
            CChessUIManager.instance.ShowUI(kingStatus);
        }
    }

    private void OnMouseExit()
    {
        if (isWhite)
        {
            CChessUIManager.instance.HideUI(kingStatus);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Projectile ���̾ ���� ������Ʈ�� �浹���� ��
        if (collision.gameObject.layer == LayerMask.NameToLayer("Projectile") && !isDead)
        {
            TakeDamage(collision); // ������ ó�� �Լ� ȣ��
        }
    }

    // ������ ó��
    private void TakeDamage(Collision collision)
    {
        GameObject damageUI = damagePool.GetObject();
        if (damagePool != null)
        {
            CUIDamageText damageText = damageUI.GetComponent<CUIDamageText>();
            damageText.Initialize(transform, Vector3.up, damagePool);
        }
        currentHealth--; // ü�� 1 ����
        if (isWhite)
        {
            UpdateHealthUI();
        }
        if (currentHealth <= 0)
        {
            StartCoroutine(Die(collision)); // ü���� 0�̸� �״� ó��
        }
    }

    // ���� �� ���ư��� ����� �ı� ó��
    private IEnumerator Die(Collision collision)
    {
        CSoundManager.Instance.PlaySfx(8);
        // ��� ŷ�� �׾��ٸ� ���� ���������� ����
        isDead = true; // �̹� ���� ���·� ǥ��
        rb.isKinematic = false; // ���� ȿ�� ����

        // ������ �浹 �������� ���ư��� ����
        Vector3 knockbackDirection = collision.relativeVelocity.normalized; // �Ѿ��� ���ƿ� ����
        rb.AddForce(knockbackDirection * 50f, ForceMode.Impulse); // ���� ���� ���ư��� ��

        yield return new WaitForSeconds(2f); // 5�� ��� ��
        if (boardManager.isTutorial)
        {
            SceneManager.LoadScene("OpeningScene");
        }
        else
        {
            boardManager.stageFloor += 1;
        }
        if (isWhite)
        {
            Destroy(gameObject);
            CBoardManager.instance.EndGame(); // ü���� 0�� �Ǹ� ���� ���� ȣ��
            if (boardManager.stageFloor == 1)
            {
                SceneManager.LoadScene("island1-2StageScene");
            }
            else if (boardManager.stageFloor == 3)
            {
                SceneManager.LoadScene("Castle3StageScene");
            }
            else if (boardManager.stageFloor == 4)
            {
                SceneManager.LoadScene("Castle4StageScene");
            }
            else if (boardManager.stageFloor == 5)
            {
                SceneManager.LoadScene("Valcano5StageScene");
            }
            else if (boardManager.stageFloor == 6)
            {
                SceneManager.LoadScene("EnddingScene");
            }

        }
    }

    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];

        CChessman c;
        int i, j;

        // Top Side
        i = CurrentX - 1;
        j = CurrentY + 1;
        if (CurrentY != 7)
        {
            for (int k = 0; k < 3; k++)
            {
                if (i != -1 && j != 8 && i != 8)
                {
                    c = CBoardManager.instance.chessMans[i, j];

                    if (c == null)
                    {
                        r[i, j] = true;
                    }
                    else if (isWhite != c.isWhite)
                    {
                        r[i, j] = true;
                    }
                }
                i++;
            }
        }

        // Down Side
        i = CurrentX - 1;
        j = CurrentY - 1;
        if (CurrentY != 0)
        {
            for (int k = 0; k < 3; k++)
            {
                if (i != -1 && j != -1 && i != 8)
                {
                    c = CBoardManager.instance.chessMans[i, j];
                    if (c == null)
                    {
                        r[i, j] = true;
                    }
                    else if (isWhite != c.isWhite)
                    {
                        r[i, j] = true;
                    }
                }
                i++;
            }
        }

        // Middle Left
        if (CurrentX - 1 != -1)
        {
            c = CBoardManager.instance.chessMans[CurrentX - 1, CurrentY];
            if (c == null)
            {
                r[CurrentX - 1, CurrentY] = true;
            }
            else if (isWhite != c.isWhite)
            {
                r[CurrentX - 1, CurrentY] = true;
            }
        }

        // Middle Right
        if (CurrentX + 1 != 8)
        {
            c = CBoardManager.instance.chessMans[CurrentX + 1, CurrentY];
            if (c == null)
            {
                r[CurrentX + 1, CurrentY] = true;
            }
            else if (isWhite != c.isWhite)
            {
                r[CurrentX + 1, CurrentY] = true;
            }
        }

        return r;
    }
}
