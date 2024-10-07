using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CKnight : CChessman
{
    #region ����
    public int health = 3;
    public int currentHealth;
    private Rigidbody rb;
    private bool isDead = false;
    public GameObject knightStatus;
    public GameObject chessHp;
    public GameObject heartPrefab;
    public GameObject emptyHeartPrefab;
    #endregion
    private void Awake()
    {
        damagePool = FindObjectOfType<CUIDamagePool>(); // ������ Ǯ ã��
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody ����
        currentHealth = health;
        if (knightStatus != null)
        {
            knightStatus.SetActive(false);
        }
        UpdateHealthUI();
        cameraTransView = FindObjectOfType<CCameraTransView>();
    }

    private void Update()
    {
        if (cameraTransView == null)
        {
            return;
        }
        if (!cameraTransView.isInTopView)
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
        CChessUIManager.instance.ShowUI(knightStatus);
    }

    private void OnMouseExit()
    {
        CChessUIManager.instance.HideUI(knightStatus);
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
        UpdateHealthUI();
        if (currentHealth <= 0)
        {
            Die(collision); // ü���� 0�̸� �״� ó��
        }
    }

    // ���� �� ���ư��� ����� �ı� ó��
    private void Die(Collision collision)
    {
        CSoundManager.Instance.PlaySfx(8);
        isDead = true; // �̹� ���� ���·� ǥ��
        rb.isKinematic = false; // ���� ȿ�� ����

        // �浹�� �Ѿ��� ����� �ӵ��� ������� ���ư��� ����
        Vector3 knockbackDirection = collision.relativeVelocity.normalized; // �Ѿ��� ���ƿ� ����
        rb.AddForce(knockbackDirection * 50f, ForceMode.Impulse); // ���� ���� ���ư��� ��

        Destroy(gameObject, 1.5f); // 1�� �� ������Ʈ �ı�
    }
    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];

        // UpLeft
        KnightMove(CurrentX - 1, CurrentY + 2, ref r);
        // UpRight
        KnightMove(CurrentX + 1, CurrentY + 2, ref r);

        // RightUp
        KnightMove(CurrentX + 2, CurrentY + 1, ref r);
        // RightDown
        KnightMove(CurrentX + 2, CurrentY - 1, ref r);

        // DownLeft
        KnightMove(CurrentX - 1, CurrentY - 2, ref r);
        // DownRight
        KnightMove(CurrentX + 1, CurrentY - 2, ref r);

        // LeftUp
        KnightMove(CurrentX - 2, CurrentY + 1, ref r);
        // LeftDown
        KnightMove(CurrentX - 2, CurrentY - 1, ref r);
        return r;
    }

    public void KnightMove(int x, int y, ref bool[,] r)
    {
        CChessman c;
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            c = CBoardManager.instance.Chessmans[x, y];
            if (c == null)
            {
                r[x, y] = true;
            }
            else if (isWhite != c.isWhite)
            {
                r[x, y] = true;
            }
        }
    }

}
