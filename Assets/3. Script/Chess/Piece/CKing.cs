using System.Collections;
using UnityEngine;

public class CKing : CChessman
{
    #region ����
    public int health = 8; // ŷ�� ü��
    public int currentHealth; // ŷ�� ���� ü��
    private Rigidbody rb; // ŷ�� Rigidbody ����
    private CCameraTransView cameraTransView;
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
        if(kingStatus != null)
        {
            kingStatus.SetActive(false);
        }
        if(isWhite)
        {
            UpdateHealthUI();
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
        if(isWhite)
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
        if(isWhite)
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
        // ��� ŷ�� �׾��ٸ� ���� ���������� ����


        isDead = true; // �̹� ���� ���·� ǥ��
        rb.isKinematic = false; // ���� ȿ�� ����

        // ������ �浹 �������� ���ư��� ����
        Vector3 knockbackDirection = collision.relativeVelocity.normalized; // �Ѿ��� ���ƿ� ����
        rb.AddForce(knockbackDirection * 50f, ForceMode.Impulse); // ���� ���� ���ư��� ��

        yield return new WaitForSeconds(5f); // 5�� ��� ��
        Destroy(gameObject, 1.5f);
        boardManager.stageFloor += 1;
        CBoardManager.instance.EndGame(); // ü���� 0�� �Ǹ� ���� ���� ȣ��
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
                    c = CBoardManager.instance.Chessmans[i, j];

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
                    c = CBoardManager.instance.Chessmans[i, j];
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
            c = CBoardManager.instance.Chessmans[CurrentX - 1, CurrentY];
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
            c = CBoardManager.instance.Chessmans[CurrentX + 1, CurrentY];
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
