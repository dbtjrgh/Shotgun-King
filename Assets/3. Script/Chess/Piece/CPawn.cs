using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CPawn : CChessman
{
    #region ����
    public int health = 3;
    public int currentHealth;
    private Rigidbody rb;
    private bool isDead = false;
    public GameObject pawnStatus;
    public GameObject chessHp;
    public GameObject heartPrefab;
    public GameObject emptyHeartPrefab;
    private Animator animator;
    private float timer;
    private float idleSwitchTime = 3.3f; // �ִϸ��̼� ��ȯ �ֱ�
    private int currentIdleIndex = -1;
    #endregion
    private void Awake()
    {
        damagePool = FindObjectOfType<CUIDamagePool>(); // ������ Ǯ ã��
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody ����
        currentHealth = health;
        if (pawnStatus != null)
        {
            pawnStatus.SetActive(false);
        }
        UpdateHealthUI();
        cameraTransView = FindObjectOfType<CCameraTransView>();
        timer = idleSwitchTime; // Ÿ�̸� �ʱ�ȭ
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
        timer -= Time.deltaTime; // Ÿ�̸� ����

        if (timer <= 0)
        {
            PlayRandomIdleAnimation();
            timer = idleSwitchTime; // Ÿ�̸� ����
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

    private void PlayRandomIdleAnimation()
    {
        // 0���� 3������ ���� �ε��� ���� (Idle_1 ~ Idle_4)
        int newIdleIndex = Random.Range(0, 4);

        // ���� �ִϸ��̼��� �������� ������� �ʵ��� Ȯ��
        if (newIdleIndex == currentIdleIndex)
        {
            newIdleIndex = (newIdleIndex + 1) % 4; // ���� �ε����� ���� ��� ���� �ε����� ����
        }

        currentIdleIndex = newIdleIndex;

        // Animator�� �Ķ���� ����
        animator.SetTrigger($"Idle_{currentIdleIndex + 1}"); // Idle_1�� 0�̹Ƿ� +1
    }
    private void OnMouseEnter()
    {
        CChessUIManager.instance.ShowUI(pawnStatus);
    }
    private void OnMouseExit()
    {
        CChessUIManager.instance.HideUI(pawnStatus);
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
        CChessman c, c2;
        int i, j;
        int[] e = CBoardManager.instance.EnPassantMove;

        i = CurrentX;
        j = CurrentY;

        // White team move
        if (isWhite)
        {
            // Diagonal Left
            if (i != 0 && j != 7)
            {
                if (e[0] == i - 1 && e[1] == j + 1)
                {
                    r[i - 1, j + 1] = true;
                }
                c = CBoardManager.instance.Chessmans[i - 1, j + 1];
                if (c != null && !c.isWhite)
                {
                    r[i - 1, j + 1] = true;
                }
            }

            // Diagonal Right
            if (i != 7 && j != 7)
            {
                if (e[0] == i + 1 && e[1] == j + 1)
                {
                    r[i - 1, j + 1] = true;
                }
                c = CBoardManager.instance.Chessmans[i + 1, j + 1];
                if (c != null && !c.isWhite)
                {
                    r[i + 1, j + 1] = true;
                }
            }

            // Middle
            if (j != 7)
            {
                c = CBoardManager.instance.Chessmans[i, j + 1];
                if (c == null)
                {
                    r[i, j + 1] = true;
                }
            }

            // Middle on first move
            if (j == 1)
            {
                c = CBoardManager.instance.Chessmans[i, j + 1];
                c2 = CBoardManager.instance.Chessmans[i, j + 2];
                if (c == null && c2 == null)
                {
                    r[i, j + 2] = true;
                }
            }
        }
        else
        {
            // Diagonal Left
            if (i != 0 && j != 0)
            {
                if (e[0] == i - 1 && e[1] == j - 1)
                {
                    r[i - 1, j - 1] = true;
                }
                c = CBoardManager.instance.Chessmans[i - 1, j - 1];
                if (c != null && c.isWhite)
                {
                    r[i - 1, j - 1] = true;
                }
            }

            // Diagonal Right
            if (i != 7 && j != 0)
            {
                if (e[0] == i + 1 && e[1] == j - 1)
                {
                    r[i + 1, j - 1] = true;
                }
                c = CBoardManager.instance.Chessmans[i + 1, j - 1];
                if (c != null && c.isWhite)
                {
                    r[i + 1, j - 1] = true;
                }
            }

            // Middle
            if (j != 0)
            {
                c = CBoardManager.instance.Chessmans[i, j - 1];
                if (c == null)
                {
                    r[i, j - 1] = true;
                }
            }

            // Middle on first move
            if (j == 6)
            {
                c = CBoardManager.instance.Chessmans[i, j - 1];
                c2 = CBoardManager.instance.Chessmans[i, j - 2];
                if (c == null && c2 == null)
                {
                    r[i, j - 2] = true;
                }
            }
        }


        return r;
    }
}
