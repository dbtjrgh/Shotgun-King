using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CPawn : CChessman
{
    #region 변수
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
    private float idleSwitchTime = 3.3f; // 애니메이션 전환 주기
    private int currentIdleIndex = -1;
    #endregion
    private void Awake()
    {
        damagePool = FindObjectOfType<CUIDamagePool>(); // 데미지 풀 찾기
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody 참조
        currentHealth = health;
        if (pawnStatus != null)
        {
            pawnStatus.SetActive(false);
        }
        UpdateHealthUI();
        cameraTransView = FindObjectOfType<CCameraTransView>();
        timer = idleSwitchTime; // 타이머 초기화
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
            targetPosition.y = transform.position.y;  // y축은 고정된 상태로 LookAt 적용

            transform.LookAt(targetPosition);
        }
        timer -= Time.deltaTime; // 타이머 감소

        if (timer <= 0)
        {
            PlayRandomIdleAnimation();
            timer = idleSwitchTime; // 타이머 리셋
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
        // 0부터 3까지의 랜덤 인덱스 생성 (Idle_1 ~ Idle_4)
        int newIdleIndex = Random.Range(0, 4);

        // 같은 애니메이션이 연속으로 실행되지 않도록 확인
        if (newIdleIndex == currentIdleIndex)
        {
            newIdleIndex = (newIdleIndex + 1) % 4; // 같은 인덱스가 나올 경우 다음 인덱스로 변경
        }

        currentIdleIndex = newIdleIndex;

        // Animator의 파라미터 설정
        animator.SetTrigger($"Idle_{currentIdleIndex + 1}"); // Idle_1은 0이므로 +1
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
        // Projectile 레이어에 속한 오브젝트와 충돌했을 때
        if (collision.gameObject.layer == LayerMask.NameToLayer("Projectile") && !isDead)
        {
            TakeDamage(collision); // 데미지 처리 함수 호출
        }
    }
    // 데미지 처리
    private void TakeDamage(Collision collision)
    {
        GameObject damageUI = damagePool.GetObject();
        if (damagePool != null)
        {
            CUIDamageText damageText = damageUI.GetComponent<CUIDamageText>();
            damageText.Initialize(transform, Vector3.up, damagePool);
        }
        currentHealth--; // 체력 1 감소
        UpdateHealthUI();
        if (currentHealth <= 0)
        {
            Die(collision); // 체력이 0이면 죽는 처리
        }
    }

    // 죽을 때 날아가는 연출과 파괴 처리
    private void Die(Collision collision)
    {
        CSoundManager.Instance.PlaySfx(8);
        isDead = true; // 이미 죽은 상태로 표시
        rb.isKinematic = false; // 물리 효과 적용

        // 충돌한 총알의 방향과 속도를 기반으로 날아가는 연출
        Vector3 knockbackDirection = collision.relativeVelocity.normalized; // 총알이 날아온 방향
        rb.AddForce(knockbackDirection * 50f, ForceMode.Impulse); // 힘을 가해 날아가게 함

        Destroy(gameObject, 1.5f); // 1초 후 오브젝트 파괴
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
